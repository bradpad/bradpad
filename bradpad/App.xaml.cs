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

        internal static List<string> ACTIONS = new List<string>() {
                {"Open Word"},
                {"Copy"},
                {"Paste"},
                {"Open Chrome"},
                {"New Tab"},
        };

        internal const Key F22 = Key.F22;
        internal const Key F23 = Key.F23;
        internal const Key F24 = Key.F24;

        class KeyMap {
            Dictionary<Key, string> keyDict = new Dictionary<Key, string>() {
                {F22, "Open Word"},
                {F23, "Copy"},
                {F24, "Paste"},
            };

            // Action name, plus pair of action and whether it is an app
            Dictionary<string, Tuple<string, bool>> allActions = new Dictionary<string, Tuple<string, bool>>()
            {
                {"Open Word", Tuple.Create("winword.exe", true)},
                {"Copy", Tuple.Create("^c", false)},
                {"Paste", Tuple.Create("^v", false)},
                {"Open Chrome", Tuple.Create("chrome.exe", true)},
                {"New Tab", Tuple.Create("^t", false)},
            };


            internal void AddAction(string name, string val, bool appFlag) {
                ACTIONS.Add(name);
                allActions[name] = Tuple.Create(val, appFlag);
            }

            internal bool ContainsKey(Key key) {
                return key == F22 || key == F23 || key == F24;
            }

            internal string GetAction(Key key) {
                return keyDict[key];
            }

            internal string GetVal(Key key) {
                return allActions[keyDict[key]].Item1;
            }

            internal bool IsApp(Key key) {
                return allActions[keyDict[key]].Item2;
            }

            internal void SetAction(Key key, string val)
            {
                keyDict[key] = val;
            }
        }


        // We want member variable dictionaries rather than a functions so we can change the mappings at runtime.
        KeyMap keyMap = new KeyMap();

        KeyboardListener KListener = new KeyboardListener();


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

        internal void SetAction(Key key, string action) {
            keyMap.SetAction(key, action);
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
                } catch {
                    Console.WriteLine("Application opening error.");
                }
            } else {
                System.Windows.Forms.SendKeys.SendWait(keyMap.GetVal(key));
            }
        }
    }
}
