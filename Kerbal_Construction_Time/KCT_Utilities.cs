using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Construction_Time
{
    static class KCT_Utilities
    {

      //  public static double BuildTimeModifier = 1.0;
        /// <summary>
        /// Formats a string from a time value into days, hours, minutes, and seconds.
        /// </summary>
        /// <param name="time">Time in seconds</param>
        /// <returns></returns>
        public static string GetFormattedTime(double time)
        {
            if (time > 0)
            {
                StringBuilder formatedTime = new StringBuilder();
                if (GameSettings.KERBIN_TIME)
                {
                    formatedTime.AppendFormat("{0,2:0} days, ", Math.Floor(time / 21600));
                    time = time % 21600;
                }
                else
                {
                    formatedTime.AppendFormat("{0,2:0} days, ", Math.Floor(time / 86400));
                    time = time % 86400;
                }
                formatedTime.AppendFormat("{0,2:0} hours, ", Math.Floor(time / 3600));
                time = time % 3600;
                formatedTime.AppendFormat("{0,2:0} minutes, ", Math.Floor(time / 60));
                time = time % 60;
                formatedTime.AppendFormat("{0,2:0} seconds", time);

                return formatedTime.ToString();
            }
            else
            {
                return "0 days,  0 hours,  0 minutes,  0 seconds";
            }

        }

        public static string GetColonFormattedTime(double time)
        {
            if (time > 0)
            {
                StringBuilder formatedTime = new StringBuilder();
                if (GameSettings.KERBIN_TIME)
                {
                    formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 21600));
                    time = time % 21600;
                }
                else
                {
                    formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 86400));
                    time = time % 86400;
                }
                formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 3600));
                time = time % 3600;
                formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 60));
                time = time % 60;
                formatedTime.AppendFormat("{0,2:00}", time);   

                return formatedTime.ToString();
            }
            else
            {
                return "00:00:00:00";
            }
        }

        public static double GetBuildTime(List<Part> parts, bool useTracker, bool useInventory)
        {
            Dictionary<String, int> invCopy = new Dictionary<string,int>();//KCT_GameStates.PartInventory;
            for (int i=0; i<KCT_GameStates.PartInventory.Count; i++)
            {
                invCopy.Add(KCT_GameStates.PartInventory.Keys.ElementAt(i), KCT_GameStates.PartInventory.Values.ElementAt(i));
            }

            double totalEffectiveCost = 0;
            foreach (Part p in parts)
            {
                double effectiveCost = 0;
                String name = p.partInfo.name;
                if (useInventory && invCopy.ContainsKey(name) && KCT_GameStates.timeSettings.InventoryEffect > 0) // If the part is in the inventory, it has a small effect on the total craft
                {
                    // Combine the part tracker and inventory effect into one so that times will still decrease as you recover+reuse
                    if (useTracker && KCT_GameStates.timeSettings.BuildEffect > 0 && KCT_GameStates.PartTracker.ContainsKey(name))
                        effectiveCost = Math.Min(p.partInfo.cost / (KCT_GameStates.timeSettings.InventoryEffect + (KCT_GameStates.timeSettings.BuildEffect * (KCT_GameStates.PartTracker[name] + 1))), p.partInfo.cost);
                    else // Otherwise the cost is just the normal cost divided by the inventory effect
                        effectiveCost = p.partInfo.cost / KCT_GameStates.timeSettings.InventoryEffect;
                    --invCopy[name];
                    if (invCopy[name] == 0)
                        invCopy.Remove(name);
                }
                else if (useTracker && KCT_GameStates.timeSettings.BuildEffect > 0 && KCT_GameStates.PartTracker.ContainsKey(name)) // The more the part is used, the faster it gets to build
                {
                    effectiveCost = Math.Min(p.partInfo.cost / (KCT_GameStates.timeSettings.BuildEffect * (KCT_GameStates.PartTracker[name] + 1)), p.partInfo.cost);
                }
                else // If the part has never been used, it takes the maximal time
                {
                    effectiveCost = p.partInfo.cost;
                }

                totalEffectiveCost += effectiveCost;
            }

            return Math.Sqrt(totalEffectiveCost) * 2000 * KCT_GameStates.timeSettings.OverallMultiplier;

                        //double totalCost = 0;
            //foreach (Part p in parts)
            //{
            //    totalCost += p.partInfo.cost;
            //}

            //return Math.Sqrt(totalCost) * 2000; // /10 *KCT_GameStates.activeVessel.vessel.Parts.Count;
        }

        public static double GetBuildTime(List<Part> parts)
        {
            return GetBuildTime(parts, true, true);
        }

        public static double GetBuildRate(int index, KCT_BuildListVessel.ListType type)
        {
            double ret = 0;
            if (type == KCT_BuildListVessel.ListType.VAB)
            {
                if (KCT_GameStates.VABUpgrades.Count - 1 >= index)
                {
                    ret = KCT_GameStates.VABUpgrades[index] * (index+1) * 0.05;
                    if (index == 0) ret += 1;
                }
            }
            else
            {
                if (KCT_GameStates.SPHUpgrades.Count - 1 >= index)
                {
                    ret = KCT_GameStates.SPHUpgrades[index] * (index+1) * 0.05;
                    if (index == 0) ret += 1;
                }
            }
            return ret;
        }

        public static double GetBuildRate(KCT_BuildListVessel ship)
        {
            if (ship.type == KCT_BuildListVessel.ListType.VAB)
                return GetBuildRate(KCT_GameStates.VABList.IndexOf(ship), ship.type);
            else if (ship.type == KCT_BuildListVessel.ListType.SPH)
                return GetBuildRate(KCT_GameStates.SPHList.IndexOf(ship), ship.type);
            else
                return 0;
        }

        private static double lastUT=0.0, UT;
        public static void ProgressBuildTime()
        {
            UT = Planetarium.GetUniversalTime();
            if (lastUT < UT && lastUT > 0)
            {
                double buildRate = 0;
                if (KCT_GameStates.VABList.Count > 0)
                {
                    for (int i = 0; i < KCT_GameStates.VABList.Count; i++)
                    {
                        buildRate = GetBuildRate(i, KCT_BuildListVessel.ListType.VAB);
                        KCT_GameStates.VABList[i].AddProgress(buildRate * (UT - lastUT));
                        if (KCT_GameStates.VABList[i].isComplete())
                            MoveVesselToWarehouse(0, i);
                    }
                }
                if (KCT_GameStates.SPHList.Count > 0)
                {
                    for (int i = 0; i < KCT_GameStates.SPHList.Count; i++)
                    {
                        buildRate = GetBuildRate(i, KCT_BuildListVessel.ListType.SPH);
                        KCT_GameStates.SPHList[i].AddProgress(buildRate * (UT - lastUT));
                        if (KCT_GameStates.SPHList[i].isComplete())
                            MoveVesselToWarehouse(1, i);
                    }
                }
               /* foreach (KCTVessel kctV in KCT_GameStates.vesselList)
                {
                    if (kctV.building)
                    {
                        kctV.progress += buildRate * (UT - lastUT);
                    }
                }*/
            }
            lastUT = UT;
        }

        private static DateTime startedFlashing;
        public static String GetButtonTexture()
        {
            String textureReturn;

            if (!KCT_GameStates.settings.enabledForSave)
                return "KerbalConstructionTime/icons/KCT_off";

            //Flash for up to 3 seconds, at half second intervals per icon
            if (KCT_GameStates.kctToolbarButton.Important && (DateTime.Now.CompareTo(startedFlashing.AddSeconds(3))) < 0 && DateTime.Now.Millisecond < 500)
                textureReturn = "KerbalConstructionTime/icons/KCT_off";
            //If it's been longer than 3 seconds, set Important to false and stop flashing
            else if (KCT_GameStates.kctToolbarButton.Important && (DateTime.Now.CompareTo(startedFlashing.AddSeconds(3))) > 0)
            {
                KCT_GameStates.kctToolbarButton.Important = false;
                textureReturn = "KerbalConstructionTime/icons/KCT_on";
            }
            //The normal icon
            else
                textureReturn = "KerbalConstructionTime/icons/KCT_on";

            return textureReturn;
        }

        public static bool CurrentGameIsCareer()
        {
            return HighLogic.CurrentGame.Mode == Game.Modes.CAREER;
        }
        public static bool CurrentGameIsSandbox()
        {
            return HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX;
        }

        public static void AddScienceWithMessage(float science)
        {
            ResearchAndDevelopment.Instance.Science += science;
            var message = new ScreenMessage("[KCT] "+science+" science added.", 4.0f, ScreenMessageStyle.UPPER_LEFT);
            ScreenMessages.PostScreenMessage(message, true);
        }

        public static void MoveVesselToWarehouse(int ListIdentifier, int index)
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                KCT_GameStates.kctToolbarButton.Important = true; //Show the button if it is hidden away
                startedFlashing = DateTime.Now; //Set the time to start flashing
            }

            if ((KCT_GameStates.warpInitiated || KCT_GameStates.settings.ForceStopWarp) && TimeWarp.CurrentRateIndex != 0)
            {
                TimeWarp.SetRate(0, true); //Turn off timewarp when a ship finishes. Fix for it not stopping on finish.
                KCT_GameStates.warpInitiated = false;
            }

            if (ListIdentifier == 0) //VAB list
            {
                KCT_BuildListVessel blv = KCT_GameStates.VABList[index];
                KCT_GameStates.VABList.RemoveAt(index);
                KCT_GameStates.VABWarehouse.Add(blv);
                if (CurrentGameIsCareer())
                    AddScienceWithMessage((float)(KCT_GameStates.RDUpgrades[0] * 0.5 * blv.buildPoints / 86400));
                foreach (Part p in blv.getShip().parts)
                    AddPartToTracker(p);
            }
            else if (ListIdentifier == 1)//SPH list
            {
                KCT_BuildListVessel blv = KCT_GameStates.SPHList[index];
                KCT_GameStates.SPHList.RemoveAt(index);
                KCT_GameStates.SPHWarehouse.Add(blv);
                if (CurrentGameIsCareer())
                    AddScienceWithMessage((float)(KCT_GameStates.RDUpgrades[0] * 0.5 * blv.buildPoints / 86400));
                foreach (Part p in blv.getShip().parts)
                    AddPartToTracker(p);
            }
            foreach (KCT_BuildListVessel blv in KCT_GameStates.VABList)
            {
                List<Part> ship = blv.getShip().parts;
                double newTime = KCT_Utilities.GetBuildTime(ship, true, false); //Don't use the part inventory when determining the time
                if (newTime < blv.buildPoints)
                {
                    blv.buildPoints = blv.buildPoints - ((blv.buildPoints - newTime) * (100 - blv.ProgressPercent())/100.0); //If progress=0% then set to new build time, 100%=no change, 50%=half of difference.
                }
            }
            foreach (KCT_BuildListVessel blv in KCT_GameStates.SPHList)
            {
                List<Part> ship = blv.getShip().parts;
                double newTime = KCT_Utilities.GetBuildTime(ship, true, false);
                if (newTime < blv.buildPoints)
                {
                    blv.buildPoints = blv.buildPoints - ((blv.buildPoints - newTime) * (100 - blv.ProgressPercent()) / 100.0); //If progress=0% then set to new build time, 100%=no change, 50%=half of difference.
                }
            }
            KCT_GUI.ResetBLWindow();
        }

        public static void AddPartToInventory(Part part)
        {
            AddPartToInventory(part.partInfo.name);
        }
        public static void AddPartToInventory(String name)
        {
            if (KCT_GameStates.PartInventory.ContainsKey(name))
            {
                ++KCT_GameStates.PartInventory[name];
            }
            else
            {
                KCT_GameStates.PartInventory.Add(name, 1);
            }
         //   Debug.Log("[KCT] Added "+name+" to part inventory");
        }


        public static bool RemovePartFromInventory(Part part)
        {
            return RemovePartFromInventory(part.partInfo.name);
        }
        public static bool RemovePartFromInventory(String name)
        {
            if (KCT_GameStates.PartInventory.ContainsKey(name))
            {
                --KCT_GameStates.PartInventory[name];
                if (KCT_GameStates.PartInventory[name] == 0)
                {
                    KCT_GameStates.PartInventory.Remove(name);
                }
                return true;
            //    Debug.Log("[KCT] Removed " + name + " from part inventory");
            }
            return false;
        }

        public static void AddPartToTracker(Part part)
        {
            AddPartToTracker(part.partInfo.name);
        }
        public static void AddPartToTracker(String name)
        {
            if (KCT_GameStates.PartTracker.ContainsKey(name))
            {
                ++KCT_GameStates.PartTracker[name];
            }
            else
            {
                KCT_GameStates.PartTracker.Add(name, 1);
            }
         //   Debug.Log("[KCT] Added "+name+" to part tracker");
        }

        public static void RemovePartFromTracker(Part part)
        {
            RemovePartFromTracker(part.partInfo.name);
        }
        public static void RemovePartFromTracker(String name)
        {
            if (KCT_GameStates.PartTracker.ContainsKey(name))
            {
                --KCT_GameStates.PartTracker[name];
                if (KCT_GameStates.PartTracker[name] == 0)
                    KCT_GameStates.PartTracker.Remove(name);
            //    Debug.Log("[KCT] Removed "+name+" from part tracker");
            }
        }


        public static void enableSimulationLocks()
        {
            InputLockManager.SetControlLock(ControlTypes.QUICKSAVE, "KCTLockSimQS");
            InputLockManager.SetControlLock(ControlTypes.QUICKLOAD, "KCTLockSimQL");
            InputLockManager.SetControlLock(ControlTypes.VESSEL_SWITCHING, "KCTLockSimVS");
          //  InputLockManager.SetControlLock(ControlTypes.PAUSE, "KCTLockSimPause");
           // InputLockManager.SetControlLock(ControlTypes.EVA_INPUT, "KCTLockSimEVA");
         //   KCT_GameStates.flightSimulated = true;
        }
        public static void disableSimulationLocks()
        {
            InputLockManager.RemoveControlLock("KCTLockSimQS");
            InputLockManager.RemoveControlLock("KCTLockSimQL");
            InputLockManager.RemoveControlLock("KCTLockSimVS");
         //   InputLockManager.RemoveControlLock("KCTLockSimPause");
           // InputLockManager.RemoveControlLock("KCTLockSimEVA");
          //  KCT_GameStates.flightSimulated = false;
        }

        public static void MakeSimulationSave()
        {
            string backupFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs";
            string saveFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
            Debug.Log("[KCT] Making simulation backup file.");
            System.IO.File.Copy(saveFile, backupFile, true);
        }

        public static void LoadSimulationSave()
        {
            string backupFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs";
            string saveFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
            KCT_Utilities.disableSimulationLocks();
            KCT_GameStates.flightSimulated = false;
            KCT_GameStates.simulationEndTime = 0;
            Debug.Log("[KCT] Swapping persistent.sfs with simulation backup file.");
            if (MCEWrapper.MCEAvailable)
            {
                Debug.Log("[KCT] Loading MCE backup file.");
                MCEWrapper.IloadMCEbackup();
            }
            System.IO.File.Copy(backupFile, saveFile, true);
            System.IO.File.Delete(backupFile);
        }

        public static void AddVesselToBuildList()
        {
            KCT_BuildListVessel blv = new KCT_BuildListVessel(EditorLogic.fetch.ship, EditorLogic.fetch.launchSiteName, KCT_Utilities.GetBuildTime(EditorLogic.fetch.ship.Parts), EditorLogic.FlagURL);
            string type = "";
            if (blv.type == KCT_BuildListVessel.ListType.VAB)
            {
                KCT_GameStates.VABList.Add(blv);
                type = "VAB";
            }
            else if (blv.type == KCT_BuildListVessel.ListType.SPH)
            {
                KCT_GameStates.SPHList.Add(blv);
                type = "SPH";
            }
            foreach (Part p in EditorLogic.fetch.ship.Parts)
            {
                if (KCT_Utilities.RemovePartFromInventory(p))
                    blv.InventoryParts.Add(p.partInfo.name);
            }
            Debug.Log("[KCT] Added " + blv.shipName + " to " + type + " build list.");
            var message = new ScreenMessage("[KCT] Added " + blv.shipName + " to "+type+" build list.", 4.0f, ScreenMessageStyle.UPPER_RIGHT);
            ScreenMessages.PostScreenMessage(message, true);
        }

        public static KCT_BuildListVessel NextShipToFinish()
        {
            KCT_BuildListVessel ship = null;
            double shortestTime = double.PositiveInfinity;
            foreach (KCT_BuildListVessel blv in KCT_GameStates.VABList)
            {
                double time = blv.timeLeft;
                if (time < shortestTime)
                {
                    ship = blv;
                    shortestTime = time;
                }
            }
            foreach (KCT_BuildListVessel blv in KCT_GameStates.SPHList)
            {
                double time = blv.timeLeft;
                if (time < shortestTime)
                {
                    ship = blv;
                    shortestTime = time;
                }
            }
  /*          if (KCT_GameStates.VABList.Count > 0)
            {
                KCT_BuildListVessel vab = KCT_GameStates.VABList[0];
                if (KCT_GameStates.SPHList.Count > 0)
                {
                    KCT_BuildListVessel sph = KCT_GameStates.SPHList[0];
                    if (vab.buildPoints - vab.progress < sph.buildPoints - sph.progress)
                    {
                        ship = vab;
                    }
                    else
                    {
                        ship = sph;
                    }
                }
                else
                    ship = vab;
            }
            else
            {
                if (KCT_GameStates.SPHList.Count > 0)
                {
                    ship = KCT_GameStates.SPHList[0];
                }
                else
                {
                    ship = null;
                }
            }*/
            return ship;
        }

        public static void RampUpWarp()
        {
            KCT_BuildListVessel ship = KCT_Utilities.NextShipToFinish();
            while ((ship.timeLeft > 15*TimeWarp.deltaTime) && (TimeWarp.CurrentRateIndex < KCT_GameStates.settings.MaxTimeWarp))
            {
                TimeWarp.SetRate(TimeWarp.CurrentRateIndex + 1, true);
            }
        }

        public static void DisableModFunctionality()
        {
            disableSimulationLocks();
            InputLockManager.RemoveControlLock("KCTLaunchLock");
            KCT_GUI.hideAll();
        }

        public static CelestialBody GetBodyByName(String name)
        {
            foreach (CelestialBody b in FlightGlobals.Bodies)
            {
                if (b.bodyName.ToLower() == name.ToLower())
                    return b;
            }
            return null;
        }

        public static object GetMemberInfoValue(System.Reflection.MemberInfo member, object sourceObject)
        {
            object newVal;
            if (member is System.Reflection.FieldInfo)
                newVal = ((System.Reflection.FieldInfo)member).GetValue(sourceObject);
            else
                newVal = ((System.Reflection.PropertyInfo)member).GetValue(sourceObject, null);
            return newVal;
        }

        public static int TotalSpentUpgrades()
        {
            int spentPoints = 0;
            foreach (int i in KCT_GameStates.VABUpgrades) spentPoints += i;
            foreach (int i in KCT_GameStates.SPHUpgrades) spentPoints += i;
            foreach (int i in KCT_GameStates.RDUpgrades) spentPoints += i;
            return spentPoints;
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