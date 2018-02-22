using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
        private App app = ((App)Application.Current);

        private string tutorialCaption = "bradpad Help Screen and Tutorial";
        private string tutorialText =
                "Welcome to the alpha release of bradpad! Right now, this product is designed to be able to open Microsoft Word and execute copy and paste instructions. The buttons on the Arduino hardware act as pedals. A step-by-step tutorial is below. In the upcoming releases, we hope to increase functionality and allow users to add custom commands. We hope you enjoy!\n\n" +
                "1. Plug the Arduino and pedal system into the computer.  The computer will detect them as a keyboard.\n" +
                "2. Pressing the left pedal will open Microsoft Word.\n" +
                "3. Pressing the middle pedal corresponds to a copy (CTRL + C shortcut) command.\n" +
                "4. Pressing the right pedal corresponds to a paste (CTRL + V shortcut) command.\n\n" +
                "If you get lost, you can press the Help button in the Settings view to view these instructions again.\n\n" +
                "The application also has the capability to customize the application that is mapped to the left pedal, but the UI is a undeveloped because this flow will be changed in the future.  This is a stretch goal implemented from our beta release goals.\n\n" +
                "Opening an Application\n" +
                "1. In the settings view, navigate to the applications view.\n" +
                "2. Input a name for the application in the \"Enter Application Name\" field.\n" +
                "3. Enter the application location. This can be found by opening the application location through file explorer, and right clicking the address. Copy this and paste it into the application where it says \"Enter Application Location\".\n" +
                "4. Click the \"Enter\" button. This triggers the left pedal (or clicking the left panel on the main screen) to open the desired application.\n" +
                "5. The process can be repeated to change the desired application.";

        public MainWindow() {
            InitializeComponent();
            foreGroundCheckBox.IsChecked = Topmost;
            //recentName = "";
            MessageBox.Show(tutorialText, tutorialCaption);
        }

        // Main Panel
        internal void F22ButtonClicked(object sender, RoutedEventArgs e) {
            app.ButtonClickedKeyPress(App.F22);
            //if (applicationInfo.ContainsKey((string)F22.Content))
            //{
            //    app.ButtonClickedKeyPress(App.F22, applicationInfo[(string)F22.Content]+ "\\" + (string)F22.Content);
            //}
        }

        internal void F23ButtonClicked(object sender, RoutedEventArgs e) {
            app.ButtonClickedKeyPress(App.F23);
        }

        internal void F24ButtonClicked(object sender, RoutedEventArgs e) {
            app.ButtonClickedKeyPress(App.F24);
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
            //settingsPanel.Visibility = Visibility.Hidden;
            //helpPanel.Visibility = Visibility.Visible;
            MessageBox.Show(tutorialText, tutorialCaption);
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
            app.SetMapping(App.F22, tt, true);
            F22.Content = t;
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
