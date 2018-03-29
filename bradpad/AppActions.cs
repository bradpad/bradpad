using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Newtonsoft.Json;

namespace bradpad {
    class AppActions {

        private const string DEFAULT = @"C:\WINDOWS\Explorer.EXE";

        string currentApplication = DEFAULT;

        // Maps application name to KeyMap
        [JsonProperty]
        Dictionary<string, KeyMap> keyMaps;

        [JsonConstructor]
        internal AppActions(Dictionary<string, KeyMap> inKeyMaps) {
            keyMaps = inKeyMaps;
        }

        internal void AddAction(string app, string name, string val, bool appFlag, bool temp) {
            keyMaps[app].AddAction(name, val, appFlag, temp);
        }

        internal string GetAction(Key key) {
            return keyMaps[currentApplication].GetAction(key);
        }

        internal string GetAction(string app, Key key) {
            if (keyMaps.ContainsKey(app)) {
                return keyMaps[app].GetAction(key);
            }
            return "";
        }

        internal List<string> GetActions(string app) {
            IEnumerable<string> defaultActions = keyMaps[""].GetActions();
            if (app != "") {
                List<string> combinedActions = keyMaps[app].GetActions().Union(defaultActions).ToList();
                combinedActions.Sort();
                return combinedActions;
            }
            List<string> defaultActionsList = defaultActions.ToList();
            defaultActionsList.Sort();
            return defaultActionsList;
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
