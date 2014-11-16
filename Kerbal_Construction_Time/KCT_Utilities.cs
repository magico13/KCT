using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Construction_Time
{
    static class KCT_Utilities
    {
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

        public static Dictionary<String, int> PartListToDict(List<String> list)
        {
            Dictionary<String, int> newInv = new Dictionary<String, int>();
            foreach (String s in list)
            {
                if (newInv.Keys.Contains(s))
                    newInv[s]++;
                else
                    newInv.Add(s, 1);
            }
            return newInv;
        }

       /* public static Dictionary<String, int> PartListToDict(List<Part> list)
        {
            Dictionary<String, int> newInv = new Dictionary<String, int>();
            foreach (Part p in list)
            {
                string s = p.partInfo.name;
                if (newInv.Keys.Contains(s))
                    newInv[s]++;
                else
                    newInv.Add(s, 1);
            }
            return newInv;
        }*/

        public static List<String> PartDictToList(Dictionary<String, int> dict)
        {
            List<String> ret = new List<string>();
            for (int i = 0; i < dict.Count; i++)
            {
                for (int j=0; j<dict.Values.ElementAt(i); j++)
                {
                    ret.Add(dict.Keys.ElementAt(i));
                }
            }
            return ret;
        }

        public static AvailablePart GetAvailablePartByName(string partName)
        {
            foreach (AvailablePart a in PartLoader.LoadedPartsList)
            {
                if (a.name == partName)
                    return a;
            }
            return null;
        }

       /* public static double GetBuildTime(List<string> partNames, bool useTracker, bool useInventory)
        {
            List<AvailablePart> parts = new List<AvailablePart>();
            foreach (string s in partNames)
                parts.Add(GetAvailablePartByName(s));
            return GetBuildTime(parts, useTracker, useInventory);
        }*/

        public static double GetBuildTime(List<ConfigNode> parts, bool useTracker, bool useInventory)
        {
            Dictionary<String, int> dict = new Dictionary<string, int>();
            if (useInventory) dict = KCT_GameStates.PartInventory;
            return GetBuildTime(parts, useTracker, dict);
        }

        public static double GetBuildTime(List<Part> parts, bool useTracker, List<String> inventory)
        {
            List<ConfigNode> aParts = new List<ConfigNode>();
            foreach (Part p in parts)
            {
                ConfigNode partNode = new ConfigNode();
                p.protoPartSnapshot.Save(partNode);
                aParts.Add(partNode);
            }
            return GetBuildTime(aParts, useTracker, PartListToDict(inventory));
        }

        /*public static double GetBuildTime(List<String> parts, bool useTracker, List<String> inventory)
        {
            List<AvailablePart> aParts = new List<AvailablePart>();
            foreach (String s in parts)
                aParts.Add(GetAvailablePartByName(s));
            return GetBuildTime(aParts, useTracker, PartListToDict(inventory));
        }*/

        public static double GetBuildTime(List<ConfigNode> parts, bool useTracker, List<String> inventory)
        {
            return GetBuildTime(parts, useTracker, PartListToDict(inventory));
        }

        public static double GetBuildTime(List<Part> parts)
        {
            List<ConfigNode> aParts = new List<ConfigNode>();
            foreach (Part p in parts)
            {
                ConfigNode partNode = new ConfigNode();
                p.protoPartSnapshot.Save(partNode);
                aParts.Add(partNode);
            }
            return GetBuildTime(aParts, true, true);
        }

        public static double GetBuildTime(List<Part> parts, bool useTracker, bool useInventory)
        {
            List<ConfigNode> aParts = new List<ConfigNode>();
            foreach (Part p in parts)
            {
                ConfigNode partNode = new ConfigNode();
                p.protoPartSnapshot.Save(partNode);
                aParts.Add(partNode);
            }
            return GetBuildTime(aParts, useTracker, useInventory);
        }

        public static double GetBuildTime(List<Part> parts, bool useTracker, Dictionary<String, int> inventory)
        {
            List<ConfigNode> aParts = new List<ConfigNode>();
            foreach (Part p in parts)
            {
                ConfigNode partNode = new ConfigNode();
                p.protoPartSnapshot.Save(partNode);
                aParts.Add(partNode);
            }
            return GetBuildTime(aParts, useTracker, inventory);
        }

        public static double GetBuildTime(List<ConfigNode> parts, bool useTracker, Dictionary<String, int> inventory)
        {
            Dictionary<String, int> invCopy = new Dictionary<string,int>(inventory);//KCT_GameStates.PartInventory;
            /*for (int i=0; i<inventory.Count; i++)
            {
                invCopy.Add(inventory.Keys.ElementAt(i), inventory.Values.ElementAt(i));
            }*/
            double totalEffectiveCost = 0;
            foreach (ConfigNode p in parts)
            {
                String name = PartNameFromNode(p) + GetTweakScaleSize(p);
                double effectiveCost = 0;
                double cost = GetPartCostFromNode(p);
                if (inventory.Count > 0 && invCopy.ContainsKey(name) && KCT_GameStates.timeSettings.InventoryEffect > 0) // If the part is in the inventory, it has a small effect on the total craft
                {
                    // Combine the part tracker and inventory effect into one so that times will still decrease as you recover+reuse
                    if (useTracker && KCT_GameStates.timeSettings.BuildEffect > 0 && KCT_GameStates.PartTracker.ContainsKey(name))
                        effectiveCost = Math.Min(cost / (KCT_GameStates.timeSettings.InventoryEffect + (KCT_GameStates.timeSettings.BuildEffect * (KCT_GameStates.PartTracker[name] + 1))), cost);
                    else // Otherwise the cost is just the normal cost divided by the inventory effect
                        effectiveCost = cost / KCT_GameStates.timeSettings.InventoryEffect;
                    --invCopy[name];
                    if (invCopy[name] == 0)
                        invCopy.Remove(name);
                }
                else if (useTracker && KCT_GameStates.timeSettings.BuildEffect > 0 && KCT_GameStates.PartTracker.ContainsKey(name)) // The more the part is used, the faster it gets to build
                {
                    effectiveCost = Math.Min(cost / (KCT_GameStates.timeSettings.BuildEffect * (KCT_GameStates.PartTracker[name] + 1)), cost);
                }
                else // If the part has never been used, it takes the maximal time
                {
                    effectiveCost = cost;
                }

                if (effectiveCost < 0) effectiveCost = 0;
                totalEffectiveCost += effectiveCost;
            }

            return Math.Sqrt(totalEffectiveCost) * 2000 * KCT_GameStates.timeSettings.OverallMultiplier;
        }

        public static string PartNameFromNode(ConfigNode part)
        {
            string name = part.GetValue("part");
            name = name.Split('_')[0];
            return name;
        }

        public static double GetBuildRate(int index, KCT_BuildListVessel.ListType type, KCT_KSC KSC)
        {
            if (KSC == null) KSC = KCT_GameStates.ActiveKSC;
            double ret = 0;
            if (type == KCT_BuildListVessel.ListType.VAB)
            {
                if (KSC.VABUpgrades.Count - 1 >= index)
                {
                    ret = KSC.VABUpgrades[index] * (index+1) * 0.05;
                    if (index == 0) ret += 0.1;
                }
            }
            else if (type == KCT_BuildListVessel.ListType.SPH)
            {
                if (KSC.SPHUpgrades.Count - 1 >= index)
                {
                    ret = KSC.SPHUpgrades[index] * (index+1) * 0.05;
                    if (index == 0) ret += 0.1;
                }
            }
            else if (type == KCT_BuildListVessel.ListType.TechNode)
            {
                ret = Math.Pow(2, KSC.RDUpgrades[1] + 1) / 86400.0;
            }
            return ret;
        }

        public static double GetBuildRate(KCT_BuildListVessel ship)
        {
            if (ship.type == KCT_BuildListVessel.ListType.VAB)
                return GetBuildRate(ship.KSC.VABList.IndexOf(ship), ship.type, ship.KSC);
            else if (ship.type == KCT_BuildListVessel.ListType.SPH)
                return GetBuildRate(ship.KSC.SPHList.IndexOf(ship), ship.type, ship.KSC);
            else
                return 0;
        }

        public static List<double> BuildRatesVAB()
        {
            List<double> rates = new List<double>();
            if (KCT_GameStates.ActiveKSC.VABUpgrades.Count > 0)
            {
                for (int i = 0; i < KCT_GameStates.ActiveKSC.VABUpgrades.Count; i++)
                    rates.Add(GetBuildRate(i, KCT_BuildListVessel.ListType.VAB, null));
            }
            else
                rates.Add(0.1);
            return rates;
        }

        public static List<double> BuildRatesSPH()
        {
            List<double> rates = new List<double>();
            if (KCT_GameStates.ActiveKSC.SPHUpgrades.Count > 0)
            {
                for (int i = 0; i < KCT_GameStates.ActiveKSC.SPHUpgrades.Count; i++)
                    rates.Add(GetBuildRate(i, KCT_BuildListVessel.ListType.SPH, null));
            }
            else
                rates.Add(0.1);
            return rates;
        }

        private static double lastUT=0.0, UT;
        public static void ProgressBuildTime()
        {
            UT = Planetarium.GetUniversalTime();
            double UTDiff = UT - lastUT;
            if (UTDiff > 0 && UTDiff < (TimeWarp.fetch.warpRates[TimeWarp.fetch.warpRates.Length-1]*2) && lastUT > 0)
            {
                foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                {
                    double buildRate = 0;
                    if (ksc.VABList.Count > 0)
                    {
                        for (int i = 0; i < ksc.VABList.Count; i++)
                        {
                            buildRate = GetBuildRate(i, KCT_BuildListVessel.ListType.VAB, ksc);
                            ksc.VABList[i].AddProgress(buildRate * (UT - lastUT));
                            if (((IKCTBuildItem)ksc.VABList[i]).IsComplete())
                                MoveVesselToWarehouse(0, i, ksc);
                        }
                    }
                    if (ksc.SPHList.Count > 0)
                    {
                        for (int i = 0; i < ksc.SPHList.Count; i++)
                        {
                            buildRate = GetBuildRate(i, KCT_BuildListVessel.ListType.SPH, ksc);
                            ksc.SPHList[i].AddProgress(buildRate * (UT - lastUT));
                            if (((IKCTBuildItem)ksc.SPHList[i]).IsComplete())
                                MoveVesselToWarehouse(1, i, ksc);
                        }
                    }

                    for (int i = 0; i < KCT_GameStates.TechList.Count; i++)
                    {
                        KCT_TechItem tech = KCT_GameStates.TechList[i];
                        buildRate = tech.BuildRate;
                        tech.progress+=(buildRate * (UT - lastUT));
                        if (tech.isComplete || KCT_GameStates.settings.InstantTechUnlock)
                        {
                            if (KCT_GameStates.settings.ForceStopWarp && TimeWarp.CurrentRate > 1f)
                                TimeWarp.SetRate(0, true);
                            if (tech.protoNode == null) continue;
                            tech.EnableTech();
                            KCT_GameStates.TechList.Remove(tech);
                        }
                    }

                    if (ksc.LaunchPadReconditioning != null)
                    {
                        IKCTBuildItem item = (IKCTBuildItem)ksc.LaunchPadReconditioning;
                        ksc.LaunchPadReconditioning.progress += (item.GetBuildRate() * (UT - lastUT));
                        if (item.IsComplete() || !KCT_GameStates.settings.Reconditioning)
                            ksc.LaunchPadReconditioning = null;
                    }
                }
            }
            lastUT = UT;
        }

        public static float GetTotalVesselCost(ProtoVessel vessel)
        {
            float total = 0;
            foreach (ProtoPartSnapshot part in vessel.protoPartSnapshots)
            {
                float dry, wet;
                total += ShipConstruction.GetPartCosts(part, part.partInfo, out dry, out wet);
            }
            return total;
        }

        public static float GetTotalVesselCost(ConfigNode vessel)
        {
            float total = 0;
            foreach (ConfigNode part in vessel.GetNodes("PART"))
            {
                total += GetPartCostFromNode(part);
            }
            return total;
        }

        public static float GetPartCostFromNode(ConfigNode part)
        {
            string name = PartNameFromNode(part);
            float dry, wet;
            float total = ShipConstruction.GetPartCosts(part, GetAvailablePartByName(name), out dry, out wet);
            return total;
        }

        public static string GetTweakScaleSize(ProtoPartSnapshot part)
        {
            string partSize = "";
            ProtoPartModuleSnapshot tweakscale = part.modules.Find(mod => mod.moduleName == "TweakScale");
            if (tweakscale != null)
            {
                ConfigNode tsCN = tweakscale.moduleValues;
                string defaultScale = tsCN.GetValue("defaultScale");
                string currentScale = tsCN.GetValue("currentScale");
                if (!defaultScale.Equals(currentScale))
                    partSize = "," + currentScale;
            }
            return partSize;
        }

        public static string GetTweakScaleSize(ConfigNode part)
        {
            string partSize = "";
            if (part.HasNode("MODULE"))
            {
                ConfigNode[] Modules = part.GetNodes("MODULE");
                if (Modules.Length > 0 && Modules.FirstOrDefault(mod => mod.GetValue("name") == "TweakScale") != null)
                {
                    ConfigNode tsCN = Modules.First(mod => mod.GetValue("name") == "TweakScale");
                    string defaultScale = tsCN.GetValue("defaultScale");
                    string currentScale = tsCN.GetValue("currentScale");
                    if (!defaultScale.Equals(currentScale))
                        partSize = "," + currentScale;
                }
            }
            return partSize;
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

        public static bool CurrentGameHasScience()
        {
            return HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX;
        }
        public static bool CurrentGameIsSandbox()
        {
            return HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX;
        }
        public static bool CurrentGameIsCareer()
        {
            return HighLogic.CurrentGame.Mode == Game.Modes.CAREER;
        }

        public static void AddScienceWithMessage(float science, TransactionReasons reason)
        {
            if (science > 0)
            {
                //ResearchAndDevelopment.Instance.Science += science;
                ResearchAndDevelopment.Instance.AddScience(science, reason);
                var message = new ScreenMessage("[KCT] " + science + " science added.", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                ScreenMessages.PostScreenMessage(message, true);
            }
        }

        public static void MoveVesselToWarehouse(int ListIdentifier, int index, KCT_KSC KSC)
        {
            if (KSC == null) KSC = KCT_GameStates.ActiveKSC;
            if (ToolbarManager.ToolbarAvailable)
            {
                KCT_GameStates.kctToolbarButton.Important = true; //Show the button if it is hidden away
                startedFlashing = DateTime.Now; //Set the time to start flashing
            }

            if (KCT_GameStates.settings.ForceStopWarp && TimeWarp.CurrentRateIndex != 0)
            {
                TimeWarp.SetRate(0, true);
                KCT_GameStates.warpInitiated = false;
            }

            StringBuilder Message = new StringBuilder();
            Message.AppendLine("The following vessel is complete:");
            KCT_BuildListVessel vessel = null;
            if (ListIdentifier == 0) //VAB list
            {
                vessel = KSC.VABList[index];
                KSC.VABList.RemoveAt(index);
                KSC.VABWarehouse.Add(vessel);
                
                Message.AppendLine(vessel.shipName);
                Message.AppendLine("Please check the VAB Storage to launch it.");
            
            }
            else if (ListIdentifier == 1)//SPH list
            {
                vessel = KSC.SPHList[index];
                KSC.SPHList.RemoveAt(index);
                KSC.SPHWarehouse.Add(vessel);

                Message.AppendLine(vessel.shipName);
                Message.AppendLine("Please check the SPH Storage to launch it.");
            }

            //Assign science based on science rate
            if (CurrentGameHasScience() && !vessel.cannotEarnScience)
                AddScienceWithMessage((float)(KSC.RDUpgrades[0] * 0.5 * vessel.buildPoints / 86400), TransactionReasons.None);

            //Add parts to the tracker
            if (!vessel.cannotEarnScience)
            {
                List<string> trackedParts = new List<string>();
                foreach (ConfigNode p in vessel.ExtractedPartNodes)
                {
                    if (!trackedParts.Contains(PartNameFromNode(p)+GetTweakScaleSize(p)))
                    {
                        AddPartToTracker(PartNameFromNode(p) + GetTweakScaleSize(p));
                        trackedParts.Add(PartNameFromNode(p) + GetTweakScaleSize(p));
                    }
                }
            }

            string stor = ListIdentifier == 0 ? "VAB" : "SPH";
            KCTDebug.Log("Moved vessel " + vessel.shipName + " to " + stor +" storage.");

            foreach (KCT_KSC KSC_iterator in KCT_GameStates.KSCs)
            {
                foreach (KCT_BuildListVessel blv in KSC_iterator.VABList)
                {
                    double newTime = KCT_Utilities.GetBuildTime(blv.ExtractedPartNodes, true, blv.InventoryParts); //Use only the parts that were originally used when recalculating
                    if (newTime < blv.buildPoints)
                    {
                        blv.buildPoints = blv.buildPoints - ((blv.buildPoints - newTime) * (100 - blv.ProgressPercent()) / 100.0); //If progress=0% then set to new build time, 100%=no change, 50%=half of difference.
                    }
                }
                foreach (KCT_BuildListVessel blv in KSC_iterator.SPHList)
                {
                    double newTime = KCT_Utilities.GetBuildTime(blv.ExtractedPartNodes, true, blv.InventoryParts);
                    if (newTime < blv.buildPoints)
                    {
                        blv.buildPoints = blv.buildPoints - ((blv.buildPoints - newTime) * (100 - blv.ProgressPercent()) / 100.0); //If progress=0% then set to new build time, 100%=no change, 50%=half of difference.
                    }
                }
            }
            KCT_GUI.ResetBLWindow();
            if (!KCT_GameStates.settings.DisableAllMessages)
                DisplayMessage("Vessel Complete!", Message, MessageSystemButton.MessageButtonColor.GREEN, MessageSystemButton.ButtonIcons.COMPLETE);
        }


        public static void AddPartToInventory(Part part)
        {
            string tweakscale = GetTweakScaleSize(part.protoPartSnapshot); //partName,tweakscale
            string nameToStore = part.partInfo.name + tweakscale;
            AddPartToInventory(nameToStore);
        }
        public static void AddPartToInventory(ConfigNode part)
        {
            AddPartToInventory(PartNameFromNode(part) + GetTweakScaleSize(part));
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
            KCTDebug.Log("Added "+name+" to part inventory");
        }


        public static bool RemovePartFromInventory(Part part)
        {
            string tweakscale = GetTweakScaleSize(part.protoPartSnapshot); //partName,tweakscale
            string nameToStore = part.partInfo.name + tweakscale;
            return RemovePartFromInventory(nameToStore);
        }
        public static bool RemovePartFromInventory(Part part, Dictionary<String, int> inventory)
        {
            string tweakscale = GetTweakScaleSize(part.protoPartSnapshot); //partName,tweakscale
            string nameToStore = part.partInfo.name + tweakscale;
            return RemovePartFromInventory(nameToStore, inventory);
        }
        public static bool RemovePartFromInventory(ConfigNode part)
        {
            return RemovePartFromInventory(PartNameFromNode(part) + GetTweakScaleSize(part));
        }
        public static bool RemovePartFromInventory(ConfigNode part, Dictionary<String, int> inventory)
        {
            return RemovePartFromInventory(PartNameFromNode(part) + GetTweakScaleSize(part), inventory);
        }
        public static bool RemovePartFromInventory(String name)
        {
            return RemovePartFromInventory(name, KCT_GameStates.PartInventory);
        }
        public static bool RemovePartFromInventory(String name, Dictionary<String, int> inventory)
        {
            if (inventory.ContainsKey(name))
            {
                --inventory[name];
                if (inventory[name] == 0)
                {
                    inventory.Remove(name);
                }
                KCTDebug.Log("Removed " + name + " from part inventory");
                return true;
            }
            return false;
        }

        public static void AddPartToTracker(Part part)
        {
            string tweakscale = GetTweakScaleSize(part.protoPartSnapshot); //partName,tweakscale
            string nameToStore = part.partInfo.name + tweakscale;
            AddPartToTracker(nameToStore);
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
         //   KCTDebug.Log("Added "+name+" to part tracker");
        }

        public static void RemovePartFromTracker(Part part)
        {
            string tweakscale = GetTweakScaleSize(part.protoPartSnapshot); //partName,tweakscale
            string nameToStore = part.partInfo.name + tweakscale;
            RemovePartFromTracker(nameToStore);
        }
        public static void RemovePartFromTracker(String name)
        {
            if (KCT_GameStates.PartTracker.ContainsKey(name))
            {
                --KCT_GameStates.PartTracker[name];
                if (KCT_GameStates.PartTracker[name] == 0)
                    KCT_GameStates.PartTracker.Remove(name);
            //    KCTDebug.Log("Removed "+name+" from part tracker");
            }
        }


        public static void enableSimulationLocks()
        {
            InputLockManager.SetControlLock(ControlTypes.QUICKSAVE, "KCTLockSimQS");
            InputLockManager.SetControlLock(ControlTypes.QUICKLOAD, "KCTLockSimQL");
        }
        public static void disableSimulationLocks()
        {
            InputLockManager.RemoveControlLock("KCTLockSimQS");
            InputLockManager.RemoveControlLock("KCTLockSimQL");
        }

        public static void MakeSimulationSave()
        {
            string backupFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs";
            string saveFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
            KCTDebug.Log("Making simulation backup file.");
            System.IO.File.Copy(saveFile, backupFile, true);
        }

        public static void LoadSimulationSave()
        {
            string backupFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs";
            string saveFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
            KCT_Utilities.disableSimulationLocks();
            KCT_GameStates.flightSimulated = false;
            Kerbal_Construction_Time.moved = false;
            KCT_GameStates.simulationEndTime = 0;
            KCTDebug.Log("Swapping persistent.sfs with simulation backup file.");
            //System.IO.File.Copy(backupFile, saveFile, true);
            //System.IO.File.Delete(backupFile);
            GamePersistence.LoadGame("KCT_simulation_backup", HighLogic.SaveFolder, true, false);
            System.IO.File.Delete(backupFile);
        }


        public static Dictionary<string, float> TimeMultipliers = new Dictionary<string, float>()
        {
            {"0", 13},
            {"0.25", 1},
            {"0.5", 1.5f},
            {"1", 2},
            {"2", 3},
            {"6", 4},
            {"12", 5},
            {"24", 6},
            {"48", 7},
            {"168", 8},
            {"672", 9},
            {"8760", 10},
            {"43800", 11},
            {"87600", 12},
        };

        public static float CostOfSimulation(CelestialBody orbitBody, string simulationLength)
        {
            float timeMultiplier = 13;
            if (TimeMultipliers.ContainsKey(simulationLength))
                 timeMultiplier = TimeMultipliers[simulationLength];

            if (orbitBody == Planetarium.fetch.Sun)
                return 10000 * timeMultiplier;

            float atmosphereMult = orbitBody.atmosphere ? 1.1f : 1f;
            bool isMoon = orbitBody.referenceBody != Planetarium.fetch.Sun;
            CelestialBody Parent = orbitBody;
            while (Parent.referenceBody != Planetarium.fetch.Sun)
            {
                Parent = Parent.referenceBody;
            }
            
            CelestialBody Kerbin = GetBodyByName("Kerbin");

            double orbitRatio = 1;
            if (Parent.orbit.semiMajorAxis >= Kerbin.orbit.semiMajorAxis)
                orbitRatio = Parent.orbit.semiMajorAxis / Kerbin.orbit.semiMajorAxis;
            else
                orbitRatio = Kerbin.orbit.semiMajorAxis / Parent.orbit.semiMajorAxis;

            double cost = Math.Pow(orbitRatio,2) * 500 * (Parent.atmosphere ? 1.1 : 1);
            if (isMoon)
                cost *= atmosphereMult * 1.1;

            cost *= timeMultiplier;
            return (float)cost;
        }

        public static double SpendFunds(double toSpend, TransactionReasons reason)
        {
            if (!CurrentGameIsCareer())
                return 0;
            KCTDebug.Log("Removing funds: " + toSpend + ", New total: " + (Funding.Instance.Funds - toSpend));
            if (toSpend < Funding.Instance.Funds)
                Funding.Instance.AddFunds(-toSpend, reason);
            return Funding.Instance.Funds;
        }

        public static double AddFunds(double toAdd, TransactionReasons reason)
        {
            if (!CurrentGameIsCareer())
                return 0;
            KCTDebug.Log("Adding funds: " + toAdd + ", New total: " + (Funding.Instance.Funds + toAdd));
            Funding.Instance.AddFunds(toAdd, reason);
            return Funding.Instance.Funds;
        }

        public static KCT_BuildListVessel AddVesselToBuildList()
        {
            return AddVesselToBuildList(true);
        }

        public static KCT_BuildListVessel AddVesselToBuildList(bool useInventory)
        {
            KCT_BuildListVessel blv = new KCT_BuildListVessel(EditorLogic.fetch.ship, EditorLogic.fetch.launchSiteName, KCT_Utilities.GetBuildTime(EditorLogic.fetch.ship.SaveShip().GetNodes("PART").ToList(), true, useInventory), EditorLogic.FlagURL);
            blv.shipName = EditorLogic.fetch.shipNameField.Text;
            Dictionary<String, int> inventory = new Dictionary<string,int>();
            if (useInventory)
                inventory = KCT_GameStates.PartInventory;
            return AddVesselToBuildList(blv, inventory);
        }

        public static KCT_BuildListVessel AddVesselToBuildList(Dictionary<String, int> inventory)
        {
            KCT_BuildListVessel blv = new KCT_BuildListVessel(EditorLogic.fetch.ship, EditorLogic.fetch.launchSiteName, KCT_Utilities.GetBuildTime(EditorLogic.fetch.ship.SaveShip().GetNodes("PART").ToList(), true, inventory), EditorLogic.FlagURL);
            blv.shipName = EditorLogic.fetch.shipNameField.Text;
            return AddVesselToBuildList(blv, inventory);
        }

        public static KCT_BuildListVessel AddVesselToBuildList(KCT_BuildListVessel blv, bool useInventory)
        {
            Dictionary<String, int> inventory = new Dictionary<string, int>();
            if (useInventory)
                inventory = KCT_GameStates.PartInventory;
            return AddVesselToBuildList(blv, inventory);
        }

        public static KCT_BuildListVessel AddVesselToBuildList(KCT_BuildListVessel blv, Dictionary<String, int> inventory)
        {
            if (CurrentGameIsCareer())
            {
                float totalCost = GetTotalVesselCost(blv.shipNode);
                double prevFunds = Funding.Instance.Funds;
                double newFunds = SpendFunds(totalCost, TransactionReasons.VesselRollout);
                if (prevFunds == newFunds)
                {
                    KCTDebug.Log("Tried to add " + blv.shipName + " to build list but not enough funds.");
                    KCTDebug.Log("Vessel cost: " + GetTotalVesselCost(blv.shipNode) + ", Current funds: " + newFunds);
                    var msg = new ScreenMessage("Not Enough Funds To Build!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    ScreenMessages.PostScreenMessage(msg, true);
                    return blv;
                }
            }
            string type = "";
            if (blv.type == KCT_BuildListVessel.ListType.VAB)
            {
                KCT_GameStates.ActiveKSC.VABList.Add(blv);
                type = "VAB";
            }
            else if (blv.type == KCT_BuildListVessel.ListType.SPH)
            {
                KCT_GameStates.ActiveKSC.SPHList.Add(blv);
                type = "SPH";
            }
            if (inventory.Count > 0)
            {
                foreach (ConfigNode p in blv.ExtractedPartNodes)
                {
                    if (KCT_Utilities.RemovePartFromInventory(p, inventory))
                        blv.InventoryParts.Add(PartNameFromNode(p)+GetTweakScaleSize(p));
                }
            }
            KCTDebug.Log("Added " + blv.shipName + " to " + type + " build list. Cost: "+blv.cost);
            //KCTDebug.Log("Cost Breakdown (total, parts, fuel): " + blv.totalCost + ", " + blv.dryCost + ", " + blv.fuelCost);
            var message = new ScreenMessage("[KCT] Added " + blv.shipName + " to " + type + " build list.", 4.0f, ScreenMessageStyle.UPPER_CENTER);
            ScreenMessages.PostScreenMessage(message, true);
            return blv;
        }

        public static IKCTBuildItem NextThingToFinish()
        {
            IKCTBuildItem thing = null;
            double shortestTime = double.PositiveInfinity;
            foreach (IKCTBuildItem blv in KCT_GameStates.ActiveKSC.VABList)
            {
                double time = blv.GetTimeLeft();
                if (time < shortestTime)
                {
                    thing = blv;
                    shortestTime = time;
                }
            }
            foreach (IKCTBuildItem blv in KCT_GameStates.ActiveKSC.SPHList)
            {
                double time = blv.GetTimeLeft();
                if (time < shortestTime)
                {
                    thing = blv;
                    shortestTime = time;
                }
            }
            foreach (IKCTBuildItem blv in KCT_GameStates.TechList)
            {
                double time = blv.GetTimeLeft();
                if (time < shortestTime)
                {
                    thing = blv;
                    shortestTime = time;
                }
            }
            if (KCT_GameStates.ActiveKSC.LaunchPadReconditioning != null)
            {
                IKCTBuildItem blv = (IKCTBuildItem)KCT_GameStates.ActiveKSC.LaunchPadReconditioning;
                double time = blv.GetTimeLeft();
                if (time < shortestTime)
                {
                    thing = blv;
                    shortestTime = time;
                }
            }
            return thing;
        }

        public static KCT_BuildListVessel NextShipToFinish()
        {
            KCT_BuildListVessel ship = null;
            double shortestTime = double.PositiveInfinity;
            foreach (KCT_BuildListVessel blv in KCT_GameStates.ActiveKSC.VABList)
            {
                double time = blv.timeLeft;
                if (time < shortestTime)
                {
                    ship = blv;
                    shortestTime = time;
                }
            }
            foreach (KCT_BuildListVessel blv in KCT_GameStates.ActiveKSC.SPHList)
            {
                double time = blv.timeLeft;
                if (time < shortestTime)
                {
                    ship = blv;
                    shortestTime = time;
                }
            }
            return ship;
        }

        public static void RampUpWarp()
        {
            //KCT_BuildListVessel ship = KCT_Utilities.NextShipToFinish();
            IKCTBuildItem ship = KCT_Utilities.NextThingToFinish();
            RampUpWarp(ship);
        }

        public static void RampUpWarp(IKCTBuildItem item)
        {
            int lastRateIndex = TimeWarp.CurrentRateIndex;
            int newRate = TimeWarp.CurrentRateIndex + 1;
            double timeLeft = item.GetTimeLeft();
            if (double.IsPositiveInfinity(timeLeft))
                timeLeft = KCT_Utilities.NextThingToFinish().GetTimeLeft();
            while ((timeLeft > 15 * TimeWarp.deltaTime) && (TimeWarp.CurrentRateIndex < KCT_GameStates.settings.MaxTimeWarp) && (lastRateIndex < newRate))
            {
                lastRateIndex = TimeWarp.CurrentRateIndex;
                TimeWarp.SetRate(lastRateIndex + 1, true);
                newRate = TimeWarp.CurrentRateIndex;
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
            foreach (int i in KCT_GameStates.ActiveKSC.VABUpgrades) spentPoints += i;
            foreach (int i in KCT_GameStates.ActiveKSC.SPHUpgrades) spentPoints += i;
            foreach (int i in KCT_GameStates.ActiveKSC.RDUpgrades) spentPoints += i;
            return spentPoints;
        }

        public static float GetRecoveryValueForChuteLanding(ProtoVessel pv)
        {
            bool probeCoreAttached = false;
            foreach (ProtoPartSnapshot pps in pv.protoPartSnapshots)
            {
                if (pps.modules.Find(module => (module.moduleName == "ModuleCommand" && ((ModuleCommand)module.moduleRef).minimumCrew == 0)) != null)
                {
                    KCTDebug.Log("Probe Core found!");
                    probeCoreAttached = true;
                }
            }
            float RecoveryMod = probeCoreAttached ? 1.0f : KCT_GameStates.settings.RecoveryModifier;
            double distanceFromKSC = SpaceCenter.Instance.GreatCircleDistance(SpaceCenter.Instance.cb.GetRelSurfaceNVector(pv.latitude, pv.longitude));
            double maxDist = SpaceCenter.Instance.cb.Radius * Math.PI;
            float recoveryPercent = RecoveryMod * Mathf.Lerp(0.98f, 0.1f, (float)(distanceFromKSC / maxDist));
            float totalReturn = 0;
            foreach (ProtoPartSnapshot pps in pv.protoPartSnapshots)
            {
                float dryCost, fuelCost;
                totalReturn += ShipConstruction.GetPartCosts(pps, pps.partInfo, out dryCost, out fuelCost);
            }
            float totalBeforeReturn = (float)Math.Round(totalReturn, 2);
            totalReturn *= recoveryPercent;
            totalReturn = (float)Math.Round(totalReturn, 2);
            KCTDebug.Log("Vessel being recovered by KCT. Percent returned: " + 100 * recoveryPercent + "%. Distance from KSC: " + Math.Round(distanceFromKSC/1000, 2) + " km");
            KCTDebug.Log("Funds being returned: " + totalReturn + "/" + totalBeforeReturn);
            return totalReturn;
        }

        public static bool StageRecoveryAddonActive
        {
            get
            {
                Type SR = AssemblyLoader.loadedAssemblies
                .Select(a => a.assembly.GetExportedTypes())
                .SelectMany(t => t)
                .FirstOrDefault(t => t.FullName == "StageRecovery.StageRecovery");

                if (SR != null) return true;

                return false;
            }
        }

        public static bool DebRefundAddonActive
        {
            get
            {
                 Type DR = AssemblyLoader.loadedAssemblies
                 .Select(a => a.assembly.GetExportedTypes())
                 .SelectMany(t => t)
                 .FirstOrDefault(t => t.FullName == "DebRefund.DebRefundManager");
                 if (DR != null) return true;

                return false;
            }
        }

        public static void DisplayMessage(String title, StringBuilder text, MessageSystemButton.MessageButtonColor color, MessageSystemButton.ButtonIcons icon)
        {
            MessageSystem.Message m = new MessageSystem.Message(title, text.ToString(), color, icon);
            MessageSystem.Instance.AddMessage(m);
        }

        public static void RecalculateEditorBuildTime(ShipConstruct ship)
        {
            KCTDebug.Log("Recalculating build time");
            List<ConfigNode> partNodes = ship.SaveShip().GetNodes("PART").ToList();
            KCT_GUI.PartsInUse.Clear();
            if (KCT_GUI.useInventory)
            {
                foreach (ConfigNode part in partNodes)
                {
                    string name = PartNameFromNode(part) + GetTweakScaleSize(part);
                    if (!KCT_GUI.PartsInUse.ContainsKey(name))
                        KCT_GUI.PartsInUse.Add(name, 1);
                    else
                        ++KCT_GUI.PartsInUse[name];
                }
            }

            if (!KCT_GameStates.EditorShipEditingMode)
                KCT_GameStates.EditorBuildTime = KCT_Utilities.GetBuildTime(partNodes, true, KCT_GUI.useInventory);
            else
            {
                List<string> partsForInventory = new List<string>();
                if (KCT_GUI.useInventory)
                {
                    List<string> newParts = new List<string>(PartDictToList(KCT_GUI.PartsInUse));
                    List<string> theInventory = new List<string>(PartDictToList(KCT_GameStates.PartInventory));
                    foreach (string s in PartDictToList(KCT_GameStates.EditedVesselParts))
                        if (newParts.Contains(s))
                            newParts.Remove(s);

                    foreach (string s in newParts)
                    {
                        if (theInventory.Contains(s))
                        {
                            theInventory.Remove(s);
                            partsForInventory.Add(s);
                        }
                    }
                }
                foreach (string s in KCT_GameStates.editedVessel.InventoryParts)
                    partsForInventory.Add(s);

                KCT_GameStates.EditorBuildTime = KCT_Utilities.GetBuildTime(partNodes, true, partsForInventory);
            }
        }

        public static bool ApproximatelyEqual(double d1, double d2, double error = 0.01 )
        {
            return (1-error) <= (d1 / d2) && (d1 / d2) <= (1+error);
        }

        public static float GetParachuteDragFromPart(AvailablePart parachute)
        {
            foreach (AvailablePart.ModuleInfo mi in parachute.moduleInfos)
            {
                if (mi.info.Contains("Fully-Deployed Drag"))
                {
                    string[] split = mi.info.Split(new Char[] {':'});
                    //todo: figure out which index has the fully-deployed drag phrase, and then the one higher is the value
                    for (int i=0; i<split.Length; i++)
                    {
                        if (split[i].Contains("Fully-Deployed Drag"))
                        {
                            return float.Parse(split[i + 1]);
                        }
                    }
                }
            }
            return 0;
        }

        public static bool IsUnmannedCommand(AvailablePart part)
        {
            foreach (AvailablePart.ModuleInfo mi in part.moduleInfos)
            {
                if (mi.info.Contains("Unmanned")) return true;
            }
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