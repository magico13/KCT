using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MagiCore;

namespace KerbalConstructionTime
{
    public class KCT_MathParsing
    {
        public static double GetStandardFormulaValue(string formulaName, Dictionary<string, string> variables)
        {
            switch (formulaName)
            {
                case "Node": return MathParsing.ParseMath("KCT_NODE", KCT_PresetManager.Instance.ActivePreset.formulaSettings.NodeFormula, variables);
                case "UpgradeFunds": return MathParsing.ParseMath("KCT_UPGRADE_FUNDS", KCT_PresetManager.Instance.ActivePreset.formulaSettings.UpgradeFundsFormula, variables);
                case "UpgradeScience": return MathParsing.ParseMath("KCT_UPGRADE_SCIENCE", KCT_PresetManager.Instance.ActivePreset.formulaSettings.UpgradeScienceFormula, variables);
                case "Research": return MathParsing.ParseMath("KCT_RESEARCH", KCT_PresetManager.Instance.ActivePreset.formulaSettings.ResearchFormula, variables);
                case "EffectivePart": return MathParsing.ParseMath("KCT_EFFECTIVE_PART", KCT_PresetManager.Instance.ActivePreset.formulaSettings.EffectivePartFormula, variables);
                case "ProceduralPart": return MathParsing.ParseMath("KCT_PROCEDURAL_PART", KCT_PresetManager.Instance.ActivePreset.formulaSettings.ProceduralPartFormula, variables);
                case "BP": return MathParsing.ParseMath("KCT_BP", KCT_PresetManager.Instance.ActivePreset.formulaSettings.BPFormula, variables);
                case "KSCUpgrade": return MathParsing.ParseMath("KCT_KSC_UPGRADE", KCT_PresetManager.Instance.ActivePreset.formulaSettings.KSCUpgradeFormula, variables);
                case "Reconditioning": return MathParsing.ParseMath("KCT_RECONDITIONING", KCT_PresetManager.Instance.ActivePreset.formulaSettings.ReconditioningFormula, variables);
                case "BuildRate": return MathParsing.ParseMath("KCT_BUILD_RATE", KCT_PresetManager.Instance.ActivePreset.formulaSettings.BuildRateFormula, variables);
                case "UpgradeReset": return MathParsing.ParseMath("KCT_UPGRADE_RESET", KCT_PresetManager.Instance.ActivePreset.formulaSettings.UpgradeResetFormula, variables);
                case "InventorySales": return MathParsing.ParseMath("KCT_INVENTORY_SALES", KCT_PresetManager.Instance.ActivePreset.formulaSettings.InventorySaleFormula, variables);
                case "RolloutCost": return MathParsing.ParseMath("KCT_ROLLOUT_COST", KCT_PresetManager.Instance.ActivePreset.formulaSettings.RolloutCostFormula, variables);
                case "NewLaunchPadCost": return MathParsing.ParseMath("KCT_NEW_LAUNCHPAD_COST", KCT_PresetManager.Instance.ActivePreset.formulaSettings.NewLaunchPadCostFormula, variables);
                default: return 0;
            }
        }

        public static double ParseBuildRateFormula(KCT_BuildListVessel.ListType type, int index, KCT_KSC KSC, bool UpgradedRates = false)
        {
            //N = num upgrades, I = rate index, L = VAB/SPH upgrade level, R = R&D level
            int level = 0, upgrades = 0;
            Dictionary<string, string> variables = new Dictionary<string, string>();
            if (type == KCT_BuildListVessel.ListType.VAB)
            {
                level = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                if (KSC.VABUpgrades.Count > index)
                    upgrades = KSC.VABUpgrades[index];
            }
            else if (type == KCT_BuildListVessel.ListType.SPH)
            {
                level = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.SpaceplaneHangar);
                if (KSC.SPHUpgrades.Count > index)
                    upgrades = KSC.SPHUpgrades[index];
            }
            if (UpgradedRates)
                upgrades++;
            variables.Add("L", level.ToString());
            variables.Add("N", upgrades.ToString());
            variables.Add("I", index.ToString());
            variables.Add("R", KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.ResearchAndDevelopment).ToString());
            //int numNodes = RDController.Instance != null ? RDController.Instance.nodes.FindAll(n => n.IsResearched).Count : 0;
            int numNodes = 0;
            if (ResearchAndDevelopment.Instance != null)
                numNodes = ResearchAndDevelopment.Instance.snapshot.GetData().GetNodes("Tech").Length;
            variables.Add("S", numNodes.ToString());

