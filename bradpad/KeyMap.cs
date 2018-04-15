using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Newtonsoft.Json;

namespace bradpad {
    class KeyMap {

        internal class ActionData {

            [JsonProperty]
            private readonly string action;

            [JsonProperty]
            private readonly bool isApp;

            [JsonConstructor]
            internal ActionData(string inAction, bool inIsApp) {
                action = inAction;
                isApp = inIsApp;
            }

            internal string Action {
                get {
                    return action;
                }
            }

            internal bool IsApp {
                get {
                    return isApp;
                }
            }
        }

        // Maps action name to ActionData.
        [JsonProperty]
        Dictionary<string, ActionData> allActions;

        // Maps Key (pedal) to action name.
        [JsonProperty]
        Dictionary<Key, string> keyDict;

        // Contains names of temporary actions.
        [JsonProperty]
        HashSet<string> tempActions = new HashSet<string>();

        [JsonConstructor]
        internal KeyMap(Dictionary<string, ActionData> inAllActions, Dictionary<Key, string> inKeyDict, HashSet<string> inTempActions) {
            allActions = inAllActions;
            keyDict = inKeyDict;
            tempActions = inTempActions;
        }

        internal KeyMap() {
            allActions = new Dictionary<string, ActionData>() {
                {"Open Word", new ActionData("winword.exe", true)},
                {"Copy", new ActionData("^c", false)},
                {"Paste", new ActionData("^v", false)},
                {"Open Chrome", new ActionData("chrome.exe", true)},
                {"New Tab", new ActionData("^t", false)},
            };

            keyDict = new Dictionary<Key, string>() {
                {App.F22, "Open Word"},
                {App.F23, "Copy"},
                {App.F24, "Paste"},
            };
        }

        internal void AddAction(string name, string val, bool appFlag, bool temp) {
            if (temp && !allActions.ContainsKey(name)) {
                tempActions.Add(name);
            } else {
                tempActions.Remove(name);
            }
            allActions[name] = new ActionData(val, appFlag);
        }

        internal byte ContainsAction(string name) {
            // Returns 0 if does not contain action, 1 if contains action as temp, and 2 if contains action permanently
            if (tempActions.Contains(name)) {
                return 1;
            } else if (allActions.ContainsKey(name)) {
                return 2;
            }
            return 0;
        }

        internal string GetAction(Key key) {
            return keyDict[key];
        }

        internal IEnumerable<string> GetActions() {
            return allActions.Keys.Except(tempActions);
        }

        internal string GetVal(Key key) {
            return allActions[keyDict[key]].Action;
        }

        internal bool IsActiveAction(string action) {
            return keyDict.ContainsValue(action);
        }

        internal bool IsApp(Key key) {
            return allActions[keyDict[key]].IsApp;
        }

        internal void SetAction(Key key, string action) {
            string prevAction = keyDict[key];
            // Remove prevAction from dictionaries if it was temporary
            if (tempActions.Contains(prevAction)) {
                tempActions.Remove(prevAction);
                allActions.Remove(prevAction);
            }
            keyDict[key] = action;
        }
        
        internal void RemoveAction(string action) {
            allActions.Remove(action);
            tempActions.Remove(action);
        }
    }
}
