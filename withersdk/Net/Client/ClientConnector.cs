using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using withersdk.Net.General;
using withersdk.Net.General.Nodes;
using withersdk.Net.Methods;
using withersdk.OS;
using withersdk.Utils;

namespace withersdk.Net.Client
{
    public class ClientConnector : IDisposable
    {
        public event EventHandler<ClientEventArgs> ClientEvent;

        public event EventHandler<TunnelState> TunnelStateUpdate;
        public event EventHandler<bool> StateUpdate;
        private CancellationTokenSource _source { get; set; }
        private TcpClient _client { get; set; }

        public ClientDeviceInfo DeviceInfo { get; private set; }

        public bool State {
            get;
            private set;
        }
        private NetworkStream _stream { get; set; }
        private KeyContainer _keyContainer { get; set; }
        public KeyPair KeyPair { get => _keyContainer.GetKeyPair(); }
        public CancellationToken CancellationToken { get; private set; }
        public ConnectArgs ConnectArgs { get; set; }

        private TunnelState _tunnelState { get; set; }
        public TunnelState TunnelState {
            get => _tunnelState;
            private set
            {
                _tunnelState = value;
                TunnelStateUpdate?.Invoke(this, _tunnelState);
            }
        }

        public ClientConnector(ConnectArgs args)
        {
            ConnectArgs = args;
            _keyContainer = new KeyContainer();
            _tunnelState = TunnelState.None;
            State = false;
            DeviceInfo = ClientDeviceInfo.Build(ConnectArgs.AppName);
        }

