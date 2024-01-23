using System;
using System.Collections.Generic;
using System.Text;

namespace withersdk.Net.Client
{
    public class ClientEventArgs
    {
        public ClientEventType Type {  get; set; }
        public string Message { get; set; }
        public byte[] Update { get; set; }
        public string Secondary { get; set; }
        public Exception Exception { get; set; }
    }

    public enum ClientEventType
    {
        Info,
        Warn,
        Error,
        Update,
        Another
    }
}
