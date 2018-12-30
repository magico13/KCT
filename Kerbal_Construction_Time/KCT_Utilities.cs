using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using KSP.UI.Screens;
using System.Collections;
using KSP.UI;

namespace KerbalConstructionTime
{
    static class KCT_Utilities
    {

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
            return newInv;
        }

        public static List<String> PartDictToList(Dictionary<String, int> dict)
        {
            List<String> ret = new List<string>();
            for (int i = 0; i < dict.Count; i++)
            {
                ret.Add(dict.Keys.ElementAt(i));
                ret.Add(dict.Values.ElementAt(i).ToString());
            }
            return ret;
        }

        public static AvailablePart GetAvailablePartByName(string partName)
        {
            return PartLoader.getPartInfoByName(partName);
        }

        public static double GetBuildTime(List<Part> parts)
        {
            //get list of parts that are in the inventory
            IList<Part> inventorySample = ScrapYardWrapper.GetPartsInInventory(parts, ScrapYardWrapper.ComparisonStrength.STRICT) ?? new List<Part>();

            double totalEffectiveCost = 0;

            List<string> globalVariables = new List<string>();

            foreach (Part p in parts)
            {
                string name = p.partInfo.name;
                double effectiveCost = 0;
                double cost = GetPartCosts(p);
                double dryCost = GetPartCosts(p, false);
                
                double drymass = p.mass;
                double wetmass = p.GetResourceMass() + drymass;

                double PartMultiplier = KCT_PresetManager.Instance.ActivePreset.partVariables.GetPartVariable(name);
                double ModuleMultiplier = KCT_PresetManager.Instance.ActivePreset.partVariables.GetModuleVariable(p.Modules);
                KCT_PresetManager.Instance.ActivePreset.partVariables.SetGlobalVariables(globalVariables, p.Modules);

                double InvEff = (inventorySample.Contains(p) ? KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect : 0);
                int builds = ScrapYardWrapper.GetBuildCount(p);
                int used = ScrapYardWrapper.GetUseCount(p);
                //C=cost, c=dry cost, M=wet mass, m=dry mass, U=part tracker, O=overall multiplier, I=inventory effect (0 if not in inv), B=build effect

                effectiveCost = KCT_MathParsing.GetStandardFormulaValue("EffectivePart", new Dictionary<string, string>() { {"C", cost.ToString()}, {"c", dryCost.ToString()}, {"M",wetmass.ToString()},
                    { "m", drymass.ToString()}, {"U", builds.ToString()}, {"u", used.ToString() }, {"O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString()}, {"I", InvEff.ToString()},
                    { "B", KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect.ToString()}, 
                    {"PV", PartMultiplier.ToString()}, {"MV", ModuleMultiplier.ToString()}});

                if (InvEff != 0)
                {
                    inventorySample.Remove(p);
                }

                if (effectiveCost < 0) effectiveCost = 0;
                totalEffectiveCost += effectiveCost;
            }
            double finalBP = KCT_PresetManager.Instance.ActivePreset.partVariables.GetGlobalVariable(globalVariables) * KCT_MathParsing.GetStandardFormulaValue("BP", new Dictionary<string, string>() { { "E", totalEffectiveCost.ToString() }, { "O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString() } });
            return finalBP;
        }

        public static double GetBuildTime(List<ConfigNode> parts)
        {
            //get list of parts that are in the inventory
            IList<ConfigNode> inventorySample = ScrapYardWrapper.GetPartsInInventory(parts, ScrapYardWrapper.ComparisonStrength.STRICT) ?? new List<ConfigNode>();

            double totalEffectiveCost = 0;
            List<string> globalVariables = new List<string>();
            foreach (ConfigNode p in parts)
            {
                string name = PartNameFromNode(p);
                string raw_name = name;
                double effectiveCost = 0;
                double cost;
                float dryCost, fuelCost;
                float dryMass, fuelMass;
                float wetMass;

                ShipConstruction.GetPartCostsAndMass(p, GetAvailablePartByName(name), out dryCost, out fuelCost, out dryMass, out fuelMass);
                cost = dryCost + fuelCost;
                wetMass = dryMass + fuelMass;
                    

                double PartMultiplier = KCT_PresetManager.Instance.ActivePreset.partVariables.GetPartVariable(raw_name);
                List<string> moduleNames = new List<string>();
                foreach (ConfigNode modNode in GetModulesFromPartNode(p))
                    moduleNames.Add(modNode.GetValue("name"));
                double ModuleMultiplier = KCT_PresetManager.Instance.ActivePreset.partVariables.GetModuleVariable(moduleNames);
                KCT_PresetManager.Instance.ActivePreset.partVariables.SetGlobalVariables(globalVariables, moduleNames);

                double InvEff = (inventorySample.Contains(p) ? KCT_PresetManager.Instance.ActivePreset.timeSettings.InventoryEffect : 0);
                int builds = ScrapYardWrapper.GetBuildCount(p);
                int used = ScrapYardWrapper.GetUseCount(p);
                //C=cost, c=dry cost, M=wet mass, m=dry mass, U=part tracker, O=overall multiplier, I=inventory effect (0 if not in inv), B=build effect

                effectiveCost = KCT_MathParsing.GetStandardFormulaValue("EffectivePart", new Dictionary<string, string>() { {"C", cost.ToString()}, {"c", dryCost.ToString()}, {"M",wetMass.ToString()},
                {"m", dryMass.ToString()}, {"U", builds.ToString()}, {"u", used.ToString()}, {"O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString()}, {"I", InvEff.ToString()}, {"B", KCT_PresetManager.Instance.ActivePreset.timeSettings.BuildEffect.ToString()}, 
                {"PV", PartMultiplier.ToString()}, {"MV", ModuleMultiplier.ToString()}});

                if (InvEff != 0)
                {
                    inventorySample.Remove(p);
                }

                if (effectiveCost < 0) effectiveCost = 0;
                totalEffectiveCost += effectiveCost;
            }
            double finalBP = KCT_PresetManager.Instance.ActivePreset.partVariables.GetGlobalVariable(globalVariables) * KCT_MathParsing.GetStandardFormulaValue("BP", new Dictionary<string, string>() { { "E", totalEffectiveCost.ToString() }, { "O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString() } });
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

        public static ConfigNode[] GetModulesFromPartNode(ConfigNode partNode)
        {
            return partNode.GetNodes("MODULE");
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

        public static void ProgressBuildTime()
        {
            double UT = 0;
            if (HighLogic.LoadedSceneIsEditor) //support for EditorTime
                UT = HighLogic.CurrentGame.flightState.universalTime;
            else
                UT = Planetarium.GetUniversalTime();
            if (KCT_GameStates.lastUT == 0)
                KCT_GameStates.lastUT = UT;
            double UTDiff = UT - KCT_GameStates.lastUT;
            if (UTDiff > 0 && (HighLogic.LoadedSceneIsEditor || UTDiff < (TimeWarp.fetch.warpRates[TimeWarp.fetch.warpRates.Length - 1] * 2)))
            {
                foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                {
                    double buildRate = 0;
                    if (ksc.VABList.Count > 0)
                    {
                        for (int i = 0; i < ksc.VABList.Count; i++)
                        {
                            buildRate = GetBuildRate(i, KCT_BuildListVessel.ListType.VAB, ksc);
                            ksc.VABList[i].AddProgress(buildRate * (UTDiff));
                            if (((IKCTBuildItem)ksc.VABList[i]).IsComplete())
                                MoveVesselToWarehouse(0, i, ksc);
                        }
                    }
                    if (ksc.SPHList.Count > 0)
                    {
                        for (int i = 0; i < ksc.SPHList.Count; i++)
                        {
                            buildRate = GetBuildRate(i, KCT_BuildListVessel.ListType.SPH, ksc);
                            ksc.SPHList[i].AddProgress(buildRate * (UTDiff));
                            if (((IKCTBuildItem)ksc.SPHList[i]).IsComplete())
                                MoveVesselToWarehouse(1, i, ksc);
                        }
                    }

                    foreach (KCT_Recon_Rollout rr in ksc.Recon_Rollout)
                    {
                        double prog = rr.progress;
                        rr.progress += rr.AsBuildItem().GetBuildRate() * (UTDiff);
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
                    //Reset the associated launchpad id when rollback completes
                    ksc.Recon_Rollout.ForEach(delegate(KCT_Recon_Rollout rr)
                    {
                        if (rr.RRType == KCT_Recon_Rollout.RolloutReconType.Rollback && rr.AsBuildItem().IsComplete())
                        {
                            KCT_BuildListVessel blv = KCT_Utilities.FindBLVesselByID(new Guid(rr.associatedID));
                            if (blv != null)
                                blv.launchSiteID = -1;
                        }
                    });
                    ksc.Recon_Rollout.RemoveAll(rr => !KCT_PresetManager.Instance.ActivePreset.generalSettings.ReconditioningTimes || (rr.RRType != KCT_Recon_Rollout.RolloutReconType.Rollout && rr.AsBuildItem().IsComplete()));

                    foreach (KCT_UpgradingBuilding kscTech in ksc.KSCTech)
                    {
                        if (!kscTech.AsIKCTBuildItem().IsComplete()) kscTech.AddProgress(kscTech.AsIKCTBuildItem().GetBuildRate() * (UTDiff));
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
                    tech.progress += (buildRate * (UTDiff));
                    if (tech.isComplete || !KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUnlockTimes)
                    {
                        if (KCT_GameStates.settings.ForceStopWarp && TimeWarp.CurrentRate > 1f)
                            TimeWarp.SetRate(0, true);
                        if (tech.protoNode == null) continue;
                        tech.EnableTech();
                        KCT_GameStates.TechList.Remove(tech);
                        if (KCT_PresetManager.PresetLoaded() && KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUpgrades)
                            KCT_GameStates.MiscellaneousTempUpgrades++;

                        for (int j = 0; j < KCT_GameStates.TechList.Count; j++)
                            KCT_GameStates.TechList[j].UpdateBuildRate(j);
                    }
                }
            }
            if (KCT_GameStates.targetedItem != null && KCT_GameStates.targetedItem.IsComplete())
            {
                TimeWarp.SetRate(0, true);
                KCT_GameStates.targetedItem = null;
                KCT_GameStates.warpInitiated = false;
            }
            KCT_GameStates.lastUT = UT;

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
            AvailablePart aPart = GetAvailablePartByName(name);
            if (aPart == null)
                return 0;
            float dryCost, fuelCost;
            float dryMass, fuelMass;
            ShipConstruction.GetPartCostsAndMass(part, aPart, out dryCost, out fuelCost, out dryMass, out fuelMass);
            //float total = ShipConstruction.GetPartCosts(part, aPart, out dry, out wet);
            
            if (includeFuel)
                return dryCost+fuelCost;
            else
                return dryCost;
        }

        public static float GetPartMassFromNode(ConfigNode part, bool includeFuel = true)
        {
            AvailablePart aPart = GetAvailablePartByName(PartNameFromNode(part));
            if (aPart == null)
                return 0;
            //total = ShipConstruction.GetPartTotalMass(part, aPart, out dry, out wet);
            float dryCost, fuelCost;
            float dryMass, fuelMass;
            ShipConstruction.GetPartCostsAndMass(part, aPart, out dryCost, out fuelCost, out dryMass, out fuelMass);
            if (includeFuel)
                return dryMass+fuelMass;
            else
                return dryMass;
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
        /* 1.4 Addition
        public static bool CurrentGameIsMission()
        {
            return HighLogic.CurrentGame.Mode == Game.Modes.MISSION || HighLogic.CurrentGame.Mode == Game.Modes.MISSION_BUILDER;
        }
        */

        public static void AddScienceWithMessage(float science, TransactionReasons reason)
        {
            if (science > 0)
            {
                //ResearchAndDevelopment.Instance.Science += science;
                ResearchAndDevelopment.Instance.AddScience(science, reason);
                var message = new ScreenMessage("[KCT] " + science + " science added.", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                ScreenMessages.PostScreenMessage(message);
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

            if ((KCT_GameStates.settings.ForceStopWarp || vessel == KCT_GameStates.targetedItem) && TimeWarp.CurrentRateIndex != 0)
            {
                TimeWarp.SetRate(0, true);
                KCT_GameStates.warpInitiated = false;
            }

            //Assign science based on science rate
            if (CurrentGameHasScience() && !vessel.cannotEarnScience)
            {
                double rate = KCT_MathParsing.GetStandardFormulaValue("Research", new Dictionary<string, string>() { { "N", KSC.RDUpgrades[0].ToString() }, { "R", KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.ResearchAndDevelopment).ToString() } });
                if (rate > 0)
                {
                    AddScienceWithMessage((float)(rate * vessel.buildPoints), TransactionReasons.None);
                }
            }

            //Add parts to the tracker
            if (!vessel.cannotEarnScience) //if the vessel was previously completed, then we shouldn't register it as a new build
            {
                ScrapYardWrapper.RecordBuild(vessel.ExtractedPartNodes);
            }

            string stor = ListIdentifier == 0 ? "VAB" : "SPH";
            KCTDebug.Log("Moved vessel " + vessel.shipName + " to " +KSC.KSCName + "'s " + stor + " storage.");


            //TODO: Can't allow recalculations since the inventory doesn't work that way as of right now
            //foreach (KCT_KSC KSC_iterator in KCT_GameStates.KSCs)
            //{
            //    foreach (KCT_BuildListVessel blv in KSC_iterator.VABList)
            //    {
            //        double newTime = KCT_Utilities.GetBuildTime(blv.ExtractedPartNodes, true, blv.InventoryParts); //Use only the parts that were originally used when recalculating
            //        if (newTime < blv.buildPoints)
            //        {
            //            blv.buildPoints = blv.buildPoints - ((blv.buildPoints - newTime) * (100 - blv.ProgressPercent()) / 100.0); //If progress=0% then set to new build time, 100%=no change, 50%=half of difference.
            //        }
            //    }
            //    foreach (KCT_BuildListVessel blv in KSC_iterator.SPHList)
            //    {
            //        double newTime = KCT_Utilities.GetBuildTime(blv.ExtractedPartNodes, true, blv.InventoryParts);
            //        if (newTime < blv.buildPoints)
            //        {
            //            blv.buildPoints = blv.buildPoints - ((blv.buildPoints - newTime) * (100 - blv.ProgressPercent()) / 100.0); //If progress=0% then set to new build time, 100%=no change, 50%=half of difference.
            //        }
            //    }
            //}
            KCT_GUI.ResetBLWindow(false);
            if (!KCT_GameStates.settings.DisableAllMessages)
            {
                DisplayMessage("Vessel Complete!", Message, MessageSystemButton.MessageButtonColor.GREEN, MessageSystemButton.ButtonIcons.COMPLETE);
            }
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
            return AddVesselToBuildList(EditorLogic.fetch.launchSiteName);
        }

        public static KCT_BuildListVessel AddVesselToBuildList(string launchSite)
        {
            if (string.IsNullOrEmpty(launchSite))
            {
                launchSite = EditorLogic.fetch.launchSiteName;
            }
            KCT_BuildListVessel blv = new KCT_BuildListVessel(EditorLogic.fetch.ship, launchSite, GetBuildTime(EditorLogic.fetch.ship.Parts), EditorLogic.FlagURL);
            blv.shipName = EditorLogic.fetch.shipNameField.text;
            return AddVesselToBuildList(blv);
        }

        public static KCT_BuildListVessel AddVesselToBuildList(KCT_BuildListVessel blv)
        {
            if (CurrentGameIsCareer())
            {
                //Check upgrades
                //First, mass limit
                List<string> facilityChecks = blv.MeetsFacilityRequirements(true);
                if (facilityChecks.Count != 0)
                {
                    PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "editorChecksFailedPopup", "Failed editor checks!",
                        "Warning! This vessel did not pass the editor checks! It will still be built, but you will not be able to launch it without upgrading. Listed below are the failed checks:\n" 
                        + string.Join("\n", facilityChecks.ToArray()), "Acknowledged", false, HighLogic.UISkin);
                }


                double totalCost = blv.GetTotalCost();
                double prevFunds = Funding.Instance.Funds;
                if (totalCost > prevFunds)
                {
                    KCTDebug.Log("Tried to add " + blv.shipName + " to build list but not enough funds.");
                    KCTDebug.Log("Vessel cost: " + GetTotalVesselCost(blv.shipNode) + ", Current funds: " + prevFunds);
                    var msg = new ScreenMessage("Not Enough Funds To Build!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    ScreenMessages.PostScreenMessage(msg);
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

            ScrapYardWrapper.ProcessVessel(blv.ExtractedPartNodes);

            KCTDebug.Log("Added " + blv.shipName + " to " + type + " build list at KSC "+KCT_GameStates.ActiveKSC.KSCName+". Cost: "+blv.cost);
            KCTDebug.Log("Launch site is " + blv.launchSite);
            //KCTDebug.Log("Cost Breakdown (total, parts, fuel): " + blv.totalCost + ", " + blv.dryCost + ", " + blv.fuelCost);
            var message = new ScreenMessage("[KCT] Added " + blv.shipName + " to " + type + " build list.", 4.0f, ScreenMessageStyle.UPPER_CENTER);
            ScreenMessages.PostScreenMessage(message);
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

        public static void RampUpWarp()
        {
            //KCT_BuildListVessel ship = KCT_Utilities.NextShipToFinish();
            IKCTBuildItem ship = KCT_Utilities.NextThingToFinish();
            RampUpWarp(ship);
        }

        public static void RampUpWarp(IKCTBuildItem item)
        {
            int newRate = TimeWarp.CurrentRateIndex;
            double timeLeft = item.GetTimeLeft();
            if (double.IsPositiveInfinity(timeLeft))
                timeLeft = KCT_Utilities.NextThingToFinish().GetTimeLeft();
            while ((newRate + 1 < TimeWarp.fetch.warpRates.Length) && (timeLeft > TimeWarp.fetch.warpRates[newRate + 1]*Planetarium.fetch.fixedDeltaTime) && (newRate < KCT_GameStates.settings.MaxTimeWarp))
            {
                newRate++;
            }
            TimeWarp.SetRate(newRate, true);
          //  Debug.Log("Fixed Delta Time: " + Planetarium.fetch.fixedDeltaTime);
        }

        public static void DisableModFunctionality()
        {
            InputLockManager.RemoveControlLock("KCTLaunchLock");
            KCT_GUI.hideAll();
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
            if (KCT_PresetManager.Instance.ActivePreset.generalSettings.SharedUpgradePool)
            {
                foreach (KCT_KSC KSC in KCT_GameStates.KSCs)
                {
                    foreach (int i in KSC.VABUpgrades) spentPoints += i;
                    foreach (int i in KSC.SPHUpgrades) spentPoints += i;
                    spentPoints += KSC.RDUpgrades[0];
                }
                spentPoints += ksc.RDUpgrades[1]; //only count this once, all KSCs share this value
            }
            else
            {
                foreach (int i in ksc.VABUpgrades) spentPoints += i;
                foreach (int i in ksc.SPHUpgrades) spentPoints += i;
                foreach (int i in ksc.RDUpgrades) spentPoints += i;
            }
            return spentPoints;
        }

        /* 1.4 Addition
        public static List<string> GetLaunchSites(bool isVAB)
        {
            EditorDriver.editorFacility = isVAB ? EditorFacility.VAB : EditorFacility.SPH;
            typeof(EditorDriver).GetMethod("setupValidLaunchSites", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.Invoke(null, null);
            return EditorDriver.ValidLaunchSites;
        }
        */

        private static bool? _KSCSwitcherInstalled = null;
        public static bool KSCSwitcherInstalled
        {
            get
            {
                if (_KSCSwitcherInstalled == null)
                {
                    Type Switcher = null;
                    AssemblyLoader.loadedAssemblies.TypeOperation(t =>
                    {
                        if (t.FullName == "regexKSP.KSCSwitcher")
                        {
                            Switcher = t;
                        }
                    });

                    _KSCSwitcherInstalled = (Switcher != null);

                    //KCTDebug.Log("KSCSwitcher status: " + _KSCSwitcherInstalled);
                }
                return (_KSCSwitcherInstalled == null ? false : (bool)_KSCSwitcherInstalled);
            }
        }

        public static string GetActiveRSSKSC()
        {
            if (!KSCSwitcherInstalled) return "Stock";
            /*Type Switcher = null;
            AssemblyLoader.loadedAssemblies.TypeOperation(t =>
            {
                if (t.FullName == "regexKSP.KSCSwitcher")
                {
                    Switcher = t;
                }
            });

            UnityEngine.Object KSCSwitcherInstance = GameObject.FindObjectOfType(Switcher);

            return (string)GetMemberInfoValue(Switcher.GetMember("activeSite")[0], KSCSwitcherInstance);*/

            //get the LastKSC.KSCLoader.instance object
            //check the Sites object (KSCSiteManager) for the lastSite, if "" then get defaultSite
            Type Loader = null;
            AssemblyLoader.loadedAssemblies.TypeOperation(t =>
            {
                if (t.FullName == "regexKSP.KSCLoader")
                {
                    Loader = t;
                }
            });
            object LoaderInstance = GetMemberInfoValue(Loader.GetMember("instance")[0], null);
            if (LoaderInstance == null)
                return "Stock";
            object SitesObj = GetMemberInfoValue(Loader.GetMember("Sites")[0], LoaderInstance);
            string lastSite = (string)GetMemberInfoValue(SitesObj.GetType().GetMember("lastSite")[0], SitesObj);

            if (lastSite == "")
            {
                string defaultSite = (string)GetMemberInfoValue(SitesObj.GetType().GetMember("defaultSite")[0], SitesObj);
                return defaultSite;
            }
            return lastSite;
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
                    if (CurrentGameIsCareer())
                        setActive.ActiveLPInstance.level = 0;
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
                //intact = new PreFlightTests.FacilityOperational("LaunchPad", "building").Test();
                intact = new PreFlightTests.FacilityOperational("LaunchPad", "LaunchPad").Test();
            }
            else if (type == KCT_BuildListVessel.ListType.SPH)
            {
               /* if (!new PreFlightTests.FacilityOperational("Runway", "End09").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "End27").Test())
                    intact = false;*/
               /* if (!new PreFlightTests.FacilityOperational("Runway", "Section1").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "Section2").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "Section3").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "Section4").Test())
                    intact = false;
                if (!new PreFlightTests.FacilityOperational("Runway", "Section5").Test())
                    intact = false;*/
                if (!new PreFlightTests.FacilityOperational("Runway", "Runway").Test())
                    intact = false;
            }
            return intact;
        }

        public static void RecalculateEditorBuildTime(ShipConstruct ship)
        {
            if (!HighLogic.LoadedSceneIsEditor)
            {
                return;
            }

            KCT_GameStates.EditorBuildTime = GetBuildTime(ship.Parts);
            KCT_GameStates.EditorRolloutCosts = KCT_MathParsing.ParseRolloutCostFormula(new KCT_BuildListVessel(ship, EditorLogic.fetch.launchSiteName, KCT_GameStates.EditorBuildTime, EditorLogic.FlagURL));
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
            if (part != null && part.Modules != null)
            {
                for (int i = 0; i < part.Modules.Count; i++ )
                {
                    if (part.Modules[i] != null && part.Modules[i].moduleName != null && part.Modules[i].moduleName.ToLower().Contains("procedural"))
                        return true;
                }
            }
            return false;
        }

        public static int BuildingUpgradeLevel(SpaceCenterFacility facility)
        {
            int lvl = BuildingUpgradeMaxLevel(facility);
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                lvl = (int)Math.Round((lvl * ScenarioUpgradeableFacilities.GetFacilityLevel(facility)));
            }
            return lvl;
        }

        public static int BuildingUpgradeLevel(string facilityID)
        {
            int lvl = BuildingUpgradeMaxLevel(facilityID);
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                lvl = (int)Math.Round((lvl * ScenarioUpgradeableFacilities.GetFacilityLevel(facilityID))); //let's not store discrete things with integers! No! Let's use floats! -Squad
            }
            return lvl;
        }

        public static int BuildingUpgradeMaxLevel(string facilityID)
        {
            int lvl = ScenarioUpgradeableFacilities.GetFacilityLevelCount(facilityID);
            if (lvl < 0)
            {
                if (!KCT_GameStates.BuildingMaxLevelCache.TryGetValue(facilityID.Split('/').Last(), out lvl))
                {
                    //screw it, let's call it 2
                    lvl = 2;
                    KCTDebug.Log($"Couldn't get actual max level or cached one for {facilityID}. Assuming 2.");
                }
            }
            return lvl;
        }

        public static int BuildingUpgradeMaxLevel(SpaceCenterFacility facility)
        {
            int lvl = ScenarioUpgradeableFacilities.GetFacilityLevelCount(facility);
            if (lvl < 0)
            {
                if (!KCT_GameStates.BuildingMaxLevelCache.TryGetValue(facility.ToString(), out lvl))
                {
                    //screw it, let's call it 2
                    lvl = 2;
                    KCTDebug.Log($"Couldn't get actual max level or cached one for {facility}. Assuming 2.");
                }
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
            total += KCT_GameStates.TemporaryModAddedUpgradesButReallyWaitForTheAPI;
            total += KCT_GameStates.PermanentModAddedUpgradesButReallyWaitForTheAPI;


            return total;
        }

        public static bool RecoverActiveVesselToStorage(KCT_BuildListVessel.ListType listType)
        {
            ShipConstruct test = new ShipConstruct();
            try
            {
                KCTDebug.Log("Attempting to recover active vessel to storage.");
                GamePersistence.SaveGame("KCT_Backup", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                KCT_GameStates.recoveredVessel = new KCT_BuildListVessel(FlightGlobals.ActiveVessel);
                KCT_GameStates.recoveredVessel.type = listType;
                if (listType == KCT_BuildListVessel.ListType.VAB)
                    KCT_GameStates.recoveredVessel.launchSite = "LaunchPad";
                else
                    KCT_GameStates.recoveredVessel.launchSite = "Runway";

                //check for symmetry parts and remove those references if they can't be found
                RemoveMissingSymmetry(KCT_GameStates.recoveredVessel.shipNode);

                //test if we can actually convert it
                bool success = test.LoadShip(KCT_GameStates.recoveredVessel.shipNode);
                if (success)
                    ShipConstruction.CreateBackup(test);
                KCTDebug.Log("Load test reported success = " + success);
                if (!success)
                {
                    KCT_GameStates.recoveredVessel = null;
                    return false;
                }

                KerbalConstructionTime.instance.StartCoroutine(RecoverVessel(FlightGlobals.ActiveVessel));
                return true;
            }
            catch
            {
                Debug.LogError("[KCT] Error while recovering craft into inventory.");
                KCT_GameStates.recoveredVessel = null;
                ShipConstruction.ClearBackups();
                return false;
            }
        }

        /// <summary>
        /// Recover the vessel, after the end of the frame. Start it in a coroutine
        /// </summary>
        /// <param name="toRecover"></param>
        /// <returns></returns>
        public static IEnumerator RecoverVessel(Vessel toRecover)
        {
            yield return new WaitForEndOfFrame();
            GameEvents.OnVesselRecoveryRequested.Fire(toRecover);
        }

        public static void RemoveMissingSymmetry(ConfigNode ship)
        {
            //loop through, find all sym = lines and find the part they reference
            int referencesRemoved = 0;
            foreach (ConfigNode partNode in ship.GetNodes("PART"))
            {
                List<string> toRemove = new List<string>();
                foreach (string symPart in partNode.GetValues("sym"))
                {
                    //find the part in the ship
                    if (ship.GetNodes("PART").FirstOrDefault(cn => cn.GetValue("part") == symPart) == null)
                        toRemove.Add(symPart);
                }

                foreach (string remove in toRemove)
                {
                    foreach (ConfigNode.Value val in partNode.values)
                    {
                        if (val.value == remove)
                        {
                            referencesRemoved++;
                            partNode.values.Remove(val);
                            break;
                        }
                    }
                }
            }
            KCTDebug.Log("Removed " + referencesRemoved + " invalid symmetry references.");
        }

        /// <summary>
        /// Overrides or disables the editor's launch button (and individual site buttons) depending on settings
        /// </summary>
        public static void HandleEditorButton()
        {
            if (KCT_GUI.PrimarilyDisabled)
            {
                return;
            }

            //also set the editor ui to 1 height
            KCT_GUI.editorWindowPosition.height = 1;


            if (KCT_GameStates.settings.OverrideLaunchButton)
            {
                KCTDebug.Log("Attempting to take control of launch button");

                EditorLogic.fetch.launchBtn.onClick = new UnityEngine.UI.Button.ButtonClickedEvent(); //delete all other listeners (sorry :( )

                EditorLogic.fetch.launchBtn.onClick.AddListener(() => { KerbalConstructionTime.ShowLaunchAlert(null); });

                /* 1.4 Addition
                //delete listeners to the launchsite specific buttons
                UILaunchsiteController controller = UnityEngine.Object.FindObjectOfType<UILaunchsiteController>();

                //IEnumerable list = controller.GetType().GetField("launchPadItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy)?.GetValue(controller) as IEnumerable;
                IEnumerable list = controller.GetType().GetPrivateMemberValue("launchPadItems", controller, 4) as IEnumerable;
                if (list != null)
                {
                    foreach (object site in list)
                    {
                        //find and disable the button
                        //why isn't EditorLaunchPadItem public despite all of its members being public?
                        UnityEngine.UI.Button button = site.GetType().GetPublicValue<UnityEngine.UI.Button>("buttonLaunch", site);
                        if (button != null)
                        {
                            button.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                            string siteName = site.GetType().GetPublicValue<string>("siteName", site);
                            button.onClick.AddListener(() => { KerbalConstructionTime.ShowLaunchAlert(siteName); });
                        }
                    }
                }
                */
            }
            else
            {
                InputLockManager.SetControlLock(ControlTypes.EDITOR_LAUNCH, "KCTLaunchLock");
                /* 1.4 Addition
                UILaunchsiteController controller = UnityEngine.Object.FindObjectOfType<UILaunchsiteController>();
                if (controller != null)
                {
                    controller.locked = true;
                }
                */
            }
        }
    }
}
/*
Copyright (C) 2018  Michael Marvin, Zachary Eck

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
