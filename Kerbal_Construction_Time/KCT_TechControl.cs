using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    public class KCT_TechItem : IKCTBuildItem
    {
        public int scienceCost;
        public string techName, techID;
        public double progress;
        public ProtoTechNode protoNode;
        public List<string> UnlockedParts;
       // public double BuildRate { get { return (Math.Pow(2, KCT_GameStates.TechUpgradesTotal + 1) / (86400.0 * KCT_GameStates.timeSettings.NodeModifier)); } } //0pts=1day/2sci, 1pt=1/4, 2=1/8, 3=1/16, 4=1/32...n=1/2^(n+1)
        private double bRate_int = -1;
        public double BuildRate
        {
            get
            {
                if (bRate_int < 0)
                {
                    UpdateBuildRate(Math.Max(KCT_GameStates.TechList.IndexOf(this), 0));
                }
                return bRate_int;
            }
        }
        public double TimeLeft { get { return (scienceCost - progress) / BuildRate; } }
        public bool isComplete { get { return progress >= scienceCost; } }

        public double EstimatedTimeLeft
        {
            get
            {
                if (BuildRate > 0)
                {
                    return TimeLeft;
                }
                else
                {
                    double rate = KCT_MathParsing.ParseNodeRateFormula(scienceCost, 0);
                    return (scienceCost - progress) / rate;
                }
            }
        }

        public KCT_TechItem(RDTech techNode)
        {
            scienceCost = techNode.scienceCost;
            techName = techNode.title;
            techID = techNode.techID;
            progress = 0;
            protoNode = ResearchAndDevelopment.Instance.GetTechState(techID);
            UnlockedParts = new List<string>();
            foreach (AvailablePart p in techNode.partsPurchased)
                UnlockedParts.Add(p.name);

            KCTDebug.Log("techID = " + techID);
            //KCTDebug.Log("BuildRate = " + BuildRate);
            KCTDebug.Log("TimeLeft = " + TimeLeft);
        }
        
        public KCT_TechItem(string ID, string name, double prog, int sci, List<string> parts)
        {
            techID = ID;
            techName = name;
            progress = prog;
            scienceCost = sci;
            UnlockedParts = parts;
        }

        public KCT_TechItem() {}

        public double UpdateBuildRate(int index)
        {
          //  double max = double.Parse(KCT_GameStates.formulaSettings.NodeMax);
            double rate = KCT_MathParsing.ParseNodeRateFormula(scienceCost, index);
            //KCT_MathParsing.GetStandardFormulaValue("Node",
              //  new Dictionary<string, string>() { { "N", KCT_GameStates.TechUpgradesTotal.ToString() }, { "S", scienceCost.ToString() }, {"R", KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.ResearchAndDevelopment).ToString() } });
          //  if (max > 0 && rate > max) rate = max;
            if (rate < 0)
                rate = 0;
            bRate_int = rate;
            return bRate_int;
        }

        public void DisableTech()
        {
            protoNode.state = RDTech.State.Unavailable;
            ResearchAndDevelopment.Instance.SetTechState(techID, protoNode);
        }

        public void EnableTech()
        {
            protoNode.state = RDTech.State.Available;
            ResearchAndDevelopment.Instance.SetTechState(techID, protoNode);
        }

        public bool isInList()
        {
            return KCT_GameStates.TechList.FirstOrDefault(t => t.techID == this.techID) != null;
           /* foreach (KCT_TechItem tech in KCT_GameStates.TechList)
            {
                if (tech.techID == this.techID)
                    return true;
            }
            return false;*/
        }

        string IKCTBuildItem.GetItemName()
        {
            return this.techName;
        }

        double IKCTBuildItem.GetBuildRate()
        {
            return this.BuildRate;
        }

        double IKCTBuildItem.GetTimeLeft()
        {
            return this.TimeLeft;
        }

        KCT_BuildListVessel.ListType IKCTBuildItem.GetListType()
        {
            return KCT_BuildListVessel.ListType.TechNode;
        }

        bool IKCTBuildItem.IsComplete()
        {
            return (this.isComplete);
        }

    }

    public class KCT_TechStorageItem
    {
        [Persistent] string techName, techID;
        [Persistent] int scienceCost;
        [Persistent] double progress;
        [Persistent] List<string> parts;
        public KCT_TechItem ToTechItem()
        {
            KCT_TechItem ret = new KCT_TechItem(techID, techName, progress, scienceCost, parts);
            return ret;
        }
        public KCT_TechStorageItem FromTechItem(KCT_TechItem techItem)
        {
            this.techName = techItem.techName;
            this.techID = techItem.techID;
            this.progress = techItem.progress;
            this.scienceCost = techItem.scienceCost;
            this.parts = techItem.UnlockedParts;

            return this;
        }
    }

    public class KCT_TechStorage : ConfigNodeStorage
    {
        [Persistent] List<KCT_TechStorageItem> techBuildList = new List<KCT_TechStorageItem>();
       // [Persistent] ConfigNode techNode = new ConfigNode();
        public override void OnEncodeToConfigNode()
        {
            base.OnEncodeToConfigNode();
            techBuildList.Clear();
            foreach (KCT_TechItem tech in KCT_GameStates.TechList)
            {
                KCT_TechStorageItem tSI = new KCT_TechStorageItem();
                techBuildList.Add(tSI.FromTechItem(tech));
            }
        }

        public override void OnDecodeFromConfigNode()
        {
            base.OnDecodeFromConfigNode();
            KCT_GameStates.TechList.Clear();
            foreach (KCT_TechStorageItem tSI in this.techBuildList)
            {
                KCT_TechItem tI = tSI.ToTechItem();
                KCT_GameStates.TechList.Add(tI);
            }
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