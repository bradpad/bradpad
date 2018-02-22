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

using Ownskit.Utils;

namespace bradpad {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        internal const Key F22 = Key.F22;
        internal const Key F23 = Key.F23;
        internal const Key F24 = Key.F24;

        class KeyMap {
            Dictionary<Key, bool> isApp = new Dictionary<Key, bool>() {
                {F22, true},
                {F23, false},
                {F24, false},
            };
                Dictionary<Key, string> keyDict = new Dictionary<Key, string>() {
                {F22, "winword.exe"},
                {F23, "^c"},
                {F24, "^v"},
            };

            internal bool ContainsKey(Key key) {
                return key == F22 || key == F23 || key == F24;
            }

            internal string GetVal(Key key) {
                return keyDict[key];
            }

            internal bool IsApp(Key key) {
                return isApp[key];
            }

            internal void SetMapping(Key key, string val, bool appFlag) {
                isApp[key] = appFlag;
                keyDict[key] = val;
            }
        }


        // We want member variable dictionaries rather than a functions so we can change the mappings at runtime.
        Dictionary<string, string> applicationInfo = new Dictionary<string, string>();
        KeyMap keyMap = new KeyMap();
        KeyboardListener KListener = new KeyboardListener();


        // This function will be called from MainWindow to send keypresses when the buttons are clicked on the screen.
        internal void ButtonClickedKeyPress(Key key) {
            SendKeyPress(key);
        }

        internal void SetMapping(Key key, string val, bool appFlag) {
            keyMap.SetMapping(key, val, appFlag);
        }

        private void ApplicationStartup(object sender, StartupEventArgs e) {
            KListener.KeyDown += new RawKeyEventHandler(KListenerKeyDown);
            KListener.KeyUp += new RawKeyEventHandler(KListenerKeyUp);
        }

        private void ApplicationExit(object sender, ExitEventArgs e) {
            KListener.Dispose();
        }

        private void HighlightButton(Key button, bool setPressed) {
            Button pressedButton = null;
            switch (button) {
                case F22:
                    pressedButton = ((MainWindow)Current.MainWindow).F22;
                    break;
                case F23:
                    pressedButton = ((MainWindow)Current.MainWindow).F23;
                    break;
                case F24:
                    pressedButton = ((MainWindow)Current.MainWindow).F24;
                    break;
                default:
                    // We should never enter this state.
                    throw new Exception();
            }

            // Use reflection to make the button appear to be pressed on keypress.  Kind of hacky but I can't find a better way to do this.
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(pressedButton, new object[] { setPressed });
        }

        private void KListenerKeyDown(object sender, RawKeyEventArgs args) {
            Console.WriteLine(args.Key.ToString());

            if (keyMap.ContainsKey(args.Key)) {
                HighlightButton(args.Key, true);
                SendKeyPress(args.Key);
            }
        }

        private void KListenerKeyUp(object sender, RawKeyEventArgs args) {
            if (keyMap.ContainsKey(args.Key)) {
                HighlightButton(args.Key, false);
            }
        }

        private void SendKeyPress(Key key) {
            if (keyMap.IsApp(key)) {
                try {
                    Process.Start(keyMap.GetVal(key));
                    //Process p = new Process();
                    //p.StartInfo.FileName = keyMap.GetVal(key);
                    //p.Start();
                } catch {
                    Console.WriteLine("Application opening error.");
                }
            } else {
                System.Windows.Forms.SendKeys.SendWait(keyMap.GetVal(key));
            }
        }
    }
}
