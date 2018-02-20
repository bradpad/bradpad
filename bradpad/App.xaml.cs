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

        // We want keyMap as a member variable rather than a function so we can change the mappings at runtime.
        Dictionary<Key, string> keyMap = new Dictionary<Key, string>() {
            {Key.F10, ""},
            {Key.F11, "^c"},
            {Key.F12, "^v"},
        };
        KeyboardListener KListener = new KeyboardListener();


        // This function will be called from MainWindow to send keypresses when the buttons are clicked on the screen.
        internal void ButtonClickedKeyPress(Key key, string pname) {
            if (keyMap.ContainsKey(key)) {
                SendKeyPress(key);
                Process p = new Process();
                p.StartInfo.FileName = pname;
                p.Start();
            }
        }

        internal void ButtonClickedKeyPress(Key key)
        {
            if (keyMap.ContainsKey(key))
            {
                SendKeyPress(key);
            }
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
                case Key.F10:
                    pressedButton = ((MainWindow)Current.MainWindow).F10;
                    break;
                case Key.F11:
                    pressedButton = ((MainWindow)Current.MainWindow).F11;
                    break;
                case Key.F12:
                    pressedButton = ((MainWindow)Current.MainWindow).F12;
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

                MainWindow window = (bradpad.MainWindow)Application.Current.MainWindow;
                object o = new object();
                RoutedEventArgs e = new RoutedEventArgs();
                if (args.Key == Key.F10)
                {   
                    window.F10ButtonClicked(o, e);
                }
                if (args.Key == Key.F11)
                {
                    window.F11ButtonClicked(o, e);
                }
                if (args.Key == Key.F12)
                {
                    window.F12ButtonClicked(o, e);
                }
            }
        }

        private void KListenerKeyUp(object sender, RawKeyEventArgs args) {
            if (keyMap.ContainsKey(args.Key)) {
                HighlightButton(args.Key, false);
            }
        }

        private void SendKeyPress(Key key) {
            System.Windows.Forms.SendKeys.SendWait(keyMap[key]);
        }
    }
}
