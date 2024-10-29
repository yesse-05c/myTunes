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


//Code based on ChatGPT prompt: Create me a rename dialog box class to rename a playlist leveraging the MusicRepo class.
namespace myTunes
{
    /// <summary>
    /// Interaction logic for RenamePlaylistDialog.xaml
    /// </summary>
    public partial class RenamePlaylistDialog : Window
    {
         public RenamePlaylistDialog(string currentPlaylistName)
        {
            InitializeComponent();
            Title = $"Rename '{currentPlaylistName}'";
        }

        public string NewPlaylistName { get; private set; }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NewPlaylistName = NewPlaylistTextBox.Text.Trim(); // Get the new name from TextBox
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
