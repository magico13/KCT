using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using KSP.UI.Screens;

namespace KerbalConstructionTime
{
    class KCT_Events
    {
        public static KCT_Events instance = new KCT_Events();
        public bool eventAdded;

        public KCT_Events()
        {
            eventAdded = false;
        }

        public void addEvents()
        {
            GameEvents.onGUILaunchScreenSpawn.Add(launchScreenOpenEvent);
            GameEvents.onVesselRecovered.Add(vesselRecoverEvent);

            //GameEvents.onLaunch.Add(vesselSituationChange);
            GameEvents.onVesselSituationChange.Add(vesselSituationChange);
            GameEvents.onGameSceneLoadRequested.Add(gameSceneEvent);
            GameEvents.OnTechnologyResearched.Add(TechUnlockEvent);
            //if (!ToolbarManager.ToolbarAvailable || !KCT_GameStates.settings.PreferBlizzyToolbar)
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onEditorShipModified.Add(ShipModifiedEvent);
            GameEvents.OnPartPurchased.Add(PartPurchasedEvent);
            //GameEvents.OnVesselRecoveryRequested.Add(RecoveryRequested);
            GameEvents.onGUIRnDComplexSpawn.Add(TechEnableEvent);
            GameEvents.onGUIRnDComplexDespawn.Add(TechDisableEvent);
            GameEvents.OnKSCFacilityUpgraded.Add(FacilityUpgradedEvent);
            GameEvents.onGameStateLoad.Add(PersistenceLoadEvent);

            GameEvents.OnKSCStructureRepaired.Add(FaciliyRepaired);
            GameEvents.OnKSCStructureCollapsed.Add(FacilityDestroyed);

            GameEvents.FindEvent<EventVoid>("OnSYInventoryAppliedToVessel")?.Add(SYInventoryApplied);
            GameEvents.FindEvent<EventVoid>("OnSYReady")?.Add(SYReady);
            GameEvents.FindEvent<EventData<Part>>("OnSYInventoryAppliedToPart")?.Add((p) => { KerbalConstructionTime.instance.editorRecalcuationRequired = true; });
            //     GameEvents.OnKSCStructureRepairing.Add(FacilityRepairingEvent);
            //  GameEvents.onLevelWasLoaded.Add(LevelLoadedEvent);

            /*  GameEvents.OnCrewmemberHired.Add((ProtoCrewMember m, int i) =>
              {
                  foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                  {
                      ksc.RecalculateBuildRates();
                      ksc.RecalculateUpgradedBuildRates();
                  }
              });
              GameEvents.OnCrewmemberSacked.Add((ProtoCrewMember m, int i) =>
              {
                  foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                  {
                      ksc.RecalculateBuildRates();
                      ksc.RecalculateUpgradedBuildRates();
                  }
              });*/

            GameEvents.onGUIAdministrationFacilitySpawn.Add(HideAllGUIs);
            GameEvents.onGUIAstronautComplexSpawn.Add(HideAllGUIs);
            GameEvents.onGUIMissionControlSpawn.Add(HideAllGUIs);
            GameEvents.onGUIRnDComplexSpawn.Add(HideAllGUIs);
            GameEvents.onGUIKSPediaSpawn.Add(HideAllGUIs);
            GameEvents.onEditorStarted.Add(() => { KCT_Utilities.HandleEditorButton(); });

            GameEvents.onFacilityContextMenuSpawn.Add(FacilityContextMenuSpawn);

            eventAdded = true;
        }

        public void HideAllGUIs()
        {
            //KCT_GUI.hideAll();
            KCT_GUI.ClickOff();
        }

        public void PersistenceLoadEvent(ConfigNode node)
        {
            //KCT_GameStates.erroredDuringOnLoad.OnLoadStart();
            KCTDebug.Log("Looking for tech nodes.");
            ConfigNode rnd = node.GetNodes("SCENARIO").FirstOrDefault(n => n.GetValue("name") == "ResearchAndDevelopment");
            if (rnd != null)
            {
                KCT_GameStates.LastKnownTechCount = rnd.GetNodes("Tech").Length;
                KCTDebug.Log("Counting " + KCT_GameStates.LastKnownTechCount + " tech nodes.");
            }
            KCT_GameStates.PersistenceLoaded = true;
        }

