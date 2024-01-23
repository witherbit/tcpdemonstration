using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using withersdk.Utils;

namespace withersdk.Net.General.Nodes
{
    public class SignedNode : INode
    {
        public string Type => this.GetType().ToString();
        public static string TypeOf { get => typeof(SignedNode).ToString(); }

        [JsonIgnore]
        public KeyPair KeyPair { get; set; }
        [JsonIgnore]
        private byte[] _signKey { get; set; }
        [JsonIgnore]
        public byte[] SignKey { get => _signKey; }
        [JsonIgnore]
        public string Content { get; set; }
        public byte[] Sign { get; set; }
        public byte[] SignPublicKey { get; set; }
        public byte[] Body { get; set; }
        public byte[] MAC { get; set; }
        public string Tag => Guid.NewGuid().ToString();

        public DateTime Timestamp => DateTime.UtcNow;

        public SignedNode(string content, KeyPair keyPair)
        {
            KeyPair = keyPair;
            Content = content;
            _signKey = new byte[32];
            RNGCryptoServiceProvider.Create().GetBytes(_signKey);
        }

        public byte[] PackToStream()
        {
            MAC = Encoding.UTF8.GetBytes(Content).ComputeHMAC(KeyPair.KMAC);

            var signed = Encoding.UTF8.GetBytes(Content).SignIt(_signKey);
            Sign = signed.Signature;
            SignPublicKey = signed.PublicKey;

            var rc6 = new RC6(256, KeyPair.Ki);
            Body = rc6.Encode(Encoding.UTF8.GetBytes(Content));

            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static SignedNode Unpack(byte[] json, KeyPair keyPair)
        {
            var instance = JsonConvert.DeserializeObject<SignedNode>(Encoding.UTF8.GetString(json));
            instance.KeyPair = keyPair;

            var rc6 = new RC6(256, instance.KeyPair.Ki);
            instance.Content = Encoding.UTF8.GetString(rc6.Decode(instance.Body));

            if (!instance.Sign.VerifySign(Encoding.UTF8.GetBytes(instance.Content), instance.SignPublicKey))
                throw new Exception(Language.INVALID_SIGNATURE_MESSAGE_EX);
            if (!Encoding.UTF8.GetBytes(instance.Content).CheckHMAC(instance.KeyPair.KMAC, instance.MAC))
                throw new Exception(Language.INVALID_MAC_MESSAGE_EX);

            return instance;
        }
    }
}
