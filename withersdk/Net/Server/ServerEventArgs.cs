using System;
using System.Collections.Generic;
using System.Text;

namespace withersdk.Net.Server
{
    public class ServerEventArgs
    {
        public ServerEventType Type { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string Secondary {  get; set; }
        public byte[] Update {  get; set; }
    }

    public enum ServerEventType
    {
        Warn,
        Error,
        Info,
        Update,
        Another
    }
}
