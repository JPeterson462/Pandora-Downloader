using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class PartnerLoginDataDto
    {
        public PartnerLoginResultDto Result { get; set; }
    }
    public class PartnerLoginResultDto
    {
        public string SyncTime { get; set; }
        public string PartnerAuthToken { get; set; }
        public int PartnerId { get; set; }
    }
}