        //private static int lastLvl = -1;
        public static bool allowedToUpgrade = false;
        public void FacilityUpgradedEvent(Upgradeables.UpgradeableFacility facility, int lvl)
        {
            if (KCT_GUI.PrimarilyDisabled)
            {
                bool isLaunchpad = facility.id.ToLower().Contains("launchpad");
                if (!isLaunchpad)
                    return;

                //is a launch pad
                KCT_GameStates.ActiveKSC.ActiveLPInstance.Upgrade(lvl);

            }


            //if (!(allowedToUpgrade || !KCT_PresetManager.Instance.ActivePreset.generalSettings.KSCUpgradeTimes))
            //{
            //    KCT_UpgradingBuilding upgrading = new KCT_UpgradingBuilding(facility.id, lvl, lvl - 1, facility.id.Split('/').Last());

            //    upgrading.isLaunchpad = facility.id.ToLower().Contains("launchpad");
            //    if (upgrading.isLaunchpad)
            //    {
            //        upgrading.launchpadID = KCT_GameStates.ActiveKSC.ActiveLaunchPadID;
            //        if (upgrading.launchpadID > 0)
            //            upgrading.commonName += KCT_GameStates.ActiveKSC.ActiveLPInstance.name;//" " + (upgrading.launchpadID+1);
            //    }

            //    if (!upgrading.AlreadyInProgress())
            //    {
            //        KCT_GameStates.ActiveKSC.KSCTech.Add(upgrading);
            //        upgrading.Downgrade();
            //        double cost = facility.GetUpgradeCost();
            //        upgrading.SetBP(cost);
            //        upgrading.cost = cost;

            //        ScreenMessages.PostScreenMessage("Facility upgrade requested!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
            //        KCTDebug.Log("Facility " + facility.id + " upgrade requested to lvl " + lvl + " for " + cost + " funds, resulting in a BP of " + upgrading.BP);
            //    }
            //    else if (lvl != upgrading.currentLevel)
            //    {
            //        //
            //        KCT_UpgradingBuilding listBuilding = upgrading.KSC.KSCTech.Find(b => b.id == upgrading.id);
            //        if (upgrading.isLaunchpad)
            //            listBuilding = upgrading.KSC.KSCTech.Find(b => b.isLaunchpad && b.launchpadID == upgrading.launchpadID);
            //        listBuilding.Downgrade();
            //        KCT_Utilities.AddFunds(listBuilding.cost, TransactionReasons.None);
            //        ScreenMessages.PostScreenMessage("Facility is already being upgraded!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
            //        KCTDebug.Log("Facility " + facility.id + " tried to upgrade to lvl " + lvl + " but already in list!");
            //    }
            //}
            //else
            //{
                KCTDebug.Log("Facility " + facility.id + " upgraded to lvl " + lvl);
                if (facility.id.ToLower().Contains("launchpad"))
                {
                    if (!allowedToUpgrade)
                        KCT_GameStates.ActiveKSC.ActiveLPInstance.Upgrade(lvl); //also repairs the launchpad
                    else
                        KCT_GameStates.ActiveKSC.ActiveLPInstance.level = lvl;
                }
                allowedToUpgrade = false;
                foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                {
                    ksc.RecalculateBuildRates();
                    ksc.RecalculateUpgradedBuildRates();
                }
                foreach (KCT_TechItem tech in KCT_GameStates.TechList)
                {
                    tech.UpdateBuildRate(KCT_GameStates.TechList.IndexOf(tech));
                }
            //}
           /* if (lvl <= lastLvl)
            {
                lastLvl = -1;
                return;
            }
            facility.SetLevel(lvl - 1);
            lastLvl = lvl;
            double cost = facility.GetUpgradeCost();
            double BP = Math.Sqrt(cost) * 2000 * KCT_GameStates.timeSettings.OverallMultiplier;*/

           // KCTDebug.Log(facility.GetNormLevel());

        }

