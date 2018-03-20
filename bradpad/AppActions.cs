﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace bradpad {
    class AppActions {

        private const string DEFAULT = @"C:\WINDOWS\Explorer.EXE";

        string currentApplication = DEFAULT;
        Dictionary<string, KeyMap> keyMaps;

        internal AppActions(Dictionary<string, KeyMap> inKeyMaps) {
            keyMaps = inKeyMaps;
        }

        internal bool ContainsKey(Key key) {
            return key == App.F22 || key == App.F23 || key == App.F24;
        }

        internal void AddAction(string app, string name, string val, bool appFlag) {
            keyMaps[app].AddAction(name, val, appFlag);
        }

        internal void AddTempAction(string app, string name, bool temp) {
            keyMaps[app].AddTempAction(name, temp);
        }

        internal string GetAction(Key key) {
            return keyMaps[currentApplication].GetAction(key);
        }

        internal string GetAction(string app, Key key) {
            return keyMaps[app].GetAction(key);
        }

        internal List<string> GetActions() {
            return keyMaps[currentApplication].GetActions();
        }

        internal string GetVal(Key key) {
            return keyMaps[currentApplication].GetVal(key);
        }

        internal bool IsApp(Key key) {
            return keyMaps[currentApplication].IsApp(key);
        }

        internal void SetAction(string app, Key key, string action) {
            keyMaps[app].SetAction(key, action);
        }

        internal void SetCurrentApplication(string currentApplicationIn) {
            if (keyMaps.ContainsKey(currentApplicationIn)) {
                currentApplication = currentApplicationIn;
            } else {
                currentApplication = DEFAULT;
            }
        }
    }
}