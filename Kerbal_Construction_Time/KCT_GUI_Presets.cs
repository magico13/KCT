using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    public static partial class KCT_GUI
    {
        private static Rect presetPosition = new Rect((3 * Screen.width / 8), (Screen.height / 4), 640, 1);
        private static int presetIndex = -1;
        private static KCT_Preset WorkingPreset;
        private static Vector2 presetScrollView;
        public static void DrawPresetWindow(int windowID)
        {
            if (WorkingPreset == null)
            {
                WorkingPreset = KCT_PresetManager.Instance.ActivePreset; //might need to copy instead of assign here
                presetIndex = KCT_PresetManager.Instance.GetIndex(WorkingPreset);
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            //preset selector
            GUILayout.BeginVertical();
            GUILayout.Label("Presets", GUILayout.ExpandHeight(false));
            //preset toolbar in a scrollview
            presetScrollView = GUILayout.BeginScrollView(presetScrollView, HighLogic.Skin.textArea, GUILayout.Width(presetPosition.width/6.0f));
            string[] presetNames = KCT_PresetManager.Instance.PresetNames(true);
            if (presetIndex == -1)
                presetIndex = presetNames.Length - 1;
            if (presetIndex < presetNames.Length - 1 && WorkingPreset != KCT_PresetManager.Instance.Presets[presetIndex])
            {
                presetIndex = presetNames.Length - 1; //Custom preset
            }
           /* presetIndex = KCT_PresetManager.Instance.GetIndex(WorkingPreset); //Check that the current preset is equal to the expected one
            if (presetIndex == -1) 
                presetIndex = presetNames.Length - 1;*/
            int prev = presetIndex;
            presetIndex = GUILayout.SelectionGrid(presetIndex, presetNames, 1);
            if (prev != presetIndex) //If a new preset was selected
            {
                if (presetIndex != presetNames.Length - 1)
                {
                    WorkingPreset = KCT_PresetManager.Instance.Presets[presetIndex];
                }
            }

            //presetIndex = GUILayout.Toolbar(presetIndex, presetNames);

            GUILayout.EndScrollView();
            if (GUILayout.Button("Save as\nNew Preset", GUILayout.ExpandHeight(false)))
            {
                //create new preset
            }
            GUILayout.EndVertical();

            //Main sections
            GUILayout.BeginVertical();
            //Preset info section
            GUILayout.BeginVertical(HighLogic.Skin.textArea);
            GUILayout.Label("Preset Name: " + WorkingPreset.name);
            GUILayout.Label("Description: " + WorkingPreset.description);
            GUILayout.Label("Author(s): " + WorkingPreset.author);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            //Features section
            GUILayout.BeginVertical(HighLogic.Skin.textArea);
            GUILayout.Label("Features");
            WorkingPreset.generalSettings.BuildTimes = GUILayout.Toggle(WorkingPreset.generalSettings.BuildTimes, "Build Times", HighLogic.Skin.button);
            WorkingPreset.generalSettings.ReconditioningTimes = GUILayout.Toggle(WorkingPreset.generalSettings.ReconditioningTimes, "Launchpad Reconditioning", HighLogic.Skin.button);
            WorkingPreset.generalSettings.TechUnlockTimes = GUILayout.Toggle(WorkingPreset.generalSettings.TechUnlockTimes, "Tech Unlock Times", HighLogic.Skin.button);
            WorkingPreset.generalSettings.KSCUpgradeTimes = GUILayout.Toggle(WorkingPreset.generalSettings.KSCUpgradeTimes, "KSC Upgrade Times", HighLogic.Skin.button);
            WorkingPreset.generalSettings.SimulationCosts = GUILayout.Toggle(WorkingPreset.generalSettings.SimulationCosts, "Simulation Costs", HighLogic.Skin.button);
            WorkingPreset.generalSettings.RequireVisitsForSimulations = GUILayout.Toggle(WorkingPreset.generalSettings.RequireVisitsForSimulations, "Must Visit Planets", HighLogic.Skin.button);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(HighLogic.Skin.textArea);
            GUILayout.Label("Time Settings");

            

            GUILayout.EndVertical(); //end time settings
            GUILayout.EndHorizontal(); //end feature/time setting split

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
            {
                KCT_PresetManager.Instance.ActivePreset = WorkingPreset;
                KCT_PresetManager.Instance.SaveActiveToSaveData();
                WorkingPreset = null;
                showSettings = false;
            }
            if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
            {
                WorkingPreset = null;
                showSettings = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical(); //end column 2
            GUILayout.EndHorizontal(); //end main split
            GUILayout.EndVertical(); //end window
        }
    }
}
