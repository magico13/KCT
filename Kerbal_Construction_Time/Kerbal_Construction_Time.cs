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
            Debug.Log("[KCT] Writing to persistence.");
            base.OnSave(node);
            KCT_DataStorage kctVS = new KCT_DataStorage();
            KCT_BuildListStorage bls = new KCT_BuildListStorage();
            KCT_TechStorage tS = new KCT_TechStorage();
            node.AddNode(kctVS.AsConfigNode());
            node.AddNode(bls.AsConfigNode());
            node.AddNode(tS.AsConfigNode());

            for (int i=0; i<KCT_GameStates.VABList.Count; i++)
            {
                Debug.Log("[KCT]: VAB"+i);
                ConfigNode CN = new ConfigNode();
                if (KCT_GameStates.VABList[i].shipNode != null)
                {
                    KCT_GameStates.VABList[i].shipNode.CopyTo(CN, "VAB" + i);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.Log("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE VAB" + i);
                    error = true;
                }
            }
            for (int i = 0; i < KCT_GameStates.SPHList.Count; i++)
            {
                Debug.Log("[KCT]: SPH" + i);
                ConfigNode CN = new ConfigNode();
                if (KCT_GameStates.SPHList[i].shipNode != null)
                {
                    KCT_GameStates.SPHList[i].shipNode.CopyTo(CN, "SPH" + i);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.Log("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE SPH" + i);
                    error = true;
                }
            }
            for (int i = 0; i < KCT_GameStates.VABWarehouse.Count; i++)
            {
                Debug.Log("[KCT]: VABWH" + i);
                ConfigNode CN = new ConfigNode();
                if (KCT_GameStates.VABWarehouse[i].shipNode != null)
                {
                    KCT_GameStates.VABWarehouse[i].shipNode.CopyTo(CN, "VABWH" + i);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.Log("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE VABWH" + i);
                    error = true;
                }
            }
            for (int i = 0; i < KCT_GameStates.SPHWarehouse.Count; i++)
            {
                Debug.Log("[KCT]: SPHWH" + i);
                ConfigNode CN = new ConfigNode();
                if (KCT_GameStates.SPHWarehouse[i].shipNode != null)
                {
                    KCT_GameStates.SPHWarehouse[i].shipNode.CopyTo(CN, "SPHWH" + i);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.Log("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE SPHWH" + i);
                    error = true;
                }
            }
            for (int i=0; i< KCT_GameStates.TechList.Count; i++)
            {
                Debug.Log("[KCT]: Tech" + i);
                ConfigNode CN = new ConfigNode("Tech"+i);
                if (KCT_GameStates.TechList[i].protoNode != null)
                {
                    KCT_GameStates.TechList[i].protoNode.Save(CN);
                    node.AddNode(CN);
                }
                else
                {
                    Debug.Log("[KCT] WARNING! DATA FAILURE EVENT ON CONFIGNODE Tech" + i);
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
            Debug.Log("[KCT] Reading from persistence.");
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

            KCT_GameStates.delayStart = true;
        }
    }

    public class Kerbal_Construction_Time : MonoBehaviour
    {
        internal Kerbal_Construction_Time()
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                Debug.Log("[KCT] Adding Toolbar Button");
                KCT_GameStates.kctToolbarButton = ToolbarManager.Instance.add("Kerbal_Construction_Time", "MainButton");
                if (!KCT_GameStates.settings.enabledForSave) KCT_GameStates.kctToolbarButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER);
                KCT_GameStates.kctToolbarButton.TexturePath = KCT_Utilities.GetButtonTexture();
                KCT_GameStates.kctToolbarButton.ToolTip = "Kerbal Construction Time";
                KCT_GameStates.kctToolbarButton.OnClick += ((e) =>
                {
                    KCT_GUI.onClick();
                });
            }
        }

        public void OnDestroy()//more toolbar stuff
        {
            if (KCT_GameStates.kctToolbarButton != null)
            {
                KCT_GameStates.kctToolbarButton.Destroy();
            }
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
                //Debug.Log("[KCT] Scenario is not null.");
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

            if (KCT_GUI.PrimarilyDisabled && InputLockManager.GetControlLock("KCTLaunchLock") == ControlTypes.EDITOR_LAUNCH)
                InputLockManager.RemoveControlLock("KCTLaunchLock");

            if (!KCT_GameStates.settings.enabledForSave)
                return;

            //Begin primary mod functions
            if (!KCT_Events.instance.eventAdded)
            {
                if (KCT_GameStates.settings.CheckForUpdates) //Check for updates
                    KCT_UpdateChecker.CheckForUpdate(false);

                KCT_Events.instance.addEvents();
            }
            KCT_GameStates.UT = Planetarium.GetUniversalTime();
            
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
                    KCT_GUI.showEditorGUI = true;
                    InputLockManager.SetControlLock(ControlTypes.EDITOR_LAUNCH, "KCTLaunchLock");
                }
                if (KCT_GameStates.EditorShipEditingMode && KCT_GameStates.delayStart)
                {
                    EditorLogic.fetch.shipNameField.Text = KCT_GameStates.editedVessel.shipName;
                    KCT_GameStates.delayStart = false;
                    string tempFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/Ships/temp.craft";
                    EditorLogic.LoadShipFromFile(tempFile);
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

            if (HighLogic.LoadedSceneIsFlight && KCT_GameStates.flightSimulated)
            {
                if (FlightGlobals.ActiveVessel.situation != Vessel.Situations.PRELAUNCH)
                {
                    KCT_GameStates.flightSimulated = false; //If you open, then close the launch window, then go to another flight it won't lock controls
                    //However, if you go to a launch already on the pad through the window, you will have controls locked :/
                    KCT_Utilities.disableSimulationLocks();
                }
                else
                {
                    KCT_Utilities.enableSimulationLocks();
                    moved = false;
                }
            }

            if (HighLogic.LoadedSceneIsFlight && !KCT_GameStates.flightSimulated && FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH
                        && FlightGlobals.ActiveVessel.GetCrewCount() == 0 && KCT_GameStates.launchedCrew.Count > 0)
            {
                CrewRoster roster = HighLogic.CurrentGame.CrewRoster;
                for (int i = 0; i < FlightGlobals.ActiveVessel.parts.Count; i++)
                {
                    Part p = FlightGlobals.ActiveVessel.parts[i];
                    {
                        List<ProtoCrewMember> crewList = KCT_GameStates.launchedCrew[i];
                        foreach (ProtoCrewMember crewMember in crewList)
                        {
                            if (crewMember != null)
                            {
                                ProtoCrewMember finalCrewMember = crewMember;
                                foreach (ProtoCrewMember rosterCrew in roster)
                                {
                                    if (rosterCrew.name == crewMember.name)
                                        finalCrewMember = rosterCrew;
                                }
                                Debug.Log("[KCT] Assigning " + finalCrewMember.name + " to " + p.partInfo.name);
                                p.AddCrewmemberAt(finalCrewMember, crewList.IndexOf(crewMember));
                                finalCrewMember.rosterStatus = ProtoCrewMember.RosterStatus.ASSIGNED;
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
        public void FixedUpdate()
        {
            if (!KCT_GameStates.settings.enabledForSave)
                return;

            KCT_GameStates.UT = Planetarium.GetUniversalTime();
            try
            {
                if (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.TRACKSTATION && !KCT_GameStates.flightSimulated)
                {
                    IKCTBuildItem ikctItem = KCT_Utilities.NextThingToFinish();

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
                            if ((10 * TimeWarp.deltaTime) > Math.Max((ikctItem.GetTimeLeft()), 0) && TimeWarp.CurrentRate > 1.0f)
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
                    else if (ikctItem != null && (KCT_GameStates.warpInitiated || KCT_GameStates.settings.ForceStopWarp) && TimeWarp.CurrentRate != 0 && (ikctItem.GetTimeLeft()) < (TimeWarp.deltaTime*2) && (!ikctItem.IsComplete())) //Still warp down even if we don't control the clock
                    {
                        TimeWarp.SetRate(0, false);
                        KCT_GameStates.warpInitiated = false;
                    }
                }
                if (HighLogic.LoadedSceneIsFlight && !KCT_GameStates.flightSimulated) //Non-simulated flights
                {
                    if (KCT_GameStates.delayStart)
                    {
                        KCT_GUI.hideAll();

                        if (FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH)
                        {
                            KCT_GameStates.launchedVessel.RemoveFromBuildList();
                            FlightGlobals.ActiveVessel.vesselName = KCT_GameStates.launchedVessel.shipName;
                        }

                        List<VesselType> invalidTypes = new List<VesselType> { VesselType.Debris, VesselType.SpaceObject, VesselType.Unknown };
                        if (!invalidTypes.Contains(FlightGlobals.ActiveVessel.vesselType) && !KCT_GameStates.BodiesVisited.Contains(FlightGlobals.ActiveVessel.mainBody.bodyName))
                        {
                            KCT_GameStates.BodiesVisited.Add(FlightGlobals.ActiveVessel.mainBody.bodyName);
                            var message = new ScreenMessage("[KCT] New simulation body unlocked: " + FlightGlobals.ActiveVessel.mainBody.bodyName, 4.0f, ScreenMessageStyle.UPPER_LEFT);
                            ScreenMessages.PostScreenMessage(message, true);
                        }
                        
                        KCT_GameStates.delayStart = false;
                    }
                }
                else if (HighLogic.LoadedScene == GameScenes.FLIGHT && KCT_GameStates.flightSimulated) //Simulated flights
                {
                    if (KCT_GameStates.delayStart)
                    {
                        Debug.Log("[KCT] Simulation started");
                        KCT_GUI.hideAll();
                        KCT_GUI.showSimulationWindow = true;
                        KCT_GUI.showTimeRemaining = true;
                        
                        KCT_GameStates.delayStart=false;
                    }
                    if (FlightGlobals.ActiveVessel.loaded && !FlightGlobals.ActiveVessel.packed && !moved)
                    {
                        moved = true;
                        if (KCT_GameStates.simulateInOrbit)
                        {
                            KCT_OrbitAdjuster.PutInOrbitAround(KCT_GameStates.simulationBody, KCT_GameStates.simOrbitAltitude, KCT_GameStates.simInclination);
                        }
                    }
                    if (KCT_GameStates.simulationEndTime > 0 && KCT_GameStates.UT >= KCT_GameStates.simulationEndTime)
                    {
                        FlightDriver.SetPause(true);
                        KCT_GUI.showSimulationCompleteFlight = true;
                    }
                }
                if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
                {
                    if (KCT_GameStates.delayStart) //Get the ships so that they'll be available later (to avoid issues with loading during flight)
                    {
                        foreach (KCT_BuildListVessel b in KCT_GameStates.VABList)
                            b.GetPartNames();

                        if (KCT_Utilities.CurrentGameIsCareer() && KCT_GameStates.TotalUpgradePoints == 0)
                        {
                            ConfigNode CN = new ConfigNode();
                            ResearchAndDevelopment.Instance.snapshot.Save(CN);
                            ConfigNode[] techNodes = CN.GetNodes("Tech");
                            Debug.Log("[KCT] technodes length: " + techNodes.Length);
                            KCT_GameStates.TotalUpgradePoints = techNodes.Length + 14;
                        }
                        KCT_GameStates.upgradesUpdated = true;
                        KCT_GameStates.delayStart = false;
                    }
                }
                if (HighLogic.LoadedSceneIsEditor)
                {
                    if (KCT_GameStates.delayStart)
                    {
                        KCT_GameStates.delayStart = false;
                        if (KCT_GameStates.EditorShipEditingMode)
                        {
                            Debug.Log("[KCT] Editing " + KCT_GameStates.editedVessel.shipName);
                            EditorLogic.fetch.shipNameField.Text = KCT_GameStates.editedVessel.shipName;
                        }
                    }
                }
                KCT_Utilities.ProgressBuildTime();
            }
            catch (IndexOutOfRangeException e)
            {
                print(e.Message);
                print(e.StackTrace);
            }

        }

        public bool SOIAlert()
        {
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                if (KCT_GameStates.VesselTypesForSOI.Contains(v.vesselType))// && SOITransitions.Contains(v.orbit.patchEndTransition))
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