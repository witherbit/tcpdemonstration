using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using withersdk.Net.General;
using withersdk.Net.Server.Objects;

namespace withersdk.Net.Server
{
    public class ServerConnector
    {
        public static event EventHandler<ServerEventArgs> ServerEvent;
        internal static void CallEvent(HostController controller, ServerEventArgs args)
        {
            ServerEvent?.Invoke(controller, args);
        }
        public HostController Controller { get; set; }
        public Thread MainThread { get; set; }
        public void Start(Configurations config)
        {
            try
            {
                Controller = new HostController(config, this);
                CallEvent(Controller, new ServerEventArgs
                {
                    Type = ServerEventType.Info,
                    Message = Language.TRY_START_SERVER
                });
                MainThread = new Thread(new ThreadStart(Controller.Listen));
                MainThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                CallEvent(Controller, new ServerEventArgs
                {
                    Type = ServerEventType.Error,
                    Message = Language.TRY_START_SERVER_EX,
                    Exception = ex
                });
                Controller.Close();
            }
        }
    }
}
