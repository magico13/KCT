using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    public class KCTDebug
    {
        public static void Log(object message, bool always = false)
        {
        #if DEBUG
            bool isBetaVersion = true;
        #else
            bool isBetaVersion = always;
        #endif
            if (KCT_GameStates.settings.Debug || isBetaVersion)
            {
                Debug.Log("[KCT] "+message);
            }
        }
    }

    public class KCT_OnLoadError
    {
        public bool OnLoadCalled, OnLoadFinished, AlertFired;
        private int timeout = 100, timer = 0;

        public bool HasErrored()
        {
            if (timer >= timeout)
            {
                return (OnLoadCalled && !OnLoadFinished);
            }
            else if (timer >= 0)
            {
                timer++;
            }
            return false;
        }

        public void OnLoadStart()
        {
            KCTDebug.Log("OnLoad Started");
            OnLoadCalled = true;
            OnLoadFinished = false;
            timer = 0;
            AlertFired = false;
        }

        public void OnLoadFinish()
        {
            OnLoadCalled = false;
            OnLoadFinished = true;
            timer = -1;
            KCTDebug.Log("OnLoad Completed");
        }

        public void FireAlert()
        {
            if (!AlertFired)
            {
                AlertFired = true;
                Debug.LogError("[KCT] ERROR! An error while KCT loading data occurred. Things will be seriously broken!");
                //Display error to user
                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "errorPopup", "Error Loading KCT Data", "ERROR! An error occurred while loading KCT data. Things will be seriously broken! Please report this error to the KCT forum thread and attach the log file. The game will be UNPLAYABLE in this state!", "Understood", false, HighLogic.UISkin);

                //Enable debug messages for future reports
                KCT_GameStates.settings.Debug = true;
                KCT_GameStates.settings.Save();
            }
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