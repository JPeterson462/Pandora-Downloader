using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class CryptoConfig
    {
        public string CipherName { get; set; }
        public string EncryptionKey { get; set; }
        public string DecryptionKey { get; set; }
        public string Algorithm { get; set; }
    }
}
