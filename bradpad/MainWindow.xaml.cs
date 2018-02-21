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
        private void HelpButtonClicked(object sender, RoutedEventArgs e)
        {
            settingsPanel.Visibility = Visibility.Hidden;
            helpPanel.Visibility = Visibility.Visible;
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e) {
            mainPanel.Visibility = Visibility.Hidden;
            settingsPanel.Visibility = Visibility.Visible;
            applicationsPanel.Visibility = Visibility.Hidden;
            helpPanel.Visibility = Visibility.Hidden;
            if (recentName != "")
            {
                F22.Content = recentName;
            }

        }

        internal void F22ButtonClicked(object sender, RoutedEventArgs e) {
            if (applicationInfo.ContainsKey((string)F22.Content))
            {
                ((App)Application.Current).ButtonClickedKeyPress(Key.F22, applicationInfo[(string)F22.Content]+ "\\" + (string)F22.Content);
            }
        }

        internal void F23ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F23);
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        internal void F24ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F24);
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
            addAppLocation.GotFocus += TextBox_GotFocus;
            addAppText.GotFocus += TextBox_GotFocus;
            currentName.Text = "Current Application Name: " + t;
            currentLocation.Text = "Current Application Location: " + tt;
        }
        private void applicationsButtonClick(object sender, RoutedEventArgs e)
        {
            applicationsPanel.Visibility = Visibility.Visible;
            settingsPanel.Visibility = Visibility.Hidden;
        }
        
    }
}
