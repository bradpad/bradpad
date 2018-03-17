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
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace bradpad {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private App app = ((App)Application.Current);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        static WinEventDelegate dele = null;
        static IntPtr m_hhook;
        private static string currentApplication = "";

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

        private static void SetUpApplicationDetector()
        {
            dele = new WinEventDelegate(WinEventProc);
            m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) //STATIC
        {
            string newApplication = GetActiveWindowTitle();
            if (newApplication == null)
            {
                return;
            }
            if (newApplication.Contains("-"))
            {
                string[] newApplicationSplit = newApplication.Split('-');
                newApplication = newApplicationSplit[newApplicationSplit.Length - 1].Trim();
            }
            if (newApplication != currentApplication)
            {
                currentApplication = newApplication;
                Console.WriteLine("Current app: " + currentApplication);
            }
            // some kind of switch function that will update buttons based on currentApplication
        }

        private static string GetActiveWindowTitle()
        {
            const int nChars = 512;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public void UpdateMainWindow()
        {
            F22.Content = app.GetAction(App.F22);
            F23.Content = app.GetAction(App.F23);
            F24.Content = app.GetAction(App.F24);
        }

        public void UpdateSettingsButtonsContent() {
            F22Settings.Content = app.GetAction(App.F22);
            F23Settings.Content = app.GetAction(App.F23);
            F24Settings.Content = app.GetAction(App.F24);
        }

        public MainWindow() {
            InitializeComponent();
            foreGroundCheckBox.IsChecked = Topmost;
            SetUpApplicationDetector();
            MessageBox.Show(tutorialText, tutorialCaption);
            UpdateMainWindow();
        }

        public void SetActionFromDropDown(string s)
        {
            if (F22Settings.Opacity == 1)
            {
                app.SetAction(App.F22, s);
            }

            if (F23Settings.Opacity == 1)
            {
                app.SetAction(App.F23, s);
            }

            if (F24Settings.Opacity == 1)
            {
                app.SetAction(App.F24, s);
            }
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

        private void FillDropDownActions() {
            actionDropdown.Items.Clear();
            ComboBoxItem selectAnAction = new ComboBoxItem();
            selectAnAction.Content = "Select an Action";
            selectAnAction.IsSelected = true;
            selectAnAction.Visibility = Visibility.Collapsed;
            actionDropdown.Items.Add(selectAnAction);
            //((ComboBoxItem)actionDropdown.Items[0]).Visibility = Visibility.Collapsed;
            foreach (string action in App.ACTIONS) {
                if (app.IsTemp(action) == false)
                {
                    actionDropdown.Items.Add(action);
                }
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

        private void NewAction(bool temp)
        {
            string name = customInputTextBox.Text;
            string action = customInputTextBox.Text;
            app.AddAction(name, action, openAppCheckBox.IsChecked == true);
            app.AddTemp(name, temp);
            SetActionFromDropDown(name);
            UpdateMainWindow();
            UpdateSettingsButtonsContent();
            FillDropDownActions();
            
        }

        private void SaveNewActionButtonClick(object sender, RoutedEventArgs e) {
            NewAction(true);
            ReturnToSettings(sender, e);
        }

        private void SavePermanentButtonClick(object sender, RoutedEventArgs e){
            NewAction(false);
            ReturnToSettings(sender, e);
        }
    }
}
