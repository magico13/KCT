using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Kerbal_Construction_Time
{
    public class KCT_BuildListVessel : IKCTBuildItem
    {
        private ShipConstruct ship;
        public double progress, buildPoints;
        public String launchSite, flag, shipName;
        public ListType type;
        public enum ListType { VAB, SPH, TechNode, Reconditioning };
        public List<string> InventoryParts;
        public ConfigNode shipNode;
        public Guid id;
        public bool cannotEarnScience;
        public float cost = 0, TotalMass = 0, DistanceFromKSC = 0;
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
        public List<Part> ExtractedParts { 
            get 
            { 
                List<Part> temp = new List<Part>();
                foreach (PseudoPart PP in this.GetPseudoParts())
                {
                    Part p = KCT_Utilities.GetAvailablePartByName(PP.name).partPrefab;
                    p.uid = PP.uid;
                    temp.Add(p);
                }
                return temp;
            } 
        }
        public List<ConfigNode> ExtractedPartNodes
        {
            get
            {
                return this.shipNode.GetNodes("PART").ToList();
            }
        }
        public bool isFinished { get { return progress >= buildPoints; } }
        public KCT_KSC KSC { get { 
            return KCT_GameStates.KSCs.FirstOrDefault(k => ( type == ListType.VAB ? (
                k.VABList.FirstOrDefault(s => s.id == this.id) != null || k.VABWarehouse.FirstOrDefault(s => s.id == this.id) != null)
            : (k.SPHList.FirstOrDefault(s => s.id == this.id) != null || k.SPHWarehouse.FirstOrDefault(s => s.id == this.id) != null))); 
        } }

        public KCT_BuildListVessel(ShipConstruct s, String ls, double bP, String flagURL)
        {
            ship = s;
            shipNode = s.SaveShip();
            shipName = s.shipName;
            //Get total ship cost
            float dry, fuel;
            s.GetShipCosts(out dry, out fuel);
            cost = dry + fuel;
            TotalMass = 0;
            foreach (Part p in s.Parts)
            {
                TotalMass += p.mass;
                TotalMass += p.GetResourceMass();
            }

            launchSite = ls;
            buildPoints = bP;
            progress = 0;
            flag = flagURL;
            if (launchSite == "LaunchPad")
                type = ListType.VAB;
            else
                type = ListType.SPH;
            InventoryParts = new List<string>();
            id = Guid.NewGuid();
            cannotEarnScience = false;
        }

        public KCT_BuildListVessel(String name, String ls, double bP, String flagURL, float spentFunds)
        {
            ship = new ShipConstruct();
            launchSite = ls;
            shipName = name;
            buildPoints = bP;
            progress = 0;
            flag = flagURL;
            if (launchSite == "LaunchPad")
                type = ListType.VAB;
            else
                type = ListType.SPH;
            InventoryParts = new List<string>();
            cannotEarnScience = false;
            cost = spentFunds;
        }

        public KCT_BuildListVessel(ProtoVessel vessel) //For recovered vessels
        {
            id = Guid.NewGuid();
            shipName = vessel.vesselName;
            //shipNode = KCT_Utilities.ProtoVesselToCraftFile(vessel);
            shipNode = KCT_GameStates.recoveryRequestVessel;

            //Fix the rotation to 0, 0, 0, 1
            ConfigNode[] parts = shipNode.GetNodes("PART");
            ConfigNode tmp = new ConfigNode("ShipNode");
            vessel.Save(tmp);
            ConfigNode[] parts2 = tmp.GetNodes("PART");
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].SetValue("rot", parts2[i].GetValue("rotation"));
                parts[i].SetValue("pos", parts2[i].GetValue("position"));
            }
            
            cost = KCT_Utilities.GetTotalVesselCost(vessel);
            TotalMass = 0;
            InventoryParts = new List<string>();
            foreach (ProtoPartSnapshot p in vessel.protoPartSnapshots)
            {
                InventoryParts.Add(p.partInfo.name + KCT_Utilities.GetTweakScaleSize(p));
                TotalMass += p.mass;
                foreach (ProtoPartResourceSnapshot rsc in p.resources)
                {
                    PartResourceDefinition def = PartResourceLibrary.Instance.GetDefinition(rsc.resourceName);
                    if (def != null)
                        TotalMass += def.density * float.Parse(rsc.resourceValues.GetValue("amount"));
                }
            }
            cannotEarnScience = true;

            buildPoints = KCT_Utilities.GetBuildTime(shipNode.GetNodes("PART").ToList(), true, InventoryParts);
            flag = HighLogic.CurrentGame.flagURL;
            progress = buildPoints;

            DistanceFromKSC = (float)SpaceCenter.Instance.GreatCircleDistance(SpaceCenter.Instance.cb.GetRelSurfaceNVector(vessel.latitude, vessel.longitude));
        }

        public void UpdateShipType(ListType newType)
        {
            this.type = newType;
            shipNode.SetValue("type", type == ListType.VAB ? "VAB" : "SPH");
            launchSite = type == ListType.VAB ? "LaunchPad" : "Runway";
        }

        public KCT_BuildListVessel NewCopy(bool RecalcTime)
        {
            KCT_BuildListVessel ret = new KCT_BuildListVessel(this.shipName, this.launchSite, this.buildPoints, this.flag, this.cost);
            ret.shipNode = this.shipNode.CreateCopy();
            ret.id = Guid.NewGuid();
            if (RecalcTime)
            {
                ret.buildPoints = KCT_Utilities.GetBuildTime(ret.ExtractedPartNodes, true, this.InventoryParts.Count > 0);
            }
            ret.TotalMass = this.TotalMass;
            return ret;
        }

        public ShipConstruct GetShip()
        {
            if (ship != null && ship.Parts != null && ship.Parts.Count > 0) //If the parts are there, then the ship is loaded
            {
                return ship;
            }
            else if (shipNode != null) //Otherwise load the ship from the ConfigNode
            {
                ship.LoadShip(shipNode);
            }
            return ship;
        }

        public void Launch()
        {
            KCT_GameStates.flightSimulated = false;
            string tempFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/Ships/temp.craft";
            UpdateRFTanks();
            shipNode.Save(tempFile);
            FlightDriver.StartWithNewLaunch(tempFile, flag, launchSite, new VesselCrewManifest());
            KCT_GameStates.LaunchFromTS = false;
        }

        private void UpdateRFTanks()
        {
            foreach (ConfigNode cn in ExtractedPartNodes)
            {
                foreach (ConfigNode module in cn.GetNodes("MODULE"))
                {
                    if (module.GetValue("name") == "ModuleFuelTanks")
                    {
                        module.SetValue("timestamp", Planetarium.GetUniversalTime().ToString());
                    }
                }
            }
        }

        //NOTE: This is an approximation. This won't properly take into account for resources and tweakscale! DO NOT USE IF YOU CARE 100% ABOUT THE MASS
        public double GetTotalMass()
        {
            if (TotalMass != 0) return TotalMass;
            double mass = 0;
            foreach (Part p in this.ExtractedParts)
            {
                mass += p.mass;
                mass += p.GetResourceMass();
            }
            return mass;
        }

        public bool RemoveFromBuildList()
        {
            string typeName="";
            bool removed = false;
            KCT_KSC theKSC = this.KSC;
            if (theKSC == null)
            {
                KCTDebug.Log("Could not find the KSC to remove vessel!");
                return false;
            }
            if (type == ListType.SPH)
            {
                if (theKSC.SPHWarehouse.Contains(this))
                    removed = theKSC.SPHWarehouse.Remove(this);
                else if (theKSC.SPHList.Contains(this))
                    removed = theKSC.SPHList.Remove(this);
                typeName="SPH";
            }
            else if (type == ListType.VAB)
            {
                if (theKSC.VABWarehouse.Contains(this))
                    removed = theKSC.VABWarehouse.Remove(this);
                else if (theKSC.VABList.Contains(this))
                    removed = theKSC.VABList.Remove(this);
                typeName="VAB";
            }
            KCTDebug.Log("Removing " + shipName + " from "+ typeName +" storage/list.");
            if (!removed)
            {
                KCTDebug.Log("Failed to remove ship from list! Performing direct comparison of ids...");
                foreach (KCT_BuildListVessel blv in theKSC.SPHWarehouse)
                {
                    if (blv.id == this.id)
                    {
                        KCTDebug.Log("Ship found in SPH storage. Removing...");
                        removed = theKSC.SPHWarehouse.Remove(blv);
                        break;
                    }
                }
                if (!removed)
                {
                    foreach (KCT_BuildListVessel blv in theKSC.VABWarehouse)
                    {
                        if (blv.id == this.id)
                        {
                            KCTDebug.Log("Ship found in VAB storage. Removing...");
                            removed = theKSC.VABWarehouse.Remove(blv);
                            break;
                        }
                    }
                }
                if (!removed)
                {
                    foreach (KCT_BuildListVessel blv in theKSC.VABList)
                    {
                        if (blv.id == this.id)
                        {
                            KCTDebug.Log("Ship found in VAB List. Removing...");
                            removed = theKSC.VABList.Remove(blv);
                            break;
                        }
                    }
                }
                if (!removed)
                {
                    foreach (KCT_BuildListVessel blv in theKSC.SPHList)
                    {
                        if (blv.id == this.id)
                        {
                            KCTDebug.Log("Ship found in SPH list. Removing...");
                            removed = theKSC.SPHList.Remove(blv);
                            break;
                        }
                    }
                }
            }
            if (removed) KCTDebug.Log("Sucessfully removed ship from storage.");
            else KCTDebug.Log("Still couldn't remove ship!");
            return removed;
        }

        public List<PseudoPart> GetPseudoParts()
        {
            List<PseudoPart> retList = new List<PseudoPart>();
            ConfigNode[] partNodes = shipNode.GetNodes("PART");
            // KCTDebug.Log("partNodes count: " + partNodes.Length);

            foreach (ConfigNode CN in partNodes)
            {
                string name = CN.GetValue("part");
                string pID = "";
                if (name != null)
                {
                    string[] split = name.Split('_');
                    name = split[0];
                    pID = split[1];
                }
                else
                {
                    name = CN.GetValue("name");
                    pID = CN.GetValue("uid");
                }
                
                //for (int i = 0; i < split.Length - 1; i++)
                //    pName += split[i];
                PseudoPart returnPart = new PseudoPart(name, pID);
                retList.Add(returnPart);
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
            return 100 * (progress / buildPoints);
        }

        string IKCTBuildItem.GetItemName()
        {
            return this.shipName;
        }

        double IKCTBuildItem.GetBuildRate()
        {
            return this.buildRate;
        }

        double IKCTBuildItem.GetTimeLeft()
        {
            return this.timeLeft;
        }

        ListType IKCTBuildItem.GetListType()
        {
            return this.type;
        }

        bool IKCTBuildItem.IsComplete()
        {
            return (progress >= buildPoints);
        }

    }

    public class PseudoPart
    {
        public string name;
        public uint uid;
        
        public PseudoPart(string PartName, uint ID)
        {
            name = PartName;
            uid = ID;
        }

        public PseudoPart(string PartName, string ID)
        {
            name = PartName;
            uid = uint.Parse(ID);
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