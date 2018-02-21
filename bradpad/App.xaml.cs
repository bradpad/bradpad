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
            {Key.F22, ""},
            {Key.F23, "^c"},
            {Key.F24, "^v"},
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
                case Key.F22:
                    pressedButton = ((MainWindow)Current.MainWindow).F22;
                    break;
                case Key.F23:
                    pressedButton = ((MainWindow)Current.MainWindow).F23;
                    break;
                case Key.F24:
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

                MainWindow window = (bradpad.MainWindow)Application.Current.MainWindow;
                object o = new object();
                RoutedEventArgs e = new RoutedEventArgs();
                if (args.Key == Key.F22)
                {   
                    window.F22ButtonClicked(o, e);
                }
                if (args.Key == Key.F23)
                {
                    window.F23ButtonClicked(o, e);
                }
                if (args.Key == Key.F24)
                {
                    window.F24ButtonClicked(o, e);
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