        public void FacilityRepairingEvent(DestructibleBuilding facility)
        {
            if (KCT_GUI.PrimarilyDisabled)
                return;
            double cost = facility.RepairCost;
            double BP = Math.Sqrt(cost) * 2000 * KCT_PresetManager.Instance.ActivePreset.timeSettings.OverallMultiplier;
            KCTDebug.Log("Facility being repaired for " + cost + " funds, resulting in a BP of " + BP);
            //facility.StopCoroutine("Repair");
        }

        public void FaciliyRepaired(DestructibleBuilding facility)
        {
            if (facility.id.Contains("LaunchPad"))
            {
                KCTDebug.Log("LaunchPad was repaired.");
                //KCT_GameStates.ActiveKSC.LaunchPads[KCT_GameStates.ActiveKSC.ActiveLaunchPadID].destroyed = false;
                KCT_GameStates.ActiveKSC.ActiveLPInstance.RefreshDestructionNode();
                KCT_GameStates.ActiveKSC.ActiveLPInstance.CompletelyRepairNode();
            }
        }

        public void FacilityDestroyed(DestructibleBuilding facility)
        {
            if (facility.id.Contains("LaunchPad"))
            {
                KCTDebug.Log("LaunchPad was damaged.");
                //KCT_GameStates.ActiveKSC.LaunchPads[KCT_GameStates.ActiveKSC.ActiveLaunchPadID].destroyed = !KCT_Utilities.LaunchFacilityIntact(KCT_BuildListVessel.ListType.VAB);
                KCT_GameStates.ActiveKSC.ActiveLPInstance.RefreshDestructionNode();
            }
        }

        public void RecoveryRequested (Vessel v)
        {
            //ShipBackup backup = ShipAssembly.MakeVesselBackup(v);
            //string tempFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/Ships/temp2.craft";
            //backup.SaveShip(tempFile);

           // KCT_GameStates.recoveryRequestVessel = backup; //ConfigNode.Load(tempFile);
        }

        public void FacilityContextMenuSpawn(KSCFacilityContextMenu menu)
        {
            KerbalConstructionTime.instance.FacilityContextMenuSpawn(menu);
        }

        private void SYInventoryApplied()
        {
            KCTDebug.Log("Inventory was applied. Recalculating.");
            if (HighLogic.LoadedSceneIsEditor)
            {
                KerbalConstructionTime.instance.editorRecalcuationRequired = true;
            }
        }

        private void SYReady()
        {
            if (HighLogic.LoadedSceneIsEditor && KCT_GameStates.EditorShipEditingMode && KCT_GameStates.editedVessel != null)
            {
                KCTDebug.Log("Removing SY tracking of this vessel.");
                string id = ScrapYardWrapper.GetPartID(KCT_GameStates.editedVessel.ExtractedPartNodes[0]);
                ScrapYardWrapper.SetProcessedStatus(id, false);

                KCTDebug.Log("Adding parts back to inventory for editing...");
                foreach (ConfigNode partNode in KCT_GameStates.editedVessel.ExtractedPartNodes)
                {
                    if (ScrapYardWrapper.PartIsFromInventory(partNode))
                    {
                        ScrapYardWrapper.AddPartToInventory(partNode, false);
                    }
                }
            }
        }

        private void ShipModifiedEvent(ShipConstruct vessel)
        {
            KerbalConstructionTime.instance.editorRecalcuationRequired = true;
        }

        public ApplicationLauncherButton KCTButtonStock = null;
        public void OnGUIAppLauncherReady()
        {
            bool vis;
            if (ToolbarManager.ToolbarAvailable && KCT_GameStates.settings.PreferBlizzyToolbar)
                return;

            if (ApplicationLauncher.Ready && (KCTButtonStock == null || !ApplicationLauncher.Instance.Contains(KCTButtonStock, out vis))) //Add Stock button
            {
                string texturePath = "KerbalConstructionTime/Icons/KCT_on";
                KCT_Events.instance.KCTButtonStock = ApplicationLauncher.Instance.AddModApplication(
                    KCT_GUI.ClickOn,
                    KCT_GUI.ClickOff,
                    KCT_GUI.onHoverOn,
                    KCT_GUI.onHoverOff,
                    KCT_Events.instance.DummyVoid,
                    KCT_Events.instance.DummyVoid,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB,
                    GameDatabase.Instance.GetTexture(texturePath, false));

                ApplicationLauncher.Instance.EnableMutuallyExclusive(KCT_Events.instance.KCTButtonStock);

              /*  if (HighLogic.LoadedScene == GameScenes.SPACECENTER && KCT_GameStates.showWindows[0])
                {
                    KCTButtonStock.SetTrue(true);
                    KCT_GUI.clicked = true;
                }*/
            }
        }
        public void DummyVoid() { }

