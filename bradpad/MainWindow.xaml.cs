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
using Microsoft.Win32;

namespace bradpad {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private App app = ((App)Application.Current);

        private string tutorialCaption = "bradpad Help Screen and Tutorial";
        private string tutorialText =
                "Welcome to the beta release of bradpad! Right now, this product is designed to be able to execute desired commands based off a few provided default commands, as well as being able to configure pedal to user-specific commands. Also, we have internal active application recognizing capabilities- this will be used to add actions for each application. For the omega release, we hope to be able to save state after application closure, as well as build upon our application recognizing capabilities to have commands specific for each application. Additionally, we will work more with Brad to ensure that all the UI is easy to use.\n\n" +
                "1. Plug the Arduino and pedal system into the computer.  The computer will detect this as a keyboard.\n" +
                "2. There are default commands provided- open word, copy, and paste.\n" +
                "3. Pressing the pedals will trigger the corresponding action on the Main screen of bradpad.\n" +
                "If you get lost, you can press the Help button in the Settings view to view these instructions again.\n\n" +
                "The application also has the capability to customize the application that is mapped to the left pedal, but the UI is a undeveloped because this flow will be changed in the future.  This is a stretch goal implemented from our beta release goals.\n\n" +
                "Configuring bradpad\n" +
                "1. Navigate to settings view by clicking Open button\n" +
                "2. Click the top dropdown menu- the first option is for global applications except Chrome and the second option is for Chrome.\n" +
                "3. Click a pedal button.\n" +
                "4. Select an Action dropdown gives you options for that pedal.\n" +
                "5. Click Add Action to add an action to the dropdown\n" +
                "6. Check the checkbox Open App only if the input is an application.\n" +
                "7. Enter the keyboard shortcut or the application executable.\n" +
                "8. Click either Save to save this command temporarily or Save Permanent to add this command in future action dropboxes.\n";

        public MainWindow() {
            InitializeComponent();
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
            UpdateSettingsButtonsContent((string)appDropdown.SelectedValue);
        }

        internal void FillAllCurrentApplications()
        {
            AllApplicationsList.Items.Clear();
            Dictionary<string, string> allApps = app.GetApplications();
            foreach(var i in allApps)
            {
                AllApplicationsList.Items.Add(new ListBoxItem {
                    Content = i.Value,
                    Tag = i.Key,
                });
            }
        }

        internal void FillAvailableApplications()
        {
            AvailableApplications.Items.Clear();
            /*string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        AvailableApplications.Items.Add(new ComboBoxItem
                        {
                            Content = subkey.GetValue("DisplayName"),
                        });
                    }
                }
            }*/
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey subKey1 = regKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
            string[] subKeyNames = subKey1.GetSubKeyNames();

            foreach (string subKeyName in subKeyNames)
            {
                Microsoft.Win32.RegistryKey subKey2 = subKey1.OpenSubKey(subKeyName);

                if (ValueNameExists(subKey2.GetValueNames(), "DisplayName") &&
                ValueNameExists(subKey2.GetValueNames(), "DisplayVersion"))
                {
                    /*AvailableApplications.Items.Add(new ListViewItem(new string[]{
                    subKey2.GetValue("DisplayName").ToString(),
                        subKey2.GetValue("DisplayVersion").ToString()
                    }));*/
                    AvailableApplications.Items.Add(new ComboBoxItem
                    {
                        Content = subKey2.GetValue("DisplayName").ToString(),
                    });
                }

                subKey2.Close();
            }

            subKey1.Close();
        }

        private bool ValueNameExists(string[] valueNames, string valueName)
        {
            foreach (string s in valueNames)
            {
                if (s.ToLower() == valueName.ToLower()) return true;
            }

            return false;
        }

        internal void UpdateSettingsButtonsContent(string actionApp) {
            F22Settings.Content = app.GetAction(actionApp, App.F22);
            F23Settings.Content = app.GetAction(actionApp, App.F23);
            F24Settings.Content = app.GetAction(actionApp, App.F24);
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
            // Make sure FillDropDownApps is called before UpdateSettingsButtonsContent or else app will crash
            FillDropDownApps();
            UpdateSettingsButtonsContent();
            saveNewActionButton.IsEnabled = false;
            savePermanentButton.IsEnabled = false;
            //applicationsPanel.Visibility = Visibility.Hidden;
        }

        // Settings Panel
        private void AppDropdownSelectionChanged(object sender, EventArgs e) {
            ComboBoxItem selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            if (selectedItem != null && selectedItem.IsEnabled) {
                // TODO: create FillDropDownApps that initializes saveButton.IsEnabled to false
                string actionApp = (string)selectedItem.Tag;
                ComboBoxItem actionItem = (ComboBoxItem)actionDropdown.SelectedItem;
                if (actionItem != null && actionItem.IsEnabled) {
                    saveButton.IsEnabled = true;
                }
                saveNewActionButton.IsEnabled = true;
                savePermanentButton.IsEnabled = true;
                UpdateSettingsButtonsContent(actionApp);
                FillDropDownActions(actionApp);
            }
        }

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

