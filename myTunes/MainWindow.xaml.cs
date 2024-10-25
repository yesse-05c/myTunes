//All AI code comes the ChatGPT-4 model.

using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.IO;

namespace myTunes

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
  

    public partial class MainWindow : Window
    {
        //Next two lines gotten from GPT prompt: Alter my code so that it has a context menu that has a play and stop option that will play and stop the selected song.
        private DispatcherTimer playbackTimer;
        private bool isPlaying = false;
        private readonly MusicRepo musicRepo;
        private readonly MediaPlayer mediaPlayer;
        
        


        public MainWindow()
        {
            InitializeComponent();

            playbackTimer = new DispatcherTimer();
            playbackTimer.Interval = TimeSpan.FromMilliseconds(100); // Set interval as needed
            
            musicRepo = new MusicRepo();
            mediaPlayer = new MediaPlayer();
            DataContext = musicRepo;
            var playlists = musicRepo.Playlists;
            playlists.Insert(0, "All Music");
            //Bind playlists
            songsListBox.ItemsSource = playlists;
            
            dataGrid.ItemsSource = musicRepo.Songs.DefaultView;
            playButton.Click += PlayButton_Click;
            stopButton.Click += StopButton_Click;
        }
        //code gotten from ChatGPT prompt: I want you to alter the code so that when a specific playlist is selected, only songs from that playlist should be displayed in the data grid. 
        private void songsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected playlist name
            string selectedPlaylist = (string)songsListBox.SelectedItem;

            // Check if a playlist is selected
            if (!string.IsNullOrEmpty(selectedPlaylist))
            {
                // Get songs from the selected playlist
                DataTable playlistSongs = musicRepo.SongsForPlaylist(selectedPlaylist);

                // Set the DataGrid's item source to display playlist songs
                dataGrid.ItemsSource = playlistSongs.DefaultView;
            }
            else
            {
                // Clear the DataGrid if no playlist is selected
                dataGrid.ItemsSource = null;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlaySelectedSong();
        }

        private void PlayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PlaySelectedSong();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopSong();
        }

        //GPT Prompt: Create a function that takes the selected item and plays it with the MediaPlayer class
        private void PlaySelectedSong()
        {
            if (dataGrid.SelectedItem is DataRowView selectedRow)
            {
                // Assuming your DataTable has these columns
                int songId = (int)selectedRow["id"];
                string title = (string)selectedRow["title"];

                // Here, implement your logic to play the song
                Console.WriteLine($"Playing song: {title}");

                // Start playback logic (e.g., using a media player library)
                // mediaPlayer.Open(new Uri(song.Filename)); // Assuming you have a Filename property
                Song currSong = musicRepo.GetSong(songId);
                PlaySong(currSong.Filename);
                //mediaPlayer.Open(new Uri(currSong.Filename));
                //mediaPlayer.Play();

                isPlaying = true;
                playbackTimer.Start();
            }
        }

        private void PlaySong(string filePath)
        {
            if (File.Exists(filePath))
            {

                mediaPlayer.Open(new Uri(filePath, UriKind.Absolute));
                mediaPlayer.Play();
            }
            else
            {
                

                MessageBox.Show("File not found: " + filePath + "Current Directory: " + Environment.CurrentDirectory);
            }
        }
        //GPT prompt: Alter my code to create a function that stops a song from playing.
        private void StopSong()
        {
            if (isPlaying)
            {
                Console.WriteLine("Stopping song.");
                mediaPlayer.Stop(); // Stop the media player
                isPlaying = false;
                playbackTimer.Stop();
            }
        }
        private void StopMenuItem_Click(object sender, RoutedEventArgs e)
        {
            StopSong();
        }
        private void PlayCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            PlaySelectedSong();
        }

        private void StopCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Stop();
            isPlaying = false;
        }
        private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isPlaying;
        }

        private void PlayCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dataGrid.ItemsSource != null;
        }

    }
}