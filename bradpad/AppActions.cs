using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Newtonsoft.Json;

namespace bradpad {
    class AppActions {

        internal const string DEFAULT = "all_applications";
        internal const string EMPTY = "";

        string currentApplication = DEFAULT;

        // Maps application name to KeyMap
        [JsonProperty]
        Dictionary<string, KeyMap> keyMaps;

        // Maps from Application path to name
        [JsonProperty]
        Dictionary<string, string> appNames;

        [JsonConstructor]
        internal AppActions(Dictionary<string, KeyMap> inKeyMaps, Dictionary<string, string> inAppNames) {
            keyMaps = inKeyMaps;
            appNames = inAppNames;
        }

        internal void AddAction(string app, string name, string val, bool appFlag, bool temp) {
            keyMaps[app.ToLower()].AddAction(name, val, appFlag, temp);
        }

        internal bool ContainsAction(string app, string name) {
            return keyMaps[app.ToLower()].ContainsAction(name);
        }

        internal string GetAction(Key key) {
            return keyMaps[currentApplication.ToLower()].GetAction(key);
        }

        internal string GetAction(string app, Key key) {
            if (keyMaps.ContainsKey(app.ToLower())) {
                return keyMaps[app.ToLower()].GetAction(key);
            }
            return EMPTY;
        }

        internal List<string> GetActions(string app) {
            IEnumerable<string> defaultActions = keyMaps[DEFAULT].GetActions();
            if (app.ToLower() != DEFAULT) {
                List<string> combinedActions = keyMaps[app.ToLower()].GetActions().Union(defaultActions).ToList();
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

        internal string GetApplication() {
            return appNames[currentApplication];
        }

        internal void SetAction(string app, Key key, string action) {
            keyMaps[app.ToLower()].SetAction(key, action);
        }

        internal void SetCurrentApplication(string currentApplicationIn) {
            if (keyMaps.ContainsKey(currentApplicationIn.ToLower())) {
                currentApplication = currentApplicationIn.ToLower();
            } else {
                currentApplication = DEFAULT;
            }
        }

        internal List<KeyValuePair<string, string>> GetApplications() {
            List<KeyValuePair<string, string>> output = appNames.ToList();
            output.Sort((x, y) => x.Value.CompareTo(y.Value));
            for (int i = 0; i < output.Count; ++i) {
                if (output[i].Key == DEFAULT) {
                    KeyValuePair<string, string> item = output[i];
                    for (int j = i; j > 0; --j) {
                        output[j] = output[j - 1];
                    }
                    output[0] = item;
                    return output;
                }
            }
            // Should never be able to remove All Applications from appNames
            throw new Exception();
        }

        internal void InsertApplication(string name, string path) {
            appNames[path.ToLower()] = name;
            //Console.WriteLine("word: " + path);
            keyMaps[path.ToLower()] = new KeyMap();
        }

        internal void RemoveApplication(string name, string path) {
            appNames.Remove(path.ToLower());
            keyMaps.Remove(path.ToLower());
        }
    }
}
