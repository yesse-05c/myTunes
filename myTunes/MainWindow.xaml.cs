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
using Microsoft.Win32;
using myTunes;
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

            // Select "All Music" initially to display all songs
            songsListBox.SelectedIndex = 0;  // Select "All Music" by default
            dataGrid.ItemsSource = musicRepo.Songs.DefaultView;

            playButton.Click += PlayButton_Click;
            stopButton.Click += StopButton_Click;
            addSongButton.Click += AddSongButton_Click; // Assume you have a button named addSongButton
            CommandBindings.Add(new CommandBinding(MediaCommands.Play, PlayCommand_Executed, PlayCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(MediaCommands.Stop, StopCommand_Executed, StopCommand_CanExecute));

        }
        private void RemovePlaylistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (songsListBox.SelectedItem is string selectedPlaylist && selectedPlaylist != "All Music")
            {
                var result = MessageBox.Show($"Are you sure you want to delete the playlist '{selectedPlaylist}'?", "Confirm Delete", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // Call the DeletePlaylist method in musicRepo
                    musicRepo.DeletePlaylist(selectedPlaylist);

                    // Refresh the ListBox to reflect changes
                    RefreshPlaylists();

                    // Select "All Music" after deletion to avoid null selection issues
                    songsListBox.SelectedIndex = 0;
                }
            }
            else
            {
                MessageBox.Show("Cannot delete the 'All Music' playlist.", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void RenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (songsListBox.SelectedItem is string selectedPlaylist && selectedPlaylist != "All Music")
            {
                // Open the rename dialog box
                var renameDialog = new RenamePlaylistDialog(selectedPlaylist);
                if (renameDialog.ShowDialog() == true)
                {
                    string newPlaylistName = renameDialog.NewPlaylistName;

                    // Validate new playlist name (no blanks or duplicates)
                    if (!string.IsNullOrWhiteSpace(newPlaylistName) && !musicRepo.Playlists.Contains(newPlaylistName))
                    {
                        // Use RenamePlaylist in MusicRepo
                        musicRepo.RenamePlaylist(selectedPlaylist, newPlaylistName);

                        // Refresh the playlist list in the UI
                        RefreshPlaylists();

                        // Optionally reselect the renamed playlist
                        songsListBox.SelectedItem = newPlaylistName;
                    }
                    else
                    {
                        MessageBox.Show("Please enter a unique name for the playlist.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Cannot rename the 'All Music' playlist.", "Rename Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        //code gotten from ChatGPT prompt: I want you to alter the code so that when a specific playlist is selected, only songs from that playlist should be displayed in the data grid. 
        private void songsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected playlist name
            string selectedPlaylist = (string)songsListBox.SelectedItem;

            if (selectedPlaylist == "All Music")
            {
                // Display all songs if "All Music" is selected
                dataGrid.ItemsSource = musicRepo.Songs.DefaultView;
            }
            // Check if a playlist is selected
            else if (!string.IsNullOrEmpty(selectedPlaylist))
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
        //Code below gotten from GPT prompt: Alter my code so that the app should allow songs to be added to the listing of songs by launching an open dialog box and allowing the user to select a song.  Provide a filter that by default shows .mp3, .m4a, .wma, and .wav files in the open file dialog box.  After selecting a song, the program should read the metadata stored in the music file (if available), add the song to the data grid, and select/highlight the song (so the user can see it among the list of potentially hundreds of songs).

        private void AddSongButton_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Music Files|*.mp3;*.m4a;*.wma;*.wav",
                Title = "Select a Song"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Read the metadata and add the song to the musicRepo
                AddSongToMusicRepo(filePath);
            }
        }

        private void AddSongToMusicRepo(string filePath)
        {
            // Use TagLib to read metadata
            var file = TagLib.File.Create(filePath);
            string title = file.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(filePath);
            string artist = file.Tag.Artists.Length > 0 ? file.Tag.Artists[0] : "Unknown Artist";
            string album = file.Tag.Album ?? "Unknown Album";

            // Add song to the MusicRepo (you'll need to implement this)
            musicRepo.AddSong(new Song
            {
                Title = title,
                Artist = artist,
                Album = album,
                Filename = filePath
            });
            // Save the updated music data to XML
            musicRepo.Save(); // Ensure that changes are saved

            // Refresh the data grid to show the new song and also ensure "All Music" is selected
            if (songsListBox.SelectedItem?.ToString() == "All Music")
            {
                dataGrid.ItemsSource = musicRepo.Songs.DefaultView; // Show all songs
            }

            // Select the newly added song (optional)
            if (musicRepo.Songs.Rows.Count > 0)
            {
                DataRow lastRow = musicRepo.Songs.Rows[musicRepo.Songs.Rows.Count - 1];
                dataGrid.SelectedItem = lastRow; // Select the DataRow directly
                dataGrid.ScrollIntoView(lastRow); // Scroll into view
            }// Refresh the data grid
            // Assuming newSong is the object you just added
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

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var addPlaylistWindow = new AddPlaylist { Owner = this };
            if (addPlaylistWindow.ShowDialog() == true)
            {
                string newPlaylistName = addPlaylistWindow.PlaylistName;
                if (musicRepo.AddPlaylist(newPlaylistName))
                {
                    RefreshPlaylists();
                    // After adding a new playlist, ensure "All Music" is still selected
                    songsListBox.SelectedIndex = 0; // This selects "All Music"
                }
                else
                {
                    MessageBox.Show("A playlist with that name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RefreshPlaylists()
        {
            // Fetch playlists from musicRepo
            var playlists = musicRepo.GetPlaylists();

            // Ensure "All Music" is always the first item
            if (!playlists.Contains("All Music"))
            {
                playlists.Insert(0, "All Music");
            }

            songsListBox.ItemsSource = playlists; // Directly set the updated playlists to the ItemsSource
        }
        private void PlaylistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (songsListBox.SelectedItem is string playlistName)
            {
                DisplaySongsForPlaylist(playlistName);
            }
        }

        private void DisplaySongsForPlaylist(string playlistName)
        {
            if (playlistName == "All Music")
            {
                // Display all songs if "All Music" is selected
                dataGrid.ItemsSource = musicRepo.Songs.DefaultView;
            }
            else
            {
                // Display songs for the specific playlist
                var songTable = musicRepo.SongsForPlaylist(playlistName);
                dataGrid.ItemsSource = songTable.DefaultView;
            }
        }

        private void moreInfo_Click(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Owner = this; // Set the owner to the main window
            about.ShowDialog();
        }


        //Next three functions gotten from GPT prompt: 
        private void RemoveFromAllMusic_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView selectedSong)
            {
                RemoveSongFromAllMusic(selectedSong);
            }
        }

        private void RemoveSongFromAllMusic(DataRowView selectedSong)
        {
            var result = MessageBox.Show("Are you sure you want to remove this song from All Music?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                int songId = (int)selectedSong["id"];
                musicRepo.RemoveSong(songId);
                RefreshDataGrid();
            }
        }

        private void RemoveFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView selectedSong && songsListBox.SelectedItem is string playlistName && playlistName != "All Music")
            {
                RemoveSongFromPlaylist(selectedSong, playlistName);
            }
        }

        private void RemoveSongFromPlaylist(DataRowView selectedSong, string playlistName)
        {
            int songId = (int)selectedSong["id"];
            musicRepo.RemoveSongFromPlaylist(songId, playlistName);
            if (songsListBox.SelectedItem?.ToString() == playlistName)
            {
                DisplaySongsForPlaylist(playlistName);
            }
        }

        private void RefreshDataGrid()
        {
            // Refresh based on the selected item in the ListBox (either "All Music" or a specific playlist)
            string selectedPlaylist = songsListBox.SelectedItem.ToString();

            if (selectedPlaylist == "All Music")
            {
                // Display all songs
                dataGrid.ItemsSource = musicRepo.Songs.DefaultView;
            }
            else if (!string.IsNullOrEmpty(selectedPlaylist))
            {
                // Display songs specific to the selected playlist
                DisplaySongsForPlaylist(selectedPlaylist);
            }
        }
    

    private void SongsDataGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && dataGrid.SelectedItem != null)
            {
                // Get the song ID to pass in drag-and-drop operation
                int songId = (int)((DataRowView)dataGrid.SelectedItem)["id"];

                // Start drag-and-drop with the song ID
                DragDrop.DoDragDrop(dataGrid, songId, DragDropEffects.Move);
            }
        }

        // Handle DragOver event to allow dropping on a specific playlist
        private void PlaylistLabel_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(int)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        // Handle Drop event to add song to the targeted playlist
        private void PlaylistLabel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(int)))
            {
                // Get the song ID from the drag data
                int songId = (int)e.Data.GetData(typeof(int));

                // Identify the target playlist by its Label content
                Label playlistLabel = sender as Label;
                if (playlistLabel != null)
                {
                    string playlistName = playlistLabel.Content.ToString();

                    // Add song to the playlist in MusicRepo
                    musicRepo.AddSongToPlaylist(songId, playlistName);

                    // Refresh DataGrid if viewing the playlist where song was dropped
                    if ((string)songsListBox.SelectedItem == playlistName)
                    {
                        DisplaySongsForPlaylist(playlistName);
                    }
                    // Save the updated music data to XML
                    musicRepo.Save(); // Call your existing Save method
                }
            }
            e.Handled = true;
        }
    }

}