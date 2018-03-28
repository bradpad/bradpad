using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Newtonsoft.Json;

namespace bradpad {
    class KeyMap {

        internal class Action {
            internal Action(string inAction, bool inIsApp) {
                action = inAction;
                isApp = inIsApp;
            }

            internal readonly string action;
            internal readonly bool isApp;
        }

        // Stores actions to display TODO: load actions from disk
        [JsonProperty]
        HashSet<string> actions = new HashSet<string>() {
                {"Open Word"},
                {"Copy"},
                {"Paste"},
                {"Open Chrome"},
                {"New Tab"},
        };

        [JsonProperty]
        Dictionary<Key, string> keyDict = new Dictionary<Key, string>() {
                {App.F22, "Open Word"},
                {App.F23, "Copy"},
                {App.F24, "Paste"},
            };

        // Action name, plus pair of action and whether it is an app
        [JsonProperty]
        Dictionary<string, Action> allActions = new Dictionary<string, Action>()
        {
                {"Open Word", new Action("winword.exe", true)},
                {"Copy", new Action("^c", false)},
                {"Paste", new Action("^v", false)},
                {"Open Chrome", new Action("chrome.exe", true)},
                {"New Tab", new Action("^t", false)},
            };

        [JsonProperty]
        HashSet<string> tempActions = new HashSet<string>();

        internal void AddAction(string name, string val, bool appFlag, bool temp) {
            actions.Add(name);
            allActions[name] = new Action(val, appFlag);
            if (temp) {
                tempActions.Add(name);
            } else {
                tempActions.Remove(name);
            }
        }

        internal string GetAction(Key key) {
            return keyDict[key];
        }

        internal IEnumerable<string> GetActions() {
            return actions.Except(tempActions);
        }

        internal string GetVal(Key key) {
            return allActions[keyDict[key]].action;
        }

        internal bool IsApp(Key key) {
            return allActions[keyDict[key]].isApp;
        }

        internal void SetAction(Key key, string action) {
            keyDict[key] = action;
        }
    }
}
