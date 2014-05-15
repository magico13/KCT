using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Construction_Time
{
    public class KCT_BuildListStorage:ConfigNodeStorage
    {
        [Persistent]
        List<BuildListItem> VABBuildList = new List<BuildListItem>();
        [Persistent]
        List<BuildListItem> SPHBuildList = new List<BuildListItem>();
        [Persistent]
        List<BuildListItem> VABWarehouse = new List<BuildListItem>();
        [Persistent]
        List<BuildListItem> SPHWarehouse = new List<BuildListItem>();


        public override void OnDecodeFromConfigNode()
        {
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                KCT_GameStates.VABList.Clear();
                KCT_GameStates.SPHList.Clear();
                KCT_GameStates.VABWarehouse.Clear();
                KCT_GameStates.SPHWarehouse.Clear();
            }
            foreach (BuildListItem b in VABBuildList)
            {
                KCT_BuildListVessel blv = b.ToBuildListVessel();
                if (ListContains(blv, KCT_GameStates.VABList) < 0)
                    KCT_GameStates.VABList.Add(blv);
            }
            foreach (BuildListItem b in SPHBuildList)
            {
                KCT_BuildListVessel blv = b.ToBuildListVessel();
                if (ListContains(blv, KCT_GameStates.SPHList) < 0)
                    KCT_GameStates.SPHList.Add(blv);
            }
            foreach (BuildListItem b in VABWarehouse)
            {
                KCT_BuildListVessel blv = b.ToBuildListVessel();
                if (ListContains(blv, KCT_GameStates.VABWarehouse) < 0)
                    KCT_GameStates.VABWarehouse.Add(blv);
            }
            foreach (BuildListItem b in SPHWarehouse)
            {
                KCT_BuildListVessel blv = b.ToBuildListVessel();
                if (ListContains(blv, KCT_GameStates.SPHWarehouse) < 0)
                    KCT_GameStates.SPHWarehouse.Add(blv);
            }
        }

        public override void OnEncodeToConfigNode()
        {
            VABBuildList.Clear();
            SPHBuildList.Clear();
            VABWarehouse.Clear();
            SPHWarehouse.Clear();
            foreach (KCT_BuildListVessel b in KCT_GameStates.VABList)
            {
                BuildListItem bls = new BuildListItem();
                bls.FromBuildListVessel(b);
                VABBuildList.Add(bls);
            }
            foreach (KCT_BuildListVessel b in KCT_GameStates.SPHList)
            {
                BuildListItem bls = new BuildListItem();
                bls.FromBuildListVessel(b);
                SPHBuildList.Add(bls);
            }
            foreach (KCT_BuildListVessel b in KCT_GameStates.VABWarehouse)
            {
                BuildListItem bls = new BuildListItem();
                bls.FromBuildListVessel(b);
                VABWarehouse.Add(bls);
            }
            foreach (KCT_BuildListVessel b in KCT_GameStates.SPHWarehouse)
            {
                BuildListItem bls = new BuildListItem();
                bls.FromBuildListVessel(b);
                SPHWarehouse.Add(bls);
            }

        }

        private int ListContains(KCT_BuildListVessel blv, List<KCT_BuildListVessel> list)
        {
            for (int i=0; i<list.Count; i++)
            {
                if (blv.id == list[i].id)
                {
                  //  Debug.Log("[KCT] Ship found");
                    return i;
                }
            }
           // Debug.Log("[KCT] Ship not found");
            return -1;
        }

        public class BuildListItem
        {
            [Persistent]
            string shipName, shipID;
            [Persistent]
            double progress, buildTime;
            [Persistent]
            String launchSite, flag;
            [Persistent]
            List<string> InventoryParts;


            public KCT_BuildListVessel ToBuildListVessel()
            {
                KCT_BuildListVessel ret = new KCT_BuildListVessel(shipName, launchSite, buildTime, flag);
                ret.progress = progress;
                if (InventoryParts != null)
                    ret.InventoryParts = InventoryParts;
                ret.id = new Guid(shipID);
                return ret;
            }

            public BuildListItem FromBuildListVessel(KCT_BuildListVessel blv)
            {
                this.progress = blv.progress;
                this.buildTime = blv.buildPoints;
                this.launchSite = blv.launchSite;
                this.flag = blv.flag;
                //this.shipURL = blv.shipURL;
                this.shipName = blv.shipName;
                this.InventoryParts = blv.InventoryParts;
                this.shipID = blv.id.ToString();
                
                return this;

            }
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