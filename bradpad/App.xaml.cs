using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Text;

using Ownskit.Utils;

namespace bradpad {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        internal const Key F22 = Key.F22;
        internal const Key F23 = Key.F23;
        internal const Key F24 = Key.F24;

        AppActions appActions = new AppActions(new Dictionary<string, KeyMap>() {
            {@"C:\WINDOWS\Explorer.EXE", new KeyMap()},
            {@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", new KeyMap()}
        });
        //KeyMap keyMap = new KeyMap();
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

        internal void AddAction(string app, string name, string val, bool appFlag) {
            appActions.AddAction(app, name, val, appFlag);
        }

        internal string GetAction(Key key) {
            return appActions.GetAction(key);
        }

        internal string GetAction(string app, Key key) {
            return appActions.GetAction(app, key);
        }

        internal List<string> GetActions() {
            return appActions.GetActions();
        }

        internal void SetAction(string app, Key key, string action) {
            appActions.SetAction(app, key, action);
        }

        internal void AddTempAction(string app, string name, bool temp) {
            appActions.AddTempAction(app, name, temp);
        }

        internal void SetCurrentApplication(string currentApplicationIn) {
            appActions.SetCurrentApplication(currentApplicationIn);
            MainWindow mainWindow = (MainWindow)Current.MainWindow;
            if (mainWindow != null) {
                ((MainWindow)Current.MainWindow).UpdateMainWindow();
            }
        }

        private void ApplicationStartup(object sender, StartupEventArgs e) {
            ActiveAppDetector.SetUpApplicationDetector();
            KListener.KeyDown += new RawKeyEventHandler(KListenerKeyDown);
            KListener.KeyUp += new RawKeyEventHandler(KListenerKeyUp);
        }

        private void ApplicationExit(object sender, ExitEventArgs e) {
            KListener.Dispose();
        }

        internal bool ContainsKey(Key key) {
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
