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

        Dictionary<string, KeyMap> appActions = new Dictionary<string, KeyMap>() {
            {@"C:\WINDOWS\Explorer.EXE", new KeyMap()},
            {@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", new KeyMap()}
        };
        string currentApplication = @"C:\WINDOWS\Explorer.EXE";
        KeyMap keyMap = new KeyMap();
        KeyboardListener KListener = new KeyboardListener();
        MainWindow mainWindow = ((MainWindow)Current.MainWindow);


        // This function will be called from MainWindow to send keypresses when the buttons are clicked on the screen.
        internal void ButtonClickedKeyPress(Key key) {
            SendKeyPress(key);
        }

        internal void AddAction(string name, string val, bool appFlag) {
            keyMap.AddAction(name, val, appFlag);
        }

        internal string GetAction(Key key) {
            return keyMap.GetAction(key);
        }

        internal List<string> GetActions() {
            return keyMap.GetActions();
        }

        internal void SetAction(Key key, string action) {
            keyMap.SetAction(key, action);
        }

        internal void AddTemp(string name, bool temp) {
            keyMap.AddTemp(name, temp);
        }

        internal bool IsTemp(string s) {
            return keyMap.IsTemp(s);
        }

        internal void SetCurrentApplication(string currentApp) {
            currentApplication = currentApp;
            mainWindow.UpdateMainWindow();
            mainWindow.UpdateSettingsButtonsContent();
        }

        private void ApplicationStartup(object sender, StartupEventArgs e) {
            KListener.KeyDown += new RawKeyEventHandler(KListenerKeyDown);
            KListener.KeyUp += new RawKeyEventHandler(KListenerKeyUp);
        }

        private void ApplicationExit(object sender, ExitEventArgs e) {
            KListener.Dispose();
        }

        private void KListenerKeyDown(object sender, RawKeyEventArgs args) {
            Console.WriteLine(args.Key.ToString());

            if (keyMap.ContainsKey(args.Key)) {
                mainWindow.HighlightButton(args.Key, true);
                SendKeyPress(args.Key);
            }
        }

        private void KListenerKeyUp(object sender, RawKeyEventArgs args) {
            if (keyMap.ContainsKey(args.Key)) {
                mainWindow.HighlightButton(args.Key, false);
            }
        }

        private void SendKeyPress(Key key) {
            if (keyMap.IsApp(key)) {
                try {
                    Process.Start(keyMap.GetVal(key));
                } catch {
                    Console.WriteLine("Application opening error.");
                }
            } else {
                System.Windows.Forms.SendKeys.SendWait(keyMap.GetVal(key));
            }
        }
    }
}