            AddCrewVariables(variables);

            return GetStandardFormulaValue("BuildRate", variables);
        }

        public static double ParseNodeRateFormula(double ScienceValue, int index = 0, bool UpgradedRates = false)
        {
            int RnDLvl = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.ResearchAndDevelopment);
            int upgrades = KCT_GameStates.TechUpgradesTotal;
            if (UpgradedRates) upgrades++;
            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("S", ScienceValue.ToString());
            variables.Add("N", upgrades.ToString());
            variables.Add("R", RnDLvl.ToString());
            variables.Add("O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString());
            variables.Add("I", index.ToString());

            AddCrewVariables(variables);

            return GetStandardFormulaValue("Node", variables);
        }

        public static double ParseRolloutCostFormula(KCT_BuildListVessel vessel)
        {
            if (!KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled || !KCT_PresetManager.Instance.ActivePreset.generalSettings.ReconditioningTimes)
                return 0;

            double loadedMass, emptyMass, loadedCost, emptyCost;
            loadedCost = vessel.GetTotalCost();
            emptyCost = vessel.emptyCost;
            loadedMass = vessel.GetTotalMass();
            emptyMass = vessel.emptyMass;

            int EditorLevel = 0, LaunchSiteLvl = 0;
            int isVABVessel = 0;
            if (vessel.type == KCT_BuildListVessel.ListType.None)
                vessel.FindTypeFromLists();
            if (vessel.type == KCT_BuildListVessel.ListType.VAB)
            {
                EditorLevel = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                LaunchSiteLvl = KCT_GameStates.ActiveKSC.ActiveLPInstance.level;//KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.LaunchPad);
                isVABVessel = 1;
            }
            else if (vessel.type == KCT_BuildListVessel.ListType.SPH)
            {
                EditorLevel = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.SpaceplaneHangar);
                LaunchSiteLvl = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.Runway);
            }
            double BP = vessel.buildPoints;

            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("M", loadedMass.ToString());
            variables.Add("m", emptyMass.ToString());
            variables.Add("C", loadedCost.ToString());
            variables.Add("c", emptyCost.ToString());
            variables.Add("VAB", isVABVessel.ToString());
            variables.Add("BP", BP.ToString());
            variables.Add("L", LaunchSiteLvl.ToString());
            variables.Add("EL", EditorLevel.ToString());
            variables.Add("SN", vessel.numStages.ToString());
            variables.Add("SP", vessel.numStageParts.ToString());
            variables.Add("SC", vessel.stagePartCost.ToString());

            AddCrewVariables(variables);

            return GetStandardFormulaValue("RolloutCost", variables);
        }

        public static double ParseReconditioningFormula(KCT_BuildListVessel vessel, bool isReconditioning)
        {
           // new Dictionary<string, string>() {{"M", vessel.GetTotalMass().ToString()}, {"O", KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier.ToString()},
            //    {"E", KCT_PresetManager.Instance.ActivePreset.timeSettings.ReconditioningEffect.ToString()}, {"X", KCT_PresetManager.Instance.ActivePreset.timeSettings.MaxReconditioning.ToString()}});
            Dictionary<string, string> variables = new Dictionary<string, string>();

            double loadedMass, emptyMass, loadedCost, emptyCost;
            loadedCost = vessel.GetTotalCost();
            emptyCost = vessel.emptyCost;
            loadedMass = vessel.GetTotalMass();
            emptyMass = vessel.emptyMass;

            int EditorLevel = 0, LaunchSiteLvl = 0;
            int isVABVessel = 0;
            if (vessel.type == KCT_BuildListVessel.ListType.VAB)
            {
                EditorLevel = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.VehicleAssemblyBuilding);
                LaunchSiteLvl = KCT_GameStates.ActiveKSC.ActiveLPInstance.level;//KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.LaunchPad);
                isVABVessel = 1;
            }
            else
            {
                EditorLevel = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.SpaceplaneHangar);
                LaunchSiteLvl = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.Runway);
            }
            double BP = vessel.buildPoints;
            double OverallMult = KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier;
            double ReconEffect = KCT_PresetManager.Instance.ActivePreset.timeSettings.ReconditioningEffect;
            double MaxRecon = KCT_PresetManager.Instance.ActivePreset.timeSettings.MaxReconditioning;

            variables.Add("M", loadedMass.ToString());
            variables.Add("m", emptyMass.ToString());
            variables.Add("C", loadedCost.ToString());
            variables.Add("c", emptyCost.ToString());
            variables.Add("VAB", isVABVessel.ToString());
            variables.Add("BP", BP.ToString());
            variables.Add("L", LaunchSiteLvl.ToString());
            variables.Add("EL", EditorLevel.ToString());
            variables.Add("O", OverallMult.ToString());
            variables.Add("E", ReconEffect.ToString());
            variables.Add("X", MaxRecon.ToString());
            int isRecon = isReconditioning ? 1 : 0;
            variables.Add("RE", isRecon.ToString());
            variables.Add("S", KCT_PresetManager.Instance.ActivePreset.timeSettings.RolloutReconSplit.ToString());

            AddCrewVariables(variables);

            return GetStandardFormulaValue("Reconditioning", variables);
        }

        public static void AddCrewVariables(Dictionary<string, string> crewVars)
        {
            //Dictionary<string, string> crewVars = new Dictionary<string, string>();
            int pilots=0, engineers=0, scientists=0;
            int pLevels=0, eLevels=0, sLevels=0;

            int pilots_total = 0, engineers_total = 0, scientists_total = 0;
            int pLevels_total = 0, eLevels_total = 0, sLevels_total = 0;

            foreach (ProtoCrewMember pcm in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (pcm.rosterStatus == ProtoCrewMember.RosterStatus.Available || pcm.rosterStatus == ProtoCrewMember.RosterStatus.Assigned)
                {
                    if (pcm.trait == "Pilot")
                    {
                        if (pcm.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                        {
                            pilots++;
                            pLevels += pcm.experienceLevel;
                        }
                        pilots_total++;
                        pLevels_total += pcm.experienceLevel;
                    }
                    else if (pcm.trait == "Engineer")
                    {
                        if (pcm.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                        {
                            engineers++;
                            eLevels += pcm.experienceLevel;
                        }
                        engineers_total++;
                        eLevels_total += pcm.experienceLevel;
                    }
                    else if (pcm.trait == "Scientist")
                    {
                        if (pcm.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                        {
                            scientists++;
                            sLevels += pcm.experienceLevel;
                        }
                        scientists_total++;
                        sLevels_total += pcm.experienceLevel;
                    }
                }
                
            }

            //KCTDebug.Log(pilots + " pilots " + pLevels + " levels");
            //KCTDebug.Log(engineers + " engineers " + eLevels + " levels");
            //KCTDebug.Log(scientists + " scientists " + sLevels + " levels");

            crewVars.Add("PiK", pilots.ToString());
            crewVars.Add("PiL", pLevels.ToString());

            crewVars.Add("EnK", engineers.ToString());
            crewVars.Add("EnL", eLevels.ToString());

            crewVars.Add("ScK", scientists.ToString());
            crewVars.Add("ScL", sLevels.ToString());

            crewVars.Add("TPiK", pilots_total.ToString());
            crewVars.Add("TPiL", pLevels_total.ToString());

            crewVars.Add("TEnK", engineers_total.ToString());
            crewVars.Add("TEnL", eLevels_total.ToString());

            crewVars.Add("TScK", scientists_total.ToString());
            crewVars.Add("TScL", sLevels_total.ToString());


            //KCTDebug.Log("Printing crewVars data");
            //foreach (var kvp in crewVars)
            //    KCTDebug.Log(kvp.Key + ":" + kvp.Value);
            //return crewVars;
        }
    }
}
