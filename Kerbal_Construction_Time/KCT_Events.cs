using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

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

            if (!StageRecoveryWrapper.StageRecoveryAvailable)
                GameEvents.onVesselDestroy.Add(vesselDestroyEvent);
            else
            {
                KCTDebug.Log("Deferring stage recovery to StageRecovery.");
                StageRecoveryWrapper.AddRecoverySuccessEvent(StageRecoverySuccessEvent);
            }

            //GameEvents.onLaunch.Add(vesselSituationChange);
            GameEvents.onVesselSituationChange.Add(vesselSituationChange);
            GameEvents.onGameSceneLoadRequested.Add(gameSceneEvent);
            GameEvents.onVesselSOIChanged.Add(SOIChangeEvent);
            GameEvents.OnTechnologyResearched.Add(TechUnlockEvent);
            //if (!ToolbarManager.ToolbarAvailable || !KCT_GameStates.settings.PreferBlizzyToolbar)
                GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onEditorShipModified.Add(ShipModifiedEvent);
            GameEvents.OnPartPurchased.Add(PartPurchasedEvent);
            GameEvents.OnVesselRecoveryRequested.Add(RecoveryRequested);
            GameEvents.onGUIRnDComplexDespawn.Add(TechDisableEvent);
            GameEvents.OnKSCFacilityUpgraded.Add(FacilityUpgradedEvent);
       //     GameEvents.OnKSCStructureRepairing.Add(FacilityRepairingEvent);

            eventAdded = true;
        }

        //private static int lastLvl = -1;
        public static bool allowedToUpgrade = false;
        public void FacilityUpgradedEvent(Upgradeables.UpgradeableFacility facility, int lvl)
        {
            if (KCT_GUI.PrimarilyDisabled)
                return;


            if (!allowedToUpgrade)
            {
                KCT_UpgradingBuilding upgrading = new KCT_UpgradingBuilding(facility, lvl, lvl - 1, facility.id.Split('/').Last());
                
                if (!upgrading.AlreadyInProgress())
                {
                    upgrading.Downgrade();
                    double cost = facility.GetUpgradeCost();
                    upgrading.SetBP(cost);
                    upgrading.cost = cost;
                    KCT_GameStates.ActiveKSC.KSCTech.Add(upgrading);
                    ScreenMessages.PostScreenMessage("Facility upgrade requested!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    KCTDebug.Log("Facility " + facility.id + " upgrade requested to lvl " + lvl + " for " + cost + " funds, resulting in a BP of " + upgrading.BP);
                }
                else if (lvl != upgrading.currentLevel)
                {
                    //
                    KCT_UpgradingBuilding listBuilding = upgrading.KSC.KSCTech.Find(b => b.id == upgrading.id);
                    listBuilding.Downgrade();
                    KCT_Utilities.AddFunds(listBuilding.cost, TransactionReasons.None);
                    ScreenMessages.PostScreenMessage("Facility is already being upgraded!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    KCTDebug.Log("Facility " + facility.id + " tried to upgrade to lvl " + lvl + " but already in list!");
                }
            }
            else
            {
                KCTDebug.Log("Facility " + facility.id + " upgraded to lvl " + lvl);
                allowedToUpgrade = false;
            }
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
            double BP = Math.Sqrt(cost) * 2000 * KCT_GameStates.timeSettings.OverallMultiplier;
            KCTDebug.Log("Facility being repaired for " + cost + " funds, resulting in a BP of " + BP);
            facility.StopCoroutine("Repair");
        }

        public void RecoveryRequested (Vessel v)
        {
            //ShipBackup backup = ShipAssembly.MakeVesselBackup(v);
            //string tempFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/Ships/temp2.craft";
            //backup.SaveShip(tempFile);

           // KCT_GameStates.recoveryRequestVessel = backup; //ConfigNode.Load(tempFile);
        }

        private void StageRecoverySuccessEvent(Vessel v, float[] infoArray, string reason)
        {
            if (!KCT_GameStates.settings.enabledForSave) return;
            KCTDebug.Log("Recovery Success Event triggered.");
            float damage = 0;
            if (infoArray.Length == 3)
                damage = infoArray[0];
            else
                KCTDebug.Log("Malformed infoArray received!");
            System.Random rand = new System.Random();
            Dictionary<string, int> destroyed = new Dictionary<string,int>();
            foreach (ProtoPartSnapshot part in v.protoVessel.protoPartSnapshots)
            {
                float random = (float)rand.NextDouble();
               // string name = part.partInfo.name + KCT_Utilities.GetTweakScaleSize(part);
                if (random < damage)
                {
                    KCT_Utilities.AddPartToInventory(part);
                }
                else
                {
                    string commonName = part.partInfo.title + KCT_Utilities.GetTweakScaleSize(part);
                    Debug.Log("[KCT] Part " + commonName + " was too damaged to be used anymore and was scrapped! Chance: "+damage);
                    if (!destroyed.ContainsKey(commonName))
                        destroyed.Add(commonName, 1);
                    else
                        ++destroyed[commonName];
                }
            }

            if (destroyed.Count > 0 && !KCT_GameStates.settings.DisableAllMessages)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("The following parts were too damaged to be reused and were scrapped:");
                foreach (KeyValuePair<string, int> entry in destroyed) msg.AppendLine(entry.Value+" x "+entry.Key);
                msg.AppendLine("\nChance of failure: " + Math.Round(100 * damage) + "%");
                KCT_Utilities.DisplayMessage("KCT: Parts Scrapped", msg, MessageSystemButton.MessageButtonColor.ORANGE, MessageSystemButton.ButtonIcons.ALERT);
            }
        }

        private void ShipModifiedEvent(ShipConstruct vessel)
        {
            KerbalConstructionTime instance = (KerbalConstructionTime)KerbalConstructionTime.instance;
            instance.editorRecalcuationRequired = true;
        }

        public ApplicationLauncherButton KCTButtonStock = null;
        public void OnGUIAppLauncherReady()
        {
            bool vis;
            if (ToolbarManager.ToolbarAvailable && KCT_GameStates.settings.PreferBlizzyToolbar)
                return;
                
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
                        GameDatabase.Instance.GetTexture("KerbalConstructionTime/Icons/KCT_on", false));

                //ApplicationLauncher.Instance.EnableMutuallyExclusive(KCTButtonStock);
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
            if (!KCT_GameStates.settings.enabledForSave) return;
            if (ev.target == RDTech.OperationResult.Successful)
            {
                KCT_TechItem tech = new KCT_TechItem();
                if (ev.host != null) 
                    tech = new KCT_TechItem(ev.host);

                //if (!KCT_GameStates.settings.InstantTechUnlock && !KCT_GameStates.settings.DisableBuildTime) tech.DisableTech();
                if (!tech.isInList())
                {
                    ++KCT_GameStates.TotalUpgradePoints;
                    ScreenMessages.PostScreenMessage("[KCT] Upgrade Point Added!", 4.0f, ScreenMessageStyle.UPPER_LEFT);

                    if (!KCT_GameStates.settings.InstantTechUnlock && !KCT_GameStates.settings.DisableBuildTime)
                    {
                        KCT_GameStates.TechList.Add(tech);
                        ScreenMessages.PostScreenMessage("[KCT] Node will unlock in " + KCT_Utilities.GetFormattedTime(tech.TimeLeft), 4.0f, ScreenMessageStyle.UPPER_LEFT);
                    }
                }
                else
                {
                    ResearchAndDevelopment.Instance.AddScience(tech.scienceCost, TransactionReasons.RnDTechResearch);
                    ScreenMessages.PostScreenMessage("[KCT] This node is already being researched!", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                    ScreenMessages.PostScreenMessage("[KCT] It will unlock in " + KCT_Utilities.GetFormattedTime((KCT_GameStates.TechList.First(t => t.techID == ev.host.techID)).TimeLeft), 4.0f, ScreenMessageStyle.UPPER_LEFT);
                }
            }
        }

        public void TechDisableEvent()
        {
            if (!KCT_GameStates.settings.InstantTechUnlock && !KCT_GameStates.settings.DisableBuildTime)
            {
                foreach (KCT_TechItem tech in KCT_GameStates.TechList)
                {
                    tech.DisableTech();
                }
            }
        }

        public void gameSceneEvent(GameScenes scene)
        {
            if (!KCT_GameStates.settings.enabledForSave) return;
            List<GameScenes> validScenes = new List<GameScenes> { GameScenes.SPACECENTER, GameScenes.TRACKSTATION, GameScenes.EDITOR };
            if (validScenes.Contains(scene))
            {
                //Check for simulation save and load it.
                if (System.IO.File.Exists(KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/KCT_simulation_backup.sfs"))
                {
                    KCT_Utilities.LoadSimulationSave();
                }
                TechDisableEvent();
            }
            if (!HighLogic.LoadedSceneIsFlight && scene == GameScenes.FLIGHT && KCT_GameStates.flightSimulated) //Backup save at simulation start
            {
                KCT_Utilities.MakeSimulationSave();
            }

            if (HighLogic.LoadedScene == scene && scene == GameScenes.EDITOR) //Fix for null reference when using new or load buttons in editor
            {
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
            }

            if (scene == GameScenes.MAINMENU)
            {
                KCT_GameStates.reset();
                KCT_GameStates.firstStart = true;
                KCT_Utilities.disableSimulationLocks();
                InputLockManager.RemoveControlLock("KCTLaunchLock");
                KCT_GameStates.activeKSCName = "Stock";
                KCT_GameStates.ActiveKSC = new KCT_KSC("Stock");
                KCT_GameStates.KSCs = new List<KCT_KSC>() { KCT_GameStates.ActiveKSC };
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

        public void vesselSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> ev)
        {
            if (ev.from == Vessel.Situations.PRELAUNCH && ev.host == FlightGlobals.ActiveVessel)
            {
                if (!KCT_GameStates.settings.enabledForSave) return;
                if (KCT_GameStates.flightSimulated && KCT_GameStates.simulationTimeLimit > 0)
                {
                    KCT_GameStates.simulationEndTime = Planetarium.GetUniversalTime() + (KCT_GameStates.simulationTimeLimit);
                    KCT_Utilities.SpendFunds(KCT_GameStates.FundsToChargeAtSimEnd, TransactionReasons.None);
                }
                if (ev.host.protoVessel.landedAt == "LaunchPad" && !KCT_GameStates.flightSimulated && KCT_GameStates.settings.Reconditioning)
                {
                    KCT_Recon_Rollout reconditioning = KCT_GameStates.ActiveKSC.Recon_Rollout.FirstOrDefault(r => ((IKCTBuildItem)r).GetItemName() == "LaunchPad Reconditioning");
                    if (reconditioning == null)
                        KCT_GameStates.ActiveKSC.Recon_Rollout.Add(new KCT_Recon_Rollout(ev.host, KCT_Recon_Rollout.RolloutReconType.Reconditioning, ev.host.id.ToString()));
                }
            }
        }

        public void vesselRecoverEvent(ProtoVessel v)
        {
            if (!KCT_GameStates.settings.enabledForSave) return;
            if (!KCT_GameStates.flightSimulated && !v.vesselRef.isEVA)
            {
               // if (KCT_GameStates.settings.Debug && HighLogic.LoadedScene != GameScenes.TRACKSTATION && (v.wasControllable || v.protoPartSnapshots.Find(p => p.modules.Find(m => m.moduleName.ToLower() == "modulecommand") != null) != null))
                if (KCT_GameStates.recoveredVessel != null && v.vesselName == KCT_GameStates.recoveredVessel.shipName)
                {
                    //KCT_GameStates.recoveredVessel = new KCT_BuildListVessel(v);
                    KCT_Utilities.SpendFunds(KCT_GameStates.recoveredVessel.cost, TransactionReasons.VesselRollout);
                    if (KCT_GameStates.recoveredVessel.type == KCT_BuildListVessel.ListType.VAB)
                        KCT_GameStates.ActiveKSC.VABWarehouse.Add(KCT_GameStates.recoveredVessel);
                    else
                        KCT_GameStates.ActiveKSC.SPHWarehouse.Add(KCT_GameStates.recoveredVessel);
                    KCT_GameStates.ActiveKSC.Recon_Rollout.Add(new KCT_Recon_Rollout(KCT_GameStates.recoveredVessel, KCT_Recon_Rollout.RolloutReconType.Recovery, KCT_GameStates.recoveredVessel.id.ToString()));
                    KCT_GameStates.recoveredVessel = null;
                }
                else
                {
                    KCTDebug.Log("Adding recovered parts to Part Inventory");
                    foreach (ProtoPartSnapshot p in v.protoPartSnapshots)
                    {
                        //string name = p.partInfo.name + KCT_Utilities.GetTweakScaleSize(p);
                        
                        KCT_Utilities.AddPartToInventory(p);
                    }


                }
            }
        }


        private float GetResourceMass(List<ProtoPartResourceSnapshot> resources)
        {
            double mass = 0;
            foreach (ProtoPartResourceSnapshot resource in resources)
            {
                ConfigNode RCN = resource.resourceValues;
                double amount = double.Parse(RCN.GetValue("amount"));
                PartResourceDefinition RD = PartResourceLibrary.Instance.GetDefinition(resource.resourceName);
                mass += amount * RD.density;
            }
            return (float)mass;
        }
        public void vesselDestroyEvent(Vessel v)
        {
            if (!KCT_GameStates.settings.enabledForSave) return;
            if (!KCT_GameStates.settings.AllowParachuteRecovery) return;

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
                bool realChuteInUse = false;

                float RCParameter = 0;

                if (!v.packed) //adopted from mission controller.
                    foreach (Part p in v.Parts)
                        p.Pack();

                if (v.protoVessel == null)
                    return;
                KCTDebug.Log("Attempting to recover vessel.");
                try
                {
                    foreach (ProtoPartSnapshot p in v.protoVessel.protoPartSnapshots)
                    {
                        //KCTDebug.Log("Has part " + p.partName + ", mass " + p.mass);
                        List<string> ModuleNames = new List<string>();
                        foreach (ProtoPartModuleSnapshot ppms in p.modules)
                        {
                            //Debug.Log(ppms.moduleName);
                            ModuleNames.Add(ppms.moduleName);
                        }
                        totalMass += p.mass;
                        totalMass += GetResourceMass(p.resources);
                        bool isParachute = false;
                        if (ModuleNames.Contains("ModuleParachute"))
                        {
                            KCTDebug.Log("Found parachute module on " + p.partInfo.name);
                            //Find the ModuleParachute (find it in the module list by checking for a module with the name ModuleParachute)
                            ProtoPartModuleSnapshot ppms = p.modules.First(mod => mod.moduleName == "ModuleParachute");
                            float drag = 500;
                            if (ppms.moduleRef != null)
                            {
                                ModuleParachute mp = (ModuleParachute)ppms.moduleRef;
                                mp.Load(ppms.moduleValues);
                                drag = mp.fullyDeployedDrag;
                            }
                            else
                            {
                                drag = KCT_Utilities.GetParachuteDragFromPart(p.partInfo);
                                KCTDebug.Log("Pulled drag info from part. Drag: " + drag);
                            }
                            //Add the part mass times the fully deployed drag (typically 500) to the dragCoeff variable (you'll see why later)
                            dragCoeff += p.mass * drag;
                            //This is most definitely a parachute part
                            isParachute = true;
                        }
                        if (ModuleNames.Contains("RealChuteModule"))
                        {
                            KCTDebug.Log("Found realchute module on " + p.partInfo.name);
                            ProtoPartModuleSnapshot realChute = p.modules.First(mod => mod.moduleName == "RealChuteModule");
                            if ((object)realChute != null) //Some of this was adopted from DebRefund, as Vendan's method of handling multiple parachutes is better than what I had.
                            {
                                Type matLibraryType = AssemblyLoader.loadedAssemblies
                                    .SelectMany(a => a.assembly.GetExportedTypes())
                                    .SingleOrDefault(t => t.FullName == "RealChute.Libraries.MaterialsLibrary");

                                ConfigNode[] parchutes = realChute.moduleValues.GetNodes("PARACHUTE");
                                foreach (ConfigNode chute in parchutes)
                                {
                                    float diameter = float.Parse(chute.GetValue("deployedDiameter"));
                                    string mat = chute.GetValue("material");
                                    System.Reflection.MethodInfo matMethod = matLibraryType.GetMethod("GetMaterial", new Type[] { mat.GetType() });
                                    object MatLibraryInstance = matLibraryType.GetProperty("instance").GetValue(null, null);
                                    object materialObject = matMethod.Invoke(MatLibraryInstance, new object[] { mat });
                                    float dragC = (float)KCT_Utilities.GetMemberInfoValue(materialObject.GetType().GetMember("dragCoefficient")[0], materialObject);

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
                }
                catch (Exception e)
                {
                    Debug.LogError("[KCT] Error while attempting to recover vessel.");
                    Debug.LogException(e);
                }
                double Vt = double.MaxValue;
                if (!realChuteInUse)
                {
                    dragCoeff = dragCoeff / (totalMass);
                    Vt = Math.Sqrt((250 * 6.674E-11 * 5.2915793E22) / (3.6E11 * 1.22309485 * dragCoeff));
                    KCTDebug.Log("Using Stock Module! Drag: " + dragCoeff + " Vt: " + Vt);
                }
                else
                {
                    Vt = Math.Sqrt((8000 * totalMass * 9.8) / (1.223 * Math.PI) * Math.Pow(RCParameter, -1)); //This should work perfect for multiple identical chutes and gives an approximation for multiple differing chutes
                    KCTDebug.Log("Using RealChute Module! Vt: " + Vt);
                }
                if (Vt < 10.0)
                {
                    KCTDebug.Log("Recovered parts from " + v.vesselName);
                    foreach (ProtoPartSnapshot p in v.protoVessel.protoPartSnapshots)
                    {
                        KCT_Utilities.AddPartToInventory(p);
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
                            KCTDebug.Log("Delegating Funds recovery to another addon.");
                        }
                        else  //Otherwise do it ourselves
                        {
                            bool probeCoreAttached = false;
                            foreach (ProtoPartSnapshot pps in v.protoVessel.protoPartSnapshots)
                            {
                                //if (pps.modules.Find(module => (module.moduleName == "ModuleCommand" && KCT_Utilities.IsUnmannedCommand(pps.partInfo))) != null)
                                if (v.protoVessel.wasControllable)
                                {
                                    KCTDebug.Log("Was controlled!");
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
                            KCTDebug.Log("Vessel being recovered by KCT. Percent returned: " + 100 * RecoveryPercent + "%. Distance from KSC: " + Math.Round(KSCDistance / 1000, 2) + " km");
                            KCTDebug.Log("Funds being returned: " + Math.Round(totalReturn, 2) + "/" + Math.Round(totalBeforeModifier, 2));

                            Message.AppendLine("Funds recovered: " + FundsRecovered + "(" + Math.Round(RecoveryPercent * 100, 1) + "%)");
                            KCT_Utilities.AddFunds(FundsRecovered, TransactionReasons.VesselRecovery);
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

    public class KCT_UpgradingBuilding : IKCTBuildItem
    {
        [Persistent] public int upgradeLevel, currentLevel;
        [Persistent] public string id, commonName;
        [Persistent] public double progress=0, BP=0, cost=0;
        //public bool allowUpgrade = false;
        private KCT_KSC _KSC = null;
        public KCT_UpgradingBuilding(Upgradeables.UpgradeableFacility facility, int newLevel, int oldLevel, string name)
        {
            id = facility.id;
            upgradeLevel = newLevel;
            currentLevel = oldLevel;
            commonName = name;
        }

        public KCT_UpgradingBuilding()
        {

        }

        public void Downgrade()
        {
            foreach (Upgradeables.UpgradeableFacility facility in GetFacilityReferences())
            {
                KCT_Events.allowedToUpgrade = true;
                facility.SetLevel(currentLevel);
            }
            //KCT_Events.allowedToUpgrade = false;
        }

        public void Upgrade()
        {
            
            foreach (Upgradeables.UpgradeableFacility facility in GetFacilityReferences())
            {
                KCT_Events.allowedToUpgrade = true;
                facility.SetLevel(upgradeLevel);
            }
            //KCT_Events.allowedToUpgrade = false;
        }

        List<Upgradeables.UpgradeableFacility> GetFacilityReferences()
        {
            return ScenarioUpgradeableFacilities.protoUpgradeables[id].facilityRefs;
        }

        public void SetBP(double cost)
        {
            BP = Math.Sqrt(cost) * 2000 * KCT_GameStates.timeSettings.OverallMultiplier;
        }

        public bool AlreadyInProgress()
        {
            return (KSC != null);
        }

        public KCT_KSC KSC
        {
            get
            {
                if (_KSC == null)
                    _KSC = KCT_GameStates.KSCs.Find(ksc => ksc.KSCTech.Find(ub => ub.id == this.id) != null);
                return _KSC;
            }
        }

        string IKCTBuildItem.GetItemName()
        {
            return commonName;
        }
        double IKCTBuildItem.GetBuildRate()
        {
            double rateTotal = 0;
            if (KSC != null)
            {
                foreach (double rate in KCT_Utilities.BuildRatesSPH(KSC))
                    rateTotal += rate;
                foreach (double rate in KCT_Utilities.BuildRatesVAB(KSC))
                    rateTotal += rate;
            }
            return rateTotal;
        }
        double IKCTBuildItem.GetTimeLeft()
        {
            return (BP - progress) / ((IKCTBuildItem)this).GetBuildRate();
        }
        bool IKCTBuildItem.IsComplete()
        {
            return progress >= BP;
        }
        KCT_BuildListVessel.ListType IKCTBuildItem.GetListType()
        {
            return KCT_BuildListVessel.ListType.KSC;
        }
        public IKCTBuildItem AsIKCTBuildItem()
        {
            return (IKCTBuildItem)this;
        }
        public void AddProgress(double amt)
        {
            progress += amt;
            if (progress > BP) progress = BP;
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