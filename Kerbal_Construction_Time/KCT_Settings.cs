using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerbal_Construction_Time
{
    public class KCT_Settings
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_Config.txt";
        //[Persistent] public double BuildTimeModifier;
        [Persistent] public bool SandboxEnabled;
        [Persistent] public double SimulationTimeLimit;
        [Persistent] public int MaxTimeWarp;
        [Persistent] public int SandboxUpgrades;
        [Persistent] public bool EnableAllBodies;
        [Persistent] public bool ForceStopWarp;
        public bool enabledForSave=true;
        //[Persistent] public bool AutoRevertOnCrash;
        //[Persistent] public bool Use6HourDays;


        public KCT_Settings() 
        {
           // BuildTimeModifier = 1.0;
            SandboxEnabled = true;
            SimulationTimeLimit = 7200;
            MaxTimeWarp = TimeWarp.fetch.warpRates.Count() - 1;
            EnableAllBodies = false;
            ForceStopWarp = false;
            SandboxUpgrades = 30;
           // AutoRevertOnCrash = true;
            //Use6HourDays = GameSettings.KERBIN_TIME;
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad);
            }
        }

        public void Save()
        {
            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(filePath);
        }
    }

    public class KCT_TimeSettings
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_TimeSettings.txt";
        [Persistent] public double OverallMultiplier, BuildEffect, InventoryEffect;
        
        public KCT_TimeSettings()
        {
            OverallMultiplier = 1.0;
            BuildEffect = 1.0;
            InventoryEffect = 100.0;
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