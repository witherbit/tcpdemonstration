using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using withersdk.Utils;

namespace withersdk.Net.General.Nodes
{
    internal class HandshakeNode : INode
    {
        public string Type => this.GetType().ToString();
        public static string TypeOf { get => typeof(HandshakeNode).ToString(); }

        [JsonIgnore]
        private byte[] _signKey { get; set; }
        [JsonIgnore]
        public byte[] SignKey { get => _signKey; }

        public HandshakeStage Stage { get; set; }
        public byte[] Sign { get; set; }
        public byte[] SignPublicKey { get; set; }
        public byte[] Body { get; set; }

        public string Tag => Guid.NewGuid().ToString();

        public DateTime Timestamp => DateTime.UtcNow;

        public HandshakeNode(byte[] body, HandshakeStage stage)
        {
            Body = body;
            Stage = stage;
            _signKey = new byte[32];
            RNGCryptoServiceProvider.Create().GetBytes(_signKey);
        }
        public byte[] PackToStream()
        {
            var signed = Body.SignIt(_signKey);
            Sign = signed.Signature;
            SignPublicKey = signed.PublicKey;
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
        public static HandshakeNode Unpack(byte[] json)
        {
            var instance = JsonConvert.DeserializeObject<HandshakeNode>(Encoding.UTF8.GetString(json));
            if (!instance.Sign.VerifySign(instance.Body, instance.SignPublicKey))
                throw new Exception(Language.INVALID_SIGNATURE_MESSAGE_EX);

            return instance;
        }
    }

    public enum HandshakeStage
    {
        Hello,
        PeerSecret,
        PeerMac,
        Finalize,
    }
}
