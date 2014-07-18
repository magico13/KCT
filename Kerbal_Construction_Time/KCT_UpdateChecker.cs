using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Construction_Time
{
    class KCT_UpdateChecker
    {
        public static bool UpdateFound = false;
        public static string CurrentVersion { get {return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public static String WebVersion = "";
        static string updateSite = "http://magico13.net/KCT/latest-0-24-0";
        public static Boolean CheckForUpdate(bool ForceCheck)
        {
            //System.Version current = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            //CurrentVersion = current.ToString();
            if (ForceCheck || WebVersion == "")
            {
                Debug.Log("[KCT] Checking for updates...");
                WWW www = new WWW(updateSite);
                while (!www.isDone) { }

                WebVersion = www.text.Trim();
                //Debug.Log("[KCT] Current version: " + CurrentVersion);
                Debug.Log("[KCT] Received version: " + WebVersion);
                
                if (WebVersion == "")
                    UpdateFound = false;
                else
                {
                    System.Version webV = new System.Version(WebVersion);
                    UpdateFound = (new System.Version(CurrentVersion).CompareTo(webV) < 0);//CompareVersions(WebVersion, CurrentVersion);
                }
            }
            if (UpdateFound)
                Debug.Log("[KCT] Update found: "+WebVersion+" Current: "+CurrentVersion);
            else
                Debug.Log("[KCT] No new updates. Current: " + CurrentVersion);
            return UpdateFound;
        }

        public static Boolean CompareVersions(string v1, string v2)
        {
            String[] v1Split = v1.Split('.');
            String[] v2Split = v2.Split('.');
            if (v1Split.Length != v2Split.Length && v1Split.Length != 4)
            {
                Debug.Log("[KCT] Version numbers not comparable!");
                return false;
            }
            if (int.Parse(v1Split[0]) > int.Parse(v2Split[0]))
                return true;
            else if (int.Parse(v1Split[1]) > int.Parse(v2Split[1]))
                return true;
            else if (int.Parse(v1Split[2]) > int.Parse(v2Split[2]))
                return true;
            else if (int.Parse(v1Split[3]) > int.Parse(v2Split[3]))
                return true;

            return false;
        }

    }
}
