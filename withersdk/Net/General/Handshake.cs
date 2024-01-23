using System;
using System.Collections.Generic;
using System.Text;
using withersdk.Utils;

namespace withersdk.Net.General
{
    public class Handshake : IDisposable
    {
        private bool disposedValue;

        public byte[] PrivateKey { get; private set; }
        public byte[] PublicKey { get; private set; }
        public byte[] SharedKey { get; private set; }
        public Handshake()
        {
            PrivateKey = X25519.CreateRandomPrivateKey();
            PublicKey = X25519.GetPublicKey(PrivateKey);
        }

        public void CreateShared(byte[] peerPublicKey)
        {
            SharedKey = X25519.GetSharedSecret(PrivateKey, peerPublicKey);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PrivateKey = null;
                    PublicKey = null;
                    SharedKey = null;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
