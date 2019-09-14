using Pandora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace PandoraPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dictionary<string, List<PandoraSong>> Songs = new Dictionary<string, List<PandoraSong>>();
        private Thread thread;
        public static PlayerContext Context;
        public MainWindow()
        {
            InitializeComponent();
        }
        public class StationCache
        {
            public string Name { get; set; }
            public List<PandoraSong> Songs { get; set; } = new List<PandoraSong>();
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
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("CONNECTING...");
            string[] caches = Directory.GetFiles(PandoraDirectory.Text, "Songs*.json");
            foreach (string cache in caches)
            {
                StationCache station = JsonConvert.DeserializeObject<StationCache>(File.ReadAllText(cache));
                if (!Songs.ContainsKey(station.Name))
                {
                    Songs[station.Name] = new List<PandoraSong>();
                }
                Songs[station.Name].AddRange(station.Songs);
            }
            Station.Items.Clear();
            foreach (string name in Songs.Keys)
            {
                Station.Items.Add(name);
            }
            Station.SelectedValue = "Thumbprint Radio";

        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
            Context = new PlayerContext();
            Context.Setup(Station.SelectedItem.ToString());
            thread = new Thread(() => PandoraThread.Run(Context));
            thread.Start();
            PausePlay.IsEnabled = true;
        }
        private void PausePlay_Click(object sender, RoutedEventArgs e)
        {
            if (PausePlay.Content.ToString() == "Pause")
            {
                Context.Pause();
                PausePlay.Content = "Play";
            }
            else if (PausePlay.Content.ToString() == "Play")
            {
                Context.Play();
                PausePlay.Content = "Pause";
            }
        }
        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            Context.Skip();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            thread.Abort();
        }
    }
}
