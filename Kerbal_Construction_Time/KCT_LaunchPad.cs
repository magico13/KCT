using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace KerbalConstructionTime
{
    public class KCT_LaunchPad : ConfigNodeStorage
    {
        [Persistent] public int level = 0;
        [Persistent] public string name = "LaunchPad";
        public ConfigNode DestructionNode = new ConfigNode("DestructionState");

        public bool destroyed
        {
            get
            {
                string nodeStr = level == 2 ? "SpaceCenter/LaunchPad/Facility/LaunchPadMedium/ksp_pad_launchPad" : "SpaceCenter/LaunchPad/Facility/building";
                ConfigNode mainNode = DestructionNode.GetNode(nodeStr);
                if (mainNode == null)
                    return false;
                else
                    return !bool.Parse(mainNode.GetValue("intact"));
            }
        }

        private string LPID = "SpaceCenter/LaunchPad";

        public KCT_LaunchPad(string LPName, int lvl=0)
        {
            name = LPName;
            level = lvl;
        }


        public void SetActive()
        {
            try
            {
                KCTDebug.Log("Switching to LaunchPad: "+name+ " lvl: "+level+" destroyed? "+destroyed);
                KCT_GameStates.ActiveKSC.ActiveLaunchPadID = KCT_GameStates.ActiveKSC.LaunchPads.IndexOf(this);

                //set the level to this level
                if (KCT_Utilities.CurrentGameIsCareer())
                {
                    foreach (Upgradeables.UpgradeableFacility facility in GetUpgradeableFacilityReferences())
                    {
                        KCT_Events.allowedToUpgrade = true;
                        facility.SetLevel(level);
                    }
                }

                //set the destroyed state to this destroyed state
                //might need to do this one frame later?
             //   RefreshDesctructibleState();
                KCT_GameStates.UpdateLaunchpadDestructionState = true;
            }
            catch (Exception e)
            {
                KCTDebug.Log("Error while calling SetActive: " + e.Message + e.StackTrace);
            }
        }

        public void SetDestructibleStateFromNode()
        {
            foreach (DestructibleBuilding facility in GetDestructibleFacilityReferences())
            {
                /*ConfigNode aNode = new ConfigNode();
                facility.Save(aNode);
                aNode.SetValue("intact", (!destroyed).ToString());*/
                ConfigNode aNode = DestructionNode.GetNode(facility.id);
                if (aNode != null)
                    facility.Load(aNode);
            }
        }

        public void RefreshDestructionNode()
        {
            DestructionNode = new ConfigNode("DestructionState");
            foreach (DestructibleBuilding facility in GetDestructibleFacilityReferences())
            {
                ConfigNode aNode = new ConfigNode(facility.id);
                facility.Save(aNode);
                DestructionNode.AddNode(aNode);
            }
        }

        List<Upgradeables.UpgradeableFacility> GetUpgradeableFacilityReferences()
        {
            return ScenarioUpgradeableFacilities.protoUpgradeables[LPID].facilityRefs;
        }

        List<DestructibleBuilding> GetDestructibleFacilityReferences()
        {

            List<DestructibleBuilding> destructibles = new List<DestructibleBuilding>();
            foreach (KeyValuePair<string, ScenarioDestructibles.ProtoDestructible> kvp in ScenarioDestructibles.protoDestructibles)
            {
                if (kvp.Key.Contains("LaunchPad"))
                {
                    destructibles.AddRange(kvp.Value.dBuildingRefs);
                }
            }
            return destructibles;
        }
    }
}
