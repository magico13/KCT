using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Kerbal_Construction_Time
{
    public class KCT_BuildListVessel
    {
        public ShipConstruct ship;
        public double progress, buildPoints;
        public String launchSite, flag, shipName;
        public ListType type;
        public enum ListType { VAB, SPH };
        public List<string> InventoryParts;
        public ConfigNode shipNode;
        public Guid id;
        public double buildRate { get { return KCT_Utilities.GetBuildRate(this); } }
        public double timeLeft
        {
            get
            {
                if (buildRate > 0)
                    return (buildPoints-progress)/buildRate;
                else
                    return double.PositiveInfinity;
            }
        }

        public KCT_BuildListVessel(ShipConstruct s, String ls, double time, String flagURL)
        {
            ship = s;
            shipNode = s.SaveShip();
            shipName = s.shipName;
            launchSite = ls;
            buildPoints = time;
            progress = 0;
            flag = flagURL;
            if (launchSite == "LaunchPad")
                type = ListType.VAB;
            else
                type = ListType.SPH;
            InventoryParts = new List<string>();
            id = Guid.NewGuid();
        }

        public KCT_BuildListVessel(String name, String ls, double time, String flagURL)
        {
            ship = new ShipConstruct();
            launchSite = ls;
            shipName = name;
            buildPoints = time;
            progress = 0;
            flag = flagURL;
            if (launchSite == "LaunchPad")
                type = ListType.VAB;
            else
                type = ListType.SPH;
            InventoryParts = new List<string>();
        }

        public ShipConstruct getShip()
        {
            if (ship!= null && ship.Parts != null && ship.Parts.Count > 0) //If the parts are there, then the ship is loaded
            {
                return ship;
            }
            else if (shipNode != null)
            {
                ship.LoadShip(shipNode);
            }
            return ship;
        }

        public void Launch()
        {
            KCT_GameStates.flightSimulated = false;
            string tempFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/Ships/temp.craft";
            shipNode.Save(tempFile);
            FlightDriver.StartWithNewLaunch(tempFile, flag, launchSite, new VesselCrewManifest());
        }

        public bool RemoveFromBuildList()
        {
            string typeName="";
            bool removed = false;
            if (type == ListType.SPH)
            {
                removed = KCT_GameStates.SPHWarehouse.Remove(this);
                typeName="SPH";
            }
            else if (type == ListType.VAB)
            {
                removed = KCT_GameStates.VABWarehouse.Remove(this);
                typeName="VAB";
            }
            Debug.Log("[KCT] Removing " + shipName + " from "+ typeName +" storage.");
            if (!removed)
            {
                Debug.Log("[KCT] Failed to remove ship from storage! Performing direct comparison of ShipNode...");
                foreach (KCT_BuildListVessel blv in KCT_GameStates.SPHWarehouse)
                {
                    if (blv.shipNode == this.shipNode)
                    {
                        Debug.Log("[KCT] Ship found in SPH storage. Removing...");
                        removed = KCT_GameStates.SPHWarehouse.Remove(blv);
                    }
                }
                if (!removed)
                {
                    foreach (KCT_BuildListVessel blv in KCT_GameStates.VABWarehouse)
                    {
                        if (blv.shipNode == this.shipNode)
                        {
                            Debug.Log("[KCT] Ship found in VAB storage. Removing...");
                            removed = KCT_GameStates.VABWarehouse.Remove(blv);
                        }
                    }
                }
            }
            if (removed) Debug.Log("[KCT] Sucessfully removed ship from storage.");
            return removed;
        }

        public List<Part> GetParts() // Doesn't work. I'd like to find a way to make it work though.
        {
            List<Part> retList = new List<Part>();
            

           /* if (ship.parts != null && ship.parts.Count > 0)
                return ship.parts;*/

            ConfigNode[] partNodes = shipNode.GetNodes("PART");
            Debug.Log("[KCT] partNodes count: " + partNodes.Length);

            foreach (ConfigNode CN in partNodes)
            {
                ProtoPart p = new ProtoPart();
                ConfigNode.LoadObjectFromConfig(p, CN);
                //object o = ConfigNode.CreateObjectFromConfig("Part", CN);
                //Part p = (Part)o;
                //retList.Add(p);
                Debug.Log("[KCT] " + p);
            }

            /*foreach (Part p in retList)
            {
                Debug.Log("[KCT] Part name: " + p.partInfo.name);
            }*/
           // 
            //ProtoVessel pv = new ProtoVessel(shipNode, HighLogic.CurrentGame);
            /*pv.protoPartSnapshots[0].partRef
            foreach (ConfigNode cn in partNodes)
            {
                Part p = new Part();
                p.protoPartSnapshot.l
            }*/
           /* foreach (ProtoPartSnapshot pps in pv.protoPartSnapshots)
            {
                retList.Add(pps.partRef);
                Debug.Log("[KCT] "+pps.partRef.partInfo.name);
            }*/
            return retList;
        }

        public double AddProgress(double toAdd)
        {
            progress+=toAdd;
            return progress;
        }

        public double ProgressPercent()
        {
            return 100 * (progress / buildPoints);
        }

        public bool isComplete()
        {
            return (progress >= buildPoints);
        }
    }
}
