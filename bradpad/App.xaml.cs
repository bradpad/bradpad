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
using System.Runtime.InteropServices;
using System.Text;

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

            Dictionary<string, bool> tempActions = new Dictionary<string, bool>()
            {
                {"Open Word", false},
                {"Copy", false},
                {"Paste", false },
                {"Open Chrome", false},
                {"New Tab", false},
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

            internal void AddTemp(string name, bool temp) {
                tempActions[name] = temp;
            }

            internal bool IsTemp(string s) {
                return tempActions[s];
            }

            internal void SetAction(Key key, string val) {
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

        internal void AddTemp(string name, bool temp) {
            keyMap.AddTemp(name, temp);
        }

        internal bool IsTemp(string s) {
            return keyMap.IsTemp(s);
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

        private void KListenerKeyDown(object sender, RawKeyEventArgs args) {
            Console.WriteLine(args.Key.ToString());

            if (keyMap.ContainsKey(args.Key)) {
                ((MainWindow)Current.MainWindow).HighlightButton(args.Key, true);
                SendKeyPress(args.Key);
            }
        }

        private void KListenerKeyUp(object sender, RawKeyEventArgs args) {
            if (keyMap.ContainsKey(args.Key)) {
                ((MainWindow)Current.MainWindow).HighlightButton(args.Key, false);
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


        // Detect Active Window
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_SYSTEM_MINIMIZESTART = 22;
        private const uint EVENT_SYSTEM_MINIMIZEEND = 23;
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);
        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        static WinEventDelegate dele0 = null;
        static WinEventDelegate dele1 = null;
        static IntPtr m_hhook0;
        static IntPtr m_hhook1;
        private static string currentApplication = "";

        public static void SetUpApplicationDetector() {
            dele0 = new WinEventDelegate(WinEventProc);
            dele1 = new WinEventDelegate(WinEventProc);
            m_hhook0 = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele0, 0, 0, WINEVENT_OUTOFCONTEXT);
            m_hhook1 = SetWinEventHook(EVENT_SYSTEM_MINIMIZEEND, EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, dele1, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) //STATIC
        {
            string newApplication = GetActiveProcessName();
            if (newApplication == null) {
                return;
            }
            if (newApplication.Contains("-")) {
                string[] newApplicationSplit = newApplication.Split('-');
                newApplication = newApplicationSplit[newApplicationSplit.Length - 1].Trim();
            }
            if (newApplication != currentApplication) {
                currentApplication = newApplication;
                Console.WriteLine("Current app: " + currentApplication);
            }
            // some kind of switch function that will update buttons based on currentApplication
        }

        private static string GetActiveProcessName() {
            const int nChars = 512;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            uint pid;
            GetWindowThreadProcessId(handle, out pid);
            Process p = Process.GetProcessById((int)pid);

            try {
                return p.MainModule.FileName;
            } catch (System.ComponentModel.Win32Exception) {
                return null;
            }
        }
    }
}
