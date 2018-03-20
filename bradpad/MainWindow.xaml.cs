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
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace bradpad {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

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
            ActiveAppDetector.SetUpApplicationDetector();
            foreGroundCheckBox.IsChecked = Topmost;
            MessageBox.Show(tutorialText, tutorialCaption);
            UpdateMainWindow();
        }

        internal void HighlightButton(Key button, bool setPressed) {
            Button pressedButton = null;
            switch (button) {
                case App.F22:
                    pressedButton = F22;
                    break;
                case App.F23:
                    pressedButton = F23;
                    break;
                case App.F24:
                    pressedButton = F24;
                    break;
                default:
                    // We should never enter this state.
                    throw new Exception();
            }

            // Use reflection to make the button appear to be pressed on keypress.  Kind of hacky but I can't find a better way to do this.
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(pressedButton, new object[] { setPressed });
        }

        internal void UpdateMainWindow() {
            F22.Content = app.GetAction(App.F22);
            F23.Content = app.GetAction(App.F23);
            F24.Content = app.GetAction(App.F24);
        }

        internal void UpdateSettingsButtonsContent() {
            if (appDropdown.Text == "Select an Application") {
                F22Settings.Content = "";
                F23Settings.Content = "";
                F24Settings.Content = "";
            } else {
                string actionApp = appDropdown.Text;
                F22Settings.Content = app.GetAction(actionApp, App.F22);
                F23Settings.Content = app.GetAction(actionApp, App.F23);
                F24Settings.Content = app.GetAction(actionApp, App.F24);
            }
        }

        // Main Panel
        private void F22ButtonClicked(object sender, RoutedEventArgs e) {
            app.ButtonClickedKeyPress(App.F22);
        }

        private void F23ButtonClicked(object sender, RoutedEventArgs e) {
            app.ButtonClickedKeyPress(App.F23);
        }

        private void F24ButtonClicked(object sender, RoutedEventArgs e) {
            app.ButtonClickedKeyPress(App.F24);
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e) {
            mainPanel.Visibility = Visibility.Hidden;
            settingsPanel.Visibility = Visibility.Visible;
            FillDropDownActions();
            UpdateSettingsButtonsContent();
            //applicationsPanel.Visibility = Visibility.Hidden;
        }

        // Settings Panel
        private void F22SettingsClicked(object sender, RoutedEventArgs e) {
            F22Settings.Opacity = 1;
            F23Settings.Opacity = 0.2;
            F24Settings.Opacity = 0.2;
            ShowActionFooter();
        }

        private void F23SettingsClicked(object sender, RoutedEventArgs e) {
            F22Settings.Opacity = 0.2;
            F23Settings.Opacity = 1;
            F24Settings.Opacity = 0.2;
            ShowActionFooter();
        }

        private void F24SettingsClicked(object sender, RoutedEventArgs e) {
            F22Settings.Opacity = 0.2;
            F23Settings.Opacity = 0.2;
            F24Settings.Opacity = 1;
            ShowActionFooter();
        }

        private void ForegroundCheckBoxClicked(object sender, RoutedEventArgs e) {
            Topmost = (bool)foreGroundCheckBox.IsChecked;
        }

        private void HelpButtonClicked(object sender, RoutedEventArgs e) {
            MessageBox.Show(tutorialText, tutorialCaption);
        }

        private void MainButtonClicked(object sender, RoutedEventArgs e) {
            mainPanel.Visibility = Visibility.Visible;
            settingsPanel.Visibility = Visibility.Hidden;
            UpdateMainWindow();
        }

        private void ShowActionFooter() {
            settingsActionFooter.Visibility = Visibility.Visible;
            settingsFooter.Visibility = Visibility.Hidden;
            settingsConfigureFooter.Visibility = Visibility.Hidden;
            FillDropDownActions();
            //settingsRowFButtons.Height = new GridLength(2, GridUnitType.Star);
            //settingsRowFooter.Height = new GridLength(7, GridUnitType.Star);
        }

        // settingsActionFooter
        private void ActionSubmitButtonClicked(object sender, RoutedEventArgs e) {
            string s = actionDropdown.Text;
            SetActionFromDropDown(s);
            UpdateMainWindow();
            UpdateSettingsButtonsContent();
            ReturnToSettings(sender, e);
        }

        private void AddActionButtonClick(object sender, RoutedEventArgs e) {
            RestoreDefaultForConfigScreen();
            settingsActionFooter.Visibility = Visibility.Hidden;
            settingsConfigureFooter.Visibility = Visibility.Visible;
        }

        private void appDropdownClosed(object sender, EventArgs e) {
            // TODO: create FillDropDownApps that initializes saveButton.IsEnabled to false
            if (appDropdown.Text != "Select an Application") {
                saveButton.IsEnabled = true;
            }
        }

        private void FillDropDownActions() {
            actionDropdown.Items.Clear();
            actionDropdown.Items.Add(new ComboBoxItem {
                Content = "Select an Action",
                IsSelected = true,
                Visibility = Visibility.Collapsed
            });
            foreach (string action in app.GetActions()) {
                actionDropdown.Items.Add(action);
            }
        }

        private void ReturnToSettings(object sender, RoutedEventArgs e) {
            settingsActionFooter.Visibility = Visibility.Hidden;
            settingsConfigureFooter.Visibility = Visibility.Hidden;
            settingsFooter.Visibility = Visibility.Visible;
            F22Settings.Opacity = 1;
            F23Settings.Opacity = 1;
            F24Settings.Opacity = 1;
        }

        // settingsConfigureFooter
        private void AppSpecificCheckBoxClicked(object sender, RoutedEventArgs e) {

        }

        private void CancelNewActionButtonClick(object sender, RoutedEventArgs e) {
            ShowActionFooter();
        }

        private void CustomInputClick(object sender, RoutedEventArgs e) {
            customInputTextBox.Text = "";
        }

        private void OpenAppCheckBoxClicked(object sender, RoutedEventArgs e) {
            if (openAppCheckBox.IsChecked == true) {
                customInputTextBox.Text = "Enter Application";
            } else {
                customInputTextBox.Text = "Enter Keyboard Shortcut";
            }
        }

        private void RestoreDefaultForConfigScreen() {
            openAppCheckBox.IsChecked = false;
            appSpecificCheckBox.IsChecked = false;
            customInputTextBox.Text = "Enter Keyboard Shortcut";
        }

        private void NewAction(bool temp) {
            string actionApp = appDropdown.Text;
            string name = customInputTextBox.Text;
            string action = customInputTextBox.Text;
            app.AddAction(actionApp, name, action, openAppCheckBox.IsChecked == true);
            if (temp) {
                app.AddTempAction(actionApp, name, temp);
            }
            SetActionFromDropDown(name);
            UpdateMainWindow();
            UpdateSettingsButtonsContent();
            FillDropDownActions();

        }

        private void SaveNewActionButtonClick(object sender, RoutedEventArgs e) {
            NewAction(true);
            ReturnToSettings(sender, e);
        }

        private void SavePermanentButtonClick(object sender, RoutedEventArgs e) {
            NewAction(false);
            ReturnToSettings(sender, e);
        }

        private void SetActionFromDropDown(string action) {
            if (F22Settings.Opacity == 1) {
                app.SetAction(appDropdown.Text, App.F22, action);
            }

            if (F23Settings.Opacity == 1) {
                app.SetAction(appDropdown.Text, App.F23, action);
            }

            if (F24Settings.Opacity == 1) {
                app.SetAction(appDropdown.Text, App.F24, action);
            }
        }
    }
}
