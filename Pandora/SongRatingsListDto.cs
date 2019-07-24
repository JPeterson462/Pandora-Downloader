using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora
{
    public class SongRatingsListDto
    {
        public SongRatingDto[] Ratings { get; set; }
    }
    public class SongRatingDto
    {
        public string Title { get; set; }
        public int Rating { get; set; }
    }
}
