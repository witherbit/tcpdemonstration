using System;
using System.Collections.Generic;
using System.Text;

namespace withersdk.Net.General
{
    internal class KeyContainer
    {
        public byte[] Ki {  get; set; }
        public byte[] KMAC { get; set; }

        public KeyPair GetKeyPair()
        {
            return new KeyPair
            {
                Ki = Ki,
                KMAC = KMAC
            };
        }
    }
}
