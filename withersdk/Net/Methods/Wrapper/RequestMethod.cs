using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using withersdk.Net.General;
using withersdk.Net.General.Nodes;

namespace withersdk.Net.Methods.Wrapper
{
    public struct RequestMethod
    {
        public class Ping : Method
        {
            public static string TypeOf { get => typeof(Ping).ToString(); }
            public string Message => "ping";
            public DateTime SendTime { get; set; }
        }

        public class SignUp : Method
        {
            public static string TypeOf { get => typeof(SignUp).ToString(); }
            public string Login { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class SignIn : Method
        {
            public static string TypeOf { get => typeof(SignIn).ToString(); }
            public string Login { get; set; }
            public string Password { get; set; }

            public string Token { get; set; }
        }
    }
}
