using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;
using withersdk.Net.General;
using withersdk.Net.General.Nodes;
using withersdk.Net.Methods;

namespace withersdk.Net.Server.Objects
{
    public class HostController
    {
        private CancellationTokenSource _source { get; set; }
        public CancellationToken CancellationToken { get; private set; }
        public Configurations Configurations {  get; }
        public ServerConnector Connector { get; }
        private static TcpListener _tcpListener;
        public List<Client> Clients { get; }

        public HostController(Configurations configurations, ServerConnector connector)
        {
            Connector = connector;
            Configurations = configurations;
            Clients = new List<Client>();
        }

        public void Listen()
        {
            if(_source == null)
            {
                _source = new CancellationTokenSource();
                CancellationToken = _source.Token;
                try
                {
                    _tcpListener = new TcpListener(IPAddress.Any, Configurations.Port);
                    _tcpListener.Start();

                    ServerConnector.CallEvent(this, new ServerEventArgs
                    {
                        Type = ServerEventType.Info,
                        Message = Language.START_SERVER
                    });

                    while (!CancellationToken.IsCancellationRequested)
                    {
                        TcpClient tcpClient = _tcpListener.AcceptTcpClient();
                        var clientObject = new Client(tcpClient, this);

                        ServerConnector.CallEvent(this, new ServerEventArgs
                        {
                            Type = ServerEventType.Info,
                            Message = Language.NEW_CONNECTION + $" {clientObject.Ip} [{clientObject.ConnectionId}]",
                        });

                        Thread clientThread = new Thread(new ThreadStart(clientObject.Receive));
                        clientThread.Start();
                    }
                    Close();
                }
                catch (Exception ex)
                {
                    ServerConnector.CallEvent(this, new ServerEventArgs
                    {
                        Type = ServerEventType.Error,
                        Message = Language.LISTENER_EX,
                        Exception = ex
                    });

                    Close();
                }
            }
        }

        public void Close()
        {
            if(_source != null)
            {
                _source.Cancel();
                _source.Dispose();
                _source = null;
                _tcpListener.Stop();
                for (int i = 0; i < Clients.Count; i++)
                {
                    Clients[i].Close();
                }
                Clients.Clear();
                ServerConnector.CallEvent(this, new ServerEventArgs
                {
                    Type = ServerEventType.Info,
                    Message = Language.CLOSE_SERVER
                });
            }
        }

        public void AddConnection(Client client)
        {
            Clients.Add(client);
        }

        public void RemoveConnection(string connectionId)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == connectionId);
            if (client != null)
            {
                client.Close();
                Clients.Remove(client);
            }
        }

        public void Send(INode node, string connectionId)
        {
            try
            {
                var pack = node.PackToStream();
                Client client = Clients.FirstOrDefault(c => c.ConnectionId == connectionId);
                if (client != null)
                {
                    client._stream.Write(pack, 0, pack.Length);

                    ServerConnector.CallEvent(this, new ServerEventArgs
                    {
                        Type = ServerEventType.Info,
                        Message = Language.SERVER_MSG_SEND + $" {client.Ip} [{client.ConnectionId}]",
                        Update = pack
                    });
                }
            }
            catch (Exception ex)
            {
                ServerConnector.CallEvent(this, new ServerEventArgs
                {
                    Type = ServerEventType.Warn,
                    Message = Language.SERVER_MSG_SEND_EX + $"[{connectionId}]",
                    Exception = ex
                });
            }
        }

        public SignedNode UnpackSignedNode(ServerEventArgs e)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == e.Secondary);
            if (client != null)
                return SignedNode.Unpack(e.Update, client.KeyPair);
            else return null;
        }

        public void SendInline(string content, string connectionId, bool sign = true)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == connectionId);
            if (client != null)
                if(sign)
                    Send(new SignedNode(content, client.KeyPair), connectionId);
                else
                    Send(new Node(content, client.KeyPair), connectionId);
        }
        public void SendInline(string content, ServerEventArgs e, bool sign = true)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == e.Secondary);
            if (client != null)
                if (sign)
                    Send(new SignedNode(content, client.KeyPair), e.Secondary);
                else
                    Send(new Node(content, client.KeyPair), e.Secondary);
        }

        public Method SendMethod(Method method, string connectionId, bool sign = true)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == connectionId);
            if (client != null)
                Send(method.SerializeToMessage(client.KeyPair, sign), connectionId);
            return method;
        }
        public Method SendMethod(Method method, ServerEventArgs e, bool sign = true)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == e.Secondary);
            if (client != null)
                Send(method.SerializeToMessage(client.KeyPair, sign), e.Secondary);
            return method;
        }

        public Node UnpackNode(ServerEventArgs e)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == e.Secondary);
            if (client != null)
                return Node.Unpack(e.Update, client.KeyPair);
            else return null;
        }

        public byte[] CreateToken(ServerEventArgs e, string password)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == e.Secondary);
            if (client != null)
            {
                return client.CreateToken(password);
            }
            else return null;
        }

        public bool VerifyToken(ServerEventArgs e, byte[] token, string password)
        {
            Client client = Clients.FirstOrDefault(c => c.ConnectionId == e.Secondary);
            if (client != null)
            {
                return client.VerifyToken(token, password);
            }
            else return false;
        }
    }
}
