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

namespace myTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class CustomCommands
    {
        // Make command accessible to XAML
        public static readonly RoutedUICommand Play = new RoutedUICommand(
                 "Play", "Play", typeof(CustomCommands));
        public static readonly RoutedUICommand Stop = new RoutedUICommand(
            "Stop", "Stop", typeof(CustomCommands));
    }

    public partial class MainWindow : Window
    {
        private readonly MusicRepo musicRepo;
        private readonly MediaPlayer mediaPlayer;
        private bool isPlaying = false;



        public MainWindow()
        {
            InitializeComponent();
            musicRepo = new MusicRepo();
            mediaPlayer = new MediaPlayer();
            var playlists = musicRepo.Playlists;
            playlists.Insert(0, "All Music");
            //Bind playlists
            songsListBox.ItemsSource = playlists;
            
            dataGrid.ItemsSource = musicRepo.Songs.DefaultView;
            
        }
        private void PlayCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Play();
            isPlaying = true;
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