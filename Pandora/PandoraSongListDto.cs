using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class PandoraSongListDto
    {
        public PandoraSongListResultDto Result { get; set; }
    }
    public class PandoraSongListResultDto
    {
        public PandoraSongDto[] Items;
    }
    public class PandoraSongDto
    {
        public string AdToken { get; set; }
        public string AlbumName { get; set; }
        public string ArtistName { get; set; }
        public PandoraAudioUrlMapDto AudioUrlMap { get; set; }
        public string AdditionalAudioUrl { get; set; }
        public string SongName { get; set; }
        public string AlbumDetailUrl { get; set; }
        public string AlbumArtUrl { get; set; }
        public string TrackToken { get; set; }
        public int SongRating { get; set; }
    }
    public class PandoraAudioUrlMapDto
    {
        public PandoraAudioUrlMapResultDto HighQuality { get; set; }
    }
    public class PandoraAudioUrlMapResultDto
    {
        public string AudioUrl { get; set; }
    }
}
