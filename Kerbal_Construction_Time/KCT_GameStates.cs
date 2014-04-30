using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerbal_Construction_Time
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
        public static bool delayStart = false;
        public static Dictionary<String, int> PartTracker = new Dictionary<string, int>();
        public static Dictionary<String, int> PartInventory = new Dictionary<string, int>();
        public static bool flightSimulated = false;
        public static String simulationReason;
        public static KCT_Settings settings = new KCT_Settings();
        public static KCT_TimeSettings timeSettings = new KCT_TimeSettings();
        public static double simulationEndTime = 0;
        public static List<KCT_BuildListVessel> VABList = new List<KCT_BuildListVessel>();
        public static List<KCT_BuildListVessel> VABWarehouse = new List<KCT_BuildListVessel>();
        public static List<KCT_BuildListVessel> SPHList = new List<KCT_BuildListVessel>();
        public static List<KCT_BuildListVessel> SPHWarehouse = new List<KCT_BuildListVessel>();
        public static KCT_BuildListVessel launchedVessel;
        public static Dictionary<uint, List<ProtoCrewMember>> launchedCrew = new Dictionary<uint, List<ProtoCrewMember>>();
        public static IButton kctToolbarButton;

        public static CelestialBody simulationBody;
        public static bool simulateInOrbit = false;
        public static double simOrbitAltitude = 0;
        public static List<String> BodiesVisited = new List<string> {"Kerbin"};

        public static void reset()
        {
            PartTracker = new Dictionary<string, int>();
            PartInventory = new Dictionary<string, int>();
            flightSimulated = false;
            vesselDict = new Dictionary<string, string>();
            simulationBody = KCT_Utilities.GetBodyByName("Kerbin");
            simulateInOrbit = false;
            BodiesVisited = new List<string> {"Kerbin"};
          /*  VABList = new List<KCT_BuildListVessel>();
            VABWarehouse = new List<KCT_BuildListVessel>();
            SPHList = new List<KCT_BuildListVessel>();
            SPHWarehouse = new List<KCT_BuildListVessel>();*/
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