using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using withersdk.Net.Client;
using withersdk.Net.General;
using withersdk.Net.General.Nodes;
using withersdk.Net.Methods.Wrapper;
using withersdk.Net.Methods;
using withersdk.Utils;
using TestApp.Pages;

namespace TestApp
{
    internal static class Client
    {
        public static ClientConnector Connector { get; private set; }
        public static RequestMethod.Ping LastPing { get; private set; }
        public static (double Request, double Response, double Total) LastPingValues { get; private set; }
        public static byte[] Token { get; private set; }
        public static void Initialize(ConnectArgs args)
        {
            Connector = new ClientConnector(args);
            Connector.ClientEvent += OnEvent;
        }

        private static void OnEvent(object sender, ClientEventArgs e)
        {
            if (e.Type == ClientEventType.Info)
            {
                
            }
            else if (e.Type == ClientEventType.Warn)
            {
                
            }
            else if (e.Type == ClientEventType.Error)
            {
                
            }
            else if (e.Type == ClientEventType.Update)
            {
                var getTime = DateTime.UtcNow;
                if (e.Message == SignedNode.TypeOf)
                {
                    var signedMessage = Connector.UnpackSignedNode(e);
                    MethodHandler(signedMessage.Content, getTime, true);
                }
                else if (e.Message == Node.TypeOf)
                {
                    var textMessage = Connector.UnpackNode(e);
                    MethodHandler(textMessage.Content, getTime, false);
                }
            }
        }

        private static bool MethodHandler(string method, DateTime getTime, bool signed = true)
        {
            if (method.GetMethodType() == ResponseMethod.Pong.TypeOf)
            {
                var pong = Method.Deserialize<ResponseMethod.Pong>(method);
                LastPingValues = LastPing.GetPingMilliseconds(pong, getTime);
            }
            else if (method.GetMethodType() == ResponseMethod.Authorization.TypeOf)
            {
                var auth = Method.Deserialize<ResponseMethod.Authorization>(method);
                if(auth.Allow && auth.Token != null)
                {
                    Token = auth.Token;
                    MainWindow.InvokeAct(()=> MainWindow.SetPage(new ChatPage()));
                }
                else
                {
                    MainWindow.InvokeAct(() =>
                    {
                        if (MainWindow.GetPage is SignUpPage)
                        {
                            var page = MainWindow.Instance.uiFrame.Content as SignUpPage;
                            page.Cancel("Такой аккаунт уже существует");
                        }
                        else if (MainWindow.GetPage is StartPage)
                        {
                            var page = MainWindow.Instance.uiFrame.Content as StartPage;
                            page.Cancel("Неправильный логин или пароль");
                        }
                    });
                }
            }
            else
                return false;
            return true;
        }

        public static void Ping()
        {
            LastPing = Connector.SendMethod(new RequestMethod.Ping
            {
                SendTime = DateTime.UtcNow,
            }) as RequestMethod.Ping;
        }
    }
}
