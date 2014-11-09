using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace Kerbal_Construction_Time
{
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class KCT_Tracking_Station : Kerbal_Construction_Time
    {

    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KCT_Flight : Kerbal_Construction_Time
    {

    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class KCT_SpaceCenter : Kerbal_Construction_Time
    {
        
    }

    [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
    public class KCT_VABEditor : Kerbal_Construction_Time
    {

    }

    [KSPAddon(KSPAddon.Startup.EditorSPH, false)]
    public class KCT_SPHEditor : Kerbal_Construction_Time
    {

    }

    public class KerbalConstructionTimeData : ScenarioModule
    {
        public override void OnSave(ConfigNode node)
        {
            Boolean error = false;
            KCTDebug.Log("Writing to persistence.");
            base.OnSave(node);
            KCT_DataStorage kctVS = new KCT_DataStorage();
            KCT_BuildListStorage bls = new KCT_BuildListStorage();
            KCT_TechStorage tS = new KCT_TechStorage();
            node.AddNode(kctVS.AsConfigNode());
            node.AddNode(bls.AsConfigNode());
            node.AddNode(tS.AsConfigNode());

            for (int i=0; i<KCT_GameStates.VABList.Count; i++)
            {
                KCTDebug.Log("VAB"+i);
                ConfigNode CN = new ConfigNode();
                if (KCT_GameStates.VABList[i].shipNode != null)
                {
                    KCT_GameStates.VABList[i].shipNode.CopyTo(CN, "VAB" + i);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.LogError("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE VAB" + i);
                    error = true;
                }
            }
            for (int i = 0; i < KCT_GameStates.SPHList.Count; i++)
            {
                KCTDebug.Log("SPH" + i);
                ConfigNode CN = new ConfigNode();
                if (KCT_GameStates.SPHList[i].shipNode != null)
                {
                    KCT_GameStates.SPHList[i].shipNode.CopyTo(CN, "SPH" + i);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.LogError("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE SPH" + i);
                    error = true;
                }
            }
            for (int i = 0; i < KCT_GameStates.VABWarehouse.Count; i++)
            {
                KCTDebug.Log("VABWH" + i);
                ConfigNode CN = new ConfigNode();
                if (KCT_GameStates.VABWarehouse[i].shipNode != null)
                {
                    KCT_GameStates.VABWarehouse[i].shipNode.CopyTo(CN, "VABWH" + i);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.LogError("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE VABWH" + i);
                    error = true;
                }
            }
            for (int i = 0; i < KCT_GameStates.SPHWarehouse.Count; i++)
            {
                KCTDebug.Log("SPHWH" + i);
                ConfigNode CN = new ConfigNode();
                if (KCT_GameStates.SPHWarehouse[i].shipNode != null)
                {
                    KCT_GameStates.SPHWarehouse[i].shipNode.CopyTo(CN, "SPHWH" + i);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.LogError("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE SPHWH" + i);
                    error = true;
                }
            }
            for (int i=0; i< KCT_GameStates.TechList.Count; i++)
            {
                KCTDebug.Log("Tech" + i);
                ConfigNode CN = new ConfigNode("Tech"+i);
                if (KCT_GameStates.TechList[i].protoNode != null)
                {
                    KCT_GameStates.TechList[i].protoNode.Save(CN);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.LogError("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE Tech" + i);
                    error = true;
                }
            }

            if (error)
            {
                //TODO: Popup with error message
            }
        }
        public override void OnLoad(ConfigNode node)
        {
            KCTDebug.Log("Reading from persistence.");
            base.OnLoad(node);
            KCT_DataStorage kctVS = new KCT_DataStorage();
            KCT_BuildListStorage bls = new KCT_BuildListStorage();
            KCT_TechStorage tS = new KCT_TechStorage();
            ConfigNode CN = node.GetNode(kctVS.GetType().Name);
            if (CN != null)
                ConfigNode.LoadObjectFromConfig(kctVS, CN);

            CN = node.GetNode(bls.GetType().Name);
            if (CN != null)
                ConfigNode.LoadObjectFromConfig(bls, CN);

            CN = node.GetNode(tS.GetType().Name);
            if (CN != null)
                ConfigNode.LoadObjectFromConfig(tS, CN);

            for (int i = 0; i < KCT_GameStates.VABList.Count; i++)
            {
                KCT_GameStates.VABList[i].shipNode = node.GetNode("VAB" + i);
            }
            for (int i = 0; i < KCT_GameStates.SPHList.Count; i++)
            {
                KCT_GameStates.SPHList[i].shipNode = node.GetNode("SPH" + i);
            }
            for (int i = 0; i < KCT_GameStates.VABWarehouse.Count; i++)
            {
                KCT_GameStates.VABWarehouse[i].shipNode = node.GetNode("VABWH" + i);
            }
            for (int i = 0; i < KCT_GameStates.SPHWarehouse.Count; i++)
            {
                KCT_GameStates.SPHWarehouse[i].shipNode = node.GetNode("SPHWH" + i);
            }
            for (int i = 0; i < KCT_GameStates.TechList.Count; i++)
            {
                KCT_GameStates.TechList[i].protoNode = new ProtoTechNode(node.GetNode("Tech" + i));
            }

            Kerbal_Construction_Time.DelayedStart();
        }
    }

    public class Kerbal_Construction_Time : MonoBehaviour
    {
        public static MonoBehaviour instance;
        internal Kerbal_Construction_Time()
        {
            instance = this;
            if (ToolbarManager.ToolbarAvailable && ToolbarManager.Instance != null)
            {
                KCTDebug.Log("Adding Toolbar Button");
                KCT_GameStates.kctToolbarButton = ToolbarManager.Instance.add("Kerbal_Construction_Time", "MainButton");
                if (KCT_GameStates.kctToolbarButton != null)
                {
                    if (!KCT_GameStates.settings.enabledForSave) KCT_GameStates.kctToolbarButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER);
                    else KCT_GameStates.kctToolbarButton.Visibility = new GameScenesVisibility(new GameScenes[] { GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.EDITOR, GameScenes.SPH });
                    KCT_GameStates.kctToolbarButton.TexturePath = KCT_Utilities.GetButtonTexture();
                    KCT_GameStates.kctToolbarButton.ToolTip = "Kerbal Construction Time";
                    KCT_GameStates.kctToolbarButton.OnClick += ((e) =>
                    {
                        KCT_GUI.onClick();
                    });
                }
            }
        }

        

        public void OnDestroy()//more toolbar stuff
        {
            if (KCT_GameStates.kctToolbarButton != null)
            {
                KCT_GameStates.kctToolbarButton.Destroy();
            }
            if (KCT_Events.instance.KCTButtonStock != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(KCT_Events.instance.KCTButtonStock);
            }

            KCT_GUI.guiDataSaver.Save();
        }

        public void Awake()
        {
            RenderingManager.AddToPostDrawQueue(0, OnDraw);
        }

        private void OnDraw()
        {
            KCT_GUI.SetGUIPositions(OnWindow);
        }

        private void OnWindow(int windowID)
        {
            KCT_GUI.DrawGUIs(windowID);
        }

        public void Start()
        {
            KCT_GameStates.settings.Load(); //Load the settings file, if it exists
            KCT_GameStates.settings.Save(); //Save the settings file, with defaults if it doesn't exist
            KCT_GameStates.timeSettings.Load(); //Load the time settings
            KCT_GameStates.timeSettings.Save(); //Save the time settings
            
            //Code for saving to the persistence.sfs
            ProtoScenarioModule scenario = HighLogic.CurrentGame.scenarios.Find(s => s.moduleName == typeof(KerbalConstructionTimeData).Name);
            if (scenario == null)
            {
                try
                {
                    Debug.Log("[KCT] Adding InternalModule scenario to game '" + HighLogic.CurrentGame.Title + "'");
                    HighLogic.CurrentGame.AddProtoScenarioModule(typeof(KerbalConstructionTimeData), new GameScenes[] {GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.SPH, GameScenes.TRACKSTATION});
                    // the game will add this scenario to the appropriate persistent file on save from now on
                }
                catch (ArgumentException ae)
                {
                    Debug.LogException(ae);
                }
                catch
                {
                    Debug.Log("[KCT] Unknown failure while adding scenario.");
                }
            }
            else
            {
                if (!scenario.targetScenes.Contains(GameScenes.SPACECENTER))
                    scenario.targetScenes.Add(GameScenes.SPACECENTER);
                if (!scenario.targetScenes.Contains(GameScenes.FLIGHT))
                    scenario.targetScenes.Add(GameScenes.FLIGHT);
                if (!scenario.targetScenes.Contains(GameScenes.EDITOR))
                    scenario.targetScenes.Add(GameScenes.EDITOR);
                if (!scenario.targetScenes.Contains(GameScenes.SPH))
                    scenario.targetScenes.Add(GameScenes.SPH);
                if (!scenario.targetScenes.Contains(GameScenes.TRACKSTATION))
                    scenario.targetScenes.Add(GameScenes.TRACKSTATION);

            }
            //End code for persistence.sfs

            if (KCT_GUI.PrimarilyDisabled)
            {
                if (InputLockManager.GetControlLock("KCTLaunchLock") == ControlTypes.EDITOR_LAUNCH)
                    InputLockManager.RemoveControlLock("KCTLaunchLock");
            }

            KACWrapper.InitKACWrapper();

            if (!KCT_Events.instance.eventAdded)
            {
                KCT_Events.instance.addEvents();
            }

            if (!KCT_GameStates.settings.enabledForSave)
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
                if (KCT_GUI.showSimulationCompleteEditor)
                {
                    KCT_GUI.hideAll();
                    KCT_GUI.showSimulationCompleteEditor = true;
                }
                else
                {
                    KCT_GUI.hideAll();
                }
                if (!KCT_GUI.PrimarilyDisabled)
                {
                    KCT_GUI.showEditorGUI = KCT_GameStates.showWindows[1];
                }
                if (KCT_GameStates.EditorShipEditingMode && KCT_GameStates.delayStart)
                {
                    KCT_GameStates.delayStart = false;
                    EditorLogic.fetch.shipNameField.Text = KCT_GameStates.editedVessel.shipName;
                }
            }
            else if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                KCT_GUI.hideAll();
                KCT_GameStates.reset();
                if (HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
                {
                    KCT_GameStates.TotalUpgradePoints = KCT_GameStates.settings.SandboxUpgrades;
                }
            }

            if (HighLogic.LoadedSceneIsFlight && !KCT_GameStates.flightSimulated && FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH
                        && FlightGlobals.ActiveVessel.GetCrewCount() == 0 && KCT_GameStates.launchedCrew.Count > 0)
            {
                KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;
                for (int i = 0; i < FlightGlobals.ActiveVessel.parts.Count; i++)
                {
                    Part p = FlightGlobals.ActiveVessel.parts[i];
                    {
                        CrewedPart cP = KCT_GameStates.launchedCrew.Find(part => part.partID == p.uid);
                        if (cP == null) continue;
                        List<ProtoCrewMember> crewList = cP.crewList;
                        foreach (ProtoCrewMember crewMember in crewList)
                        {
                            if (crewMember != null)
                            {
                                ProtoCrewMember finalCrewMember = crewMember;
                                foreach (ProtoCrewMember rosterCrew in roster.Crew)
                                {
                                    if (rosterCrew.name == crewMember.name)
                                        finalCrewMember = rosterCrew;
                                }
                                KCTDebug.Log("Assigning " + finalCrewMember.name + " to " + p.partInfo.name);
                                p.AddCrewmemberAt(finalCrewMember, crewList.IndexOf(crewMember));
                                finalCrewMember.rosterStatus = ProtoCrewMember.RosterStatus.Assigned;
                                if (finalCrewMember.seat != null)
                                    finalCrewMember.seat.SpawnCrew();
                            }
                        }
                    }
                }
                KCT_GameStates.launchedCrew.Clear();
            }
        }

        public static bool moved = false;
        private static bool updateChecked = false;
        public void FixedUpdate()
        {
            if (!KCT_GameStates.settings.enabledForSave)
                return;

            KCT_GameStates.UT = Planetarium.GetUniversalTime();
            try
            {
                if (!KCT_GUI.PrimarilyDisabled && (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.TRACKSTATION && !KCT_GameStates.flightSimulated))
                {
                    IKCTBuildItem ikctItem = KCT_Utilities.NextThingToFinish();
                    if (KCT_GameStates.targetedItem == null && ikctItem != null) KCT_GameStates.targetedItem = ikctItem;
                    if (KCT_GameStates.canWarp && ikctItem != null && !ikctItem.IsComplete())
                    {
                        int warpRate = TimeWarp.CurrentRateIndex;
                        if (SOIAlert())
                        {
                            TimeWarp.SetRate(0, true);
                            KCT_GameStates.canWarp = false;
                            KCT_GameStates.warpInitiated = false;

                        }
                        else if (warpRate < KCT_GameStates.lastWarpRate) //if something else changes the warp rate then release control to them, such as Kerbal Alarm Clock
                        {
                            KCT_GameStates.canWarp = false;
                            KCT_GameStates.lastWarpRate = 0;
                        }
                        else
                        {
                            if (ikctItem == KCT_GameStates.targetedItem && (10 * TimeWarp.deltaTime) > Math.Max((ikctItem.GetTimeLeft()), 0) && TimeWarp.CurrentRate > 1.0f)
                            {
                                TimeWarp.SetRate(--warpRate, true);
                            }
                            else if (warpRate == 0 && KCT_GameStates.warpInitiated)
                            {
                                KCT_GameStates.canWarp = false;
                                KCT_GameStates.warpInitiated = false;

                            }
                            KCT_GameStates.lastWarpRate = warpRate;
                        }

                    }
                    else if (ikctItem != null && ikctItem == KCT_GameStates.targetedItem && (KCT_GameStates.warpInitiated || KCT_GameStates.settings.ForceStopWarp) && TimeWarp.CurrentRate != 0 && (ikctItem.GetTimeLeft()) < (TimeWarp.deltaTime * 2) && (!ikctItem.IsComplete())) //Still warp down even if we don't control the clock
                    {
                        TimeWarp.SetRate(0, false);
                        KCT_GameStates.warpInitiated = false;
                    }
                    else if (ikctItem != null && (KCT_GameStates.settings.ForceStopWarp) && TimeWarp.CurrentRate != 0 &&  (!ikctItem.IsComplete()))
                    {
                        if ((10 * TimeWarp.deltaTime) > Math.Max((ikctItem.GetTimeLeft()), 0) && TimeWarp.CurrentRate > 1.0f)
                        {
                            TimeWarp.SetRate(TimeWarp.CurrentRateIndex-1, true);
                        }
                    }
                }

                if (HighLogic.LoadedScene == GameScenes.FLIGHT && KCT_GameStates.flightSimulated) //Simulated flights
                {
                    if (FlightGlobals.ActiveVessel.loaded && !FlightGlobals.ActiveVessel.packed && !moved)
                    {
                        //moved = true;
                        int secondsForMove = 3;
                        if (KCT_GameStates.simulateInOrbit && loadDeferTime == DateTime.MaxValue)
                        {
                            loadDeferTime = DateTime.Now;
                        }
                        else if (KCT_GameStates.simulateInOrbit && (!KCT_GameStates.delayMove || DateTime.Now.CompareTo(loadDeferTime.AddSeconds(secondsForMove)) > 0))
                        {
                            KCTDebug.Log("Moving vessel to orbit. " + KCT_GameStates.simulationBody.bodyName + ":" + KCT_GameStates.simOrbitAltitude + ":" + KCT_GameStates.simInclination);
                            KCT_OrbitAdjuster.PutInOrbitAround(KCT_GameStates.simulationBody, KCT_GameStates.simOrbitAltitude, KCT_GameStates.simInclination);
                            moved = true;
                            loadDeferTime = DateTime.MaxValue;
                        }
                        else if (!KCT_GameStates.simulateInOrbit)
                            moved = true;

                        if (KCT_GameStates.simulateInOrbit && loadDeferTime != DateTime.MaxValue && lastSeconds != (loadDeferTime.AddSeconds(secondsForMove) - DateTime.Now).Seconds)
                        {
                            double remaining = (loadDeferTime.AddSeconds(secondsForMove) - DateTime.Now).TotalSeconds;
                            ScreenMessages.PostScreenMessage("[KCT] Moving vessel in " + Math.Round(remaining) + " seconds", (float)(remaining - Math.Floor(remaining)), ScreenMessageStyle.UPPER_CENTER);
                            lastSeconds = (int)remaining;
                        }
                    }
                    if (KCT_GameStates.simulationEndTime > 0 && KCT_GameStates.UT >= KCT_GameStates.simulationEndTime)
                    {
                        FlightDriver.SetPause(true);
                        KCT_GUI.showSimulationCompleteFlight = true;
                    }
                    if (FlightGlobals.ActiveVessel.situation != Vessel.Situations.PRELAUNCH && KCT_GameStates.simulationEndTime == 0 && KCT_GameStates.simulationTimeLimit > 0)
                    {
                        KCT_GameStates.simulationEndTime = Planetarium.GetUniversalTime() + KCT_GameStates.simulationTimeLimit; //Just in case the event doesn't fire
                    }
                }

                if (!KCT_GUI.PrimarilyDisabled)
                    KCT_Utilities.ProgressBuildTime();
            }
            catch (IndexOutOfRangeException e)
            {
                print(e.Message);
                print(e.StackTrace);
            }

        }
        private static DateTime loadDeferTime = DateTime.MaxValue;
        private static int lastSeconds = 0;

        public static void DelayedStart()
        {
            if (!updateChecked)
            {
                if (KCT_GameStates.settings.CheckForUpdates && !KCT_GameStates.firstStart) //Check for updates
                    KCT_UpdateChecker.CheckForUpdate(false, KCT_GameStates.settings.VersionSpecific);
                updateChecked = true;
            }

            List<GameScenes> validScenes = new List<GameScenes> { GameScenes.SPACECENTER, GameScenes.TRACKSTATION, GameScenes.SPH, GameScenes.EDITOR };
            if (validScenes.Contains(HighLogic.LoadedScene))
            {
                //Check for simulation save and load it.
                if (System.IO.File.Exists(KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs"))
                {
                    KCT_Utilities.LoadSimulationSave();
                }
            }

            if (!HighLogic.LoadedSceneIsFlight && KCT_GameStates.buildSimulatedVessel)
            {
                KCT_GameStates.buildSimulatedVessel = false;
                KCT_BuildListVessel toBuild = KCT_GameStates.launchedVessel.NewCopy(false);
                toBuild.buildPoints = KCT_Utilities.GetBuildTime(toBuild.ExtractedPartNodes, true, KCT_GUI.useInventory);
                KCT_Utilities.AddVesselToBuildList(toBuild, KCT_GUI.useInventory);
            }

            if (!HighLogic.LoadedSceneIsFlight && KCT_GameStates.FundsToChargeAtSimEnd != 0)
            {
                KCT_Utilities.SpendFunds(KCT_GameStates.FundsToChargeAtSimEnd, TransactionReasons.None);
                KCT_GameStates.FundsToChargeAtSimEnd = 0;
            }
            if (!HighLogic.LoadedSceneIsFlight && KCT_GameStates.FundsGivenForVessel != 0)
            {
                KCT_Utilities.SpendFunds(KCT_GameStates.FundsGivenForVessel, TransactionReasons.VesselRollout);
                KCT_GameStates.FundsGivenForVessel = 0;
            }

            if (HighLogic.LoadedSceneIsFlight && !KCT_GameStates.flightSimulated)
            {
                KCT_GUI.hideAll();
                if (FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH)
                {
                    bool removed = KCT_GameStates.launchedVessel.RemoveFromBuildList();
                    if (removed) //Only do these when the vessel is first removed from the list
                    {
                        //Add the cost of the ship to the funds so it can be removed again by KSP
                        KCT_Utilities.AddFunds(KCT_Utilities.GetTotalVesselCost(FlightGlobals.ActiveVessel.protoVessel), TransactionReasons.VesselRollout);
                        FlightGlobals.ActiveVessel.vesselName = KCT_GameStates.launchedVessel.shipName;
                    }
                }

                List<VesselType> invalidTypes = new List<VesselType> { VesselType.Debris, VesselType.SpaceObject, VesselType.Unknown };
                if (!invalidTypes.Contains(FlightGlobals.ActiveVessel.vesselType) && !KCT_GameStates.BodiesVisited.Contains(FlightGlobals.ActiveVessel.mainBody.bodyName))
                {
                    KCT_GameStates.BodiesVisited.Add(FlightGlobals.ActiveVessel.mainBody.bodyName);
                    var message = new ScreenMessage("[KCT] New simulation body unlocked: " + FlightGlobals.ActiveVessel.mainBody.bodyName, 4.0f, ScreenMessageStyle.UPPER_LEFT);
                    ScreenMessages.PostScreenMessage(message, true);
                    KCTDebug.Log("Unlocked sim body: " + FlightGlobals.ActiveVessel.mainBody.bodyName);
                }
            }
            if (HighLogic.LoadedSceneIsFlight && KCT_GameStates.flightSimulated)
            {
                KCTDebug.Log("Simulation started");
                KCT_GUI.hideAll();
                KCT_GUI.showSimulationWindow = true;
                KCT_GUI.showTimeRemaining = true;
                Planetarium.SetUniversalTime(KCT_GameStates.simulationUT);
            }
            if (HighLogic.LoadedSceneIsEditor)
            {
                if (KCT_GameStates.EditorShipEditingMode)
                {
                    KCTDebug.Log("Editing " + KCT_GameStates.editedVessel.shipName);
                    EditorLogic.fetch.shipNameField.Text = KCT_GameStates.editedVessel.shipName;
                }
                if (!KCT_GUI.PrimarilyDisabled)
                {
                    if (KCT_GameStates.settings.OverrideLaunchButton)
                    {
                        KCTDebug.Log("Taking control of launch button");
                        EditorLogic.fetch.launchBtn.methodToInvoke = "ShowLaunchAlert";
                        EditorLogic.fetch.launchBtn.scriptWithMethodToInvoke = Kerbal_Construction_Time.instance;
                    }
                    else
                        InputLockManager.SetControlLock(ControlTypes.EDITOR_LAUNCH, "KCTLaunchLock");
                    KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                }
            }
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                if (KCT_Utilities.CurrentGameHasScience() && KCT_GameStates.TotalUpgradePoints == 0)
                {
                    ConfigNode CN = new ConfigNode();
                    ResearchAndDevelopment.Instance.snapshot.Save(CN);
                    ConfigNode[] techNodes = CN.GetNodes("Tech");
                    KCTDebug.Log("technodes length: " + techNodes.Length);
                    KCT_GameStates.TotalUpgradePoints = techNodes.Length + 14;
                }
                if (!KCT_GUI.PrimarilyDisabled)
                {
                    KCT_GUI.showBuildList = KCT_GameStates.showWindows[0];
                    KCT_GUI.ResetBLWindow();
                }
                else
                {
                    KCT_GUI.showBuildList = false;
                    KCT_GameStates.showWindows[0] = false;
                }
                if (KCT_GameStates.firstStart)
                {
                    KCTDebug.Log("Showing first start.");
                    KCT_GUI.showFirstRun = true;
                }
                KCT_GameStates.firstStart = false;
                if (KCT_GameStates.LaunchFromTS)
                {
                    KCT_GameStates.launchedVessel.Launch();
                }

                HighLogic.CurrentGame.Parameters.SpaceCenter.CanLaunchAtPad = false;
                HighLogic.CurrentGame.Parameters.SpaceCenter.CanLaunchAtRunway = false;
            }
        }

        public bool SOIAlert()
        {
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                if (KCT_GameStates.VesselTypesForSOI.Contains(v.vesselType))
                {
                    if (v != FlightGlobals.ActiveVessel)
                    {
                        if (!KCT_GameStates.vesselDict.ContainsKey(v.id.ToString()))
                        {
                            KCT_GameStates.vesselDict.Add(v.id.ToString(), v.mainBody.bodyName);
                            print("Vessel " + v.id.ToString() + " added to lstVessels.");

                        }
                        else if (v.mainBody.bodyName != KCT_GameStates.vesselDict[v.id.ToString()])
                        {
                            KCT_GameStates.lastSOIVessel = v.name;
                            print("Vessel " + v.id.ToString() + " SOI change.");
                            KCT_GameStates.vesselDict[v.id.ToString()] = v.mainBody.bodyName;
                            KCT_GUI.showSOIAlert = true;
                            return true;

                        }
                    }
                }
            }
            return false;
        }

        public void ShowLaunchAlert()
        {
            if (KCT_GUI.PrimarilyDisabled)
                EditorLogic.fetch.launchVessel();
            else
            {
                KCT_GUI.showLaunchAlert = true;
                EditorLogic.fetch.Lock(true, true, true, "KCTGUILock");
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