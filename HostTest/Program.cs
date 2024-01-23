using HostTest.Tables;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using withersdk.Net.Client;
using withersdk.Net.General;
using withersdk.Net.General.Nodes;
using withersdk.Net.Methods;
using withersdk.Net.Methods.Wrapper;
using withersdk.Net.Server;
using withersdk.Net.Server.Objects;
using withersdk.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static withersdk.Net.Methods.Wrapper.RequestMethod;

namespace HostTest
{
    internal class Program
    {
        static ServerConnector connector;
        static readonly string Version = "0.1.1";
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Tweak.WriteLine(
                 "     ~r~ ___        ~y~ ___   _           _      ~dge~ _   _            _  \n" +
                @"     ~r~|  _ \      ~y~(  _ \( )         ( )_    ~dge~( ) ( )          ( )_ " + "\n" +
                 "     ~r~| (_) )  __ ~y~| ( (_) |__    _ _|  _)   ~dge~| |_| |  _    ___|  _)\n" +
                @"     ~r~|    / / __ \~y~ |  _|  _  \/ _  ) |     ~dge~|  _  |/ _ \/  __) |  " + "\n" +
                @"     ~r~| |\ \(  ___/~y~ (_( ) | | | (_| | |_    ~dge~| | | | (_) )__  \ |_" + "\n" +
                @"     ~r~(_) (_)\____)~y~____/(_) (_)\__ _)\__)   ~dge~(_) (_)\___/(____/\__)");
            Tweak.Line();
            Tweak.WriteLine($"\t~y~ReChat Host ~b~v{Version}~y~ by ~dge~WitherBit ~y~[~b~https://github.com/witherbit~y~]");
            Tweak.Line();
            Db.Initialize();
            ServerConnector.ServerEvent += OnEvent;
            connector = new ServerConnector();
            connector.Start(new Configurations
            {
                Port = 5553
            });
            while (true) ;
        }

        private static void OnEvent(object sender, ServerEventArgs e)
        {
            var server = sender as HostController;
            if (e.Type == ServerEventType.Info)
            {
                Tweak.WriteLine($"~c~{DateTime.Now.ToString("G")} [INFO]:" + e.Message);
            }
            else if (e.Type == ServerEventType.Warn)
            {
                Tweak.WriteLine($"~y~{DateTime.Now.ToString("G")} [WARN]:" + e.Message);
            }
            else if (e.Type == ServerEventType.Error)
            {
                Tweak.WriteLine($"~r~{DateTime.Now.ToString("G")} [ERROR]:" + e.Message);
            }
            else if (e.Type == ServerEventType.Update)
            {
                var getTime = DateTime.UtcNow;
                Tweak.WriteLine($"~b~{DateTime.Now.ToString("G")} [UPDATE]:" + e.Message);
                bool handled = false;
                if (e.Message == SignedNode.TypeOf.ToString())
                {
                    var signedMessage = server.UnpackSignedNode(e);
                    Console.WriteLine(signedMessage.ToConsoleOutputString());
                    handled = MethodHandler(signedMessage.Content, e, getTime, true);
                }
                else if (e.Message == Node.TypeOf.ToString())
                {
                    var textMessage = server.UnpackNode(e);
                    Console.WriteLine(textMessage.ToConsoleOutputString());
                    handled = MethodHandler(textMessage.Content, e, getTime, false);
                }
                if (handled)
                    Tweak.WriteLine($"~dge~{DateTime.Now.ToString("G")} [METHOD]: Метод был обработан");
                else
                    Tweak.WriteLine($"~dge~{DateTime.Now.ToString("G")} [METHOD]: Метод не был обработан");
            }
        }

        private static bool MethodHandler(string method, ServerEventArgs e, DateTime getTime, bool sign = true)
        {
            if(method.GetMethodType() == RequestMethod.Ping.TypeOf)
            {
                connector.Controller.SendMethod(new ResponseMethod.Pong
                {
                    GetTime = getTime,
                    SendTime = DateTime.UtcNow
                }, e, sign);
            }
            else if (method.GetMethodType() == RequestMethod.SignUp.TypeOf)
            {
                var signUp = Method.Deserialize<SignUp>(method);
                var user = signUp.Login.GetUserByLogin();
                if (user != null)
                {
                    connector.Controller.SendMethod(new ResponseMethod.Authorization
                    {
                        Allow = false,
                        IsExist = true,
                    }, e, sign);
                }
                else
                {
                    var password = signUp.Password.Rfc2898DeriveInline();
                    new Users
                    {
                        Login = signUp.Login,
                        Password = password,
                        Name = signUp.Name,
                        Email = signUp.Email,
                        Time = DateTime.UtcNow
                    }.InsertUser();
                    connector.Controller.SendMethod(new ResponseMethod.Authorization
                    {
                        Allow = true,
                        IsExist = false,
                        Token = connector.Controller.CreateToken(e, signUp.Password)
                    }, e, sign);
                }
            }
            else if (method.GetMethodType() == RequestMethod.SignIn.TypeOf)
            {
                var signIn = Method.Deserialize<SignIn>(method);
                var user = signIn.Login.GetUserByLogin();
                if (user != null)
                {
                    if (user.Password.Rfc2898VerifyInline(signIn.Password))
                    {
                        Tweak.WriteLine($"~dge~{DateTime.Now.ToString("G")} [AUTH]: from Id {user.Id}\n\tName: {user.Name}\n\tLogin: {user.Login}\n\tPassword: {user.Password}\n\tEmail: {user.Email}\n\tTime: {user.Time.ToString("G")}");
                        connector.Controller.SendMethod(new ResponseMethod.Authorization
                        {
                            Allow = true,
                            IsExist = true,
                            Token = connector.Controller.CreateToken(e, signIn.Password)
                        }, e, sign);
                    }
                    else
                    {
                        connector.Controller.SendMethod(new ResponseMethod.Authorization
                        {
                            Allow = false,
                            IsExist = false,
                        }, e, sign);
                    }
                }
                else
                {
                    connector.Controller.SendMethod(new ResponseMethod.Authorization
                    {
                        Allow = false,
                        IsExist = false,
                    }, e, sign);
                }
            }
            else
                return false;
            return true;
        }
    }
}
