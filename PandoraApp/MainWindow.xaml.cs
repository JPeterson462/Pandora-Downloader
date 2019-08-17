using Newtonsoft.Json;
using Pandora;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PandoraApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct Song
        {
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Album { get; set; }
        }
        private PandoraService pandoraService;
        private RESTfulService restfulService;
        private PandoraState pandoraState;
        private PandoraConfig config;
        public MainWindow()
        {
            pandoraService = new PandoraService();
            restfulService = new RESTfulService();
            InitializeComponent();
        }
        public void ReloadSongLibrary()
        {
            Files.Items.Clear();
            string[] files = Directory.GetFiles(PandoraDirectory.Text);
            foreach (string file in files)
            {
                if (file.EndsWith(".mp4")) {
                    TagLib.File mp3File = TagLib.File.Create(file);
                    Files.Items.Add(new Song()
                    {
                        Title = mp3File.Tag.Title,
                        Artist = string.Join(", ", mp3File.Tag.AlbumArtists),
                        Album = mp3File.Tag.Album
                    }); ;
                }
            }
        }
        private void ChangeDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog directoryChooser = new FolderBrowserDialog();
            DialogResult result = directoryChooser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(directoryChooser.SelectedPath))
            {
                PandoraDirectory.Text = directoryChooser.SelectedPath;
            }
        }
        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            string[] files = Directory.GetFiles(PandoraDirectory.Text);
            foreach (string file in files)
            {
                if (file.EndsWith("config.json"))
                {
                    config = JsonConvert.DeserializeObject<PandoraConfig>(File.ReadAllText(file));
                }
            }
            ReloadSongLibrary();

            pandoraState = await pandoraService.ConnectAsync(restfulService);
            bool success = await pandoraService.LoginAsync(config, restfulService, pandoraState);
            if (success)
            {
                List<PandoraStation> stations = await pandoraService.GetPandoraStationsAsync(config, restfulService, pandoraState);
                Station.Items.Clear();
                foreach (PandoraStation station in stations)
                {
                    Station.Items.Add(station.Name);
                }
                Station.SelectedValue = "Thumbprint Radio";
            }
            else
            {
                Log.Text = Log.Text + "\n" + "Login Failed!";
            }
        }
        public bool IsInLibrary(string songName, string albumName, string artistName)
        {
            if (Files.Items.Count == 0)
            {
                return false;
            }
            foreach (Song song in Files.Items)
            {
                if (song.Title == songName && song.Album == albumName && song.Artist == artistName)
                {
                    return true;
                }
            }
            return false;
        }
        private string SanitizeFilename(string filename)
        {
            string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(filename, "").Trim();
        }
        private async void DownloadSongs_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new WebClient())
            {
                List<PandoraStation> stations = await pandoraService.GetPandoraStationsAsync(config, restfulService, pandoraState);
                PandoraStation station = stations.Where(s => s.Name == Station.Text).FirstOrDefault();
                if (station != null)
                {
                    int songsLeft = int.Parse(NumberOfSongs.Text);
                    int count = 0;
                    while (songsLeft > 0)
                    {
                        List<PandoraSong> playlist = await pandoraService.GetPandoraPlaylistAsync(restfulService, pandoraState, station);
                        foreach (PandoraSong song in playlist)
                        {
                            if (!IsInLibrary(song.Title, song.AlbumName, song.ArtistName) && songsLeft > 0)
                            {
                                System.Diagnostics.Debug.WriteLine(song.Title + "; " + song.AlbumName + "; " + song.ArtistName);
                                string formattedTitle = song.Title;
                                client.DownloadFile(song.AudioUrl, PandoraDirectory.Text + "/" + SanitizeFilename(formattedTitle) + ".mp4");
                                if (song.AlbumArtUrl != null)
                                {
                                    client.DownloadFile(song.AlbumArtUrl, PandoraDirectory.Text + "/" + SanitizeFilename(song.AlbumName) + ".png");
                                }
                                TagLib.File mp3File = TagLib.File.Create(PandoraDirectory.Text + "/" + SanitizeFilename(formattedTitle) + ".mp4");
                                mp3File.Tag.Title = song.Title;
                                mp3File.Tag.Album = song.AlbumName;
                                mp3File.Tag.AlbumArtists = song.ArtistName.Split(',');
                                mp3File.Tag.Pictures = new TagLib.IPicture[] {
                                    new TagLib.Picture(PandoraDirectory.Text + "/" + SanitizeFilename(song.AlbumName) + ".png")
                                };
                                mp3File.Save();
                                Log.AppendText("\n" + "Downloading (" + (count + 1) + "/" + NumberOfSongs.Text + "): " + song.Title + " by " + song.ArtistName);
                                Files.Items.Add(new Song()
                                {
                                    Title = song.Title,
                                    Album = song.AlbumName,
                                    Artist = song.ArtistName
                                });
                                songsLeft--;
                                count++;
                            }
                        }
                    }
                    Files.Items.SortDescriptions.Add(new SortDescription(Files.Columns[0].SortMemberPath, ListSortDirection.Ascending));
                }
                else
                {
                    Log.Text = Log.Text + "\n" + "Station not found: " + Station.Text;
                }
            }            
        }
        private void Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Files.SelectedItems.Count == 1)
            {
                Song song = (Song)Files.SelectedItems[0];
                if (File.Exists(PandoraDirectory.Text + "/" + SanitizeFilename(song.Album + ".png")))
                {
                    AlbumArt.Source = new BitmapImage(new Uri(PandoraDirectory.Text + "/" + SanitizeFilename(song.Album + ".png")));
                }
                else
                {
                    AlbumArt.Source = null;
                }
            }
            else
            {
                AlbumArt.Source = null;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
        private void Log_TextChanged(object sender, TextChangedEventArgs e)
        {
            Log.ScrollToEnd();
        }
        private void GenerateConfig_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(PandoraDirectory.Text))
            {
                ConfigGenerator configGenerator = new ConfigGenerator();
                if (configGenerator.ShowDialog() ?? false)
                {
                    Dictionary<string, object> config = new Dictionary<string, object>()
                    {
                        { "Username", configGenerator.Username.Text },
                        { "Password", configGenerator.Password.Password }
                    };
                    File.WriteAllText(PandoraDirectory.Text + "/config.json", JsonConvert.SerializeObject(config));
                }
            }
        }
    }
}
