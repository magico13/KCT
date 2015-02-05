using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    public static partial class KCT_GUI
    {
        public static bool showMainGUI, showEditorGUI, showSOIAlert, showLaunchAlert, showSimulationCompleteEditor, showSimulationWindow, showTimeRemaining, 
            showSimulationCompleteFlight, showBuildList, showClearLaunch, showShipRoster, showCrewSelect, showSettings, showSimConfig, showBodyChooser, showUpgradeWindow,
            showBLPlus, showRename, showFirstRun, showSimLengthChooser;

        public static bool clicked = false;

        public static GUIDataSaver guiDataSaver = new GUIDataSaver();

        private static bool unlockEditor;

        private static Vector2 scrollPos;

        private static Rect iconPosition = new Rect(Screen.width / 4, Screen.height - 30, 50, 30);//110
        private static Rect mainWindowPosition = new Rect(Screen.width / 3.5f, Screen.height / 3.5f, 350, 200);
        public static Rect editorWindowPosition = new Rect(Screen.width / 3.5f, Screen.height / 3.5f, 275, 135);
        private static Rect SOIAlertPosition = new Rect(Screen.width / 3, Screen.height / 3, 250, 100);

        private static Rect centralWindowPosition = new Rect((Screen.width - 150) / 2, (Screen.height - 50) / 2, 150, 50);

        //private static Rect launchAlertPosition = new Rect((Screen.width-75)/2, (Screen.height-100)/2, 150, 100);
        //private static Rect simulationCompleteEditorPosition = new Rect((Screen.width - 75) / 2, (Screen.height - 100) / 2, 150, 100);
        //private static Rect simulationCompleteFlightPosition = new Rect((Screen.width - 75) / 2, (Screen.height - 100) / 2, 150, 100);
        private static Rect simulationWindowPosition = new Rect((Screen.width - 250) / 2, (Screen.height - 250) / 2, 250, 1);
        public static Rect timeRemainingPosition = new Rect((Screen.width-90) / 4, Screen.height - 85, 90, 55);
        public static Rect buildListWindowPosition = new Rect(Screen.width - 400, 40, 400, 1);
        private static Rect crewListWindowPosition = new Rect((Screen.width-360)/2, (Screen.height / 4), 360, 1);
        private static Rect settingsPosition = new Rect((3 * Screen.width / 8), (Screen.height / 4), 300, 1);
        private static Rect upgradePosition = new Rect((3 * Screen.width / 8), (Screen.height / 4), 240, 1);
        private static Rect simulationConfigPosition = new Rect((Screen.width / 2)-150, (Screen.height / 4), 300, 1);
        private static Rect bLPlusPosition = new Rect(Screen.width-500, 40, 100, 1);

        private static GUISkin windowSkin;// = HighLogic.Skin;// = new GUIStyle(HighLogic.Skin.window);

        private static bool isKSCLocked = false, isEditorLocked = false;


        private static List<GameScenes> validScenes = new List<GameScenes> { GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.SPACECENTER, GameScenes.TRACKSTATION };
        public static void SetGUIPositions(GUI.WindowFunction OnWindow)
        {
            GUISkin oldSkin = GUI.skin;
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && windowSkin == null)
                windowSkin = GUI.skin;
            GUI.skin = windowSkin;

            if (validScenes.Contains(HighLogic.LoadedScene)) //&& KCT_GameStates.settings.enabledForSave)//!(HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX && !KCT_GameStates.settings.SandboxEnabled))
            {
                /*if (!ToolbarManager.ToolbarAvailable && GUI.Button(iconPosition, "KCT", GUI.skin.button))
                {
                    onClick();
                }*/
                if (ToolbarManager.ToolbarAvailable && KCT_GameStates.kctToolbarButton != null)
                {
                    KCT_GameStates.kctToolbarButton.TexturePath = KCT_Utilities.GetButtonTexture(); //Set texture, allowing for flashing of icon.
                }


                if (showSettings)
                    settingsPosition = GUILayout.Window(8955, settingsPosition, KCT_GUI.DrawSettings, "KCT Settings", HighLogic.Skin.window);
                if (!KCT_GameStates.settings.enabledForSave)
                    return;

                if (showMainGUI)
                    mainWindowPosition = GUILayout.Window(8950, mainWindowPosition, KCT_GUI.DrawMainGUI, "Kerbal Construction Time", HighLogic.Skin.window);
                if (showEditorGUI)
                    editorWindowPosition = GUILayout.Window(8953, editorWindowPosition, KCT_GUI.DrawEditorGUI, "Kerbal Construction Time", HighLogic.Skin.window);
                if (showSOIAlert)
                    SOIAlertPosition = GUILayout.Window(8951, SOIAlertPosition, KCT_GUI.DrawSOIAlertWindow, "SOI Change", HighLogic.Skin.window);
                if (showLaunchAlert)
                    centralWindowPosition = GUILayout.Window(8951, centralWindowPosition, KCT_GUI.DrawLaunchAlert, "KCT", HighLogic.Skin.window);
                if (showSimulationCompleteEditor)
                    centralWindowPosition = GUILayout.Window(8951, centralWindowPosition, KCT_GUI.DrawSimulationCompleteEditor, "Simulation Complete!", HighLogic.Skin.window);
                if (showSimulationCompleteFlight)
                    centralWindowPosition = GUILayout.Window(8952, centralWindowPosition, KCT_GUI.DrawSimulationCompleteFlight, "Simulation Complete!", HighLogic.Skin.window);
                if (showSimulationWindow)
                    simulationWindowPosition = GUILayout.Window(8955, simulationWindowPosition, KCT_GUI.DrawSimulationWindow, "KCT Simulation", HighLogic.Skin.window);
                if (showTimeRemaining && KCT_GameStates.simulationTimeLimit > 0)
                    timeRemainingPosition = GUILayout.Window(8951, timeRemainingPosition, KCT_GUI.DrawSimulationTimeWindow, "Time left:", HighLogic.Skin.window);
                if (showBuildList)
                    buildListWindowPosition = GUILayout.Window(8950, buildListWindowPosition, KCT_GUI.DrawBuildListWindow, "Build List", HighLogic.Skin.window);
                if (showClearLaunch)
                    centralWindowPosition = GUILayout.Window(8952, centralWindowPosition, KCT_GUI.DrawClearLaunch, "Launch site not clear!", HighLogic.Skin.window);
                if (showShipRoster)
                    crewListWindowPosition = GUILayout.Window(8953, crewListWindowPosition, KCT_GUI.DrawShipRoster, "Select Crew", HighLogic.Skin.window);
                if (showCrewSelect)
                    crewListWindowPosition = GUILayout.Window(8954, crewListWindowPosition, KCT_GUI.DrawCrewSelect, "Select Crew", HighLogic.Skin.window);
                if (showSimConfig)
                    simulationConfigPosition = GUILayout.Window(8951, simulationConfigPosition, KCT_GUI.DrawSimulationConfigure, "Simulation Configuration", HighLogic.Skin.window);
                if (showBodyChooser)
                    centralWindowPosition = GUILayout.Window(8952, centralWindowPosition, KCT_GUI.DrawBodyChooser, "Choose Body", HighLogic.Skin.window);
                if (showUpgradeWindow)
                    upgradePosition = GUILayout.Window(8952, upgradePosition, KCT_GUI.DrawUpgradeWindow, "Upgrades", HighLogic.Skin.window);
                if (showBLPlus)
                    bLPlusPosition = GUILayout.Window(8953, bLPlusPosition, KCT_GUI.DrawBLPlusWindow, "Options", HighLogic.Skin.window);
                if (showRename)
                    centralWindowPosition = GUILayout.Window(8954, centralWindowPosition, KCT_GUI.DrawRenameWindow, "Rename", HighLogic.Skin.window);
                if (showFirstRun)
                    centralWindowPosition = GUILayout.Window(8954, centralWindowPosition, KCT_GUI.DrawFirstRun, "Kerbal Construction Time", HighLogic.Skin.window);
                if (showSimLengthChooser)
                    centralWindowPosition = GUILayout.Window(8952, centralWindowPosition, KCT_GUI.DrawSimLengthChooser, "Time Limit", HighLogic.Skin.window);

                if (unlockEditor)
                {
                    EditorLogic.fetch.Unlock("KCTGUILock");
                    unlockEditor = false;
                }


                //Disable KSC things when certain windows are shown.
                if (showFirstRun || showRename || showUpgradeWindow || showSettings || showCrewSelect || showShipRoster || showClearLaunch)
                {
                    if (!isKSCLocked)
                    {
                        InputLockManager.SetControlLock(ControlTypes.KSC_FACILITIES, "KCTKSCLock");
                        isKSCLocked = true;
                    }
                }
                else //if (!showBuildList)
                {
                    if (isKSCLocked)
                    {
                        InputLockManager.RemoveControlLock("KCTKSCLock");
                        isKSCLocked = false;
                    }
                }
                GUI.skin = oldSkin;
            }
        }

        public static bool PrimarilyDisabled { get { return (!KCT_GameStates.settings.enabledForSave || KCT_GameStates.settings.DisableBuildTime); } }

        private static void CheckKSCLock()
        {
            //On mouseover code for build list inspired by Engineer's editor mousover code
            Vector2 mousePos = Input.mousePosition;
            mousePos.y = Screen.height - mousePos.y;
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && !isKSCLocked)
            {
                if ((showBuildList && buildListWindowPosition.Contains(mousePos)) || (showBLPlus && bLPlusPosition.Contains(mousePos)))
                {
                    InputLockManager.SetControlLock(ControlTypes.KSC_FACILITIES, "KCTKSCLock");
                    isKSCLocked = true;
                }
                //KCTDebug.Log("KSC Locked");
            }
            else if (HighLogic.LoadedScene == GameScenes.SPACECENTER && isKSCLocked)
            {
                if (!(showBuildList && buildListWindowPosition.Contains(mousePos)) && !(showBLPlus && bLPlusPosition.Contains(mousePos)))
                {
                    InputLockManager.RemoveControlLock("KCTKSCLock");
                    isKSCLocked = false;
                }
                //KCTDebug.Log("KSC UnLocked");
            }
        }

        private static void CheckEditorLock()
        {
            //On mouseover code for editor inspired by Engineer's editor mousover code
            Vector2 mousePos = Input.mousePosition;
            mousePos.y = Screen.height - mousePos.y;
            if ((showEditorGUI && editorWindowPosition.Contains(mousePos)) && !isEditorLocked)
            {
                EditorLogic.fetch.Lock(true, false, true, "KCTEditorMouseLock");
                isEditorLocked = true;
                //KCTDebug.Log("KSC Locked");
            }
            else if (!(showEditorGUI && editorWindowPosition.Contains(mousePos)) && isEditorLocked)
            {
                EditorLogic.fetch.Unlock("KCTEditorMouseLock");
                isEditorLocked = false;
                //KCTDebug.Log("KSC UnLocked");
            }
        }

        public static void onClick()
        {
            clicked = !clicked;
            if (ToolbarManager.ToolbarAvailable && KCT_GameStates.kctToolbarButton != null)
                if (KCT_GameStates.kctToolbarButton.Important) KCT_GameStates.kctToolbarButton.Important = false;

          /*  if (!KCT_GameStates.settings.enabledForSave)
            {
                ShowSettings();
                return;
            }*/

            if (PrimarilyDisabled && (HighLogic.LoadedScene == GameScenes.SPACECENTER))
            {
                if (!showSettings)
                    ShowSettings();
                else
                    showSettings = false;
            }
            else if (KCT_GameStates.settings.DisableBuildTime && HighLogic.LoadedSceneIsEditor)
            {
                if (!showSimConfig)
                {
                    simulationConfigPosition.height = 1;
                    EditorLogic.fetch.Lock(true, true, true, "KCTGUILock");
                    showSimConfig = true;
                }
                else
                {
                    showSimConfig = false;
                    unlockEditor = true;
                }
            }
            else if (HighLogic.LoadedScene == GameScenes.FLIGHT && !KCT_GameStates.flightSimulated && !PrimarilyDisabled)
            {
                //showMainGUI = !showMainGUI;
                buildListWindowPosition.height = 1;
                showBuildList = !showBuildList;
                showBLPlus = false;
                listWindow = -1;
            }
            else if (HighLogic.LoadedScene == GameScenes.FLIGHT && KCT_GameStates.flightSimulated)
            {
                showSimulationWindow = !showSimulationWindow;
                simulationWindowPosition.height = 1;
            }
            else if ((HighLogic.LoadedScene == GameScenes.EDITOR) && !PrimarilyDisabled)
            {
                showEditorGUI = !showEditorGUI;
                KCT_GameStates.showWindows[1] = showEditorGUI;
            }
            else if ((HighLogic.LoadedScene == GameScenes.SPACECENTER) || (HighLogic.LoadedScene == GameScenes.TRACKSTATION) && !PrimarilyDisabled)
            {
                buildListWindowPosition.height = 1;
                showBuildList = !showBuildList;
                showBLPlus = false;
                listWindow = -1;
               // KCT_GameStates.showWindows[0] = showBuildList;
            }

            if (!KCT_GameStates.settings.PreferBlizzyToolbar)
            {
                if (showBuildList || showSettings || showEditorGUI || showSimulationWindow)
                {
                    KCT_Events.instance.KCTButtonStock.SetTrue(false);
                }
                else
                {
                    KCT_Events.instance.KCTButtonStock.SetFalse(false);
                }
            }
        }

        public static void onHoverOn()
        {
            if (!PrimarilyDisabled && !clicked)
            {
                if (HighLogic.LoadedScene == GameScenes.SPACECENTER && !showBuildList)
                {
                    showBuildList = true;
                }
            }
        }
        public static void onHoverOff()
        {
            if (!PrimarilyDisabled && !clicked)
            {
                if (HighLogic.LoadedScene == GameScenes.SPACECENTER && showBuildList)
                {
                    showBuildList = false;
                }
            }
        }


        public static void hideAll()
        {
            showEditorGUI = false;
            showLaunchAlert = false;
            showMainGUI = false;
            showSOIAlert = false;
            showSimulationCompleteEditor = false;
            showSimulationCompleteFlight = false;
            showSimulationWindow = false;
            showTimeRemaining = false;
            showBuildList = false;
            showClearLaunch = false;
            showShipRoster = false;
            showCrewSelect = false;
            showSettings = false;
            showSimConfig = false;
            showBodyChooser = false;
            showUpgradeWindow = false;
            showBLPlus = false;
            showRename = false;
            showFirstRun = false;
            showSimLengthChooser = false;
        }

        public static void DrawGUIs(int windowID)
        {
            if (showMainGUI)
                DrawMainGUI(windowID);
            if (showEditorGUI)
                DrawEditorGUI(windowID);
            if (showSOIAlert)
                DrawSOIAlertWindow(windowID + 1);
            if (showLaunchAlert)
                DrawLaunchAlert(windowID);
            if (showSimulationCompleteEditor)
                DrawSimulationCompleteEditor(windowID);
            if (showSimulationCompleteFlight)
                DrawSimulationCompleteFlight(windowID);
            if (showSimulationWindow)
                DrawSimulationWindow(windowID);
            if (showTimeRemaining && KCT_GameStates.simulationTimeLimit > 0)
                DrawSimulationTimeWindow(windowID);
            if (showBuildList)
                DrawBuildListWindow(windowID);
            if (showClearLaunch)
                DrawClearLaunch(windowID);
            if (showShipRoster)
                DrawShipRoster(windowID);
            if (showCrewSelect)
                DrawCrewSelect(windowID);
            if (showUpgradeWindow)
                DrawUpgradeWindow(windowID);
            if (showRename)
                DrawRenameWindow(windowID);
            if (showFirstRun)
                DrawFirstRun(windowID);
            if (showSimLengthChooser)
                DrawSimLengthChooser(windowID);
        }

        public static void DrawMainGUI(int windowID) //Deprecated to all hell now I think
        {
            //GUIStyle mySty = new GUIStyle(GUI.skin.button);
            //mySty.normal.textColor = mySty.focused.textColor = Color.white;
            //mySty.hover.textColor = mySty.active.textColor = Color.yellow;
            //mySty.onNormal.textColor = mySty.onFocused.textColor = mySty.onHover.textColor = mySty.onActive.textColor = Color.green;
            //mySty.padding = new RectOffset(16, 16, 8, 8);

            //sets the layout for the GUI, which is pretty much just some debug stuff for me.
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            //GUILayout.Label("#Parts", GUILayout.ExpandHeight(true));
            GUILayout.Label("Build Time (s)", GUILayout.ExpandHeight(true));
            GUILayout.Label("Build Time Remaining: ", GUILayout.ExpandHeight(true));
            GUILayout.Label("UT: ", GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Warp until ready."))
            {
            //    if (FlightGlobals.ActiveVessel.id != KCT_GameStates.activeVessel.vessel.id)
                {
            //        FlightGlobals.SetActiveVessel(KCT_GameStates.activeVessel.vessel);
                }
                KCT_GameStates.canWarp = true;

            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label(KCT_GameStates.activeVessel.vessel.Parts.Count.ToString(), GUILayout.ExpandHeight(true));
           // GUILayout.Label(KCT_GameStates.activeVessel.buildTime.ToString(), GUILayout.ExpandHeight(true));
            //GUILayout.Label(KCT_Utilities.GetFormatedTime(KCT_GameStates.activeVessel.finishDate - KCT_GameStates.UT), GUILayout.ExpandHeight(true));
           // GUILayout.Label(KCT_Utilities.GetFormattedTime(KCT_GameStates.activeVessel.buildTime - KCT_GameStates.activeVessel.progress), GUILayout.ExpandHeight(true));
            GUILayout.Label(KCT_Utilities.GetFormattedTime(KCT_GameStates.UT).ToString(), GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Stop warp"))
            {
                KCT_GameStates.canWarp = false;
                TimeWarp.SetRate(0, true);

            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();
        }

        public static bool showInventory = false, useInventory = true;
        //private static string currentCategoryString = "NONE";
        private static int currentCategoryInt = -1;
        private static string buildRateForDisplay;
        private static int rateIndexHolder = 0;
        public static Dictionary<string, int> PartsInUse = new Dictionary<string, int>();
        private static void DrawEditorGUI(int windowID)
        {
            GUILayout.BeginVertical();
            //GUILayout.Label("Current KSC: " + KCT_GameStates.ActiveKSC.KSCName);
            if (!KCT_GameStates.EditorShipEditingMode) //Build mode
            {
                double buildTime = KCT_GameStates.EditorBuildTime;
                KCT_BuildListVessel.ListType type = EditorLogic.fetch.launchSiteName == "LaunchPad" ? KCT_BuildListVessel.ListType.VAB : KCT_BuildListVessel.ListType.SPH;
                GUILayout.Label("Total Build Points (BP):", GUILayout.ExpandHeight(true));
                GUILayout.Label(Math.Round(buildTime, 2).ToString(), GUILayout.ExpandHeight(true));
                GUILayout.BeginHorizontal();
                GUILayout.Label("Build Time at ");
                if (buildRateForDisplay == null) buildRateForDisplay = KCT_Utilities.GetBuildRate(0, type, null).ToString();
                buildRateForDisplay = GUILayout.TextField(buildRateForDisplay, GUILayout.Width(75));
                GUILayout.Label(" BP/s:");
                List<double> rates = new List<double>();
                if (type == KCT_BuildListVessel.ListType.VAB) rates = KCT_Utilities.BuildRatesVAB(null);
                else rates = KCT_Utilities.BuildRatesSPH(null);
                double bR;
                if (double.TryParse(buildRateForDisplay, out bR))
                {
                    if (GUILayout.Button("*", GUILayout.ExpandWidth(false)))
                    {
                        rateIndexHolder = (rateIndexHolder + 1) % rates.Count;
                        bR = rates[rateIndexHolder];
                        buildRateForDisplay = bR.ToString();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Label(KCT_Utilities.GetFormattedTime(buildTime / bR));
                }
                else
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Label("Invalid Build Rate");
                }

                bool useHolder = useInventory;
                useInventory = GUILayout.Toggle(useInventory, " Use parts from inventory?");
                if (useInventory != useHolder) KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);

                if (!KCT_GameStates.settings.OverrideLaunchButton)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Build"))
                    {
                        KCT_Utilities.AddVesselToBuildList(useInventory);
                        PartCategories CategoryCurrent = PartCategories.none;
                        switch (currentCategoryInt)
                        {
                            case 0: CategoryCurrent = PartCategories.Pods; break;
                            case 1: CategoryCurrent = PartCategories.FuelTank; break;
                            case 2: CategoryCurrent = PartCategories.Engine; break;
                            case 3: CategoryCurrent = PartCategories.Control; break;
                            case 4: CategoryCurrent = PartCategories.Structural; break;
                            case 5: CategoryCurrent = PartCategories.Aero; break;
                            case 6: CategoryCurrent = PartCategories.Utility; break;
                            case 7: CategoryCurrent = PartCategories.Science; break;
                            default: CategoryCurrent = PartCategories.none; break;
                        }
                        InventoryCategoryChanged(CategoryCurrent);
                        KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                    }
                    if (GUILayout.Button("Simulate"))
                    {
                        simulationConfigPosition.height = 1;
                        EditorLogic.fetch.Lock(true, true, true, "KCTGUILock");
                        showSimConfig = true;
                    }
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Show/Hide Build List"))
                {
                    showBuildList = !showBuildList;
                }

                if (GUILayout.Button("Part Inventory"))
                {
                    showInventory = !showInventory;
                    editorWindowPosition.width = 275;
                    editorWindowPosition.height = 135;
                }
            }
            else //Edit mode
            {
                if (showInventory) //The part inventory is not shown in the editor mode
                {
                    showInventory = false;
                    editorWindowPosition.width = 275;
                    editorWindowPosition.height = 1;
                }

                KCT_BuildListVessel ship = KCT_GameStates.editedVessel;
                double origBP = ship.isFinished ? KCT_Utilities.GetBuildTime(ship.ExtractedPartNodes, true, ship.InventoryParts) : ship.buildPoints; //If the ship is finished, recalculate times. Else, use predefined times.
                double buildTime = KCT_GameStates.EditorBuildTime;
                double difference = Math.Abs(buildTime - origBP);
                double progress;
                if (ship.isFinished) progress = origBP;
                else progress = ship.progress;
                double newProgress = Math.Max(0, progress - (1.1 * difference));
                GUILayout.Label("Original: " + Math.Max(0, Math.Round(progress, 2)) + "/" + Math.Round(origBP, 2) + " BP (" + Math.Max(0, Math.Round(100 * (progress / origBP), 2)) + "%)");
                GUILayout.Label("Edited: " + Math.Round(newProgress, 2) + "/" + Math.Round(buildTime, 2) + " BP (" + Math.Round(100 * newProgress / buildTime, 2) + "%)");

                KCT_BuildListVessel.ListType type = EditorLogic.fetch.launchSiteName == "LaunchPad" ? KCT_BuildListVessel.ListType.VAB : KCT_BuildListVessel.ListType.SPH;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Build Time at ");
                if (buildRateForDisplay == null) buildRateForDisplay = KCT_Utilities.GetBuildRate(0, type, null).ToString();
                buildRateForDisplay = GUILayout.TextField(buildRateForDisplay, GUILayout.Width(75));
                GUILayout.Label(" BP/s:");
                List<double> rates = new List<double>();
                if (ship.type == KCT_BuildListVessel.ListType.VAB) rates = KCT_Utilities.BuildRatesVAB(null);
                else rates = KCT_Utilities.BuildRatesSPH(null);
                double bR;
                if (double.TryParse(buildRateForDisplay, out bR))
                {
                    if (GUILayout.Button("*", GUILayout.ExpandWidth(false)))
                    {
                        rateIndexHolder = (rateIndexHolder + 1) % rates.Count;
                        bR = rates[rateIndexHolder];
                        buildRateForDisplay = bR.ToString();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Label(KCT_Utilities.GetFormattedTime(Math.Abs(buildTime - newProgress) / bR));
                }
                else
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Label("Invalid Build Rate");
                }

                bool oldInv = useInventory;
                useInventory = GUILayout.Toggle(useInventory, " Pull new parts from inventory?");
                if (oldInv != useInventory) KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save Edits"))
                {
                    

                    Dictionary<string, int> partsForInventory = new Dictionary<string, int>();
                    //List<string> partsForInventory = new List<string>();
                    if (KCT_GUI.useInventory)
                    {
                        Dictionary<string, int> newParts = new Dictionary<string, int>(KCT_GUI.PartsInUse);
                        //List<string> newParts = new List<string>(KCT_Utilities.PartDictToList(KCT_GUI.PartsInUse));
                        //List<string> theInventory = new List<string>(KCT_Utilities.PartDictToList(KCT_GameStates.PartInventory));
                        Dictionary<string, int> theInventory = new Dictionary<string, int>(KCT_GameStates.PartInventory);
                       /* foreach (string s in KCT_Utilities.PartDictToList(KCT_GameStates.EditedVesselParts))
                            if (newParts.Contains(s))
                                newParts.Remove(s);*/
                        foreach (KeyValuePair<string, int> kvp in KCT_GameStates.EditedVesselParts)
                        {
                            if (newParts.ContainsKey(kvp.Key))
                            {
                                if (newParts[kvp.Key] >= kvp.Value)
                                    newParts[kvp.Key] -= kvp.Value;
                                else
                                    newParts[kvp.Key] = 0;
                            }
                        }

                        /*foreach (string s in newParts)
                        {
                            if (theInventory.Contains(s))
                            {
                                theInventory.Remove(s);
                                partsForInventory.Add(s);
                            }
                        }*/
                        foreach (KeyValuePair<string, int> kvp in newParts)
                        {
                            if (theInventory.ContainsKey(kvp.Key))
                            {
                                if (theInventory[kvp.Key] >= kvp.Value)
                                {
                                    theInventory[kvp.Key] -= kvp.Value;
                                    KCT_Utilities.AddToDict(partsForInventory, kvp.Key, kvp.Value);
                                }
                                else
                                {
                                    KCT_Utilities.AddToDict(partsForInventory, kvp.Key, theInventory[kvp.Key]);
                                    theInventory[kvp.Key] = 0;
                                }
                            }
                        }

                    }
                    //foreach (string s in ship.InventoryParts)
                    //    partsForInventory.Add(s);
                    foreach (KeyValuePair<string, int> kvp in ship.InventoryParts)
                        KCT_Utilities.AddToDict(partsForInventory, kvp.Key, kvp.Value);

                    KCT_Utilities.AddFunds(ship.cost, TransactionReasons.VesselRollout);
                    KCT_BuildListVessel newShip = KCT_Utilities.AddVesselToBuildList(partsForInventory);//new KCT_BuildListVessel(EditorLogic.fetch.ship, EditorLogic.fetch.launchSiteName, buildTime, EditorLogic.FlagURL);
                    if (newShip == null)
                    {
                        KCT_Utilities.SpendFunds(ship.cost, TransactionReasons.VesselRollout);
                        return;
                    }
                    
                    ship.RemoveFromBuildList();
                    newShip.progress = newProgress;
                    KCTDebug.Log("Finished? " + ship.isFinished);
                    if (ship.isFinished)
                        newShip.cannotEarnScience = true;

                    //foreach (string s in newShip.InventoryParts) //Compare the old inventory parts and the new one, removing the new ones from the old
                    foreach (KeyValuePair<string, int> kvp in newShip.InventoryParts)
                    {
                        if (ship.InventoryParts.ContainsKey(kvp.Key))
                        {
                            if (ship.InventoryParts[kvp.Key] >= newShip.InventoryParts[kvp.Key])
                                ship.InventoryParts[kvp.Key] -= newShip.InventoryParts[kvp.Key];
                            else
                                ship.InventoryParts[kvp.Key] = 0;
                            //ship.InventoryParts.Remove(s);
                        }
                    }
                    //foreach (string s in ship.InventoryParts) //Add the remaining old parts to the overall inventory
                    foreach (KeyValuePair<string, int> kvp in ship.InventoryParts)
                        KCT_Utilities.AddPartToInventory(kvp.Key, kvp.Value);
                    
                    GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE); 

                    KCT_GameStates.EditorShipEditingMode = false;

                    InputLockManager.RemoveControlLock("KCTEditExit");
                    InputLockManager.RemoveControlLock("KCTEditLoad");
                    InputLockManager.RemoveControlLock("KCTEditNew");
                    InputLockManager.RemoveControlLock("KCTEditLaunch");
                    EditorLogic.fetch.Unlock("KCTEditorMouseLock");
                    KCTDebug.Log("Edits saved.");
                    HighLogic.LoadScene(GameScenes.SPACECENTER);
                }
                if (GUILayout.Button("Cancel Edits"))
                {
                    KCT_GameStates.EditorShipEditingMode = false;

                    InputLockManager.RemoveControlLock("KCTEditExit");
                    InputLockManager.RemoveControlLock("KCTEditLoad");
                    InputLockManager.RemoveControlLock("KCTEditNew");
                    InputLockManager.RemoveControlLock("KCTEditLaunch");
                    EditorLogic.fetch.Unlock("KCTEditorMouseLock");
                    KCTDebug.Log("Edits cancelled.");
                    HighLogic.LoadScene(GameScenes.SPACECENTER);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Simulate"))
                {
                    simulationConfigPosition.height = 1;
                    EditorLogic.fetch.Lock(true, true, true, "KCTGUILock");
                    showSimConfig = true;
                    KCT_GameStates.launchedVessel = new KCT_BuildListVessel(EditorLogic.fetch.ship, EditorLogic.fetch.launchSiteName, buildTime, EditorLogic.FlagURL);
                }
                GUILayout.EndHorizontal();
            }


            if (showInventory)
            {
                List<string> categories = new List<string> { "Pods", "Fuel.", "Eng.", "Ctl.", "Struct.", "Aero", "Util.", "Sci." };
                int lastCat = currentCategoryInt;
                currentCategoryInt = GUILayout.Toolbar(currentCategoryInt, categories.ToArray(), GUILayout.ExpandWidth(false));
                
                PartCategories CategoryCurrent = PartCategories.none;
                switch (currentCategoryInt)
                {
                    case 0: CategoryCurrent = PartCategories.Pods; break;
                    case 1: CategoryCurrent = PartCategories.FuelTank; break;
                    case 2: CategoryCurrent = PartCategories.Engine; break;
                    case 3: CategoryCurrent = PartCategories.Control; break;
                    case 4: CategoryCurrent = PartCategories.Structural; break;
                    case 5: CategoryCurrent = PartCategories.Aero; break;
                    case 6: CategoryCurrent = PartCategories.Utility; break;
                    case 7: CategoryCurrent = PartCategories.Science; break;
                    default: CategoryCurrent = PartCategories.none; break;
                }

                if (GUI.changed)
                {
                    editorWindowPosition.height = 1;
                    if (lastCat == currentCategoryInt)
                    {
                        currentCategoryInt = -1;
                        CategoryCurrent = PartCategories.none;
                    }
                    InventoryCategoryChanged(CategoryCurrent);
                }


                float windowWidth = editorWindowPosition.width;
                GUILayout.BeginVertical();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((InventoryForCategory.Count+1) * 27, Screen.height / 4F)));
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:");
                GUILayout.Label("Available:", GUILayout.Width(windowWidth / 7));
                GUILayout.Label("In use:", GUILayout.Width(windowWidth / 7));
                GUILayout.EndHorizontal();

                var ordered = InventoryForCategory.OrderBy(x => PartsInUse.ContainsKey(x.Key) ? PartsInUse[x.Key] : 0).ToDictionary(x => x.Key, x => x.Value).Reverse();
                foreach (KeyValuePair<string, int> entry in ordered)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(InventoryCommonNames[entry.Key]);
                    GUILayout.Label(entry.Value.ToString(), GUILayout.Width(windowWidth / 7));
                    int inUse = PartsInUse.ContainsKey(entry.Key) ? PartsInUse[entry.Key] : 0;
                    GUILayout.Label(inUse.ToString(), GUILayout.Width(windowWidth / 7));
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
            

            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();

            CheckEditorLock();
        }

        private static Dictionary<string, int> InventoryForCategory = new Dictionary<string, int>();
        private static Dictionary<string, string> InventoryCommonNames = new Dictionary<string, string>();
        private static void InventoryCategoryChanged(PartCategories category)
        {
            InventoryForCategory.Clear();
            InventoryCommonNames.Clear();
            foreach (KeyValuePair<string, int> entry in KCT_GameStates.PartInventory)
            {
                string name = entry.Key;
                string baseName = name.Split(',').Length == 1 ? name : name.Split(',')[0];
                AvailablePart aPart = KCT_Utilities.GetAvailablePartByName(baseName);
                if (aPart != null && aPart.category == category)
                {
                    string tweakscale = "";
                    if (name.Split(',').Length == 2)
                        tweakscale = "," + name.Split(',')[1];
                    name = aPart.title + tweakscale;
                    if (!InventoryForCategory.ContainsKey(entry.Key))
                    {
                        InventoryForCategory.Add(entry.Key, entry.Value);
                        InventoryCommonNames.Add(entry.Key, name);
                    }
                }
            }
        }

        public static void DrawSOIAlertWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("   Warp stopped due to SOI change.", GUILayout.ExpandHeight(true));
            GUILayout.Label("Vessel name: " + KCT_GameStates.lastSOIVessel, GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Close"))
            {
                showSOIAlert = false;
            }
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();
        }

        private static string orbitAltString = "", orbitIncString = "", simLength = "0.25", UTString = "";
        private static bool advancedSimConfig = false;
        public static void DrawSimulationConfigure(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Body: ");
            if (KCT_GameStates.simulationBody == null)
                KCT_GameStates.simulationBody = KCT_Utilities.GetBodyByName("Kerbin");
            GUILayout.Label(KCT_GameStates.simulationBody.bodyName);
            if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
            {
                //show body chooser
                showSimConfig = false;
                showBodyChooser = true;
                centralWindowPosition.height = 1;
                simulationConfigPosition.height = 1;
            }
            GUILayout.EndHorizontal();
            if (KCT_GameStates.simulationBody.bodyName == "Kerbin")
            {
                bool changed = KCT_GameStates.simulateInOrbit;
                KCT_GameStates.simulateInOrbit = GUILayout.Toggle(KCT_GameStates.simulateInOrbit, " Start in orbit?");
                if (KCT_GameStates.simulateInOrbit != changed)
                    simulationConfigPosition.height = 1;
            }
            if (KCT_GameStates.simulationBody.bodyName != "Kerbin" || (KCT_GameStates.simulationBody.bodyName == "Kerbin" && KCT_GameStates.simulateInOrbit))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Orbit Altitude (km): ");
                orbitAltString = GUILayout.TextField(orbitAltString, GUILayout.Width(100));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Min: " + KCT_GameStates.simulationBody.maxAtmosphereAltitude/1000);
                GUILayout.Label("Max: " + Math.Floor(KCT_GameStates.simulationBody.sphereOfInfluence)/1000);
                GUILayout.EndHorizontal();

                if (!KCT_GameStates.simulateInOrbit) KCT_GameStates.simulateInOrbit = true;
            }
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Simulation Length: ");
            GUILayout.Label(KCT_Utilities.GetColonFormattedTime(float.Parse(simLength) * 3600));
            if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
            {
                //show body chooser
                showSimConfig = false;
                showSimLengthChooser = true;
                centralWindowPosition.height = 1;
                simulationConfigPosition.height = 1;
            }
            GUILayout.EndHorizontal();

            //simLength = GUILayout.TextField(simLength);
            float nullFloat, nF2;
            
            float cost = 0;
            if (!KCT_GameStates.settings.NoCostSimulations)
            {
                cost = KCT_GameStates.simulateInOrbit ? KCT_Utilities.CostOfSimulation(KCT_GameStates.simulationBody, simLength) : 100 * (KCT_Utilities.TimeMultipliers.ContainsKey(simLength) ? KCT_Utilities.TimeMultipliers[simLength] : 1);
                cost *= (EditorLogic.fetch.ship.GetShipCosts(out nullFloat, out nF2) / 25000); //Cost of simulation is less for ships less than 25k funds, and more for higher amounts
                GUILayout.Label("Cost: " + Math.Round(cost, 1));
            }
            

            bool tmp = advancedSimConfig;
            advancedSimConfig = GUILayout.Toggle(advancedSimConfig, " Show Advanced Options");
            if (tmp != advancedSimConfig)
            {
                simulationConfigPosition.height = 1;
            }
            if (advancedSimConfig)
            {
                if (KCT_GameStates.simulateInOrbit)
                {
                    KCT_GameStates.delayMove = GUILayout.Toggle(KCT_GameStates.delayMove, " Delay move to orbit");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Inclination: ");
                    orbitIncString = GUILayout.TextField(orbitIncString, GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("UT: ");
                UTString = GUILayout.TextField(UTString, GUILayout.Width(150));
                GUILayout.EndHorizontal();
            }


            GUILayout.BeginHorizontal();
            if (((KCT_Utilities.CurrentGameIsCareer() && (Funding.Instance.Funds >= cost))
                || !KCT_Utilities.CurrentGameIsCareer()) && GUILayout.Button("Simulate"))
            {
                if (KCT_GameStates.simulationBody.bodyName != "Kerbin")
                    KCT_GameStates.simulateInOrbit = true;

                KCT_GameStates.simulationTimeLimit = 3600 * double.Parse(simLength);
                KCT_GameStates.simulationDefaultTimeLimit = KCT_GameStates.simulationTimeLimit;

                if (KCT_GameStates.simulateInOrbit)
                {
                    if (!double.TryParse(orbitAltString, out KCT_GameStates.simOrbitAltitude))
                        KCT_GameStates.simOrbitAltitude = KCT_GameStates.simulationBody.maxAtmosphereAltitude + 1000;
                    else
                        KCT_GameStates.simOrbitAltitude = Math.Min(Math.Max(1000 * KCT_GameStates.simOrbitAltitude, KCT_GameStates.simulationBody.maxAtmosphereAltitude), KCT_GameStates.simulationBody.sphereOfInfluence);

                    if (!advancedSimConfig || !double.TryParse(orbitIncString, out KCT_GameStates.simInclination))
                        KCT_GameStates.simInclination = 0;
                    else
                        KCT_GameStates.simInclination = KCT_GameStates.simInclination % 360;
                }
                if (!advancedSimConfig || !double.TryParse(UTString, out KCT_GameStates.simulationUT))
                    KCT_GameStates.simulationUT = Planetarium.GetUniversalTime();
                else
                    KCT_GameStates.simulationUT = Math.Max(0, KCT_GameStates.simulationUT);


                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                unlockEditor = true;
                showSimConfig = false;
                centralWindowPosition.height = 1;
                if (!KCT_GameStates.settings.NoCostSimulations)
                {
                    KCT_Utilities.SpendFunds(cost, TransactionReasons.None);
                    KCT_GameStates.SimulationCost = cost;
                }
                if (KCT_Utilities.CurrentGameIsCareer())
                {
                    if (KCT_GameStates.FundsGivenForVessel != 0)
                        KCT_Utilities.SpendFunds(KCT_GameStates.FundsGivenForVessel, TransactionReasons.VesselRollout);

                    KCT_GameStates.FundsGivenForVessel = EditorLogic.fetch.ship.GetShipCosts(out nullFloat, out nF2);
                    KCT_Utilities.AddFunds(KCT_GameStates.FundsGivenForVessel, TransactionReasons.VesselRollout);
                }
                KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                KCT_GameStates.launchedVessel = new KCT_BuildListVessel(EditorLogic.fetch.ship, EditorLogic.fetch.launchSiteName, KCT_GameStates.EditorBuildTime, EditorLogic.FlagURL);
                EditorLogic.fetch.launchVessel();
            }
            if (GUILayout.Button("Cancel"))
            {
                showSimConfig = false;
                centralWindowPosition.height = 1;
                unlockEditor = true;
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();

            CheckEditorLock();
        }

        public static void DrawBodyChooser(int windowID)
        {
            GUILayout.BeginVertical();
            if (KCT_GameStates.settings.EnableAllBodies)
            {
                foreach (CelestialBody body in FlightGlobals.Bodies)
                {
                    if (GUILayout.Button(body.bodyName))
                    {
                        KCT_GameStates.simulationBody = body;
                        showBodyChooser = false;
                        showSimConfig = true;
                        centralWindowPosition.height = 1;
                        centralWindowPosition.y = (Screen.height - 50) / 2;
                    }
                }
            }
            else
            {
                foreach (String bodyName in KCT_GameStates.BodiesVisited)
                {
                    if (GUILayout.Button(bodyName))
                    {
                        KCT_GameStates.simulationBody = KCT_Utilities.GetBodyByName(bodyName);
                        showBodyChooser = false;
                        showSimConfig = true;
                        centralWindowPosition.height = 1;
                        centralWindowPosition.y = (Screen.height - 50) / 2;
                    }
                }
            }
            //centralWindowPosition.center.Set(Screen.width / 2f, Screen.height / 2f);
            centralWindowPosition.y = (Screen.height-centralWindowPosition.height) / 2;
            GUILayout.EndVertical();

            CheckEditorLock();
        }

        public static void DrawSimLengthChooser(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Time (cost multiplier)");
            foreach (String len in KCT_Utilities.TimeMultipliers.Keys)
            {
                float time = float.Parse(len) * 3600;
                string formatted = KCT_Utilities.GetColonFormattedTime(time);
                if (GUILayout.Button(formatted+" (x"+KCT_Utilities.TimeMultipliers[len]+")"))
                {
                    simLength = len;
                    showSimLengthChooser = false;
                    showSimConfig = true;
                    centralWindowPosition.height = 1;
                    centralWindowPosition.y = (Screen.height - 50) / 2;
                }
            }
            centralWindowPosition.y = (Screen.height - centralWindowPosition.height) / 2;
            GUILayout.EndVertical();
        }

        public static void DrawLaunchAlert(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Build Vessel"))
            {
                KCT_Utilities.AddVesselToBuildList(useInventory);
                PartCategories CategoryCurrent = PartCategories.none;
                switch (currentCategoryInt)
                {
                    case 0: CategoryCurrent = PartCategories.Pods; break;
                    case 1: CategoryCurrent = PartCategories.Propulsion; break;
                    case 2: CategoryCurrent = PartCategories.Control; break;
                    case 3: CategoryCurrent = PartCategories.Structural; break;
                    case 4: CategoryCurrent = PartCategories.Aero; break;
                    case 5: CategoryCurrent = PartCategories.Utility; break;
                    case 6: CategoryCurrent = PartCategories.Science; break;
                    default: CategoryCurrent = PartCategories.none; break;
                }
                InventoryCategoryChanged(CategoryCurrent);
                KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                showLaunchAlert = false;
                unlockEditor = true;
            }
            if (GUILayout.Button("Simulate Vessel"))
            {
                simulationConfigPosition.height = 1;
                showLaunchAlert = false;
                showSimConfig = true;
            }
            if (GUILayout.Button("Cancel"))
            {
                showLaunchAlert = false;
                centralWindowPosition.height = 1;
                unlockEditor = true;
            }
            GUILayout.EndVertical();

        }

        public static void DrawSimulationCompleteEditor(int windowID)
        {
            String reason = KCT_GameStates.simulationReason;
            GUILayout.BeginVertical();
            if (reason=="CRASHED")
                GUILayout.Label("Vessel destroyed");
            else if (reason=="APOAPSIS")
                GUILayout.Label("Apoapsis exceeded 250km");
            else if (reason=="PERIAPSIS")
                GUILayout.Label("Stable orbit reached");
            else if (reason=="USER")
                GUILayout.Label("The user ended the simulation");
            else if (reason == "TIME")
                GUILayout.Label("Time is up");

            if (GUILayout.Button("Add to Build List"))
            {
                KCT_GameStates.flightSimulated = false;
                KCT_Utilities.disableSimulationLocks();
                KCT_Utilities.AddVesselToBuildList();
                showSimulationCompleteEditor = false;
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Restart Simulation"))
            {
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                EditorLogic.fetch.launchVessel();
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Close"))
            {
                showSimulationCompleteEditor = false;
                centralWindowPosition.height = 1;
            }
            GUILayout.EndVertical();
        }

        public static void DrawSimulationCompleteFlight(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Build"))
            {
                KCT_GameStates.buildSimulatedVessel = true;
                KCTDebug.Log("Ship added from simulation.");
                var message = new ScreenMessage("[KCT] Ship will be added upon simulation completion!", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                ScreenMessages.PostScreenMessage(message, true);

                KCT_GameStates.simulationReason = "USER";
                KCTDebug.Log("Simulation complete: USER");
                KCT_Utilities.disableSimulationLocks();
                KCT_GameStates.flightSimulated = false;
                KCT_GameStates.simulationEndTime = 0;
                centralWindowPosition.height = 1;

                if (FlightDriver.CanRevertToPrelaunch)
                {
                    if (FlightDriver.LaunchSiteName == "LaunchPad")
                        FlightDriver.RevertToPrelaunch(EditorFacility.VAB);
                    else if (FlightDriver.LaunchSiteName == "Runway")
                        FlightDriver.RevertToPrelaunch(EditorFacility.SPH);
                }
                else
                {
                    HighLogic.LoadScene(GameScenes.SPACECENTER);
                }
            }

            if ((!KCT_Utilities.CurrentGameIsCareer() || KCT_GameStates.settings.NoCostSimulations || Funding.Instance.Funds >= (KCT_GameStates.SimulationCost*1.1))
                && GUILayout.Button("Purchase Additional Time\n" + ((KCT_GameStates.settings.NoCostSimulations || !KCT_Utilities.CurrentGameIsCareer()) ? "Free" : Math.Round(KCT_GameStates.SimulationCost * 1.1).ToString() + " funds")))
            {
                showSimulationCompleteFlight = false;
                if (KCT_Utilities.CurrentGameIsCareer() && !KCT_GameStates.settings.NoCostSimulations)
                {
                    KCT_GameStates.FundsToChargeAtSimEnd += KCT_GameStates.SimulationCost * 1.1F;
                    KCT_Utilities.SpendFunds(KCT_GameStates.SimulationCost * 1.1F, TransactionReasons.None);
                }
                KCT_GameStates.simulationEndTime += KCT_GameStates.simulationDefaultTimeLimit;
                KCT_GameStates.simulationTimeLimit += KCT_GameStates.simulationDefaultTimeLimit;
                KCT_GameStates.SimulationCost *= 1.1F;
                FlightDriver.SetPause(false);
                TimeWarp.SetRate(0, true);
                centralWindowPosition.height = 1;
            }

            if (FlightDriver.CanRevertToPostInit && GUILayout.Button("Restart Simulation"))
            {
                KerbalConstructionTime.moved = false;
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                KCT_GameStates.simulationEndTime = 0;
                FlightDriver.RevertToLaunch();
                centralWindowPosition.height = 1;
            }

            if (FlightDriver.CanRevertToPrelaunch && GUILayout.Button("Revert to Editor"))
            {
                KCT_GameStates.simulationReason = "USER";
                KCTDebug.Log("Simulation complete: " + "USER");
                KCT_Utilities.disableSimulationLocks();
                KCT_GameStates.flightSimulated = false;
                KCT_GameStates.simulationEndTime = 0;
                if (FlightDriver.LaunchSiteName == "LaunchPad")
                    FlightDriver.RevertToPrelaunch(EditorFacility.VAB);
                else if (FlightDriver.LaunchSiteName == "Runway")
                    FlightDriver.RevertToPrelaunch(EditorFacility.SPH);
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Go to Space Center"))
            {
                KCT_GameStates.flightSimulated = false;
                KCT_Utilities.disableSimulationLocks();
                HighLogic.LoadScene(GameScenes.SPACECENTER);
                centralWindowPosition.height = 1;
            }
            GUILayout.EndVertical();
        }

        public static void DrawSimulationTimeWindow(int windowID)
        {

            GUILayout.BeginVertical();
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            double time = KCT_GameStates.simulationEndTime - KCT_GameStates.UT;
            if (time > 0)
                GUILayout.Label(KCT_Utilities.GetColonFormattedTime(time));
            else
                GUILayout.Label("Pre-launch");
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();
        }

        public static void DrawSimulationWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("This is a simulation. It will end when one of the following conditions are met:");
            GUILayout.Label("The time limit is exceeded");
            GUILayout.Label("The flight scene is exited");
            GUILayout.Label(" ");
            GUILayout.Label("All progress is lost in a simulation.");
            if (GUILayout.Button("Build It!"))
            {
                KCT_GameStates.buildSimulatedVessel = true;
                KCTDebug.Log("Ship added from simulation.");
                var message = new ScreenMessage("[KCT] Ship will be added upon simulation completion!", 4.0f, ScreenMessageStyle.UPPER_LEFT);
                ScreenMessages.PostScreenMessage(message, true);
            }
            if (FlightDriver.CanRevertToPostInit && GUILayout.Button("Restart Simulation"))
            {
                showSimulationWindow = false;
                KerbalConstructionTime.moved = false;
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                KCT_GameStates.simulationEndTime = 0;
             //   if (MCEWrapper.MCEAvailable) //Support for MCE
             //       MCEWrapper.IloadMCEbackup();
                FlightDriver.RevertToLaunch();
                centralWindowPosition.height = 1;
            }
            if (FlightDriver.CanRevertToPrelaunch && GUILayout.Button("Revert to Editor"))
            {
                showSimulationWindow = false;
                KCT_GameStates.simulationReason = "USER";
                KCTDebug.Log("Simulation complete: " + "USER");
                KCT_Utilities.disableSimulationLocks();
                KCT_GameStates.flightSimulated = false;
                KCT_GameStates.simulationEndTime = 0;
              //  if (MCEWrapper.MCEAvailable) //Support for MCE
              //      MCEWrapper.IloadMCEbackup();
                if (FlightDriver.LaunchSiteName == "LaunchPad")
                    FlightDriver.RevertToPrelaunch(EditorFacility.VAB);
                else if (FlightDriver.LaunchSiteName == "Runway")
                    FlightDriver.RevertToPrelaunch(EditorFacility.SPH);
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Close"))
            {
                showSimulationWindow = !showSimulationWindow;
            }
            GUILayout.EndVertical();

            if (simulationWindowPosition.width > 250)
                simulationWindowPosition.width = 250;
        }

        public static void ResetBLWindow()
        {
            buildListWindowPosition.height = 1;
            buildListWindowPosition.width = 400;
          //  listWindow = -1;
        }

        private static void ScrapVessel()
        {
            InputLockManager.RemoveControlLock("KCTPopupLock");
            //List<KCT_BuildListVessel> buildList = b.
            KCT_BuildListVessel b = KCT_Utilities.FindBLVesselByID(IDSelected);// = listWindow == 0 ? KCT_GameStates.VABList[IndexSelected] : KCT_GameStates.SPHList[IndexSelected];
            if (b == null)
            {
                KCTDebug.Log("Tried to remove a vessel that doesn't exist!");
                return;
            }
            KCTDebug.Log("Scrapping " + b.shipName);
            if (!b.isFinished)
            {
                List<ConfigNode> parts = b.ExtractedPartNodes;
                float totalCost = 0;
                foreach (ConfigNode p in parts)
                    totalCost += KCT_Utilities.GetPartCostFromNode(p);
                if (b.InventoryParts != null)
                {
                    //foreach (KeyValuePair<string, int> kvp in b.InventoryParts)
                    List<ConfigNode> toRemove = new List<ConfigNode>();
                    foreach (ConfigNode cn in parts)
                    {
                        //ConfigNode aP = parts.Find(a => (KCT_Utilities.PartNameFromNode(a) + KCT_Utilities.GetTweakScaleSize(a)) == kvp.Key);
                        //if (aP == null)
                        //    aP = parts.Find(a => (KCT_Utilities.PartNameFromNode(a)) == kvp.Key);
                        string name = KCT_Utilities.PartNameFromNode(cn);
                        if (!KCT_Utilities.PartIsProcedural(cn))
                            name += KCT_Utilities.GetTweakScaleSize(cn);
                        if (b.InventoryParts.ContainsKey(name))
                        {
                            totalCost -= KCT_Utilities.GetPartCostFromNode(cn);
                            //parts.Remove(cn);
                            int amt = 1;
                            if (KCT_Utilities.PartIsProcedural(cn))
                            {
                                amt = (int)(1000 * KCT_Utilities.GetPartCostFromNode(cn, false));
                            }
                            if (b.InventoryParts[name] >= amt)
                            {
                                b.InventoryParts[name] -= amt;
                                KCT_Utilities.AddPartToInventory(name, amt);
                            }
                            else
                            {
                                KCT_Utilities.AddPartToInventory(name, b.InventoryParts[name]);
                                b.InventoryParts[name] = 0;
                            }
                            if (b.InventoryParts[name] == 0)
                                b.InventoryParts.Remove(name);
                            toRemove.Add(cn);
                        }   
                    }
                    foreach (ConfigNode cn in toRemove)
                    {
                        parts.Remove(cn);
                    }
                }
                totalCost = (int)(totalCost * b.ProgressPercent() / 100);
                float sum = 0;
                while (parts.Find(a => KCT_Utilities.GetPartCostFromNode(a) < (totalCost - sum)) != null)
                {
                    ConfigNode aP = parts.Find(a => KCT_Utilities.GetPartCostFromNode(a) < (totalCost - sum));
                    sum += KCT_Utilities.GetPartCostFromNode(aP);
                    parts.Remove(aP);
                    KCT_Utilities.AddPartToInventory(aP);
                }
                //buildList.RemoveAt(IndexSelected);
                b.RemoveFromBuildList();
            }
            else
            {
                foreach (ConfigNode p in b.ExtractedPartNodes)
                    KCT_Utilities.AddPartToInventory(p);
               // buildList.RemoveAt(IndexSelected);
                b.RemoveFromBuildList();
            }
            KCT_Utilities.AddFunds(b.cost, TransactionReasons.VesselRollout);
        }

        public static void DummyVoid() { InputLockManager.RemoveControlLock("KCTPopupLock"); }

        private static bool IsCrewable(List<Part> ship)
        {
            foreach (Part p in ship)
                if (p.CrewCapacity > 0) return true;
            return false;
        }

        private static int FirstCrewable(List<Part> ship)
        {
            for (int i = 0; i < ship.Count; i++)
            {
                Part p = ship[i];
                //Debug.Log(p.partInfo.name+":"+p.CrewCapacity);
                if (p.CrewCapacity > 0) return i;
            }
            return -1;
        }

        public static void DrawClearLaunch(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Recover Flight and Proceed"))
            {
                List<ProtoVessel> list = ShipConstruction.FindVesselsLandedAt(HighLogic.CurrentGame.flightState, KCT_GameStates.launchedVessel.launchSite);
                foreach (ProtoVessel pv in list)
                    ShipConstruction.RecoverVesselFromFlight(pv, HighLogic.CurrentGame.flightState);
                if (!IsCrewable(KCT_GameStates.launchedVessel.ExtractedParts))
                    KCT_GameStates.launchedVessel.Launch();
                else
                {
                    showClearLaunch = false;
                    centralWindowPosition.height = 1;
                    pseudoParts = KCT_GameStates.launchedVessel.GetPseudoParts();
                    parts = KCT_GameStates.launchedVessel.ExtractedParts;
                    KCT_GameStates.launchedCrew = new List<CrewedPart>();
                    foreach (PseudoPart pp in pseudoParts)
                        KCT_GameStates.launchedCrew.Add(new CrewedPart(pp.uid, new List<ProtoCrewMember>()));
                    CrewFirstAvailable();
                    showShipRoster = true;
                }
                centralWindowPosition.height = 1;
            }

            if (GUILayout.Button("Cancel"))
            {
                showClearLaunch = false;
                centralWindowPosition.height = 1;
            }
            GUILayout.EndVertical();
        }


        private static int partIndexToCrew;
        private static int indexToCrew;
        //private static List<String> partNames;
        private static List<PseudoPart> pseudoParts;
        private static List<Part> parts;
        private static bool randomCrew, autoHire;
        public static void DrawShipRoster(int windowID)
        {
            System.Random rand = new System.Random();
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(Screen.height/2));
            GUILayout.BeginHorizontal();
            randomCrew = GUILayout.Toggle(randomCrew, " Randomize Filling");
            autoHire = GUILayout.Toggle(autoHire, " Auto-Hire Applicants");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill All"))
            {
                //foreach (AvailablePart p in KCT_GameStates.launchedVessel.GetPartNames())
                for (int j = 0; j < parts.Count; j++)
                {
                    Part p = parts[j];//KCT_Utilities.GetAvailablePartByName(KCT_GameStates.launchedVessel.GetPartNames()[j]).partPrefab;
                    if (p.CrewCapacity > 0)
                    {
                        //if (!KCT_GameStates.launchedCrew.Keys.Contains(p.uid))
                        //KCT_GameStates.launchedCrew.Add(new List<ProtoCrewMember>());
                        for (int i = 0; i < p.CrewCapacity; i++)
                        {
                            if (KCT_GameStates.launchedCrew[j].crewList.Count <= i)
                            {
                                if (CrewAvailable().Count > 0)
                                {
                                    int index = randomCrew ? new System.Random().Next(CrewAvailable().Count) : 0;
                                    ProtoCrewMember crewMember = CrewAvailable()[index];
                                    if (crewMember != null) KCT_GameStates.launchedCrew[j].crewList.Add(crewMember);
                                }
                                else if (autoHire)
                                {
                                    if (HighLogic.CurrentGame.CrewRoster.Applicants.Count() == 0)
                                        HighLogic.CurrentGame.CrewRoster.GetNextApplicant();
                                    int index = randomCrew ? rand.Next(HighLogic.CurrentGame.CrewRoster.Applicants.Count() - 1) : 0;
                                    ProtoCrewMember hired = HighLogic.CurrentGame.CrewRoster.Applicants.ElementAt(index);
                                    HighLogic.CurrentGame.CrewRoster.HireApplicant(hired, Planetarium.GetUniversalTime());
                                    List<ProtoCrewMember> activeCrew;
                                    activeCrew = KCT_GameStates.launchedCrew[j].crewList;
                                    if (activeCrew.Count > i)
                                    {
                                        activeCrew.Insert(i, hired);
                                        if (activeCrew[i + 1] == null)
                                            activeCrew.RemoveAt(i + 1);
                                    }
                                    else
                                    {
                                        for (int k = activeCrew.Count; k < i; k++)
                                        {
                                            activeCrew.Insert(k, null);
                                        }
                                        activeCrew.Insert(i, hired);
                                    }
                                    KCT_GameStates.launchedCrew[j].crewList = activeCrew;
                                }
                            }
                            else if (KCT_GameStates.launchedCrew[j].crewList[i] == null)
                            {
                                if (CrewAvailable().Count > 0)
                                {
                                    int index = randomCrew ? new System.Random().Next(CrewAvailable().Count) : 0;
                                    ProtoCrewMember crewMember = CrewAvailable()[index];
                                    if (crewMember != null) KCT_GameStates.launchedCrew[j].crewList[i] = crewMember;
                                }
                                else if (autoHire)
                                {
                                    if (HighLogic.CurrentGame.CrewRoster.Applicants.Count() == 0)
                                        HighLogic.CurrentGame.CrewRoster.GetNextApplicant();
                                    int index = randomCrew ? rand.Next(HighLogic.CurrentGame.CrewRoster.Applicants.Count() - 1) : 0;
                                    ProtoCrewMember hired = HighLogic.CurrentGame.CrewRoster.Applicants.ElementAt(index);
                                    HighLogic.CurrentGame.CrewRoster.HireApplicant(hired, Planetarium.GetUniversalTime());
                                    List<ProtoCrewMember> activeCrew;
                                    activeCrew = KCT_GameStates.launchedCrew[j].crewList;
                                    if (activeCrew.Count > i)
                                    {
                                        activeCrew.Insert(i, hired);
                                        if (activeCrew[i + 1] == null)
                                            activeCrew.RemoveAt(i + 1);
                                    }
                                    else
                                    {
                                        for (int k = activeCrew.Count; k < i; k++)
                                        {
                                            activeCrew.Insert(k, null);
                                        }
                                        activeCrew.Insert(i, hired);
                                    }
                                    KCT_GameStates.launchedCrew[j].crewList = activeCrew;
                                }
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button("Clear All"))
            {
                foreach (CrewedPart cP in KCT_GameStates.launchedCrew)
                {
                    cP.crewList.Clear();
                }
            }
            GUILayout.EndHorizontal();
            int numberItems = 0;
            foreach (Part p in parts)
            {
                //Part p = KCT_Utilities.GetAvailablePartByName(s).partPrefab;
                if (p.CrewCapacity>0)
                {
                    numberItems += 1 + p.CrewCapacity;
                }
            }
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(numberItems * 25 + 10), GUILayout.MaxHeight(Screen.height / 2));
            for (int j = 0; j < parts.Count; j++)
            {
                //Part p = KCT_Utilities.GetAvailablePartByName(KCT_GameStates.launchedVessel.GetPartNames()[j]).partPrefab;
                Part p = parts[j];
                if (p.CrewCapacity>0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p.partInfo.title.Length <= 15 ? p.partInfo.title : p.partInfo.title.Substring(0, 15));
                    if (GUILayout.Button("Fill", GUILayout.Width(75)))
                    {
                        if (KCT_GameStates.launchedCrew.Find(part => part.partID == p.craftID) == null)
                            KCT_GameStates.launchedCrew.Add(new CrewedPart(p.craftID, new List<ProtoCrewMember>()));
                        for (int i=0; i<p.CrewCapacity; i++)
                        {
                            if (KCT_GameStates.launchedCrew[j].crewList.Count <= i)
                            {
                                if (CrewAvailable().Count > 0)
                                {
                                    int index = randomCrew ? new System.Random().Next(CrewAvailable().Count) : 0;
                                    ProtoCrewMember crewMember = CrewAvailable()[index];
                                    if (crewMember != null) KCT_GameStates.launchedCrew[j].crewList.Add(crewMember);
                                }
                                else if (autoHire)
                                {
                                    if (HighLogic.CurrentGame.CrewRoster.Applicants.Count() == 0)
                                        HighLogic.CurrentGame.CrewRoster.GetNextApplicant();
                                    int index = randomCrew ? rand.Next(HighLogic.CurrentGame.CrewRoster.Applicants.Count() - 1) : 0;
                                    ProtoCrewMember hired = HighLogic.CurrentGame.CrewRoster.Applicants.ElementAt(index);
                                    HighLogic.CurrentGame.CrewRoster.HireApplicant(hired, Planetarium.GetUniversalTime());
                                    List<ProtoCrewMember> activeCrew;
                                    activeCrew = KCT_GameStates.launchedCrew[j].crewList;
                                    if (activeCrew.Count > i)
                                    {
                                        activeCrew.Insert(i, hired);
                                        if (activeCrew[i + 1] == null)
                                            activeCrew.RemoveAt(i + 1);
                                    }
                                    else
                                    {
                                        for (int k = activeCrew.Count; k < i; k++)
                                        {
                                            activeCrew.Insert(k, null);
                                        }
                                        activeCrew.Insert(i, hired);
                                    }
                                    KCT_GameStates.launchedCrew[j].crewList = activeCrew;
                                }
                            }
                            else if (KCT_GameStates.launchedCrew[j].crewList[i] == null)
                            {
                                if (CrewAvailable().Count > 0)
                                {
                                    int index = randomCrew ? new System.Random().Next(CrewAvailable().Count) : 0;
                                    KCT_GameStates.launchedCrew[j].crewList[i] = CrewAvailable()[index];
                                }
                                else if (autoHire)
                                {
                                    if (HighLogic.CurrentGame.CrewRoster.Applicants.Count() == 0)
                                        HighLogic.CurrentGame.CrewRoster.GetNextApplicant();
                                    int index = randomCrew ? rand.Next(HighLogic.CurrentGame.CrewRoster.Applicants.Count() - 1) : 0;
                                    ProtoCrewMember hired = HighLogic.CurrentGame.CrewRoster.Applicants.ElementAt(index);
                                    HighLogic.CurrentGame.CrewRoster.HireApplicant(hired, Planetarium.GetUniversalTime());
                                    List<ProtoCrewMember> activeCrew;
                                    activeCrew = KCT_GameStates.launchedCrew[j].crewList;
                                    if (activeCrew.Count > i)
                                    {
                                        activeCrew.Insert(i, hired);
                                        if (activeCrew[i + 1] == null)
                                            activeCrew.RemoveAt(i + 1);
                                    }
                                    else
                                    {
                                        for (int k = activeCrew.Count; k < i; k++)
                                        {
                                            activeCrew.Insert(k, null);
                                        }
                                        activeCrew.Insert(i, hired);
                                    }
                                    KCT_GameStates.launchedCrew[j].crewList = activeCrew;
                                }
                            }
                        }
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(75)))
                    {
                        KCT_GameStates.launchedCrew[j].crewList.Clear();
                    }
                    GUILayout.EndHorizontal();
                    for (int i = 0; i < p.CrewCapacity; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (i < KCT_GameStates.launchedCrew[j].crewList.Count && KCT_GameStates.launchedCrew[j].crewList[i] != null)
                        {
                            GUILayout.Label(KCT_GameStates.launchedCrew[j].crewList[i].name);
                            if (GUILayout.Button("Remove", GUILayout.Width(120)))
                            {
                                KCT_GameStates.launchedCrew[j].crewList[i].rosterStatus = ProtoCrewMember.RosterStatus.Available;
                                //KCT_GameStates.launchedCrew[j].RemoveAt(i);
                                KCT_GameStates.launchedCrew[j].crewList[i] = null;
                            }
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Empty");
                            if (CrewAvailable().Count > 0 && GUILayout.Button("Add", GUILayout.Width(120)))
                            {
                                showShipRoster = false;
                                showCrewSelect = true;
                                partIndexToCrew = j;
                                indexToCrew = i;
                                crewListWindowPosition.height = 1;
                            }
                            if (CrewAvailable().Count == 0 && GUILayout.Button("Hire New", GUILayout.Width(120)))
                            {
                                int index = randomCrew ? rand.Next(HighLogic.CurrentGame.CrewRoster.Applicants.Count() - 1) : 0;
                                ProtoCrewMember hired = HighLogic.CurrentGame.CrewRoster.Applicants.ElementAt(index);
                                //hired.rosterStatus = ProtoCrewMember.RosterStatus.AVAILABLE;
                                //HighLogic.CurrentGame.CrewRoster.AddCrewMember(hired);
                                HighLogic.CurrentGame.CrewRoster.HireApplicant(hired, Planetarium.GetUniversalTime());
                                List<ProtoCrewMember> activeCrew;
                                activeCrew = KCT_GameStates.launchedCrew[j].crewList;
                                if (activeCrew.Count > i)
                                {
                                    activeCrew.Insert(i, hired);
                                    if (activeCrew[i + 1] == null)
                                        activeCrew.RemoveAt(i + 1);
                                }
                                else
                                {
                                    for (int k = activeCrew.Count; k < i; k++)
                                    {
                                        activeCrew.Insert(k, null);
                                    }
                                    activeCrew.Insert(i, hired);
                                }
                                //availableCrew.Remove(crew);
                                KCT_GameStates.launchedCrew[j].crewList = activeCrew;
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Launch"))
            {
                //if (HighLogic.LoadedScene != GameScenes.TRACKSTATION)
                    KCT_GameStates.launchedVessel.Launch();
               /* else
                {
                    HighLogic.LoadScene(GameScenes.SPACECENTER);
                    KCT_GameStates.LaunchFromTS = true;
                    //KCT_GameStates.launchedVessel.Launch();
                }*/
                showShipRoster = false;
                crewListWindowPosition.height = 1;
            }
            if (GUILayout.Button("Cancel"))
            {
                showShipRoster = false;
                KCT_GameStates.launchedCrew.Clear();
                crewListWindowPosition.height = 1;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public static void CrewFirstAvailable()
        {
            int partIndex = FirstCrewable(parts);
            if (partIndex > -1)
            {
                Part p = parts[partIndex];
                if (KCT_GameStates.launchedCrew.Find(part => part.partID == p.craftID) == null)
                    KCT_GameStates.launchedCrew.Add(new CrewedPart(p.craftID, new List<ProtoCrewMember>()));
                for (int i = 0; i < p.CrewCapacity; i++)
                {
                    if (KCT_GameStates.launchedCrew[partIndex].crewList.Count <= i)
                    {
                        if (CrewAvailable().Count > 0)
                        {
                            int index = randomCrew ? new System.Random().Next(CrewAvailable().Count) : 0;
                            ProtoCrewMember crewMember = CrewAvailable()[index];
                            if (crewMember != null) KCT_GameStates.launchedCrew[partIndex].crewList.Add(crewMember);
                        }
                    }
                    else if (KCT_GameStates.launchedCrew[partIndex].crewList[i] == null)
                    {
                        if (CrewAvailable().Count > 0)
                        {
                            int index = randomCrew ? new System.Random().Next(CrewAvailable().Count) : 0;
                            KCT_GameStates.launchedCrew[partIndex].crewList[i] = CrewAvailable()[index];
                        }
                    }
                }
            }
        }

        private static List<ProtoCrewMember> CrewAvailable()
        {
            List<ProtoCrewMember> availableCrew = new List<ProtoCrewMember>();
            foreach (ProtoCrewMember crewMember in HighLogic.CurrentGame.CrewRoster.Crew) //Initialize available crew list
            {
                bool available = true;
                if (crewMember.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                {
                    foreach (CrewedPart cP in KCT_GameStates.launchedCrew)
                    {
                        if (cP.crewList.Contains(crewMember))
                            available = false;
                    }
                }
                else
                    available = false;
                if (available)
                    availableCrew.Add(crewMember);
            }
            return availableCrew;
        }

        public static void DrawCrewSelect(int windowID)
        {
            List<ProtoCrewMember> availableCrew = CrewAvailable();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxHeight(Screen.height / 2));
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(availableCrew.Count * 28 * 2 + 35), GUILayout.MaxHeight(Screen.height / 2));

            float cWidth = 80;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            GUILayout.Label("Courage:", GUILayout.Width(cWidth));
            GUILayout.Label("Stupidity:", GUILayout.Width(cWidth));
            //GUILayout.Space(cWidth/2);
            GUILayout.EndHorizontal();

            foreach (ProtoCrewMember crew in availableCrew)
            {
                GUILayout.BeginHorizontal();
                //GUILayout.Label(crew.name);
                if (GUILayout.Button(crew.name+"\n"+crew.experienceTrait.Title+" "+crew.experienceLevel))
                {
                    List<ProtoCrewMember> activeCrew;
                    activeCrew = KCT_GameStates.launchedCrew[partIndexToCrew].crewList;
                    if (activeCrew.Count > indexToCrew)
                    {
                        activeCrew.Insert(indexToCrew, crew);
                        if (activeCrew[indexToCrew + 1] == null)
                            activeCrew.RemoveAt(indexToCrew + 1);
                    }
                    else
                    {
                        for (int i = activeCrew.Count; i < indexToCrew; i++)
                        {
                            activeCrew.Insert(i, null);
                        }
                        activeCrew.Insert(indexToCrew, crew);
                    }
                    availableCrew.Remove(crew);
                    KCT_GameStates.launchedCrew[partIndexToCrew].crewList = activeCrew;
                    showCrewSelect = false;
                    showShipRoster = true;
                    crewListWindowPosition.height = 1;
                    break;
                }
                GUILayout.HorizontalSlider(crew.courage, 0, 1, HighLogic.Skin.horizontalSlider, HighLogic.Skin.horizontalSliderThumb, GUILayout.Width(cWidth));
                GUILayout.HorizontalSlider(crew.stupidity, 0, 1, HighLogic.Skin.horizontalSlider, HighLogic.Skin.horizontalSliderThumb, GUILayout.Width(cWidth));

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("Cancel"))
            {
                showCrewSelect = false;
                showShipRoster = true;
                crewListWindowPosition.height = 1;
            }
            GUILayout.EndVertical();
        }

        public static string newMultiplier, newBuildEffect, newInvEffect, newTimeWarp, newSandboxUpgrades, newUpgradeCount, newTimeLimit, newRecoveryModifier, 
            newReconEffect, maxReconditioning, newNodeModifier;
        public static bool enabledForSave, enableAllBodies, forceStopWarp, instantTechUnlock, disableBuildTimes, checkForUpdates, versionSpecific, disableRecMsgs, disableAllMsgs, 
            freeSims, recon, debug, overrideLaunchBtn, autoAlarms, useBlizzyToolbar, allowParachuteRecovery;

        public static double reconSplit;
        public static string newRecoveryModDefault;
        public static bool disableBuildTimesDefault, instantTechUnlockDefault, enableAllBodiesDefault, freeSimsDefault, reconDefault;
        private static void ShowSettings()
        {
            settingSelected = 0;
            newMultiplier = KCT_GameStates.timeSettings.OverallMultiplier.ToString();
            newBuildEffect = KCT_GameStates.timeSettings.BuildEffect.ToString();
            newInvEffect = KCT_GameStates.timeSettings.InventoryEffect.ToString();
            newReconEffect = (86400 / KCT_GameStates.timeSettings.ReconditioningEffect).ToString();
            enabledForSave = KCT_GameStates.settings.enabledForSave;
            enableAllBodies = KCT_GameStates.settings.EnableAllBodies;
            newTimeWarp = KCT_GameStates.settings.MaxTimeWarp.ToString();
            forceStopWarp = KCT_GameStates.settings.ForceStopWarp;
            newSandboxUpgrades = KCT_GameStates.settings.SandboxUpgrades.ToString();
            newUpgradeCount = KCT_GameStates.TotalUpgradePoints.ToString();
            //newTimeLimit = KCT_GameStates.settings.SimulationTimeLimit.ToString();
            instantTechUnlock = KCT_GameStates.settings.InstantTechUnlock;
            disableBuildTimes = KCT_GameStates.settings.DisableBuildTime;
            checkForUpdates = KCT_GameStates.settings.CheckForUpdates;
            versionSpecific = KCT_GameStates.settings.VersionSpecific;
            newRecoveryModifier = (KCT_GameStates.settings.RecoveryModifier*100).ToString();
            disableRecMsgs = KCT_GameStates.settings.DisableRecoveryMessages;
            disableAllMsgs = KCT_GameStates.settings.DisableAllMessages;
            freeSims = KCT_GameStates.settings.NoCostSimulations;
            recon = KCT_GameStates.settings.Reconditioning;
            debug = KCT_GameStates.settings.Debug;
            overrideLaunchBtn = KCT_GameStates.settings.OverrideLaunchButton;
            autoAlarms = KCT_GameStates.settings.AutoKACAlarams;
            useBlizzyToolbar = KCT_GameStates.settings.PreferBlizzyToolbar;
            allowParachuteRecovery = KCT_GameStates.settings.AllowParachuteRecovery;

            reconSplit = KCT_GameStates.timeSettings.RolloutReconSplit;
            maxReconditioning = KCT_GameStates.timeSettings.MaxReconditioning.ToString();
            newNodeModifier = KCT_GameStates.timeSettings.NodeModifier.ToString();

            disableBuildTimesDefault = KCT_GameStates.settings.DisableBuildTimeDefault;
            instantTechUnlockDefault = KCT_GameStates.settings.InstantTechUnlockDefault;
            enableAllBodiesDefault = KCT_GameStates.settings.EnableAllBodiesDefault;
            freeSimsDefault = KCT_GameStates.settings.NoCostSimulationsDefault;
            newRecoveryModDefault = (KCT_GameStates.settings.RecoveryModifierDefault*100).ToString();
            reconDefault = KCT_GameStates.settings.ReconditioningDefault;

            settingsPosition.height = 1;
            showSettings = !showSettings;
        }


        private static int settingSelected = 0;
        private static void DrawSettings(int windowID)
        {
            int width1 = 200;
            int width2 = 100;
            int lastSetting = settingSelected;
            GUILayout.BeginVertical();
            settingSelected = GUILayout.Toolbar(settingSelected, new string[] { "Game", "Global", "Time", "Defaults" });
            if (lastSetting != settingSelected) settingsPosition.height = 1;
            if (settingSelected == 0)
            {
                GUILayout.Label("Game Settings");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enabled for this save?", GUILayout.Width(width1));
                enabledForSave = GUILayout.Toggle(enabledForSave, enabledForSave ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                if (KCT_Utilities.CurrentGameIsSandbox())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Number of Upgrade Points", GUILayout.Width(width1));
                    newUpgradeCount = GUILayout.TextField(newUpgradeCount, 3, GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Build Times", GUILayout.Width(width1));
                disableBuildTimes = !GUILayout.Toggle(!disableBuildTimes, !disableBuildTimes ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                if (KCT_Utilities.CurrentGameHasScience())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Instant Tech Unlock", GUILayout.Width(width1));
                    instantTechUnlock = GUILayout.Toggle(instantTechUnlock, instantTechUnlock ? " Enabled" : " Disabled", GUILayout.Width(width2));
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Override Body Tracker", GUILayout.Width(width1));
                enableAllBodies = GUILayout.Toggle(enableAllBodies, enableAllBodies ? " Overridden" : " Normal", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                if (KCT_Utilities.CurrentGameIsCareer())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Funds Recovery Mod", GUILayout.Width(width1));
                    newRecoveryModifier = GUILayout.TextField(newRecoveryModifier, 4, GUILayout.Width(40));
                    GUILayout.EndHorizontal();
                }
                if (KCT_Utilities.CurrentGameIsCareer())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Free Simulations", GUILayout.Width(width1));
                    freeSims = GUILayout.Toggle(freeSims, freeSims ? " Free" : " Not Free", GUILayout.Width(width2));
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Reconditioning", GUILayout.Width(width1));
                recon = GUILayout.Toggle(recon, recon ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
            }
            //GUILayout.Label("");
            if (settingSelected == 1)
            {
                GUILayout.Label("Global Settings");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Max TimeWarp", GUILayout.Width(width1));
                //string newMultiplier = KCT_GameStates.timeSettings.OverallMultiplier.ToString();
                int warpIndex = 0;
                int.TryParse(newTimeWarp, out warpIndex);
                GUILayout.Label(TimeWarp.fetch.warpRates[Math.Min(TimeWarp.fetch.warpRates.Count() - 1, Math.Max(0, warpIndex))].ToString() + "x");
                newTimeWarp = GUILayout.TextField(newTimeWarp, 1, GUILayout.Width(20));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Force Stop Timewarp on Complete", GUILayout.Width(width1));
                forceStopWarp = GUILayout.Toggle(forceStopWarp, forceStopWarp ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Recovery Messages", GUILayout.Width(width1));
                disableRecMsgs = !GUILayout.Toggle(!disableRecMsgs, !disableRecMsgs ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("All Messages", GUILayout.Width(width1));
                disableAllMsgs = !GUILayout.Toggle(!disableAllMsgs, !disableAllMsgs ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Override Launch Button", GUILayout.Width(width1));
                overrideLaunchBtn = GUILayout.Toggle(overrideLaunchBtn, overrideLaunchBtn ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Debugging", GUILayout.Width(width1));
                debug = GUILayout.Toggle(debug, debug ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Automatic KAC Alarms", GUILayout.Width(width1));
                autoAlarms = GUILayout.Toggle(autoAlarms, autoAlarms ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Prefered Toolbar", GUILayout.Width(width1));
                useBlizzyToolbar = GUILayout.Toggle(useBlizzyToolbar, useBlizzyToolbar ? " Toolbar Mod" : " Stock", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Parachute Recovery", GUILayout.Width(width1));
                allowParachuteRecovery = GUILayout.Toggle(allowParachuteRecovery, allowParachuteRecovery ? " Active" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Auto Check For Updates", GUILayout.Width(width1));
                checkForUpdates = GUILayout.Toggle(checkForUpdates, checkForUpdates ? " Enabled" : " Disabled");
                if (GUILayout.Button("Check"))
                    KCT_UpdateChecker.CheckForUpdate(true, versionSpecific);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Check Only for this KSP Version?", GUILayout.Width(width1));
                versionSpecific = GUILayout.Toggle(versionSpecific, versionSpecific ? " Yes" : " No");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Current: " + KCT_UpdateChecker.CurrentVersion);
                if (KCT_UpdateChecker.WebVersion != "")
                    GUILayout.Label("Latest: " + KCT_UpdateChecker.WebVersion);
                GUILayout.EndHorizontal();

            }
            //GUILayout.Label("");
            if (settingSelected == 2)
            {
                GUILayout.Label("Global Time Settings");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Overall Multiplier", GUILayout.Width(width1));
                //string newMultiplier = KCT_GameStates.timeSettings.OverallMultiplier.ToString();
                newMultiplier = GUILayout.TextField(newMultiplier, 10, GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Build Effect", GUILayout.Width(width1));
                //string newBuildEffect = KCT_GameStates.timeSettings.BuildEffect.ToString();
                newBuildEffect = GUILayout.TextField(newBuildEffect, 10, GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Inventory Effect", GUILayout.Width(width1));
                //string newInvEffect = KCT_GameStates.timeSettings.InventoryEffect.ToString();
                newInvEffect = GUILayout.TextField(newInvEffect, 10, GUILayout.Width(100));
                GUILayout.EndHorizontal();

              /*  GUILayout.BeginHorizontal();
                GUILayout.Label("Tech Modifier", GUILayout.Width(width1));
                newNodeModifier = GUILayout.TextField(newNodeModifier, 10, GUILayout.Width(100));
                GUILayout.EndHorizontal();*/

                GUILayout.Label("Reconditioning/Rollout:");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Total: ");
                double mult;
                if (!double.TryParse(newMultiplier, out mult)) mult = KCT_GameStates.timeSettings.OverallMultiplier;
                double days = mult * 86400;
                GUILayout.Label(days + " BP per ");
                newReconEffect = GUILayout.TextField(newReconEffect, 10, GUILayout.Width(100));
                GUILayout.Label(" tons.");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Max Recon.: ");
                maxReconditioning = GUILayout.TextField(maxReconditioning, 10, GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUILayout.Label("Rollout %:");
                GUILayout.BeginHorizontal();
                reconSplit = GUILayout.HorizontalSlider((float)reconSplit, 0, 1);
                reconSplit = Math.Round(reconSplit, 2);
                GUILayout.Label(" "+reconSplit*100 + "%", GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }
            if (settingSelected == 3)
            {
                GUILayout.Label("Game Settings Defaults");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Build Times", GUILayout.Width(width1));
                disableBuildTimesDefault = !GUILayout.Toggle(!disableBuildTimesDefault, !disableBuildTimesDefault ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Instant Tech Unlock", GUILayout.Width(width1));
                instantTechUnlockDefault = GUILayout.Toggle(instantTechUnlockDefault, instantTechUnlockDefault ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Override Body Tracker", GUILayout.Width(width1));
                enableAllBodiesDefault = GUILayout.Toggle(enableAllBodiesDefault, enableAllBodiesDefault ? " Overridden" : " Normal", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Funds Recovery Mod", GUILayout.Width(width1));
                newRecoveryModDefault = GUILayout.TextField(newRecoveryModDefault, 4, GUILayout.Width(40));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Free Simulations", GUILayout.Width(width1));
                freeSimsDefault = GUILayout.Toggle(freeSimsDefault, freeSimsDefault ? " Free" : " Not Free", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Upgrades for New Sandbox", GUILayout.Width(width1));
                newSandboxUpgrades = GUILayout.TextField(newSandboxUpgrades, 3, GUILayout.Width(40));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Reconditioning", GUILayout.Width(width1));
                reconDefault = GUILayout.Toggle(reconDefault, reconDefault ? " Enabled" : " Disabled", GUILayout.Width(width2));
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                if (!enabledForSave && KCT_GameStates.settings.enabledForSave)
                    KCT_Utilities.DisableModFunctionality();
                KCT_GameStates.settings.enabledForSave = enabledForSave;
                KCT_GameStates.TotalUpgradePoints = int.Parse(newUpgradeCount);
                KCT_GameStates.settings.MaxTimeWarp = Math.Min(TimeWarp.fetch.warpRates.Count()-1, Math.Max(0, int.Parse(newTimeWarp)));
                KCT_GameStates.settings.EnableAllBodies = enableAllBodies;
                KCT_GameStates.settings.ForceStopWarp = forceStopWarp;
                KCT_GameStates.settings.InstantTechUnlock = instantTechUnlock;
                KCT_GameStates.settings.SandboxUpgrades = int.Parse(newSandboxUpgrades);
                KCT_GameStates.settings.DisableBuildTime = disableBuildTimes;
                KCT_GameStates.settings.RecoveryModifier = Math.Min(1, Math.Max(float.Parse(newRecoveryModifier) / 100f, 0));
                KCT_GameStates.settings.CheckForUpdates = checkForUpdates;
                KCT_GameStates.settings.VersionSpecific = versionSpecific;
                KCT_GameStates.settings.DisableRecoveryMessages = disableRecMsgs;
                KCT_GameStates.settings.DisableAllMessages = disableAllMsgs;
                KCT_GameStates.settings.NoCostSimulations = freeSims;
                KCT_GameStates.settings.Reconditioning = recon;
                KCT_GameStates.settings.OverrideLaunchButton = overrideLaunchBtn;
                KCT_GameStates.settings.Debug = debug;
                KCT_GameStates.settings.AutoKACAlarams = autoAlarms;
                KCT_GameStates.settings.PreferBlizzyToolbar = useBlizzyToolbar;
                KCT_GameStates.settings.AllowParachuteRecovery = allowParachuteRecovery;

                KCT_GameStates.settings.DisableBuildTimeDefault = disableBuildTimesDefault;
                KCT_GameStates.settings.InstantTechUnlockDefault = instantTechUnlockDefault;
                KCT_GameStates.settings.EnableAllBodiesDefault = enableAllBodiesDefault;
                KCT_GameStates.settings.NoCostSimulationsDefault = freeSimsDefault;
                KCT_GameStates.settings.RecoveryModifierDefault = Math.Min(1, Math.Max(float.Parse(newRecoveryModDefault) / 100f, 0));
                KCT_GameStates.settings.ReconditioningDefault = reconDefault;

                KCT_GameStates.settings.Save();

                KCT_GameStates.timeSettings.OverallMultiplier = double.Parse(newMultiplier);
                KCT_GameStates.timeSettings.BuildEffect = double.Parse(newBuildEffect);
                KCT_GameStates.timeSettings.InventoryEffect = double.Parse(newInvEffect);
                KCT_GameStates.timeSettings.NodeModifier = double.Parse(newNodeModifier);
                KCT_GameStates.timeSettings.MaxReconditioning = double.Parse(maxReconditioning);
                KCT_GameStates.timeSettings.RolloutReconSplit = reconSplit;
                double reconTime = double.Parse(newReconEffect);
                reconTime = (86400 / reconTime);
                KCT_GameStates.timeSettings.ReconditioningEffect = reconTime;
                KCT_GameStates.timeSettings.Save();
                showSettings = false;
                if (!PrimarilyDisabled) showBuildList = true;
                if (!enabledForSave) InputLockManager.RemoveControlLock("KCTKSCLock");
            }
            if (GUILayout.Button("Cancel"))
            {
                showSettings = false;
                if (!PrimarilyDisabled) showBuildList = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();
        }

        public static void  CheckToolbar()
        {
            if (ToolbarManager.ToolbarAvailable && ToolbarManager.Instance != null && KCT_GameStates.settings.PreferBlizzyToolbar && KCT_GameStates.kctToolbarButton == null)
            {
                KCTDebug.Log("Adding Toolbar Button");
                KCT_GameStates.kctToolbarButton = ToolbarManager.Instance.add("Kerbal_Construction_Time", "MainButton");
                if (KCT_GameStates.kctToolbarButton != null)
                {
                    if (!KCT_GameStates.settings.enabledForSave) KCT_GameStates.kctToolbarButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER);
                    else KCT_GameStates.kctToolbarButton.Visibility = new GameScenesVisibility(new GameScenes[] { GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.EDITOR });
                    KCT_GameStates.kctToolbarButton.TexturePath = KCT_Utilities.GetButtonTexture();
                    KCT_GameStates.kctToolbarButton.ToolTip = "Kerbal Construction Time";
                    KCT_GameStates.kctToolbarButton.OnClick += ((e) =>
                    {
                        KCT_GUI.onClick();
                    });
                }
            }
            bool vis;
            if (ApplicationLauncher.Ready && (!KCT_GameStates.settings.PreferBlizzyToolbar || !ToolbarManager.ToolbarAvailable) && (KCT_Events.instance.KCTButtonStock == null || !ApplicationLauncher.Instance.Contains(KCT_Events.instance.KCTButtonStock, out vis))) //Add Stock button
            {
                KCT_Events.instance.KCTButtonStock = ApplicationLauncher.Instance.AddModApplication(
                    KCT_GUI.onClick,
                    KCT_GUI.onClick,
                    KCT_Events.instance.DummyVoid, //TODO: List next ship here?
                    KCT_Events.instance.DummyVoid,
                    KCT_Events.instance.DummyVoid,
                    KCT_Events.instance.DummyVoid,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    GameDatabase.Instance.GetTexture("KerbalConstructionTime/Icons/KCT_on", false));

                ApplicationLauncher.Instance.EnableMutuallyExclusive(KCT_Events.instance.KCTButtonStock);
            }
        }

        private static int upgradeWindowHolder = 0;
        private static int sciCost = 0, fundsCost = 0;
        private static double nodeRate = 0, upNodeRate = 0;
        private static void DrawUpgradeWindow(int windowID)
        {
            int spentPoints = KCT_Utilities.TotalSpentUpgrades(null);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Total Points: " + KCT_GameStates.TotalUpgradePoints);
            GUILayout.Label("Available: " + (KCT_GameStates.TotalUpgradePoints - spentPoints));
            GUILayout.EndHorizontal();

            if (KCT_Utilities.CurrentGameHasScience())
            {
                //int cost = (int)Math.Min(Math.Pow(2, KCT_GameStates.PurchasedUpgrades[0]+2), 512);
                if (sciCost == 0)
                {
                    sciCost = (int)KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.UpgradeScienceFormula, new Dictionary<string, string>() { { "N", KCT_GameStates.PurchasedUpgrades[0].ToString() } });
                    int max = int.Parse(KCT_GameStates.formulaSettings.UpgradeScienceMax);
                    if (max > 0 && sciCost > max) sciCost = max;
                }
                int cost = sciCost;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Buy Point: ");
                if (GUILayout.Button(cost + " Sci", GUILayout.ExpandWidth(false)))
                {
                    sciCost = (int)KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.UpgradeScienceFormula, new Dictionary<string, string>() { { "N", KCT_GameStates.PurchasedUpgrades[0].ToString() } });
                    int max = int.Parse(KCT_GameStates.formulaSettings.UpgradeScienceMax);
                    if (max > 0 && sciCost > max) sciCost = max;
                    cost = sciCost;

                    if (ResearchAndDevelopment.Instance.Science >= cost)
                    {
                        //ResearchAndDevelopment.Instance.Science -= cost;
                        ResearchAndDevelopment.Instance.AddScience(-cost, TransactionReasons.None);
                        ++KCT_GameStates.TotalUpgradePoints;
                        ++KCT_GameStates.PurchasedUpgrades[0];

                        sciCost = (int)KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.UpgradeScienceFormula, new Dictionary<string, string>() { { "N", KCT_GameStates.PurchasedUpgrades[0].ToString() } });
                        if (max > 0 && sciCost > max) sciCost = max;
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (KCT_Utilities.CurrentGameIsCareer())
            {
                //double cost = Math.Min(Math.Pow(2, KCT_GameStates.PurchasedUpgrades[1]+4), 1024) * 1000;
                if (fundsCost == 0)
                {
                    fundsCost = (int)KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.UpgradeFundsFormula, new Dictionary<string, string>() { { "N", KCT_GameStates.PurchasedUpgrades[1].ToString() } });
                    int max = int.Parse(KCT_GameStates.formulaSettings.UpgradeFundsMax);
                    if (max > 0 && fundsCost > max) fundsCost = max;
                }
                int cost = fundsCost;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Buy Point: ");
                if (GUILayout.Button(cost + " Funds", GUILayout.ExpandWidth(false)))
                {
                    fundsCost = (int)KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.UpgradeFundsFormula, new Dictionary<string, string>() { { "N", KCT_GameStates.PurchasedUpgrades[1].ToString() } });
                    int max = int.Parse(KCT_GameStates.formulaSettings.UpgradeFundsMax);
                    if (max > 0 && fundsCost > max) fundsCost = max;
                    cost = fundsCost;

                    if (Funding.Instance.Funds >= cost)
                    {
                        KCT_Utilities.SpendFunds(cost, TransactionReasons.None);
                        ++KCT_GameStates.TotalUpgradePoints;
                        ++KCT_GameStates.PurchasedUpgrades[1];


                        fundsCost = (int)KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.UpgradeFundsFormula, new Dictionary<string, string>() { { "N", KCT_GameStates.PurchasedUpgrades[1].ToString() } });
                        if (max > 0 && fundsCost > max) fundsCost = max;
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Reset Upgrades: ");
            if (GUILayout.Button("2 Points", GUILayout.ExpandWidth(false)))
            {
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 1)
                {
                    KCT_GameStates.ActiveKSC.VABUpgrades = new List<int>() {0};
                    KCT_GameStates.ActiveKSC.SPHUpgrades = new List<int>() { 0 };
                    KCT_GameStates.ActiveKSC.RDUpgrades = new List<int>() { 0, 0 };
                    KCT_GameStates.TechUpgradesTotal = 0;
                    foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                        ksc.RDUpgrades[1] = 0;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VAB")) { upgradeWindowHolder = 0; upgradePosition.height = 1; }
            if (GUILayout.Button("SPH")) { upgradeWindowHolder = 1; upgradePosition.height = 1; }
            if (KCT_Utilities.CurrentGameHasScience() && GUILayout.Button("R&D")) { upgradeWindowHolder = 2; upgradePosition.height = 1; }
            GUILayout.EndHorizontal();
            KCT_KSC KSC = KCT_GameStates.ActiveKSC;

            if (upgradeWindowHolder==0) //VAB
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("VAB Upgrades");
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height((KSC.VABUpgrades.Count + 1) * 26), GUILayout.MaxHeight(3 * Screen.height / 4));
                GUILayout.BeginVertical();
                for (int i = 0; i < KSC.VABUpgrades.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Rate "+(i+1));
                    GUILayout.Label(KCT_Utilities.GetBuildRate(i, KCT_BuildListVessel.ListType.VAB, KSC) + " BP/s");
                    if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0 && (i == 0 || KCT_Utilities.GetBuildRate(i, KCT_BuildListVessel.ListType.VAB, KSC) + ((i + 1) * 0.05)
                        <= KCT_Utilities.GetBuildRate(i - 1, KCT_BuildListVessel.ListType.VAB, KSC)))
                    {
                        if (GUILayout.Button("+" + ((i + 1) * 0.05), GUILayout.Width(45)))
                        {
                            ++KSC.VABUpgrades[i];
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Rate " + (KSC.VABUpgrades.Count + 1));
                GUILayout.Label("0 BP/s");
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0 && ((KSC.VABUpgrades.Count + 1) * 0.05)
                    <= KCT_Utilities.GetBuildRate(KSC.VABUpgrades.Count - 1, KCT_BuildListVessel.ListType.VAB, KSC))
                {
                    if (GUILayout.Button("+" + ((KSC.VABUpgrades.Count + 1) * 0.05), GUILayout.Width(45)))
                    {
                        KSC.VABUpgrades.Add(1);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }

            if (upgradeWindowHolder == 1) //SPH
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("SPH Upgrades");
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height((KSC.SPHUpgrades.Count + 1) * 26), GUILayout.MaxHeight(3 * Screen.height / 4));
                GUILayout.BeginVertical();
                for (int i = 0; i < KSC.SPHUpgrades.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Rate " + (i + 1));
                    GUILayout.Label(KCT_Utilities.GetBuildRate(i, KCT_BuildListVessel.ListType.SPH, KSC) + " BP/s");
                    if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0 && (i == 0 || KCT_Utilities.GetBuildRate(i, KCT_BuildListVessel.ListType.SPH, KSC) + ((i + 1) * 0.05)
                        <= KCT_Utilities.GetBuildRate(i - 1, KCT_BuildListVessel.ListType.SPH, KSC)))
                    {
                        if (GUILayout.Button("+" + ((i + 1) * 0.05), GUILayout.Width(45)))
                        {
                            ++KSC.SPHUpgrades[i];
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Rate " + (KSC.SPHUpgrades.Count + 1));
                GUILayout.Label("0 BP/s");
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0 && ((KSC.SPHUpgrades.Count + 1) * 0.05)
                    <= KCT_Utilities.GetBuildRate(KSC.SPHUpgrades.Count - 1, KCT_BuildListVessel.ListType.SPH, KSC))
                {
                    if (GUILayout.Button("+" + ((KSC.SPHUpgrades.Count + 1) * 0.05), GUILayout.Width(45)))
                    {
                        KSC.SPHUpgrades.Add(1);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
            if (upgradeWindowHolder == 2) //R&D
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("R&D Upgrades");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Research");
                GUILayout.Label((KSC.RDUpgrades[0] * 0.5) + " sci/86400 BP");
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0)
                {
                    if (GUILayout.Button("+0.5", GUILayout.Width(45)))
                    {
                        ++KSC.RDUpgrades[0];
                    }
                }
                GUILayout.EndHorizontal();

                double days = GameSettings.KERBIN_TIME ? 4 : 1;
                if (nodeRate == 0)
                {
                    nodeRate = KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.NodeFormula, new Dictionary<string, string>() { {"N", KSC.RDUpgrades[1].ToString()} });
                    double max = double.Parse(KCT_GameStates.formulaSettings.NodeMax);
                    if (max > 0 && nodeRate > max) nodeRate = max;

                    upNodeRate = KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.NodeFormula, new Dictionary<string, string>() { { "N", (KSC.RDUpgrades[1]+1).ToString() } });
                    if (max > 0 && upNodeRate > max) upNodeRate = max;
                }
                double sci = 86400 * nodeRate;

                double sciPerDay = sci / days;
                //days *= KCT_GameStates.timeSettings.NodeModifier;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Development");
                GUILayout.Label(sciPerDay + " sci/day");
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0)
                {
                    bool everyKSCCanUpgrade = true;
                    foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                    {
                        if (KCT_GameStates.TotalUpgradePoints - KCT_Utilities.TotalSpentUpgrades(ksc) <= 0)
                        {
                            everyKSCCanUpgrade = false;
                            break;
                        }
                    }
                    if (everyKSCCanUpgrade && GUILayout.Button(86400*upNodeRate/days + " sci/day", GUILayout.ExpandWidth(false)))
                    {
                        ++KCT_GameStates.TechUpgradesTotal;
                        foreach (KCT_KSC ksc in KCT_GameStates.KSCs)
                            ksc.RDUpgrades[1] = KCT_GameStates.TechUpgradesTotal;

                        nodeRate = KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.NodeFormula, 
                            new Dictionary<string, string>() { { "N", KCT_GameStates.TechUpgradesTotal.ToString() }, { "S", "0" } });
                        double max = double.Parse(KCT_GameStates.formulaSettings.NodeMax);
                        if (max > 0 && nodeRate > max) nodeRate = max;

                        upNodeRate = KCT_Utilities.ParseMath(KCT_GameStates.formulaSettings.NodeFormula, 
                            new Dictionary<string, string>() { { "N", KCT_GameStates.TechUpgradesTotal.ToString() }, { "S", "0" } });
                        if (max > 0 && upNodeRate > max) upNodeRate = max;

                        foreach (KCT_TechItem tech in KCT_GameStates.TechList)
                        {
                            tech.UpdateBuildRate();
                        }
                    }
                }
                GUILayout.EndHorizontal();

            }
            if (GUILayout.Button("Close")) { showUpgradeWindow = false; if (!PrimarilyDisabled) showBuildList = true; }
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();
        }

        private static string newName = "";
        public static void DrawRenameWindow(int windowID)
        {
            if (centralWindowPosition.y != (Screen.height - centralWindowPosition.height) / 2)
            {
                centralWindowPosition.y = (Screen.height - centralWindowPosition.height) / 2;
                centralWindowPosition.height = 1;
            }
            GUILayout.BeginVertical();
            GUILayout.Label("Name:");
            newName = GUILayout.TextField(newName);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                KCT_BuildListVessel b = KCT_Utilities.FindBLVesselByID(IDSelected);
                b.shipName = newName; //Change the name from our point of view
                showRename = false;
                centralWindowPosition.width = 150;
                centralWindowPosition.x = (Screen.width - 150) / 2;
                showBuildList = true;
            }
            if (GUILayout.Button("Cancel"))
            {
                centralWindowPosition.width = 150;
                centralWindowPosition.x = (Screen.width - 150) / 2;
                showRename = false;
                showBuildList = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public static void DrawFirstRun(int windowID)
        {
            if (centralWindowPosition.width != 450)
            {
                centralWindowPosition.Set((Screen.width - 450) / 2, (Screen.height - 200) / 2, 450, 200);
            }
            GUILayout.BeginVertical();
            GUILayout.Label("Welcome to KCT! It is advised that you spend your " + (KCT_GameStates.TotalUpgradePoints-KCT_Utilities.TotalSpentUpgrades(null)) + " upgrades to increase the build rate in the building you will primarily be using.");
            GUILayout.Label("Please see the getting started guide included in the download or available from the forum for more information!");
            if (KCT_GameStates.settings.CheckForUpdates)
                GUILayout.Label("Due to your settings, automatic update checking is enabled. You can disable it in the Settings menu!");
            else
                GUILayout.Label("Due to your settings, automatic update checking is disabled. You can enable it in the Settings menu!");
            GUILayout.Label("\nNote: 0.24 introduced a bug that causes time to freeze while hovering over the Build List with the mouse cursor. Just move the cursor off of the window and time will resume.");
            if (GUILayout.Button("Spend Upgrades"))
            {
                showFirstRun = false;
                centralWindowPosition.height = 1;
                centralWindowPosition.width = 150;
                showUpgradeWindow = true;
            }
            if (GUILayout.Button("Settings"))
            {
                showFirstRun = false;
                centralWindowPosition.height = 1;
                centralWindowPosition.width = 150;
                ShowSettings();
            }
            if (GUILayout.Button("Close"))
            {
                showFirstRun = false;
                centralWindowPosition.height = 1;
                centralWindowPosition.width = 150;
                if (KCT_GameStates.settings.CheckForUpdates)
                    KCT_UpdateChecker.CheckForUpdate(true, KCT_GameStates.settings.VersionSpecific);
            }
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                GUI.DragWindow();
        }
    }

    public class GUIPosition
    {
        [Persistent] public string guiName;
        [Persistent] public float xPos, yPos;
        [Persistent] public bool visible;

        public GUIPosition() { }
        public GUIPosition(string name, float x, float y, bool vis)
        {
            guiName = name;
            xPos = x;
            yPos = y;
            visible = vis;
        }
    }

    public class GUIDataSaver
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_Windows.txt";
        [Persistent] GUIPosition editorPositionSaved, buildListPositionSaved, timeLimitPositionSaved;
        public void Save()
        {
           // buildListPositionSaved = new GUIPosition("buildList", KCT_GUI.buildListWindowPosition.x, KCT_GUI.buildListWindowPosition.y, KCT_GameStates.showWindows[0]);
            editorPositionSaved = new GUIPosition("editor", KCT_GUI.editorWindowPosition.x, KCT_GUI.editorWindowPosition.y, KCT_GameStates.showWindows[1]);
            timeLimitPositionSaved = new GUIPosition("timeLimit", KCT_GUI.timeRemainingPosition.x, KCT_GUI.timeRemainingPosition.y, KCT_GUI.showTimeRemaining);

            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(filePath);
        }

        public void Load()
        {
            if (!System.IO.File.Exists(filePath))
                return;

            ConfigNode cnToLoad = ConfigNode.Load(filePath);
            ConfigNode.LoadObjectFromConfig(this, cnToLoad);

            /*KCT_GUI.buildListWindowPosition.x = buildListPositionSaved.xPos;
            KCT_GUI.buildListWindowPosition.y = buildListPositionSaved.yPos;
            KCT_GameStates.showWindows[0] = buildListPositionSaved.visible;*/

            KCT_GUI.editorWindowPosition.x = editorPositionSaved.xPos;
            KCT_GUI.editorWindowPosition.y = editorPositionSaved.yPos;
            KCT_GameStates.showWindows[1] = editorPositionSaved.visible;

            KCT_GUI.timeRemainingPosition.x = timeLimitPositionSaved.xPos;
            KCT_GUI.timeRemainingPosition.y = timeLimitPositionSaved.yPos;
            //We don't care about it's visibility. That's determined separately.
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