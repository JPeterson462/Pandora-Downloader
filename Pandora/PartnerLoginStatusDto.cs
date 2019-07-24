using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class PartnerLoginStatusDto
    {
        public string Stat { get; set; }
        public PartnerLoginStatusResultDto Result { get; set; }
    }
    public class PartnerLoginStatusResultDto
    {
        public string UserAuthToken { get; set; }
        public long UserId { get; set; }
    }
}
