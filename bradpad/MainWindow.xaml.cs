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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace bradpad {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        Dictionary<string, string> applicationInfo = new Dictionary<string, string>();
        string recentName;

        public MainWindow() {
            InitializeComponent();
            recentName = "";
        }

        private void MainButtonClicked(object sender, RoutedEventArgs e) {
            mainPanel.Visibility = Visibility.Visible;
            settingsPanel.Visibility = Visibility.Hidden;
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e) {
            mainPanel.Visibility = Visibility.Hidden;
            settingsPanel.Visibility = Visibility.Visible;
            applicationsPanel.Visibility = Visibility.Hidden;
            if(recentName != "")
            {
                F10.Content = recentName;
            }

        }

        internal void F10ButtonClicked(object sender, RoutedEventArgs e) {
            if (applicationInfo.ContainsKey((string)F10.Content))
            {
                ((App)Application.Current).ButtonClickedKeyPress(Key.F10, applicationInfo[(string)F10.Content]);
            }
        }

        internal void F11ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F11);
        }

        internal void F12ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F12);
        }
        
        private void foreGroundButtonClicked(object sender, RoutedEventArgs e)
        {
            if(foreGroundCheck.IsChecked == false)
            {
                this.Topmost = false;
            }
            else
            {
                this.Topmost = true;
            }
        }
        private void appButtonClicked(object sender, RoutedEventArgs e)
        {
            string t = addAppText.Text;
            string tt = addAppLocation.Text;
            addAppText.Text = "Enter Application Name";
            addAppLocation.Text = "Enter Application Location";
            applicationInfo[t] = tt;
            recentName = t;

        }
        private void applicationsButtonClick(object sender, RoutedEventArgs e)
        {
            applicationsPanel.Visibility = Visibility.Visible;
            settingsPanel.Visibility = Visibility.Hidden;
        }
        
    }
}
