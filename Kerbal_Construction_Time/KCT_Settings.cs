using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalConstructionTime
{
    public class KCT_Settings
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_Config.txt";
        //[Persistent] public double BuildTimeModifier;
        //[Persistent] public bool SandboxEnabled;
        [Persistent] public int MaxTimeWarp;
        //[Persistent] public int SandboxUpgrades;
        [Persistent] public bool ForceStopWarp;
        //[Persistent] public bool DisableRecoveryMessages;
        [Persistent] public bool DisableAllMessages;
        //[Persistent] public bool CheckForUpdates, VersionSpecific;
        [Persistent] public bool AutoKACAlarms;
        /*[Persistent] public float RecoveryModifierDefault;
        [Persistent] public bool NoCostSimulationsDefault;
        [Persistent] public bool InstantTechUnlockDefault;
        [Persistent] public bool InstantKSCUpgradeDefault;
        [Persistent] public bool DisableBuildTimeDefault;
        [Persistent] public bool EnableAllBodiesDefault;
        [Persistent] public bool ReconditioningDefault;*/
        [Persistent] public bool Debug;
        [Persistent] public bool OverrideLaunchButton;
        [Persistent] public bool PreferBlizzyToolbar;
        //[Persistent] public bool AllowParachuteRecovery;
        [Persistent] public bool NoSimGUI;
        [Persistent] public bool CheckForDebugUpdates = GameSettings.SEND_PROGRESS_DATA;
        //[Persistent] public bool DisableSpecialSurprise;

        [Persistent] public bool RandomizeCrew;
        [Persistent] public bool AutoHireCrew;

        [Persistent] public int WindowMode = 1;

        //Game specific settings
        //public bool enabledForSave = true;
        /*public float RecoveryModifier;
        public bool NoCostSimulations;
        public bool InstantTechUnlock;
        public bool InstantKSCUpgrades;
        public bool DisableBuildTime;
        public bool EnableAllBodies;
        public bool Reconditioning;*/
        
        //[Persistent] public bool AutoRevertOnCrash;
        //[Persistent] public bool Use6HourDays;


        public KCT_Settings() 
        {
           // BuildTimeModifier = 1.0;
           // SandboxEnabled = true;
            MaxTimeWarp = TimeWarp.fetch.warpRates.Count() - 1;
            
            ForceStopWarp = false;
            //SandboxUpgrades = 45;
            //DisableRecoveryMessages = false;
            DisableAllMessages = false;
           // CheckForUpdates = GameSettings.SEND_PROGRESS_DATA;
            //VersionSpecific = false;
            Debug = false;
            OverrideLaunchButton = false;
          //  RecoveryModifier = 0.75F;
          //  Reconditioning = true;
            AutoKACAlarms = true;
            PreferBlizzyToolbar = false;
          //  AllowParachuteRecovery = true;
            NoSimGUI = false;
           // DisableSpecialSurprise = false;

          /*  RecoveryModifierDefault = 0.75f;
            NoCostSimulationsDefault = false;
            InstantTechUnlockDefault = false;
            InstantKSCUpgradeDefault = false;
            DisableBuildTimeDefault = false;
            EnableAllBodiesDefault = false;
            ReconditioningDefault = true;*/

           // AutoRevertOnCrash = true;
            //Use6HourDays = GameSettings.KERBIN_TIME;
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad);

             /*   if (RecoveryModifierDefault < 0) RecoveryModifierDefault = 0;
                if (RecoveryModifierDefault > 1) RecoveryModifierDefault = 1;

                if (KCT_GameStates.firstStart)
                {
                    RecoveryModifier = RecoveryModifierDefault;
                    NoCostSimulations = NoCostSimulationsDefault;
                    InstantTechUnlock = InstantTechUnlockDefault;
                    InstantKSCUpgrades = InstantKSCUpgradeDefault;
                    DisableBuildTime = DisableBuildTimeDefault;
                    EnableAllBodies = EnableAllBodiesDefault;
                    Reconditioning = ReconditioningDefault;
                }*/

                KCT_GUI.autoHire = AutoHireCrew;
                KCT_GUI.randomCrew = RandomizeCrew;
            }
        }

        public void Save()
        {
            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(filePath);
        }
    }

    /*public class KCT_TimeSettings
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_TimeSettings.txt";
        [Persistent] public double OverallMultiplier, BuildEffect, InventoryEffect, ReconditioningEffect, MaxReconditioning, RolloutReconSplit, NodeModifier;
        
        public KCT_TimeSettings()
        {
            OverallMultiplier = 1.0;
            BuildEffect = 1.0;
            InventoryEffect = 100.0;
            ReconditioningEffect = 1728;
            MaxReconditioning = 345600; // This is 4 days / 16 days at 1 BP/s  (or 200 tons)
            RolloutReconSplit = 0.25;
            NodeModifier = 1.0;
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad);
                if (OverallMultiplier < 0)
                    OverallMultiplier = 0;
                if (BuildEffect < 0)
                    BuildEffect = 0;
                if (InventoryEffect < 0)
                    InventoryEffect = 0;
            }
        }

        public void Save()
        {
            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(filePath);
        }
    }*/

    /*public class KCT_FormulaSettings
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_Formulas.cfg";
        [Persistent] public string NodeFormula, UpgradeFundsFormula, UpgradeScienceFormula, ResearchFormula, EffectivePartFormula, ProceduralPartFormula, BPFormula,
            KSCUpgradeFormula, ReconditioningFormula, BuildRateFormula;
       // [Persistent] public string NodeMax, UpgradeFundsMax, UpgradeScienceMax;
        public KCT_FormulaSettings()
        {
            NodeFormula = "2^([N]+1) / 86400"; //Rate = 2^(N+1)/86400 BP/s
        //    NodeMax = "0";
            UpgradeFundsFormula = "min(2^([N]+4) * 1000, 1024000)";
        //    UpgradeFundsMax = "1024000";
            UpgradeScienceFormula = "min(2^([N]+2) * 1.0, 512)";
       //     UpgradeScienceMax = "512";
            ResearchFormula = "[N]*0.5/86400";
            EffectivePartFormula = "min([C]/([I] + ([B]*([U]+1))), [C])";
            ProceduralPartFormula = "(([C]-[A]) + ([A]*10/max([I],1))) / max([B]*([U]+1),1)";
            BPFormula = "([E]^(1/2))*2000*[O]";
            KSCUpgradeFormula = "([C]^(1/2))*1000*[O]";
            ReconditioningFormula = "min([M]*[O]*[E], [X])";
            BuildRateFormula = "(([I]+1)*0.05*[N] + max(0.1-[I], 0))*sign(2*[L]-[I]+1)"; //N = num upgrades, I = rate index, L = VAB/SPH upgrade level, R = R&D level
                //lvl0->2 rates, lvl1->4 rates, lvl2->6 rates
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad.GetNode("KCT_FormulaSettings"));
            }
        }

        public void Save()
        {
            ConfigNode cnTemp = new ConfigNode("KCT_FormulaSettings");
            ConfigNode.CreateConfigFromObject(this, new ConfigNode()).CopyTo(cnTemp);
            ConfigNode toSave = new ConfigNode();
            toSave.AddNode(cnTemp);
            toSave.Save(filePath);
        }
    }*/
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