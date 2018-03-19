using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace bradpad {
    class ActiveAppDetector {

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_SYSTEM_MINIMIZEEND = 23;
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private static WinEventDelegate dele0 = null;
        private static WinEventDelegate dele1 = null;
        private static IntPtr m_hhook0;
        private static IntPtr m_hhook1;
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

                // Set current application
                App app = (App)Application.Current;
                if (app != null) {
                    app.SetCurrentApplication(newApplication);
                }
            }
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
