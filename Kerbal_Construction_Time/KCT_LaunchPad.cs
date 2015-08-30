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
        [Persistent] public bool destroyed = false;
        [Persistent] public string name = "LaunchPad";

        private string LPID = "SpaceCenter/LaunchPad";

        public KCT_LaunchPad(string LPName, int lvl=0, bool dstry=false)
        {
            name = LPName;
            level = lvl;
            destroyed = dstry;
        }


        public void SetActive()
        {
            try
            {
                KCTDebug.Log("Switching to LaunchPad: "+name+ " lvl: "+level+" destroyed? "+destroyed);
                KCT_GameStates.ActiveKSC.ActiveLaunchPadID = KCT_GameStates.ActiveKSC.LaunchPads.IndexOf(this);

                //set the level to this level
                foreach (Upgradeables.UpgradeableFacility facility in GetUpgradeableFacilityReferences())
                {
                    KCT_Events.allowedToUpgrade = true;
                    facility.SetLevel(level);
                }

                //set the destroyed state to this destroyed state
                //might need to do this one frame later?
               // RefreshDesctructibleState();
                KCT_GameStates.UpdateLaunchpadDestructionState = true;
            }
            catch (Exception e)
            {
                KCTDebug.Log("Exception while calling SetActive " + e.StackTrace);
            }
        }

        public void RefreshDesctructibleState()
        {
            foreach (DestructibleBuilding facility in GetDestructibleFacilityReferences())
            {
               /* KCTDebug.Log(facility.id);

                if (destroyed)
                    facility.Demolish();
                else
                    facility.Repair();*/


                
                foreach (DestructibleBuilding.CollapsibleObject collapsable in facility.CollapsibleObjects)
                {
                    collapsable.SetDestroyed(destroyed);
                }
                
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
