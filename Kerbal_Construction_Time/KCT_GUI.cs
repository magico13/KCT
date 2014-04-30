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
            showSimulationCompleteFlight, showBuildList, showClearLaunch, showShipRoster, showCrewSelect, showSettings, showSimConfig, showBodyChooser;

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
        private static Rect buildListWindowPosition = new Rect(Screen.width / 3.5f, Screen.height / 3.5f, Screen.width/4, 64);
        private static Rect crewListWindowPosition = new Rect((3*Screen.width/8), (Screen.height / 4), Screen.width / 4, 1);
        private static Rect settingsPosition = new Rect((3 * Screen.width / 8), (Screen.height / 4), Screen.width / 4, 1);
        private static Rect simulationConfigPosition = new Rect((Screen.width / 2)-120, (Screen.height / 4), 240, 1);
        private static GUIStyle windowStyle = new GUIStyle(HighLogic.Skin.window);

        private static List<GameScenes> validScenes = new List<GameScenes> { GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.SPH, GameScenes.SPACECENTER, GameScenes.TRACKSTATION };
        public static void SetGUIPositions(GUI.WindowFunction OnWindow)
        {
            if (validScenes.Contains(HighLogic.LoadedScene)) //&& KCT_GameStates.settings.enabledForSave)//!(HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX && !KCT_GameStates.settings.SandboxEnabled))
            {
                if (!ToolbarManager.ToolbarAvailable && KCT_GameStates.settings.enabledForSave && GUI.Button(iconPosition, "KCT", GUI.skin.button))
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
                showBuildList = !showBuildList;
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
                showBuildList = !showBuildList;
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
            if (showTimeRemaining)
                DrawSimulationTimeWindow(windowID);
            if (showBuildList)
                DrawBuildListWindow(windowID);
            if (showClearLaunch)
                DrawClearLaunch(windowID);
            if (showShipRoster)
                DrawShipRoster(windowID);
            if (showCrewSelect)
                DrawCrewSelect(windowID);
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
            //GUILayout.Label("#Parts", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("Build Time (s)", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("Build Time Remaining: ", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("UT: ", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Warp until ready.", GUILayout.ExpandWidth(true)))
            {
            //    if (FlightGlobals.ActiveVessel.id != KCT_GameStates.activeVessel.vessel.id)
                {
            //        FlightGlobals.SetActiveVessel(KCT_GameStates.activeVessel.vessel);
                }
                KCT_GameStates.canWarp = true;

            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label(KCT_GameStates.activeVessel.vessel.Parts.Count.ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
           // GUILayout.Label(KCT_GameStates.activeVessel.buildTime.ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            //GUILayout.Label(KCT_Utilities.GetFormatedTime(KCT_GameStates.activeVessel.finishDate - KCT_GameStates.UT), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
           // GUILayout.Label(KCT_Utilities.GetFormattedTime(KCT_GameStates.activeVessel.buildTime - KCT_GameStates.activeVessel.progress), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label(KCT_Utilities.GetFormattedTime(KCT_GameStates.UT).ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Stop warp", GUILayout.ExpandWidth(true)))
            {
                KCT_GameStates.canWarp = false;
                TimeWarp.SetRate(0, true);

            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private static bool showInventory = false;
        private static string currentCategoryString = "NONE";
        private static void DrawEditorGUI(int windowID)
        {
            double buildTime = KCT_Utilities.GetBuildTime(EditorLogic.fetch.ship.Parts);
            GUILayout.BeginVertical();
            GUILayout.Label("Estimated Build Time:", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label(KCT_Utilities.GetFormattedTime(buildTime), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Launch!", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                showLaunchAlert = true;
            }
            if (GUILayout.Button("Part Inventory", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
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
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(Math.Min((activeList.Count + 1) * 30, Screen.height / 1.4F))); //
                GUILayout.BeginHorizontal();
                /*GUILayout.Label("Name:", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUILayout.Label("Available:", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUILayout.Label("In use:", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
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
                        GUILayout.Label(name, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        GUILayout.Label(KCT_GameStates.PartInventory.Values.ElementAt(i).ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        int inUse = 0;
                        String pName = KCT_GameStates.PartInventory.Keys.ElementAt(i);
                        for (int j = 0; j < EditorLogic.fetch.ship.parts.Count; j++)
                        {
                            if (EditorLogic.fetch.ship.parts[j].partInfo.name == pName)
                                ++inUse;
                        }
                        GUILayout.Label(inUse.ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        GUILayout.EndHorizontal();
                    }
                }*/

                   GUILayout.BeginVertical();
                   GUILayout.Label("Name:", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                   for (int i=0; i<activeList.Count; i++)
                   {
                        string name = activeList.Keys.ElementAt(i);
                        GUILayout.Label(name, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));   
                   }
                   GUILayout.EndVertical();
                
                   GUILayout.BeginVertical();
                   GUILayout.Label("Available:", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                   for (int i = 0; i < activeList.Count; i++)
                   {
                       GUILayout.Label(activeList.Values.ElementAt(i).ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                   }
                   GUILayout.EndVertical();

                   GUILayout.BeginVertical();
                   GUILayout.Label("In use:", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                   for (int i = 0; i < activeList.Count; i++)
                   {
                       int inUse = 0;
                       String name = activeList.Keys.ElementAt(i);
                       for (int j = 0; j < EditorLogic.fetch.ship.parts.Count; j++)
                       {
                           if (EditorLogic.fetch.ship.parts[j].partInfo.title == name)
                               ++inUse;
                       }
                       GUILayout.Label(inUse.ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
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
            GUI.DragWindow();

        }

        public static void DrawSOIAlertWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("   Warp stopped due to SOI change.", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("Vessel name: " + KCT_GameStates.lastSOIVessel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (GUILayout.Button("Close", GUILayout.ExpandWidth(true)))
            {
                showSOIAlert = false;
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private static string orbitAltString="";
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

                if (KCT_GameStates.simulateInOrbit)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Orbit Altitude: ");
                    orbitAltString=GUILayout.TextField(orbitAltString);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Min: " + KCT_GameStates.simulationBody.maxAtmosphereAltitude);
                    GUILayout.Label("Max: " + Math.Floor(KCT_GameStates.simulationBody.sphereOfInfluence));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Orbit Altitude: ");
                orbitAltString=GUILayout.TextField(orbitAltString);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Min: " + KCT_GameStates.simulationBody.maxAtmosphereAltitude);
                GUILayout.Label("Max: " + KCT_GameStates.simulationBody.sphereOfInfluence);
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Simulate", GUILayout.ExpandWidth(true)))
            {
                if (KCT_GameStates.simulationBody.bodyName != "Kerbin")
                    KCT_GameStates.simulateInOrbit = true;

                if (KCT_GameStates.simulateInOrbit)
                {
                    if (!double.TryParse(orbitAltString, out KCT_GameStates.simOrbitAltitude))
                        KCT_GameStates.simOrbitAltitude = KCT_GameStates.simulationBody.maxAtmosphereAltitude + 1000;
                    else
                        KCT_GameStates.simOrbitAltitude = Math.Min(Math.Max(KCT_GameStates.simOrbitAltitude, KCT_GameStates.simulationBody.maxAtmosphereAltitude), KCT_GameStates.simulationBody.sphereOfInfluence);
                }
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                EditorLogic.fetch.launchVessel();
                showSimConfig = false;
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(true)))
            {
                showSimConfig = false;
                centralWindowPosition.height = 1;
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
            if (GUILayout.Button("Add to Build List", GUILayout.ExpandWidth(true)))
            {
                KCT_GameStates.flightSimulated = false;
                KCT_Utilities.disableSimulationLocks();
                KCT_Utilities.AddVesselToBuildList();
                showLaunchAlert = false;
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Simulate Launch", GUILayout.ExpandWidth(true)))
            {
                /*KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                EditorLogic.fetch.launchVessel();*/
                showLaunchAlert = false;
                showSimConfig = true;
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(true)))
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
                GUILayout.Label("Vessel destroyed", GUILayout.ExpandWidth(true));
            else if (reason=="APOAPSIS")
                GUILayout.Label("Apoapsis exceeded 250km", GUILayout.ExpandWidth(true));
            else if (reason=="PERIAPSIS")
                GUILayout.Label("Stable orbit reached", GUILayout.ExpandWidth(true));
            else if (reason=="USER")
                GUILayout.Label("The user ended the simulation", GUILayout.ExpandWidth(true));
            else if (reason == "TIME")
                GUILayout.Label("Time is up", GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Add to Build List", GUILayout.ExpandWidth(true)))
            {
                KCT_GameStates.flightSimulated = false;
                KCT_Utilities.disableSimulationLocks();
                KCT_Utilities.AddVesselToBuildList();
                showSimulationCompleteEditor = false;
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Restart Simulation", GUILayout.ExpandWidth(true)))
            {
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                EditorLogic.fetch.launchVessel();
                centralWindowPosition.height = 1;
            }
            if (GUILayout.Button("Close", GUILayout.ExpandWidth(true)))
            {
                showSimulationCompleteEditor = false;
                centralWindowPosition.height = 1;
            }
            GUILayout.EndVertical();
        }

        public static void DrawSimulationCompleteFlight(int windowID)
        {
            GUILayout.BeginVertical();
           /* if (GUILayout.Button("Build", GUILayout.ExpandWidth(true))) //Doesn't work until I can add it to the build list no problem
            {
                KCT_GameStates.flightSimulated = false;
                KCT_Utilities.disableSimulationLocks();
                //FlightDriver.RevertToLaunch();
                if (FlightDriver.LaunchSiteName == "LaunchPad")
                    FlightDriver.RevertToPrelaunch(GameScenes.EDITOR);
                else
                    FlightDriver.RevertToPrelaunch(GameScenes.SPH);
                
            }*/

            if (GUILayout.Button("Restart Simulation", GUILayout.ExpandWidth(true)))
            {
                KCT_GameStates.flightSimulated = true;
                KCT_Utilities.enableSimulationLocks();
                KCT_GameStates.simulationEndTime = 0;
                if (MCEWrapper.MCEAvailable) //Support for MCE
                    MCEWrapper.IloadMCEbackup();
                FlightDriver.RevertToLaunch();
                centralWindowPosition.height = 1;
            }

            if (GUILayout.Button("Revert to Editor", GUILayout.ExpandWidth(true)))
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
                GUILayout.Label(KCT_Utilities.GetColonFormattedTime(time), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label("Pre-launch", GUILayout.ExpandWidth(true));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public static void DrawSimulationWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("This is a simulation. It will end when one of the following conditions are met:", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            GUILayout.Label("The time limit is exceeded", GUILayout.ExpandWidth(true));
            GUILayout.Label("The flight scene is exited", GUILayout.ExpandWidth(true));
            GUILayout.Label(" ", GUILayout.ExpandWidth(true));
            GUILayout.Label("You cannot save or switch vessels during a simulation.", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("End Simulation", GUILayout.ExpandWidth(true)))
            {
                showSimulationCompleteFlight = true;
                showSimulationWindow = false;
            }
            if (GUILayout.Button("Close", GUILayout.ExpandWidth(true)))
            {
                showSimulationWindow = !showSimulationWindow;
            }
            GUILayout.EndVertical();
        }

        private static void ShowSettings()
        {
            newMultiplier = KCT_GameStates.timeSettings.OverallMultiplier.ToString();
            newBuildEffect = KCT_GameStates.timeSettings.BuildEffect.ToString();
            newInvEffect = KCT_GameStates.timeSettings.InventoryEffect.ToString();
            enabledForSave = KCT_GameStates.settings.enabledForSave;
            enableAllBodies = KCT_GameStates.settings.EnableAllBodies;
            newTimeWarp = KCT_GameStates.settings.MaxTimeWarp.ToString();
            showSettings = !showSettings;
        }

        private static int listWindow = 0;
        public static void DrawBuildListWindow(int windowID)
        {
            GUILayout.BeginVertical();
            //List next vessel to finish
            GUILayout.BeginHorizontal();
            GUILayout.Label("Next vessel:", GUILayout.ExpandWidth(true));
            KCT_BuildListVessel ship = KCT_Utilities.NextShipToFinish();
            if (ship != null)
            {
                GUILayout.Label(ship.shipName, GUILayout.ExpandWidth(true));
                if (ship.type == KCT_BuildListVessel.ListType.VAB)
                    GUILayout.Label("VAB", GUILayout.ExpandWidth(true));
                else
                    GUILayout.Label("SPH", GUILayout.ExpandWidth(true));
                GUILayout.Label(KCT_Utilities.GetColonFormattedTime(ship.buildTime - ship.progress), GUILayout.ExpandWidth(true));
                //Allow warp when a vessel is loaded, but not time jump
                if (TimeWarp.CurrentRateIndex == 0 && GUILayout.Button("Warp to" + System.Environment.NewLine + "Complete", GUILayout.ExpandWidth(true)))
                {
                    KCT_GameStates.canWarp = true;
                    //Should ramp up time instead
                    //TimeWarp.SetRate(7, true);
                    KCT_Utilities.RampUpWarp();
                    KCT_GameStates.warpInitiated = true;
                }
                else if (TimeWarp.CurrentRateIndex > 0 && GUILayout.Button("Stop" + System.Environment.NewLine + "Warp", GUILayout.ExpandWidth(true)))
                {
                    KCT_GameStates.canWarp = false;
                    TimeWarp.SetRate(0, true);
                    KCT_GameStates.lastWarpRate = 0;
                }
            }
            else
            {
                GUILayout.Label("No Building Vessels", GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();

            //Buttons for VAB/SPH lists
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VAB List", GUILayout.ExpandWidth(true)))
            {
                if (listWindow == 1)
                    listWindow = 0;
                else
                    listWindow = 1;
                buildListWindowPosition.height = 32 * KCT_GameStates.VABList.Count+3*32;
            }
            if (GUILayout.Button("SPH List", GUILayout.ExpandWidth(true)))
            {
                if (listWindow == 2)
                    listWindow = 0;
                else
                    listWindow = 2;
                buildListWindowPosition.height = 32 * KCT_GameStates.SPHList.Count + 3 * 32;
            }
            if (GUILayout.Button("VAB Storage", GUILayout.ExpandWidth(true)))
            {
                if (listWindow == 3)
                    listWindow = 0;
                else
                    listWindow = 3;
                buildListWindowPosition.height = 32 * KCT_GameStates.VABWarehouse.Count + 3 * 32;
            }
            if (GUILayout.Button("SPH Storage", GUILayout.ExpandWidth(true)))
            {
                if (listWindow == 4)
                    listWindow = 0;
                else
                    listWindow = 4;
                buildListWindowPosition.height = 32 * KCT_GameStates.SPHWarehouse.Count + 3 * 32;
            }
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && GUILayout.Button("Settings"))
            {
                ShowSettings();
            }
            GUILayout.EndHorizontal();
            //Content of lists
            if (listWindow==1) //VAB Build List
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.VABList;
                GUILayout.Label("VAB Build List", GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:", GUILayout.ExpandWidth(true));
                GUILayout.Label("Progress:", GUILayout.ExpandWidth(true));
                GUILayout.Label("Time Left:", GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(Math.Min((buildList.Count) * 27, Screen.height / 1.4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName, GUILayout.ExpandWidth(true));
                    GUILayout.Label(Math.Round(b.ProgressPercent(), 2).ToString() + "%", GUILayout.ExpandWidth(true));
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(b.buildTime - b.progress), GUILayout.ExpandWidth(true));
                    if (i > 0 && GUILayout.Button("^", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i - 1, b);
                    }
                    if (GUILayout.Button("TOP", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(0, b);
                    }
                    if (i < buildList.Count - 1 && GUILayout.Button("v", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i + 1, b);
                    }
                    if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                    {
                        if (b.InventoryParts != null)
                            foreach (String s in b.InventoryParts)
                                KCT_Utilities.AddPartToInventory(s);
                        buildList.RemoveAt(i);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow==2) //SPH Build List
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.SPHList;
                GUILayout.Label("SPH Build List", GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:", GUILayout.ExpandWidth(true));
                GUILayout.Label("Progress:", GUILayout.ExpandWidth(true));
                GUILayout.Label("Time Left:", GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(Math.Min((buildList.Count) * 27, Screen.height / 1.4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName, GUILayout.ExpandWidth(true));
                    GUILayout.Label(Math.Round(b.ProgressPercent(), 2).ToString() + "%", GUILayout.ExpandWidth(true));
                    GUILayout.Label(KCT_Utilities.GetColonFormattedTime(b.buildTime - b.progress), GUILayout.ExpandWidth(true));
                    if (i > 0 && GUILayout.Button("^", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i - 1, b);
                    }
                    if (GUILayout.Button("TOP", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(0, b);
                    }
                    if (i < buildList.Count - 1 && GUILayout.Button("v", GUILayout.ExpandWidth(false)))
                    {
                        buildList.RemoveAt(i);
                        buildList.Insert(i + 1, b);
                    }
                    if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                    {
                        if (b.InventoryParts != null)
                            foreach (String s in b.InventoryParts)
                                KCT_Utilities.AddPartToInventory(s);
                        buildList.RemoveAt(i);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow==3) //VAB Warehouse
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.VABWarehouse;
                GUILayout.Label("VAB Storage", GUILayout.ExpandWidth(true));
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(Math.Min((buildList.Count) * 27 + 10, Screen.height / 1.4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Scrap", GUILayout.ExpandWidth(false)))
                    {
                        foreach (Part p in b.getShip().parts)
                        {
                            KCT_Utilities.AddPartToInventory(p);
                        }
                        buildList.RemoveAt(i);
                    }
                    if (GUILayout.Button("Launch", GUILayout.ExpandWidth(false)))
                    {
                        KCT_GameStates.launchedVessel = b;
                        if (ShipAssembly.CheckLaunchSiteClear(HighLogic.CurrentGame.flightState, "LaunchPad", false))
                        {
                           // buildList.RemoveAt(i);
                            if (!IsCrewable(b.getShip().parts))
                                b.Launch();
                            else
                            {
                                showBuildList = false;
                                centralWindowPosition.height = 1;
                                KCT_GameStates.launchedCrew.Clear();
                                KCT_GameStates.launchedCrew.Add(FirstCrewable(b.getShip().parts).uid, HighLogic.CurrentGame.CrewRoster.DefaultCrewForVessel(b.shipNode, true).GetAllCrew(true));
                                showShipRoster = true;
                            }
                        }
                        else
                        {
                            showBuildList = false;
                            showClearLaunch = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else if (listWindow==4) //SPH Warehouse
            {
                List<KCT_BuildListVessel> buildList = KCT_GameStates.SPHWarehouse;
                GUILayout.Label("SPH Storage", GUILayout.ExpandWidth(true));
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(Math.Min((buildList.Count) * 27 + 10, Screen.height / 1.4F)));
                for (int i = 0; i < buildList.Count; i++)
                {
                    KCT_BuildListVessel b = buildList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(b.shipName, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Scrap", GUILayout.ExpandWidth(false)))
                    {
                        foreach (Part p in b.getShip().parts)
                        {
                            KCT_Utilities.AddPartToInventory(p);
                        }
                        buildList.RemoveAt(i);
                    }
                    if (GUILayout.Button("Launch", GUILayout.ExpandWidth(false)))
                    {
                        KCT_GameStates.launchedVessel = b;
                        if (ShipAssembly.CheckLaunchSiteClear(HighLogic.CurrentGame.flightState, "Runway", false))
                        {
                            //buildList.RemoveAt(i);
                            if (!IsCrewable(b.getShip().parts))//For a ship that hasn't been "loaded", this returns true!?
                                b.Launch();
                            else
                            {
                                showBuildList = false;
                                centralWindowPosition.height = 1;
                                KCT_GameStates.launchedCrew.Clear();
                                KCT_GameStates.launchedCrew.Add(FirstCrewable(b.getShip().parts).uid, HighLogic.CurrentGame.CrewRoster.DefaultCrewForVessel(b.shipNode, true).GetAllCrew(true));
                                showShipRoster = true;
                            }
                        }
                        else
                        {
                            showBuildList = false;
                            showClearLaunch = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else
            {
                if (buildListWindowPosition.height > 32*3) buildListWindowPosition.height = 32*3;
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private static bool IsCrewable(List<Part> ship)
        {
            foreach (Part p in ship)
                if (p.CrewCapacity > 0) return true;
            return false;
        }

        private static Part FirstCrewable(List<Part> ship)
        {
            foreach (Part p in ship)
            {
                //Debug.Log(p.partInfo.name+":"+p.CrewCapacity);
                if (p.CrewCapacity > 0) return p;
            }
            return null;
        }

        private static bool CanJumpTime() 
        {//So, I can't get SOI changes ahead of time except for the active vessel, but I can check if the vessels are in certain situations.
            //For example, if the vessel is landed/prelaunch/splashed down then its safe to jump time
            //If there are no vessels, then its also safe.
            //If the periapsis and apoapsis are within 10% of the apoapsis, then the orbit is pretty circular, and thus (probably) no SOI changes
            //If the vessel is marked as debris then we don't really care about it
            if (FlightGlobals.Vessels == null || FlightGlobals.Vessels.Count == 0)
            {
                return true;
            }
            else
            {
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (KCT_GameStates.VesselTypesForSOI.Contains(v.vesselType))
                    {
                        if (v.situation == Vessel.Situations.ESCAPING || v.situation == Vessel.Situations.FLYING)
                        {
                            return false;
                        }
                        else if (v.situation == Vessel.Situations.ORBITING || v.situation == Vessel.Situations.SUB_ORBITAL)
                        {
                            if (v.orbit != null && (v.orbit.ApA - v.orbit.PeA) <= (v.orbit.ApA*0.1))
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                        continue;
                }
            }
            return true;
        }

        public static void DrawClearLaunch(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Recover Flight and Proceed", GUILayout.ExpandWidth(true)))
            {
                List<ProtoVessel> list = ShipConstruction.FindVesselsAtLaunchSite(HighLogic.CurrentGame.flightState, KCT_GameStates.launchedVessel.launchSite);
                foreach (ProtoVessel pv in list)
                    ShipConstruction.RecoverVesselFromFlight(pv, HighLogic.CurrentGame.flightState);
                if (!IsCrewable(KCT_GameStates.launchedVessel.getShip().parts))
                    KCT_GameStates.launchedVessel.Launch();
                else
                {
                    showClearLaunch = false;
                    centralWindowPosition.height = 1;
                    showShipRoster = true;
                }
                //KCT_GameStates.launchedVessel.Launch();
                centralWindowPosition.height = 1;
            }

            if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(true)))
            {
                showClearLaunch = false;
                centralWindowPosition.height = 1;
            }
            GUILayout.EndVertical();
        }


        private static Part partToCrew;
        private static int indexToCrew;
        public static void DrawShipRoster(int windowID)
        {
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(Screen.height/2));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill All"))
            {
                foreach (Part p in KCT_GameStates.launchedVessel.getShip().parts)
                {
                    if (p.CrewCapacity > 0)
                    {
                        if (!KCT_GameStates.launchedCrew.Keys.Contains(p.uid))
                            KCT_GameStates.launchedCrew.Add(p.uid, new List<ProtoCrewMember>());
                        for (int i = 0; i < p.CrewCapacity; i++)
                        {
                            if (KCT_GameStates.launchedCrew[p.uid].Count <= i)
                            {
                                ProtoCrewMember crewMember = CrewAvailable().First();
                                if (crewMember != null) KCT_GameStates.launchedCrew[p.uid].Add(crewMember);
                            }
                            else if (KCT_GameStates.launchedCrew[p.uid][i] == null)
                            {
                                ProtoCrewMember crewMember = CrewAvailable().First();
                                if (crewMember != null) KCT_GameStates.launchedCrew[p.uid][i] = crewMember;
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
            foreach (Part p in KCT_GameStates.launchedVessel.getShip().Parts)
            {
                if (p.CrewCapacity>0)
                {
                    numberItems += 1 + p.CrewCapacity;
                }
            }
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(numberItems * 25 + 10), GUILayout.MaxHeight(Screen.height / 2));
            foreach (Part p in KCT_GameStates.launchedVessel.getShip().parts)
            {
                if (p.CrewCapacity>0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p.partInfo.title, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Fill", GUILayout.Width(75)))
                    {
                        if (!KCT_GameStates.launchedCrew.Keys.Contains(p.uid))
                            KCT_GameStates.launchedCrew.Add(p.uid, new List<ProtoCrewMember>());
                        for (int i=0; i<p.CrewCapacity; i++)
                        {
                            if (KCT_GameStates.launchedCrew[p.uid].Count <= i)
                            {
                                ProtoCrewMember crewMember = CrewAvailable().First();
                                if (crewMember!=null) KCT_GameStates.launchedCrew[p.uid].Add(crewMember);
                            }
                            else if (KCT_GameStates.launchedCrew[p.uid][i] == null)
                            {
                                KCT_GameStates.launchedCrew[p.uid][i] = CrewAvailable().First();
                            }
                        }
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(75)))
                    {
                        KCT_GameStates.launchedCrew[p.uid].Clear();
                    }
                    GUILayout.EndHorizontal();
                    for (int i = 0; i < p.CrewCapacity; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (KCT_GameStates.launchedCrew.ContainsKey(p.uid) && i < KCT_GameStates.launchedCrew[p.uid].Count && KCT_GameStates.launchedCrew[p.uid][i] != null)
                        {
                            GUILayout.Label(KCT_GameStates.launchedCrew[p.uid][i].name, GUILayout.ExpandWidth(true));
                            if (GUILayout.Button("Remove", GUILayout.Width(120)))
                            {
                                KCT_GameStates.launchedCrew[p.uid][i].rosterStatus = ProtoCrewMember.RosterStatus.AVAILABLE;
                                KCT_GameStates.launchedCrew[p.uid].RemoveAt(i);
                            }
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Empty", GUILayout.ExpandWidth(true));
                            if (GUILayout.Button("Add", GUILayout.Width(120)))
                            {
                                showShipRoster = false;
                                showCrewSelect = true;
                                partToCrew = p;
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
                    foreach (List<ProtoCrewMember> list in KCT_GameStates.launchedCrew.Values)
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
            GUILayout.Label("Name:", GUILayout.ExpandWidth(true));
            foreach (ProtoCrewMember crew in availableCrew) 
            {
                GUILayout.Label(crew.name, GUILayout.ExpandWidth(true), GUILayout.Height(25));
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Courage:", GUILayout.ExpandWidth(true));
            foreach (ProtoCrewMember crew in availableCrew)
            {
                GUILayout.Label(crew.courage.ToString(), GUILayout.ExpandWidth(true), GUILayout.Height(25));
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Stupidity:", GUILayout.ExpandWidth(true));
            foreach (ProtoCrewMember crew in availableCrew)
            {
                GUILayout.Label(crew.stupidity.ToString(), GUILayout.ExpandWidth(true), GUILayout.Height(25));
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("", GUILayout.ExpandWidth(true));
            foreach (ProtoCrewMember crew in availableCrew)
            {
                if (GUILayout.Button("Select", GUILayout.ExpandWidth(true), GUILayout.Height(25)))
                {
                    List<ProtoCrewMember> activeCrew;
                    if (KCT_GameStates.launchedCrew.ContainsKey(partToCrew.uid))
                    {
                        activeCrew = KCT_GameStates.launchedCrew[partToCrew.uid];
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
                        KCT_GameStates.launchedCrew[partToCrew.uid]=activeCrew;
                    }
                    else
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
                    }
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


        public static string newMultiplier, newBuildEffect, newInvEffect, newTimeWarp;
        public static bool enabledForSave, enableAllBodies;
        private static void DrawSettings(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Game Settings");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled for this save?");
            enabledForSave = GUILayout.Toggle(enabledForSave, enabledForSave ? " Enabled":" Disabled");
            GUILayout.EndHorizontal();
            //GUILayout.Label("General Settings");
            GUILayout.Label("");
            GUILayout.Label("Global Settings");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Max TimeWarp");
            //string newMultiplier = KCT_GameStates.timeSettings.OverallMultiplier.ToString();
            int warpIndex = 0;
            int.TryParse(newTimeWarp, out warpIndex);
            GUILayout.Label(TimeWarp.fetch.warpRates[Math.Min(TimeWarp.fetch.warpRates.Count() - 1, Math.Max(0, warpIndex))].ToString() + "x");
            newTimeWarp = GUILayout.TextField(newTimeWarp, 1, GUILayout.Width(20));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Override Body Tracker?");
            enableAllBodies = GUILayout.Toggle(enableAllBodies, enableAllBodies ? " Overridden" : " Normal");
            GUILayout.EndHorizontal();

            GUILayout.Label("");
            GUILayout.Label("Global Time Settings");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Overall Multiplier");
            //string newMultiplier = KCT_GameStates.timeSettings.OverallMultiplier.ToString();
            newMultiplier = GUILayout.TextField(newMultiplier, 10, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Build Effect");
            //string newBuildEffect = KCT_GameStates.timeSettings.BuildEffect.ToString();
            newBuildEffect = GUILayout.TextField(newBuildEffect, 10, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Inventory Effect");
            //string newInvEffect = KCT_GameStates.timeSettings.InventoryEffect.ToString();
            newInvEffect = GUILayout.TextField(newInvEffect, 10, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                if (!enabledForSave && KCT_GameStates.settings.enabledForSave)
                    KCT_Utilities.DisableModFunctionality();
                KCT_GameStates.settings.enabledForSave = enabledForSave;

                KCT_GameStates.settings.MaxTimeWarp = Math.Min(TimeWarp.fetch.warpRates.Count()-1, Math.Max(0, int.Parse(newTimeWarp)));
                KCT_GameStates.settings.EnableAllBodies = enableAllBodies;
                KCT_GameStates.settings.Save();

                KCT_GameStates.timeSettings.OverallMultiplier = double.Parse(newMultiplier);
                KCT_GameStates.timeSettings.BuildEffect = double.Parse(newBuildEffect);
                KCT_GameStates.timeSettings.InventoryEffect = double.Parse(newInvEffect);
                KCT_GameStates.timeSettings.Save();
                showSettings = false;
            }
            if (GUILayout.Button("Cancel"))
            {
                showSettings = false;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
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