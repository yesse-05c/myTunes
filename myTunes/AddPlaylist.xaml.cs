using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace myTunes
{
    /// <summary>
    /// Interaction logic for AddPlaylist.xaml
    /// </summary>
    public partial class AddPlaylist : Window
    {
        public string? PlaylistName { get; private set; }
        public AddPlaylist()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistName = playlistNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(PlaylistName))
            {
                MessageBox.Show("Please enter a playlist name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                DialogResult = true; // Closes the window and returns true
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Set DialogResult to false to indicate cancellation
            Close();
        }
    }
}
