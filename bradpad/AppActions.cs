using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Newtonsoft.Json;

namespace bradpad {
    class AppActions {

        internal const string DEFAULT = "ALL_APPLICATIONS";
        internal const string EMPTY = "";

        string currentApplication = DEFAULT;

        // Maps application name to KeyMap
        [JsonProperty]
        Dictionary<string, KeyMap> keyMaps;

        // Maps from Application path to name
        Dictionary<string, string> appNames;

        [JsonConstructor]
        internal AppActions(Dictionary<string, KeyMap> inKeyMaps) {
            keyMaps = inKeyMaps;
            appNames = new Dictionary<string, string>();
            appNames.Add(DEFAULT ,"All Applications");
            appNames.Add(@"C:\WINDOWS\Explorer.EXE", "Windows Explorer");
            appNames.Add(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "Google Chrome");
        }

        internal void AddAction(string app, string name, string val, bool appFlag, bool temp) {
            keyMaps[app].AddAction(name, val, appFlag, temp);
        }

        internal string GetAction(Key key) {
            //HACKY FIX
            //return keyMaps[currentApplication].GetAction(key);
            foreach(var i in keyMaps)
            {
                if(i.Key.Equals(currentApplication, StringComparison.InvariantCultureIgnoreCase))
                {
                    return keyMaps[i.Key].GetAction(key);
                }
            }
            return keyMaps[currentApplication].GetAction(key);
        }

        internal string GetAction(string app, Key key) {
            if (keyMaps.ContainsKey(app)) {
                return keyMaps[app].GetAction(key);
            }
            return EMPTY;
        }

        internal List<string> GetActions(string app) {
            IEnumerable<string> defaultActions = keyMaps[DEFAULT].GetActions();
            if (app != DEFAULT) {
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

        internal string GetApplication()
        {
            //HACKY FIX
            //return appNames[currentApplication];
            foreach (var i in appNames)
            {
                if (i.Key.Equals(currentApplication, StringComparison.InvariantCultureIgnoreCase))
                {
                    return appNames[i.Key];
                }
            }
            return appNames[currentApplication];
        }

        internal void SetAction(string app, Key key, string action) {
            keyMaps[app].SetAction(key, action);
        }

        internal void SetCurrentApplication(string currentApplicationIn) {
            /*if (keyMaps.ContainsKey(currentApplicationIn)) {
                //Console.WriteLine("testtest" + currentApplicationIn);
                currentApplication = currentApplicationIn;
            } else {
                //Console.WriteLine("test" + currentApplicationIn);
                currentApplication = DEFAULT;
            }*/
            foreach(var i in keyMaps)
            {
                if(i.Key.Equals(currentApplicationIn, StringComparison.InvariantCultureIgnoreCase))
                {
                    currentApplication = currentApplicationIn;
                    return;
                }
            }
            currentApplication = DEFAULT;
        }

        internal Dictionary<string, string> GetApplications()
        {
            return appNames;
        }

        internal void InsertApplication(string name, string path)
        {
            appNames[path] = name;
            Console.WriteLine("word: " + path);
            keyMaps[path] = new KeyMap();
        }
    }
}
