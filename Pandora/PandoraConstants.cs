using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class PandoraConstants
    {
        public const string ANDROID_DECRYPTION_KEY = "R=U!LH$O2B#";
	    public const string ANDROID_ENCRYPTION_KEY = "6#26FRL$ZWD";
	    public const string BLOWFISH_ECB_PKCS5_PADDING = "Blowfish/ECB/PKCS5Padding";
	    public const string BASE_URL = "https://tuner.pandora.com/services/json/?";
	    public const string BASE_NON_TLS_URL = "http://tuner.pandora.com/services/json/?";
	    public const string ANDROID_PARTNER_PASSWORD = "AC7IBG09A3DTSYM4R41UJWL07VLN8JI7";
        public static string BuildUrl(string suffix)
        {
            return BASE_URL + suffix;
        }
        public static CryptoConfig GetCryptoConfig()
        {
            return new CryptoConfig
            {
                Algorithm = Crypto.Algorithm.BLOWFISH,
                DecryptionKey = ANDROID_DECRYPTION_KEY,
                EncryptionKey = ANDROID_ENCRYPTION_KEY,
                CipherName = BLOWFISH_ECB_PKCS5_PADDING
            };
        }
    }
}
