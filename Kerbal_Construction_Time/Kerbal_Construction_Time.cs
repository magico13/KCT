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
                ConfigNode CN = new ConfigNode();
                KCT_GameStates.VABList[i].shipNode.CopyTo(CN, "VAB" + i);
                node.AddNode(CN);
            }
            for (int i = 0; i < KCT_GameStates.SPHList.Count; i++)
            {
                ConfigNode CN = new ConfigNode();
                KCT_GameStates.SPHList[i].shipNode.CopyTo(CN, "SPH" + i);
                node.AddNode(CN);
            }
            for (int i = 0; i < KCT_GameStates.VABWarehouse.Count; i++)
            {
                ConfigNode CN = new ConfigNode();
                KCT_GameStates.VABWarehouse[i].shipNode.CopyTo(CN, "VABWH" + i);
                node.AddNode(CN);
            }
            for (int i = 0; i < KCT_GameStates.SPHWarehouse.Count; i++)
            {
                ConfigNode CN = new ConfigNode();
                KCT_GameStates.SPHWarehouse[i].shipNode.CopyTo(CN, "SPHWH" + i);
                node.AddNode(CN);
            }
            for (int i=0; i< KCT_GameStates.TechList.Count; i++)
            {
                ConfigNode CN = new ConfigNode("Tech"+i);
                KCT_GameStates.TechList[i].protoNode.Save(CN);
                node.AddNode(CN);
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

            //if (HighLogic.LoadedScene == GameScenes.FLIGHT)
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

        private static bool eventAdded = false;
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
                    Debug.Log("[KCT[ Adding InternalModule scenario to game '" + HighLogic.CurrentGame.Title + "'");
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
            if (!eventAdded)
            {
                if (KCT_GameStates.settings.CheckForUpdates) //Check for updates
                    KCT_UpdateChecker.CheckForUpdate(false);

                //addToolbarButton();
                if (!KCT_GameStates.settings.DisableBuildTime)
                {
                    GameEvents.onGUILaunchScreenSpawn.Add(launchScreenOpenEvent);
                }
                GameEvents.onVesselRecovered.Add(vesselRecoverEvent);
                GameEvents.onVesselDestroy.Add(vesselDestroyEvent);
                GameEvents.onLaunch.Add(vesselLaunchEvent);
                //GameEvents.onFlightReady.Add(flightReadyEvent);
                GameEvents.onGameSceneLoadRequested.Add(gameSceneEvent);
                GameEvents.onVesselSOIChanged.Add(SOIChangeEvent);
                GameEvents.OnTechnologyResearched.Add(TechUnlockEvent);

                eventAdded = true;
            }
            KCT_GameStates.UT = Planetarium.GetUniversalTime();
            
            if ((HighLogic.LoadedScene == GameScenes.EDITOR) || (HighLogic.LoadedScene == GameScenes.SPH))
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
           /* else if (HighLogic.LoadedSceneIsFlight && !KCT_GameStates.flightSimulated)
            {

            }*/

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
                    //if (KCT_GameStates.launchedCrew.ContainsKey(p.uid))
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

        public void TechUnlockEvent(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> ev)
        {
            if (ev.target == RDTech.OperationResult.Successful)
            {
                KCT_TechItem tech = new KCT_TechItem(ev.host);
                if (!KCT_GameStates.settings.InstantTechUnlock && !KCT_GameStates.settings.DisableBuildTime) tech.DisableTech();
                if (!tech.isInList())
                {
                    ++KCT_GameStates.TotalUpgradePoints;
                    var message = new ScreenMessage("[KCT] Upgrade Point Added!", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                    ScreenMessages.PostScreenMessage(message, true);

                    if (!KCT_GameStates.settings.InstantTechUnlock && !KCT_GameStates.settings.DisableBuildTime)
                    {
                        KCT_GameStates.TechList.Add(tech);
                        message = new ScreenMessage("[KCT] Node will unlock in " + tech.TimeLeft / 86400 + " days.", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                        ScreenMessages.PostScreenMessage(message, true);
                    }
                }
                else
                {
                    ResearchAndDevelopment.Instance.Science += tech.scienceCost;
                    var message = new ScreenMessage("[KCT] This node is already being researched!", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                    ScreenMessages.PostScreenMessage(message, true);
                }
            }
        }

        public void gameSceneEvent(GameScenes scene)
        {
            List<GameScenes> validScenes = new List<GameScenes> {GameScenes.SPACECENTER, GameScenes.TRACKSTATION, GameScenes.SPH, GameScenes.EDITOR};
            if (validScenes.Contains(scene))
            {
                //Check for simulation save and load it.
                if (System.IO.File.Exists(KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs"))
                {
                    KCT_Utilities.LoadSimulationSave();
                }
            }
            if (HighLogic.LoadedScene != GameScenes.FLIGHT && scene == GameScenes.FLIGHT && KCT_GameStates.flightSimulated) //Backup save at simulation start
            {
                KCT_Utilities.MakeSimulationSave();
            }

            if (HighLogic.LoadedScene == scene && (scene == GameScenes.EDITOR || scene == GameScenes.SPH)) //Fix for null reference when using new or load buttons in editor
            {
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE); 
            }
        }

        public void SOIChangeEvent(GameEvents.HostedFromToAction<Vessel,CelestialBody> ev)
        {
            List<VesselType> invalidTypes = new List<VesselType> { VesselType.Debris, VesselType.SpaceObject, VesselType.Unknown };
            if (!invalidTypes.Contains(ev.host.vesselType) && !KCT_GameStates.BodiesVisited.Contains(ev.to.bodyName) && !KCT_GameStates.flightSimulated)
            {
                KCT_GameStates.BodiesVisited.Add(ev.to.bodyName);
                var message = new ScreenMessage("[KCT] New simulation body unlocked: " + ev.to.bodyName, 4.0f, ScreenMessageStyle.UPPER_LEFT);
                ScreenMessages.PostScreenMessage(message, true);
            }
        }

        public void launchScreenOpenEvent(GameEvents.VesselSpawnInfo v)
        {
            KCT_GameStates.flightSimulated = true;
        }

        public void vesselLaunchEvent(EventReport e)
        {
            if (KCT_GameStates.flightSimulated && KCT_GameStates.settings.SimulationTimeLimit>0)
            {
                KCT_GameStates.simulationEndTime = Planetarium.GetUniversalTime() + (KCT_GameStates.settings.SimulationTimeLimit);
            }
        }

        public void vesselRecoverEvent(ProtoVessel v)
        {
            if (!KCT_GameStates.flightSimulated && !v.vesselRef.isEVA)
            {
                Debug.Log("[KCT] Adding recovered parts to Part Inventory");
                foreach (ProtoPartSnapshot p in v.protoPartSnapshots)
                {
                    //Debug.Log(p.partInfo.name);
                    KCT_Utilities.AddPartToInventory(p.partInfo.name);
                }
            }
        }

        public void vesselDestroyEvent(Vessel v)
        {
            if (v != null && !(HighLogic.LoadedSceneIsFlight && v.isActiveVessel) && v.mainBody.bodyName == "Kerbin" && (!v.loaded || v.packed) && v.altitude < 35000 &&
               (v.situation == Vessel.Situations.FLYING || v.situation == Vessel.Situations.SUB_ORBITAL) && !v.isEVA)
            {
                double totalMass = 0;
                double dragCoeff = 0;
                //double totalArea = 0;
                bool realChuteInUse = false;

                float totalDrag = 0;

                if (!v.packed) //adopted from mission controller. Not sure why they have to be packed
                    foreach (Part p in v.Parts)
                        p.Pack();

               if (v.protoVessel == null)
                    return;
                foreach (ProtoPartSnapshot p in v.protoVessel.protoPartSnapshots)
                {
                    //Debug.Log("[KCT] Has part " + p.partName + ", mass " + p.mass);
                    List<string> ModuleNames = new List<string>();
                    foreach (ProtoPartModuleSnapshot ppms in p.modules)
                    {
                        //Debug.Log(ppms.moduleName);
                        ModuleNames.Add(ppms.moduleName);
                    }
                    totalMass += p.mass;
                    bool isParachute = false;
                    if (ModuleNames.Contains("ModuleParachute"))
                    {
                        Debug.Log("[KCT] Found parachute module on " + p.partInfo.name);
                        ModuleParachute mp = (ModuleParachute)p.modules.First(mod => mod.moduleName == "ModuleParachute").moduleRef;
                        dragCoeff += p.mass * mp.fullyDeployedDrag;
                        isParachute = true;
                    }
                    if (ModuleNames.Contains("RealChuteModule"))
                    {
                        Debug.Log("[KCT] Found realchute module on " + p.partInfo.name);
                        PartModule realChute = p.modules.First(mod => mod.moduleName == "RealChuteModule").moduleRef;//p.partRef.Modules["RealChuteModule"];
                        Type rCType = realChute.GetType();
                        if ((object)realChute != null)
                        {
                            System.Reflection.MemberInfo member = rCType.GetMember("deployedDiameter")[0];
                            float area = (float)KCT_Utilities.GetMemberInfoValue(member, realChute);
                            area = Mathf.PI*Mathf.Pow(area/2, 2); //Determine the area manually since the "deployedArea" parameter no longer exists in RC
                            Debug.Log("Chute area: " + area);
                            
                            member = rCType.GetMember("material")[0];
                            string mat = (string)KCT_Utilities.GetMemberInfoValue(member, realChute);
                            Debug.Log("Material is "+mat);

                            Type matLibraryType = AssemblyLoader.loadedAssemblies
                                .SelectMany(a => a.assembly.GetExportedTypes())
                                .SingleOrDefault(t => t.FullName == "RealChute.Libraries.MaterialsLibrary");
                            
                            System.Reflection.MethodInfo matMethod = matLibraryType.GetMethod("GetMaterial", new Type[] { mat.GetType() });
                            object MatLibraryInstance = matLibraryType.GetProperty("instance").GetValue(null, null);
                            object materialObject = matMethod.Invoke(MatLibraryInstance, new object[]{mat});

                            float dragC = (float)KCT_Utilities.GetMemberInfoValue(materialObject.GetType().GetMember("dragCoefficient")[0], materialObject);
                            Debug.Log("dragC: " + dragC);
                            isParachute = true;
                            realChuteInUse = true;
                            totalDrag += (1 * 100 * dragC * area / 2000f);
                        }
                    }
                    if (!isParachute)
                    {
                        dragCoeff += p.mass * 0.2;
                    }
                }
                double Vt = 9999;
                if (!realChuteInUse)
                {
                    dragCoeff = dragCoeff / (totalMass);
                    Vt = Math.Sqrt(250 * 6.674e-11 * 5.2915793e22 / (((600000) ^ 2) * 1.22309485 * dragCoeff)) / 1000; //Not sure if this is right, but it seems to be close enough.
                    Debug.Log("[KCT] Using Stock Module! Drag: " + dragCoeff + " Vt: " + Vt);
                }
                else
                {
                    Debug.Log("[KCT] Using RealChute Module! Drag/Mass ratio: " + (totalDrag / totalMass));
                    //Debug.Log("[KCT] " + v.atmDensity);
                    if ((totalDrag / totalMass) >= 8) //Once again, not sure if this is right, but it seems possibly correct from limited testing.
                    {
                        Vt = 0;
                    }
                }
                if (Vt < 10.0)
                {
                    Debug.Log("[KCT] Recovered parts from " + v.vesselName);
                    foreach (ProtoPartSnapshot p in v.protoVessel.protoPartSnapshots)
                    {
                        Debug.Log("[KCT] " + p.partInfo.name);
                        KCT_Utilities.AddPartToInventory(p.partInfo.name);
                    }
                }
            }
        }

        private void revertToEditor(String reason)
        {
            KCT_GameStates.simulationReason = reason;
            Debug.Log("[KCT] Simulation complete: " + reason);
            // FlightDriver.SetPause(true);
            KCT_GUI.showSimulationCompleteEditor = true;
            KCT_Utilities.disableSimulationLocks();
            KCT_GameStates.flightSimulated = false;
            KCT_GameStates.simulationEndTime = 0;
            if (MCEWrapper.MCEAvailable) //Support for MCE
                MCEWrapper.IloadMCEbackup();
            if (FlightDriver.LaunchSiteName == "LaunchPad")
                FlightDriver.RevertToPrelaunch(GameScenes.EDITOR);
            else if (FlightDriver.LaunchSiteName == "Runway")
                FlightDriver.RevertToPrelaunch(GameScenes.SPH);
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
                        //KCT_OrbitAdjuster.PutInOrbitAround("Moho", 100000);
                        moved = true;
                        if (KCT_GameStates.simulateInOrbit)
                        {
                            KCT_OrbitAdjuster.PutInOrbitAround(KCT_GameStates.simulationBody, KCT_GameStates.simOrbitAltitude, KCT_GameStates.simInclination);
                        }
                    }
                  /*  if (KCT_GameStates.activeVessel.vessel.GetOrbit().ApA > 250000 || KCT_GameStates.activeVessel.vessel.GetOrbit().PeA > 70000)
                    {
                        //REVERT TO EDITOR
                        if (KCT_GameStates.activeVessel.vessel.GetOrbit().ApA > 250000)
                            revertToEditor("APOAPSIS");
                        else if (KCT_GameStates.activeVessel.vessel.GetOrbit().PeA > 70000)
                            revertToEditor("PERIAPSIS");
                    }*/
                    if (KCT_GameStates.simulationEndTime > 0 && KCT_GameStates.UT >= KCT_GameStates.simulationEndTime)
                    {
                        FlightDriver.SetPause(true);
                        KCT_GUI.showSimulationCompleteFlight = true;
                      //  revertToEditor("TIME");
                    }
                    /*if (FlightGlobals.fetch.activeVessel.state == Vessel.State.DEAD) //No longer needed. Any selection will work properly.
                    {
                        if (KCT_GameStates.settings.AutoRevertOnCrash)
                            revertToEditor("CRASHED");
                        else
                        {
                            FlightDriver.SetPause(true);
                            KCT_GUI.showSimulationCompleteFlight = true;
                        }
                    }*/
                    
                }
                if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
                {
                    if (KCT_GameStates.delayStart) //Get the ships so that they'll be available later (to avoid issues with loading during flight)
                    {
                        foreach (KCT_BuildListVessel b in KCT_GameStates.VABList)
                            b.GetPartNames();
                        
                       /* foreach (KCT_BuildListVessel b in KCT_GameStates.VABList)
                            b.GetShip();
                        foreach (KCT_BuildListVessel b in KCT_GameStates.SPHList)
                            b.GetShip();
                        foreach (KCT_BuildListVessel b in KCT_GameStates.VABWarehouse)
                            b.GetShip();
                        foreach (KCT_BuildListVessel b in KCT_GameStates.SPHWarehouse)
                            b.GetShip();*/


                        if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER && KCT_GameStates.TotalUpgradePoints == 0)
                        {
                            ConfigNode CN = new ConfigNode();
                            ResearchAndDevelopment.Instance.snapshot.Save(CN);
                            ConfigNode[] techNodes = CN.GetNodes("Tech");
                            Debug.Log("[KCT] technodes length: " + techNodes.Length);
                            KCT_GameStates.TotalUpgradePoints = techNodes.Length - 1;
                        }
                        KCT_GameStates.delayStart = false;
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