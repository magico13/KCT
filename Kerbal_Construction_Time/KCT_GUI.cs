using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Construction_Time
{
    public static class KCT_GUI
    {
        public static bool showMainGUI, showEditorGUI, showSOIAlert, showLaunchAlert, showSimulationCompleteEditor, showSimulationWindow, showTimeRemaining, 
            showSimulationCompleteFlight, showBuildList, showClearLaunch, showShipRoster, showCrewSelect, showSettings, showSimConfig, showBodyChooser, showUpgradeWindow,
            showBLPlus, showRename;

        private static bool unlockEditor;

        private static Vector2 scrollPos;

        private static Rect iconPosition = new Rect(Screen.width / 4, Screen.height - 30, 50, 30);//110
        private static Rect mainWindowPosition = new Rect(Screen.width / 3.5f, Screen.height / 3.5f, 350, 200);
        private static Rect editorWindowPosition = new Rect(Screen.width / 3.5f, Screen.height / 3.5f, 275, 135);
        private static Rect SOIAlertPosition = new Rect(Screen.width / 3, Screen.height / 3, 250, 100);

        private static Rect centralWindowPosition = new Rect((Screen.width - 150) / 2, (Screen.height - 50) / 2, 150, 50);

        //private static Rect launchAlertPosition = new Rect((Screen.width-75)/2, (Screen.height-100)/2, 150, 100);
        //private static Rect simulationCompleteEditorPosition = new Rect((Screen.width - 75) / 2, (Screen.height - 100) / 2, 150, 100);
        //private static Rect simulationCompleteFlightPosition = new Rect((Screen.width - 75) / 2, (Screen.height - 100) / 2, 150, 100);
        private static Rect simulationWindowPosition = new Rect((Screen.width - 250) / 2, (Screen.height - 250) / 2, 250, 250);
        private static Rect timeRemainingPosition = new Rect((Screen.width-90) / 4, Screen.height - 85, 90, 55);
        private static Rect buildListWindowPosition = new Rect(Screen.width / 3.5f, Screen.height / 3.5f, 460, 1);
        private static Rect crewListWindowPosition = new Rect((3*Screen.width/8), (Screen.height / 4), Screen.width / 4, 1);
        private static Rect settingsPosition = new Rect((3 * Screen.width / 8), (Screen.height / 4), 300, 1);
        private static Rect upgradePosition = new Rect((3 * Screen.width / 8), (Screen.height / 4), 240, 1);
        private static Rect simulationConfigPosition = new Rect((Screen.width / 2)-120, (Screen.height / 4), 240, 1);
        private static Rect bLPlusPosition = new Rect(1, 1 / 2, 150, 1);
        private static GUIStyle windowStyle = new GUIStyle(HighLogic.Skin.window);

        private static List<GameScenes> validScenes = new List<GameScenes> { GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.SPH, GameScenes.SPACECENTER, GameScenes.TRACKSTATION };
        public static void SetGUIPositions(GUI.WindowFunction OnWindow)
        {
            if (validScenes.Contains(HighLogic.LoadedScene)) //&& KCT_GameStates.settings.enabledForSave)//!(HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX && !KCT_GameStates.settings.SandboxEnabled))
            {
                if (!ToolbarManager.ToolbarAvailable && GUI.Button(iconPosition, "KCT", GUI.skin.button))
                {
                    onClick();
                }
                else if (ToolbarManager.ToolbarAvailable)
                {
                    KCT_GameStates.kctToolbarButton.TexturePath = KCT_Utilities.GetButtonTexture(); //Set texture, allowing for flashing of icon.
                }

                if (showSettings)
                    settingsPosition = GUILayout.Window(8955, settingsPosition, KCT_GUI.DrawSettings, "KCT Settings", windowStyle);
                if (!KCT_GameStates.settings.enabledForSave)
                    return;

                if (showMainGUI)
                    mainWindowPosition = GUILayout.Window(8950, mainWindowPosition, KCT_GUI.DrawMainGUI, "Kerbal Construction Time", windowStyle);
                if (showEditorGUI)
                    editorWindowPosition = GUILayout.Window(8950, editorWindowPosition, KCT_GUI.DrawEditorGUI, "Kerbal Construction Time", windowStyle);
                if (showSOIAlert)
                    SOIAlertPosition = GUILayout.Window(8951, SOIAlertPosition, KCT_GUI.DrawSOIAlertWindow, "SOI Change", windowStyle);
                if (showLaunchAlert)
                    centralWindowPosition = GUILayout.Window(8951, centralWindowPosition, KCT_GUI.DrawLaunchAlert, "Build or Simulate?", windowStyle);
                if (showSimulationCompleteEditor)
                    centralWindowPosition = GUILayout.Window(8951, centralWindowPosition, KCT_GUI.DrawSimulationCompleteEditor, "Simulation Complete!", windowStyle);
                if (showSimulationCompleteFlight)
                    centralWindowPosition = GUILayout.Window(8952, centralWindowPosition, KCT_GUI.DrawSimulationCompleteFlight, "Simulation Complete!", windowStyle);
                if (showSimulationWindow)
                    simulationWindowPosition = GUILayout.Window(8950, simulationWindowPosition, KCT_GUI.DrawSimulationWindow, "KCT Simulation", windowStyle);
                if (showTimeRemaining)
                    timeRemainingPosition = GUILayout.Window(8951, timeRemainingPosition, KCT_GUI.DrawSimulationTimeWindow, "Time left:", windowStyle);
                if (showBuildList)
                    buildListWindowPosition = GUILayout.Window(8950, buildListWindowPosition, KCT_GUI.DrawBuildListWindow, "Build List", windowStyle);
                if (showClearLaunch)
                    centralWindowPosition = GUILayout.Window(8952, centralWindowPosition, KCT_GUI.DrawClearLaunch, "Launch site not clear!", windowStyle);
                if (showShipRoster)
                    crewListWindowPosition = GUILayout.Window(8953, crewListWindowPosition, KCT_GUI.DrawShipRoster, "Select Crew", windowStyle);
                if (showCrewSelect)
                    crewListWindowPosition = GUILayout.Window(8954, crewListWindowPosition, KCT_GUI.DrawCrewSelect, "Select Crew", windowStyle);
                if (showSimConfig)
                    simulationConfigPosition = GUILayout.Window(8951, simulationConfigPosition, KCT_GUI.DrawSimulationConfigure, "Simulation Configuration", windowStyle);
                if (showBodyChooser)
                    centralWindowPosition = GUILayout.Window(8952, centralWindowPosition, KCT_GUI.DrawBodyChooser, "Choose Body", windowStyle);
                if (showUpgradeWindow)
                    upgradePosition = GUILayout.Window(8952, upgradePosition, KCT_GUI.DrawUpgradeWindow, "Upgrades", windowStyle);
                if (showBLPlus)
                    bLPlusPosition = GUILayout.Window(8953, bLPlusPosition, KCT_GUI.DrawBLPlusWindow, "Options", windowStyle);
                if (showRename)
                    centralWindowPosition = GUILayout.Window(8954, centralWindowPosition, KCT_GUI.DrawRenameWindow, "Rename", windowStyle);

                if (unlockEditor)
                {
                    EditorLogic.fetch.Unlock("KCTGUILock");
                    unlockEditor = false;
                }
            }
        }

        public static void onClick()
        {
            if (ToolbarManager.ToolbarAvailable)
                if (KCT_GameStates.kctToolbarButton.Important) KCT_GameStates.kctToolbarButton.Important = false;

            if (!KCT_GameStates.settings.enabledForSave)
            {
                ShowSettings();
                return;
            }

            if (HighLogic.LoadedScene == GameScenes.FLIGHT && !KCT_GameStates.flightSimulated)
            {
                //showMainGUI = !showMainGUI;
                buildListWindowPosition.height = 1;
                showBuildList = !showBuildList;
                showBLPlus = false;
                listWindow = 0;
            }
            else if (HighLogic.LoadedScene == GameScenes.FLIGHT && KCT_GameStates.flightSimulated)
            {
                showSimulationWindow = !showSimulationWindow;
            }
            else if ((HighLogic.LoadedScene == GameScenes.EDITOR) || (HighLogic.LoadedScene == GameScenes.SPH))
            {
                showEditorGUI = !showEditorGUI;
            }
            else if ((HighLogic.LoadedScene == GameScenes.SPACECENTER) || (HighLogic.LoadedScene == GameScenes.TRACKSTATION))
            {
                buildListWindowPosition.height = 1;
                showBuildList = !showBuildList;
                showBLPlus = false;
                listWindow = 0;
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
            if (showTimeRemaining && KCT_GameStates.settings.SimulationTimeLimit > 0)
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
            {
                GUI.DragWindow();
            }
        }

        private static bool showInventory = false;
        private static string currentCategoryString = "NONE";
        private static void DrawEditorGUI(int windowID)
        {
            double buildTime = KCT_Utilities.GetBuildTime(EditorLogic.fetch.ship.Parts);
            KCT_BuildListVessel.ListType type = EditorLogic.fetch.launchSiteName == "LaunchPad" ? KCT_BuildListVessel.ListType.VAB : KCT_BuildListVessel.ListType.SPH;
            GUILayout.BeginVertical();
            GUILayout.Label("Total Build Points (BP):", GUILayout.ExpandHeight(true));
            GUILayout.Label(buildTime.ToString(), GUILayout.ExpandHeight(true));
            GUILayout.Label("Build Time at " + KCT_Utilities.GetBuildRate(0, type) + " BP/s:", GUILayout.ExpandHeight(true));
            GUILayout.Label(KCT_Utilities.GetFormattedTime(buildTime / KCT_Utilities.GetBuildRate(0, type)), GUILayout.ExpandHeight(true));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build"))
            {
                KCT_GameStates.flightSimulated = false;
                KCT_Utilities.disableSimulationLocks();
                KCT_Utilities.AddVesselToBuildList();
                //showLaunchAlert = true;
            }
            if (GUILayout.Button("Simulate"))
            {
                simulationConfigPosition.height = 1;
                EditorLogic.fetch.Lock(true, false, true, "KCTGUILock");
                showSimConfig = true;
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Part Inventory", GUILayout.ExpandHeight(true)))
            {
                showInventory = !showInventory;
                editorWindowPosition.width = 275;
                editorWindowPosition.height = 135;
            }

            
            if (showInventory)
            {
                List<String> categories = new List<String> { "Pods", "Propulsion", "Control", "Structural", "Aero", "Utility", "Science" };
                Dictionary<String, int> activeList = new Dictionary<string, int>();
                PartCategories CategoryCurrent = PartCategories.none;
                switch (currentCategoryString)
                {
                    case "Aero": CategoryCurrent = PartCategories.Aero; break;
                    case "Control": CategoryCurrent = PartCategories.Control; break;
                    case "Pods": CategoryCurrent = PartCategories.Pods; break;
                    case "Propulsion": CategoryCurrent = PartCategories.Propulsion; break;
                    case "Science": CategoryCurrent = PartCategories.Science; break;
                    case "Structural": CategoryCurrent = PartCategories.Structural; break;
                    case "Utility": CategoryCurrent = PartCategories.Utility; break;
                }

                for (int i = 0; i < KCT_GameStates.PartInventory.Count; i++)
                {
                    PartCategories category = PartCategories.none;
                    string name = KCT_GameStates.PartInventory.Keys.ElementAt(i);
                    foreach (AvailablePart p in PartLoader.LoadedPartsList)
                    {
                        if (p.name == name)
                        {
                            name = p.title;
                            category = p.category;
                            if (p.category == CategoryCurrent && !activeList.Keys.Contains(name))
                                activeList.Add(name, KCT_GameStates.PartInventory.Values.ElementAt(i));
                            break;
                        }
                    }
                }
                GUILayout.BeginHorizontal();
                foreach (string cat in categories)
                {
                    if (GUILayout.Button(cat))
                    {
                        currentCategoryString = cat;
                        editorWindowPosition.height = 135;
                    }
                }
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((activeList.Count + 1) * 30, Screen.height / 1.4F))); //
                GUILayout.BeginHorizontal();
                /*GUILayout.Label("Name:", GUILayout.ExpandHeight(true));
                GUILayout.Label("Available:", GUILayout.ExpandHeight(true));
                GUILayout.Label("In use:", GUILayout.ExpandHeight(true));
                GUILayout.EndHorizontal();

                PartCategories CategoryCurrent = PartCategories.none;
                switch (currentCategoryString)
                {
                    case "AERO": CategoryCurrent = PartCategories.Aero; break;
                    case "CONTROL": CategoryCurrent = PartCategories.Control; break;
                    case "PODS": CategoryCurrent = PartCategories.Pods; break;
                    case "PROPULSION": CategoryCurrent = PartCategories.Propulsion; break;
                    case "SCIENCE": CategoryCurrent = PartCategories.Science; break;
                    case "STRUCTURAL": CategoryCurrent = PartCategories.Structural; break;
                    case "UTILITY": CategoryCurrent = PartCategories.Utility; break;
                }

                for (int i = 0; i < KCT_GameStates.PartInventory.Count; i++)
                {
                    PartCategories category = PartCategories.none;
                    string name = KCT_GameStates.PartInventory.Keys.ElementAt(i);
                    foreach (AvailablePart p in PartLoader.LoadedPartsList)
                    {
                        if (p.name == name)
                        {
                            name = p.title;
                            category = p.category;
                            break;
                        }
                    }
                    if (CategoryCurrent == category)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(name, GUILayout.ExpandHeight(true));
                        GUILayout.Label(KCT_GameStates.PartInventory.Values.ElementAt(i).ToString(), GUILayout.ExpandHeight(true));
                        int inUse = 0;
                        String pName = KCT_GameStates.PartInventory.Keys.ElementAt(i);
                        for (int j = 0; j < EditorLogic.fetch.ship.parts.Count; j++)
                        {
                            if (EditorLogic.fetch.ship.parts[j].partInfo.name == pName)
                                ++inUse;
                        }
                        GUILayout.Label(inUse.ToString(), GUILayout.ExpandHeight(true));
                        GUILayout.EndHorizontal();
                    }
                }*/

                   GUILayout.BeginVertical();
                   GUILayout.Label("Name:", GUILayout.ExpandHeight(true));
                   for (int i=0; i<activeList.Count; i++)
                   {
                        string name = activeList.Keys.ElementAt(i);
                        GUILayout.Label(name, GUILayout.ExpandHeight(true));   
                   }
                   GUILayout.EndVertical();
                
                   GUILayout.BeginVertical();
                   GUILayout.Label("Available:", GUILayout.ExpandHeight(true));
                   for (int i = 0; i < activeList.Count; i++)
                   {
                       GUILayout.Label(activeList.Values.ElementAt(i).ToString(), GUILayout.ExpandHeight(true));
                   }
                   GUILayout.EndVertical();

                   GUILayout.BeginVertical();
                   GUILayout.Label("In use:", GUILayout.ExpandHeight(true));
                   for (int i = 0; i < activeList.Count; i++)
                   {
                       int inUse = 0;
                       String name = activeList.Keys.ElementAt(i);
                       for (int j = 0; j < EditorLogic.fetch.ship.parts.Count; j++)
                       {
                           if (EditorLogic.fetch.ship.parts[j].partInfo.title == name)
                               ++inUse;
                       }
                       GUILayout.Label(inUse.ToString(), GUILayout.ExpandHeight(true));
                   }
                   GUILayout.EndVertical();
                   GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
            }
           /* else
            {
                if (editorWindowPosition.height > 135)
                    editorWindowPosition.height = 135;
            }*/
            

            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                GUI.DragWindow();
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
            {
                GUI.DragWindow();
            }
        }

        private static string orbitAltString="", orbitIncString="";
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
                GUILayout.Label("Orbit Altitude: ");
                orbitAltString = GUILayout.TextField(orbitAltString, GUILayout.Width(100));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Min: " + KCT_GameStates.simulationBody.maxAtmosphereAltitude);
                GUILayout.Label("Max: " + Math.Floor(KCT_GameStates.simulationBody.sphereOfInfluence));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Inclination: ");
                orbitIncString = GUILayout.TextField(orbitIncString, GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Simulate"))
            {
                if (KCT_GameStates.simulationBody.bodyName != "Kerbin")
                    KCT_GameStates.simulateInOrbit = true;

                if (KCT_GameStates.simulateInOrbit)
                {
                    if (!double.TryParse(orbitAltString, out KCT_GameStates.simOrbitAltitude))
                        KCT_GameStates.simOrbitAltitude = KCT_GameStates.simulationBody.maxAtmosphereAltitude + 1000;
                    else
                        KCT_GameStates.simOrbitAltitude = Math.Min(Math.Max(KCT_GameStates.simOrbitAltitude, KCT_GameStates.simulationBody.maxAtmosphereAltitude), KCT_GameStates.simulationBody.sphereOfInfluence);

                    if (!double.TryParse(orbitIncString, out KCT_GameStates.simInclination))
                        KCT_GameStates.simInclination = 0;
                    else
                        KCT_GameStates.simInclination = KCT_GameStates.simInclination % 360;
                }
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                unlockEditor = true;
                EditorLogic.fetch.launchVessel();
                showSimConfig = false;
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Cancel"))
            {
                showSimConfig = false;
                centralWindowPosition.height = 1;
                unlockEditor = true;
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
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
        }

        public static void DrawLaunchAlert(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Add to Build List"))
            {
                KCT_GameStates.flightSimulated = false;
                KCT_Utilities.disableSimulationLocks();
                KCT_Utilities.AddVesselToBuildList();
                showLaunchAlert = false;
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Simulate Launch"))
            {
                /*KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                EditorLogic.fetch.launchVessel();*/
                showLaunchAlert = false;
                showSimConfig = true;
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Cancel"))
            {
                showLaunchAlert = false;
                centralWindowPosition.height = 1;
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
           /* if (GUILayout.Button("Build")) //Doesn't work until I can add it to the build list no problem
            {
                KCT_GameStates.flightSimulated = false;
                KCT_Utilities.disableSimulationLocks();
                //FlightDriver.RevertToLaunch();
                if (FlightDriver.LaunchSiteName == "LaunchPad")
                    FlightDriver.RevertToPrelaunch(GameScenes.EDITOR);
                else
                    FlightDriver.RevertToPrelaunch(GameScenes.SPH);
                
            }*/

            if (GUILayout.Button("Restart Simulation"))
            {
                Kerbal_Construction_Time.moved = false;
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                KCT_GameStates.simulationEndTime = 0;
                if (MCEWrapper.MCEAvailable) //Support for MCE
                    MCEWrapper.IloadMCEbackup();
                FlightDriver.RevertToLaunch();
                centralWindowPosition.height = 1;
            }

            if (GUILayout.Button("Revert to Editor"))
            {
                KCT_GameStates.simulationReason = "USER";
                Debug.Log("[KCT] Simulation complete: " + "USER");
                KCT_Utilities.disableSimulationLocks();
                KCT_GameStates.flightSimulated = false;
                KCT_GameStates.simulationEndTime = 0;
                if (MCEWrapper.MCEAvailable) //Support for MCE
                    MCEWrapper.IloadMCEbackup();
                if (FlightDriver.LaunchSiteName == "LaunchPad")
                    FlightDriver.RevertToPrelaunch(GameScenes.EDITOR);
                else if (FlightDriver.LaunchSiteName == "Runway")
                    FlightDriver.RevertToPrelaunch(GameScenes.SPH);
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
            {
                GUI.DragWindow();
            }
        }

        public static void DrawSimulationWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("This is a simulation. It will end when one of the following conditions are met:");
            GUILayout.Label("The time limit is exceeded");
            GUILayout.Label("The flight scene is exited");
            GUILayout.Label(" ");
            GUILayout.Label("You cannot save or switch vessels during a simulation.");
            GUILayout.Label("Note: If you want to build this ship, press the Build button in the editor.");
            /*if (GUILayout.Button("End Simulation"))
            {
                showSimulationCompleteFlight = true;
                showSimulationWindow = false;
            }*/
            if (GUILayout.Button("Restart Simulation"))
            {
                showSimulationWindow = false;
                Kerbal_Construction_Time.moved = false;
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                KCT_GameStates.simulationEndTime = 0;
                if (MCEWrapper.MCEAvailable) //Support for MCE
                    MCEWrapper.IloadMCEbackup();
                FlightDriver.RevertToLaunch();
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Revert to Editor"))
            {
                showSimulationWindow = false;
                KCT_GameStates.simulationReason = "USER";
                Debug.Log("[KCT] Simulation complete: " + "USER");
                KCT_Utilities.disableSimulationLocks();
                KCT_GameStates.flightSimulated = false;
                KCT_GameStates.simulationEndTime = 0;
                if (MCEWrapper.MCEAvailable) //Support for MCE
                    MCEWrapper.IloadMCEbackup();
                if (FlightDriver.LaunchSiteName == "LaunchPad")
                    FlightDriver.RevertToPrelaunch(GameScenes.EDITOR);
                else if (FlightDriver.LaunchSiteName == "Runway")
                    FlightDriver.RevertToPrelaunch(GameScenes.SPH);
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Close"))
            {
                showSimulationWindow = !showSimulationWindow;
            }
            GUILayout.EndVertical();
        }

        public static string newMultiplier, newBuildEffect, newInvEffect, newTimeWarp, newSandboxUpgrades, newUpgradeCount, newTimeLimit;
        public static bool enabledForSave, enableAllBodies, forceStopWarp, instantTechUnlock;
        private static void ShowSettings()
        {
            newMultiplier = KCT_GameStates.timeSettings.OverallMultiplier.ToString();
            newBuildEffect = KCT_GameStates.timeSettings.BuildEffect.ToString();
            newInvEffect = KCT_GameStates.timeSettings.InventoryEffect.ToString();
            enabledForSave = KCT_GameStates.settings.enabledForSave;
            enableAllBodies = KCT_GameStates.settings.EnableAllBodies;
            newTimeWarp = KCT_GameStates.settings.MaxTimeWarp.ToString();
            forceStopWarp = KCT_GameStates.settings.ForceStopWarp;
            newSandboxUpgrades = KCT_GameStates.settings.SandboxUpgrades.ToString();
            newUpgradeCount = KCT_GameStates.TotalUpgradePoints.ToString();
            newTimeLimit = KCT_GameStates.settings.SimulationTimeLimit.ToString();
            instantTechUnlock = KCT_GameStates.settings.InstantTechUnlock;
            settingsPosition.height = 1;
            showSettings = !showSettings;
        }

        public static void ResetBLWindow()
        {
            buildListWindowPosition.height = 1;
            buildListWindowPosition.width = 460;
        }

        private static int listWindow = 0;
        public static void DrawBuildListWindow(int windowID)
        {
            int width1 = 120;
            int width2 = 100;
            int butW = 20;
            GUILayout.BeginVertical();
            //List next vessel to finish
            GUILayout.BeginHorizontal();
            GUILayout.Label("Next vessel:");
            IKCTBuildItem buildItem = KCT_Utilities.NextThingToFinish();
            if (buildItem != null)
            {
                //KCT_BuildListVessel ship = (KCT_BuildListVessel)buildItem;
                GUILayout.Label(buildItem.GetItemName());
                if (buildItem.GetListType() == KCT_BuildListVessel.ListType.VAB)
                {
                    GUILayout.Label("VAB");
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(buildItem.GetTimeLeft()));
                }
                else if (buildItem.GetListType() == KCT_BuildListVessel.ListType.SPH)
                {
                    GUILayout.Label("SPH");
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(buildItem.GetTimeLeft()));
                }
                else if (buildItem.GetListType() == KCT_BuildListVessel.ListType.TechNode)
                {
                    GUILayout.Label("Tech");
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(buildItem.GetTimeLeft()));
                }
                
                
                if (TimeWarp.CurrentRateIndex == 0 && GUILayout.Button("Warp to" + System.Environment.NewLine + "Complete"))
                {
                    KCT_GameStates.canWarp = true;
                    //Should ramp up time instead
                    //TimeWarp.SetRate(7, true);
                    KCT_Utilities.RampUpWarp();
                    KCT_GameStates.warpInitiated = true;
                }
                else if (TimeWarp.CurrentRateIndex > 0 && GUILayout.Button("Stop" + System.Environment.NewLine + "Warp"))
                {
                    KCT_GameStates.canWarp = false;
                    TimeWarp.SetRate(0, true);
                    KCT_GameStates.lastWarpRate = 0;
                }
            }
            else
            {
                GUILayout.Label("No Building Vessels");
            }
            GUILayout.EndHorizontal();

            //Buttons for VAB/SPH lists
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VAB List"))
            {
                if (listWindow == 1)
                    listWindow = 0;
                else
                    listWindow = 1;
                //buildListWindowPosition.height = 32 * KCT_GameStates.VABList.Count+3*32;
                buildListWindowPosition.height = 1;
                showBLPlus = false;
            }
            if (GUILayout.Button("SPH List"))
            {
                if (listWindow == 2)
                    listWindow = 0;
                else
                    listWindow = 2;
                //buildListWindowPosition.height = 32 * KCT_GameStates.SPHList.Count + 3 * 32;
                buildListWindowPosition.height = 1;
                showBLPlus = false;
            }
            if (GUILayout.Button("VAB Storage"))
            {
                if (listWindow == 3)
                    listWindow = 0;
                else
                    listWindow = 3;
                //buildListWindowPosition.height = 32 * KCT_GameStates.VABWarehouse.Count + 3 * 32;
                buildListWindowPosition.height = 1;
                showBLPlus = false;
            }
            if (GUILayout.Button("SPH Storage"))
            {
                if (listWindow == 4)
                    listWindow = 0;
                else
                    listWindow = 4;
                //buildListWindowPosition.height = 32 * KCT_GameStates.SPHWarehouse.Count + 3 * 32;
                buildListWindowPosition.height = 1;
                showBLPlus = false;
            }
            if (KCT_Utilities.CurrentGameIsCareer() && !KCT_GameStates.settings.InstantTechUnlock && GUILayout.Button("Tech"))
            {
                if (listWindow == 5)
                    listWindow = 0;
                else
                    listWindow = 5;
                //buildListWindowPosition.height = 32 * KCT_GameStates.SPHWarehouse.Count + 3 * 32;
                buildListWindowPosition.height = 1;
                showBLPlus = false;
            }
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
            //Content of lists
            if (listWindow==1) //VAB Build List
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.VABList;
                GUILayout.Label("VAB Build List");
                GUILayout.BeginHorizontal();
                //GUILayout.Label("Name:", GUILayout.Width(width1));
                GUILayout.Label("Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1/2));
                GUILayout.Label("Time Left:", GUILayout.Width(width2));
                GUILayout.Label("BP:", GUILayout.Width(width1 / 2));
                GUILayout.Space((butW + 4) * 3);
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((buildList.Count) * 25 + 10, Screen.height / 1.4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    //GUILayout.Label(b.shipName, GUILayout.Width(width1));
                    GUILayout.Label(b.shipName);
                    GUILayout.Label(Math.Round(b.ProgressPercent(), 2).ToString() + "%", GUILayout.Width(width1/2));
                    if (b.buildRate > 0)
                        GUILayout.Label(KCT_Utilities.GetColonFormattedTime(b.timeLeft), GUILayout.Width(width2));
                    else
                        GUILayout.Label("Est: " + KCT_Utilities.GetColonFormattedTime(b.buildPoints - b.progress), GUILayout.Width(width2));
                    GUILayout.Label(Math.Round(b.buildPoints, 2).ToString(), GUILayout.Width(width1 / 2));
                    if (i > 0 && GUILayout.Button("^", GUILayout.Width(butW)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i - 1, b);
                    }
                    else if (i == 0)
                    {
                        GUILayout.Space(butW+4);
                    }
                    /*if (i > 0 && GUILayout.Button("TOP", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(0, b);
                    }*/
                    if (i < buildList.Count - 1 && GUILayout.Button("v", GUILayout.Width(butW)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i + 1, b);
                    }
                    else if (i >= buildList.Count - 1)
                    {
                        GUILayout.Space(butW+4);
                    }
                    if (GUILayout.Button("*", GUILayout.Width(butW)))
                    {
                        if (IndexSelected == i)
                            showBLPlus = !showBLPlus;
                        else
                            showBLPlus = true;
                        IndexSelected = i;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow==2) //SPH Build List
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.SPHList;
                GUILayout.Label("SPH Build List");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1/2));
                GUILayout.Label("Time Left:", GUILayout.Width(width2));
                GUILayout.Label("BP:", GUILayout.Width(width1/2));
                GUILayout.Space((butW + 4) * 3);
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((buildList.Count) * 25 + 10, Screen.height / 1.4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName);
                    GUILayout.Label(Math.Round(b.ProgressPercent(), 2).ToString() + "%", GUILayout.Width(width1/2));
                    if (b.buildRate > 0)
                        GUILayout.Label(KCT_Utilities.GetColonFormattedTime(b.timeLeft), GUILayout.Width(width2));
                    else
                        GUILayout.Label("Est: " + KCT_Utilities.GetColonFormattedTime(b.buildPoints - b.progress), GUILayout.Width(width2));
                    GUILayout.Label(Math.Round(b.buildPoints, 2).ToString(), GUILayout.Width(width1 / 2));
                    if (i > 0 && GUILayout.Button("^", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i - 1, b);
                    }
                    else if (i==0)
                    {
                        GUILayout.Space(butW+4);
                    }
                    /*if (GUILayout.Button("TOP", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(0, b);
                    }*/
                    if (i < buildList.Count - 1 && GUILayout.Button("v", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i + 1, b);
                    }
                    else if (i >= buildList.Count - 1)
                    {
                        GUILayout.Space(butW + 4);
                    }
                    if (GUILayout.Button("*", GUILayout.Width(butW)))
                    {
                        if (IndexSelected == i)
                            showBLPlus = !showBLPlus;
                        else
                            showBLPlus = true;
                        IndexSelected = i;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow==3) //VAB Warehouse
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.VABWarehouse;
                GUILayout.Label("VAB Storage");
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((buildList.Count) * 25 + 10, Screen.height / 1.4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName);
                    if (GUILayout.Button("Launch", GUILayout.ExpandWidth(false)))
                    {
                        KCT_GameStates.launchedVessel = b;
                        if (ShipAssembly.CheckLaunchSiteClear(HighLogic.CurrentGame.flightState, "LaunchPad", false))
                        {
                           // buildList.RemoveAt(i);
                            if (!IsCrewable(b.ExtractedParts))
                                b.Launch();
                            else
                            {
                                showBuildList = false;
                                centralWindowPosition.height = 1;
                                KCT_GameStates.launchedCrew.Clear();
                                //KCT_GameStates.launchedCrew.Add(FirstCrewable(b.ExtractedParts).uid, HighLogic.CurrentGame.CrewRoster.DefaultCrewForVessel(b.shipNode, true).GetAllCrew(true));
                                partNames = KCT_GameStates.launchedVessel.GetPartNames();
                                parts = KCT_GameStates.launchedVessel.ExtractedParts;
                                if (KCT_GameStates.launchedCrew.Count != partNames.Count)
                                {
                                    KCT_GameStates.launchedCrew = new List<List<ProtoCrewMember>>();
                                    foreach (string s in partNames)
                                        KCT_GameStates.launchedCrew.Add(new List<ProtoCrewMember>());
                                }
                                KCT_GameStates.launchedCrew[FirstCrewable(parts)] = HighLogic.CurrentGame.CrewRoster.DefaultCrewForVessel(b.shipNode, true).GetAllCrew(true);
                                showShipRoster = true;
                            }
                        }
                        else
                        {
                            showBuildList = false;
                            showClearLaunch = true;
                        }
                    }
                    if (GUILayout.Button("*", GUILayout.Width(butW)))
                    {
                        if (IndexSelected == i)
                            showBLPlus = !showBLPlus;
                        else
                            showBLPlus = true;
                        IndexSelected = i;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow==4) //SPH Warehouse
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.SPHWarehouse;
                GUILayout.Label("SPH Storage");
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((buildList.Count) * 25 + 10, Screen.height / 1.4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName);
                    if (GUILayout.Button("Launch", GUILayout.ExpandWidth(false)))
                    {
                        KCT_GameStates.launchedVessel = b;
                        if (ShipAssembly.CheckLaunchSiteClear(HighLogic.CurrentGame.flightState, "Runway", false))
                        {
                            //buildList.RemoveAt(i);
                            if (!IsCrewable(b.ExtractedParts))//For a ship that hasn't been "loaded", this returns true!?
                                b.Launch();
                            else
                            {
                                showBuildList = false;
                                centralWindowPosition.height = 1;
                                KCT_GameStates.launchedCrew.Clear();
                                //KCT_GameStates.launchedCrew.Add(FirstCrewable(b.GetShip().parts).uid, HighLogic.CurrentGame.CrewRoster.DefaultCrewForVessel(b.shipNode, true).GetAllCrew(true));
                                partNames = KCT_GameStates.launchedVessel.GetPartNames();
                                parts = KCT_GameStates.launchedVessel.ExtractedParts;
                                if (KCT_GameStates.launchedCrew.Count != partNames.Count)
                                {
                                    KCT_GameStates.launchedCrew = new List<List<ProtoCrewMember>>();
                                    foreach (string s in partNames)
                                        KCT_GameStates.launchedCrew.Add(new List<ProtoCrewMember>());
                                }
                                KCT_GameStates.launchedCrew[FirstCrewable(parts)] = HighLogic.CurrentGame.CrewRoster.DefaultCrewForVessel(b.shipNode, true).GetAllCrew(true);
                                
                                showShipRoster = true;
                            }
                        }
                        else
                        {
                            showBuildList = false;
                            showClearLaunch = true;
                        }
                    }
                    if (GUILayout.Button("*", GUILayout.Width(butW)))
                    {
                        if (IndexSelected == i)
                            showBLPlus = !showBLPlus;
                        else
                            showBLPlus = true;
                        IndexSelected = i;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow == 5) //R&D
            {
                List<KCT_TechItem> techList = KCT_GameStates.TechList;
                GUILayout.Label("Research and Development");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Node Name:");
                GUILayout.Label("Progress:", GUILayout.Width(width1));
                GUILayout.Label("Time Left:", GUILayout.Width(width1));
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Math.Min((techList.Count) * 25 + 10, Screen.height / 1.4F)));
                for (int i = 0; i < techList.Count; i++)
                {
                    KCT_TechItem t = techList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(t.techName);
                    GUILayout.Label(Math.Round(100 * t.progress / t.scienceCost, 2) + " %", GUILayout.Width(width1));
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(t.TimeLeft), GUILayout.Width(width1));
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            /*else
            {
                //if (buildListWindowPosition.height > 32*3) buildListWindowPosition.height = 32*3;
                //buildListWindowPosition.height = 1;
            }*/
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                GUI.DragWindow();
            }
        }

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
                List<ProtoVessel> list = ShipConstruction.FindVesselsAtLaunchSite(HighLogic.CurrentGame.flightState, KCT_GameStates.launchedVessel.launchSite);
                foreach (ProtoVessel pv in list)
                    ShipConstruction.RecoverVesselFromFlight(pv, HighLogic.CurrentGame.flightState);
                if (!IsCrewable(KCT_GameStates.launchedVessel.ExtractedParts))
                    KCT_GameStates.launchedVessel.Launch();
                else
                {
                    showClearLaunch = false;
                    centralWindowPosition.height = 1;
                    partNames = KCT_GameStates.launchedVessel.GetPartNames();
                    parts = KCT_GameStates.launchedVessel.ExtractedParts;
                    if (KCT_GameStates.launchedCrew.Count != partNames.Count)
                    {
                        KCT_GameStates.launchedCrew = new List<List<ProtoCrewMember>>();
                        foreach (string s in partNames)
                            KCT_GameStates.launchedCrew.Add(new List<ProtoCrewMember>());
                    }
                    KCT_GameStates.launchedCrew[FirstCrewable(parts)] = HighLogic.CurrentGame.CrewRoster.DefaultCrewForVessel(KCT_GameStates.launchedVessel.shipNode, true).GetAllCrew(true);
                    showShipRoster = true;
                }
                //KCT_GameStates.launchedVessel.Launch();
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
        private static List<String> partNames;
        private static List<Part> parts;
        public static void DrawShipRoster(int windowID)
        {
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(Screen.height/2));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill All"))
            {
                //foreach (AvailablePart p in KCT_GameStates.launchedVessel.GetPartNames())
                for (int j = 0; j < partNames.Count; j++)
                {
                    Part p = parts[j];//KCT_Utilities.GetAvailablePartByName(KCT_GameStates.launchedVessel.GetPartNames()[j]).partPrefab;
                    if (p.CrewCapacity > 0)
                    {
                        //if (!KCT_GameStates.launchedCrew.Keys.Contains(p.uid))
                        //KCT_GameStates.launchedCrew.Add(new List<ProtoCrewMember>());
                        for (int i = 0; i < p.CrewCapacity; i++)
                        {
                            if (KCT_GameStates.launchedCrew[j].Count <= i)
                            {
                                ProtoCrewMember crewMember = CrewAvailable().First();
                                if (crewMember != null) KCT_GameStates.launchedCrew[j].Add(crewMember);
                            }
                            if (KCT_GameStates.launchedCrew[j][i] == null)
                            {
                                ProtoCrewMember crewMember = CrewAvailable().First();
                                if (crewMember != null) KCT_GameStates.launchedCrew[j][i] = crewMember;
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button("Clear All"))
            {
                KCT_GameStates.launchedCrew.Clear();
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
            for (int j = 0; j < partNames.Count; j++)
            {
                //Part p = KCT_Utilities.GetAvailablePartByName(KCT_GameStates.launchedVessel.GetPartNames()[j]).partPrefab;
                Part p = parts[j];
                if (p.CrewCapacity>0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p.partInfo.title);
                    if (GUILayout.Button("Fill", GUILayout.Width(75)))
                    {
                        //if (!KCT_GameStates.launchedCrew.Contains(p.uid))
                        KCT_GameStates.launchedCrew.Add(new List<ProtoCrewMember>());
                        for (int i=0; i<p.CrewCapacity; i++)
                        {
                            if (KCT_GameStates.launchedCrew[j].Count <= i)
                            {
                                ProtoCrewMember crewMember = CrewAvailable().First();
                                if (crewMember!=null) KCT_GameStates.launchedCrew[j].Add(crewMember);
                            }
                            else if (KCT_GameStates.launchedCrew[j][i] == null)
                            {
                                KCT_GameStates.launchedCrew[j][i] = CrewAvailable().First();
                            }
                        }
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(75)))
                    {
                        KCT_GameStates.launchedCrew[j].Clear();
                    }
                    GUILayout.EndHorizontal();
                    for (int i = 0; i < p.CrewCapacity; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (i < KCT_GameStates.launchedCrew[j].Count && KCT_GameStates.launchedCrew[j][i] != null)
                        {
                            GUILayout.Label(KCT_GameStates.launchedCrew[j][i].name);
                            if (GUILayout.Button("Remove", GUILayout.Width(120)))
                            {
                                KCT_GameStates.launchedCrew[j][i].rosterStatus = ProtoCrewMember.RosterStatus.AVAILABLE;
                                KCT_GameStates.launchedCrew[j].RemoveAt(i);
                            }
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Empty");
                            if (GUILayout.Button("Add", GUILayout.Width(120)))
                            {
                                showShipRoster = false;
                                showCrewSelect = true;
                                partIndexToCrew = j;
                                indexToCrew = i;
                                crewListWindowPosition.height = 1;
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
                KCT_GameStates.launchedVessel.Launch();
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

        private static List<ProtoCrewMember> CrewAvailable()
        {
            List<ProtoCrewMember> availableCrew = new List<ProtoCrewMember>();
            foreach (ProtoCrewMember crewMember in HighLogic.CurrentGame.CrewRoster) //Initialize available crew list
            {
                bool available = true;
                if (crewMember.rosterStatus == ProtoCrewMember.RosterStatus.AVAILABLE)
                {
                    foreach (List<ProtoCrewMember> list in KCT_GameStates.launchedCrew)
                    {
                        if (list.Contains(crewMember))
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
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(availableCrew.Count * 28 + 35), GUILayout.MaxHeight(Screen.height / 2));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Name:");
            foreach (ProtoCrewMember crew in availableCrew) 
            {
                GUILayout.Label(crew.name, GUILayout.Height(25));
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Courage:");
            foreach (ProtoCrewMember crew in availableCrew)
            {
                GUILayout.Label(crew.courage.ToString(), GUILayout.Height(25));
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Stupidity:");
            foreach (ProtoCrewMember crew in availableCrew)
            {
                GUILayout.Label(crew.stupidity.ToString(), GUILayout.Height(25));
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("");
            foreach (ProtoCrewMember crew in availableCrew)
            {
                if (GUILayout.Button("Select", GUILayout.Height(25)))
                {
                    List<ProtoCrewMember> activeCrew;
                    //if (KCT_GameStates.launchedCrew.ContainsKey(partToCrew.uid))
                    {
                        activeCrew = KCT_GameStates.launchedCrew[partIndexToCrew];
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
                        KCT_GameStates.launchedCrew[partIndexToCrew] = activeCrew;
                    }
                   /* else
                    {
                        activeCrew = new List<ProtoCrewMember>();
                        if (activeCrew.Count>indexToCrew)
                            activeCrew.Insert(indexToCrew, crew);
                        else
                        {
                            for (int i=activeCrew.Count; i<indexToCrew; i++)
                            {
                                activeCrew.Insert(i, null);
                            }
                            activeCrew.Insert(indexToCrew, crew);
                        }
                        KCT_GameStates.launchedCrew.Add(partToCrew.uid, activeCrew);
                    }*/
                    showCrewSelect = false;
                    showShipRoster = true;
                    crewListWindowPosition.height = 1;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            if (GUILayout.Button("Cancel"))
            {
                showCrewSelect = false;
                showShipRoster = true;
                crewListWindowPosition.height = 1;
            }
            GUILayout.EndVertical();
        }

        private static void DrawSettings(int windowID)
        {
            int width1 = 200;
            int width2 = 100;
            GUILayout.BeginVertical();
            GUILayout.Label("Game Settings");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled for this save?", GUILayout.Width(width1));
            enabledForSave = GUILayout.Toggle(enabledForSave, enabledForSave ? " Enabled" : " Disabled", GUILayout.Width(width2));
            GUILayout.EndHorizontal();
            if (HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Number of Upgrade Points", GUILayout.Width(width1));
                newUpgradeCount = GUILayout.TextField(newUpgradeCount, 3, GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Label("");
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
            GUILayout.Label("Simulation Time Limit", GUILayout.Width(width1));
            newTimeLimit = GUILayout.TextField(newTimeLimit, GUILayout.Width(width2));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Override Body Tracker?", GUILayout.Width(width1));
            enableAllBodies = GUILayout.Toggle(enableAllBodies, enableAllBodies ? " Overridden" : " Normal", GUILayout.Width(width2));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Force Stop Timewarp on Complete?", GUILayout.Width(width1));
            forceStopWarp = GUILayout.Toggle(forceStopWarp, forceStopWarp ? " Enabled" : " Disabled", GUILayout.Width(width2));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Upgrades for New Sandbox", GUILayout.Width(width1));
            newSandboxUpgrades = GUILayout.TextField(newSandboxUpgrades, 3, GUILayout.Width(40));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Instant Tech Unlock", GUILayout.Width(width1));
            instantTechUnlock = GUILayout.Toggle(instantTechUnlock, instantTechUnlock ? " Enabled" : " Disabled", GUILayout.Width(width2));
            GUILayout.EndHorizontal();

            GUILayout.Label("");
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

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                if (!enabledForSave && KCT_GameStates.settings.enabledForSave)
                    KCT_Utilities.DisableModFunctionality();
                KCT_GameStates.settings.enabledForSave = enabledForSave;
                KCT_GameStates.TotalUpgradePoints = int.Parse(newUpgradeCount);

                KCT_GameStates.settings.MaxTimeWarp = Math.Min(TimeWarp.fetch.warpRates.Count()-1, Math.Max(0, int.Parse(newTimeWarp)));
                KCT_GameStates.settings.SimulationTimeLimit = Math.Max(double.Parse(newTimeLimit), 0);
                KCT_GameStates.settings.EnableAllBodies = enableAllBodies;
                KCT_GameStates.settings.ForceStopWarp = forceStopWarp;
                KCT_GameStates.settings.InstantTechUnlock = instantTechUnlock;
                KCT_GameStates.settings.SandboxUpgrades = int.Parse(newSandboxUpgrades);
                KCT_GameStates.settings.Save();

                KCT_GameStates.timeSettings.OverallMultiplier = double.Parse(newMultiplier);
                KCT_GameStates.timeSettings.BuildEffect = double.Parse(newBuildEffect);
                KCT_GameStates.timeSettings.InventoryEffect = double.Parse(newInvEffect);
                KCT_GameStates.timeSettings.Save();
                showSettings = false;
                if (enabledForSave) showBuildList = true;
            }
            if (GUILayout.Button("Cancel"))
            {
                showSettings = false;
                if (enabledForSave) showBuildList = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                GUI.DragWindow();
            }
        }

        private static int upgradeWindowHolder = 0;
        private static void DrawUpgradeWindow(int windowID)
        {
            int spentPoints = KCT_Utilities.TotalSpentUpgrades();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Total Points: " + KCT_GameStates.TotalUpgradePoints);
            GUILayout.Label("Available: " + (KCT_GameStates.TotalUpgradePoints - spentPoints));
            GUILayout.EndHorizontal();

            if (KCT_Utilities.CurrentGameIsCareer())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Buy Upgrade:\n250 Science");
                if (GUILayout.Button("Purchase", GUILayout.ExpandWidth(false)))
                {
                    if (ResearchAndDevelopment.Instance.Science >= 250.0F)
                    {
                        ResearchAndDevelopment.Instance.Science -= 250.0F;
                        ++KCT_GameStates.TotalUpgradePoints;
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VAB")) { upgradeWindowHolder = 0; upgradePosition.height = 1; }
            if (GUILayout.Button("SPH")) { upgradeWindowHolder = 1; upgradePosition.height = 1; }
            if (KCT_Utilities.CurrentGameIsCareer() && GUILayout.Button("R&D")) { upgradeWindowHolder = 2; upgradePosition.height = 1; }
            GUILayout.EndHorizontal();

            if (upgradeWindowHolder==0) //VAB
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("VAB Upgrades");
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height((KCT_GameStates.VABUpgrades.Count + 1) * 26), GUILayout.MaxHeight(3 * Screen.height / 4));
                GUILayout.BeginVertical();
                for (int i = 0; i < KCT_GameStates.VABUpgrades.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Rate "+(i+1));
                    GUILayout.Label(KCT_Utilities.GetBuildRate(i, KCT_BuildListVessel.ListType.VAB)+" BP/s");
                    if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0 && (i == 0 || KCT_Utilities.GetBuildRate(i, KCT_BuildListVessel.ListType.VAB)+((i+1)*0.05) 
                        <= KCT_Utilities.GetBuildRate(i-1, KCT_BuildListVessel.ListType.VAB)))
                    {
                        if (GUILayout.Button("+" + ((i + 1) * 0.05), GUILayout.Width(45)))
                        {
                            ++KCT_GameStates.VABUpgrades[i];
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Rate " + (KCT_GameStates.VABUpgrades.Count+1));
                GUILayout.Label("0 BP/s");
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0 && ((KCT_GameStates.VABUpgrades.Count + 1) * 0.05) 
                    <= KCT_Utilities.GetBuildRate(KCT_GameStates.VABUpgrades.Count - 1, KCT_BuildListVessel.ListType.VAB))
                {
                    if (GUILayout.Button("+" + ((KCT_GameStates.VABUpgrades.Count + 1) * 0.05), GUILayout.Width(45)))
                    {
                        KCT_GameStates.VABUpgrades.Add(1);
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
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height((KCT_GameStates.SPHUpgrades.Count + 1) * 26), GUILayout.MaxHeight(3 * Screen.height / 4));
                GUILayout.BeginVertical();
                for (int i = 0; i < KCT_GameStates.SPHUpgrades.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Rate " + (i + 1));
                    GUILayout.Label(KCT_Utilities.GetBuildRate(i, KCT_BuildListVessel.ListType.SPH) + " BP/s");
                    if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0 && (i == 0 || KCT_Utilities.GetBuildRate(i, KCT_BuildListVessel.ListType.SPH) + ((i + 1) * 0.05)
                        <= KCT_Utilities.GetBuildRate(i - 1, KCT_BuildListVessel.ListType.SPH)))
                    {
                        if (GUILayout.Button("+" + ((i + 1) * 0.05), GUILayout.Width(45)))
                        {
                            ++KCT_GameStates.SPHUpgrades[i];
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Rate " + (KCT_GameStates.SPHUpgrades.Count+1));
                GUILayout.Label("0 BP/s");
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0 && ((KCT_GameStates.SPHUpgrades.Count + 1) * 0.05)
                    <= KCT_Utilities.GetBuildRate(KCT_GameStates.SPHUpgrades.Count - 1, KCT_BuildListVessel.ListType.SPH))
                {
                    if (GUILayout.Button("+" + ((KCT_GameStates.SPHUpgrades.Count + 1) * 0.05), GUILayout.Width(45)))
                    {
                        KCT_GameStates.SPHUpgrades.Add(1);
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
                GUILayout.Label((KCT_GameStates.RDUpgrades[0]*0.5) + " sci/86400 BP");
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0)
                {
                    if (GUILayout.Button("+0.5", GUILayout.Width(45)))
                    {
                        ++KCT_GameStates.RDUpgrades[0];
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Development");
                GUILayout.Label("1 day/"+Math.Pow(2, KCT_GameStates.RDUpgrades[1]+1)+" sci");
                if (KCT_GameStates.TotalUpgradePoints - spentPoints > 0)
                {
                    if (GUILayout.Button("1d/" + Math.Pow(2, KCT_GameStates.RDUpgrades[1]+2), GUILayout.ExpandWidth(false)))
                    {
                        ++KCT_GameStates.RDUpgrades[1];
                    }
                }
                GUILayout.EndHorizontal();

            }
            if (GUILayout.Button("Close")) { showUpgradeWindow = false; showBuildList = true; }
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                GUI.DragWindow();
            }
        }

        private static int IndexSelected=0;
        private static void DrawBLPlusWindow(int windowID)
        {
            bLPlusPosition.xMin = buildListWindowPosition.xMax;
            bLPlusPosition.width = 100;
            bLPlusPosition.yMin = buildListWindowPosition.yMin;
            bLPlusPosition.height = 175;
            //bLPlusPosition.height = bLPlusPosition.yMax - bLPlusPosition.yMin;
            List<KCT_BuildListVessel> buildList = new List<KCT_BuildListVessel>();
            switch (listWindow)
            {
                case 1: buildList = KCT_GameStates.VABList; break;
                case 2: buildList = KCT_GameStates.SPHList; break;
                case 3: buildList = KCT_GameStates.VABWarehouse; break;
                case 4: buildList = KCT_GameStates.SPHWarehouse; break;
                default: showBLPlus = false; break;
            }
            KCT_BuildListVessel b = buildList[IndexSelected];
            GUILayout.BeginVertical();
            if (GUILayout.Button("Scrap"))
            {
                if (listWindow < 3)
                {
                    if (b.InventoryParts != null)
                        foreach (String s in b.InventoryParts)
                            KCT_Utilities.AddPartToInventory(s);
                    buildList.RemoveAt(IndexSelected);
                }
                else
                {
                    foreach (string p in b.GetPartNames())
                        KCT_Utilities.AddPartToInventory(p);
                    buildList.RemoveAt(IndexSelected);
                }
                showBLPlus = false;
                ResetBLWindow();
            }
            if (GUILayout.Button("View (NF)"))
            {
                showBLPlus = false;
                if (b.type == KCT_BuildListVessel.ListType.VAB)
                {
                    HighLogic.LoadScene(GameScenes.EDITOR);
                    //EditorLogic.fetch.ship = b.getShip();
                }
                else if (b.type == KCT_BuildListVessel.ListType.SPH)
                {
                    HighLogic.LoadScene(GameScenes.SPH);
                    //EditorLogic.fetch.ship = b.getShip();
                }
            }
            if (GUILayout.Button("Rename"))
            {
                centralWindowPosition.width = Screen.width / 8;
                centralWindowPosition.xMin = (Screen.width - centralWindowPosition.width) / 2;
                centralWindowPosition.height = 1;
                showBLPlus = false;
                showRename = true;
                newName = b.shipName;
                //newDesc = b.getShip().shipDescription;
            }
            if (GUILayout.Button("Duplicate"))
            {
                KCT_Utilities.AddVesselToBuildList(b.NewCopy(true));
            }
            if (GUILayout.Button("Close"))
            {
                showBLPlus = false;
            }
            GUILayout.EndVertical();
        }

        private static string newName = "";
        public static void DrawRenameWindow(int windowID)
        {
            if (centralWindowPosition.yMin != (Screen.height - centralWindowPosition.height) / 2)
            {
                centralWindowPosition.yMin = (Screen.height - centralWindowPosition.height) / 2;
                centralWindowPosition.height = 1;
            }
            GUILayout.BeginVertical();
            GUILayout.Label("Name:");
            newName = GUILayout.TextField(newName);
            //GUILayout.Label("Description:");
            //newDesc = GUILayout.TextArea(newDesc);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                List<KCT_BuildListVessel> buildList = new List<KCT_BuildListVessel>();
                switch (listWindow)
                {
                    case 1: buildList = KCT_GameStates.VABList; break;
                    case 2: buildList = KCT_GameStates.SPHList; break;
                    case 3: buildList = KCT_GameStates.VABWarehouse; break;
                    case 4: buildList = KCT_GameStates.SPHWarehouse; break;
                    default: showBLPlus = false; break;
                }
                KCT_BuildListVessel b = buildList[IndexSelected];
                b.shipName = newName; //Change the name from our point of view
               // b.GetShip().shipName = newName; //Change the name in the actual ship
               // b.shipNode = b.GetShip().SaveShip(); //Save the change to the ship config node
                //b.getShip().shipDescription = newDesc;
                showRename = false;
                centralWindowPosition.width = 150;
                centralWindowPosition.xMin = (Screen.width - 150) / 2;
            }
            if (GUILayout.Button("Cancel"))
            {
                centralWindowPosition.width = 150;
                centralWindowPosition.xMin = (Screen.width - 150) / 2;
                showRename = false;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
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