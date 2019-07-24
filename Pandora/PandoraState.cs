using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class PandoraState
    {
        public string PartnerAuthToken { get; set; }
        public long SyncTime { get; set; }
        public int PartnerId { get; set; }
        public string UserAuthToken { get; set; }
        public long UserId { get; set; }
    }
}
