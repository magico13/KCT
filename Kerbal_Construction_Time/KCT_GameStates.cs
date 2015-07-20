using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalConstructionTime
{
    public static class KCT_GameStates
    {
        public static double UT;
        public static bool canWarp = false, warpInitiated = false;
        public static int lastWarpRate = 0;
        public static string lastSOIVessel = "";
        public static Dictionary<string, string> vesselDict = new Dictionary<string, string>();
        public static List<VesselType> VesselTypesForSOI = new List<VesselType>() { VesselType.Base, VesselType.Lander, VesselType.Probe, VesselType.Ship, VesselType.Station };
        public static List<Orbit.PatchTransitionType> SOITransitions = new List<Orbit.PatchTransitionType> { Orbit.PatchTransitionType.ENCOUNTER, Orbit.PatchTransitionType.ESCAPE };
        public static bool delayStart = false, delayMove = false;
        public static Dictionary<String, int> PartTracker = new Dictionary<string, int>();
        public static Dictionary<String, int> PartInventory = new Dictionary<string, int>();
        public static bool flightSimulated = false;
        public static String simulationReason;
        public static KCT_Settings settings = new KCT_Settings();
       // public static KCT_TimeSettings timeSettings = new KCT_TimeSettings();
      //  public static KCT_FormulaSettings formulaSettings = new KCT_FormulaSettings();
        //public static ShipBackup recoveryRequestVessel;

        public static KCT_KSC ActiveKSC = null;
        public static List<KCT_KSC> KSCs = new List<KCT_KSC>();
        public static string activeKSCName = "";

        /*public static List<KCT_BuildListVessel> VABList = new List<KCT_BuildListVessel>();
        public static List<KCT_BuildListVessel> VABWarehouse = new List<KCT_BuildListVessel>();
        public static List<KCT_BuildListVessel> SPHList = new List<KCT_BuildListVessel>();
        public static List<KCT_BuildListVessel> SPHWarehouse = new List<KCT_BuildListVessel>();
        public static List<KCT_TechItem> TechList = new List<KCT_TechItem>();
        public static List<int> VABUpgrades = new List<int>() {0};
        public static List<int> SPHUpgrades = new List<int>() {0};
        public static List<int> RDUpgrades = new List<int>() {0, 0};
        public static KCT_Reconditioning LaunchPadReconditioning;*/
        public static int TechUpgradesTotal = 0;
        public static List<KCT_TechItem> TechList = new List<KCT_TechItem>();

        public static List<int> PurchasedUpgrades = new List<int>() { 0, 0 };
        public static float InventorySaleUpgrades = 0, InventorySalesFigures = 0;
        public static int UpgradesResetCounter = 0;
        //public static int TotalUpgradePoints = 0;
        public static KCT_BuildListVessel launchedVessel, editedVessel, recoveredVessel;
        //public static Dictionary<uint, List<ProtoCrewMember>> launchedCrew = new Dictionary<uint, List<ProtoCrewMember>>();
        public static List<CrewedPart> launchedCrew = new List<CrewedPart>();
        public static IButton kctToolbarButton;
        public static bool EditorShipEditingMode = false, buildSimulatedVessel = false;
        public static bool firstStart = false;
        public static IKCTBuildItem targetedItem = null;
        public static double EditorBuildTime = 0, EditorRolloutCosts = 0;
        public static Dictionary<string, int> EditedVesselParts = new Dictionary<string, int>();
        public static bool LaunchFromTS = false;
        public static bool LoadingSimulationSave = false;

        public static List<bool> showWindows = new List<bool> { false, true }; //build list, editor
        public static string KACAlarmId = "";
        public static double KACAlarmUT = 0;
        
        //Things pertaining to simulations
        public static CelestialBody simulationBody;
        public static bool simulateInOrbit = false, simulationInitialized = false;
        public static double simulationUT = 0;
        public static double simulationEndTime = 0, simulationTimeLimit = 0, simulationDefaultTimeLimit = 0;
        public static double simOrbitAltitude = 0, simInclination = 0;
        public static List<String> BodiesVisited = new List<string> { KCT_Utilities.GetBodyByName("Earth") != null ? "Earth" : "Kerbin" };
        public static float SimulationCost = 0, FundsToChargeAtSimEnd = 0, FundsGivenForVessel = 0;
        public static int EditorSimulationCount = 0;

        public static bool TestFlightPartFailures = true;
        public static bool RemoteTechEnabled = true;

        public static KCT_OnLoadError erroredDuringOnLoad = new KCT_OnLoadError();


        public static void reset()
        {
            //firstStart = true;
            PartTracker = new Dictionary<string, int>();
            PartInventory = new Dictionary<string, int>();
            flightSimulated = false;
            simulationInitialized = false;
            vesselDict = new Dictionary<string, string>();
            BodiesVisited = new List<string> { KCT_Utilities.GetBodyByName("Earth") != null ? "Earth" : "Kerbin" };
            simulationBody = KCT_Utilities.GetBodyByName(BodiesVisited[0]);
            simulateInOrbit = false;
            firstStart = false;
            
          /*  VABUpgrades = new List<int>() {0};
            SPHUpgrades = new List<int>() {0};
            RDUpgrades = new List<int>() {0, 0};*/
            PurchasedUpgrades = new List<int>() { 0, 0 };
           // LaunchPadReconditioning = null;
            targetedItem = null;
            KCT_GUI.fundsCost = -13;
            KCT_GUI.sciCost = -13;
            KCT_GUI.nodeRate = -13;
            KCT_GUI.upNodeRate = -13;
            KCT_GUI.researchRate = -13;
            KCT_GUI.upResearchRate = -13;

            InventorySaleUpgrades = 0;
            InventorySalesFigures = 0;

            //ActiveKSC = new KCT_KSC("Stock");
            //KSCs = new List<KCT_KSC>() {ActiveKSC};


           /* VABList = new List<KCT_BuildListVessel>();
            VABWarehouse = new List<KCT_BuildListVessel>();
            SPHList = new List<KCT_BuildListVessel>();
            SPHWarehouse = new List<KCT_BuildListVessel>();
            TechList = new List<KCT_TechItem>();*/
        }

    }

    public class CrewedPart
    {
        public List<ProtoCrewMember> crewList;
        public uint partID;

        public CrewedPart(uint ID, List<ProtoCrewMember> crew)
        {
            partID = ID;
            crewList = crew;
        }

        public CrewedPart FromPart(Part part, List<ProtoCrewMember> crew)
        {
            partID = part.flightID;
            crewList = crew;
            return this;
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