using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace bradpad {
    class KeyMap {

        // Stores actions to display TODO: load ACTIONS from disk
        internal List<string> actions = new List<string>() {
                {"Open Word"},
                {"Copy"},
                {"Paste"},
                {"Open Chrome"},
                {"New Tab"},
        };

        Dictionary<Key, string> keyDict = new Dictionary<Key, string>() {
                {App.F22, "Open Word"},
                {App.F23, "Copy"},
                {App.F24, "Paste"},
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

        List<string> tempActions = new List<string>();

        internal void AddAction(string name, string val, bool appFlag) {
            actions.Add(name);
            allActions[name] = Tuple.Create(val, appFlag);
        }

        internal string GetAction(Key key) {
            return keyDict[key];
        }

        internal List<string> GetActions() {
            return actions.Except(tempActions).ToList();
        }

        internal string GetVal(Key key) {
            return allActions[keyDict[key]].Item1;
        }

        internal bool IsApp(Key key) {
            return allActions[keyDict[key]].Item2;
        }

        internal void AddTempAction(string name, bool temp) {
            tempActions.Add(name);
        }

        internal void SetAction(Key key, string action) {
            keyDict[key] = action;
        }
    }
}
