using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
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

        public static double ParseColonFormattedTime(string timeString, bool toUT=true)
        {
            //toUT is for converting a string that is given as a formatted UT (Starting with Y1, D1)
            double time = 0;
            string[] parts = timeString.Split(':');
            int len = parts.Length;
            double sPerDay = GameSettings.KERBIN_TIME ? 6 * 3600 : 24 * 3600;
            double sPerYear = GameSettings.KERBIN_TIME ? 426 * sPerDay : 365 * sPerDay;
            try
            {
                time = double.Parse(parts[len - 1]);
                if (len > 1)
                    time += 60 * double.Parse(parts[len - 2]); //minutes
                if (len > 2)
                    time += 3600 * double.Parse(parts[len - 3]); //hours
                if (len > 3)
                { 
                    time += sPerDay * double.Parse(parts[len - 4]); //days
                    if (toUT)
                        time -= sPerDay;
                }
                if (len > 4)
                {
                    time += sPerYear * double.Parse(parts[len - 5]); //years
                    if (toUT)
                        time -= sPerYear;
                }
            }
            catch
            {
                KCTDebug.Log("Error parsing time string.");
                time = -1;
            }
            return time;
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

        public static Dictionary<String, int> PartListToDictAlternating(List<String> list)
        {
            Dictionary<String, int> newInv = new Dictionary<String, int>();
            int length = list.Count;
            for (int i = 0; i < length; i += 2)
            {
                KCT_Utilities.AddToDict(newInv, list[i], int.Parse(list[i + 1]));
            }
                /*foreach (String s in list)
                {
                    if (newInv.Keys.Contains(s))
                        newInv[s]++;
                    else
                        newInv.Add(s, 1);
                }*/
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
                //for (int j=0; j<dict.Values.ElementAt(i); j++)
              //  {
                    ret.Add(dict.Keys.ElementAt(i));
                    ret.Add(dict.Values.ElementAt(i).ToString());
               // }
            }
            return ret;
        }

        public static AvailablePart GetAvailablePartByName(string partName)
        {
            /*foreach (AvailablePart a in PartLoader.LoadedPartsList)
            {
                if (a.name == partName)
                    return a;
            }
            return null;*/
            return PartLoader.getPartInfoByName(partName);
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
           /* List<ConfigNode> aParts = new List<ConfigNode>();
            foreach (Part p in parts)
            {
                ConfigNode partNode = new ConfigNode();
                p.protoPartSnapshot.Save(partNode);
                aParts.Add(partNode);
            }*/
            return GetBuildTime(parts, useTracker, PartListToDict(inventory));
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
            /*List<ConfigNode> aParts = new List<ConfigNode>();
            foreach (Part p in parts)
            {
                ConfigNode partNode = new ConfigNode();
                p.protoPartSnapshot.Save(partNode);
                aParts.Add(partNode);
            }
            return GetBuildTime(aParts, true, true);*/
            return GetBuildTime(parts, true, true);
        }

        public static double GetBuildTime(List<Part> parts, bool useTracker, bool useInventory)
        {
            /*List<ConfigNode> aParts = new List<ConfigNode>();
            foreach (Part p in parts)
            {
                ConfigNode partNode = new ConfigNode();
                p.protoPartSnapshot.Save(partNode);
                aParts.Add(partNode);
            }
            return GetBuildTime(aParts, useTracker, useInventory);*/
            return GetBuildTime(parts, useTracker, useInventory?KCT_GameStates.PartInventory:new Dictionary<string, int>());
        }

        public static double GetBuildTime(List<Part> parts, bool useTracker, Dictionary<String, int> inventory)
        {
           /* List<ConfigNode> aParts = new List<ConfigNode>();
            foreach (Part p in parts)
            {
                ConfigNode partNode = new ConfigNode();
                p.protoPartSnapshot.Save(partNode);
                aParts.Add(partNode);
            }
            return GetBuildTime(aParts, useTracker, inventory);*/

            //Let's make this one actually work as well.
            Dictionary<String, int> invCopy = new Dictionary<string, int>(inventory);
            double totalEffectiveCost = 0;
            foreach (Part p in parts)
            {
                String name = p.partInfo.name;
                double effectiveCost = 0;
                //double cost = ShipConstruction.GetPartCosts(p.protoPartSnapshot, p.partInfo, out dC, out wC);
                double cost = GetPartCosts(p);
                double dryCost = GetPartCosts(p, false);
                
                double drymass = p.mass;
                double wetmass = p.GetResourceMass() + drymass;
                if (!KCT_Utilities.PartIsProcedural(p))
                {
                    name += GetTweakScaleSize(p);
                    double InvEff = (invCopy.ContainsKey(name) && invCopy[name] > 0) ? KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect : 0;
                    int used = (useTracker && KCT_GameStates.PartTracker.ContainsKey(name)) ? KCT_GameStates.PartTracker[name] : 0;
                    //C=cost, c=dry cost, M=wet mass, m=dry mass, U=part tracker, O=overall multiplier, I=inventory effect (0 if not in inv), B=build effect
                    effectiveCost = KCT_MathParsing.GetStandardFormulaValue("EffectivePart", new Dictionary<string, string>() { {"C", cost.ToString()}, {"c", dryCost.ToString()}, {"M",wetmass.ToString()},
                    {"m", drymass.ToString()}, {"U", used.ToString()}, {"O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString()}, {"I", InvEff.ToString()}, {"B", KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect.ToString()}});

                    if (InvEff != 0)
                    {
                        --invCopy[name];
                        if (invCopy[name] == 0)
                            invCopy.Remove(name);
                    }
                }
                else //Procedural parts get handled differently
                {
                    double costRemaining = dryCost - (invCopy.ContainsKey(name) ? (invCopy[name] / 1000f) : 0);
                    costRemaining = Math.Max(costRemaining, 0);
                    double costRemoved = dryCost - costRemaining;
                    //C=cost, c=dry cost, A=cost covered by inv, M=wet mass, m=dry mass, U=part tracker, O=overall multiplier, I=inventory effect (0 if not in inv), B=build effect

                    double InvEff = (invCopy.ContainsKey(name) && invCopy[name] > 0) ? KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect : 0;
                    int used = (useTracker && KCT_GameStates.PartTracker.ContainsKey(name)) ? KCT_GameStates.PartTracker[name] : 0;

                    effectiveCost = KCT_MathParsing.GetStandardFormulaValue("ProceduralPart", new Dictionary<string, string>() { {"A", costRemoved.ToString()},{"C", cost.ToString()}, {"c", dryCost.ToString()}, {"M",wetmass.ToString()},
                    {"m", drymass.ToString()}, {"U", used.ToString()}, {"O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString()}, {"I", InvEff.ToString()}, {"B", KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect.ToString()}});

                    if (InvEff != 0)
                    {
                        invCopy[name] -= (int)(costRemoved * 1000);
                        if (invCopy[name] == 0)
                            invCopy.Remove(name);
                    }
                }

                if (effectiveCost < 0) effectiveCost = 0;
                totalEffectiveCost += effectiveCost;
            }
            double finalBP = KCT_MathParsing.GetStandardFormulaValue("BP", new Dictionary<string, string>() { { "E", totalEffectiveCost.ToString() }, { "O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString() } });
            return finalBP;
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
                String name = PartNameFromNode(p);
                double effectiveCost = 0;
                double cost = GetPartCostFromNode(p);
                double dryCost = GetPartCostFromNode(p, false);
                //double wetmass = GetPartMassFromNode(p, true);
                //double drymass = GetPartMassFromNode(p, false);
                float dryMass, fuelMass;
                float wetMass = ShipConstruction.GetPartTotalMass(p, GetAvailablePartByName(name), out dryMass, out fuelMass);
                    
                if (!KCT_Utilities.PartIsProcedural(p))
                {
                    name += GetTweakScaleSize(p);
                    double InvEff = (invCopy.ContainsKey(name) && invCopy[name] > 0) ? KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect : 0;
                    int used = (useTracker && KCT_GameStates.PartTracker.ContainsKey(name)) ? KCT_GameStates.PartTracker[name] : 0;
                    //C=cost, c=dry cost, M=wet mass, m=dry mass, U=part tracker, O=overall multiplier, I=inventory effect (0 if not in inv), B=build effect
                    effectiveCost = KCT_MathParsing.GetStandardFormulaValue("EffectivePart", new Dictionary<string, string>() { {"C", cost.ToString()}, {"c", dryCost.ToString()}, {"M",wetMass.ToString()},
                    {"m", dryMass.ToString()}, {"U", used.ToString()}, {"O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString()}, {"I", InvEff.ToString()}, {"B", KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect.ToString()}});

                    if (InvEff != 0)
                    {
                        --invCopy[name];
                        if (invCopy[name] == 0)
                            invCopy.Remove(name);
                    }
                   /* if (invCopy.Count > 0 && invCopy.ContainsKey(name) && KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect > 0) // If the part is in the inventory, it has a small effect on the total craft
                    {
                        // Combine the part tracker and inventory effect into one so that times will still decrease as you recover+reuse
                        if (useTracker && KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect > 0 && KCT_GameStates.PartTracker.ContainsKey(name))
                            effectiveCost = Math.Min(cost / (KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect + (KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect * (KCT_GameStates.PartTracker[name] + 1))), cost);
                        else // Otherwise the cost is just the normal cost divided by the inventory effect
                            effectiveCost = cost / KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect;
                        --invCopy[name];
                        if (invCopy[name] == 0)
                            invCopy.Remove(name);
                    }
                    else if (useTracker && KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect > 0 && KCT_GameStates.PartTracker.ContainsKey(name)) // The more the part is used, the faster it gets to build
                    {
                        effectiveCost = Math.Min(cost / (KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect * (KCT_GameStates.PartTracker[name] + 1)), cost);
                    }
                    else // If the part has never been used, it takes the maximal time
                    {
                        effectiveCost = cost;
                    }*/
                }
                else //Procedural parts get handled differently
                {
                   // name = PartNameFromNode(p);
                    double costRemaining = dryCost - (invCopy.ContainsKey(name) ? (invCopy[name] / 1000f) : 0);
                    costRemaining = Math.Max(costRemaining, 0);
                    double costRemoved = dryCost - costRemaining;
                    //C=cost, c=dry cost, A=cost covered by inv, M=wet mass, m=dry mass, U=part tracker, O=overall multiplier, I=inventory effect (0 if not in inv), B=build effect

                    double InvEff = (invCopy.ContainsKey(name) && invCopy[name] > 0) ? KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect : 0;
                    int used = (useTracker && KCT_GameStates.PartTracker.ContainsKey(name)) ? KCT_GameStates.PartTracker[name] : 0;

                    effectiveCost = KCT_MathParsing.GetStandardFormulaValue("ProceduralPart", new Dictionary<string, string>() { {"A", costRemoved.ToString()},{"C", cost.ToString()}, {"c", dryCost.ToString()}, {"M",wetMass.ToString()},
                    {"m", dryMass.ToString()}, {"U", used.ToString()}, {"O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString()}, {"I", InvEff.ToString()}, {"B", KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect.ToString()}});

                    if (InvEff != 0)
                    {
                        invCopy[name] -= (int)(costRemoved * 1000);
                        if (invCopy[name] == 0)
                            invCopy.Remove(name);
                    }

                    /*
                    if (invCopy.Count > 0 && invCopy.ContainsKey(name) && KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect > 0)
                    {
                        // Combine the part tracker and inventory effect into one so that times will still decrease as you recover+reuse
                        if (useTracker && KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect > 0 && KCT_GameStates.PartTracker.ContainsKey(name))
                            effectiveCost = (costRemaining + (costRemoved * 10 / KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect)) / Math.Max(KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect * (KCT_GameStates.PartTracker[name] + 1), 1);
                        else // Otherwise the cost is just the normal cost divided by the inventory effect
                            effectiveCost = costRemaining + (costRemoved * 10 / KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect);
                        invCopy[name] -= (int)(costRemoved*1000);
                        if (invCopy[name] == 0)
                            invCopy.Remove(name);
                    }
                    else if (useTracker && KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect > 0 && KCT_GameStates.PartTracker.ContainsKey(name)) // The more the part is used, the faster it gets to build
                    {
                        effectiveCost = Math.Min(cost / (KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect * (KCT_GameStates.PartTracker[name] + 1)), cost);
                    }
                    else // If the part has never been used, it takes the maximal time
                    {
                        effectiveCost = cost;
                    }*/
                }

                if (effectiveCost < 0) effectiveCost = 0;
                totalEffectiveCost += effectiveCost;
            }
            double finalBP = KCT_MathParsing.GetStandardFormulaValue("BP", new Dictionary<string, string>() { { "E", totalEffectiveCost.ToString() }, { "O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString() } });
            return finalBP;
            //return Math.Sqrt(totalEffectiveCost) * 2000 * KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier;
        }

        public static string PartNameFromNode(ConfigNode part)
        {
            string name = part.GetValue("part");
            if (name != null)
                name = name.Split('_')[0];
            else
                name = part.GetValue("name");
            return name;
        }

        public static double GetPartCosts(Part part, bool includeFuel = true)
        {
            double cost = 0;
            cost = part.partInfo.cost + part.GetModuleCosts(part.partInfo.cost);
            foreach (PartResource rsc in part.Resources)
            {
                PartResourceDefinition def = PartResourceLibrary.Instance.GetDefinition(rsc.resourceName);
                if (!includeFuel)
                {
                    cost -= rsc.maxAmount * def.unitCost;
                }
                else //accounts for if you remove some fuel from a tank
                {
                    cost -= (rsc.maxAmount - rsc.amount) * def.unitCost;
                }
            }
            return cost;
        }

        
        public static double GetBuildRate(int index, KCT_BuildListVessel.ListType type, KCT_KSC KSC, bool UpgradedRate = false)
        {
            if (KSC == null) KSC = KCT_GameStates.ActiveKSC;
            double ret = 0;
            if (type == KCT_BuildListVessel.ListType.VAB)
            {
                if (!UpgradedRate && KSC.VABRates.Count > index)
                {
                    return KSC.VABRates[index];
                }
                else if (UpgradedRate && KSC.UpVABRates.Count > index)
                {
                    return KSC.UpVABRates[index];
                }
                else
                {
                    return 0;
                }
            }
            else if (type == KCT_BuildListVessel.ListType.SPH)
            {
                if (!UpgradedRate && KSC.SPHRates.Count > index)
                {
                    return KSC.SPHRates[index];
                }
                else if (UpgradedRate && KSC.UpSPHRates.Count > index)
                {
                    return KSC.UpSPHRates[index];
                }
                else
                {
                    return 0;
                }
            }
            else if (type == KCT_BuildListVessel.ListType.TechNode)
            {
                ret = KCT_GameStates.TechList[index].BuildRate;
            }
            return ret;
        }

        public static double GetBuildRate(KCT_BuildListVessel ship)
        {
            if (ship.type == KCT_BuildListVessel.ListType.None)
                ship.FindTypeFromLists();

            if (ship.type == KCT_BuildListVessel.ListType.VAB)
                return GetBuildRate(ship.KSC.VABList.IndexOf(ship), ship.type, ship.KSC);
            else if (ship.type == KCT_BuildListVessel.ListType.SPH)
                return GetBuildRate(ship.KSC.SPHList.IndexOf(ship), ship.type, ship.KSC);
            else
                return 0;
        }

        public static List<double> BuildRatesVAB(KCT_KSC KSC)
        {
            if (KSC == null) KSC = KCT_GameStates.ActiveKSC;
            return KSC.VABRates;
        }

        public static List<double> BuildRatesSPH(KCT_KSC KSC)
        {
            if (KSC == null) KSC = KCT_GameStates.ActiveKSC;
            return KSC.SPHRates;
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

                    foreach (KCT_Recon_Rollout rr in ksc.Recon_Rollout)
                    {
                        double prog = rr.progress;
                        rr.progress += rr.AsBuildItem().GetBuildRate() * (UT - lastUT);
                        if (rr.progress > rr.BP) rr.progress = rr.BP;

                        if (KCT_Utilities.CurrentGameIsCareer() && rr.RRType == KCT_Recon_Rollout.RolloutReconType.Rollout && rr.cost > 0)
                        {
                            int steps = 0;
                            if ((steps = (int)(Math.Floor((rr.progress/rr.BP)*10) - Math.Floor((prog/rr.BP)*10))) > 0) //passed 10% of the progress
                            {
                                if (Funding.Instance.Funds < rr.cost / 10) //If they can't afford to continue the rollout, progress stops
                                    rr.progress = prog;
                                else
                                    KCT_Utilities.SpendFunds(rr.cost / 10, TransactionReasons.None);
                            }
                        }
                    }

                    ksc.Recon_Rollout.RemoveAll(rr => !KCT_PresetManager.Instance.ActivePreset.generalSettings.ReconditioningTimes || (rr.RRType != KCT_Recon_Rollout.RolloutReconType.Rollout && rr.AsBuildItem().IsComplete()));

                    foreach (KCT_UpgradingBuilding kscTech in ksc.KSCTech)
                    {
                        if (!kscTech.AsIKCTBuildItem().IsComplete()) kscTech.AddProgress(kscTech.AsIKCTBuildItem().GetBuildRate() * (UT - lastUT));
                        if (HighLogic.LoadedScene == GameScenes.SPACECENTER && (kscTech.AsIKCTBuildItem().IsComplete() || !KCT_PresetManager.Instance.ActivePreset.generalSettings.KSCUpgradeTimes))
                        {
                            if (ScenarioUpgradeableFacilities.Instance != null && KCT_GameStates.erroredDuringOnLoad.OnLoadFinished)
                                kscTech.Upgrade();
                        }
                    }
                    if (HighLogic.LoadedScene == GameScenes.SPACECENTER) ksc.KSCTech.RemoveAll(ub => ub.UpgradeProcessed);

                }
                for (int i = 0; i < KCT_GameStates.TechList.Count; i++)
                {
                    KCT_TechItem tech = KCT_GameStates.TechList[i];
                    double buildRate = tech.BuildRate;
                    tech.progress += (buildRate * (UT - lastUT));
                    if (tech.isComplete || !KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUnlockTimes)
                    {
                        if (KCT_GameStates.settings.ForceStopWarp && TimeWarp.CurrentRate > 1f)
                            TimeWarp.SetRate(0, true);
                        if (tech.protoNode == null) continue;
                        tech.EnableTech();
                        KCT_GameStates.TechList.Remove(tech);
                        if (KCT_PresetManager.PresetLoaded() && KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUpgrades)
                            KCT_GameStates.MiscellaneousTempUpgrades++;
                    }
                }
            }
            lastUT = UT;
        }

        public static float GetTotalVesselCost(ProtoVessel vessel, bool includeFuel = true)
        {
            float total = 0, totalDry = 0;
            foreach (ProtoPartSnapshot part in vessel.protoPartSnapshots)
            {
                float dry, wet;
                total += ShipConstruction.GetPartCosts(part, part.partInfo, out dry, out wet);
                totalDry += dry;
            }
            if (includeFuel)
                return total;
            else
                return totalDry;
        }

        public static float GetTotalVesselCost(ConfigNode vessel, bool includeFuel = true)
        {
            float total = 0;
            foreach (ConfigNode part in vessel.GetNodes("PART"))
            {
                total += GetPartCostFromNode(part, includeFuel);
            }
            return total;
        }

        public static float GetPartCostFromNode(ConfigNode part, bool includeFuel = true)
        {
            string name = PartNameFromNode(part);
            float dry, wet;
            float total = ShipConstruction.GetPartCosts(part, GetAvailablePartByName(name), out dry, out wet);
            if (includeFuel)
                return total;
            else
                return dry;
        }

        public static float GetPartMassFromNode(ConfigNode part, bool includeFuel = true)
        {
            float dry = 0, total = 0, wet = 0;
           /* if (float.TryParse(part.GetValue("mass"), out dry))
            {
                //mass += p.GetResourceMass();
                foreach (ConfigNode rsc in part.GetNodes("RESOURCE"))
                {
                    PartResourceDefinition def = PartResourceLibrary.Instance.GetDefinition(rsc.GetValue("name"));
                    wet = dry + def.density * float.Parse(rsc.GetValue("amount"));
                }
            }
            else
            {
                AvailablePart p = KCT_Utilities.GetAvailablePartByName(KCT_Utilities.PartNameFromNode(part));
                if (part != null)
                {
                    dry = p.partPrefab.mass;
                    wet = dry + p.partPrefab.GetResourceMass();
                }
            }*/

            total = ShipConstruction.GetPartTotalMass(part, GetAvailablePartByName(PartNameFromNode(part)), out dry, out wet);
            if (includeFuel)
                return total;
            else
                return dry;
        }

        public static string GetTweakScaleSize(ProtoPartSnapshot part)
        {
            string partSize = "";
            if (part.modules != null)
            {
                ProtoPartModuleSnapshot tweakscale = part.modules.Find(mod => mod.moduleName == "TweakScale");
                if (tweakscale != null)
                {
                    ConfigNode tsCN = tweakscale.moduleValues;
                    string defaultScale = tsCN.GetValue("defaultScale");
                    string currentScale = tsCN.GetValue("currentScale");
                    if (!defaultScale.Equals(currentScale))
                        partSize = "," + currentScale;
                }
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

        public static string GetTweakScaleSize(Part part)
        {
            string partSize = "";
            if (part.Modules != null && part.Modules.Contains("TweakScale"))
            {
                PartModule tweakscale = part.Modules["TweakScale"];
                //ConfigNode tsCN = tweakscale.snapshot.moduleValues;

                object defaultScale = tweakscale.Fields.GetValue("defaultScale");//tsCN.GetValue("defaultScale");
                object currentScale = tweakscale.Fields.GetValue("currentScale");//tsCN.GetValue("currentScale");
                if (!defaultScale.Equals(currentScale))
                    partSize = "," + currentScale.ToString();
            }
            return partSize;
        }

        /*
         * Tests to see if two ConfigNodes have the same information. Currently requires same ordering of subnodes
         * */
        public static Boolean ConfigNodesAreEquivalent(ConfigNode node1, ConfigNode node2)
        {
            //Check that the number of subnodes are equal
            if (node1.GetNodes().Length != node2.GetNodes().Length)
                return false;
            //Check that all the values are identical
            foreach (string valueName in node1.values.DistinctNames())
            {
                if (!node2.HasValue(valueName))
                    return false;
                if (node1.GetValue(valueName) != node2.GetValue(valueName))
                    return false;
            }

            //Check all subnodes for equality
            for (int index = 0; index < node1.GetNodes().Length; ++index)
            {
                if (!ConfigNodesAreEquivalent(node1.nodes[index], node2.nodes[index]))
                    return false;
            }

            //If all these tests pass, we consider the nodes to be equivalent
            return true;
        }

        private static DateTime startedFlashing;
        public static String GetButtonTexture()
        {
            String textureReturn;

            if (!KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled)
                return "KerbalConstructionTime/Icons/KCT_off";

            //Flash for up to 3 seconds, at half second intervals per icon
            if (KCT_GameStates.kctToolbarButton.Important && (DateTime.Now.CompareTo(startedFlashing.AddSeconds(3))) < 0 && DateTime.Now.Millisecond < 500)
                textureReturn = "KerbalConstructionTime/Icons/KCT_off";
            //If it's been longer than 3 seconds, set Important to false and stop flashing
            else if (KCT_GameStates.kctToolbarButton.Important && (DateTime.Now.CompareTo(startedFlashing.AddSeconds(3))) > 0)
            {
                KCT_GameStates.kctToolbarButton.Important = false;
                textureReturn = "KerbalConstructionTime/Icons/KCT_on";
            }
            //The normal icon
            else
                textureReturn = "KerbalConstructionTime/Icons/KCT_on";

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
            if (KCT_GameStates.kctToolbarButton != null)
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
                Message.AppendLine("Please check the VAB Storage at "+KSC.KSCName+" to launch it.");
            
            }
            else if (ListIdentifier == 1)//SPH list
            {
                vessel = KSC.SPHList[index];
                KSC.SPHList.RemoveAt(index);
                KSC.SPHWarehouse.Add(vessel);

                Message.AppendLine(vessel.shipName);
                Message.AppendLine("Please check the SPH Storage at " + KSC.KSCName + " to launch it.");
            }

            //Assign science based on science rate
            double rate = KCT_MathParsing.GetStandardFormulaValue("Research", new Dictionary<string, string>() { { "N", KSC.RDUpgrades[0].ToString() }, { "R", KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.ResearchAndDevelopment).ToString() } });
            if (CurrentGameHasScience() && !vessel.cannotEarnScience && rate > 0)
                AddScienceWithMessage((float)(rate*vessel.buildPoints), TransactionReasons.None);

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
            KCTDebug.Log("Moved vessel " + vessel.shipName + " to " +KSC.KSCName + "'s " + stor + " storage.");

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
            KCT_GUI.ResetBLWindow(false);
            if (!KCT_GameStates.settings.DisableAllMessages)
                DisplayMessage("Vessel Complete!", Message, MessageSystemButton.MessageButtonColor.GREEN, MessageSystemButton.ButtonIcons.COMPLETE);
        }

        public static void AddPartToInventory(ProtoPartSnapshot part)
        {
            string name = part.partInfo.name;
            int amt = 1;
            if (KCT_Utilities.PartIsProcedural(part))
            {
                float cost = part.partInfo.cost + part.moduleCosts;
                KCTDebug.Log("PP cost: " + cost);
                foreach (ProtoPartResourceSnapshot resource in part.resources)
                {
                    cost -= (float)(PartResourceLibrary.Instance.GetDefinition(resource.resourceName).unitCost * float.Parse(resource.resourceValues.GetValue("amount")));
                }
                KCTDebug.Log("After fuel costs: " + cost);
                amt = (int)(cost * 1000);

                AddPartToInventory(name, amt);
                return;
            }
            else
            {
                string tweakscale = GetTweakScaleSize(part); //partName,tweakscale
                name += tweakscale;
                AddPartToInventory(name, amt);
                return;
            }
        }

        public static void AddPartToInventory(Part part)
        {
            string name = part.partInfo.name;
            int amt = 1;
            if (KCT_Utilities.PartIsProcedural(part))
            {
                float cost = part.partInfo.cost + part.GetModuleCosts(0);
                KCTDebug.Log("PP cost: " + cost);
                foreach (PartResource resource in part.Resources.list)
                {
                    cost -= (float)(PartResourceLibrary.Instance.GetDefinition(resource.resourceName).unitCost * resource.amount);
                }
                KCTDebug.Log("After fuel costs: " + cost);
                amt = (int)(cost * 1000);

                AddPartToInventory(name, amt);
                return;
            }
            else
            {
                string tweakscale = GetTweakScaleSize(part.protoPartSnapshot); //partName,tweakscale
                string nameToStore = part.partInfo.name + tweakscale;
                AddPartToInventory(nameToStore, amt);
                return;
            }
        }
        public static void AddPartToInventory(ConfigNode part)
        {
            string name = PartNameFromNode(part) + GetTweakScaleSize(part);
            int amt = 1;
            if (KCT_Utilities.PartIsProcedural(part))
            {
                name = PartNameFromNode(part);
                amt = (int)(1000 * GetPartCostFromNode(part, false));
            }
            AddPartToInventory(name, amt);
        }
        public static void AddPartToInventory(String name)
        {
            AddPartToInventory(name, 1);
        }
        public static void AddPartToInventory(String name, int amt)
        {
            if (KCT_GameStates.PartInventory.ContainsKey(name))
            {
                KCT_GameStates.PartInventory[name] += amt;
            }
            else
            {
                KCT_GameStates.PartInventory.Add(name, amt);
            }
            KCTDebug.Log("Added "+amt+"x"+name+" to part inventory");
        }


        public static int RemovePartFromInventory(Part part)
        {
            string name = part.partInfo.name;
            int amt = 1;
            if (KCT_Utilities.PartIsProcedural(part))
            {
                float cost = part.partInfo.cost + part.GetModuleCosts(0);
                foreach (PartResource resource in part.Resources.list)
                {
                    cost -= (float)(PartResourceLibrary.Instance.GetDefinition(resource.resourceName).unitCost * resource.maxAmount);
                }
                amt = (int)(cost * 1000);
            }
            else
                name += GetTweakScaleSize(part.protoPartSnapshot); 
            /*string tweakscale = GetTweakScaleSize(part.protoPartSnapshot); //partName,tweakscale
            string nameToStore = part.partInfo.name + tweakscale;
            return RemovePartFromInventory(nameToStore);*/
            return RemovePartFromInventory(name, amt);
        }
        public static int RemovePartFromInventory(Part part, Dictionary<String, int> inventory)
        {
            string name = part.partInfo.name;
            int amt = 1;
            if (KCT_Utilities.PartIsProcedural(part))
            {
                float cost = part.partInfo.cost + part.GetModuleCosts(0);
                foreach (PartResource resource in part.Resources.list)
                {
                    cost -= (float)(PartResourceLibrary.Instance.GetDefinition(resource.resourceName).unitCost * resource.maxAmount);
                }
                amt = (int)(cost * 1000);
            }
            else
                name += GetTweakScaleSize(part.protoPartSnapshot); 
            /*string tweakscale = GetTweakScaleSize(part.protoPartSnapshot); //partName,tweakscale
            string nameToStore = part.partInfo.name + tweakscale;*/
            return RemovePartFromInventory(name, inventory, amt);
        }
        public static int RemovePartFromInventory(ConfigNode part)
        {
            string name = PartNameFromNode(part) + GetTweakScaleSize(part);
            int amt = 1;
            if (KCT_Utilities.PartIsProcedural(part))
            {
                name = PartNameFromNode(part);
                amt = (int)(1000 * GetPartCostFromNode(part, false));
            }
            //return RemovePartFromInventory(PartNameFromNode(part) + GetTweakScaleSize(part));
            return RemovePartFromInventory(name, amt);
        }
        public static int RemovePartFromInventory(ConfigNode part, Dictionary<String, int> inventory)
        {
            string name = PartNameFromNode(part) + GetTweakScaleSize(part);
            int amt = 1;
            if (KCT_Utilities.PartIsProcedural(part))
            {
                name = PartNameFromNode(part);
                amt = (int)(1000 * GetPartCostFromNode(part, false));
            }
            //return RemovePartFromInventory(PartNameFromNode(part) + GetTweakScaleSize(part), inventory, 1);
            return RemovePartFromInventory(name, inventory, amt);
        }
        public static int RemovePartFromInventory(String name, int amt)
        {
            return RemovePartFromInventory(name, KCT_GameStates.PartInventory, amt);
        }
        public static int RemovePartFromInventory(String name, Dictionary<String, int> inventory, int amt)
        {
            int removed = 0;
            if (inventory.ContainsKey(name))
            {
                removed = inventory[name] >= amt ? amt : inventory[name];
                inventory[name] -= removed;
                if (inventory[name] == 0)
                {
                    inventory.Remove(name);
                }
                KCTDebug.Log("Removed " + removed + "x" + name + " from part inventory");
            }
            return removed;
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
            //System.IO.File.Copy(saveFile, backupFile, true);
            GamePersistence.SaveGame("KCT_simulation_backup", HighLogic.SaveFolder, SaveMode.OVERWRITE);
        }

        public static void LoadSimulationSave(bool newMethod)
        {
            string backupFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs";
            string saveFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
            KCT_Utilities.disableSimulationLocks();
            KCT_GameStates.flightSimulated = false;
            KerbalConstructionTime.moved = false;
            KCT_GameStates.simulationEndTime = 0;
            KCT_GameStates.RemoteTechEnabled = true;
            KCTDebug.Log("Swapping persistent.sfs with simulation backup file.");
            if (newMethod)
            {
                ConfigNode lastShip = ShipConstruction.ShipConfig;
                EditorFacility lastEditor = HighLogic.CurrentGame.editorFacility;

                Game newGame = GamePersistence.LoadGame("KCT_simulation_backup", HighLogic.SaveFolder, true, false);
                GamePersistence.SaveGame(newGame, "persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                GameScenes targetScene = HighLogic.LoadedScene;
                newGame.startScene = targetScene;

                // This has to be before... newGame.Start()
                if (targetScene == GameScenes.EDITOR)
                {
                    newGame.editorFacility = lastEditor;
                }


                newGame.Start();

                // ... And this has to be after. <3 KSP
                if (targetScene == GameScenes.EDITOR)
                {
                    EditorDriver.StartupBehaviour = EditorDriver.StartupBehaviours.LOAD_FROM_CACHE;
                    ShipConstruction.ShipConfig = lastShip;
                }
            }
            else
            {
                System.IO.File.Copy(backupFile, saveFile, true);
            }
            System.IO.File.Delete(backupFile);
            
            //GamePersistence.LoadGame("KCT_simulation_backup", HighLogic.SaveFolder, true, false);
            //System.IO.File.Delete(backupFile);
            KCT_GameStates.LoadingSimulationSave = false;
            KCT_GameStates.simulationInitialized = false;

            KCT_GameStates.TestFlightPartFailures = true;
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

        public static float CostOfSimulation(CelestialBody body, string simulationLength, ShipConstruct ship, int SimCount, bool landed)
        {
            if (simulationLength == "" || simulationLength == "-1")
                simulationLength = "31536000000"; //1000 Earth years
            CelestialBody Kerbin = GetBodyByName("Kerbin");
            if (Kerbin == null)
                Kerbin = GetBodyByName("Earth");

            double length = KCT_Utilities.ParseColonFormattedTime(simulationLength, false);
            length = Math.Min(length, 31536000000.0);
            if (length <= 0)
                length = 31536000000.0;
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("L", length.ToString()); //Sim length in seconds
            vars.Add("M", body.Mass.ToString()); //Body mass
            vars.Add("KM", Kerbin.Mass.ToString()); //Kerbin mass
            vars.Add("A", body.atmosphere ? "1" : "0"); //Presence of atmosphere
            vars.Add("S", (body != Planetarium.fetch.Sun && body.referenceBody != Planetarium.fetch.Sun) ? "1" : "0"); //Is a moon (satellite)

            float out1, out2;
            vars.Add("m", ship.GetTotalMass().ToString()); //Vessel loaded mass
            vars.Add("C", ship.GetShipCosts(out out1, out out2).ToString()); //Vessel loaded cost

            vars.Add("s", SimCount.ToString()); //Number of times simulated this editor session


            CelestialBody Parent = body;
            if (Parent != Planetarium.fetch.Sun)
            {
                while (Parent.referenceBody != Planetarium.fetch.Sun)
                {
                    Parent = Parent.referenceBody;
                }
            }
            double orbitRatio = 1;
            if (Parent.orbit != null)
            {
                if (Parent.orbit.semiMajorAxis >= Kerbin.orbit.semiMajorAxis)
                    orbitRatio = Parent.orbit.semiMajorAxis / Kerbin.orbit.semiMajorAxis;
                else
                    orbitRatio = Kerbin.orbit.semiMajorAxis / Parent.orbit.semiMajorAxis;
            }
            vars.Add("SMA", orbitRatio.ToString());
            vars.Add("PM", Parent.Mass.ToString());

            if ((body.bodyName == "Kerbin" || body.bodyName == "Earth") && landed)
            {
                return (float)(KCT_MathParsing.GetStandardFormulaValue("KerbinSimCost", vars));
            }
            else
            {
                return (float)(KCT_MathParsing.GetStandardFormulaValue("SimCost", vars));
            }



            /*float timeMultiplier = 13;
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
            if (Kerbin == null)
                Kerbin = GetBodyByName("Earth");

            double orbitRatio = 1;
            if (Parent.orbit.semiMajorAxis >= Kerbin.orbit.semiMajorAxis)
                orbitRatio = Parent.orbit.semiMajorAxis / Kerbin.orbit.semiMajorAxis;
            else
                orbitRatio = Kerbin.orbit.semiMajorAxis / Parent.orbit.semiMajorAxis;

            double cost = Math.Pow(orbitRatio,2) * 500 * (Parent.atmosphere ? 1.1 : 1);
            if (isMoon)
                cost *= atmosphereMult * 1.1;

            cost *= timeMultiplier;
            return (float)cost;*/
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
                //Check upgrades
                //First, mass limit
                List<string> facilityChecks = blv.MeetsFacilityRequirements();
                if (facilityChecks.Count != 0)
                {
                    //ScreenMessages.PostScreenMessage("Did not pass editor checks!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    //ScreenMessages.PostScreenMessage(failedReason, 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    PopupDialog.SpawnPopupDialog("Failed editor checks!", "Warning! This vessel did not pass the editor checks! It will still be built, but you will not be able to launch it without upgrading. Listed below are the failed checks:\n" + String.Join("\n", facilityChecks.ToArray()), "Acknowledged", false, HighLogic.Skin);
                    //return null;
                }


                double totalCost = blv.GetTotalCost();
                double prevFunds = Funding.Instance.Funds;
                //double newFunds = SpendFunds(totalCost, TransactionReasons.VesselRollout);
                if (totalCost > prevFunds)
                {
                    KCTDebug.Log("Tried to add " + blv.shipName + " to build list but not enough funds.");
                    KCTDebug.Log("Vessel cost: " + GetTotalVesselCost(blv.shipNode) + ", Current funds: " + prevFunds);
                    var msg = new ScreenMessage("Not Enough Funds To Build!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    ScreenMessages.PostScreenMessage(msg, true);
                    return null;
                }
                else
                {
                    SpendFunds(totalCost, TransactionReasons.VesselRollout);
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
                   // if (KCT_Utilities.RemovePartFromInventory(p, inventory))
                    {
                        if (!KCT_Utilities.PartIsProcedural(p))
                            AddToDict(blv.InventoryParts, PartNameFromNode(p) + GetTweakScaleSize(p), KCT_Utilities.RemovePartFromInventory(p, inventory));
                           // blv.InventoryParts.Add(PartNameFromNode(p) + GetTweakScaleSize(p), 1);
                        else
                            AddToDict(blv.InventoryParts, PartNameFromNode(p), KCT_Utilities.RemovePartFromInventory(p, inventory));
                           // blv.InventoryParts.Add(PartNameFromNode(p), (int)(1000 * GetPartCostFromNode(p, false)));
                    }
                }
            }
            KCTDebug.Log("Added " + blv.shipName + " to " + type + " build list at KSC "+KCT_GameStates.ActiveKSC.KSCName+". Cost: "+blv.cost);
            KCTDebug.Log("Launch site is " + blv.launchSite);
            //KCTDebug.Log("Cost Breakdown (total, parts, fuel): " + blv.totalCost + ", " + blv.dryCost + ", " + blv.fuelCost);
            var message = new ScreenMessage("[KCT] Added " + blv.shipName + " to " + type + " build list.", 4.0f, ScreenMessageStyle.UPPER_CENTER);
            ScreenMessages.PostScreenMessage(message, true);
            return blv;
        }

        public static IKCTBuildItem NextThingToFinish()
        {
            IKCTBuildItem thing = null;
            if (KCT_GameStates.ActiveKSC == null)
                return null;
            double shortestTime = double.PositiveInfinity;
            foreach (KCT_KSC KSC in KCT_GameStates.KSCs)
            {
                foreach (IKCTBuildItem blv in KSC.VABList)
                {
                    double time = blv.GetTimeLeft();
                    if (time < shortestTime)
                    {
                        thing = blv;
                        shortestTime = time;
                    }
                }
                foreach (IKCTBuildItem blv in KSC.SPHList)
                {
                    double time = blv.GetTimeLeft();
                    if (time < shortestTime)
                    {
                        thing = blv;
                        shortestTime = time;
                    }
                }
                
                foreach (IKCTBuildItem rr in KSC.Recon_Rollout)
                {
                    if (rr.IsComplete())
                        continue;
                    double time = rr.GetTimeLeft();
                    if (time < shortestTime)
                    {
                        thing = rr;
                        shortestTime = time;
                    }
                }
                foreach (IKCTBuildItem ub in KSC.KSCTech)
                {
                    if (ub.IsComplete())
                        continue;
                    double time = ub.GetTimeLeft();
                    if (time < shortestTime)
                    {
                        thing = ub;
                        shortestTime = time;
                    }
                }
            }
            /*if (KCT_Utilities.ReconditioningActive(null))
            {
                IKCTBuildItem blv = (IKCTBuildItem)KCT_GameStates.ActiveKSC.GetReconditioning();
                double time = blv.GetTimeLeft();
                if (time < shortestTime)
                {
                    thing = blv;
                    shortestTime = time;
                }
            }*/
            foreach (IKCTBuildItem blv in KCT_GameStates.TechList)
            {
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
            /*foreach (CelestialBody b in FlightGlobals.Bodies)
            {
                if (b.bodyName.ToLower() == name.ToLower())
                    return b;
            }*/
            return FlightGlobals.Bodies.FirstOrDefault(b => b.bodyName.ToLower() == name.ToLower());
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

        public static int TotalSpentUpgrades(KCT_KSC ksc)
        {
            if (ksc == null) ksc = KCT_GameStates.ActiveKSC;
            int spentPoints = 0;
            foreach (int i in ksc.VABUpgrades) spentPoints += i;
            foreach (int i in ksc.SPHUpgrades) spentPoints += i;
            foreach (int i in ksc.RDUpgrades) spentPoints += i;
            return spentPoints;
        }

       /* public static float GetRecoveryValueForChuteLanding(ProtoVessel pv)
        {
            bool probeCoreAttached = false;
           // foreach (ProtoPartSnapshot pps in pv.protoPartSnapshots)
            {
                //if (pps.modules.Find(module => (module.moduleName == "ModuleCommand" && ((ModuleCommand)module.moduleRef).minimumCrew == 0)) != null)
                if (pv.wasControllable)
                {
                    KCTDebug.Log("Vessel was controllable!");
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
        }*/

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

        public static Type TestFlightInterface;
        private static bool? _TestFlightInstalled = null;
        public static bool TestFlightInstalled
        {
            get
            {
                if (_TestFlightInstalled == null)
                {
                    TestFlightInterface = AssemblyLoader.loadedAssemblies
                    .Select(a => a.assembly.GetExportedTypes())
                    .SelectMany(t => t)
                    .FirstOrDefault(t => t.FullName == "TestFlightCore.TestFlightInterface");

                    _TestFlightInstalled = (TestFlightInterface != null);
                }
                return (_TestFlightInstalled == null ? false : (bool)_TestFlightInstalled);
            }
        }

        private static bool? _RTInstalled = null;
        public static bool RemoteTechInstalled
        {
            get
            {
                if (_RTInstalled == null)
                {
                    Type RTCore = AssemblyLoader.loadedAssemblies
                    .Select(a => a.assembly.GetExportedTypes())
                    .SelectMany(t => t)
                    .FirstOrDefault(t => t.FullName == "RemoteTech.RTCore");

                    _RTInstalled = (RTCore != null);
                }
                return (_RTInstalled == null ? false : (bool)_RTInstalled);
            }
        }

        public static void DisableRemoteTechLocks()
        {
            /*InputLockManager.RemoveControlLock("RTLockStaging");
            InputLockManager.RemoveControlLock("RTLockSAS");
            InputLockManager.RemoveControlLock("RTLockRCS");
            InputLockManager.RemoveControlLock("RTLockActions");*/
           /* Type RTSettings = AssemblyLoader.loadedAssemblies
                    .Select(a => a.assembly.GetExportedTypes())
                    .SelectMany(t => t)
                    .FirstOrDefault(t => t.FullName == "RemoteTech.RTSettings");

            if (RTSettings == null)
                return;

            var RTSettingInstance = GetMemberInfoValue(RTSettings.GetMember("Instance")[0], RTSettings);

            System.Reflection.PropertyInfo multiplier = RTSettingInstance.GetType().GetProperty("RangeMultiplier");
            multiplier.SetValue(RTSettingInstance, float.PositiveInfinity, null);*/

            //Not working when an antenna is also attached
            Type RTCore = AssemblyLoader.loadedAssemblies
                    .Select(a => a.assembly.GetExportedTypes())
                    .SelectMany(t => t)
                    .FirstOrDefault(t => t.FullName == "RemoteTech.RTCore");
            if (RTCore == null)
                return;
            System.Object RTInstance = GetMemberInfoValue(RTCore.GetMember("Instance")[0], null);


            foreach (Part p in FlightGlobals.ActiveVessel.parts)
            {
                if (p.Modules.Contains("ModuleRTAntenna"))
                {
                    PartModule antenna = p.Modules["ModuleRTAntenna"];
                    if (antenna != null)
                    {
                        //System.Reflection.MethodInfo onDestroy = antenna.GetType().GetMethod("OnDestroy");
                        //onDestroy.Invoke(antenna, null);
                        System.Object RTAntennas = GetMemberInfoValue(RTInstance.GetType().GetMember("Antennas")[0], RTInstance);
                        System.Reflection.MethodInfo unRegisterMethod = RTAntennas.GetType().GetMethod("Unregister");
                        unRegisterMethod.Invoke(RTAntennas, new object[] { FlightGlobals.ActiveVessel.id, antenna });

                        System.Reflection.MethodInfo unRegisterProtosMethod = RTAntennas.GetType().GetMethod("UnregisterProtos");
                        unRegisterProtosMethod.Invoke(RTAntennas, new object[] { FlightGlobals.ActiveVessel.id });
                    }
                }
             /*   if (p.Modules.Contains("ModuleSPU"))
                {
                    PartModule spu = p.Modules["ModuleSPU"];
                    if (spu != null)
                    {
                        System.Reflection.MethodInfo onDestroy = spu.GetType().GetMethod("OnDestroy");
                        onDestroy.Invoke(spu, null);
                    }
                }*/
            }

            foreach (Part p in FlightGlobals.ActiveVessel.parts)
            {
                if (p.Modules.Contains("ModuleSPU"))
                {
                    PartModule spu = p.Modules["ModuleSPU"];
                    if (spu != null)
                    {
                        System.Reflection.MethodInfo onDestroy = spu.GetType().GetMethod("OnDestroy");
                        onDestroy.Invoke(spu, null);
                    }
                }
            }

            InputLockManager.RemoveControlLock("RTLockStaging");
            InputLockManager.RemoveControlLock("RTLockSAS");
            InputLockManager.RemoveControlLock("RTLockRCS");
            InputLockManager.RemoveControlLock("RTLockActions");
        }

        public static void EnableRemoteTechLocks()
        {

            //Not quite working properly
            /*Type RTCore = AssemblyLoader.loadedAssemblies
                    .Select(a => a.assembly.GetExportedTypes())
                    .SelectMany(t => t)
                    .FirstOrDefault(t => t.FullName == "RemoteTech.RTCore");

            System.Object RTInstance = GetMemberInfoValue(RTCore.GetMember("Instance")[0], null);
            System.Object RTSatellites = GetMemberInfoValue(RTInstance.GetType().GetMember("Satellites")[0], RTInstance);
            Type RTSatType = RTSatellites.GetType();
            System.Reflection.MethodInfo registerMethod = RTSatType.GetMethod("RegisterProto");
            registerMethod.Invoke(RTSatellites, new object[] { FlightGlobals.ActiveVessel });*/
        }

        public static List<string> GetAllOpenKKLaunchSites(string type = "RocketPad")
        {
            List<string> sites = new List<string>();
            Type KKLaunchSiteManager = AssemblyLoader.loadedAssemblies
                    .Select(a => a.assembly.GetExportedTypes())
                    .SelectMany(t => t)
                    .FirstOrDefault(t => t.FullName == "KerbalKonstructs.LaunchSites.LaunchSiteManager");
            if (KKLaunchSiteManager == null)
            {
                if (type == "RocketPad")
                    sites.Add("LaunchPad");
                else
                    sites.Add("Runway");
                return sites;
            }
            KCTDebug.Log("Site manager is good.");

            System.Object siteProperty = GetMemberInfoValue(KKLaunchSiteManager.GetProperty("AllLaunchSites", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy), null);
            if (siteProperty == null)
            {
                KCTDebug.Log("siteProperty null");
                if (type == "RocketPad")
                    sites.Add("LaunchPad");
                else
                    sites.Add("Runway");
                return sites;
            }

            foreach (var launchSite in siteProperty as System.Collections.IList)
            {
                //get each launch site available
                string name = GetMemberInfoValue(launchSite.GetType().GetMember("name")[0], launchSite) as string;
                string openState = GetMemberInfoValue(launchSite.GetType().GetMember("openclosestate")[0], launchSite) as string;
                string category = GetMemberInfoValue(launchSite.GetType().GetMember("category")[0], launchSite) as string;

                KCTDebug.Log("Launchsite info: Name: " + name + " Status: " + openState + " Category: " + category);
                if (category == type && (!CurrentGameIsCareer() || openState == "Open"))
                    sites.Add(name);
            }
            
            return sites;
        }

        private static bool? _KSCSwitcherInstalled = null;
        public static bool KSCSwitcherInstalled
        {
            get
            {
                if (_KSCSwitcherInstalled == null)
                {
                    Type Switcher = AssemblyLoader.loadedAssemblies
                    .Select(a => a.assembly.GetExportedTypes())
                    .SelectMany(t => t)
                    .FirstOrDefault(t => t.FullName == "regexKSP.KSCSwitcher");

                    _KSCSwitcherInstalled = (Switcher != null);

                    //KCTDebug.Log("KSCSwitcher status: " + _KSCSwitcherInstalled);
                }
                return (_KSCSwitcherInstalled == null ? false : (bool)_KSCSwitcherInstalled);
            }
        }

        public static string GetActiveRSSKSC()
        {
            if (!KSCSwitcherInstalled) return "Stock";
            Type Switcher = AssemblyLoader.loadedAssemblies
                    .Select(a => a.assembly.GetExportedTypes())
                    .SelectMany(t => t)
                    .FirstOrDefault(t => t.FullName == "regexKSP.KSCSwitcher");

            UnityEngine.Object KSCSwitcherInstance = GameObject.FindObjectOfType(Switcher);

            return (string)GetMemberInfoValue(Switcher.GetMember("activeSite")[0], KSCSwitcherInstance);

            //System.Reflection.FieldInfo site = RSS.GetField("activeSite");

            //return (string)KCT_Utilities.GetMemberInfoValue(site, null);
        }

        public static void SetActiveKSCToRSS()
        {
            string site = GetActiveRSSKSC();
            SetActiveKSC(site);
            /*
            if (site == "") site = "Stock";
            if (KCT_GameStates.ActiveKSC == null || site != KCT_GameStates.ActiveKSC.KSCName)
            {
                KCTDebug.Log("Setting active site to " + site);
                KCT_KSC setActive = KCT_GameStates.KSCs.FirstOrDefault(ksc => ksc.KSCName == site);
                if (setActive != null)
                {
                    KCT_GameStates.ActiveKSC = setActive;
                }
                else
                {
                    setActive = new KCT_KSC(site);
                    KCT_GameStates.KSCs.Add(setActive);
                    KCT_GameStates.ActiveKSC = setActive;
                }
                KCT_GameStates.activeKSCName = site;
            }*/
        }

        public static void SetActiveKSC(string site)
        {
            if (site == "") site = "Stock";
            if (KCT_GameStates.ActiveKSC == null || site != KCT_GameStates.ActiveKSC.KSCName)
            {
                KCTDebug.Log("Setting active site to " + site);
                KCT_KSC setActive = KCT_GameStates.KSCs.FirstOrDefault(ksc => ksc.KSCName == site);
                if (setActive != null)
                {
                    KCT_GameStates.ActiveKSC = setActive;
                }
                else
                {
                    setActive = new KCT_KSC(site);
                    KCT_GameStates.KSCs.Add(setActive);
                    KCT_GameStates.ActiveKSC = setActive;
                }
            }
            KCT_GameStates.activeKSCName = site;
        }

        public static void DisplayMessage(String title, StringBuilder text, MessageSystemButton.MessageButtonColor color, MessageSystemButton.ButtonIcons icon)
        {
            MessageSystem.Message m = new MessageSystem.Message(title, text.ToString(), color, icon);
            MessageSystem.Instance.AddMessage(m);
        }

        public static bool LaunchFacilityIntact(KCT_BuildListVessel.ListType type)
        {
            bool intact = true;
            if (type == KCT_BuildListVessel.ListType.VAB)
            {
                intact = new PreFlightTests.FacilityOperational("LaunchPad", "building").Test();
            }
            else if (type == KCT_BuildListVessel.ListType.SPH)
            {
               /* if (!new PreFlightTests.FacilityOperational("Runway", "End09").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "End27").Test())
                    intact = false;*/
                if (!new PreFlightTests.FacilityOperational("Runway", "Section1").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "Section2").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "Section3").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "Section4").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "Section5").Test())
                    intact = false;
            }
            return intact;
        }

        public static void RecalculateEditorBuildTime(ShipConstruct ship)
        {
            if (!HighLogic.LoadedSceneIsEditor)
                return;
            //KCTDebug.Log("Recalculating build time");
            //List<ConfigNode> partNodes = ship.SaveShip().GetNodes("PART").ToList();
            KCT_GUI.PartsInUse.Clear();
            if (KCT_GUI.useInventory)
            {
                foreach (Part part in ship.parts)
                {
                    string name = part.partInfo.name;
                    int amt = 1;
                    if (!PartIsProcedural(part))
                    {
                        name += GetTweakScaleSize(part);
                    }
                    else
                    {
                        
                       /* float dryCost, fuelCost;
                        ShipConstruction.GetPartCosts(partNode, part.partInfo, out dryCost, out fuelCost);*/
                        amt = (int)(1000 * GetPartCosts(part));
                    }
                    if (!KCT_GUI.PartsInUse.ContainsKey(name))
                        KCT_GUI.PartsInUse.Add(name, amt);
                    else
                        KCT_GUI.PartsInUse[name] += amt;
                }
            }

            if (!KCT_GameStates.EditorShipEditingMode)
            {
                KCT_GameStates.EditorBuildTime = KCT_Utilities.GetBuildTime(ship.Parts, true, KCT_GUI.useInventory);
                KCT_GameStates.EditorRolloutCosts = KCT_MathParsing.ParseRolloutCostFormula(new KCT_BuildListVessel(ship, EditorLogic.fetch.launchSiteName, KCT_GameStates.EditorBuildTime, EditorLogic.FlagURL));
            }
            else
            {
                //List<string> partsForInventory = new List<string>();
                Dictionary<string, int> partsForInventory = new Dictionary<string, int>();
                if (KCT_GUI.useInventory)
                {
                    Dictionary<string, int> newParts = new Dictionary<string, int>(KCT_GUI.PartsInUse);
                    Dictionary<string, int> theInventory = new Dictionary<string, int>(KCT_GameStates.PartInventory);
                    foreach (KeyValuePair<string, int> kvp in KCT_GameStates.EditedVesselParts)
                    {
                        if (newParts.ContainsKey(kvp.Key))
                        {
                            if (newParts[kvp.Key] >= kvp.Value)
                            {
                                newParts[kvp.Key] -= kvp.Value;
                            }
                            else
                            {
                                newParts[kvp.Key] = 0;
                            }
                        }
                    }


                    /*foreach (string s in PartDictToList(KCT_GameStates.EditedVesselParts))
                        if (newParts.Contains(s))
                            newParts.Remove(s);*/

                    foreach (KeyValuePair<string, int> kvp in newParts)
                    {
                        if (theInventory.ContainsKey(kvp.Key))
                        {
                            if (theInventory[kvp.Key] >= kvp.Value)
                            {
                                theInventory[kvp.Key] -= kvp.Value;
                                KCT_Utilities.AddToDict(partsForInventory, kvp.Key, kvp.Value);
                            }
                            else
                            {
                                KCT_Utilities.AddToDict(partsForInventory, kvp.Key, theInventory[kvp.Key]);
                                theInventory[kvp.Key] = 0;
                            }
                            // theInventory.Remove(s);
                            // partsForInventory.Add(s);
                        }
                    }
                }
                foreach (KeyValuePair<string, int> kvp in KCT_GameStates.editedVessel.InventoryParts)
                    KCT_Utilities.AddToDict(partsForInventory, kvp.Key, kvp.Value);

                KCT_GameStates.EditorBuildTime = KCT_Utilities.GetBuildTime(ship.parts, true, partsForInventory);

                KCT_GameStates.EditorRolloutCosts = KCT_MathParsing.ParseRolloutCostFormula(new KCT_BuildListVessel(ship, EditorLogic.fetch.launchSiteName, KCT_GameStates.EditorBuildTime, EditorLogic.FlagURL));

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
                    string[] split = mi.info.Split(new Char[] {':', '\n'});
                    //TODO: Get SR code and put that in here, maybe with TryParse instead of Parse
                    for (int i=0; i<split.Length; i++)
                    {
                        if (split[i].Contains("Fully-Deployed Drag"))
                        {
                            float drag = 500;
                            if (!float.TryParse(split[i+1], out drag))
                            {
                                string[] split2 = split[i + 1].Split('>');
                                if (!float.TryParse(split2[1], out drag))
                                {
                                    Debug.Log("[KCT] Failure trying to read parachute data. Assuming 500 drag.");
                                    drag = 500;
                                }
                            }
                            return drag;
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

        public static bool ReconditioningActive(KCT_KSC KSC, string launchSite = "LaunchPad")
        {
            if (KSC == null) KSC = KCT_GameStates.ActiveKSC;

            KCT_Recon_Rollout recon = KSC.GetReconditioning(launchSite);
            return (recon != null);
        }

        public static KCT_BuildListVessel FindBLVesselByID(Guid id)
        {
            KCT_BuildListVessel ret = null;
            foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
            {
                KCT_BuildListVessel tmp = ksc.VABList.Find(v => v.id == id);
                if (tmp != null)
                {
                    ret = tmp;
                    break;
                }
                tmp = ksc.SPHList.Find(v => v.id == id);
                if (tmp != null)
                {
                    ret = tmp;
                    break;
                }
                tmp = ksc.VABWarehouse.Find(v => v.id == id);
                if (tmp != null)
                {
                    ret = tmp;
                    break;
                }
                tmp = ksc.SPHWarehouse.Find(v => v.id == id);
                if (tmp != null)
                {
                    ret = tmp;
                    break;
                }
            }
            return ret;
        }

        /**
         * Don't actually use this!
         * */
        public static ConfigNode ProtoVesselToCraftFile(ProtoVessel vessel)
        {
            ConfigNode craft = new ConfigNode("ShipNode");
            ConfigNode pvNode = new ConfigNode();
            vessel.Save(pvNode);
            //KCTDebug.Log(pvNode);

            craft.AddValue("ship", pvNode.GetValue("name"));
            craft.AddValue("version", Versioning.GetVersionString());
            craft.AddValue("description", "Craft file converted automatically by Kerbal Construction Time.");
            craft.AddValue("type", "VAB");
            ConfigNode[] parts = pvNode.GetNodes("PART");
            foreach (ConfigNode part in parts)
            {
                ConfigNode newPart = new ConfigNode("PART");
                newPart.AddValue("part", part.GetValue("name") + "_" + part.GetValue("uid"));
                newPart.AddValue("partName", "Part");
                newPart.AddValue("pos", part.GetValue("position"));
                newPart.AddValue("rot", part.GetValue("rotation"));
                newPart.AddValue("attRot", part.GetValue("rotation"));
                newPart.AddValue("mir", part.GetValue("mirror"));
                newPart.AddValue("istg", part.GetValue("istg"));
                newPart.AddValue("dstg", part.GetValue("dstg"));
                newPart.AddValue("sidx", part.GetValue("sidx"));
                newPart.AddValue("sqor", part.GetValue("sqor"));
                newPart.AddValue("attm", part.GetValue("attm"));
                newPart.AddValue("modCost", part.GetValue("modCost"));

                foreach (string attn in part.GetValues("attN"))
                {
                    string attach_point = attn.Split(',')[0];
                    if (attach_point == "None")
                        continue;
                    int attachedIndex = int.Parse(attn.Split(',')[1]);
                    string attached = parts[attachedIndex].GetValue("name") + "_" + parts[attachedIndex].GetValue("uid");
                    newPart.AddValue("link", attached);
                    newPart.AddValue("attN", attach_point + "," + attached);
                }

                newPart.AddNode(part.GetNode("EVENTS"));
                newPart.AddNode(part.GetNode("ACTIONS"));
                foreach (ConfigNode mod in part.GetNodes("MODULE"))
                    newPart.AddNode(mod);
                foreach (ConfigNode rsc in part.GetNodes("RESOURCE"))
                    newPart.AddNode(rsc);
                craft.AddNode(newPart);
            }


            return craft;
        }

        public static void AddToDict(Dictionary<string, int> dict, string key, int value)
        {
            if (value <= 0) return;
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] += value;
        }

        public static bool RemoveFromDict(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
                return false;
            else if (dict[key] < value)
                return false;
            else
            {
                dict[key] -= value;
                return true;
            }
                
        }

        public static bool PartIsProcedural(ConfigNode part)
        {
            ConfigNode[] modules = part.GetNodes("MODULE");
            if (modules == null)
                return false;
            foreach (ConfigNode mod in modules)
            {
                if (mod.HasValue("name") && mod.GetValue("name").ToLower().Contains("procedural"))
                    return true;
            }
            return false;
        }

        public static bool PartIsProcedural(ProtoPartSnapshot part)
        {
            if (part.modules != null)
                return part.modules.Find(m => m != null && m.moduleName != null && m.moduleName.ToLower().Contains("procedural")) != null;
            return false;
        }

        public static bool PartIsProcedural(Part part)
        {
            if (part.Modules != null)
            {
                for (int i = 0; i < part.Modules.Count; i++ )
                {
                    if (part.Modules[i].moduleName.ToLower().Contains("procedural"))
                        return true;
                }
            }
            return false;
        }

        public static int BuildingUpgradeLevel(SpaceCenterFacility facility)
        {
            int lvl = 0;
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                lvl = (int)(2*ScenarioUpgradeableFacilities.GetFacilityLevel(facility));
            }
            else
            {
                //lvl = ScenarioUpgradeableFacilities.GetFacilityLevelCount(facility);
                lvl = 2;
            }
            return lvl;
        }


        public static int TotalUpgradePoints()
        {
            int total = 0;
            //Starting points
            total += KCT_PresetManager.Instance.StartingUpgrades(HighLogic.CurrentGame.Mode);
            //R&D
            if (KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUpgrades)
            {
                //Completed tech nodes
                if (CurrentGameHasScience())
                {
                    total += KCT_GameStates.LastKnownTechCount;
                    if (KCT_GameStates.LastKnownTechCount == 0)
                        total += ResearchAndDevelopment.Instance != null ? ResearchAndDevelopment.Instance.snapshot.GetData().GetNodes("Tech").Length : 0;
                }

                //In progress tech nodes
                total += KCT_GameStates.TechList.Count;
            }
            //Purchased funds
            total += KCT_GameStates.PurchasedUpgrades[0];
            //Purchased science
            total += KCT_GameStates.PurchasedUpgrades[1];
            //Inventory sales
            total += (int)KCT_GameStates.InventorySaleUpgrades;
            //Temp upgrades (currently for when tech nodes finish)
            total += KCT_GameStates.MiscellaneousTempUpgrades;
            
            //Misc. (when API)



            return total;
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