        public void PartPurchasedEvent(AvailablePart part)
        {
            if (HighLogic.CurrentGame.Parameters.Difficulty.BypassEntryPurchaseAfterResearch)
                return;
            KCT_TechItem tech = KCT_GameStates.TechList.Find(t => t.techID == part.TechRequired);
            if (tech!= null && tech.isInList())
            {
                ScreenMessages.PostScreenMessage("[KCT] You must wait until the node is fully researched to purchase parts!", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                KCT_Utilities.AddFunds(part.entryCost, TransactionReasons.RnDPartPurchase);
                tech.protoNode.partsPurchased.Remove(part);
                tech.DisableTech();
            }
        }

        public void TechUnlockEvent(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> ev)
        {
            //TODO: Check if any of the parts are experimental, if so, do the normal KCT stuff and then set them experimental again
            if (!KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled) return;
            if (ev.target == RDTech.OperationResult.Successful)
            {
                KCT_TechItem tech = new KCT_TechItem();
                if (ev.host != null)
                    tech = new KCT_TechItem(ev.host);

                foreach (AvailablePart expt in ev.host.partsPurchased)
                {
                    if (ResearchAndDevelopment.IsExperimentalPart(expt))
                        KCT_GameStates.ExperimentalParts.Add(expt);
                }

                //if (!KCT_GameStates.settings.InstantTechUnlock && !KCT_GameStates.settings.DisableBuildTime) tech.DisableTech();
                if (!tech.isInList())
                {
                    if (KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUpgrades)
                        ScreenMessages.PostScreenMessage("[KCT] Upgrade Point Added!", 4.0f, ScreenMessageStyle.UPPER_LEFT);

                    if (KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUnlockTimes && KCT_PresetManager.Instance.ActivePreset.generalSettings.BuildTimes)
                    {
                        KCT_GameStates.TechList.Add(tech);
                        foreach (KCT_TechItem techItem in KCT_GameStates.TechList)
                            techItem.UpdateBuildRate(KCT_GameStates.TechList.IndexOf(techItem));
                        double timeLeft = tech.BuildRate > 0 ? tech.TimeLeft : tech.EstimatedTimeLeft;
                        ScreenMessages.PostScreenMessage("[KCT] Node will unlock in " + MagiCore.Utilities.GetFormattedTime(timeLeft), 4.0f, ScreenMessageStyle.UPPER_LEFT);
                    }
                }
                else
                {
                    ResearchAndDevelopment.Instance.AddScience(tech.scienceCost, TransactionReasons.RnDTechResearch);
                    ScreenMessages.PostScreenMessage("[KCT] This node is already being researched!", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                    ScreenMessages.PostScreenMessage("[KCT] It will unlock in " + MagiCore.Utilities.GetFormattedTime((KCT_GameStates.TechList.First(t => t.techID == ev.host.techID)).TimeLeft), 4.0f, ScreenMessageStyle.UPPER_LEFT);
                }
            }
        }

        public void TechDisableEvent()
        {
            TechDisableEventFinal(true);
        }

        public void TechEnableEvent()
        {
            if (KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUnlockTimes && KCT_PresetManager.Instance.ActivePreset.generalSettings.BuildTimes)
            {
                foreach (KCT_TechItem techItem in KCT_GameStates.TechList)
                    techItem.EnableTech();
            }
        }

        public void TechDisableEventFinal(bool save=false)
        {
            if (KCT_PresetManager.Instance != null && KCT_PresetManager.Instance.ActivePreset != null)
            {
                if (KCT_PresetManager.Instance.ActivePreset.generalSettings.TechUnlockTimes && KCT_PresetManager.Instance.ActivePreset.generalSettings.BuildTimes)
                {
                    foreach (KCT_TechItem tech in KCT_GameStates.TechList)
                    {
                       /* foreach (String partName in tech.UnlockedParts)
                        {
                            AvailablePart expt = KCT_Utilities.GetAvailablePartByName(partName);
                            if (expt != null && ResearchAndDevelopment.IsExperimentalPart(expt))
                                if (!KCT_GameStates.ExperimentalParts.Contains(expt))
                                    KCT_GameStates.ExperimentalParts.Add(expt);
                        }*/
                        //ResearchAndDevelopment.AddExperimentalPart()
                        tech.DisableTech();
                    }
                /*    foreach (AvailablePart expt in KCT_GameStates.ExperimentalParts)
                        ResearchAndDevelopment.AddExperimentalPart(expt);*/
                    //Need to somehow update the R&D instance
                    if (save)
                    {
                        GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                    }
                }
            }
        }

        public void gameSceneEvent(GameScenes scene)
        {
            if (scene == GameScenes.MAINMENU)
            {
                KCT_GameStates.reset();
                KCT_GameStates.firstStart = false;
                InputLockManager.RemoveControlLock("KCTLaunchLock");
                KCT_GameStates.activeKSCName = "Stock";
                KCT_GameStates.ActiveKSC = new KCT_KSC("Stock");
                KCT_GameStates.KSCs = new List<KCT_KSC>() { KCT_GameStates.ActiveKSC };
                KCT_GameStates.LastKnownTechCount = 0;

                KCT_GameStates.PermanentModAddedUpgradesButReallyWaitForTheAPI = 0;
                KCT_GameStates.TemporaryModAddedUpgradesButReallyWaitForTheAPI = 0;

                if (KCT_PresetManager.Instance != null)
                {
                    KCT_PresetManager.Instance.ClearPresets();
                    KCT_PresetManager.Instance = null;
                }

                return;
            }

            KCT_GameStates.MiscellaneousTempUpgrades = 0;

            /*if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                if (scene == GameScenes.SPACECENTER)
                {
                    KCT_PresetManager.Instance.FindPresetFiles();
                    KCT_PresetManager.Instance.LoadPresets();
                }
            }*/

            if (KCT_PresetManager.PresetLoaded() && !KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled) return;
            List<GameScenes> validScenes = new List<GameScenes> { GameScenes.SPACECENTER, GameScenes.TRACKSTATION, GameScenes.EDITOR };
            if (validScenes.Contains(scene))
            {
                TechDisableEventFinal();
            }

            if (HighLogic.LoadedScene == scene && scene == GameScenes.EDITOR) //Fix for null reference when using new or load buttons in editor
            {
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
            }

            if (HighLogic.LoadedSceneIsEditor)
            {
                EditorLogic.fetch.Unlock("KCTEditorMouseLock");
            }
        }

        public void launchScreenOpenEvent(GameEvents.VesselSpawnInfo v)
        {
            if (!KCT_GUI.PrimarilyDisabled)
            {
               // PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "Warning!", "To launch vessels you must first build them in the VAB or SPH, then launch them through the main KCT window in the Space Center!", "Ok", false, HighLogic.UISkin);
                //open the build list to the right page
                string selection = "VAB";
                if (v.craftSubfolder.Contains("SPH"))
                    selection = "SPH";
                KCT_GUI.ClickOn();
                KCT_GUI.SelectList("");
                KCT_GUI.SelectList(selection);
                KCTDebug.Log("Opening the GUI to the " + selection);
            }
        }

        public void vesselSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> ev)
        {
            if (ev.from == Vessel.Situations.PRELAUNCH && ev.host == FlightGlobals.ActiveVessel)
            {
                if (KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled &&
                    KCT_PresetManager.Instance.ActivePreset.generalSettings.ReconditioningTimes)
                {
                    //KCT_Recon_Rollout reconditioning = KCT_GameStates.ActiveKSC.Recon_Rollout.FirstOrDefault(r => ((IKCTBuildItem)r).GetItemName() == "LaunchPad Reconditioning");
                    //if (reconditioning == null)
                    if (HighLogic.CurrentGame.editorFacility == EditorFacility.VAB)
                    {
                        string launchSite = FlightDriver.LaunchSiteName;
                        if (launchSite == "LaunchPad") launchSite = KCT_GameStates.ActiveKSC.ActiveLPInstance.name;
                        KCT_GameStates.ActiveKSC.Recon_Rollout.Add(new KCT_Recon_Rollout(ev.host, KCT_Recon_Rollout.RolloutReconType.Reconditioning, ev.host.id.ToString(), launchSite));

                    }

                }
            }
        }

        public void vesselRecoverEvent(ProtoVessel v, bool unknownAsOfNow)
        {
            KCTDebug.Log("VesselRecoverEvent");
            if (!KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled) return;
            if (!v.vesselRef.isEVA)
            {
               // if (KCT_GameStates.settings.Debug && HighLogic.LoadedScene != GameScenes.TRACKSTATION && (v.wasControllable || v.protoPartSnapshots.Find(p => p.modules.Find(m => m.moduleName.ToLower() == "modulecommand") != null) != null))
                if (KCT_GameStates.recoveredVessel != null && v.vesselName == KCT_GameStates.recoveredVessel.shipName)
                {
                    //KCT_GameStates.recoveredVessel = new KCT_BuildListVessel(v);
                    //rebuy the ship if ScrapYard isn't overriding funds
                    if (!ScrapYardWrapper.OverrideFunds)
                    {
                        KCT_Utilities.SpendFunds(KCT_GameStates.recoveredVessel.cost, TransactionReasons.VesselRollout); //pay for the ship again
                    }

                    //pull all of the parts out of the inventory
                    //This is a bit funky since we grab the part id from our part, grab the inventory part out, then try to reapply that ontop of our part
                    if (ScrapYardWrapper.Available)
                    {
                        foreach (ConfigNode partNode in KCT_GameStates.recoveredVessel.ExtractedPartNodes)
                        {
                            string id = ScrapYardWrapper.GetPartID(partNode);
                            ConfigNode inventoryVersion = ScrapYardWrapper.FindInventoryPart(id);
                            if (inventoryVersion != null)
                            {
                                //apply it to our copy of the part
                                ConfigNode ourTracker = partNode.GetNodes("MODULE").FirstOrDefault(n => string.Equals(n.GetValue("name"), "ModuleSYPartTracker", StringComparison.Ordinal));
                                if (ourTracker != null)
                                {
                                    ourTracker.SetValue("TimesRecovered", inventoryVersion.GetValue("_timesRecovered"));
                                    ourTracker.SetValue("Inventoried", inventoryVersion.GetValue("_inventoried"));
                                }
                            }
                        }


                        //process the vessel in ScrapYard
                        ScrapYardWrapper.ProcessVessel(KCT_GameStates.recoveredVessel.ExtractedPartNodes);

                        //reset the BP
                        KCT_GameStates.recoveredVessel.buildPoints = KCT_Utilities.GetBuildTime(KCT_GameStates.recoveredVessel.ExtractedPartNodes);
                    }
                    if (KCT_GameStates.recoveredVessel.type == KCT_BuildListVessel.ListType.VAB)
                    {
                        KCT_GameStates.ActiveKSC.VABWarehouse.Add(KCT_GameStates.recoveredVessel);
                    }
                    else
                    {
                        KCT_GameStates.ActiveKSC.SPHWarehouse.Add(KCT_GameStates.recoveredVessel);
                    }

                    KCT_GameStates.ActiveKSC.Recon_Rollout.Add(new KCT_Recon_Rollout(KCT_GameStates.recoveredVessel, KCT_Recon_Rollout.RolloutReconType.Recovery, KCT_GameStates.recoveredVessel.id.ToString()));
                    KCT_GameStates.recoveredVessel = null;
                }
            }
        }


        private float GetResourceMass(List<ProtoPartResourceSnapshot> resources)
        {
            double mass = 0;
            foreach (ProtoPartResourceSnapshot resource in resources)
            {
                double amount = resource.amount;
                PartResourceDefinition RD = PartResourceLibrary.Instance.GetDefinition(resource.resourceName);
                mass += amount * RD.density;
            }
            return (float)mass;
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
