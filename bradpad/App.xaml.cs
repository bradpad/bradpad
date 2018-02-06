﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Ownskit.Utils;

namespace bradpad {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        KeyboardListener KListener = new KeyboardListener();

        private void Application_Startup(object sender, StartupEventArgs e) {
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
        }

        void KListener_KeyDown(object sender, RawKeyEventArgs args) {
            //if (args.ToString().Length > 0 && char.IsLower(args.ToString().ToCharArray()[0])) {
            //    key=args.ToString(); // Prints the text of pressed button, takes in account big and small letters. E.g. "Shift+a" => "A"
            //} else {
            //    Console.WriteLine(args.Key.ToString());
            //}
            Console.WriteLine(args.Key.ToString());
            Console.WriteLine(System.Windows.Input.Key.Separator);
            switch (args.Key) {
                case System.Windows.Input.Key.F22:
                    System.Windows.Forms.SendKeys.SendWait("^t");
                    break;
                case System.Windows.Input.Key.F23:
                    System.Windows.Forms.SendKeys.SendWait("^t");
                    break;
                case System.Windows.Input.Key.F24:
                    System.Windows.Forms.SendKeys.SendWait("^t");
                    break;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e) {
            KListener.Dispose();
        }
    }
}
