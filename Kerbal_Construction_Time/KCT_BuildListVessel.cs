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
        public double progress, buildTime;
        public String launchSite, flag, shipName;
        public ListType type;
        public enum ListType { VAB, SPH };
        public List<string> InventoryParts;
        public ConfigNode shipNode;
        public Guid id;

        public KCT_BuildListVessel(ShipConstruct s, String ls, double time, String flagURL)
        {
            ship = s;
            shipNode = s.SaveShip();
            shipName = s.shipName;
            launchSite = ls;
            buildTime = time;
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
            buildTime = time;
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
            if (ship.Parts != null && ship.Parts.Count > 0) //If the parts are there, then the ship is loaded
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

        public void RemoveFromBuildList()
        {
            string typeName="";
            if (type == ListType.SPH)
            {
                KCT_GameStates.SPHWarehouse.Remove(this);
                typeName="SPH";
            }
            else if (type == ListType.VAB)
            {
                KCT_GameStates.VABWarehouse.Remove(this);
                typeName="VAB";
            }
            Debug.Log("[KCT] Removing " + shipName + " from "+ typeName +" build list.");
        }

        public List<Part> GetParts() // Doesn't work. I'd like to find a way to make it work though.
        {
           /* if (ship.parts != null && ship.parts.Count > 0)
                return ship.parts;
            */

            List<Part> retList = new List<Part>();
           // ConfigNode[] partNodes = shipNode.GetNodes("PART");
            ProtoVessel pv = new ProtoVessel(shipNode, HighLogic.CurrentGame);
            /*pv.protoPartSnapshots[0].partRef
            foreach (ConfigNode cn in partNodes)
            {
                Part p = new Part();
                p.protoPartSnapshot.l
            }*/
            foreach (ProtoPartSnapshot pps in pv.protoPartSnapshots)
            {
                retList.Add(pps.partRef);
                Debug.Log("[KCT] "+pps.partRef.partInfo.name);
            }
            return retList;
        }

        public double AddProgress(double toAdd)
        {
            progress+=toAdd;
            return progress;
        }

        public double ProgressPercent()
        {
            return 100 * (progress / buildTime);
        }

        public bool isComplete()
        {
            return (progress >= buildTime);
        }
    }
}
