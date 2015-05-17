using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalConstructionTime
{
    class KCT_PresetManager
    {
        public static KCT_PresetManager Instance;

        public KCT_PresetManager()
        {
            Instance = this;
        }

    }

    class KCT_Preset
    {
        public KCT_Preset_General generalSettings;
        public KCT_Preset_Time timeSettings;
        public KCT_Preset_Formula formulaSettings;

        string name = "", description = "", author = "";

        public KCT_Preset(string presetName, string presetDescription, string presetAuthor)
        {
            name = presetName;
            description = presetDescription;
            author = presetAuthor;

            generalSettings = new KCT_Preset_General();
            timeSettings = new KCT_Preset_Time();
            formulaSettings = new KCT_Preset_Formula();
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

            ConfigNode.LoadObjectFromConfig(generalSettings, node.GetNode("KCT_Preset_General"));
            ConfigNode.LoadObjectFromConfig(timeSettings, node.GetNode("KCT_Preset_Time"));
            ConfigNode.LoadObjectFromConfig(formulaSettings, node.GetNode("KCT_Preset_Formula"));
        }
    }

    class KCT_Preset_General : ConfigNodeStorage
    {
        [Persistent]
        bool NoCostSimulations = false, InstantTechUnlock = false, InstantKSCUpgrades = false, DisableBuildTime = false, EnableAllBodies = false, Reconditioning = true;
    }

    class KCT_Preset_Time : ConfigNodeStorage
    {
        [Persistent]
        double OverallMutliplier = 1.0, BuildEffect = 1.0, InventoryEffect = 100.0, ReconditioningEffect = 1728, MaxReconditioning = 345600, RolloutReconSplit = 0.25, NodeModifier = 1.0;
    }

    class KCT_Preset_Formula : ConfigNodeStorage
    {
        [Persistent]
        string NodeFormula = "2^([N]+1) / 86400",
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
