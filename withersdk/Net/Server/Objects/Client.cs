using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using withersdk.Net.General;
using withersdk.Net.General.Nodes;
using withersdk.Utils;

namespace withersdk.Net.Server.Objects
{
    public class Client
    {
        private TcpClient _client;
        public string Ip
        {
            get
            {
                IPEndPoint ipep = (IPEndPoint)_client.Client.RemoteEndPoint;
                IPAddress ipa = ipep.Address;
                return ipa.ToString();
            }
        }
        public string ConnectionId { get; private set; }
        internal NetworkStream _stream { get; set; }
        public HostController Controller { get; private set; }

        public ClientDeviceInfo DeviceInfo { get; private set; }
        private KeyContainer _keyContainer {  get; set; }
        public KeyPair KeyPair { get => _keyContainer.GetKeyPair(); }
        public Client(TcpClient tcpClient, HostController controller)
        {
            _keyContainer = new KeyContainer();
            var time = DateTime.UtcNow.Ticks;
            Console.WriteLine(time);
            var key = Guid.NewGuid().ToString();
            ConnectionId = Convert.ToBase64String(Encoding.UTF8.GetBytes(key + time));
            _client = tcpClient;
            Controller = controller;
            Controller.AddConnection(this);
        }

        public void Receive()
        {
            try
            {
                _stream = _client.GetStream();
                MakeTunnel();
                ServerConnector.CallEvent(Controller, new ServerEventArgs
                {
                    Type = ServerEventType.Info,
                    Message = Language.SERVER_BUFFER_READ_START + $" {Ip} [{ConnectionId}]"
                });
                while (!Controller.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var update = ReadStream();
                        ServerConnector.CallEvent(Controller, new ServerEventArgs
                        {
                            Type = ServerEventType.Update,
                            Message = update.GetMessageType().ToString(),
                            Update = update,
                            Secondary = ConnectionId
                        });
                    }
                    catch (Exception ex)
                    {
                        ServerConnector.CallEvent(Controller, new ServerEventArgs
                        {
                            Type = ServerEventType.Error,
                            Message = Language.READ_BUFFER_EX + $" {Ip} [{ConnectionId}]"
                        });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ServerConnector.CallEvent(Controller, new ServerEventArgs
                {
                    Type = ServerEventType.Error,
                    Message = Language.LISTENER_CLIENT_EX + $" {Ip} [{ConnectionId}]"
                });
            }
            finally
            {
                CloseInline();
            }
        }

        private void MakeTunnel()
        {
            ServerConnector.CallEvent(Controller, new ServerEventArgs
            {
                Type = ServerEventType.Info,
                Message = Language.SERVER_TUNNEL_CREATE + $" {Ip} [{ConnectionId}]"
            });
            byte[] pack;
            Handshake handshake = new Handshake();
            while ((pack = ReadStream()).GetMessageType() != HandshakeNode.TypeOf && !Controller.CancellationToken.IsCancellationRequested); //0 get hello
            var hHello = HandshakeNode.Unpack(pack);
            if (hHello.Stage == HandshakeStage.Hello)
            {
                DeviceInfo = ClientDeviceInfo.Deserialize(hHello.Body);
                ServerConnector.CallEvent(Controller, new ServerEventArgs
                {
                    Type = ServerEventType.Info,
                    Message = Language.SERVER_TUNNEL_CREATE1 + $" {Ip} [{ConnectionId}]\n" +
                    $"Hardware ID:\n" +
                    $"OS: \t{DeviceInfo.OS}\n" +
                    $"Version: \t{DeviceInfo.OSVersion}\n" +
                    $"Architecture: \t{DeviceInfo.Architecture}\n" +
                    $"Machine name: \t{DeviceInfo.MachineName}\n" +
                    $"Machine user: \t{DeviceInfo.UserName}\n" +
                    $"Type: \t{DeviceInfo.Type}\n" +
                    $"App name: \t{DeviceInfo.AppName}\n" +
                    $"Runtime: \t{DeviceInfo.Runtime}\n" +
                    $"CPU's: \t{DeviceInfo.CPU}\n" +
                    $"GPU's: \t{DeviceInfo.GPU}\n" +
                    $"RAM: \t{DeviceInfo.RAM}\n" +
                    $"Is mono: \t{DeviceInfo.IsMono}\n"
                });
                Send(new HandshakeNode(handshake.PublicKey, HandshakeStage.PeerSecret)); //1 send server public ki
                while ((pack = ReadStream()).GetMessageType() != HandshakeNode.TypeOf && !Controller.CancellationToken.IsCancellationRequested) ; //2 get public ki
                var hPeerSecret = HandshakeNode.Unpack(pack);
                if (hPeerSecret.Body.Length == 32 && hPeerSecret.Stage == HandshakeStage.PeerSecret)
                {
                    ServerConnector.CallEvent(Controller, new ServerEventArgs
                    {
                        Type = ServerEventType.Info,
                        Message = Language.SERVER_TUNNEL_CREATE2 + $" {Ip} [{ConnectionId}]"
                    });
                    handshake.CreateShared(hPeerSecret.Body);
                    _keyContainer.Ki = handshake.SharedKey.ToArray();
                    handshake.Dispose();
                    handshake = new Handshake();
                    Send(new HandshakeNode(handshake.PublicKey, HandshakeStage.PeerMac)); //3 send server kmac
                    while ((pack = ReadStream()).GetMessageType() != HandshakeNode.TypeOf && !Controller.CancellationToken.IsCancellationRequested) ; //4 get public kmac
                    var hMacSecret = HandshakeNode.Unpack(pack);
                    if (hMacSecret.Body.Length == 32 && hMacSecret.Stage == HandshakeStage.PeerMac)
                    {
                        ServerConnector.CallEvent(Controller, new ServerEventArgs
                        {
                            Type = ServerEventType.Info,
                            Message = Language.SERVER_TUNNEL_CREATE3 + $" {Ip} [{ConnectionId}]"
                        });
                        handshake.CreateShared(hMacSecret.Body);
                        _keyContainer.KMAC = handshake.SharedKey.ToArray();
                        handshake.Dispose();
                        handshake = null;
                        Send(new HandshakeNode("Complete?".FromUTF8().ComputeHMAC(_keyContainer.KMAC), HandshakeStage.Finalize)); //5 send server finalize
                        while ((pack = ReadStream()).GetMessageType() != HandshakeNode.TypeOf && !Controller.CancellationToken.IsCancellationRequested) ; //6 get finalize
                        var hMac = HandshakeNode.Unpack(pack);
                        if ("Complete!".FromUTF8().CheckHMAC(_keyContainer.KMAC, hMac.Body) && hMac.Stage == HandshakeStage.Finalize)
                        {
                            Send(new SignedNode("OK", KeyPair)); //7 send OK status
                            ServerConnector.CallEvent(Controller, new ServerEventArgs
                            {
                                Type = ServerEventType.Info,
                                Message = Language.SERVER_TUNNEL_COMPLETE + $" {Ip} [{ConnectionId}]"
                            });
                        }
                        else
                        {
                            ServerConnector.CallEvent(Controller, new ServerEventArgs
                            {
                                Type = ServerEventType.Warn,
                                Message = Language.SERVER_TUNNEL_DENY_MAC + $" {Ip} [{ConnectionId}]"
                            });
                            CloseInline();
                        }
                    }
                    else
                    {
                        ServerConnector.CallEvent(Controller, new ServerEventArgs
                        {
                            Type = ServerEventType.Warn,
                            Message = Language.SERVER_TUNNEL_DENY_PEER + $" {Ip} [{ConnectionId}]"
                        });
                        CloseInline();
                    }
                }
                else
                {
                    ServerConnector.CallEvent(Controller, new ServerEventArgs
                    {
                        Type = ServerEventType.Warn,
                        Message = Language.SERVER_TUNNEL_DENY_PEER + $" {Ip} [{ConnectionId}]"
                    });
                    CloseInline();
                }
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

            ServerConnector.CallEvent(Controller, new ServerEventArgs
            {
                Type = ServerEventType.Info,
                Message = Language.SERVER_GET_PACKET + $" {Ip} [{ConnectionId}]"
            });

            return response.ToArray();
        }
        private void Send(INode message)
        {
            Controller.Send(message, ConnectionId);
        }
        public void Close()
        {
            ServerConnector.CallEvent(Controller, new ServerEventArgs
            {
                Type = ServerEventType.Info,
                Message = Language.CLOSE_CLIENT + $" {Ip} [{ConnectionId}]"
            });
            if (_stream != null)
                _stream.Close();
            if (_client != null)
                _client.Close();
        }

        public void CloseInline()
        {
            Controller.RemoveConnection(ConnectionId);
        }
    }
}
