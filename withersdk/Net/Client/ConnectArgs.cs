using System;
using System.Collections.Generic;
using System.Text;

namespace withersdk.Net.Client
{
    public struct ConnectArgs
    {
        public int Port { get; set; }
        public string Address { get; set; }
        public string AppName { get; set; } 
    }
}