        public void Connect()
        {
            if (_source == null)
            {
                _source = new CancellationTokenSource();
                CancellationToken = _source.Token;
                _client = new TcpClient();
                try
                {
                    _client.Connect(ConnectArgs.Address, ConnectArgs.Port);
                    _stream = _client.GetStream();
                    ClientEvent?.Invoke(this, new ClientEventArgs
                    {
                        Type = ClientEventType.Info,
                        Message = Language.CLIENT_START_CONNECT,
                    });
                    Thread receiveThread = new Thread(new ThreadStart(Receive));
                    receiveThread.Start();
                }
                catch (Exception ex)
                {
                    ClientEvent?.Invoke(this, new ClientEventArgs
                    {
                        Type = ClientEventType.Error,
                        Message = Language.CLIENT_START_CONNECT_EX,
                        Exception = ex
                    });
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            if (_source != null)
            {
                State = false;
                _source.Cancel();
                _source.Dispose();
                _source = null;
                if (_stream != null)
                    _stream.Close();
                if (_client != null)
                    _client.Close();
                ClientEvent?.Invoke(this, new ClientEventArgs
                {
                    Type = ClientEventType.Info,
                    Message = Language.CLIENT_STOP,
                });
                StateUpdate?.Invoke(this, State);
            }
        }

        private void Receive()
        {
            try
            {
                while (!_client.Connected);
                State = true;
                StateUpdate?.Invoke(this, State);
                MakeTunnel();
                ClientEvent?.Invoke(this, new ClientEventArgs
                {
                    Type = ClientEventType.Info,
                    Message = Language.CLIENT_BUFFER_READ_START,
                });
                while (!CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var update = ReadStream();
                        ClientEvent?.Invoke(this, new ClientEventArgs
                        {
                            Type = ClientEventType.Update,
                            Message = update.GetMessageType().ToString(),
                            Update = update
                        });
                    }
                    catch (Exception ex)
                    {
                        ClientEvent?.Invoke(this, new ClientEventArgs
                        {
                            Type = ClientEventType.Warn,
                            Message = Language.CLIENT_BUFFER_READ_EX,
                            Exception = ex
                        });
                        Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientEvent?.Invoke(this, new ClientEventArgs
                {
                    Type = ClientEventType.Error,
                    Message = Language.CLIENT_RECEIVE_EX,
                    Exception = ex
                });
                Disconnect();
            }
        }
        private void MakeTunnel()
        {
            ClientEvent?.Invoke(this, new ClientEventArgs
            {
                Type = ClientEventType.Info,
                Message = Language.CLIENT_TUNNEL_CREATE,
            });
            TunnelState = TunnelState.Hello_Prepare;
            byte[] pack;
            Handshake handshake = new Handshake();
            Send(new HandshakeNode(DeviceInfo.Serialize(), HandshakeStage.Hello)); //0 send hello
            TunnelState = TunnelState.Hello_Send;
            while ((pack = ReadStream()).GetMessageType() != HandshakeNode.TypeOf && !CancellationToken.IsCancellationRequested) ; //1 get server public ki
            TunnelState = TunnelState.Pack_Get_Peer;
            var hPeerSecret = HandshakeNode.Unpack(pack);
            TunnelState = TunnelState.Pack_Dec_Peer;
            if (hPeerSecret.Body.Length == 32 && hPeerSecret.Stage == HandshakeStage.PeerSecret)
            {
                ClientEvent?.Invoke(this, new ClientEventArgs
                {
                    Type = ClientEventType.Info,
                    Message = Language.CLIENT_TUNNEL_CREATE1,
                });
                handshake.CreateShared(hPeerSecret.Body);
                _keyContainer.Ki = handshake.SharedKey.ToArray();
                TunnelState = TunnelState.Pack_Peer_Prepare;
                Send(new HandshakeNode(handshake.PublicKey, HandshakeStage.PeerSecret)); //2 send public ki
                TunnelState = TunnelState.Pack_Peer_Send;
                handshake.Dispose();
                handshake = new Handshake();
                while ((pack = ReadStream()).GetMessageType() != HandshakeNode.TypeOf && !CancellationToken.IsCancellationRequested) ; //3 get server kmac
                TunnelState = TunnelState.Pack_Get_MAC;
                var hMacSecret = HandshakeNode.Unpack(pack);
                TunnelState = TunnelState.Pack_Dec_MAC;
                if (hMacSecret.Body.Length == 32 && hMacSecret.Stage == HandshakeStage.PeerMac)
                {
                    ClientEvent?.Invoke(this, new ClientEventArgs
                    {
                        Type = ClientEventType.Info,
                        Message = Language.CLIENT_TUNNEL_CREATE2,
                    });
                    handshake.CreateShared(hMacSecret.Body);
                    _keyContainer.KMAC = handshake.SharedKey.ToArray();
                    TunnelState = TunnelState.Pack_MAC_Prepare;
                    Send(new HandshakeNode(handshake.PublicKey, HandshakeStage.PeerMac)); //4 send public kmac
                    TunnelState = TunnelState.Pack_MAC_Send;
                    handshake.Dispose();
                    handshake = null;
                    while ((pack = ReadStream()).GetMessageType() != HandshakeNode.TypeOf && !CancellationToken.IsCancellationRequested) ; //5 get server finalize
                    TunnelState = TunnelState.Pack_Get_Check;
                    var hMac = HandshakeNode.Unpack(pack);
                    TunnelState = TunnelState.Pack_Dec_Check;
                    if ("Complete?".FromUTF8().CheckHMAC(_keyContainer.KMAC, hMac.Body) && hMac.Stage == HandshakeStage.Finalize)
                    {
                        ClientEvent?.Invoke(this, new ClientEventArgs
                        {
                            Type = ClientEventType.Info,
                            Message = Language.CLIENT_TUNNEL_CREATE3,
                        });
                        TunnelState = TunnelState.Pack_Check_Prepare;
                        Send(new HandshakeNode("Complete!".FromUTF8().ComputeHMAC(_keyContainer.KMAC), HandshakeStage.Finalize)); //6 send finalize
                        TunnelState = TunnelState.Pack_Check_Send;
                        while ((pack = ReadStream()).GetMessageType() != SignedNode.TypeOf && !CancellationToken.IsCancellationRequested) ; //7 get server status
                        TunnelState = TunnelState.Establish;
                        var response = SignedNode.Unpack(pack, KeyPair);
                        if(response.Content == "OK")
                        {
                            TunnelState = TunnelState.Connected;
                            ClientEvent?.Invoke(this, new ClientEventArgs
                            {
                                Type = ClientEventType.Info,
                                Message = Language.CLIENT_TUNNEL_COMPLETE,
                            });
                        }
                        else
                        {
                            ClientEvent?.Invoke(this, new ClientEventArgs
                            {
                                Type = ClientEventType.Warn,
                                Message = Language.CLIENT_TUNNEL_DENY,
                            });
                            Disconnect();
                        }
                    }
                    else
                    {
                        ClientEvent?.Invoke(this, new ClientEventArgs
                        {
                            Type = ClientEventType.Warn,
                            Message = Language.CLIENT_TUNNEL_DENY_MAC,
                        });
                        Disconnect();
                    }
                }
                else
                {
                    ClientEvent?.Invoke(this, new ClientEventArgs
                    {
                        Type = ClientEventType.Warn,
                        Message = Language.CLIENT_TUNNEL_DENY_PEER,
                    });
                    Disconnect();
                }
            }
            else
            {
                ClientEvent?.Invoke(this, new ClientEventArgs
                {
                    Type = ClientEventType.Warn,
                    Message = Language.CLIENT_TUNNEL_DENY_PEER,
                });
                Disconnect();
            }
        }
        private byte[] ReadStream()
        {

            var response = new List<byte>();
            do
            {
                response.Add((byte)_stream.ReadByte());
            }
            while (_stream.DataAvailable);

            ClientEvent?.Invoke(this, new ClientEventArgs
            {
                Type = ClientEventType.Info,
                Message = Language.CLIENT_GET_PACKET,
            });

            return response.ToArray();
        }

        public void Send(INode node)
        {
            try
            {
                if (_source != null)
                {
                    var pack = node.PackToStream();
                    _stream.Write(pack, 0, pack.Length);
                    ClientEvent?.Invoke(this, new ClientEventArgs
                    {
                        Type = ClientEventType.Info,
                        Message = Language.CLIENT_MSG_SEND,
                        Update = pack
                    });
                }
            }
            catch (Exception ex)
            {
                ClientEvent?.Invoke(this, new ClientEventArgs
                {
                    Type = ClientEventType.Error,
                    Message = Language.CLIENT_MSG_SEND_EX,
                    Exception = ex
                });
            }
        }

        public SignedNode UnpackSignedNode(ClientEventArgs e)
        {
            return SignedNode.Unpack(e.Update, KeyPair);
        }

        public void SendInline(string content, bool sign = true)
        {
            if (sign)
                Send(new SignedNode(content, KeyPair));
            else
                Send(new Node(content, KeyPair));
        }
        public Node UnpackNode(ClientEventArgs e)
        {
            return Node.Unpack(e.Update, KeyPair);
        }

        public Method SendMethod(Method method, bool sign = true)
        {
            Send(method.SerializeToMessage(KeyPair, sign));
            return method;
        }

        #region dispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~ClientConnector()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    public enum TunnelState
    {
        None = -1,
        Hello_Prepare,
        Hello_Send,
        Pack_Get_Peer,
        Pack_Dec_Peer,
        Pack_Peer_Prepare,
        Pack_Peer_Send,
        Pack_Get_MAC,
        Pack_Dec_MAC,
        Pack_MAC_Prepare,
        Pack_MAC_Send,
        Pack_Get_Check,
        Pack_Dec_Check,
        Pack_Check_Prepare,
        Pack_Check_Send,
        Establish,
        Connected
    }
}
