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
using Ownskit.Utils;

namespace bradpad {
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


        // This function will be called from MainWindow to send keypresses when the buttons are clicked on the screen.
        internal void ButtonClickedKeyPress(Key key) {
            SendKeyPress(key);
        }

        internal void AddAction(string app, string name, string val, bool appFlag, bool temp) {
            appActions.AddAction(app, name, val, appFlag, temp);
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

        internal void SetAction(string app, Key key, string action) {
            appActions.SetAction(app, key, action);
            SaveSettings();
        }

        internal void SetCurrentApplication(string inCurrentApplication) {
            appActions.SetCurrentApplication(inCurrentApplication);
            MainWindow mainWindow = (MainWindow)Current.MainWindow;
            if (mainWindow != null && mainWindow.IsLoaded) {
                ((MainWindow)Current.MainWindow).UpdateMainWindow();
            }
        }

        private void ApplicationStartup(object sender, StartupEventArgs e) {
            ActiveAppDetector.SetUpApplicationDetector();
            KListener.KeyDown += new RawKeyEventHandler(KListenerKeyDown);
            KListener.KeyUp += new RawKeyEventHandler(KListenerKeyUp);
            LoadSettings();
        }

        private void ApplicationExit(object sender, ExitEventArgs e) {
            KListener.Dispose();
        }

        private bool ContainsKey(Key key) {
            return key == App.F22 || key == App.F23 || key == App.F24;
        }

        private void KListenerKeyDown(object sender, RawKeyEventArgs args) {
            Console.WriteLine(args.Key.ToString());

            if (ContainsKey(args.Key) && !pressed[args.Key]) {
                ((MainWindow)Current.MainWindow).HighlightButton(args.Key, true);
                pressed[args.Key] = true;
                SendKeyPress(args.Key);
            }
        }

        private void KListenerKeyUp(object sender, RawKeyEventArgs args) {
            Console.WriteLine(args.Key.ToString());

            if (ContainsKey(args.Key)) {
                pressed[args.Key] = false;
                ((MainWindow)Current.MainWindow).HighlightButton(args.Key, false);
            }
        }

        private void LoadSettings() {
            if (File.Exists("settings.json")) {
                using (StreamReader file = File.OpenText("settings.json")) {
                    JsonSerializer serializer = new JsonSerializer();
                    appActions = (AppActions)serializer.Deserialize(file, typeof(AppActions));
                }
            } else {
                appActions = new AppActions(new Dictionary<string, KeyMap>() {
                    {AppActions.DEFAULT, new KeyMap()},
                    {@"C:\WINDOWS\Explorer.EXE", new KeyMap()},
                    {@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", new KeyMap()}
                });
            }
        }

        private void SaveSettings() {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter("settings.json"))
            using (JsonWriter writer = new JsonTextWriter(sw)) {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, appActions);
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
    }
}
