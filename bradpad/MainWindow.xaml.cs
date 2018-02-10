﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace bradpad {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void MainButtonClicked(object sender, RoutedEventArgs e) {
            mainPanel.Visibility = Visibility.Visible;
            settingsPanel.Visibility = Visibility.Hidden;
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e) {
            mainPanel.Visibility = Visibility.Hidden;
            settingsPanel.Visibility = Visibility.Visible;
        }
        
        internal void F22ButtonClicked(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F22);
        }

        private void F23ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F23);
        }

        private void F24ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F24);
        }
    }
}
