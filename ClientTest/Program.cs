using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using withersdk.Net.Client;
using withersdk.Net.General;
using withersdk.Net.General.Nodes;
using withersdk.Net.Methods;
using withersdk.Net.Methods.Wrapper;
using withersdk.Utils;

namespace ClientTest
{
    internal class Program
    {

        static ClientConnector connector;
        static RequestMethod.Ping Ping;
        static void Main(string[] args)
        {

            connector = new ClientConnector(new ConnectArgs
            {
                Address = "127.0.0.1",
                Port = 5553
            });
            connector.ClientEvent += OnEvent;
            Tweak.ReadAfterWriteLine("нажмите Enter для подключения");
            connector.Connect();
            while (true)
            {
                var msg = Console.ReadLine();
                if (msg == "ping")
                {
                    Ping = connector.SendMethod(new RequestMethod.Ping
                    {
                        SendTime = DateTime.UtcNow,
                    }) as RequestMethod.Ping;
                }
                if (msg == "sping" || msg == "signedping")
                {
                    Ping = connector.SendMethod(new RequestMethod.Ping
                    {
                        SendTime = DateTime.UtcNow,
                    }, true) as RequestMethod.Ping;
                }
            }
        }

        private static void OnEvent(object sender, ClientEventArgs e)
        {
            if(e.Type == ClientEventType.Info)
            {
                Tweak.WriteLine($"~c~{DateTime.Now.ToString("F")} [INFO]:" + e.Message);
            }
            else if (e.Type == ClientEventType.Warn)
            {
                Tweak.WriteLine($"~y~{DateTime.Now.ToString("F")} [WARN]:" + e.Message);
            }
            else if (e.Type == ClientEventType.Error)
            {
                Tweak.WriteLine($"~r~{DateTime.Now.ToString("F")} [ERROR]:" + e.Message + "\n" + e.Exception.ToString());
            }
            else if (e.Type == ClientEventType.Update)
            {
                var getTime = DateTime.UtcNow;
                Tweak.WriteLine($"~b~{DateTime.Now.ToString("F")} [UPDATE]:" + e.Message);
                if (e.Message == SignedNode.TypeOf)
                {
                    var signedMessage = connector.UnpackSignedNode(e);
                    Console.WriteLine(signedMessage.ToConsoleOutputString());
                    if (signedMessage.Content.GetMethodType() == ResponseMethod.Pong.TypeOf) //signpong get
                    {
                        var pong = Method.Deserialize<ResponseMethod.Pong>(signedMessage.Content);
                        var ping = Ping.GetPingMilliseconds(pong, getTime);
                        Console.WriteLine($"Signed ping state:\n\tRequest: {ping.RequestMs}ms\n\tResponse: {ping.ResponseMs}ms\n\tTotal: {ping.TotalMs}\n\tHost message: {pong.Message}");
                    }
                }
                else if (e.Message == Node.TypeOf)
                {
                    var textMessage = connector.UnpackNode(e);
                    Console.WriteLine(textMessage.ToConsoleOutputString());
                    if (textMessage.Content.GetMethodType() == ResponseMethod.Pong.TypeOf) //pong get
                    {
                        var pong = Method.Deserialize<ResponseMethod.Pong>(textMessage.Content);
                        var ping = Ping.GetPingMilliseconds(pong, getTime);
                        Console.WriteLine($"Ping state:\n\tRequest: {ping.RequestMs}ms\n\tResponse: {ping.ResponseMs}ms\n\tTotal: {ping.TotalMs}\n\tHost message: {pong.Message}");
                    }
                }
            }
        }
    }
}
