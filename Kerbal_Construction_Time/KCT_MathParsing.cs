using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    public class KCT_MathParsing
    {
        public static double GetStandardFormulaValue(string formulaName, Dictionary<string, string> variables)
        {
            switch (formulaName)
            {
                case "Node": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.NodeFormula, variables);
                case "UpgradeFunds": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.UpgradeFundsFormula, variables);
                case "UpgradeScience": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.UpgradeScienceFormula, variables);
                case "Research": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.ResearchFormula, variables);
                case "EffectivePart": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.EffectivePartFormula, variables);
                case "ProceduralPart": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.ProceduralPartFormula, variables);
                case "BP": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.BPFormula, variables);
                case "KSCUpgrade": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.KSCUpgradeFormula, variables);
                case "Reconditioning": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.ReconditioningFormula, variables);
                case "BuildRate": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.BuildRateFormula, variables);
                case "SimCost": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.SimCostFormula, variables);
                case "KerbinSimCost": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.KerbinSimCostFormula, variables);
                case "UpgradeReset": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.UpgradeResetFormula, variables);
                case "InventorySales": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.InventorySaleFormula, variables);
                case "RolloutCost": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.RolloutCostFormula, variables);
                case "NewLaunchPadCost": return ParseMath(KCT_PresetManager.Instance.ActivePreset.formulaSettings.NewLaunchPadCostFormula, variables);
                default: return 0;
            }
        }

        public static double ParseMath(string input, Dictionary<string, string> variables)
        {
           // KCTDebug.Log("Input_raw: " + input);
            string raw = input;
            foreach (KeyValuePair<string, string> kvp in variables)
            {
                if (input.Contains("[" + kvp.Key + "]"))
                {
                    input = input.Replace("[" + kvp.Key + "]", kvp.Value);
                }
            }
            //KCTDebug.Log("Input_replaced: " + input);

            double currentVal = 0;
            string stack = "";
            string lastOp = "+";
            string[] ops = { "+", "-", "*", "/", "%", "^", "(", "e", "E" };
            string[] functions = { "min", "max", "l", "L", "abs", "sign" };
            for (int i = 0; i < input.Length; ++i)
            {
                string ch = input[i].ToString();
                bool isOp = false, isFunction = false;
                //  KCTDebug.Log(ch);
                foreach (string op in ops)
                {
                    if (op == ch)
                    {
                        isOp = true;
                        break;
                    }
                }
                if (!isOp)
                {
                    foreach (string fun in functions)
                    {
                        if (fun[0] == input[i])
                        {
                            isFunction = true;
                            break;
                        }
                    }
                }

                if (isOp)
                {
                    if (ch == "-" && (stack.Trim() == ""))
                    {
                        stack += ch;
                    }
                    else if (ch == "e" || ch == "E")
                    {
                        int index;
                        for (index = i+2; index < input.Length; ++index)
                        {
                            string ch2 = input[index].ToString();
                            if (ops.Contains(ch2))
                                break;
                        }
                        string sub = input.Substring(i + 1, index - i - 1);
                        double exp = ParseMath(sub, variables);
                        double newVal = double.Parse(stack) * Math.Pow(10, exp);
                        currentVal = DoMath(currentVal, lastOp, newVal.ToString());
                        stack = "0";
                        lastOp = "+";
                        i = index - 1;
                    }
                    else if (ch == "(")
                    {
                        int j = FindEndParenthesis(input, i)[0];
                        string sub = input.Substring(i + 1, j - i - 1);
                        string val = ParseMath(sub, variables).ToString();
                        input = input.Substring(0, i) + val + input.Substring(j + 1);
                        --i;
                    }
                    else
                    {
                        currentVal = DoMath(currentVal, lastOp, stack);
                        lastOp = ch;
                        stack = "";
                    }
                }
                else if (isFunction)
                {
                    int subStart = input.IndexOf('(', i)+1;
                    string function = input.Substring(i, subStart - i - 1);
                    //KCTDebug.Log(function);
                    int[] parenComma = FindEndParenthesis(input, subStart-1);
                    int j = parenComma[0];
                    int comma = parenComma[1];
                    string sub = input.Substring(subStart, j - subStart);
                   // KCTDebug.Log("fn: "+function+" sub: "+sub);
                    double val = 0.0;

                    if (function == "l")
                    {
                        val = ParseMath(sub, variables);
                        val = Math.Log(val);
                    }
                    else if (function == "L")
                    {
                        val = ParseMath(sub, variables);
                        val = Math.Log10(val);
                    }
                    else if (function == "max" || function == "min")
                    {
                        string[] parts = new string[2];
                        parts[0] = input.Substring(subStart, parenComma[1] - subStart);
                        parts[1] = input.Substring(parenComma[1] + 1, j - parenComma[1] - 1);
                        double sub1 = ParseMath(parts[0], variables);
                        double sub2 = ParseMath(parts[1], variables);
                        if (function == "max")
                            val = Math.Max(sub1, sub2);
                        else if (function == "min")
                            val = Math.Min(sub1, sub2);
                    }
                    else if (function == "sign")
                    {
                        val = ParseMath(sub, variables);
                        if (val >= 0)
                            val = 1;
                        else
                            val = -1;
                    }
                    else if (function == "abs")
                    {
                        val = ParseMath(sub, variables);
                        val = Math.Abs(val);
                    }

                    input = input.Substring(0, i) + val.ToString() + input.Substring(j + 1);
                    i--;
                }
                else
                {
                    stack += ch;
                }
            }
            currentVal = DoMath(currentVal, lastOp, stack);
            //KCTDebug.Log("(" + raw + ")=(" + input + ")=" + currentVal);
            return currentVal;
        }

        private static int[] FindEndParenthesis(string str, int curPos)
        {
            int depth = 0;
            int j = 0, commaPos = -1;
            for (j = curPos + 1; j < str.Length; ++j)
            {
                if (str[j] == '(') depth++;
                if (str[j] == ')') depth--;

                if (str[j] == ',' && depth == 0) 
                    commaPos = j;

                if (depth < 0)
                {
                    break;
                }
            }
            return new int[] {j, commaPos};
        }

        private static double DoMath(double currentVal, string operation, string newVal)
        {
            double newValue = 0;
            if (String.IsNullOrEmpty(newVal))
                return currentVal;
            if (!double.TryParse(newVal, out newValue))
            {
                Debug.LogError("[KCT] Tried to parse a non-double value: " + newVal);
                return currentVal;
            }
            switch (operation)
            {
                case "+": currentVal += newValue; break;
                case "-": currentVal -= newValue; break;
                case "*": currentVal *= newValue; break;
                case "/": currentVal /= newValue; break;
                case "%": currentVal = currentVal % long.Parse(newVal); break;
                case "^": currentVal = Math.Pow(currentVal, newValue); break;
                default: break;
            }

            return currentVal;
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

            foreach (ProtoCrewMember pcm in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (pcm.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                {
                    if (pcm.trait == "Pilot")
                    {
                        pilots++;
                        pLevels += pcm.experienceLevel;
                    }
                    else if (pcm.trait == "Engineer")
                    {
                        engineers++;
                        eLevels += pcm.experienceLevel;
                    }
                    else if (pcm.trait == "Scientist")
                    {
                        scientists++;
                        sLevels += pcm.experienceLevel;
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

            //return crewVars;
        }
    }
}
