using Pandora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace PandoraPlayer
{
    public class PlayerContext
    {
        private List<PandoraSong> queue;
        private int currentSong;
        private bool playing;
        private SoundPlayer player;
        private string Station;
        public void Setup(string station)
        {
            Station = station;
        }
        public void LoadSongs(string station)
        {
            List<PandoraSong> options = MainWindow.Songs[station];
            queue = options.OrderBy(arg => Guid.NewGuid()).Take(Math.Min(20, options.Count)).ToList();
            currentSong = 0;
            playing = false;
        }
        public void Pause()
        {
            if (playing)
            {
                playing = false;
                player.Stop();
            }
        }
        public void Play()
        {
            if (!playing)
            {
                playing = true;
                player.Play();
            }
        }
        public void PlayNext()
        {
            Console.WriteLine("Playing " + queue[currentSong].AudioUrl);
            player = new SoundPlayer(queue[currentSong].AudioUrl);
            player.PlaySync();
            currentSong++;
            if (currentSong == queue.Count)
            {
                LoadSongs(Station);
            }
        }
        public void Skip()
        {
            player.Stop();
            PlayNext();
        }
    }
}