        private void FillDropDownApps() {
            appDropdown.Items.Clear();
            /*appDropdown.Items.Add(new ComboBoxItem {
                Content = "Select an Application",
                IsEnabled = false,
                IsSelected = true,
                Tag = AppActions.EMPTY,
                Visibility = Visibility.Collapsed
            });
            */

            // TODO: replace these with a loop that adds applications that Brad wants
            /*appDropdown.Items.Add(new ComboBoxItem {
                Content = "All Applications",
                Tag = AppActions.DEFAULT,
                IsSelected = true,
            });
            appDropdown.Items.Add(new ComboBoxItem {
                Content = "Windows Explorer",
                Tag = @"C:\WINDOWS\Explorer.EXE"
            });
            appDropdown.Items.Add(new ComboBoxItem {
                Content = "Google Chrome",
                Tag = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
            });*/
            Dictionary<string, string> allApps = app.GetApplications();
            foreach(var i in allApps)
            {
                if (i.Value == "All Applications")
                {
                    appDropdown.Items.Add(new ComboBoxItem
                    {
                        Content = i.Value,
                        Tag = i.Key,
                        IsSelected = true,
                    });
                }
                else
                {
                    appDropdown.Items.Add(new ComboBoxItem
                    {
                        Content = i.Value,
                        Tag = i.Key,
                    });
                }
            }
        }

        private void SettingsButtonFromApplicationClicked(object sender, RoutedEventArgs e)
        {
            applicationsPanel.Visibility = Visibility.Hidden;
            settingsPanel.Visibility = Visibility.Visible;
        }

        private void ConfigureAppsButtonClicked(object sender, RoutedEventArgs e)
        {
            FillAvailableApplications();
            FillAllCurrentApplications();
            settingsPanel.Visibility = Visibility.Hidden;
            applicationsPanel.Visibility = Visibility.Visible;
        }

        private void AvailableApplicationsChanged(object sender, RoutedEventArgs e)
        {

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
            FillDropDownActions((string)appDropdown.SelectedValue);
            //settingsRowFButtons.Height = new GridLength(2, GridUnitType.Star);
            //settingsRowFooter.Height = new GridLength(7, GridUnitType.Star);
        }

        // settingsActionFooter
        private void ActionSubmitButtonClicked(object sender, RoutedEventArgs e) {
            string s = actionDropdown.Text;
            SetActionFromDropDown(s);
            UpdateSettingsButtonsContent();
            ReturnToSettings(sender, e);
        }

        private void AddActionButtonClick(object sender, RoutedEventArgs e) {
            RestoreDefaultForConfigScreen();
            settingsActionFooter.Visibility = Visibility.Hidden;
            settingsConfigureFooter.Visibility = Visibility.Visible;
        }

        private void ActionDropdownSelectionChanged(object sender, EventArgs e) {
            ComboBoxItem selectedItem = (ComboBoxItem)actionDropdown.SelectedItem;
            if (selectedItem != null && selectedItem.IsEnabled && ((ComboBoxItem)appDropdown.SelectedItem).IsEnabled) {
                saveButton.IsEnabled = true;
            }
        }

        private void FillDropDownActions(string actionApp) {
            actionDropdown.Items.Clear();
            actionDropdown.Items.Add(new ComboBoxItem {
                Content = "Select an Action",
                IsEnabled = false,
                IsSelected = true,
                Visibility = Visibility.Collapsed
            });
            // Disable save button because we're selecting "Select an Action"
            saveButton.IsEnabled = false;
            foreach (string action in app.GetActions(actionApp)) {
                actionDropdown.Items.Add(new ComboBoxItem {
                    Content = action
                });
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
            //appSpecificCheckBox.IsChecked = false;
            customInputTextBox.Text = "Enter Keyboard Shortcut";
        }

        // TODO: only enable saving actions when user has selected application or entered keyboard shortcut
        private void NewAction(bool temp) {
            string actionApp = (string)appDropdown.SelectedValue;
            string name = customInputTextBox.Text;
            string action = customInputTextBox.Text;
            app.AddAction(actionApp, name, action, openAppCheckBox.IsChecked == true, temp);
            SetActionFromDropDown(name);
            UpdateMainWindow();
            UpdateSettingsButtonsContent();
            FillDropDownActions(actionApp);
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
            string actionApp = (string)appDropdown.SelectedValue;

            if (F22Settings.Opacity == 1) {
                app.SetAction(actionApp, App.F22, action);
            }

            if (F23Settings.Opacity == 1) {
                app.SetAction(actionApp, App.F23, action);
            }

            if (F24Settings.Opacity == 1) {
                app.SetAction(actionApp, App.F24, action);
            }
        }
    }
}
