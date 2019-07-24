using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class Crypto
    {
        public class Algorithm
        {
            public const string BLOWFISH = "Blowfish";
        }
        public string Encrypt(CryptoConfig config, string plaintext)
        {
            if (config.Algorithm == Algorithm.BLOWFISH)
            {
                Blowfish encrypt = new Blowfish(PandoraConstants.ANDROID_ENCRYPTION_KEY);
                return encrypt.Encrypt(plaintext);
            }
            return null;
        }
        public string Decrypt(CryptoConfig config, string ciphertext)
        {
            if (config.Algorithm == Algorithm.BLOWFISH)
            {
                Blowfish decrypt = new Blowfish(PandoraConstants.ANDROID_DECRYPTION_KEY);
                return decrypt.Decrypt(ciphertext, false);

            }
            return null;
        }
    }
}
