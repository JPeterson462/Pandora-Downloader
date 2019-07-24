using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class PandoraStationListDto
    {
        public PandoraStationListResultDto Result { get; set; }
    }
    public class PandoraStationListResultDto
    {
        public PandoraStationDto[] Stations { get; set; }
    }
    public class PandoraStationDto
    {
        public string StationId { get; set; }
        public string StationToken { get; set; }
        public bool IsQuickMix { get; set; }
        public string StationName { get; set; }
    }
}
