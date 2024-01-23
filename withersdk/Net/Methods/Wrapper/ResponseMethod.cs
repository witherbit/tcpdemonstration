using System;
using System.Collections.Generic;
using System.Text;

namespace withersdk.Net.Methods.Wrapper
{
    public struct ResponseMethod
    {
        public class Pong : Method
        {
            public static string TypeOf { get => typeof(Pong).ToString(); }
            public string Message => "pong";
            public DateTime GetTime {  get; set; }
            public DateTime SendTime { get; set; }
        }

        public class Authorization : Method
        {
            public static string TypeOf { get => typeof(Authorization).ToString(); }
            public bool IsExist { get; set; }
            public bool Allow { get; set; }
            public byte[] Token { get; set; }
        }
    }
}
