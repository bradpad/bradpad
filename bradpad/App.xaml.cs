using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ownskit.Utils;

namespace bradpad {
    internal enum Mode { Apps, Main, Settings }
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        internal const Key F22 = Key.F22;
        internal const Key F23 = Key.F23;
        internal const Key F24 = Key.F24;

        AppActions appActions;
        KeyboardListener KListener = new KeyboardListener();
        Dictionary<Key, bool> pressed = new Dictionary<Key, bool>() {
            {F22, false},
            {F23, false},
            {F24, false}
        };
        Mode mode = Mode.Main;


        // This function will be called from MainWindow to send keypresses when the buttons are clicked on the screen.
        internal void ButtonClickedKeyPress(Key key) {
            SendKeyPress(key);
        }

        internal void AddAction(string app, string name, string val, bool appFlag, bool temp) {
            appActions.AddAction(app, name, val, appFlag, temp);
        }

        internal byte ContainsAction(string app, string name) {
            return appActions.ContainsAction(app, name);
        }

        internal string GetAction(Key key) {
            return appActions.GetAction(key);
        }

        internal string GetAction(string app, Key key) {
            return appActions.GetAction(app, key);
        }

        internal List<string> GetActions(string app) {
            return appActions.GetActions(app);
        }

        internal List<KeyValuePair<string, string>> GetApplications() {
            return appActions.GetApplications();
        }

        internal string GetApplication() {
            return appActions.GetApplication();
        }

        internal bool IsActiveAction(string app, string action) {
            return appActions.IsActiveAction(app, action);
        }

        internal void LoadSettings() {
            if (!File.Exists("settings.json")) {
                appActions = new AppActions(
                    new Dictionary<string, KeyMap>() {
                        {AppActions.DEFAULT, new KeyMap()},
                        {@"C:\WINDOWS\Explorer.EXE".ToLower(), new KeyMap()},
                        {@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe".ToLower(), new KeyMap()}
                    },
                    new Dictionary<string, string> {
                        { AppActions.DEFAULT, "All Applications" },
                        { @"C:\WINDOWS\Explorer.EXE".ToLower(), "Windows Explorer" },
                        { @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe".ToLower(), "Google Chrome" }
                    }
                );
                ((MainWindow)Current.MainWindow).Topmost = true;
                SaveSettings();
            } else {
                try {
                    string text = File.ReadAllText("settings.json");
                    JObject settings = JObject.Parse(text);
                    appActions = (AppActions)settings["AppActions"].ToObject(typeof(AppActions));
                    ((MainWindow)Current.MainWindow).Topmost = (bool)settings["Foreground"].ToObject(typeof(bool));
                } catch (NullReferenceException) {
                    File.Delete("settings.json");
                    LoadSettings();
                }
            }
        }

        internal void LoadSettings(AppActions inAppActions) {
            appActions = inAppActions;
        }

        internal void SaveSettings() {
            Dictionary<string, object> settings = new Dictionary<string, object> {
                {"AppActions", appActions},
                {"Foreground", ((MainWindow)Current.MainWindow).Topmost}
            };
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter("settings.json"))
            using (JsonWriter writer = new JsonTextWriter(sw)) {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, settings);
            }
        }

        internal void SetAction(string app, Key key, string action) {
            appActions.SetAction(app, key, action);
        }

        internal void RemoveAction(string app, string action) {
            appActions.RemoveAction(app, action);
        }

        internal void SetCurrentApplication(string inCurrentApplication) {
            appActions.SetCurrentApplication(inCurrentApplication);
            MainWindow mainWindow = (MainWindow)Current.MainWindow;
            if (mainWindow != null && mainWindow.IsLoaded) {
                ((MainWindow)Current.MainWindow).UpdateMainWindow();
            }
        }

        internal void SetMode(Mode inMode) {
            mode = inMode;
        }

        private void ApplicationStartup(object sender, StartupEventArgs e) {
            ActiveAppDetector.SetUpApplicationDetector();
            KListener.KeyDown += new RawKeyEventHandler(KListenerKeyDown);
            KListener.KeyUp += new RawKeyEventHandler(KListenerKeyUp);
        }

        private void ApplicationExit(object sender, ExitEventArgs e) {
            KListener.Dispose();
        }

        private bool ContainsKey(Key key) {
            return key == F22 || key == F23 || key == F24;
        }

        private void KListenerKeyDown(object sender, RawKeyEventArgs args) {
            Console.WriteLine(args.Key.ToString());

            if (ContainsKey(args.Key) && !pressed[args.Key]) {
                MainWindow mainWindow = ((MainWindow)Current.MainWindow);
                mainWindow.HighlightButton(args.Key, true);
                pressed[args.Key] = true;
                switch (mode) {
                    case Mode.Main:
                        SendKeyPress(args.Key);
                        break;
                    case Mode.Settings:
                        mainWindow.SettingsPedalPress(args.Key);
                        break;
                }
            }
        }

        private void KListenerKeyUp(object sender, RawKeyEventArgs args) {
            Console.WriteLine(args.Key.ToString());

            if (ContainsKey(args.Key)) {
                pressed[args.Key] = false;
                ((MainWindow)Current.MainWindow).HighlightButton(args.Key, false);
            }
        }

        private void SendKeyPress(Key key) {
            if (appActions.IsApp(key)) {
                try {
                    Process.Start(appActions.GetVal(key));
                } catch {
                    Console.WriteLine("Application opening error.");
                }
            } else {
                System.Windows.Forms.SendKeys.SendWait(appActions.GetVal(key));
            }
        }

        internal void InsertApplication(string name, string path) {
            appActions.InsertApplication(name, path);
        }

        internal void RemoveApplication(string name, string path) {
            appActions.RemoveApplication(name, path);
        }
    }
}
