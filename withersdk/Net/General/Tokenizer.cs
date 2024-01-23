using System;
using System.Collections.Generic;
using System.Text;
using withersdk.Utils;

namespace withersdk.Net.General
{
    internal static class Tokenizer
    {
        public static byte[] CreateToken(this withersdk.Net.Server.Objects.Client client, string password)
        {
            var ip = client.Ip;
            var id = client.ConnectionId;
            var info = client.DeviceInfo;
            return $"Ip:{ip}ConnectionId:{id}ClientAppName:{info.AppName}ClientUserName:{info.UserName}AbstractConnections:{info.Serialize()} ++PPs{password}".Rfc2898DeriveInlineBytes();
        }
        public static bool VerifyToken(this withersdk.Net.Server.Objects.Client client, byte[]token, string password)
        {
            var ip = client.Ip;
            var id = client.ConnectionId;
            var info = client.DeviceInfo;
            
            return token.Rfc2898VerifyInlineBytes($"Ip:{ip}ConnectionId:{id}ClientAppName:{info.AppName}ClientUserName:{info.UserName}AbstractConnections:{info.Serialize()} ++PPs{password}");
        }
    }
}
