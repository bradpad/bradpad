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

        //Dictionary<string, string> applicationInfo = new Dictionary<string, string>();
        //string recentName;

        public MainWindow() {
            InitializeComponent();
            foreGroundCheckBox.IsChecked = Topmost;
            //recentName = "";
            MessageBox.Show(
                "Welcome to the alpha release of bradpad! Right now, this product is designed to do copy and paste instructions and open a desired application. The buttons on the Arduino hardware expedite this procedure. A step-by-step tutorial is below. In the upcoming releases, we hope to be able to have more functionality, as well as being able to add custom commands. We hope you enjoy!\n\n" +
                "Opening an Application\n" +
                "1. The first function is the ability to open up applications.\n" +
                "2. In the settings view, navigate to the applications view.\n" +
                "3. Input a name for the application in the \"Enter Application Name\" field.\n" +
                "4. Enter the application location. This can be found by opening the application location through file explorer, and right clicking the address. Copy this and paste it into the application where it says \"Enter Application Location\".\n" +
                "5. Click the \"Enter\" button. This triggers the left pedal (or clicking the left panel on the main screen) to open the desired application.\n" +
                "6. The process can be repeated to change the desired application.\n\n" +
                "Copy and Paste\n" +
                "1. The middle pedal corresponds to copy (the ctrl+c shortcut).\n" +
                "2. The right pedal corresponds to paste (the ctrl+v shortcut).\n\n" +
                "If you get lost, you can press the Help button in the Settings view to view these instructions again.",
                "bradpad Help Screen and Tutorial"
            );
        }

        // Main Panel
        internal void F22ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F22);
            //if (applicationInfo.ContainsKey((string)F22.Content))
            //{
            //    ((App)Application.Current).ButtonClickedKeyPress(Key.F22, applicationInfo[(string)F22.Content]+ "\\" + (string)F22.Content);
            //}
        }

        internal void F23ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F23);
        }

        internal void F24ButtonClicked(object sender, RoutedEventArgs e) {
            ((App)Application.Current).ButtonClickedKeyPress(Key.F24);
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e) {
            mainPanel.Visibility = Visibility.Hidden;
            settingsPanel.Visibility = Visibility.Visible;
            applicationsPanel.Visibility = Visibility.Hidden;
            helpPanel.Visibility = Visibility.Hidden;
            //if (recentName != "")
            //{
            //    F22.Content = recentName;
            //}
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
        
        private void ForegroundCheckBoxClicked(object sender, RoutedEventArgs e) {
            if(foreGroundCheckBox.IsChecked == false) {
                Topmost = false;
            } else {
                Topmost = true;
            }
        }
        private void appButtonClicked(object sender, RoutedEventArgs e)
        {
            string t = addAppText.Text;
            string tt = addAppLocation.Text;
            addAppText.Text = "Enter Application Name";
            addAppLocation.Text = "Enter Application Location";
            //applicationInfo[t] = tt;
            //recentName = t;
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

        public void TextBox_GotFocus(object sender, RoutedEventArgs e) {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }
    }
}
