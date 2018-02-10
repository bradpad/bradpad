using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Ownskit.Utils;

namespace bradpad {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        Dictionary<Key, string> keyMap = new Dictionary<Key, string>() {
            {Key.F, "^t"},
            {Key.F23, "^t"},
            {Key.F24, "^t"},
        };
        KeyboardListener KListener = new KeyboardListener();


        public void SendKeyPress(Key key) {
            if (keyMap.ContainsKey(key)) {
                System.Windows.Forms.SendKeys.SendWait(keyMap[key]);
            }
        }

        private void ApplicationStartup(object sender, StartupEventArgs e) {
            KListener.KeyDown += new RawKeyEventHandler(KListenerKeyDown);
        }

        private void ApplicationExit(object sender, ExitEventArgs e) {
            KListener.Dispose();
        }

        void KListenerKeyDown(object sender, RawKeyEventArgs args) {
            //if (args.ToString().Length > 0 && char.IsLower(args.ToString().ToCharArray()[0])) {
            //    key=args.ToString(); // Prints the text of pressed button, takes in account big and small letters. E.g. "Shift+a" => "A"
            //} else {
            //    Console.WriteLine(args.Key.ToString());
            //}
            Console.WriteLine(args.Key.ToString());
            SendKeyPress(args.Key);
            //switch (args.Key) {
            //    case System.Windows.Input.Key.F22:
            //        System.Windows.Forms.SendKeys.SendWait("^t");
            //        break;
            //    case System.Windows.Input.Key.F23:
            //        System.Windows.Forms.SendKeys.SendWait("^t");
            //        break;
            //    case System.Windows.Input.Key.F24:
            //        System.Windows.Forms.SendKeys.SendWait("^t");
            //        break;
            //}
        }
    }
}
