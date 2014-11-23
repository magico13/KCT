using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Construction_Time
{
    public static partial class KCT_GUI
    {
        private static int listWindow = -1;
        public static void DrawBuildListWindow(int windowID)
        {
            //GUI.skin = HighLogic.Skin;
            int width1 = 120;
            int width2 = 100;
            int butW = 20;
            GUILayout.BeginVertical();
            //GUILayout.Label("Current KSC: " + KCT_GameStates.ActiveKSC.KSCName);
            //List next vessel to finish
            GUILayout.BeginHorizontal();
            GUILayout.Label("Next:", windowSkin.label);
            IKCTBuildItem buildItem = KCT_Utilities.NextThingToFinish();
            if (buildItem != null)
            {
                //KCT_BuildListVessel ship = (KCT_BuildListVessel)buildItem;
                GUILayout.Label(buildItem.GetItemName());
                if (buildItem.GetListType() == KCT_BuildListVessel.ListType.VAB || buildItem.GetListType() == KCT_BuildListVessel.ListType.Reconditioning)
                {
                    GUILayout.Label("VAB", windowSkin.label);
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(buildItem.GetTimeLeft()));
                }
                else if (buildItem.GetListType() == KCT_BuildListVessel.ListType.SPH)
                {
                    GUILayout.Label("SPH", windowSkin.label);
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(buildItem.GetTimeLeft()));
                }
                else if (buildItem.GetListType() == KCT_BuildListVessel.ListType.TechNode)
                {
                    GUILayout.Label("Tech", windowSkin.label);
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(buildItem.GetTimeLeft()));
                }

                if (!HighLogic.LoadedSceneIsEditor && TimeWarp.CurrentRateIndex == 0 && GUILayout.Button("Warp to" + System.Environment.NewLine + "Complete"))
                {
                    KCT_GameStates.targetedItem = buildItem;
                    KCT_GameStates.canWarp = true;
                    KCT_Utilities.RampUpWarp();
                    KCT_GameStates.warpInitiated = true;
                }
                else if (!HighLogic.LoadedSceneIsEditor && TimeWarp.CurrentRateIndex > 0 && GUILayout.Button("Stop" + System.Environment.NewLine + "Warp"))
                {
                    KCT_GameStates.canWarp = false;
                    TimeWarp.SetRate(0, true);
                    KCT_GameStates.lastWarpRate = 0;
                }

                if (KCT_GameStates.settings.AutoKACAlarams && KACWrapper.APIReady)
                {
                    double UT = Planetarium.GetUniversalTime();
                    if (!KCT_Utilities.ApproximatelyEqual(KCT_GameStates.KACAlarmUT - UT, buildItem.GetTimeLeft()))
                    {
                        KCTDebug.Log("KAC Alarm being created!");
                        KCT_GameStates.KACAlarmUT = (buildItem.GetTimeLeft() + UT);
                        KACWrapper.KACAPI.KACAlarm alarm = KACWrapper.KAC.Alarms.FirstOrDefault(a => a.ID == KCT_GameStates.KACAlarmId);
                        if (alarm == null)
                        {
                            alarm = KACWrapper.KAC.Alarms.FirstOrDefault(a => (a.Name.StartsWith("KCT: ")));
                        }
                        if (alarm != null)
                        {
                            KCTDebug.Log("Removing existing alarm");
                            KACWrapper.KAC.DeleteAlarm(alarm.ID);
                        }
                        KCT_GameStates.KACAlarmId = KACWrapper.KAC.CreateAlarm(KACWrapper.KACAPI.AlarmTypeEnum.Raw, "KCT: " + buildItem.GetItemName() + " Complete", KCT_GameStates.KACAlarmUT);
                        KCTDebug.Log("Alarm created with ID: " + KCT_GameStates.KACAlarmId);
                    }
                }
            }
            else
            {
                GUILayout.Label("No Active Projects");
            }
            GUILayout.EndHorizontal();

            //Buttons for VAB/SPH lists
            List<string> buttonList = new List<string> { "VAB", "SPH" };
            if (KCT_Utilities.CurrentGameHasScience() && !KCT_GameStates.settings.InstantTechUnlock) buttonList.Add("Tech");
            GUILayout.BeginHorizontal();
            //if (HighLogic.LoadedScene == GameScenes.SPACECENTER) { buttonList.Add("Upgrades"); buttonList.Add("Settings"); }
            int lastSelected = listWindow;
            listWindow = GUILayout.Toolbar(listWindow, buttonList.ToArray());

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && GUILayout.Button("Upgrades"))
            {
                showUpgradeWindow = true;
                showBuildList = false;
                showBLPlus = false;
            }
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && GUILayout.Button("Settings"))
            {
                showBuildList = false;
                showBLPlus = false;
                ShowSettings();
            }
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                buildListWindowPosition.height = 1;
                showBLPlus = false;
                if (lastSelected == listWindow)
                {
                    listWindow = -1;
                }
            }
            //Content of lists
            if (listWindow == 0) //VAB Build List
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.ActiveKSC.VABList;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1 / 2));
                GUILayout.Label("Time Left:", GUILayout.Width(width2));
                GUILayout.Label("BP:", GUILayout.Width(width1 / 2 + 10));
                GUILayout.Space((butW + 4) * 3);
                GUILayout.EndHorizontal();
                if (KCT_Utilities.ReconditioningActive(null))
                {
                    GUILayout.BeginHorizontal();
                    IKCTBuildItem item = (IKCTBuildItem)KCT_GameStates.ActiveKSC.GetReconditioning();
                    GUILayout.Label(item.GetItemName());
                    GUILayout.Label(KCT_GameStates.ActiveKSC.GetReconditioning().ProgressPercent().ToString() + "%", GUILayout.Width(width1 / 2));
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(item.GetTimeLeft()), GUILayout.Width(width2));
                    GUILayout.Label(Math.Round(KCT_GameStates.ActiveKSC.GetReconditioning().BP, 2).ToString(), GUILayout.Width(width1 / 2 + 10));
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("Warp To", GUILayout.Width((butW + 4) * 3)))
                    {
                        KCT_GameStates.targetedItem = item;
                        KCT_GameStates.canWarp = true;
                        KCT_Utilities.RampUpWarp(item);
                        KCT_GameStates.warpInitiated = true;
                    }
                    else if (HighLogic.LoadedSceneIsEditor)
                        GUILayout.Space((butW + 4) * 3);
                    //GUILayout.Space((butW + 4) * 3);
                    GUILayout.EndHorizontal();
                }
                //TODO: Add rollout and rollback here

                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((buildList.Count) * 25 + 10, Screen.height / 4F)));
                {
                    if (buildList.Count == 0)
                    {
                        GUILayout.Label("No vessels under construction! Go to the VAB to build some.");
                    }
                    for (int i = 0; i < buildList.Count; i++)
                    {
                        KCT_BuildListVessel b = buildList[i];
                        GUILayout.BeginHorizontal();
                        //GUILayout.Label(b.shipName, GUILayout.Width(width1));
                        GUILayout.Label(b.shipName);
                        GUILayout.Label(Math.Round(b.ProgressPercent(), 2).ToString() + "%", GUILayout.Width(width1 / 2));
                        if (b.buildRate > 0)
                            GUILayout.Label(KCT_Utilities.GetColonFormattedTime(b.timeLeft), GUILayout.Width(width2));
                        else
                            GUILayout.Label("Est: " + KCT_Utilities.GetColonFormattedTime((b.buildPoints - b.progress) / KCT_Utilities.GetBuildRate(0, KCT_BuildListVessel.ListType.VAB, null)), GUILayout.Width(width2));
                        GUILayout.Label(Math.Round(b.buildPoints, 2).ToString(), GUILayout.Width(width1 / 2 + 10));
                        if (i > 0 && GUILayout.Button("^", GUILayout.Width(butW)))
                        {
                            buildList.RemoveAt(i);
                            buildList.Insert(i - 1, b);
                        }
                        else if (i == 0)
                        {
                            GUILayout.Space(butW + 4);
                        }
                        if (i < buildList.Count - 1 && GUILayout.Button("v", GUILayout.Width(butW)))
                        {
                            buildList.RemoveAt(i);
                            buildList.Insert(i + 1, b);
                        }
                        else if (i >= buildList.Count - 1)
                        {
                            GUILayout.Space(butW + 4);
                        }
                        if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("*", GUILayout.Width(butW)))
                        {
                            if (IndexSelected == i)
                                showBLPlus = !showBLPlus;
                            else
                                showBLPlus = true;
                            IndexSelected = i;
                        }
                        else if (HighLogic.LoadedSceneIsEditor)
                        {
                            //GUILayout.Space(butW);
                            if (GUILayout.Button("X", GUILayout.Width(butW)))
                            {
                                InputLockManager.SetControlLock(ControlTypes.EDITOR_SOFT_LOCK, "KCTPopupLock");
                                IndexSelected = i;
                                DialogOption[] options = new DialogOption[2];
                                options[0] = new DialogOption("Yes", ScrapVessel);
                                options[1] = new DialogOption("No", DummyVoid);
                                MultiOptionDialog diag = new MultiOptionDialog("Are you sure you want to scrap this vessel?", windowTitle: "Scrap Vessel", options: options);
                                PopupDialog.SpawnPopupDialog(diag, false, windowSkin);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    //ADD Storage here!
                    buildList = KCT_GameStates.ActiveKSC.VABWarehouse;
                    GUILayout.Label("VAB Storage");
                    if (buildList.Count == 0)
                    {
                        GUILayout.Label("No vessels in storage. They will be moved here automatically when they are finished building.");
                    }
                    for (int i = 0; i < buildList.Count; i++)
                    {
                        KCT_BuildListVessel b = buildList[i];
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(b.shipName);
                        KCT_Recon_Rollout rollout = KCT_GameStates.ActiveKSC.GetReconRollout(KCT_Recon_Rollout.RolloutReconType.Rollout);
                        if (!HighLogic.LoadedSceneIsEditor && (rollout == null || b.id.ToString() != rollout.associatedID) && GUILayout.Button("Rollout", GUILayout.ExpandWidth(false)))
                        {

                        }
                        else if (!HighLogic.LoadedSceneIsEditor && b.id.ToString() == rollout.associatedID && rollout.ProgressPercent() >= 100 && GUILayout.Button("Launch", GUILayout.ExpandWidth(false)))
                        {
                            if (KCT_Utilities.ReconditioningActive(null))
                            {
                                //can't launch now
                                ScreenMessage message = new ScreenMessage("[KCT] Cannot launch while LaunchPad is being reconditioned. It will be finished in "
                                    + KCT_Utilities.GetFormattedTime(((IKCTBuildItem)KCT_GameStates.ActiveKSC.GetReconditioning()).GetTimeLeft()), 4.0f, ScreenMessageStyle.UPPER_CENTER);
                                ScreenMessages.PostScreenMessage(message, true);
                            }
                            else
                            {
                                KCT_GameStates.launchedVessel = b;
                                if (ShipAssembly.CheckLaunchSiteClear(HighLogic.CurrentGame.flightState, "LaunchPad", false))
                                {
                                    showBLPlus = false;
                                    // buildList.RemoveAt(i);
                                    if (!IsCrewable(b.ExtractedParts))
                                        b.Launch();
                                    else
                                    {
                                        showBuildList = false;
                                        centralWindowPosition.height = 1;
                                        KCT_GameStates.launchedCrew.Clear();
                                        parts = KCT_GameStates.launchedVessel.ExtractedParts;
                                        pseudoParts = KCT_GameStates.launchedVessel.GetPseudoParts();
                                        KCT_GameStates.launchedCrew = new List<CrewedPart>();
                                        foreach (PseudoPart pp in pseudoParts)
                                            KCT_GameStates.launchedCrew.Add(new CrewedPart(pp.uid, new List<ProtoCrewMember>()));
                                        CrewFirstAvailable();
                                        showShipRoster = true;
                                    }
                                }
                                else
                                {
                                    showBuildList = false;
                                    showClearLaunch = true;
                                }
                            }
                        }
                        if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("*", GUILayout.Width(butW)))
                        {
                            if (IndexSelected == i)
                                showBLPlus = !showBLPlus;
                            else
                                showBLPlus = true;
                            IndexSelected = i;
                        }
                        else if (HighLogic.LoadedSceneIsEditor)
                            GUILayout.Space(butW);
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow == 1) //SPH Build List
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.ActiveKSC.SPHList;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1 / 2));
                GUILayout.Label("Time Left:", GUILayout.Width(width2));
                GUILayout.Label("BP:", GUILayout.Width(width1 / 2 + 10));
                GUILayout.Space((butW + 4) * 3);
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((buildList.Count) * 25 + 10, Screen.height / 4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName);
                    GUILayout.Label(Math.Round(b.ProgressPercent(), 2).ToString() + "%", GUILayout.Width(width1 / 2));
                    if (b.buildRate > 0)
                        GUILayout.Label(KCT_Utilities.GetColonFormattedTime(b.timeLeft), GUILayout.Width(width2));
                    else
                        GUILayout.Label("Est: " + KCT_Utilities.GetColonFormattedTime((b.buildPoints - b.progress) / KCT_Utilities.GetBuildRate(0, KCT_BuildListVessel.ListType.SPH, null)), GUILayout.Width(width2));
                    GUILayout.Label(Math.Round(b.buildPoints, 2).ToString(), GUILayout.Width(width1 / 2 + 10));
                    if (i > 0 && GUILayout.Button("^", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i - 1, b);
                    }
                    else if (i == 0)
                    {
                        GUILayout.Space(butW + 4);
                    }
                    if (i < buildList.Count - 1 && GUILayout.Button("v", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i + 1, b);
                    }
                    else if (i >= buildList.Count - 1)
                    {
                        GUILayout.Space(butW + 4);
                    }
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("*", GUILayout.Width(butW)))
                    {
                        if (IndexSelected == i)
                            showBLPlus = !showBLPlus;
                        else
                            showBLPlus = true;
                        IndexSelected = i;
                    }
                    else if (HighLogic.LoadedSceneIsEditor)
                    {
                        //GUILayout.Space(butW);
                        if (GUILayout.Button("X", GUILayout.Width(butW)))
                        {
                            InputLockManager.SetControlLock(ControlTypes.EDITOR_SOFT_LOCK, "KCTPopupLock");
                            IndexSelected = i;
                            DialogOption[] options = new DialogOption[2];
                            options[0] = new DialogOption("Yes", ScrapVessel);
                            options[1] = new DialogOption("No", DummyVoid);
                            MultiOptionDialog diag = new MultiOptionDialog("Are you sure you want to scrap this vessel?", windowTitle: "Scrap Vessel", options: options);
                            PopupDialog.SpawnPopupDialog(diag, false, windowSkin);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow == 2) //VAB Warehouse
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.ActiveKSC.VABWarehouse;
                //GUILayout.Label("VAB Storage");
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((buildList.Count) * 25 + 10, Screen.height / 4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName);
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("Launch", GUILayout.ExpandWidth(false)))
                    {
                        if (KCT_Utilities.ReconditioningActive(null))
                        {
                            //can't launch now
                            ScreenMessage message = new ScreenMessage("[KCT] Cannot launch while LaunchPad is being reconditioned. It will be finished in "
                                + KCT_Utilities.GetFormattedTime(((IKCTBuildItem)KCT_GameStates.ActiveKSC.GetReconditioning()).GetTimeLeft()), 4.0f, ScreenMessageStyle.UPPER_CENTER);
                            ScreenMessages.PostScreenMessage(message, true);
                        }
                        else
                        {
                            KCT_GameStates.launchedVessel = b;
                            if (ShipAssembly.CheckLaunchSiteClear(HighLogic.CurrentGame.flightState, "LaunchPad", false))
                            {
                                showBLPlus = false;
                                // buildList.RemoveAt(i);
                                if (!IsCrewable(b.ExtractedParts))
                                    b.Launch();
                                else
                                {
                                    showBuildList = false;
                                    centralWindowPosition.height = 1;
                                    KCT_GameStates.launchedCrew.Clear();
                                    parts = KCT_GameStates.launchedVessel.ExtractedParts;
                                    pseudoParts = KCT_GameStates.launchedVessel.GetPseudoParts();
                                    KCT_GameStates.launchedCrew = new List<CrewedPart>();
                                    foreach (PseudoPart pp in pseudoParts)
                                        KCT_GameStates.launchedCrew.Add(new CrewedPart(pp.uid, new List<ProtoCrewMember>()));
                                    CrewFirstAvailable();
                                    showShipRoster = true;
                                }
                            }
                            else
                            {
                                showBuildList = false;
                                showClearLaunch = true;
                            }
                        }
                    }
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("*", GUILayout.Width(butW)))
                    {
                        if (IndexSelected == i)
                            showBLPlus = !showBLPlus;
                        else
                            showBLPlus = true;
                        IndexSelected = i;
                    }
                    else if (HighLogic.LoadedSceneIsEditor)
                        GUILayout.Space(butW);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow == 3) //SPH Warehouse
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.ActiveKSC.SPHWarehouse;
                //GUILayout.Label("SPH Storage");
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((buildList.Count) * 25 + 10, Screen.height / 4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName);
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("Launch", GUILayout.ExpandWidth(false)))
                    {
                        showBLPlus = false;
                        KCT_GameStates.launchedVessel = b;
                        if (ShipAssembly.CheckLaunchSiteClear(HighLogic.CurrentGame.flightState, "Runway", false))
                        {
                            if (!IsCrewable(b.ExtractedParts))
                                b.Launch();
                            else
                            {
                                showBuildList = false;
                                centralWindowPosition.height = 1;
                                KCT_GameStates.launchedCrew.Clear();
                                parts = KCT_GameStates.launchedVessel.ExtractedParts;
                                pseudoParts = KCT_GameStates.launchedVessel.GetPseudoParts();
                                KCT_GameStates.launchedCrew = new List<CrewedPart>();
                                foreach (PseudoPart pp in pseudoParts)
                                    KCT_GameStates.launchedCrew.Add(new CrewedPart(pp.uid, new List<ProtoCrewMember>()));
                                CrewFirstAvailable();
                                showShipRoster = true;
                            }
                        }
                        else
                        {
                            showBuildList = false;
                            showClearLaunch = true;
                        }
                    }
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("*", GUILayout.Width(butW)))
                    {
                        if (IndexSelected == i)
                            showBLPlus = !showBLPlus;
                        else
                            showBLPlus = true;
                        IndexSelected = i;
                    }
                    else if (HighLogic.LoadedSceneIsEditor)
                        GUILayout.Space(butW);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow == 4) //Tech nodes
            {
                List<KCT_TechItem> techList = KCT_GameStates.TechList;
                //GUILayout.Label("Tech Node Research");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Node Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1));
                GUILayout.Label("Time Left:", GUILayout.Width(width1));
                GUILayout.Space(width2);
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((techList.Count) * 25 + 10, Screen.height / 4F)));
                for (int i = 0; i < techList.Count; i++)
                {
                    KCT_TechItem t = techList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(t.techName);
                    GUILayout.Label(Math.Round(100 * t.progress / t.scienceCost, 2) + " %", GUILayout.Width(width1));
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(t.TimeLeft), GUILayout.Width(width1));
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("Warp To", GUILayout.Width(width2)))
                    {
                        KCT_GameStates.targetedItem = t;
                        KCT_GameStates.canWarp = true;
                        KCT_Utilities.RampUpWarp(t);
                        KCT_GameStates.warpInitiated = true;
                    }
                    else if (HighLogic.LoadedSceneIsEditor)
                        GUILayout.Space(width2);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            /*else
            {
                //if (buildListWindowPosition.height > 32*3) buildListWindowPosition.height = 32*3;
                //buildListWindowPosition.height = 1;
            }*/
            if (KCT_UpdateChecker.UpdateFound)
                GUILayout.Label("Update available! Current: " + KCT_UpdateChecker.CurrentVersion + " Latest: " + KCT_UpdateChecker.WebVersion);
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();


            CheckKSCLock();

            /*
            if (Event.current.type == EventType.Repaint && buildListWindowPosition.Contains(Event.current.mousePosition))
            {
                //Mouseover event
                if (InputLockManager.IsUnlocked(ControlTypes.KSC_ALL))
                    InputLockManager.SetControlLock(ControlTypes.KSC_ALL, "KCTKSCLock");
            }
            else
            {
                //Mouse away
                if (InputLockManager.GetControlLock("KCTKSCLock") != ControlTypes.None && InputLockManager.IsLocked(ControlTypes.KSC_ALL))
                    InputLockManager.RemoveControlLock("KCTKSCLock");
            }*/
        }



    }
}
