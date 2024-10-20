﻿using System.Data;
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
    public partial class MainWindow : Window
    {
        private readonly MusicRepo musicRepo;

        public MainWindow()
        {
            InitializeComponent();
            musicRepo = new MusicRepo();
            var playlists = musicRepo.Playlists;
            playlists.Insert(0, "All Music");
            //Bind playlists
            songsListBox.ItemsSource = playlists;
            
            dataGrid.ItemsSource = musicRepo.Songs.DefaultView;
            
        }
    } 
}