using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;

namespace KerbalConstructionTime
{
    public static partial class KCT_GUI
    {
        private static List<string> launchSites = new List<string>();
        private static int MouseOnRolloutButton = -1;
        private static int listWindow = -1;
        private static bool VABSelected, SPHSelected, TechSelected;
        public static void SelectList(string list)
        {
            buildListWindowPosition.height = 1;
            switch (list)
            {
                case "VAB":
                    if (VABSelected)
                    {
                        listWindow = -1;
                        VABSelected = false;
                    }
                    else
                    {
                        listWindow = 0;
                        VABSelected = true;
                        SPHSelected = false;
                        TechSelected = false;
                    }
                    break;
                case "SPH":
                    if (SPHSelected)
                    {
                        listWindow = -1;
                        SPHSelected = false;
                    }
                    else
                    {
                        listWindow = 1;
                        VABSelected = false;
                        SPHSelected = true;
                        TechSelected = false;
                    }
                    break;
                case "Tech":
                    if (TechSelected)
                    {
                        listWindow = -1;
                        TechSelected = false;
                    }
                    else
                    {
                        listWindow = 2;
                        VABSelected = false;
                        SPHSelected = false;
                        TechSelected = true;
                    }
                    break;
                default:
                    listWindow = -1;
                    TechSelected = false;
                    VABSelected = false;
                    SPHSelected = false;
                    break;
            }
        }

        public enum VesselPadStatus { InStorage, RollingOut, RolledOut, RollingBack, Recovering };
        private static double costOfNewLP = -13;

        public static void DrawBuildListWindow(int windowID)
        {
            //if (buildListWindowPosition.xMax > Screen.width)
            //    buildListWindowPosition.x = Screen.width - buildListWindowPosition.width;

            //if (Input.touchCount == 0) MouseOnRolloutButton = false;

            //GUI.skin = HighLogic.UISkin;
            GUIStyle redText = new GUIStyle(GUI.skin.label);
            redText.normal.textColor = Color.red;
            GUIStyle yellowText = new GUIStyle(GUI.skin.label);
            yellowText.normal.textColor = Color.yellow;
            GUIStyle greenText = new GUIStyle(GUI.skin.label);
            greenText.normal.textColor = Color.green;

            GUIStyle normalButton = new GUIStyle(GUI.skin.button);
            GUIStyle yellowButton = new GUIStyle(GUI.skin.button);
            yellowButton.normal.textColor = Color.yellow;
            yellowButton.hover.textColor = Color.yellow;
            yellowButton.active.textColor = Color.yellow;
            GUIStyle redButton = new GUIStyle(GUI.skin.button);
            redButton.normal.textColor = Color.red;
            redButton.hover.textColor = Color.red;
            redButton.active.textColor = Color.red;

            GUIStyle greenButton = new GUIStyle(GUI.skin.button);
            greenButton.normal.textColor = Color.green;
            greenButton.hover.textColor = Color.green;
            greenButton.active.textColor = Color.green;


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
                
                string txt = buildItem.GetItemName(), locTxt = "VAB";
                if (buildItem.GetListType() == KCT_BuildListVessel.ListType.Reconditioning)
                {
                    KCT_Recon_Rollout reconRoll = buildItem as KCT_Recon_Rollout;
                    if (reconRoll.RRType == KCT_Recon_Rollout.RolloutReconType.Reconditioning)
                    {
                        txt = "Reconditioning";
                        locTxt = reconRoll.launchPadID;
                    }
                    else if (reconRoll.RRType == KCT_Recon_Rollout.RolloutReconType.Rollout)
                    {
                        KCT_BuildListVessel associated = reconRoll.KSC.VABWarehouse.FirstOrDefault(blv => blv.id.ToString() == reconRoll.associatedID);
                        txt = associated.shipName + " Rollout";
                        locTxt = reconRoll.launchPadID;
                    }
                    else if (reconRoll.RRType == KCT_Recon_Rollout.RolloutReconType.Rollback)
                    {
                        KCT_BuildListVessel associated = reconRoll.KSC.VABWarehouse.FirstOrDefault(blv => blv.id.ToString() == reconRoll.associatedID);
                        txt = associated.shipName + " Rollback";
                        locTxt = reconRoll.launchPadID;
                    }
                    else
                    {
                        locTxt = "VAB";
                    }
                }
                else if (buildItem.GetListType() == KCT_BuildListVessel.ListType.VAB)
                {
                    locTxt = "VAB";
                }
                else if (buildItem.GetListType() == KCT_BuildListVessel.ListType.SPH)
                {
                    locTxt = "SPH";
                }
                else if (buildItem.GetListType() == KCT_BuildListVessel.ListType.TechNode)
                {
                    locTxt = "Tech";
                }
                else if (buildItem.GetListType() == KCT_BuildListVessel.ListType.KSC)
                {
                    locTxt = "KSC";
                }

                GUILayout.Label(txt);
                GUILayout.Label(locTxt, windowSkin.label);
                GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(buildItem.GetTimeLeft()));

                if (!HighLogic.LoadedSceneIsEditor && TimeWarp.CurrentRateIndex == 0 && GUILayout.Button("Warp to" + System.Environment.NewLine + "Complete"))
                {
                    KCT_GameStates.targetedItem = buildItem;
                    KCT_GameStates.canWarp = true;
                    KCT_Utilities.RampUpWarp();
                    KCT_GameStates.warpInitiated = true;
                   /* if (buildItem.GetBuildRate() > 0)
                    {
                        TimeWarp.fetch.WarpTo(Planetarium.GetUniversalTime() + buildItem.GetTimeLeft(), KCT_GameStates.settings.MaxTimeWarp, 1);
                    }*/
                }
                else if (!HighLogic.LoadedSceneIsEditor && TimeWarp.CurrentRateIndex > 0 && GUILayout.Button("Stop" + System.Environment.NewLine + "Warp"))
                {
                    KCT_GameStates.canWarp = false;
                    TimeWarp.SetRate(0, true);
                    KCT_GameStates.lastWarpRate = 0;
                }

