using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalConstructionTime
{
   /* public class KCT_KSCTech : IKCTBuildItem
    {
        [Persistent] public string Name = ""; //ex: VAB Upgrade LVL 2, LaunchPad Repair
        [Persistent] public double progress = 0, BP = 0;

        private KCT_KSC _KSC = null;
        public KCT_KSC KSC
        {
            get
            {
                if (_KSC == null)
                    _KSC = KCT_GameStates.KSCs.Find(ksc => ksc.KSCTech.Contains(this));
                return _KSC;
            }
        }

        string IKCTBuildItem.GetItemName()
        {
            return Name;
        }
        double IKCTBuildItem.GetBuildRate()
        {
            double rateTotal = 0;
            if (KSC != null)
            {
                foreach (double rate in KCT_Utilities.BuildRatesSPH(KSC))
                    rateTotal += rate;
                foreach (double rate in KCT_Utilities.BuildRatesVAB(KSC))
                    rateTotal += rate;
            }
            return rateTotal;
        }
        double IKCTBuildItem.GetTimeLeft()
        {
            return (BP - progress)/((IKCTBuildItem)this).GetBuildRate();
        }
        KCT_BuildListVessel.ListType IKCTBuildItem.GetListType()
        {
            return KCT_BuildListVessel.ListType.Reconditioning;
        }
        bool IKCTBuildItem.IsComplete()
        {
            return progress >= BP;
        }
        public IKCTBuildItem AsIKCTBuildItem()
        {
            return (IKCTBuildItem)this;
        }
        public void AddProgress(double amt)
        {
            progress += amt;
            if (progress > BP) progress = BP;
        }

        public void ActivateTech()
        {
           // SpaceCenterFacility facility = GetFacility();
            string fName = GetFacilityName();
            Upgradeables.UpgradeableFacility upFacil = null;
            if (ScenarioUpgradeableFacilities.protoUpgradeables.ContainsKey(fName))
                upFacil = ScenarioUpgradeableFacilities.protoUpgradeables[fName].facilityRefs.Find(f => f.name == fName);
            if (upFacil != null)
            {
                int lvl = GetLevel();
                if (lvl > 0)
                    upFacil.SetLevel(lvl);
            }

        }

        public bool IsInListAlready(KCT_KSCTech up)
        {
            return (KSC != null || KCT_GameStates.ActiveKSC.KSCTech.Find(u => u.Name == up.Name) != null);
        }

        public SpaceCenterFacility GetFacility()
        {
            if (Name.Contains("Admin"))
                return SpaceCenterFacility.Administration;
            else if (Name.Contains("Astronaut"))
                return SpaceCenterFacility.AstronautComplex;
            else if (Name.Contains("LaunchPad"))
                return SpaceCenterFacility.LaunchPad;
            else if (Name.Contains("Mission"))
                return SpaceCenterFacility.MissionControl;
            else if (Name.Contains("R&D"))
                return SpaceCenterFacility.ResearchAndDevelopment;
            else if (Name.Contains("Runway"))
                return SpaceCenterFacility.Runway;
            else if (Name.Contains("SPH"))
                return SpaceCenterFacility.SpaceplaneHangar;
            else if (Name.Contains("Tracking"))
                return SpaceCenterFacility.TrackingStation;
            else if (Name.Contains("VAB"))
                return SpaceCenterFacility.VehicleAssemblyBuilding;
            else
                return SpaceCenterFacility.Administration;
        }

        public string GetFacilityName()
        {
            if (Name.Contains("Admin"))
                return "Administration";
            else if (Name.Contains("Astronaut"))
                return "AstronautComplex";
            else if (Name.Contains("LaunchPad"))
                return "LaunchPad";
            else if (Name.Contains("Mission"))
                return "MissionControl";
            else if (Name.Contains("R&D"))
                return "ResearchAndDevelopment";
            else if (Name.Contains("Runway"))
                return "Runway";
            else if (Name.Contains("SPH"))
                return "SpaceplaneHangar";
            else if (Name.Contains("Tracking"))
                return "TrackingStation";
            else if (Name.Contains("VAB"))
                return "VehicleAssemblyBuilding";
            else
                return "";
        }

        public int GetLevel()
        {
            int lvl = 0;
            if (Name.Contains("LVL"))
            {
                string[] split = Name.Split(' ');
                int.TryParse(split[split.Count() - 1], out lvl);
            }

            return lvl;
        }
    }*/

    public class KCT_KSC
    {
        public string KSCName;
        public List<KCT_BuildListVessel> VABList = new List<KCT_BuildListVessel>();
        public List<KCT_BuildListVessel> VABWarehouse = new List<KCT_BuildListVessel>();
        public List<KCT_BuildListVessel> SPHList = new List<KCT_BuildListVessel>();
        public List<KCT_BuildListVessel> SPHWarehouse = new List<KCT_BuildListVessel>();
        public List<KCT_UpgradingBuilding> KSCTech = new List<KCT_UpgradingBuilding>();
        //public List<KCT_TechItem> TechList = new List<KCT_TechItem>();
        public List<int> VABUpgrades = new List<int>() { 0 };
        public List<int> SPHUpgrades = new List<int>() { 0 };
        public List<int> RDUpgrades = new List<int>() { 0, 0 };
        public List<KCT_Recon_Rollout> Recon_Rollout = new List<KCT_Recon_Rollout>();

        public KCT_KSC(string name)
        {
            //KCTDebug.Log("Creating KSC with name: " + name);
            KSCName = name;
            //We propogate the tech list and upgrades throughout each KSC, since it doesn't make sense for each one to have its own tech.
            RDUpgrades[1] = KCT_GameStates.TechUpgradesTotal;
            //TechList = KCT_GameStates.ActiveKSC.TechList;
        }

        public KCT_Recon_Rollout GetReconditioning()
        {
            return Recon_Rollout.FirstOrDefault(r => ((IKCTBuildItem)r).GetItemName() == "LaunchPad Reconditioning");
        }

        public KCT_Recon_Rollout GetReconRollout(KCT_Recon_Rollout.RolloutReconType type)
        {
            return Recon_Rollout.FirstOrDefault(r => r.RRType == type);
        }


        public ConfigNode AsConfigNode()
        {
            KCTDebug.Log("Saving KSC "+KSCName);
            ConfigNode node = new ConfigNode("KSC");
            node.AddValue("KSCName", KSCName);
            
            ConfigNode vabup = new ConfigNode("VABUpgrades");
            foreach (int upgrade in VABUpgrades)
            {
                vabup.AddValue("Upgrade", upgrade.ToString());
            }
            node.AddNode(vabup);
            
            ConfigNode sphup = new ConfigNode("SPHUpgrades");
            foreach (int upgrade in SPHUpgrades)
            {
                sphup.AddValue("Upgrade", upgrade.ToString());
            }
            node.AddNode(sphup);
            
            ConfigNode rdup = new ConfigNode("RDUpgrades");
            foreach (int upgrade in RDUpgrades)
            {
                rdup.AddValue("Upgrade", upgrade.ToString());
            }
            node.AddNode(rdup);
            
            ConfigNode vabl = new ConfigNode("VABList");
            foreach (KCT_BuildListVessel blv in VABList)
            {
                KCT_BuildListStorage.BuildListItem ship = new KCT_BuildListStorage.BuildListItem();
                ship.FromBuildListVessel(blv);
                ConfigNode cnTemp = new ConfigNode("KCTVessel");
                cnTemp = ConfigNode.CreateConfigFromObject(ship, cnTemp);
                ConfigNode shipNode = new ConfigNode("ShipNode");
                blv.shipNode.CopyTo(shipNode);
                cnTemp.AddNode(shipNode);
                vabl.AddNode(cnTemp);
            }
            node.AddNode(vabl);
            
            ConfigNode sphl = new ConfigNode("SPHList");
            foreach (KCT_BuildListVessel blv in SPHList)
            {
                KCT_BuildListStorage.BuildListItem ship = new KCT_BuildListStorage.BuildListItem();
                ship.FromBuildListVessel(blv);
                ConfigNode cnTemp = new ConfigNode("KCTVessel");
                cnTemp = ConfigNode.CreateConfigFromObject(ship, cnTemp);
                ConfigNode shipNode = new ConfigNode("ShipNode");
                blv.shipNode.CopyTo(shipNode);
                cnTemp.AddNode(shipNode);
                sphl.AddNode(cnTemp);
            }
            node.AddNode(sphl);
            
            ConfigNode vabwh = new ConfigNode("VABWarehouse");
            foreach (KCT_BuildListVessel blv in VABWarehouse)
            {
                KCT_BuildListStorage.BuildListItem ship = new KCT_BuildListStorage.BuildListItem();
                ship.FromBuildListVessel(blv);
                ConfigNode cnTemp = new ConfigNode("KCTVessel");
                cnTemp = ConfigNode.CreateConfigFromObject(ship, cnTemp);
                ConfigNode shipNode = new ConfigNode("ShipNode");
                blv.shipNode.CopyTo(shipNode);
                cnTemp.AddNode(shipNode);
                vabwh.AddNode(cnTemp);
            }
            node.AddNode(vabwh);
            
            ConfigNode sphwh = new ConfigNode("SPHWarehouse");
            foreach (KCT_BuildListVessel blv in SPHWarehouse)
            {
                KCT_BuildListStorage.BuildListItem ship = new KCT_BuildListStorage.BuildListItem();
                ship.FromBuildListVessel(blv);
                ConfigNode cnTemp = new ConfigNode("KCTVessel");
                cnTemp = ConfigNode.CreateConfigFromObject(ship, cnTemp);
                ConfigNode shipNode = new ConfigNode("ShipNode");
                blv.shipNode.CopyTo(shipNode);
                cnTemp.AddNode(shipNode);
                sphwh.AddNode(cnTemp);
            }
            node.AddNode(sphwh);

            ConfigNode upgradeables = new ConfigNode("KSCTech");
            foreach (KCT_UpgradingBuilding buildingTech in KSCTech)
            {
                ConfigNode bT = new ConfigNode("UpgradingBuilding");
                bT = ConfigNode.CreateConfigFromObject(buildingTech, bT);
                upgradeables.AddNode(bT);
            }
            node.AddNode(upgradeables);

            /*ConfigNode tech = new ConfigNode("TechList");
            foreach (KCT_TechItem techItem in TechList)
            {
                KCT_TechStorageItem techNode = new KCT_TechStorageItem();
                techNode.FromTechItem(techItem);
                ConfigNode cnTemp = new ConfigNode("Tech");
                cnTemp = ConfigNode.CreateConfigFromObject(techNode, cnTemp);
                ConfigNode protoNode = new ConfigNode("ProtoNode");
                techItem.protoNode.Save(protoNode);
                cnTemp.AddNode(protoNode);
                tech.AddNode(cnTemp);
            }
            node.AddNode(tech);*/
            
            ConfigNode RRCN = new ConfigNode("Recon_Rollout");
            foreach (KCT_Recon_Rollout rr in Recon_Rollout)
            {
                ConfigNode rrCN = new ConfigNode("Recon_Rollout_Item");
                rrCN = ConfigNode.CreateConfigFromObject(rr, rrCN);
                RRCN.AddNode(rrCN);
            }
            node.AddNode(RRCN);
            return node;
        }

        public KCT_KSC FromConfigNode(ConfigNode node)
        {
            VABUpgrades.Clear();
            SPHUpgrades.Clear();
            RDUpgrades.Clear();
            VABList.Clear();
            VABWarehouse.Clear();
            SPHList.Clear();
            SPHWarehouse.Clear();
            KSCTech.Clear();
            //TechList.Clear();
            Recon_Rollout.Clear();


            this.KSCName = node.GetValue("KSCName");
            ConfigNode vabup = node.GetNode("VABUpgrades");
            foreach (string upgrade in vabup.GetValues("Upgrade"))
            {
                this.VABUpgrades.Add(int.Parse(upgrade));
            }
            ConfigNode sphup = node.GetNode("SPHUpgrades");
            foreach (string upgrade in sphup.GetValues("Upgrade"))
            {
                this.SPHUpgrades.Add(int.Parse(upgrade));
            }
            ConfigNode rdup = node.GetNode("RDUpgrades");
            foreach (string upgrade in rdup.GetValues("Upgrade"))
            {
                this.RDUpgrades.Add(int.Parse(upgrade));
            }

            ConfigNode tmp = node.GetNode("VABList");
            foreach (ConfigNode vessel in tmp.GetNodes("KCTVessel"))
            {
                KCT_BuildListStorage.BuildListItem listItem = new KCT_BuildListStorage.BuildListItem();
                ConfigNode.LoadObjectFromConfig(listItem, vessel);
                KCT_BuildListVessel blv = listItem.ToBuildListVessel();
                blv.shipNode = vessel.GetNode("ShipNode");
                this.VABList.Add(blv);
            }

            tmp = node.GetNode("SPHList");
            foreach (ConfigNode vessel in tmp.GetNodes("KCTVessel"))
            {
                KCT_BuildListStorage.BuildListItem listItem = new KCT_BuildListStorage.BuildListItem();
                ConfigNode.LoadObjectFromConfig(listItem, vessel);
                KCT_BuildListVessel blv = listItem.ToBuildListVessel();
                blv.shipNode = vessel.GetNode("ShipNode");
                this.SPHList.Add(blv);
            }

            tmp = node.GetNode("VABWarehouse");
            foreach (ConfigNode vessel in tmp.GetNodes("KCTVessel"))
            {
                KCT_BuildListStorage.BuildListItem listItem = new KCT_BuildListStorage.BuildListItem();
                ConfigNode.LoadObjectFromConfig(listItem, vessel);
                KCT_BuildListVessel blv = listItem.ToBuildListVessel();
                blv.shipNode = vessel.GetNode("ShipNode");
                this.VABWarehouse.Add(blv);
            }

            tmp = node.GetNode("SPHWarehouse");
            foreach (ConfigNode vessel in tmp.GetNodes("KCTVessel"))
            {
                KCT_BuildListStorage.BuildListItem listItem = new KCT_BuildListStorage.BuildListItem();
                ConfigNode.LoadObjectFromConfig(listItem, vessel);
                KCT_BuildListVessel blv = listItem.ToBuildListVessel();
                blv.shipNode = vessel.GetNode("ShipNode");
                this.SPHWarehouse.Add(blv);
            }

           /* tmp = node.GetNode("TechList");
            foreach (ConfigNode techNode in tmp.GetNodes("Tech"))
            {
                KCT_TechStorageItem techStorageItem = new KCT_TechStorageItem();
                ConfigNode.LoadObjectFromConfig(techStorageItem, techNode);
                KCT_TechItem techItem = techStorageItem.ToTechItem();
                techItem.protoNode = new ProtoTechNode(techNode.GetNode("ProtoNode"));
                this.TechList.Add(techItem);
            }*/

            tmp = node.GetNode("Recon_Rollout");
            foreach (ConfigNode RRCN in tmp.GetNodes("Recon_Rollout_Item"))
            {
                KCT_Recon_Rollout tempRR = new KCT_Recon_Rollout();
                ConfigNode.LoadObjectFromConfig(tempRR, RRCN);
                Recon_Rollout.Add(tempRR);
            }

            if (node.HasNode("KSCTech"))
            {
                tmp = node.GetNode("KSCTech");
                foreach (ConfigNode upBuild in tmp.GetNodes("UpgradingBuilding"))
                {
                    KCT_UpgradingBuilding tempUP = new KCT_UpgradingBuilding();
                    ConfigNode.LoadObjectFromConfig(tempUP, upBuild);
                    KSCTech.Add(tempUP);
                }
            }

            return this;
        }
    }
}
