using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    class KCT_PresetManager
    {
        public static KCT_PresetManager Instance;
        public KCT_Preset ActivePreset;
        public List<KCT_Preset> Presets;
        public List<string> PresetPaths;

        //protected string PresetPath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/Presets/";

        public KCT_PresetManager() 
        { 
            Presets = new List<KCT_Preset>();
            PresetPaths = new List<string>();
            FindPresetFiles();
            LoadPresets();

            //KCTDebug.Log("First preset name is " + Presets[0].name);
        }

        public static bool PresetLoaded()
        {
            return Instance != null && Instance.ActivePreset != null;
        }

        public KCT_Preset FindPresetByShortName(string name)
        {
            return Presets.Find(p => p.shortName == name);
        }

        public void ClearPresets()
        {
            Presets.Clear();
            PresetPaths.Clear();
            ActivePreset = null;
        }

        public int GetIndex(KCT_Preset preset, bool softMatch=false)
        {
            foreach (KCT_Preset preset2 in Presets)
            {
                //if (PresetsEqual(preset, preset2, softMatch))
                if (KCT_Utilities.ConfigNodesAreEquivalent(preset.AsConfigNode(), preset2.AsConfigNode()))
                    return Presets.IndexOf(preset2);
            }
            return -1;
            /*
            if (Presets.Contains(preset))
                return Presets.IndexOf(preset);
            else
                return -1;*/
        }

       /* public bool PresetsEqual(KCT_Preset preset1, KCT_Preset preset2, bool softMatch=false) //softMatch means names can be different, but settings must be the same
        {
            if (!softMatch)
            {
                if (preset1.name != preset2.name)
                    return false;
                if (preset1.shortName != preset2.shortName)
                    return false;
                if (preset1.description != preset2.description)
                    return false;
                if (preset1.author != preset2.author)
                    return false;
            }
            if (preset1.generalSettings.AsConfigNode().GetValues() != preset2.generalSettings.AsConfigNode().GetValues()) //TODO: Use a better method of checking the nodes are equal. KCT2 had one I think
                return false;
            if (preset1.timeSettings.AsConfigNode().GetValues() != preset2.timeSettings.AsConfigNode().GetValues())
                return false;
            if (preset1.formulaSettings.AsConfigNode().GetValues() != preset2.formulaSettings.AsConfigNode().GetValues())
                return false;

            return true;
        }*/

        public string[] PresetShortNames(bool IncludeCustom)
        {
            List<string> names = new List<string>();
            foreach (KCT_Preset preset in Presets)
            {
                names.Add(preset.shortName);
            }
            if (IncludeCustom)
                names.Add("Custom");
            return names.ToArray();
        }

        public void SetActiveFromSaveData()
        {
            string SavedFile = KSPUtil.ApplicationRootPath + "/saves/" + HighLogic.SaveFolder+"/KCT_Settings.cfg";
            if (System.IO.File.Exists(SavedFile))
            {
                KCT_Preset saved = new KCT_Preset(SavedFile);
                if (FindPresetByShortName(saved.name) != null) //Get settings from the original preset, if it exists
                {
                    ActivePreset = FindPresetByShortName(saved.shortName);
                    KCTDebug.Log("Loading settings from preset, rather than save. Name: " + ActivePreset.name);
                }
                else
                {
                    ActivePreset = saved;
                    KCTDebug.Log("Loading saved settings.");
                }
            }
            else
            {
                KCT_Preset defaultSettings = FindPresetByShortName("default");
                if (defaultSettings != null)
                    ActivePreset = defaultSettings;
                else
                    ActivePreset = new KCT_Preset("UNINIT", "UNINIT", "NA", "NA");
            }
        }

        public void SaveActiveToSaveData()
        {
            string SavedFile = KSPUtil.ApplicationRootPath + "/saves/" + HighLogic.SaveFolder + "/KCT_Settings.cfg";
            ActivePreset.SaveToFile(SavedFile);
        }

        public void FindPresetFiles()
        {
            PresetPaths.Clear();
            //Check the KCT folder first
            foreach (string dir2 in Directory.GetDirectories(KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime"))
            {
                if (dir2.Contains("KCT_Presets")) //Found a presets folder
                {
                    //Add all the files in the folder
                    foreach (string file in Directory.GetFiles(dir2, "*.cfg"))
                    {
                        KCTDebug.Log("Found preset at " + file);
                        PresetPaths.Add(file);
                    }
                }
            }

            foreach (string dir in Directory.GetDirectories(KSPUtil.ApplicationRootPath + "GameData/"))
            {
                if (dir.Contains("KerbalConstructionTime")) continue; //Don't check the KCT folder again
                foreach (string dir2 in Directory.GetDirectories(dir))
                {
                    if (dir2.Contains("KCT_Presets")) //Found a presets folder
                    {
                        //Add all the files in the folder
                        foreach (string file in Directory.GetFiles(dir2, "*.cfg"))
                        {
                            KCTDebug.Log("Found preset at " + file);
                            PresetPaths.Add(file);
                        }
                    }
                }
            }
        }

        public void LoadPresets()
        {
            Presets.Clear();
            
           // foreach (string file in System.IO.Directory.GetFiles(PresetPath, "*.cfg"))
            foreach (string file in PresetPaths)
            {
                try
                {
                    KCT_Preset newPreset = new KCT_Preset(file);
                    if (KCT_Utilities.CurrentGameIsCareer() && !newPreset.CareerEnabled) continue; //Don't display presets that aren't designed for this game mode
                    if (HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX && !newPreset.ScienceEnabled) continue;
                    if (KCT_Utilities.CurrentGameIsSandbox() && !newPreset.SandboxEnabled) continue;
                    //KCT_Preset existing = Presets.Find(p => p.name == newPreset.name);
                    KCT_Preset existing = FindPresetByShortName(newPreset.shortName);
                    if (existing != null) //Ensure there is only one preset with a given name. Take the last one found as the final one.
                    {
                        Presets.Remove(existing);
                    }
                    Presets.Add(newPreset);
                }
                catch
                {
                    Debug.LogError("[KCT] Could not load preset at " + file);
                }
            }
        }

        public void DeletePresetFile(string shortName)
        {
            KCT_Preset toDelete = FindPresetByShortName(shortName);
            if (toDelete != null && toDelete.AllowDeletion)
            {
                File.Delete(toDelete.presetFileLocation);
            }
            FindPresetFiles();
            LoadPresets();
        }

        public int StartingUpgrades(Game.Modes mode)
        {
            if (mode == Game.Modes.CAREER)
            {
                return ActivePreset.start_upgrades[0];
            }
            else if (mode == Game.Modes.SCIENCE_SANDBOX)
            {
                return ActivePreset.start_upgrades[1];
            }
            else
            {
                return ActivePreset.start_upgrades[2];
            }
        }
    }

    public class KCT_Preset
    {
        public string presetFileLocation = "";

        public KCT_Preset_General generalSettings = new KCT_Preset_General();
        public KCT_Preset_Time timeSettings = new KCT_Preset_Time();
        public KCT_Preset_Formula formulaSettings = new KCT_Preset_Formula();

        public string name = "UNINIT", shortName = "UNINIT", description = "NA", author = "NA";
        public bool CareerEnabled = true, ScienceEnabled = true, SandboxEnabled = true; //These just control whether it should appear during these game types
        public bool AllowDeletion = true;


        private int[] upgrades_internal;
        public int[] start_upgrades //TODO: Actually implement the starting points
        {
            get
            {
                if (upgrades_internal == null)
                {
                    upgrades_internal = new int[3] {0, 0, 0}; //career, science, sandbox
                    string[] upgrades = generalSettings.StartingPoints.Split(',');
                    for (int i=0; i<3; i++)
                        if (!int.TryParse(upgrades[i], out upgrades_internal[i]))
                            upgrades_internal[i] = 0;
                }
                return upgrades_internal;
            }
        }

        public KCT_Preset(string filePath)
        {
            LoadFromFile(filePath);
        }

        public KCT_Preset(string presetName, string presetShortName, string presetDescription, string presetAuthor)
        {
            name = presetName;
            shortName = presetShortName;
            description = presetDescription;
            author = presetAuthor;
        }

        public KCT_Preset(KCT_Preset Source)
        {
            name = Source.name;
            shortName = Source.shortName;
            description = Source.description;
            author = Source.author;
            AllowDeletion = Source.AllowDeletion;

            CareerEnabled = Source.CareerEnabled;
            ScienceEnabled = Source.ScienceEnabled;
            SandboxEnabled = Source.SandboxEnabled;

           // generalSettings = Source.generalSettings;
            //timeSettings = Source.timeSettings;
            //formulaSettings = Source.formulaSettings;

            ConfigNode.LoadObjectFromConfig(generalSettings, Source.generalSettings.AsConfigNode());
            ConfigNode.LoadObjectFromConfig(timeSettings, Source.timeSettings.AsConfigNode());
            ConfigNode.LoadObjectFromConfig(formulaSettings, Source.formulaSettings.AsConfigNode());
        }

        public ConfigNode AsConfigNode()
        {
            ConfigNode node = new ConfigNode("KCT_Preset");
            node.AddValue("name", name);
            node.AddValue("shortName", shortName);
            node.AddValue("description", description);
            node.AddValue("author", author);

            node.AddValue("allowDeletion", AllowDeletion);

            node.AddValue("career", CareerEnabled);
            node.AddValue("science", ScienceEnabled);
            node.AddValue("sandbox", SandboxEnabled);

            node.AddNode(generalSettings.AsConfigNode());
            node.AddNode(timeSettings.AsConfigNode());
            node.AddNode(formulaSettings.AsConfigNode());
            return node;
        }

        public void FromConfigNode(ConfigNode node)
        {
            name = node.GetValue("name");
            shortName = node.GetValue("shortName");
            description = node.GetValue("description");
            author = node.GetValue("author");

            bool.TryParse(node.GetValue("allowDeletion"), out AllowDeletion);

            bool.TryParse(node.GetValue("career"), out CareerEnabled);
            bool.TryParse(node.GetValue("science"), out ScienceEnabled);
            bool.TryParse(node.GetValue("sandbox"), out SandboxEnabled);

            //ConfigNode toLoad = new ConfigNode("KCT_Preset_General");
            //toLoad.AddNode(node.getn)
            ConfigNode.LoadObjectFromConfig(generalSettings, node.GetNode("KCT_Preset_General"));
            ConfigNode.LoadObjectFromConfig(timeSettings, node.GetNode("KCT_Preset_Time"));
            ConfigNode.LoadObjectFromConfig(formulaSettings, node.GetNode("KCT_Preset_Formula"));
        }

        public void SaveToFile(string filePath)
        {
            ConfigNode node = new ConfigNode("KCT_Preset");
            node.AddNode(this.AsConfigNode());
            node.Save(filePath);
        }

        public void LoadFromFile(string filePath)
        {
            KCTDebug.Log("Loading a preset from " + filePath);
            presetFileLocation = filePath;
            ConfigNode node = ConfigNode.Load(filePath);
            this.FromConfigNode(node.GetNode("KCT_Preset")); //.GetNode("KCT_Preset")
        }

        public void SetActive()
        {
            KCT_PresetManager.Instance.ActivePreset = this;
        }
    }

    public class KCT_Preset_General : ConfigNodeStorage
    {
        [Persistent]
        public bool Enabled = true, BuildTimes = true, ReconditioningTimes = true, TechUnlockTimes = true, KSCUpgradeTimes = true,
            Simulations = true, SimulationCosts = true, RequireVisitsForSimulations = true,
            TechUpgrades = true;
        [Persistent]
        public string StartingPoints = "15,15,45"; //Career, Science, and Sandbox modes
    }

    public class KCT_Preset_Time : ConfigNodeStorage
    {
        [Persistent]
        public double OverallMultiplier = 1.0, BuildEffect = 1.0, InventoryEffect = 100.0, ReconditioningEffect = 1728, MaxReconditioning = 345600, RolloutReconSplit = 0.25;
    }

    public class KCT_Preset_Formula : ConfigNodeStorage
    {
        [Persistent]
        public string NodeFormula = "2^([N]+1) / 86400",
            UpgradeFundsFormula = "min(2^([N]+4) * 1000, 1024000)",
            UpgradeScienceFormula = "min(2^([N]+2) * 1.0, 512)",
            ResearchFormula = "[N]*0.5/86400",
            EffectivePartFormula = "min([C]/([I] + ([B]*([U]+1))), [C])",
            ProceduralPartFormula = "(([C]-[A]) + ([A]*10/max([I],1))) / max([B]*([U]+1),1)",
            BPFormula = "([E]^(1/2))*2000*[O]",
            KSCUpgradeFormula = "([C]^(1/2))*1000*[O]",
            ReconditioningFormula = "min([M]*[O]*[E], [X])*abs([RE]-[S])",
            BuildRateFormula = "(([I]+1)*0.05*[N] + max(0.1-[I], 0))*sign(2*[L]-[I]+1)", //TODO: Implement simulation cost formulas, reset formula
            SimCostFormula = "max([C]/50000 * min([PM]/[KM], 80) * ([S]/10 + 1) * ([A]/10 + 1) * ([L]^0.5) * 100, 500)", //[M] = body mass, [PM] = parent mass, [A] = presence of atmosphere (1 or 0), [m] = mass of vessel, [C] = cost of vessel, [s] = # times simulated this editor session, [SMA] = ratio parent planet SMA to Kerbin SMA, [L] = Simulation length in seconds, [KM] = Kerbin Mass, [S] = 1/0 if a satellite
            KerbinSimCostFormula = "max([C]/50000 * ([L]^0.5) * 10, 100)",
            UpgradeResetFormula = "2*([N]+1)", //N = number of times it's been reset
            InventorySaleFormula = "([V]+[P] / 10000)^(0.5)", //Gives the TOTAL amount of points, decimals are kept //[V] = inventory value in funds, [P] = Value of all previous sales combined
            RolloutCostFormula = "0"; //[M]=Vessel loaded mass, [m]=vessel empty mass, [C]=vessel loaded cost, [c]=vessel empty cost, [BP]=vessel BPs, [E]=editor level, [L]=launch site level (pad), [VAB]=1 if VAB craft, 0 if SPH
    }
}