                if (KCT_GameStates.settings.AutoKACAlarms && KACWrapper.APIReady && buildItem.GetTimeLeft() > 30) //don't check if less than 30 seconds to completion. Might fix errors people are seeing
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
                        txt = "KCT: ";
                        if (buildItem.GetListType() == KCT_BuildListVessel.ListType.Reconditioning)
                        {
                            KCT_Recon_Rollout reconRoll = buildItem as KCT_Recon_Rollout;
                            if (reconRoll.RRType == KCT_Recon_Rollout.RolloutReconType.Reconditioning)
                            {
                                txt += reconRoll.launchPadID + " Reconditioning";
                            }
                            else if (reconRoll.RRType == KCT_Recon_Rollout.RolloutReconType.Rollout)
                            {
                                KCT_BuildListVessel associated = reconRoll.KSC.VABWarehouse.FirstOrDefault(blv => blv.id.ToString() == reconRoll.associatedID);
                                txt += associated.shipName + " rollout at " + reconRoll.launchPadID;
                            }
                            else if (reconRoll.RRType == KCT_Recon_Rollout.RolloutReconType.Rollback)
                            {
                                KCT_BuildListVessel associated = reconRoll.KSC.VABWarehouse.FirstOrDefault(blv => blv.id.ToString() == reconRoll.associatedID);
                                txt += associated.shipName + " rollback at " + reconRoll.launchPadID;
                            }
                            else
                            {
                                txt += buildItem.GetItemName() + " Complete";
                            }
                        }
                        else
                            txt += buildItem.GetItemName() + " Complete";
                        KCT_GameStates.KACAlarmId = KACWrapper.KAC.CreateAlarm(KACWrapper.KACAPI.AlarmTypeEnum.Raw, txt, KCT_GameStates.KACAlarmUT);
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
           // List<string> buttonList = new List<string> { "VAB", "SPH", "KSC" };
            //if (KCT_Utilities.CurrentGameHasScience() && !KCT_GameStates.settings.InstantTechUnlock) buttonList.Add("Tech");
            GUILayout.BeginHorizontal();
            //if (HighLogic.LoadedScene == GameScenes.SPACECENTER) { buttonList.Add("Upgrades"); buttonList.Add("Settings"); }
          //  int lastSelected = listWindow;
           // listWindow = GUILayout.Toolbar(listWindow, buttonList.ToArray());

            bool VABSelectedNew = GUILayout.Toggle(VABSelected, "VAB", GUI.skin.button);
            bool SPHSelectedNew = GUILayout.Toggle(SPHSelected, "SPH", GUI.skin.button);
            bool TechSelectedNew = false;
            if (KCT_Utilities.CurrentGameHasScience())
                TechSelectedNew = GUILayout.Toggle(TechSelected, "Tech", GUI.skin.button);
            if (VABSelectedNew != VABSelected)
                SelectList("VAB");
            else if (SPHSelectedNew != SPHSelected)
                SelectList("SPH");
            else if (TechSelectedNew != TechSelected)
                SelectList("Tech");

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

          /*  if (GUI.changed)
            {
                buildListWindowPosition.height = 1;
                showBLPlus = false;
                if (lastSelected == listWindow)
                {
                    listWindow = -1;
                }
            }*/
            //Content of lists
            if (listWindow == 0) //VAB Build List
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.ActiveKSC.VABList;
                GUILayout.BeginHorizontal();
              //  GUILayout.Space((butW + 4) * 3);
                GUILayout.Label("Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1 / 2));
                GUILayout.Label("Time Left:", GUILayout.Width(width2));
                //GUILayout.Label("BP:", GUILayout.Width(width1 / 2 + 10));
                GUILayout.EndHorizontal();
                //if (KCT_Utilities.ReconditioningActive(null))
                foreach (KCT_Recon_Rollout reconditioning in KCT_GameStates.ActiveKSC.Recon_Rollout.FindAll(r => r.RRType == KCT_Recon_Rollout.RolloutReconType.Reconditioning))
                {
                    GUILayout.BeginHorizontal();
                    IKCTBuildItem item = reconditioning.AsBuildItem();
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("Warp To", GUILayout.Width((butW + 4) * 3)))
                    {
                        KCT_GameStates.targetedItem = item;
                        KCT_GameStates.canWarp = true;
                        KCT_Utilities.RampUpWarp(item);
                        KCT_GameStates.warpInitiated = true;
                        /*if (item.GetBuildRate() > 0)
                        {
                            TimeWarp.fetch.WarpTo(Planetarium.GetUniversalTime() + item.GetTimeLeft(), KCT_GameStates.settings.MaxTimeWarp, 1);
                        }*/
                    }
                    
                    GUILayout.Label("Reconditioning: "+reconditioning.launchPadID);
                    GUILayout.Label(reconditioning.ProgressPercent().ToString() + "%", GUILayout.Width(width1 / 2));
                    GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(item.GetTimeLeft()), GUILayout.Width(width2));
                    //GUILayout.Label(Math.Round(KCT_GameStates.ActiveKSC.GetReconditioning().BP, 2).ToString(), GUILayout.Width(width1 / 2 + 10));
                    
