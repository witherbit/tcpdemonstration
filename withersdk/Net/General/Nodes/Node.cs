using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using withersdk.Utils;

namespace withersdk.Net.General.Nodes
{
    public class Node : INode
    {
        public string Type => this.GetType().ToString();
        public static string TypeOf { get => typeof(Node).ToString(); }

        [JsonIgnore]
        public KeyPair KeyPair { get; set; }

        [JsonIgnore]
        public string Content { get; set; }

        public byte[] Body { get; set; }
        public byte[] MAC { get; set; }
        public string Tag => Guid.NewGuid().ToString();

        public DateTime Timestamp => DateTime.UtcNow;

        public Node(string content, KeyPair keyPair)
        {
            KeyPair = keyPair;
            Content = content;
        }

        public byte[] PackToStream()
        {
            MAC = Encoding.UTF8.GetBytes(Content).ComputeHMAC(KeyPair.KMAC);

            var rc6 = new RC6(256, KeyPair.Ki);
            Body = rc6.Encode(Encoding.UTF8.GetBytes(Content));

            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static Node Unpack(byte[] json, KeyPair keyPair)
        {
            var instance = JsonConvert.DeserializeObject<Node>(Encoding.UTF8.GetString(json));
            instance.KeyPair = keyPair;

            var rc6 = new RC6(256, instance.KeyPair.Ki);
            instance.Content = Encoding.UTF8.GetString(rc6.Decode(instance.Body));

            if (!Encoding.UTF8.GetBytes(instance.Content).CheckHMAC(instance.KeyPair.KMAC, instance.MAC))
                throw new Exception(Language.INVALID_MAC_MESSAGE_EX);

            return instance;
        }
    }
}
