using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalConstructionTime
{
    public abstract class ConfigNodeStorage : IPersistenceLoad, IPersistenceSave
    {
        public ConfigNodeStorage() { }

        void IPersistenceLoad.PersistenceLoad()
        {
            OnDecodeFromConfigNode();
        }

        void IPersistenceSave.PersistenceSave()
        {
            OnEncodeToConfigNode();
        }

        public virtual void OnDecodeFromConfigNode() { }
        public virtual void OnEncodeToConfigNode() { }

        public ConfigNode AsConfigNode()
        {
            try
            {
                //Create a new Empty Node with the class name
                ConfigNode cnTemp = new ConfigNode(this.GetType().Name);
                //Load the current object in there
                cnTemp = ConfigNode.CreateConfigFromObject(this, cnTemp);
                return cnTemp;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                //Logging and return value?                    
                return new ConfigNode(this.GetType().Name);
            }
        }
    }

    public class FakePart : ConfigNodeStorage
    {
        [Persistent] public string part = "";
    }

    public class FakeTechNode : ConfigNodeStorage
    {
        [Persistent] public string id = "";
        [Persistent] public string state = "";

        public ProtoTechNode ToProtoTechNode()
        {
            ProtoTechNode ret = new ProtoTechNode();
            ret.techID = id;
            if (state == "Available")
                ret.state = RDTech.State.Available;
            else
                ret.state = RDTech.State.Unavailable;
            return ret;
        }

        public FakeTechNode FromProtoTechNode(ProtoTechNode node)
        {
            this.id = node.techID;
            this.state = node.state.ToString();
            return this;
        }
    }
    public class KCT_DataStorage : ConfigNodeStorage
    {
        [Persistent] bool enabledForSave = (HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX
            || (HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX));




        //[Persistent] bool firstStart = true;
        [Persistent] List<int> VABUpgrades = new List<int>() {0};
        [Persistent] List<int> SPHUpgrades = new List<int>() {0};
        [Persistent] List<int> RDUpgrades = new List<int>() {0,0};
        [Persistent] List<int> PurchasedUpgrades = new List<int>() {0,0};
        [Persistent] List<String> PartTracker = new List<String>();
        [Persistent] List<String> PartInventory = new List<String>();
        [Persistent] string activeKSC = "";
        [Persistent] float SalesFigures = 0;
        [Persistent] int UpgradesResetCounter = 0, TechUpgrades = 0, SavedUpgradePointsPreAPI = 0;


        public override void OnDecodeFromConfigNode()
        {
            //KCT_GameStates.PartTracker = ListToDict(PartTracker);
            //KCT_GameStates.PartInventory = ListToDict(PartInventory);
          /*  KCT_GameStates.ActiveKSC.VABUpgrades = VABUpgrades;
            KCT_GameStates.ActiveKSC.SPHUpgrades = SPHUpgrades;
            KCT_GameStates.ActiveKSC.RDUpgrades = RDUpgrades;*/
            KCT_GameStates.PurchasedUpgrades = PurchasedUpgrades;
            KCT_GameStates.activeKSCName = activeKSC;
            //KCT_GameStates.InventorySalesFigures = SalesFigures;
            //KCT_GameStates.InventorySaleUpgrades = (float)KCT_MathParsing.GetStandardFormulaValue("InventorySales", new Dictionary<string, string> { { "V", "0" }, { "P", SalesFigures.ToString() } });
            KCT_GameStates.UpgradesResetCounter = UpgradesResetCounter;
            KCT_GameStates.TechUpgradesTotal = TechUpgrades;
            KCT_GameStates.PermanentModAddedUpgradesButReallyWaitForTheAPI = SavedUpgradePointsPreAPI;

            SetSettings();
            //KCT_GameStates.firstStart = firstStart;
        }

        public override void OnEncodeToConfigNode()
        {
            //PartTracker = DictToList(KCT_GameStates.PartTracker);
            //PartInventory = DictToList(KCT_GameStates.PartInventory);
           // enabledForSave = KCT_GameStates.settings.enabledForSave;
            /*VABUpgrades = KCT_GameStates.VABUpgrades;
            SPHUpgrades = KCT_GameStates.SPHUpgrades;
            RDUpgrades = KCT_GameStates.RDUpgrades;*/
            TechUpgrades = KCT_GameStates.TechUpgradesTotal;
            PurchasedUpgrades = KCT_GameStates.PurchasedUpgrades;
            //firstStart = KCT_GameStates.firstStart;
            activeKSC = KCT_GameStates.ActiveKSC.KSCName;
            SalesFigures = KCT_GameStates.InventorySalesFigures;
            UpgradesResetCounter = KCT_GameStates.UpgradesResetCounter;
            SavedUpgradePointsPreAPI = KCT_GameStates.PermanentModAddedUpgradesButReallyWaitForTheAPI;

            GetSettings();
        }

        private void SetSettings()
        {
            //KCT_GameStates.settings.enabledForSave = enabledForSave;
            /*KCT_GameStates.settings.RecoveryModifier = RecoveryModifier;
            KCT_GameStates.settings.DisableBuildTime = DisableBuildTime;
            KCT_GameStates.settings.InstantTechUnlock = InstantTechUnlock;
            KCT_GameStates.settings.EnableAllBodies = EnableAllBodies;
            KCT_GameStates.settings.Reconditioning = Reconditioning;*/
        }

        private void GetSettings()
        {
           // enabledForSave = KCT_GameStates.settings.enabledForSave;
            /*RecoveryModifier = KCT_GameStates.settings.RecoveryModifier;
            DisableBuildTime = KCT_GameStates.settings.DisableBuildTime;
            InstantTechUnlock = KCT_GameStates.settings.InstantTechUnlock;
            EnableAllBodies = KCT_GameStates.settings.EnableAllBodies;
            Reconditioning = KCT_GameStates.settings.Reconditioning;*/
        }

        private bool VesselIsInWorld(Guid id)
        {
            foreach (Vessel vssl in FlightGlobals.Vessels)
            {
                if (vssl.id == id)
                    return true;
            }
            return false;
        }
        public List<String> DictToList(Dictionary<String, int> dict)
        {
            List<String> list = new List<String>();
            foreach (string k in dict.Keys)
            {
                int val = dict[k];
                list.Add(k);
                list.Add(val.ToString());
            }
            return list;
        }
        public Dictionary<String, int> ListToDict(List<String> list)
        {
            Dictionary<String, int> dict = new Dictionary<String, int>();
            for (int i = 0; i < list.Count; i+=2 )
            {
                dict.Add(list[i], int.Parse(list[i + 1]));
            }
            return dict;
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
