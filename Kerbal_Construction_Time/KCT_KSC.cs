using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerbal_Construction_Time
{
    public class KCT_KSC
    {
        public string KSCName;
        public List<KCT_BuildListVessel> VABList = new List<KCT_BuildListVessel>();
        public List<KCT_BuildListVessel> VABWarehouse = new List<KCT_BuildListVessel>();
        public List<KCT_BuildListVessel> SPHList = new List<KCT_BuildListVessel>();
        public List<KCT_BuildListVessel> SPHWarehouse = new List<KCT_BuildListVessel>();
        //public List<KCT_TechItem> TechList = new List<KCT_TechItem>();
        public List<int> VABUpgrades = new List<int>() { 0 };
        public List<int> SPHUpgrades = new List<int>() { 0 };
        public List<int> RDUpgrades = new List<int>() { 0, 0 };
        public KCT_Reconditioning LaunchPadReconditioning;

        public KCT_KSC(string name)
        {
            KSCName = name;
            //We propogate the tech list and upgrades throughout each KSC, since it doesn't make sense for each one to have its own tech.
            RDUpgrades[1] = KCT_GameStates.TechUpgradesTotal;
            //TechList = KCT_GameStates.ActiveKSC.TechList;
        }

        public ConfigNode AsConfigNode()
        {
            ConfigNode node = new ConfigNode("KSC");
            node.SetValue("KSCName", KSCName);

            ConfigNode vabup = new ConfigNode("VABUpgrades");
            foreach (int upgrade in VABUpgrades)
            {
                vabup.SetValue("Upgrade", upgrade.ToString());
            }
            node.AddNode(vabup);

            ConfigNode sphup = new ConfigNode("SPHUpgrades");
            foreach (int upgrade in SPHUpgrades)
            {
                vabup.SetValue("Upgrade", upgrade.ToString());
            }
            node.AddNode(sphup);

            ConfigNode rdup = new ConfigNode("RDUpgrades");
            foreach (int upgrade in RDUpgrades)
            {
                vabup.SetValue("Upgrade", upgrade.ToString());
            }
            node.AddNode(rdup);

            ConfigNode vabl = new ConfigNode("VABList");
            foreach (KCT_BuildListVessel blv in VABList)
            {
                KCT_BuildListStorage.BuildListItem ship = new KCT_BuildListStorage.BuildListItem();
                ship.FromBuildListVessel(blv);
                ConfigNode cnTemp = new ConfigNode("Vessel");
                cnTemp = ConfigNode.CreateConfigFromObject(ship, cnTemp);
                cnTemp.AddNode(blv.shipNode);
                vabl.AddNode(cnTemp);
            }
            node.AddNode(vabl);

            ConfigNode sphl = new ConfigNode("SPHList");
            foreach (KCT_BuildListVessel blv in SPHList)
            {
                KCT_BuildListStorage.BuildListItem ship = new KCT_BuildListStorage.BuildListItem();
                ship.FromBuildListVessel(blv);
                ConfigNode cnTemp = new ConfigNode("Vessel");
                cnTemp = ConfigNode.CreateConfigFromObject(ship, cnTemp);
                cnTemp.AddNode(blv.shipNode);
                sphl.AddNode(cnTemp);
            }
            node.AddNode(sphl);

            ConfigNode vabwh = new ConfigNode("VABWarehouse");
            foreach (KCT_BuildListVessel blv in VABWarehouse)
            {
                KCT_BuildListStorage.BuildListItem ship = new KCT_BuildListStorage.BuildListItem();
                ship.FromBuildListVessel(blv);
                ConfigNode cnTemp = new ConfigNode("Vessel");
                cnTemp = ConfigNode.CreateConfigFromObject(ship, cnTemp);
                cnTemp.AddNode(blv.shipNode);
                vabwh.AddNode(cnTemp);
            }
            node.AddNode(vabwh);

            ConfigNode sphwh = new ConfigNode("SPHWarehouse");
            foreach (KCT_BuildListVessel blv in SPHWarehouse)
            {
                KCT_BuildListStorage.BuildListItem ship = new KCT_BuildListStorage.BuildListItem();
                ship.FromBuildListVessel(blv);
                ConfigNode cnTemp = new ConfigNode("Vessel");
                cnTemp = ConfigNode.CreateConfigFromObject(ship, cnTemp);
                cnTemp.AddNode(blv.shipNode);
                sphwh.AddNode(cnTemp);
            }
            node.AddNode(sphwh);

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

            if (LaunchPadReconditioning != null)
            {
                ConfigNode cnTemp = new ConfigNode("Reconditioning");
                cnTemp = ConfigNode.CreateConfigFromObject(LaunchPadReconditioning, cnTemp);
                node.AddNode(cnTemp);
            }

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
            //TechList.Clear();
            LaunchPadReconditioning = null;


            this.KSCName = node.GetValue("KSCName");
            ConfigNode vabup = node.GetNode("VABUpgrades");
            foreach (string upgrade in vabup.GetValues("Upgrade"))
            {
                this.VABUpgrades.Add(int.Parse(upgrade));
            }
            ConfigNode sphup = node.GetNode("SPHUpgrades");
            foreach (string upgrade in vabup.GetValues("Upgrade"))
            {
                this.SPHUpgrades.Add(int.Parse(upgrade));
            }
            ConfigNode rdup = node.GetNode("RDUpgrades");
            foreach (string upgrade in vabup.GetValues("Upgrade"))
            {
                this.RDUpgrades.Add(int.Parse(upgrade));
            }

            ConfigNode tmp = node.GetNode("VABList");
            foreach (ConfigNode vessel in tmp.GetNodes("Vessel"))
            {
                KCT_BuildListStorage.BuildListItem listItem = new KCT_BuildListStorage.BuildListItem();
                ConfigNode.LoadObjectFromConfig(listItem, vessel);
                KCT_BuildListVessel blv = listItem.ToBuildListVessel();
                blv.shipNode = vessel.GetNode("Vessel"); //TODO: Find out what the hell this is saved as.
                this.VABList.Add(blv);
            }

            tmp = node.GetNode("SPHList");
            foreach (ConfigNode vessel in tmp.GetNodes("Vessel"))
            {
                KCT_BuildListStorage.BuildListItem listItem = new KCT_BuildListStorage.BuildListItem();
                ConfigNode.LoadObjectFromConfig(listItem, vessel);
                KCT_BuildListVessel blv = listItem.ToBuildListVessel();
                blv.shipNode = vessel.GetNode("Vessel"); //TODO: Find out what the hell this is saved as.
                this.SPHList.Add(blv);
            }

            tmp = node.GetNode("VABWarehouse");
            foreach (ConfigNode vessel in tmp.GetNodes("Vessel"))
            {
                KCT_BuildListStorage.BuildListItem listItem = new KCT_BuildListStorage.BuildListItem();
                ConfigNode.LoadObjectFromConfig(listItem, vessel);
                KCT_BuildListVessel blv = listItem.ToBuildListVessel();
                blv.shipNode = vessel.GetNode("Vessel"); //TODO: Find out what the hell this is saved as.
                this.VABWarehouse.Add(blv);
            }

            tmp = node.GetNode("SPHWarehouse");
            foreach (ConfigNode vessel in tmp.GetNodes("Vessel"))
            {
                KCT_BuildListStorage.BuildListItem listItem = new KCT_BuildListStorage.BuildListItem();
                ConfigNode.LoadObjectFromConfig(listItem, vessel);
                KCT_BuildListVessel blv = listItem.ToBuildListVessel();
                blv.shipNode = vessel.GetNode("Vessel"); //TODO: Find out what the hell this is saved as.
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

            tmp = node.GetNode("Reconditioning");
            if (tmp != null)
            {
                ConfigNode.LoadObjectFromConfig(this.LaunchPadReconditioning, tmp);
            }

            return this;
        }
    }
}
