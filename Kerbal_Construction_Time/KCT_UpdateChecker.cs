using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    class KCT_UpdateChecker
    {
        public static bool UpdateFound = false;
        public static string CurrentVersion { get {return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public static String WebVersion = "";
        public static Boolean CheckForUpdate(bool ForceCheck, bool versionSpecific)
        {
#if DEBUG
            string updateSite = "http://magico13.net/KCT/latest_beta";
#else
            string updateSite = versionSpecific ? "http://magico13.net/KCT/latest-0-25-0" : "http://magico13.net/KCT/latest";
#endif
            if (ForceCheck || WebVersion == "")
            {
                Debug.Log("[KCT] Checking for updates...");
                WWW www = new WWW(updateSite);
                while (!www.isDone) { }

                WebVersion = www.text.Trim();
                Debug.Log("[KCT] Received version: " + WebVersion);
                
                if (WebVersion == "")
                    UpdateFound = false;
                else
                {
                    System.Version webV = new System.Version(WebVersion);
                    UpdateFound = (new System.Version(CurrentVersion).CompareTo(webV) < 0);
                }
            }
            if (UpdateFound)
                Debug.Log("[KCT] Update found: "+WebVersion+" Current: "+CurrentVersion);
            else
                Debug.Log("[KCT] No new updates. Current: " + CurrentVersion);
            return UpdateFound;
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