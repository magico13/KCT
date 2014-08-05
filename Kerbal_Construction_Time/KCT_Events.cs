using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

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

            if (!StageRecoveryWrapper.StageRecoveryAvailable)
                GameEvents.onVesselDestroy.Add(vesselDestroyEvent);
            else
            {
                Debug.Log("[KCT] Deferring stage recovery to StageRecovery. It will just provide me with part names.");
                StageRecoveryWrapper.AddRecoverySuccessEvent(StageRecoverySuccessEvent);
            }

            GameEvents.onLaunch.Add(vesselLaunchEvent);
            GameEvents.onGameSceneLoadRequested.Add(gameSceneEvent);
            GameEvents.onVesselSOIChanged.Add(SOIChangeEvent);
            GameEvents.OnTechnologyResearched.Add(TechUnlockEvent);
            if (!ToolbarManager.ToolbarAvailable)
                GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);

            eventAdded = true;
        }

        private void StageRecoverySuccessEvent(Vessel v, float[] infoArray, string reason)
        {
            Debug.Log("[KCT] Recovery Success Event triggered.");
            float damage = 0;
            if (infoArray.Length == 3)
                damage = infoArray[0];
            else
                Debug.Log("[KCT] Malformed infoArray received!");
            System.Random rand = new System.Random();
            //Debug.Log("[KCT] dmg:" + damage);
            //Debug.Log("[KCT] vessel parts count: " + v.protoVessel.protoPartSnapshots.Count);
            //List<Part> parts = v.parts;
            foreach (ProtoPartSnapshot part in v.protoVessel.protoPartSnapshots)
            {
                float random = (float)rand.NextDouble();
                //Debug.Log("[KCT] rand:" + random + " dmg:" + damage);
                if (random < damage)
                    KCT_Utilities.AddPartToInventory(part.partInfo.name);
                else
                    Debug.Log("[KCT] Part "+part.partInfo.name+" was too damaged to be used anymore and was scrapped!");
            }
        }

        public ApplicationLauncherButton KCTButtonStock = null;
        public void OnGUIAppLauncherReady()
        {
            bool vis;
            if (ApplicationLauncher.Ready && (KCTButtonStock == null || !ApplicationLauncher.Instance.Contains(KCTButtonStock, out vis))) //Add Stock button
            {
                KCTButtonStock = ApplicationLauncher.Instance.AddModApplication(
                    KCT_GUI.onClick,
                    KCT_GUI.onClick,
                    DummyVoid, //TODO: List next ship here?
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    GameDatabase.Instance.GetTexture("KerbalConstructionTime/icons/KCT_on", false));
            }
        }
        void DummyVoid() { }

        public void TechUnlockEvent(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> ev)
        {
            if (!KCT_GameStates.settings.enabledForSave) return;
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
                        message = new ScreenMessage("[KCT] Node will unlock in " + KCT_Utilities.GetFormattedTime(tech.TimeLeft), 4.0f, ScreenMessageStyle.UPPER_LEFT);
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
            if (!KCT_GameStates.settings.enabledForSave) return;
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

            if (scene == GameScenes.MAINMENU)
            {
                KCT_GameStates.reset();
                KCT_GameStates.firstStart = true;
                KCT_Utilities.disableSimulationLocks();
                InputLockManager.RemoveControlLock("KCTLaunchLock");
            }
            if (HighLogic.LoadedSceneIsEditor)
            {
                EditorLogic.fetch.Unlock("KCTEditorMouseLock");
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
            if (!KCT_GUI.PrimarilyDisabled)
                KCT_GameStates.flightSimulated = true;
        }

        public void vesselLaunchEvent(EventReport e)
        {
            if (!KCT_GameStates.settings.enabledForSave) return;
            if (KCT_GameStates.flightSimulated && KCT_GameStates.simulationTimeLimit > 0)
            {
                KCT_GameStates.simulationEndTime = Planetarium.GetUniversalTime() + (KCT_GameStates.simulationTimeLimit);
            }
        }

        public void vesselRecoverEvent(ProtoVessel v)
        {
            if (!KCT_GameStates.settings.enabledForSave) return;
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
            Dictionary<string, int> PartsRecovered = new Dictionary<string, int>();
            float FundsRecovered = 0, KSCDistance = 0, RecoveryPercent = 0;
            StringBuilder Message = new StringBuilder();

            if (FlightGlobals.fetch == null)
                return;

            if (v != null && !(HighLogic.LoadedSceneIsFlight && v.isActiveVessel) && v.mainBody.bodyName == "Kerbin" && (!v.loaded || v.packed) && v.altitude < 35000 &&
               (v.situation == Vessel.Situations.FLYING || v.situation == Vessel.Situations.SUB_ORBITAL) && !v.isEVA)
            {
                double totalMass = 0;
                double dragCoeff = 0;
                //double totalArea = 0;
                bool realChuteInUse = false;

                float RCParameter = 0;

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
                        //PartModule realChute = p.modules.First(mod => mod.moduleName == "RealChuteModule").moduleRef;//p.partRef.Modules["RealChuteModule"];
                        ProtoPartModuleSnapshot realChute = p.modules.First(mod => mod.moduleName == "RealChuteModule");
                        /*  Type rCType = realChute.moduleRef.GetType();
                          Debug.Log(rCType.ToString());*/
                        if ((object)realChute != null) //Some of this was adopted from DebRefund, as Vendan's method of handling multiple parachutes is better than what I had.
                        {
                            Type matLibraryType = AssemblyLoader.loadedAssemblies
                                .SelectMany(a => a.assembly.GetExportedTypes())
                                .SingleOrDefault(t => t.FullName == "RealChute.Libraries.MaterialsLibrary");

                            ConfigNode[] parchutes = realChute.moduleValues.GetNodes("PARACHUTE");
                            foreach (ConfigNode chute in parchutes)
                            {
                                float diameter = float.Parse(chute.GetValue("deployedDiameter"));
                                //area = (float)(Math.Pow(area / 2, 2) * Math.PI);
                                //Debug.Log(area);
                                string mat = chute.GetValue("material");
                                // Debug.Log(mat);
                                System.Reflection.MethodInfo matMethod = matLibraryType.GetMethod("GetMaterial", new Type[] { mat.GetType() });
                                object MatLibraryInstance = matLibraryType.GetProperty("instance").GetValue(null, null);
                                object materialObject = matMethod.Invoke(MatLibraryInstance, new object[] { mat });
                                float dragC = (float)KCT_Utilities.GetMemberInfoValue(materialObject.GetType().GetMember("dragCoefficient")[0], materialObject);
                                // Debug.Log(dragC);
                                //totalDrag += (1 * 100 * dragC * area / 2000f);
                                RCParameter += dragC * (float)Math.Pow(diameter, 2);

                            }
                            isParachute = true;
                            realChuteInUse = true;
                        }
                    }
                    if (!isParachute)
                    {
                        if (p.partRef != null)
                            dragCoeff += p.mass * p.partRef.maximum_drag;
                        else
                            dragCoeff += p.mass * 0.2;
                    }
                }
                double Vt = double.MaxValue;
                if (!realChuteInUse)
                {
                    dragCoeff = dragCoeff / (totalMass);
                    Vt = Math.Sqrt((250 * 6.674E-11 * 5.2915793E22) / (3.6E11 * 1.22309485 * dragCoeff));
                    Debug.Log("[KCT] Using Stock Module! Drag: " + dragCoeff + " Vt: " + Vt);
                }
                else
                {
                    Vt = Math.Sqrt((8000 * totalMass * 9.8) / (1.223 * Math.PI) * Math.Pow(RCParameter, -1)); //This should work perfect for multiple identical chutes and gives an approximation for multiple differing chutes
                    Debug.Log("[SR] Using RealChute Module! Vt: " + Vt);
                }
                if (Vt < 10.0)
                {
                    Debug.Log("[KCT] Recovered parts from " + v.vesselName);
                    foreach (ProtoPartSnapshot p in v.protoVessel.protoPartSnapshots)
                    {
                        //Debug.Log("[KCT] " + p.partInfo.name);
                        KCT_Utilities.AddPartToInventory(p.partInfo.name);
                        if (!PartsRecovered.ContainsKey(p.partInfo.title))
                            PartsRecovered.Add(p.partInfo.title, 1);
                        else
                            ++PartsRecovered[p.partInfo.title];
                    }

                    Message.AppendLine("Vessel name: " + v.vesselName);
                    Message.AppendLine("Parts recovered: ");
                    for (int i = 0; i < PartsRecovered.Count; i++)
                    {
                        Message.AppendLine(PartsRecovered.Values.ElementAt(i) + "x " + PartsRecovered.Keys.ElementAt(i));
                    }

                    if (KCT_Utilities.CurrentGameIsCareer())
                    {
                        if (KCT_Utilities.StageRecoveryAddonActive || KCT_Utilities.DebRefundAddonActive) //Delegate funds handling to Stage Recovery or DebRefund if it's present
                        {
                            Debug.Log("[KCT] Delegating Funds recovery to another addon.");
                        }
                        else  //Otherwise do it ourselves
                        {
                            bool probeCoreAttached = false;
                            foreach (ProtoPartSnapshot pps in v.protoVessel.protoPartSnapshots)
                            {
                                if (pps.modules.Find(module => (module.moduleName == "ModuleCommand" && ((ModuleCommand)module.moduleRef).minimumCrew == 0)) != null)
                                {
                                    Debug.Log("[KCT] Probe Core found!");
                                    probeCoreAttached = true;
                                    break;
                                }
                            }
                            float RecoveryMod = probeCoreAttached ? 1.0f : KCT_GameStates.settings.RecoveryModifier;
                            KSCDistance = (float)SpaceCenter.Instance.GreatCircleDistance(SpaceCenter.Instance.cb.GetRelSurfaceNVector(v.protoVessel.latitude, v.protoVessel.longitude));
                            double maxDist = SpaceCenter.Instance.cb.Radius * Math.PI;
                            RecoveryPercent = RecoveryMod * Mathf.Lerp(0.98f, 0.1f, (float)(KSCDistance / maxDist));
                            float totalReturn = 0;
                            foreach (ProtoPartSnapshot pps in v.protoVessel.protoPartSnapshots)
                            {
                                float dryCost, fuelCost;
                                totalReturn += Math.Max(ShipConstruction.GetPartCosts(pps, pps.partInfo, out dryCost, out fuelCost), 0);
                            }
                            float totalBeforeModifier = totalReturn;
                            totalReturn *= RecoveryPercent;
                            FundsRecovered = totalReturn;
                            Debug.Log("[KCT] Vessel being recovered by KCT. Percent returned: " + 100 * RecoveryPercent + "%. Distance from KSC: " + Math.Round(KSCDistance / 1000, 2) + " km");
                            Debug.Log("[KCT] Funds being returned: " + Math.Round(totalReturn, 2) + "/" + Math.Round(totalBeforeModifier, 2));

                            Message.AppendLine("Funds recovered: " + FundsRecovered + "(" + Math.Round(RecoveryPercent * 100, 1) + "%)");
                            KCT_Utilities.AddFunds(FundsRecovered);
                            /*FundsRecovered = KCT_Utilities.GetRecoveryValueForChuteLanding(v.protoVessel);
                            
                            */
                        }
                    }
                    Message.AppendLine("\nAdditional information:");
                    Message.AppendLine("Distance from KSC: " + Math.Round(KSCDistance / 1000, 2) + " km");
                    if (!realChuteInUse)
                    {
                        Message.AppendLine("Stock module used. Terminal velocity (less than 10 needed): " + Math.Round(Vt, 2));
                    }
                    else
                    {
                        Message.AppendLine("RealChute module used. Terminal velocity (less than 10 needed): " + Math.Round(Vt, 2));
                    }
                    if (!(KCT_Utilities.StageRecoveryAddonActive || KCT_Utilities.DebRefundAddonActive) &&
                        (KCT_Utilities.CurrentGameIsCareer() || !KCT_GUI.PrimarilyDisabled) &&
                        !(KCT_GameStates.settings.DisableAllMessages || KCT_GameStates.settings.DisableRecoveryMessages))
                    {
                        KCT_Utilities.DisplayMessage("Stage Recovered", Message, MessageSystemButton.MessageButtonColor.BLUE, MessageSystemButton.ButtonIcons.MESSAGE);
                    }
                }
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