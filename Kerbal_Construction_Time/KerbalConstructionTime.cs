using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;
using System.Collections;
using KSP.UI.Screens;
using KSP.UI;

namespace KerbalConstructionTime
{
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class KCT_Tracking_Station : KerbalConstructionTime
    {

    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KCT_Flight : KerbalConstructionTime
    {

    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class KCT_SpaceCenter : KerbalConstructionTime
    {

    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class KCT_Editor : KerbalConstructionTime
    {

    }

    [KSPScenario(ScenarioCreationOptions.AddToAllGames, new GameScenes[] { GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION})]
    public class KerbalConstructionTimeData : ScenarioModule
    {
        public override void OnSave(ConfigNode node)
        {
            /* 1.4 Addition
            if (KCT_Utilities.CurrentGameIsMission())
            {
                return;
            }
            */
            // Boolean error = false;
            KCTDebug.Log("Writing to persistence.");
            base.OnSave(node);
            KCT_DataStorage kctVS = new KCT_DataStorage();
            node.AddNode(kctVS.AsConfigNode());
            foreach (KCT_KSC KSC in KCT_GameStates.KSCs)
            {
                if (KSC != null && KSC.KSCName != null && KSC.KSCName.Length > 0)
                    node.AddNode(KSC.AsConfigNode());
            }
            ConfigNode tech = new ConfigNode("TechList");
            foreach (KCT_TechItem techItem in KCT_GameStates.TechList)
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
            node.AddNode(tech);
        }
        public override void OnLoad(ConfigNode node)
        {
            
            base.OnLoad(node);
            /* 1.4 Addition
            if (KCT_Utilities.CurrentGameIsMission())
            {
                return;
            }
            */
            KCTDebug.Log("Reading from persistence.");
            KCT_GameStates.KSCs.Clear();
            KCT_GameStates.ActiveKSC = null;
            //KCT_Utilities.SetActiveKSC("Stock");
            KCT_GameStates.TechList.Clear();
            KCT_GameStates.TechUpgradesTotal = 0;

            KCT_DataStorage kctVS = new KCT_DataStorage();
            ConfigNode CN = node.GetNode(kctVS.GetType().Name);
            if (CN != null)
                ConfigNode.LoadObjectFromConfig(kctVS, CN);

            foreach (ConfigNode ksc in node.GetNodes("KSC"))
            {
                string name = ksc.GetValue("KSCName");
                KCT_KSC loaded_KSC = new KCT_KSC(name);
                loaded_KSC.FromConfigNode(ksc);
                if (loaded_KSC != null && loaded_KSC.KSCName != null && loaded_KSC.KSCName.Length > 0)
                {
                    loaded_KSC.RDUpgrades[1] = KCT_GameStates.TechUpgradesTotal;
                    if (KCT_GameStates.KSCs.Find(k => k.KSCName == loaded_KSC.KSCName) == null)
                        KCT_GameStates.KSCs.Add(loaded_KSC);
                }
            }
            KCT_Utilities.SetActiveKSCToRSS();

            
            ConfigNode tmp = node.GetNode("TechList");
            if (tmp != null)
            {
                foreach (ConfigNode techNode in tmp.GetNodes("Tech"))
                {
                    KCT_TechStorageItem techStorageItem = new KCT_TechStorageItem();
                    ConfigNode.LoadObjectFromConfig(techStorageItem, techNode);
                    KCT_TechItem techItem = techStorageItem.ToTechItem();
                    techItem.protoNode = new ProtoTechNode(techNode.GetNode("ProtoNode"));
                    KCT_GameStates.TechList.Add(techItem);
                }
            }

            KCT_GUI.CheckToolbar();
            KCT_GameStates.erroredDuringOnLoad.OnLoadFinish();
            //KerbalConstructionTime.DelayedStart();
        }
    }

    //[KSPAddon(KSPAddon.Startup.EditorAny | KSPAddon.Startup.Flight | KSPAddon.Startup.SpaceCentre | KSPAddon.Startup.TrackingStation, false)]
    public class KerbalConstructionTime : MonoBehaviour
    {
        internal void FacilityContextMenuSpawn(KSCFacilityContextMenu menu)
        {
            KCT_KSCContextMenuOverrider overrider = new KCT_KSCContextMenuOverrider(menu);
            StartCoroutine(overrider.OnContextMenuSpawn());
        }

        public bool editorRecalcuationRequired;
        public int updateRateThrottle;

        public static KerbalConstructionTime instance;

        public void OnDestroy()//more toolbar stuff
        {
            if (KCT_GameStates.kctToolbarButton != null)
            {
                KCT_GameStates.kctToolbarButton.Destroy();
            }
            if (KCT_Events.instance.KCTButtonStock != null)
            {
                KSP.UI.Screens.ApplicationLauncher.Instance.RemoveModApplication(KCT_Events.instance.KCTButtonStock);
            }

            KCT_GUI.guiDataSaver.Save();
        }

        private void OnGUI()
        {
            /* 1.4 Addition
            if (KCT_Utilities.CurrentGameIsMission())
            {
                return;
            }
            */
            KCT_GUI.SetGUIPositions();
        }

        public void Awake()
        {
            /* 1.4 Addition
            if (KCT_Utilities.CurrentGameIsMission())
            {
                return;
            }
            */
            KCTDebug.Log("Awake called");
            KCT_GameStates.erroredDuringOnLoad.OnLoadStart();
            KCT_GameStates.PersistenceLoaded = false;

            instance = this;

            KCT_GameStates.settings.Load(); //Load the settings file, if it exists

            string SavedFile = KSPUtil.ApplicationRootPath + "/saves/" + HighLogic.SaveFolder + "/KCT_Settings.cfg";
            if (!System.IO.File.Exists(SavedFile))
            {
                KCT_GameStates.firstStart = true;
            }

            if (KCT_PresetManager.Instance == null)
            {
                KCT_PresetManager.Instance = new KCT_PresetManager();
            }
            KCT_PresetManager.Instance.SetActiveFromSaveData();


            //Add the toolbar button
            if (ToolbarManager.ToolbarAvailable && ToolbarManager.Instance != null && KCT_GameStates.settings.PreferBlizzyToolbar)
            {
                KCTDebug.Log("Adding Toolbar Button");
                KCT_GameStates.kctToolbarButton = ToolbarManager.Instance.add("Kerbal_Construction_Time", "MainButton");
                if (KCT_GameStates.kctToolbarButton != null)
                {
                    if (KCT_PresetManager.PresetLoaded() && !KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled) KCT_GameStates.kctToolbarButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER);
                    else KCT_GameStates.kctToolbarButton.Visibility = new GameScenesVisibility(new GameScenes[] { GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.EDITOR });
                    KCT_GameStates.kctToolbarButton.TexturePath = KCT_Utilities.GetButtonTexture();
                    KCT_GameStates.kctToolbarButton.ToolTip = "Kerbal Construction Time";
                    KCT_GameStates.kctToolbarButton.OnClick += ((e) =>
                    {
                        KCT_GUI.ClickToggle();
                    });
                }
            }
            KCTDebug.Log("Awake finished");
        }

        public void Start()
        {
            /* 1.4 Addition
            if (KCT_Utilities.CurrentGameIsMission())
            {
                return;
            }
            */

            KCTDebug.Log("Start called");

            //add the events
            if (!KCT_Events.instance.eventAdded)
            {
                KCT_Events.instance.addEvents();
            }

            KCT_GameStates.settings.Save(); //Save the settings file, with defaults if it doesn't exist
            KCT_PresetManager.Instance.SaveActiveToSaveData();

            // Ghetto event queue
            if (HighLogic.LoadedScene == GameScenes.EDITOR)
            {
                InvokeRepeating("EditorRecalculation", 1, 1);

                KCT_GUI.buildRateForDisplay = null;
                if (!KCT_GUI.PrimarilyDisabled)
                {
                    KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                }
            }

            if (KCT_GUI.PrimarilyDisabled)
            {
                if (InputLockManager.GetControlLock("KCTLaunchLock") == ControlTypes.EDITOR_LAUNCH)
                    InputLockManager.RemoveControlLock("KCTLaunchLock");
            }

            KACWrapper.InitKACWrapper();

            if (!KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled)
            {
                if (InputLockManager.GetControlLock("KCTKSCLock") == ControlTypes.KSC_FACILITIES)
                    InputLockManager.RemoveControlLock("KCTKSCLock");
                return;
            }

            //Begin primary mod functions

            KCT_GameStates.UT = Planetarium.GetUniversalTime();

            KCT_GUI.guiDataSaver.Load();

            if (HighLogic.LoadedSceneIsEditor)
            {
                KCT_GUI.hideAll();
                if (!KCT_GUI.PrimarilyDisabled)
                {
                    KCT_GUI.showEditorGUI = KCT_GameStates.showWindows[1];
                    if (KCT_GUI.showEditorGUI)
                        KCT_GUI.ClickOn();
                    else
                        KCT_GUI.ClickOff();
                }
            }
            else if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                bool shouldStart = KCT_GUI.showFirstRun;
                KCT_GUI.hideAll();
                if (!shouldStart)
                {
                    KCT_GUI.showBuildList = KCT_GameStates.showWindows[0];
                    if (KCT_GUI.showBuildList)
                        KCT_GUI.ClickOn();
                    else
                        KCT_GUI.ClickOff();
                }
                KCT_GUI.showFirstRun = shouldStart;
            }

            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH)
            {
                if (FlightGlobals.ActiveVessel.GetCrewCount() == 0 && KCT_GameStates.launchedCrew.Count > 0)
                {
                    KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;

                    for (int i = 0; i < FlightGlobals.ActiveVessel.parts.Count; i++)
                    {
                        Part p = FlightGlobals.ActiveVessel.parts[i];
                        //KCTDebug.Log("craft: " + p.craftID);
                        {
                            CrewedPart cP = KCT_GameStates.launchedCrew.Find(part => part.partID == p.craftID);
                            if (cP == null) continue;
                            List<ProtoCrewMember> crewList = cP.crewList;
                            foreach (ProtoCrewMember crewMember in crewList)
                            {
                                if (crewMember != null)
                                {
                                    ProtoCrewMember finalCrewMember = crewMember;
                                    if (crewMember.type == ProtoCrewMember.KerbalType.Crew)
                                    {
                                        finalCrewMember = roster.Crew.FirstOrDefault(c => c.name == crewMember.name);
                                    }
                                    else if (crewMember.type == ProtoCrewMember.KerbalType.Tourist)
                                    {
                                        finalCrewMember = roster.Tourist.FirstOrDefault(c => c.name == crewMember.name);
                                    }
                                    if (finalCrewMember == null)
                                    {
                                        Debug.LogError("Error when assigning " + crewMember.name + " to " + p.partInfo.name +". Cannot find Kerbal in list.");
                                        continue;
                                    }
                                    try
                                    {
                                        KCTDebug.Log("Assigning " + finalCrewMember.name + " to " + p.partInfo.name);
                                        if (p.AddCrewmember(finalCrewMember))//p.AddCrewmemberAt(finalCrewMember, crewList.IndexOf(crewMember)))
                                        {
                                            finalCrewMember.rosterStatus = ProtoCrewMember.RosterStatus.Assigned;
                                            if (finalCrewMember.seat != null)
                                                finalCrewMember.seat.SpawnCrew();
                                        }
                                        else
                                        {
                                            Debug.LogError("Error when assigning " + crewMember.name + " to " + p.partInfo.name);
                                            finalCrewMember.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                                            continue;
                                        }
                                    }
                                    catch
                                    {
                                        Debug.LogError("Error when assigning " + crewMember.name + " to " + p.partInfo.name);
                                        finalCrewMember.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    KCT_GameStates.launchedCrew.Clear();
                }
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                KCT_GUI.hideAll();
                if (KCT_GameStates.launchedVessel != null && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH)
                {
                    KCT_GameStates.launchedVessel.KSC = null; //it's invalid now
                    KCTDebug.Log("Attempting to remove launched vessel from build list");
                    bool removed = KCT_GameStates.launchedVessel.RemoveFromBuildList();
                    if (removed) //Only do these when the vessel is first removed from the list
                    {
                        //Add the cost of the ship to the funds so it can be removed again by KSP
                        KCT_Utilities.AddFunds(KCT_GameStates.launchedVessel.cost, TransactionReasons.VesselRollout);
                        FlightGlobals.ActiveVessel.vesselName = KCT_GameStates.launchedVessel.shipName;
                    }
                    KCT_Recon_Rollout rollout = KCT_GameStates.ActiveKSC.Recon_Rollout.FirstOrDefault(r => r.associatedID == KCT_GameStates.launchedVessel.id.ToString());
                    if (rollout != null)
                        KCT_GameStates.ActiveKSC.Recon_Rollout.Remove(rollout);
                }
            }
            ratesUpdated = false;
            KCTDebug.Log("Start finished");
            DelayedStart();
        }

        private void EditorRecalculation()
        {
            if (editorRecalcuationRequired && !KCT_GUI.PrimarilyDisabled)
            {
                KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                editorRecalcuationRequired = false;
            }
        }


        private static int lvlCheckTimer = 0;
        private static bool ratesUpdated = false;
        public void FixedUpdate()
        {
            /* 1.4 Addition
            if (KCT_Utilities.CurrentGameIsMission())
            {
                return;
            }
            */

            double lastUT = KCT_GameStates.UT > 0 ? KCT_GameStates.UT : Planetarium.GetUniversalTime();
            KCT_GameStates.UT = Planetarium.GetUniversalTime();
            try
            {
                if (KCT_Events.instance != null && KCT_Events.instance.KCTButtonStock != null)
                    if (KCT_GUI.clicked)
                        KCT_Events.instance.KCTButtonStock.SetTrue(false);
                    else
                        KCT_Events.instance.KCTButtonStock.SetFalse(false);

                if (!KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled)
                    return;

                if (!KCT_GameStates.erroredDuringOnLoad.AlertFired && KCT_GameStates.erroredDuringOnLoad.HasErrored())
                {
                    KCT_GameStates.erroredDuringOnLoad.FireAlert();
                }

                if (KCT_GameStates.UpdateLaunchpadDestructionState)
                {
                    KCTDebug.Log("Updating launchpad destruction state.");
                    KCT_GameStates.UpdateLaunchpadDestructionState = false;
                    KCT_GameStates.ActiveKSC.ActiveLPInstance.SetDestructibleStateFromNode();
                    if (KCT_GameStates.ActiveKSC.ActiveLPInstance.upgradeRepair)
                    {
                        //repair everything, then update the node
                        KCT_GameStates.ActiveKSC.ActiveLPInstance.RefreshDestructionNode();
                        KCT_GameStates.ActiveKSC.ActiveLPInstance.CompletelyRepairNode();
                        KCT_GameStates.ActiveKSC.ActiveLPInstance.SetDestructibleStateFromNode();
                    }

                }

                if (!ratesUpdated)
                {
                    if (HighLogic.LoadedScene == GameScenes.SPACECENTER) 
                    {
                        if (ScenarioUpgradeableFacilities.GetFacilityLevelCount(SpaceCenterFacility.VehicleAssemblyBuilding) >= 0)
                        {
                            ratesUpdated = true;
                            KCTDebug.Log("Updating build rates");
                            foreach (KCT_KSC KSC in KCT_GameStates.KSCs)
                            {
                                KSC?.RecalculateBuildRates();
                                KSC?.RecalculateUpgradedBuildRates();
                            }

                            KCTDebug.Log("Rates updated");

                            foreach (SpaceCenterFacility facility in Enum.GetValues(typeof(SpaceCenterFacility)))
                            {
                                KCT_GameStates.BuildingMaxLevelCache[facility.ToString()] = ScenarioUpgradeableFacilities.GetFacilityLevelCount(facility);
                                KCTDebug.Log("Cached " + facility.ToString() + " max at " + KCT_GameStates.BuildingMaxLevelCache[facility.ToString()]);
                            }
                        }
                    }
                    else
                    {
                        ratesUpdated = true;
                    }
                }

                if (KCT_GameStates.ActiveKSC?.ActiveLPInstance != null && HighLogic.LoadedScene == GameScenes.SPACECENTER && KCT_Utilities.CurrentGameIsCareer())
                {
                    if (lvlCheckTimer++ > 30)
                    {
                        lvlCheckTimer = 0;
                        if (KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.LaunchPad) != KCT_GameStates.ActiveKSC.ActiveLPInstance.level)
                        {
                            KCT_GameStates.ActiveKSC.SwitchLaunchPad(KCT_GameStates.ActiveKSC.ActiveLaunchPadID, false);
                            KCT_GameStates.UpdateLaunchpadDestructionState = true;
                        }
                    }
                }
                //Warp code
                if (!KCT_GUI.PrimarilyDisabled && (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.TRACKSTATION))
                {
                    IKCTBuildItem ikctItem = KCT_Utilities.NextThingToFinish();
                    if (KCT_GameStates.targetedItem == null && ikctItem != null) KCT_GameStates.targetedItem = ikctItem;
                    double remaining = ikctItem != null ? ikctItem.GetTimeLeft() : -1;
                    double dT = TimeWarp.CurrentRate / (KCT_GameStates.UT - lastUT);
                    if (dT >= 20)
                        dT = 0.1;
                    //KCTDebug.Log("dt: " + dT);
                    int nBuffers = 1;
                    if (KCT_GameStates.canWarp && ikctItem != null && !ikctItem.IsComplete())
                    {
                        int warpRate = TimeWarp.CurrentRateIndex;
                        if (warpRate < KCT_GameStates.lastWarpRate) //if something else changes the warp rate then release control to them, such as Kerbal Alarm Clock
                        {
                            KCT_GameStates.canWarp = false;
                            KCT_GameStates.lastWarpRate = 0;
                        }
                        else
                        {
                            if (ikctItem == KCT_GameStates.targetedItem && warpRate > 0 && TimeWarp.fetch.warpRates[warpRate] * dT * nBuffers > Math.Max(remaining, 0))
                            {
                                int newRate = warpRate;
                                //find the first rate that is lower than the current rate
                                while (newRate > 0)
                                {
                                    if (TimeWarp.fetch.warpRates[newRate] * dT * nBuffers < remaining)
                                    break;
                                newRate--;
                                }
                                KCTDebug.Log("Warping down to " + newRate + " (delta: " + (TimeWarp.fetch.warpRates[newRate] * dT) + ")");
                                TimeWarp.SetRate(newRate, true); //hopefully a faster warp down than before
                                warpRate = newRate;
                            }
                            else if (warpRate == 0 && KCT_GameStates.warpInitiated)
                            {
                                KCT_GameStates.canWarp = false;
                                KCT_GameStates.warpInitiated = false;
                                KCT_GameStates.targetedItem = null;

                            }
                            KCT_GameStates.lastWarpRate = warpRate;
                        }

                    }
                    else if (ikctItem != null && ikctItem == KCT_GameStates.targetedItem && (KCT_GameStates.warpInitiated || KCT_GameStates.settings.ForceStopWarp) && TimeWarp.CurrentRateIndex > 0 && (remaining < 1) && (!ikctItem.IsComplete())) //Still warp down even if we don't control the clock
                    {
                        TimeWarp.SetRate(0, true);
                        KCT_GameStates.warpInitiated = false;
                        KCT_GameStates.targetedItem = null;
                    }
                }

                if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
                {
                    KCT_Utilities.SetActiveKSCToRSS();
                }

                if (!KCT_GUI.PrimarilyDisabled && HighLogic.LoadedScene == GameScenes.SPACECENTER)
                {
                    if (KSP.UI.Screens.VesselSpawnDialog.Instance != null && KSP.UI.Screens.VesselSpawnDialog.Instance.Visible)
                    {
                        KSP.UI.Screens.VesselSpawnDialog.Instance.ButtonClose();
                        KCTDebug.Log("Attempting to close spawn dialog!");
                    }
                }

                if (!KCT_GUI.PrimarilyDisabled)
                    KCT_Utilities.ProgressBuildTime();

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void LateUpdate()
        {
            /* 1.4 Addition
            if (KCT_Utilities.CurrentGameIsMission())
            {
                return;
            }
            */
            // FIXME really should run this only once, and then again on techlist change.
            // For now, spam per frame
            if (KSP.UI.Screens.RDController.Instance != null)
            {
                for (int i = KSP.UI.Screens.RDController.Instance.nodes.Count; i-- > 0;)
                {
                    KSP.UI.Screens.RDNode node = KSP.UI.Screens.RDController.Instance.nodes[i];
                    if (node?.tech != null)
                    {
                        if (HasTechInList(node.tech.techID))
                        {
                            node.graphics?.SetIconColor(XKCDColors.KSPNotSoGoodOrange);
                        }
                        // else reset? Bleh, why bother.
                    }
                }
            }
        }

        protected bool HasTechInList(string id)
        {
            for (int i = KCT_GameStates.TechList.Count; i-- > 0;)
                if (KCT_GameStates.TechList[i].techID == id)
                    return true;

            return false;
        }

        public static void DelayedStart()
        {
            /* 1.4 Addition
            if (KCT_Utilities.CurrentGameIsMission())
            {
                return;
            }
            */
            KCTDebug.Log("DelayedStart start");
            if (KCT_PresetManager.Instance?.ActivePreset == null || !KCT_PresetManager.Instance.ActivePreset.generalSettings.Enabled)
                return;

            if (KCT_GUI.PrimarilyDisabled) return;

            //The following should only be executed when fully enabled for the save

            if (KCT_GameStates.ActiveKSC == null)
            {
                KCT_Utilities.SetActiveKSCToRSS();
            }

            KCTDebug.Log("Checking vessels for missing parts.");
            //check that all parts are valid in all ships. If not, warn the user and disable that vessel (once that code is written)
            if (!KCT_GameStates.vesselErrorAlerted)
            {
                List<KCT_BuildListVessel> erroredVessels = new List<KCT_BuildListVessel>();
                foreach (KCT_KSC KSC in KCT_GameStates.KSCs) //this is faster on subsequent scene changes
                {
                    foreach (KCT_BuildListVessel blv in KSC.VABList)
                    {
                        if (!blv.allPartsValid)
                        {
                            //error!
                            KCTDebug.Log(blv.shipName + " contains invalid parts!");
                            erroredVessels.Add(blv);
                        }
                    }
                    foreach (KCT_BuildListVessel blv in KSC.VABWarehouse)
                    {
                        if (!blv.allPartsValid)
                        {
                            //error!
                            KCTDebug.Log(blv.shipName + " contains invalid parts!");
                            erroredVessels.Add(blv);
                        }
                    }
                    foreach (KCT_BuildListVessel blv in KSC.SPHList)
                    {
                        if (!blv.allPartsValid)
                        {
                            //error!
                            KCTDebug.Log(blv.shipName + " contains invalid parts!");
                            erroredVessels.Add(blv);
                        }
                    }
                    foreach (KCT_BuildListVessel blv in KSC.SPHWarehouse)
                    {
                        if (!blv.allPartsValid)
                        {
                            //error!
                            KCTDebug.Log(blv.shipName + " contains invalid parts!");
                            erroredVessels.Add(blv);
                        }
                    }
                }
                if (erroredVessels.Count > 0)
                    PopUpVesselError(erroredVessels);
                KCT_GameStates.vesselErrorAlerted = true;
            }
           
            if (HighLogic.LoadedSceneIsEditor)
            {
                if (KCT_GameStates.EditorShipEditingMode)
                {
                    KCTDebug.Log("Editing " + KCT_GameStates.editedVessel.shipName);
                    EditorLogic.fetch.shipNameField.text = KCT_GameStates.editedVessel.shipName;
                }
            }
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                KCTDebug.Log("SP Start");
                if (!KCT_GUI.PrimarilyDisabled)
                {
                    if (ToolbarManager.ToolbarAvailable && KCT_GameStates.settings.PreferBlizzyToolbar)
                        if (KCT_GameStates.showWindows[0])
                            KCT_GUI.ClickOn();
                    else
                    {
                        if (KCT_Events.instance != null && KCT_Events.instance.KCTButtonStock != null)
                        {
                            if (KCT_GameStates.showWindows[0])
                                KCT_GUI.ClickOn();
                        }
                    }
                    KCT_GUI.ResetBLWindow();
                }
                else
                {
                    KCT_GUI.showBuildList = false;
                    KCT_GameStates.showWindows[0] = false;
                }
                KCTDebug.Log("SP UI done");

                if (KCT_GameStates.firstStart)
                {
                    KCTDebug.Log("Showing first start.");
                    KCT_GameStates.firstStart = false;
                    KCT_GUI.showFirstRun = true;
                    
                    //initialize the proper launchpad
                    KCT_GameStates.ActiveKSC.ActiveLPInstance.level = KCT_Utilities.BuildingUpgradeLevel(SpaceCenterFacility.LaunchPad);

                }

                KCTDebug.Log("SP switch starting");
                KCT_GameStates.ActiveKSC.SwitchLaunchPad(KCT_GameStates.ActiveKSC.ActiveLaunchPadID);
                KCTDebug.Log("SP switch done");

                foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                {
                    //foreach (KCT_Recon_Rollout rr in ksc.Recon_Rollout)
                    for (int i = 0; i < ksc.Recon_Rollout.Count; i++)
                    {
                        KCT_Recon_Rollout rr = ksc.Recon_Rollout[i];
                        if (rr.RRType != KCT_Recon_Rollout.RolloutReconType.Reconditioning && KCT_Utilities.FindBLVesselByID(new Guid(rr.associatedID)) == null)
                        {
                            KCTDebug.Log("Invalid Recon_Rollout at " + ksc.KSCName + ". ID " + rr.associatedID + " not found.");
                            ksc.Recon_Rollout.Remove(rr);
                            i--;
                        }
                    }
                }
                KCTDebug.Log("SP done");
            }
            KCTDebug.Log("DelayedStart finished");
        }

        public static void PopUpVesselError(List<KCT_BuildListVessel> errored)
        {
            DialogGUIBase[] options = new DialogGUIBase[2];
            options[0] = new DialogGUIButton("Understood", () => { });
           // new DialogGUIBase("Understood", () => { }); //do nothing and close the window
            options[1] = new DialogGUIButton("Delete Vessels", () =>
            {
                foreach (KCT_BuildListVessel blv in errored)
                {
                    blv.RemoveFromBuildList();
                    KCT_Utilities.AddFunds(blv.cost, TransactionReasons.VesselRollout);
                    //remove any associated recon_rollout
                }
            });

            string txt = "The following KCT vessels contain missing or invalid parts and have been quarantined. Either add the missing parts back into your game or delete the vessels. A file containing the ship names and missing parts has been added to your save folder.\n";
            string txtToWrite = "";
            foreach (KCT_BuildListVessel blv in errored)
            {
                txt += blv.shipName + "\n";
                txtToWrite += blv.shipName+"\n";
                txtToWrite += String.Join("\n", blv.MissingParts().ToArray());
                txtToWrite += "\n\n";
            }

            //HighLogic.SaveFolder
            //make new file for missing ships
            string filename = KSPUtil.ApplicationRootPath + "/saves/" + HighLogic.SaveFolder + "/missingParts.txt";
            System.IO.File.WriteAllText(filename, txtToWrite);


            //remove all rollout and recon items since they're invalid without the ships
            foreach (KCT_BuildListVessel blv in errored)
            {
                //remove any associated recon_rollout
                foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                {
                    for (int i = 0; i < ksc.Recon_Rollout.Count; i++)
                    {
                        KCT_Recon_Rollout rr = ksc.Recon_Rollout[i];
                        if (rr.associatedID == blv.id.ToString())
                        {
                            ksc.Recon_Rollout.Remove(rr);
                            i--;
                        }
                    }
                }
            }


            MultiOptionDialog diag = new MultiOptionDialog("missingPartsPopup", txt, "Vessels Contain Missing Parts", null, options);
            PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), diag, false, HighLogic.UISkin);
            //PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "Vessel Contains Missing Parts", "The KCT vessel " + errored.shipName + " contains missing or invalid parts. You will not be able to do anything with the vessel until the parts are available again.", "Understood", false, HighLogic.UISkin);
        }

        public static void ShowLaunchAlert(string launchSite)
        {
            KCTDebug.Log("Showing Launch Alert");
            if (KCT_GUI.PrimarilyDisabled)
            {
                EditorLogic.fetch.launchVessel();
            }
            else
            {
                KCT_Utilities.AddVesselToBuildList(launchSite);
                KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
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
