using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using withersdk.Net.General;
using withersdk.Net.General.Nodes;
using static withersdk.Net.Methods.Wrapper.RequestMethod;

namespace withersdk.Net.Methods
{
    public abstract class Method
    {
        public string Type => this.GetType().ToString();
        public virtual string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static T Deserialize<T>(string method)
        {
            return JsonConvert.DeserializeObject<T>(method);
        }
        public virtual INode SerializeToMessage(KeyPair keyPair, bool sign = false)
        {
            if (sign)
                return new SignedNode(Serialize(), keyPair);
            else
                return new Node(Serialize(), keyPair);
        }
    }
}
