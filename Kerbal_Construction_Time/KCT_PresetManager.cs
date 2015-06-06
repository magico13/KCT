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

        public void SetActiveFromSaveData()
        {
            string SavedFile = KSPUtil.ApplicationRootPath + "/saves/" + HighLogic.SaveFolder+"/KCT_Settings.cfg";
            if (System.IO.File.Exists(SavedFile))
            {
                ActivePreset = new KCT_Preset(SavedFile);
            }
            else
            {
                ActivePreset = new KCT_Preset("UNINIT", "NA", "NA");
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
            foreach (string dir in Directory.GetDirectories(KSPUtil.ApplicationRootPath + "GameData/"))
            {
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
                    Presets.Add(newPreset);
                }
                catch
                {
                    Debug.LogError("[KCT] Could not load preset at " + file);
                }
            }
        }
    }

    class KCT_Preset
    {
        public KCT_Preset_General generalSettings = new KCT_Preset_General();
        public KCT_Preset_Time timeSettings = new KCT_Preset_Time();
        public KCT_Preset_Formula formulaSettings = new KCT_Preset_Formula();

        public string name = "UNINIT", description = "NA", author = "NA";

        public KCT_Preset(string filePath)
        {
            LoadFromFile(filePath);
        }

        public KCT_Preset(string presetName, string presetDescription, string presetAuthor)
        {
            name = presetName;
            description = presetDescription;
            author = presetAuthor;
        }

        public ConfigNode AsConfigNode()
        {
            ConfigNode node = new ConfigNode("KCT_Preset");
            node.AddValue("name", name);
            node.AddValue("description", description);
            node.AddValue("author", author);
            node.AddNode(generalSettings.AsConfigNode());
            node.AddNode(timeSettings.AsConfigNode());
            node.AddNode(formulaSettings.AsConfigNode());
            return node;
        }

        public void FromConfigNode(ConfigNode node)
        {
            name = node.GetValue("name");
            description = node.GetValue("description");
            author = node.GetValue("author");

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
            ConfigNode node = ConfigNode.Load(filePath);
            this.FromConfigNode(node.GetNode("KCT_Preset")); //.GetNode("KCT_Preset")
        }

        public void SetActive()
        {
            KCT_PresetManager.Instance.ActivePreset = this;
        }
    }

    class KCT_Preset_General : ConfigNodeStorage
    {
        [Persistent]
        public bool BuildTimes = true, ReconditioningTimes = true, TechUnlockTimes = true, KSCUpgradeTimes = true, SimulationCosts = true, RequireVisitsForSimulations = true;
    }

    class KCT_Preset_Time : ConfigNodeStorage
    {
        [Persistent]
        public double OverallMultiplier = 1.0, BuildEffect = 1.0, InventoryEffect = 100.0, ReconditioningEffect = 1728, MaxReconditioning = 345600, RolloutReconSplit = 0.25, NodeModifier = 1.0;
    }

    class KCT_Preset_Formula : ConfigNodeStorage
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
            ReconditioningFormula = "min([M]*[O]*[E], [X])",
            BuildRateFormula = "(([I]+1)*0.05*[N] + max(0.1-[I], 0))*sign(2*[L]-[I]+1)";
    }
}
