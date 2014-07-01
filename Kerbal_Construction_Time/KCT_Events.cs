using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Construction_Time
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
            if (!KCT_GameStates.settings.DisableBuildTime)
            {
                GameEvents.onGUILaunchScreenSpawn.Add(launchScreenOpenEvent);
            }
            GameEvents.onVesselRecovered.Add(vesselRecoverEvent);
            GameEvents.onVesselDestroy.Add(vesselDestroyEvent);
            GameEvents.onLaunch.Add(vesselLaunchEvent);
            GameEvents.onGameSceneLoadRequested.Add(gameSceneEvent);
            GameEvents.onVesselSOIChanged.Add(SOIChangeEvent);
            GameEvents.OnTechnologyResearched.Add(TechUnlockEvent);

            eventAdded = true;
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
            List<GameScenes> validScenes = new List<GameScenes> { GameScenes.SPACECENTER, GameScenes.TRACKSTATION, GameScenes.SPH, GameScenes.EDITOR };
            if (validScenes.Contains(scene))
            {
                //Check for simulation save and load it.
                if (System.IO.File.Exists(KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs"))
                {
                    KCT_Utilities.LoadSimulationSave();
                }
            }
            if (!HighLogic.LoadedSceneIsFlight && scene == GameScenes.FLIGHT && KCT_GameStates.flightSimulated) //Backup save at simulation start
            {
                KCT_Utilities.MakeSimulationSave();
            }

            if (HighLogic.LoadedScene == scene && (scene == GameScenes.EDITOR || scene == GameScenes.SPH)) //Fix for null reference when using new or load buttons in editor
            {
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
            }
        }

        public void SOIChangeEvent(GameEvents.HostedFromToAction<Vessel, CelestialBody> ev)
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
            if (KCT_GameStates.flightSimulated && KCT_GameStates.settings.SimulationTimeLimit > 0)
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
                            area = Mathf.PI * Mathf.Pow(area / 2, 2); //Determine the area manually since the "deployedArea" parameter no longer exists in RC
                            Debug.Log("Chute area: " + area);

                            member = rCType.GetMember("material")[0];
                            string mat = (string)KCT_Utilities.GetMemberInfoValue(member, realChute);
                            Debug.Log("Material is " + mat);

                            Type matLibraryType = AssemblyLoader.loadedAssemblies
                                .SelectMany(a => a.assembly.GetExportedTypes())
                                .SingleOrDefault(t => t.FullName == "RealChute.Libraries.MaterialsLibrary");

                            System.Reflection.MethodInfo matMethod = matLibraryType.GetMethod("GetMaterial", new Type[] { mat.GetType() });
                            object MatLibraryInstance = matLibraryType.GetProperty("instance").GetValue(null, null);
                            object materialObject = matMethod.Invoke(MatLibraryInstance, new object[] { mat });

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
    }
}
