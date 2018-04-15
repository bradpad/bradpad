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

        internal string GetAction(Key key) {
           
            return keyMaps[currentApplication.ToLower()].GetAction(key);
            /*foreach(var i in keyMaps)
            {
                if(i.Key.Equals(currentApplication, StringComparison.InvariantCultureIgnoreCase))
                {
                    return keyMaps[i.Key].GetAction(key);
                }
            }
            return keyMaps[currentApplication].GetAction(key);*/
        }

        internal string GetAction(string app, Key key) {
            if (keyMaps.ContainsKey(app.ToLower())) {
                return keyMaps[app.ToLower()].GetAction(key);
            }
            return EMPTY;
        }

        internal List<string> GetActions(string app) {
            IEnumerable<string> defaultActions = keyMaps[DEFAULT].GetActions();
            if (app != DEFAULT) {
                List<string> combinedActions = keyMaps[app.ToLower()].GetActions().Union(defaultActions).ToList();
                combinedActions.Sort();
                return combinedActions;
            }
            List<string> defaultActionsList = defaultActions.ToList();
            defaultActionsList.Sort();
            return defaultActionsList;
        }

        internal string GetVal(Key key) {
            return keyMaps[currentApplication.ToLower()].GetVal(key);
        }

        internal bool IsApp(Key key) {
            return keyMaps[currentApplication.ToLower()].IsApp(key);
        }

        internal string GetApplication()
        {
            return appNames[currentApplication.ToLower()];
            /*foreach (var i in appNames)
            {
                if (i.Key.Equals(currentApplication, StringComparison.InvariantCultureIgnoreCase))
                {
                    return appNames[i.Key];
                }
            }
            return appNames[currentApplication];*/
        }

        internal void SetAction(string app, Key key, string action) {
            keyMaps[app.ToLower()].SetAction(key, action);
        }

        internal void SetCurrentApplication(string currentApplicationIn) {
            if (keyMaps.ContainsKey(currentApplicationIn.ToLower())) {
                //Console.WriteLine("testtest" + currentApplicationIn);
                currentApplication = currentApplicationIn;
            } else {
                //Console.WriteLine("test" + currentApplicationIn);
                currentApplication = DEFAULT;
            }
            /*
            foreach(var i in keyMaps)
            {
                if(i.Key.Equals(currentApplicationIn, StringComparison.InvariantCultureIgnoreCase))
                {
                    currentApplication = currentApplicationIn;
                    return;
                }
            }
            currentApplication = DEFAULT;*/
        }

        internal Dictionary<string, string> GetApplications()
        {
            return appNames;
        }

        internal void InsertApplication(string name, string path)
        {
            appNames[path.ToLower()] = name;
            //Console.WriteLine("word: " + path);
            keyMaps[path.ToLower()] = new KeyMap();
        }

        internal void RemoveApplication(string name, string path)
        {
            appNames.Remove(path.ToLower());
            keyMaps.Remove(path.ToLower());
        }
    }
}
