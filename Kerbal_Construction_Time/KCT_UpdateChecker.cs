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
        public static Boolean CheckForUpdate(bool ForceCheck, bool versionSpecific)
        {
            string updateSite = versionSpecific ? "http://magico13.net/KCT/latest-0-24-0" : "http://magico13.net/KCT/latest";
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
/*
Copyright (C) 2014  Michael Marvin, Zachary Eck

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/