                    //GUILayout.Space((butW + 4) * 3);
                    GUILayout.EndHorizontal();
                }

                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));
                {
                    if (buildList.Count == 0)
                    {
                        GUILayout.Label("No vessels under construction! Go to the VAB to build more.");
                    }
                    for (int i = 0; i < buildList.Count; i++)
                    {
                        KCT_BuildListVessel b = buildList[i];
                        if (!b.allPartsValid)
                            continue;
                        GUILayout.BeginHorizontal();
                        //GUILayout.Label(b.shipName, GUILayout.Width(width1));

                        if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("*", GUILayout.Width(butW)))
                        {
                            if (IDSelected == b.id)
                                showBLPlus = !showBLPlus;
                            else
                                showBLPlus = true;
                            IDSelected = b.id;
                        }
                        else if (HighLogic.LoadedSceneIsEditor)
                        {
                            //GUILayout.Space(butW);
                            if (GUILayout.Button("X", GUILayout.Width(butW)))
                            {
                                InputLockManager.SetControlLock(ControlTypes.EDITOR_SOFT_LOCK, "KCTPopupLock");
                                IDSelected = b.id;
                                DialogGUIBase[] options = new DialogGUIBase[2];
                                options[0] = new DialogGUIButton("Yes", ScrapVessel);
                                options[1] = new DialogGUIButton("No", DummyVoid);
                                MultiOptionDialog diag = new MultiOptionDialog("scrapVesselPopup", "Are you sure you want to scrap this vessel?", "Scrap Vessel", null, options: options);
                                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), diag, false, HighLogic.UISkin);
                            }
                        }

                        if (i > 0 && GUILayout.Button("^", GUILayout.Width(butW)))
                        {
                            buildList.RemoveAt(i);
                            if (GameSettings.MODIFIER_KEY.GetKey())
                            {
                                buildList.Insert(0, b);
                            }
                            else
                            {
                                buildList.Insert(i - 1, b);
                            }
                        }
                        else if (i == 0)
                        {
                      //      GUILayout.Space(butW + 4);
                        }
                        if (i < buildList.Count - 1 && GUILayout.Button("v", GUILayout.Width(butW)))
                        {
                            buildList.RemoveAt(i);
                            if (GameSettings.MODIFIER_KEY.GetKey())
                            {
                                buildList.Add(b);
                            }
                            else
                            {
                                buildList.Insert(i + 1, b);
                            }
                        }
                        else if (i >= buildList.Count - 1)
                        {
                      //      GUILayout.Space(butW + 4);
                        }


                        GUILayout.Label(b.shipName);
                        GUILayout.Label(Math.Round(b.ProgressPercent(), 2).ToString() + "%", GUILayout.Width(width1 / 2));
                        if (b.buildRate > 0)
                            GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(b.timeLeft), GUILayout.Width(width2));
                        else
                            GUILayout.Label("Est: " + MagiCore.Utilities.GetColonFormattedTime((b.buildPoints - b.progress) / KCT_Utilities.GetBuildRate(0, KCT_BuildListVessel.ListType.VAB, null)), GUILayout.Width(width2));
                       // GUILayout.Label(Math.Round(b.buildPoints, 2).ToString(), GUILayout.Width(width1 / 2 + 10));
                        GUILayout.EndHorizontal();
                    }

                    //ADD Storage here!
                    buildList = KCT_GameStates.ActiveKSC.VABWarehouse;
                    GUILayout.Label("__________________________________________________");
                    GUILayout.Label("VAB Storage");
                    if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.IsRecoverable && FlightGlobals.ActiveVessel.IsClearToSave() == ClearToSaveStatus.CLEAR && GUILayout.Button("Recover Active Vessel"))
                    {
                        if (!KCT_Utilities.RecoverActiveVesselToStorage(KCT_BuildListVessel.ListType.VAB))
                        {
                            PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "vesselRecoverErrorPopup", "Error!", "There was an error while recovering the ship. Sometimes reloading the scene and trying again works. Sometimes a vessel just can't be recovered this way and you must use the stock recover system.", "OK", false, HighLogic.UISkin);
                        }
                    }
                    if (buildList.Count == 0)
                    {
                        GUILayout.Label("No vessels in storage!\nThey will be stored here when they are complete.");
                    }
                    
                    //KCT_Recon_Rollout rollback = KCT_GameStates.ActiveKSC.GetReconRollout(KCT_Recon_Rollout.RolloutReconType.Rollback);
                    bool rolloutEnabled = KCT_PresetManager.Instance.ActivePreset.generalSettings.ReconditioningTimes && KCT_PresetManager.Instance.ActivePreset.timeSettings.RolloutReconSplit > 0;
                    for (int i = 0; i < buildList.Count; i++)
                    {
                        KCT_BuildListVessel b = buildList[i];
                        if (!b.allPartsValid)
                            continue;
                        string launchSite = b.launchSite;
                        if (launchSite == "LaunchPad")
                        {
                            if (b.launchSiteID >= 0)
                                launchSite = KCT_GameStates.ActiveKSC.LaunchPads[b.launchSiteID].name;
                            else
                                launchSite = KCT_GameStates.ActiveKSC.ActiveLPInstance.name;
                        }
                        KCT_Recon_Rollout rollout = KCT_GameStates.ActiveKSC.GetReconRollout(KCT_Recon_Rollout.RolloutReconType.Rollout, launchSite);
                        KCT_Recon_Rollout rollback = KCT_GameStates.ActiveKSC.Recon_Rollout.FirstOrDefault(r => r.associatedID == b.id.ToString() && r.RRType == KCT_Recon_Rollout.RolloutReconType.Rollback);
                        KCT_Recon_Rollout recovery = KCT_GameStates.ActiveKSC.Recon_Rollout.FirstOrDefault(r => r.associatedID == b.id.ToString() && r.RRType == KCT_Recon_Rollout.RolloutReconType.Recovery);
                        GUIStyle textColor = new GUIStyle(GUI.skin.label);
                        GUIStyle buttonColor = new GUIStyle(GUI.skin.button);

                        VesselPadStatus padStatus = VesselPadStatus.InStorage;
                        if (rollback != null)
                            padStatus = VesselPadStatus.RollingBack;
                        if (recovery != null)
                            padStatus = VesselPadStatus.Recovering;

                        string status = "In Storage";
                        if (rollout != null && rollout.associatedID == b.id.ToString())
                        {
                            padStatus = VesselPadStatus.RollingOut;
                            status = "Rolling Out to "+launchSite;
                            textColor = yellowText;
                            if (rollout.AsBuildItem().IsComplete())
                            {
                                padStatus = VesselPadStatus.RolledOut;
                                status = "At "+launchSite;
                                textColor = greenText;
                            }
                        }
                        else if (rollback != null)
                        {
                            status = "Rolling Back from "+launchSite;
                            textColor = yellowText;
                        }
                        else if (recovery != null)
                        {
                            status = "Recovering";
                            textColor = redText;
                        }

                        GUILayout.BeginHorizontal();
                        if (!HighLogic.LoadedSceneIsEditor && (padStatus == VesselPadStatus.InStorage || padStatus == VesselPadStatus.RolledOut))
                        {
                            if (GUILayout.Button("*", GUILayout.Width(butW)))
                            {
                                if (IDSelected == b.id)
                                    showBLPlus = !showBLPlus;
                                else
                                    showBLPlus = true;
                                IDSelected = b.id;
                            }
                        }
                        else
                            GUILayout.Space(butW + 4);

                        GUILayout.Label(b.shipName, textColor);
                        GUILayout.Label(status+"   ", textColor, GUILayout.ExpandWidth(false));
                        bool siteHasActiveRolloutOrRollback = rollout != null || KCT_GameStates.ActiveKSC.GetReconRollout(KCT_Recon_Rollout.RolloutReconType.Rollback, launchSite) != null;
                        if (rolloutEnabled && !HighLogic.LoadedSceneIsEditor && recovery == null && !siteHasActiveRolloutOrRollback) //rollout if the pad isn't busy
                        {
                            bool hasRecond = false;
                            GUIStyle btnColor = greenButton;
                            if (KCT_GameStates.ActiveKSC.ActiveLPInstance.destroyed)
                                btnColor = redButton;
                            else if (hasRecond = KCT_GameStates.ActiveKSC.GetReconditioning(KCT_GameStates.ActiveKSC.ActiveLPInstance.name) != null)
                                btnColor = yellowButton;
                            KCT_Recon_Rollout tmpRollout = new KCT_Recon_Rollout(b, KCT_Recon_Rollout.RolloutReconType.Rollout, b.id.ToString(), launchSite);
                            if (tmpRollout.cost > 0d)
                                GUILayout.Label("√" + tmpRollout.cost.ToString("N0"));
                            string rolloutText = (i == MouseOnRolloutButton ? MagiCore.Utilities.GetColonFormattedTime(tmpRollout.AsBuildItem().GetTimeLeft()) : "Rollout");
                            if (GUILayout.Button(rolloutText, btnColor, GUILayout.ExpandWidth(false)))
                            {
                                if (KCT_PresetManager.Instance.ActivePreset.generalSettings.ReconditioningBlocksPad && hasRecond)
                                {
                                    PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "cannotRollOutReconditioningPopup", "Cannot Roll out!", "You must finish reconditioning the launchpad before you can roll out to it!", "Acknowledged", false, HighLogic.UISkin);
                                }
                                else
                                {
                                    List<string> facilityChecks = b.MeetsFacilityRequirements(false);
                                    if (facilityChecks.Count == 0)
                                    {
                                        if (!KCT_GameStates.ActiveKSC.ActiveLPInstance.destroyed)
                                        {
                                            b.launchSiteID = KCT_GameStates.ActiveKSC.ActiveLaunchPadID;

                                            if (rollout != null)
                                            {
                                                rollout.SwapRolloutType();
                                            }
                                            // tmpRollout.launchPadID = KCT_GameStates.ActiveKSC.ActiveLPInstance.name;
                                            KCT_GameStates.ActiveKSC.Recon_Rollout.Add(tmpRollout);
                                        }
                                        else
                                        {
                                            PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "cannotLaunchRepairPopup", "Cannot Launch!", "You must repair the launchpad before you can launch a vessel from it!", "Acknowledged", false, HighLogic.UISkin);
                                        }
                                    }
                                    else
                                    {
                                        PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "cannotLaunchEditorChecksPopup", "Cannot Launch!", "Warning! This vessel did not pass the editor checks! Until you upgrade the VAB and/or Launchpad it cannot be launched. Listed below are the failed checks:\n" + String.Join("\n", facilityChecks.ToArray()), "Acknowledged", false, HighLogic.UISkin);
                                    }
                                }
                            }
                            if (Event.current.type == EventType.Repaint)
                                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                                    MouseOnRolloutButton = i;
                                else if (i == MouseOnRolloutButton)
                                    MouseOnRolloutButton = -1;
                        }
                        else if (rolloutEnabled && !HighLogic.LoadedSceneIsEditor && recovery == null && rollout != null && b.id.ToString() == rollout.associatedID && !rollout.AsBuildItem().IsComplete() && rollback == null &&
                            GUILayout.Button(MagiCore.Utilities.GetColonFormattedTime(rollout.AsBuildItem().GetTimeLeft()), GUILayout.ExpandWidth(false))) //swap rollout to rollback
                        {
                            rollout.SwapRolloutType();
                        }
                        else if (rolloutEnabled && !HighLogic.LoadedSceneIsEditor && recovery == null && rollback != null && !rollback.AsBuildItem().IsComplete())
                        {
                            if (rollout == null)
                            {
                                if (GUILayout.Button(MagiCore.Utilities.GetColonFormattedTime(rollback.AsBuildItem().GetTimeLeft()), GUILayout.ExpandWidth(false))) //switch rollback back to rollout
                                    rollback.SwapRolloutType();
                            }
                            else
                            {
                                GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(rollback.AsBuildItem().GetTimeLeft()), GUILayout.ExpandWidth(false));
                            }
                        }
                        else if (HighLogic.LoadedScene != GameScenes.TRACKSTATION && recovery == null && (!rolloutEnabled || (rollout != null && b.id.ToString() == rollout.associatedID && rollout.AsBuildItem().IsComplete())))
                        {
                            KCT_LaunchPad pad = KCT_GameStates.ActiveKSC.LaunchPads.Find(lp => lp.name == launchSite);
                            bool operational = pad!=null ? !pad.destroyed : !KCT_GameStates.ActiveKSC.ActiveLPInstance.destroyed;
                            GUIStyle btnColor = greenButton;
                            string launchTxt = "Launch";
                            if (!operational)
                            {
                                launchTxt = "Repairs Required";
                                btnColor = redButton;
                            }
                            else if (KCT_Utilities.ReconditioningActive(null, launchSite))
                            {
                                launchTxt = "Reconditioning";
                                btnColor = yellowButton;
                            }
                            if (rolloutEnabled && GameSettings.MODIFIER_KEY.GetKey() && GUILayout.Button("Roll Back", GUILayout.ExpandWidth(false)))
                            {
                                rollout.SwapRolloutType();
                            }
                            else if (!GameSettings.MODIFIER_KEY.GetKey() && GUILayout.Button(launchTxt, btnColor, GUILayout.ExpandWidth(false)))
                            {
                                if (b.launchSiteID >= 0)
                                {
                                    KCT_GameStates.ActiveKSC.SwitchLaunchPad(b.launchSiteID);
                                }
                                b.launchSiteID = KCT_GameStates.ActiveKSC.ActiveLaunchPadID;

                                List<string> facilityChecks = b.MeetsFacilityRequirements(false);
                                if (facilityChecks.Count == 0)
                                {
                                   // bool operational = !KCT_GameStates.ActiveKSC.ActiveLPInstance.destroyed;// && KCT_Utilities.LaunchFacilityIntact(KCT_BuildListVessel.ListType.VAB);//new PreFlightTests.FacilityOperational("LaunchPad", "building").Test();
                                    if (!operational)
                                    {
                                        //ScreenMessages.PostScreenMessage("You must repair the launchpad prior to launch!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                                        PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "cannotLaunchRepairPopup", "Cannot Launch!", "You must repair the launchpad before you can launch a vessel from it!", "Acknowledged", false, HighLogic.UISkin);
                                    }
                                    else if (KCT_Utilities.ReconditioningActive(null, launchSite))
                                    {
                                        //can't launch now
                                        ScreenMessage message = new ScreenMessage("[KCT] Cannot launch while LaunchPad is being reconditioned. It will be finished in "
                                            + MagiCore.Utilities.GetFormattedTime(((IKCTBuildItem)KCT_GameStates.ActiveKSC.GetReconditioning(launchSite)).GetTimeLeft()), 4.0f, ScreenMessageStyle.UPPER_CENTER);
                                        ScreenMessages.PostScreenMessage(message);
                                    }
                                    else
                                    {
                                        /*if (rollout != null)
                                            KCT_GameStates.ActiveKSC.Recon_Rollout.Remove(rollout);*/
                                        KCT_GameStates.launchedVessel = b;
                                        KCT_GameStates.launchedVessel.KSC = null;
                                        if (ShipConstruction.FindVesselsLandedAt(HighLogic.CurrentGame.flightState, b.launchSite).Count == 0)//  ShipConstruction.CheckLaunchSiteClear(HighLogic.CurrentGame.flightState, "LaunchPad", false))
                                        {
                                            showBLPlus = false;
                                            // buildList.RemoveAt(i);
                                            if (!IsCrewable(b.ExtractedParts))
                                                b.Launch();
                                            else
                                            {
                                                showBuildList = false;
                                                if (KCT_Events.instance.KCTButtonStock != null)
                                                {
                                                    KCT_Events.instance.KCTButtonStock.SetFalse();
                                                }

                                                centralWindowPosition.height = 1;
                                                AssignInitialCrew();
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
                                else
                                {
                                    PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "cannotLaunchEditorChecksPopup", "Cannot Launch!", "Warning! This vessel did not pass the editor checks! Until you upgrade the VAB and/or Launchpad it cannot be launched. Listed below are the failed checks:\n" + String.Join("\n", facilityChecks.ToArray()), "Acknowledged", false, HighLogic.UISkin);
                                }
                            }
                        }
                        else if (!HighLogic.LoadedSceneIsEditor && recovery != null)
                        {
                            GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(recovery.AsBuildItem().GetTimeLeft()), GUILayout.ExpandWidth(false));
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.BeginHorizontal();
                int lpCount = KCT_GameStates.ActiveKSC.LaunchPadCount;
                if (lpCount > 1 && GUILayout.Button("<<", GUILayout.ExpandWidth(false)))
                {
                    //Simple fix for mod function being "weird" in the negative direction
                    //http://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
                    KCT_GameStates.ActiveKSC.SwitchLaunchPad(((KCT_GameStates.ActiveKSC.ActiveLaunchPadID - 1) % lpCount + lpCount) % lpCount);
                    if (HighLogic.LoadedSceneIsEditor)
                    {
                        KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label("Current: " + KCT_GameStates.ActiveKSC.ActiveLPInstance.name+" ("+(KCT_GameStates.ActiveKSC.ActiveLPInstance.level+1)+")");
                if (costOfNewLP == -13)
                    costOfNewLP = KCT_MathParsing.GetStandardFormulaValue("NewLaunchPadCost", new Dictionary<string, string> { { "N", KCT_GameStates.ActiveKSC.LaunchPads.Count.ToString() } });
              //  if (KCT_Utilities.KSCSwitcherInstalled) //todo
              //      costOfNewLP = -1; //disable purchasing additional launchpads when playing with KSC Switcher (until upgrades are properly per KSC)
                if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false)))
                {
                    renamingLaunchPad = true;
                    newName = KCT_GameStates.ActiveKSC.ActiveLPInstance.name;
                    showRename = true;
                    showBuildList = false;
                    showBLPlus = false;
                }
                if (costOfNewLP >= 0 && GUILayout.Button("New", GUILayout.ExpandWidth(false)))
                {
                    //open dialog to unlock new
                    costOfNewLP = KCT_MathParsing.GetStandardFormulaValue("NewLaunchPadCost", new Dictionary<string, string> { { "N", KCT_GameStates.ActiveKSC.LaunchPads.Count.ToString() } });
                    DialogGUIBase[] options = new DialogGUIBase[2];
                    options[0] = new DialogGUIButton("Yes", () =>
                    {
                        if (!KCT_Utilities.CurrentGameIsCareer())
                        {
                            KCTDebug.Log("Building new launchpad!");
                            KCT_GameStates.ActiveKSC.LaunchPads.Add(new KCT_LaunchPad("LaunchPad " + (KCT_GameStates.ActiveKSC.LaunchPads.Count + 1), 2));
                        }
                        else if (Funding.CanAfford((float)costOfNewLP))
                        {
                            KCTDebug.Log("Building new launchpad!");
                            //take the funds
                            KCT_Utilities.SpendFunds(costOfNewLP, TransactionReasons.StructureConstruction);
                            //create new launchpad at level -1
                            KCT_GameStates.ActiveKSC.LaunchPads.Add(new KCT_LaunchPad("LaunchPad " + (KCT_GameStates.ActiveKSC.LaunchPads.Count + 1), -1));
                            //create new upgradeable
                            KCT_UpgradingBuilding newPad = new KCT_UpgradingBuilding();//(null, 0, -1, "LaunchPad");
                            newPad.id = "SpaceCenter/LaunchPad";
                            newPad.isLaunchpad = true;
                            newPad.launchpadID = KCT_GameStates.ActiveKSC.LaunchPads.Count-1;
                            newPad.upgradeLevel = 0;
                            newPad.currentLevel = -1;
                            newPad.cost = costOfNewLP;
                            newPad.SetBP(costOfNewLP);
                            newPad.commonName = "LaunchPad " + (KCT_GameStates.ActiveKSC.LaunchPads.Count);
                            KCT_GameStates.ActiveKSC.KSCTech.Add(newPad);

                        }
                        costOfNewLP = -13;
                    });
                    options[1] = new DialogGUIButton("No", DummyVoid);
                    MultiOptionDialog diag = new MultiOptionDialog("newLaunchpadPopup", "It will cost " + Math.Round(costOfNewLP, 2).ToString("N") + " funds to build a new launchpad. Would you like to build it?", "Build LaunchPad", null, 300, options);
                    PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), diag, false, HighLogic.UISkin);
                }
                GUILayout.FlexibleSpace();
                if (lpCount > 1 && GUILayout.Button(">>", GUILayout.ExpandWidth(false)))
                {
                    KCT_GameStates.ActiveKSC.SwitchLaunchPad((KCT_GameStates.ActiveKSC.ActiveLaunchPadID + 1) % KCT_GameStates.ActiveKSC.LaunchPadCount);
                    if (HighLogic.LoadedSceneIsEditor)
                    {
                        KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                    }
                }
                GUILayout.EndHorizontal();
            }
            else if (listWindow == 1) //SPH Build List
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.ActiveKSC.SPHList;
                GUILayout.BeginHorizontal();
              //  GUILayout.Space((butW + 4) * 3);
                GUILayout.Label("Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1 / 2));
                GUILayout.Label("Time Left:", GUILayout.Width(width2));
                //GUILayout.Label("BP:", GUILayout.Width(width1 / 2 + 10));
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));
                {
                    if (buildList.Count == 0)
                    {
                        GUILayout.Label("No vessels under construction! Go to the SPH to build more.");
                    }
                    for (int i = 0; i < buildList.Count; i++)
                    {
                        KCT_BuildListVessel b = buildList[i];
                        if (!b.allPartsValid)
                            continue;
                        GUILayout.BeginHorizontal();
                        if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("*", GUILayout.Width(butW)))
                        {
                            if (IDSelected == b.id)
                                showBLPlus = !showBLPlus;
                            else
                                showBLPlus = true;
                            IDSelected = b.id;
                        }
                        else if (HighLogic.LoadedSceneIsEditor)
                        {
                            //GUILayout.Space(butW);
                            if (GUILayout.Button("X", GUILayout.Width(butW)))
                            {
                                InputLockManager.SetControlLock(ControlTypes.EDITOR_SOFT_LOCK, "KCTPopupLock");
                                IDSelected = b.id;
                                DialogGUIBase[] options = new DialogGUIBase[2];
                                options[0] = new DialogGUIButton("Yes", ScrapVessel);
                                options[1] = new DialogGUIButton("No", DummyVoid);
                                MultiOptionDialog diag = new MultiOptionDialog("scrapConfirmPopup", "Are you sure you want to scrap " + b.shipName + "?", "Scrap Vessel", null, 300, options);
                                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), diag, false, HighLogic.UISkin);
                            }
                        }

                        if (i > 0 && GUILayout.Button("^", GUILayout.Width(butW)))
                        {
                            buildList.RemoveAt(i);
                            if (GameSettings.MODIFIER_KEY.GetKey())
                            {
                                buildList.Insert(0, b);
                            }
                            else
                            {
                                buildList.Insert(i - 1, b);
                            }
                        }
                        else if (i == 0)
                        {
                  //          GUILayout.Space(butW + 4);
                        }
                        if (i < buildList.Count - 1 && GUILayout.Button("v", GUILayout.Width(butW)))
                        {
                            buildList.RemoveAt(i);
                            if (GameSettings.MODIFIER_KEY.GetKey())
                            {
                                buildList.Add(b);
                            }
                            else
                            {
                                buildList.Insert(i + 1, b);
                            }
                        }
                        else if (i >= buildList.Count - 1)
                        {
                   //         GUILayout.Space(butW + 4);
                        }
                        
                        GUILayout.Label(b.shipName);
                        GUILayout.Label(Math.Round(b.ProgressPercent(), 2).ToString() + "%", GUILayout.Width(width1 / 2));
                        if (b.buildRate > 0)
                            GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(b.timeLeft), GUILayout.Width(width2));
                        else
                            GUILayout.Label("Est: " + MagiCore.Utilities.GetColonFormattedTime((b.buildPoints - b.progress) / KCT_Utilities.GetBuildRate(0, KCT_BuildListVessel.ListType.SPH, null)), GUILayout.Width(width2));
                        //GUILayout.Label(Math.Round(b.buildPoints, 2).ToString(), GUILayout.Width(width1 / 2 + 10));
                        GUILayout.EndHorizontal();
                    }

                    buildList = KCT_GameStates.ActiveKSC.SPHWarehouse;
                    GUILayout.Label("__________________________________________________");
                    GUILayout.Label("SPH Storage");
                    if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.IsRecoverable && FlightGlobals.ActiveVessel.IsClearToSave() == ClearToSaveStatus.CLEAR && GUILayout.Button("Recover Active Vessel"))
                    {
                        if (!KCT_Utilities.RecoverActiveVesselToStorage(KCT_BuildListVessel.ListType.SPH))
                        {
                            PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "recoverShipErrorPopup", "Error!", "There was an error while recovering the ship. Sometimes reloading the scene and trying again works. Sometimes a vessel just can't be recovered this way and you must use the stock recover system.", "OK", false, HighLogic.UISkin);
                        }
                    }

                    for (int i = 0; i < buildList.Count; i++)
                    {
                        KCT_BuildListVessel b = buildList[i];
                        if (!b.allPartsValid)
                            continue;
                        string status = "";
                        KCT_Recon_Rollout recovery = KCT_GameStates.ActiveKSC.Recon_Rollout.FirstOrDefault(r => r.associatedID == b.id.ToString() && r.RRType == KCT_Recon_Rollout.RolloutReconType.Recovery);
                        if (recovery != null)
                            status = "Recovering";

                        GUILayout.BeginHorizontal();
                        if (!HighLogic.LoadedSceneIsEditor && status == "")
                        {
                            if (GUILayout.Button("*", GUILayout.Width(butW)))
                            {
                                if (IDSelected == b.id)
                                    showBLPlus = !showBLPlus;
                                else
                                    showBLPlus = true;
                                IDSelected = b.id;
                            }
                        }
                        else
                            GUILayout.Space(butW + 4);

                        GUILayout.Label(b.shipName);
                        GUILayout.Label(status + "   ", GUILayout.ExpandWidth(false));
                        //ScenarioDestructibles.protoDestructibles["KSCRunway"].
                        if (HighLogic.LoadedScene != GameScenes.TRACKSTATION && recovery == null && GUILayout.Button("Launch", GUILayout.ExpandWidth(false)))
                        {
                            List<string> facilityChecks = b.MeetsFacilityRequirements(false);
                            if (facilityChecks.Count == 0)
                            {
                                bool operational = KCT_Utilities.LaunchFacilityIntact(KCT_BuildListVessel.ListType.SPH);//new PreFlightTests.FacilityOperational("Runway", "building").Test();
                                if (!operational)
                                {
                                    ScreenMessages.PostScreenMessage("You must repair the runway prior to launch!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                                }
                                else
                                {
                                    showBLPlus = false;
                                    KCT_GameStates.launchedVessel = b;
                                    KCT_GameStates.launchedVessel.KSC = null;
                                    if (ShipConstruction.FindVesselsLandedAt(HighLogic.CurrentGame.flightState, "Runway").Count == 0)
                                    {
                                        if (!IsCrewable(b.ExtractedParts))
                                            b.Launch();
                                        else
                                        {
                                            showBuildList = false;
                                            if (KCT_Events.instance.KCTButtonStock != null)
                                            {
                                                KCT_Events.instance.KCTButtonStock.SetFalse();
                                            }
                                            centralWindowPosition.height = 1;
                                            AssignInitialCrew();
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
                            else
                            {
                                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "cannotLaunchEditorChecksPopup", "Cannot Launch!", "Warning! This vessel did not pass the editor checks! Until you upgrade the SPH and/or Runway it cannot be launched. Listed below are the failed checks:\n" + String.Join("\n", facilityChecks.ToArray()), "Acknowledged", false, HighLogic.UISkin);
                            }
                        }
                        else if (recovery != null)
                        {
                            GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(recovery.AsBuildItem().GetTimeLeft()), GUILayout.ExpandWidth(false));
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (buildList.Count == 0)
                    {
                        GUILayout.Label("No vessels in storage!\nThey will be stored here when they are complete.");
                    }
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow == 2) //Tech nodes
            {
                List<KCT_UpgradingBuilding> KSCList = KCT_GameStates.ActiveKSC.KSCTech;
                List<KCT_TechItem> techList = KCT_GameStates.TechList;
                //GUILayout.Label("Tech Node Research");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1/2));
                GUILayout.Label("Time Left:", GUILayout.Width(width1));
                GUILayout.Space(70);
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));

                if (KCT_Utilities.CurrentGameIsCareer())
                {
                if (KSCList.Count == 0)
                        GUILayout.Label("No KSC upgrade projects are currently underway.");
                foreach (KCT_UpgradingBuilding KCTTech in KSCList)
                {
                    GUILayout.BeginHorizontal();
                    /*
                    int i = KSCList.IndexOf(KCTTech);
                    if (i > 0 && GUILayout.Button("^", GUILayout.Width(butW)))
                    {
                        KSCList.RemoveAt(i);
                        if (GameSettings.MODIFIER_KEY.GetKey())
                        {
                            KSCList.Insert(0, KCTTech);
                        }
                        else
                        {
                            KSCList.Insert(i - 1, KCTTech);
                        }
                    }
                    if (i < KSCList.Count - 1 && GUILayout.Button("v", GUILayout.Width(butW)))
                    {
                        KSCList.RemoveAt(i);
                        if (GameSettings.MODIFIER_KEY.GetKey())
                        {
                            KSCList.Add(KCTTech);
                        }
                        else
                        {
                            KSCList.Insert(i + 1, KCTTech);
                        }
                    }
                    */

                    GUILayout.Label(KCTTech.AsIKCTBuildItem().GetItemName());
                    GUILayout.Label(Math.Round(100 * KCTTech.progress / KCTTech.BP, 2) + " %", GUILayout.Width(width1 / 2));
                    GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(KCTTech.AsIKCTBuildItem().GetTimeLeft()), GUILayout.Width(width1));
                    if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("Warp", GUILayout.Width(70)))
                    {
                        KCT_GameStates.targetedItem = KCTTech;
                        KCT_GameStates.canWarp = true;
                        KCT_Utilities.RampUpWarp(KCTTech);
                        KCT_GameStates.warpInitiated = true;
                        /*if (KCTTech.AsIKCTBuildItem().GetBuildRate() > 0)
                        {
                            TimeWarp.fetch.WarpTo(Planetarium.GetUniversalTime() + KCTTech.AsIKCTBuildItem().GetTimeLeft(), KCT_GameStates.settings.MaxTimeWarp, 1);
                        }*/
                    }
                    else if (HighLogic.LoadedSceneIsEditor)
                        GUILayout.Space(70);
                    GUILayout.EndHorizontal();
                }
            }

                if (techList.Count == 0)
                    GUILayout.Label("No tech nodes are being researched!\nBegin research by unlocking tech in the R&D building.");
                bool forceRecheck = false;
                int cancelID = -1;
                for (int i = 0; i < techList.Count; i++)
                {
                    KCT_TechItem t = techList[i];
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("X", GUILayout.Width(butW)))
                    {
                        forceRecheck = true;
                        cancelID = i;
                        DialogGUIBase[] options = new DialogGUIBase[2];
                        options[0] = new DialogGUIButton("Yes", () => { CancelTechNode(cancelID); });
                        options[1] = new DialogGUIButton("No", DummyVoid);
                        MultiOptionDialog diag = new MultiOptionDialog("cancelNodePopup", "Are you sure you want to stop researching "+t.techName+"?", "Cancel Node?", null, 300, options);
                        PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), diag, false, HighLogic.UISkin);

                        /*if (CancelTechNode(i))
                        {
                            i--;
                            GUILayout.EndHorizontal();
                            continue;
                        }*/
                    }

                    if (i > 0 && t.BuildRate != techList[0].BuildRate)
                    {
                        if (i > 0 && GUILayout.Button("^", GUILayout.Width(butW)))
                        {
                            techList.RemoveAt(i);
                            if (GameSettings.MODIFIER_KEY.GetKey())
                            {
                                techList.Insert(0, t);
                            }
                            else
                            {
                                techList.Insert(i - 1, t);
                            }
                            forceRecheck = true;
                        }
                    }
                    if ((i == 0 && t.BuildRate != techList[techList.Count - 1].BuildRate) || t.BuildRate != techList[techList.Count - 1].BuildRate)
                    {
                        if (i < techList.Count - 1 && GUILayout.Button("v", GUILayout.Width(butW)))
                        {
                            techList.RemoveAt(i);
                            if (GameSettings.MODIFIER_KEY.GetKey())
                            {
                                techList.Add(t);
                            }
                            else
                            {
                                techList.Insert(i + 1, t);
                            }
                            forceRecheck = true;
                        }
                    }
                    if (forceRecheck)
                    {
                        forceRecheck = false;
                        for (int j=0; j<techList.Count; j++)
                            techList[j].UpdateBuildRate(j);
                    }

                    GUILayout.Label(t.techName);
                    GUILayout.Label(Math.Round(100 * t.progress / t.scienceCost, 2) + " %", GUILayout.Width(width1/2));
                    if (t.BuildRate > 0)
                        GUILayout.Label(MagiCore.Utilities.GetColonFormattedTime(t.TimeLeft), GUILayout.Width(width1));
                    else
                        GUILayout.Label("Est: " + MagiCore.Utilities.GetColonFormattedTime(t.EstimatedTimeLeft), GUILayout.Width(width1));
                    if (t.BuildRate > 0)
                    {
                        if (!HighLogic.LoadedSceneIsEditor && GUILayout.Button("Warp", GUILayout.Width(45)))
                        {
                            KCT_GameStates.targetedItem = t;
                            KCT_GameStates.canWarp = true;
                            KCT_Utilities.RampUpWarp(t);
                            KCT_GameStates.warpInitiated = true;
                        }
                        else if (HighLogic.LoadedSceneIsEditor)
                            GUILayout.Space(45);
                    }
                    else
                        GUILayout.Space(45);
                    
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }

            if (KCT_UpdateChecker.UpdateFound)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Version: " + KCT_UpdateChecker.CurrentVersion);
                GUILayout.Label("Latest: " + KCT_UpdateChecker.WebVersion);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

           // if (ToolbarManager.ToolbarAvailable && ToolbarManager.Instance != null && KCT_GameStates.settings.PreferBlizzyToolbar)
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();

            ClampWindow(ref buildListWindowPosition, strict: true);
        }

        public static void CancelTechNode(int index)
        {
            
            if (KCT_GameStates.TechList.Count > index)
            {
                KCT_TechItem node = KCT_GameStates.TechList[index];
                KCTDebug.Log("Cancelling tech: " + node.techName);
                if (KCT_Utilities.CurrentGameHasScience())
                {
                    ResearchAndDevelopment.Instance.AddScience(node.scienceCost, TransactionReasons.None); //Should maybe do tech research as the reason
                }
                node.DisableTech();
                KCT_GameStates.TechList.RemoveAt(index);
            }
        }

        private static Guid IDSelected = new Guid();
        private static void DrawBLPlusWindow(int windowID)
        {
            //bLPlusPosition.xMax = buildListWindowPosition.xMin;
            //bLPlusPosition.width = 100;
            bLPlusPosition.yMin = buildListWindowPosition.yMin;
            bLPlusPosition.height = 225;
            //bLPlusPosition.height = bLPlusPosition.yMax - bLPlusPosition.yMin;
            KCT_BuildListVessel b = KCT_Utilities.FindBLVesselByID(IDSelected);
            GUILayout.BeginVertical();
            string launchSite = b.launchSite;
            float rbMultiplier = KCT_PresetManager.Instance.ActivePreset.generalSettings.RushMultiplier;

            if (launchSite == "LaunchPad")
            {
                if (b.launchSiteID >= 0)
                    launchSite = b.KSC.LaunchPads[b.launchSiteID].name;
                else
                    launchSite = b.KSC.ActiveLPInstance.name;
            }
            KCT_Recon_Rollout rollout = KCT_GameStates.ActiveKSC.GetReconRollout(KCT_Recon_Rollout.RolloutReconType.Rollout, launchSite);
            bool onPad = rollout != null && rollout.AsBuildItem().IsComplete() && rollout.associatedID == b.id.ToString();
            //This vessel is rolled out onto the pad
            /* 1.4 Addition
            if (!onPad && GUILayout.Button("Select LaunchSite"))
            {
                launchSites = KCT_Utilities.GetLaunchSites(b.type == KCT_BuildListVessel.ListType.VAB);

                if (launchSites.Any())
                {
                    showBLPlus = false;
                    showLaunchSiteSelector = true;
                    centralWindowPosition.width = 300;
                }
                else
                {
                    PopupDialog.SpawnPopupDialog(new MultiOptionDialog("KCTNoLaunchsites", "No launch sites available to choose from. Try visiting an editor first.", "No Launch Sites", null, new DialogGUIButton("OK", () => { })), false, HighLogic.UISkin);
                }
            }
            */
            if (!onPad && GUILayout.Button("Scrap"))
            {
                InputLockManager.SetControlLock(ControlTypes.KSC_ALL, "KCTPopupLock");
                DialogGUIBase[] options = new DialogGUIBase[2];
                options[0] = new DialogGUIButton("Yes", ScrapVessel);
                options[1] = new DialogGUIButton("No", DummyVoid);
                MultiOptionDialog diag = new MultiOptionDialog("scrapVesselConfirmPopup", "Are you sure you want to scrap this vessel?", "Scrap Vessel", null, 300, options);
                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), diag, false, HighLogic.UISkin);
                showBLPlus = false;
                ResetBLWindow();
            }
            if (!onPad && GUILayout.Button("Edit"))
            {
                showBLPlus = false;
                editorWindowPosition.height = 1;
                string tempFile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/Ships/temp.craft";
                b.shipNode.Save(tempFile);
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                KCT_GameStates.editedVessel = b;
                KCT_GameStates.editedVessel.KSC = null;
                KCT_GameStates.EditorShipEditingMode = true;

                InputLockManager.SetControlLock(ControlTypes.EDITOR_EXIT, "KCTEditExit");
                InputLockManager.SetControlLock(ControlTypes.EDITOR_LOAD, "KCTEditLoad");
                InputLockManager.SetControlLock(ControlTypes.EDITOR_NEW, "KCTEditNew");
                InputLockManager.SetControlLock(ControlTypes.EDITOR_LAUNCH, "KCTEditLaunch");

                EditorDriver.StartAndLoadVessel(tempFile, b.type == KCT_BuildListVessel.ListType.VAB ? EditorFacility.VAB : EditorFacility.SPH);
            }
            if (GUILayout.Button("Rename"))
            {
                centralWindowPosition.width = 360;
                centralWindowPosition.x = (Screen.width - 360) / 2;
                centralWindowPosition.height = 1;
                showBuildList = false;
                showBLPlus = false;
                showRename = true;
                newName = b.shipName;
                renamingLaunchPad = false;
                //newDesc = b.getShip().shipDescription;
            }
            if (GUILayout.Button("Duplicate"))
            {
                KCT_Utilities.AddVesselToBuildList(b.NewCopy(true));
            }
            if (KCT_GameStates.ActiveKSC.Recon_Rollout.Find(rr => rr.RRType == KCT_Recon_Rollout.RolloutReconType.Rollout && rr.associatedID == b.id.ToString()) != null && GUILayout.Button("Rollback"))
            {
                KCT_GameStates.ActiveKSC.Recon_Rollout.Find(rr => rr.RRType == KCT_Recon_Rollout.RolloutReconType.Rollout && rr.associatedID == b.id.ToString()).SwapRolloutType();
                showBLPlus = false;
            }
            if (!b.isFinished && GUILayout.Button("Warp To"))
            {
                KCT_GameStates.targetedItem = b;
                KCT_GameStates.canWarp = true;
                KCT_Utilities.RampUpWarp(b);
                KCT_GameStates.warpInitiated = true;
                showBLPlus = false;
               /* if (b.buildRate > 0)
                {
                    TimeWarp.fetch.WarpTo(Planetarium.GetUniversalTime() + b.timeLeft, KCT_GameStates.settings.MaxTimeWarp, 1);
                }*/
            }
            if (!b.isFinished && GUILayout.Button("Move to Top"))
            {
                if (b.type == KCT_BuildListVessel.ListType.VAB)
                {
                    b.RemoveFromBuildList();
                    KCT_GameStates.ActiveKSC.VABList.Insert(0, b);
                }
                else if (b.type == KCT_BuildListVessel.ListType.SPH)
                {
                    b.RemoveFromBuildList();
                    KCT_GameStates.ActiveKSC.SPHList.Insert(0, b);
                }
            }
            if (!b.isFinished 
                && (KCT_PresetManager.Instance.ActivePreset.generalSettings.MaxRushClicks == 0 || b.rushBuildClicks < KCT_PresetManager.Instance.ActivePreset.generalSettings.MaxRushClicks) 
                && GUILayout.Button("Rush Build 10%\n√" + Math.Round(rbMultiplier * b.GetTotalCost())))
            {
                double cost = b.GetTotalCost();
                double rush = cost * rbMultiplier;
                double remainingBP = b.buildPoints - b.progress;
                if (Funding.Instance.Funds >= rush)
                {
                    b.AddProgress(remainingBP * 0.1);
                    KCT_Utilities.SpendFunds(rush, TransactionReasons.None);
                    ++b.rushBuildClicks;
                }

            }
            if (GUILayout.Button("Close"))
            {
                showBLPlus = false;
            }
            GUILayout.EndVertical();
            float width = bLPlusPosition.width;
            bLPlusPosition.x = buildListWindowPosition.x - width;
            bLPlusPosition.width = width;
        }


        private static Vector2 launchSiteScrollView;
        public static void DrawLaunchSiteChooser(int windowID)
        {
            GUILayout.BeginVertical();
            launchSiteScrollView = GUILayout.BeginScrollView(launchSiteScrollView, GUILayout.Height((float)Math.Min(Screen.height * 0.75, 25*launchSites.Count + 10)));

            foreach (string launchsite in launchSites)
            {
                if (GUILayout.Button(launchsite))
                {
                    //Set the chosen vessel's launch site to the selected site
                    KCT_BuildListVessel blv = KCT_Utilities.FindBLVesselByID(IDSelected);
                    blv.launchSite = launchsite;
                    showLaunchSiteSelector = false;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            CenterWindow(ref centralWindowPosition);
        }
    }
}
