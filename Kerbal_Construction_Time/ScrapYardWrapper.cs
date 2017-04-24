using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//TODO: Change namespace to your mod's namespace
namespace KerbalConstructionTime
{
    //DO NOT CHANGE ANYTHING BELOW THIS LINE
    public sealed class ScrapYardWrapper
    {
        private static bool? available;
        private static Type SYType;
        private static object _instance;

        public enum ComparisonStrength
        {
            NAME, //says they're equal if names match
            COSTS, //says Name and dry costs are the same
            MODULES, //as above, plus tracked modules (except MdouleSYPartTracker) match
            TRACKER, //as above, plus the number of times used must match
            STRICT //as above, plus the ids match
        }

        /// <summary>
        /// True if ScrapYard is available, false if not
        /// </summary>
        public static bool Available
        {
            get
            {
                if (available == null)
                {
                    SYType = AssemblyLoader.loadedAssemblies
                        .Select(a => a.assembly.GetExportedTypes())
                        .SelectMany(t => t)
                        .FirstOrDefault(t => t.FullName == "ScrapYard.APIManager");
                    available = SYType != null;
                }
                return available.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Removes inventory parts, refunds funds, marks it as tracked
        /// </summary>
        /// <param name="parts">The vessel as a List of Parts</param>
        /// <returns>True if processed, false otherwise</returns>
        public static bool ProcessVessel(List<Part> parts)
        {
            if (!Available)
            {
                return false;
            }
            return (bool)invokeMethod("ProcessVessel_Parts", parts);
        }

        /// <summary>
        /// Removes inventory parts, refunds funds, marks it as tracked
        /// </summary>
        /// <param name="parts">The vessel as a List of part ConfigNodes</param>
        /// <returns>True if processed, false otherwise</returns>
        public static bool ProcessVessel(List<ConfigNode> parts)
        {
            if (!Available)
            {
                return false;
            }
            return (bool)invokeMethod("ProcessVessel_Nodes", parts);
        }

        /// <summary>
        /// Adds a list of parts to the Inventory
        /// </summary>
        /// <param name="parts">The list of parts to add</param>
        /// <param name="incrementRecovery">If true, increments the number of recoveries in the tracker</param>
        public static void AddPartsToInventory(List<Part> parts, bool incrementRecovery)
        {
            if (Available)
            {
                invokeMethod("AddPartsToInventory_Parts", parts, incrementRecovery);
            }
        }

        /// <summary>
        /// Adds a list of parts to the Inventory
        /// </summary>
        /// <param name="parts">The list of parts to add</param>
        /// <param name="incrementRecovery">If true, increments the number of recoveries in the tracker</param>
        public static void AddPartsToInventory(List<ConfigNode> parts, bool incrementRecovery)
        {
            if (Available)
            {
                invokeMethod("AddPartsToInventory_Nodes", parts, incrementRecovery);
            }
        }

        /// <summary>
        /// Records a build in the part tracker
        /// </summary>
        /// <param name="parts">The vessel as a list of Parts.</param>
        public static void RecordBuild(List<Part> parts)
        {
            if (Available)
            {
                invokeMethod("RecordBuild_Parts", parts);
            }
        }

        /// <summary>
        /// Records a build in the part tracker
        /// </summary>
        /// <param name="parts">The vessel as a list of ConfigNodes.</param>
        public static void RecordBuild(List<ConfigNode> parts)
        {
            if (Available)
            {
                invokeMethod("RecordBuild_Nodes", parts);
            }
        }

        /// <summary>
        /// Takes a List of Parts and returns the Parts that are present in the inventory. 
        /// </summary>
        /// <param name="sourceParts">Source list of parts</param>
        /// <param name="strictness">How strict of a comparison to use. Defaults to MODULES</param>
        /// <returns>List of Parts that are in the inventory</returns>
        public static List<Part> GetPartsInInventory(List<Part> sourceParts, ComparisonStrength strictness = ComparisonStrength.MODULES)
        {
            if (!Available)
            {
                return null;
            }
            return (List<Part>)invokeMethod("GetPartsInInventory_Parts", sourceParts, strictness.ToString());
            //Why do a ToString on an enum instead of casting to int? Because if the internal enum changes then the intended strictness is kept.
        }

        /// <summary>
        /// Takes a List of part ConfigNodes and returns the ConfigNodes that are present in the inventory. 
        /// </summary>
        /// <param name="sourceParts">Source list of parts</param>
        /// <param name="strictness">How strict of a comparison to use. Defaults to MODULES</param>
        /// <returns>List of part ConfigNodes that are in the inventory</returns>
        public static List<ConfigNode> GetPartsInInventory(List<ConfigNode> sourceParts, ComparisonStrength strictness = ComparisonStrength.MODULES)
        {
            if (!Available)
            {
                return null;
            }
            return (List<ConfigNode>)invokeMethod("GetPartsInInventory_ConfigNodes", sourceParts, strictness.ToString());
            //Why do a ToString on an enum instead of casting to int? Because if the internal enum changes then the intended strictness is kept.
        }

        /// <summary>
        /// Checks if the part is pulled from the inventory or is new
        /// </summary>
        /// <param name="part">The part to check</param>
        /// <returns>True if from inventory, false if new</returns>
        public static bool PartIsFromInventory(Part part)
        {
            if (!Available)
            {
                return false;
            }
            return (bool)invokeMethod("PartIsFromInventory_Part", part);
        }

        /// <summary>
        /// Checks if the part is pulled from the inventory or is new
        /// </summary>
        /// <param name="part">The part to check</param>
        /// <returns>True if from inventory, false if new</returns>
        public static bool PartIsFromInventory(ConfigNode part)
        {
            if (!Available)
            {
                return false;
            }
            return (bool)invokeMethod("PartIsFromInventory_Node", part);
        }

        /// <summary>
        /// Gets the number of builds for a part
        /// </summary>
        /// <param name="part">The part to check</param>
        /// <returns>Number of builds for the part</returns>
        public static int GetBuildCount(Part part)
        {
            if (!Available)
            {
                return 0;
            }
            return (int)invokeMethod("GetBuildCount_Part", part);
        }

        /// <summary>
        /// Gets the number of builds for a part
        /// </summary>
        /// <param name="partNode">The ConfigNode of the part to check</param>
        /// <returns>Number of builds for the part</returns>
        public static int GetBuildCount(ConfigNode part)
        {
            if (!Available)
            {
                return 0;
            }
            return (int)invokeMethod("GetBuildCount_Node", part);
        }

        /// <summary>
        /// Gets the number of total uses of a part
        /// </summary>
        /// <param name="part">The part to check</param>
        /// <returns>Number of uses of the part</returns>
        public static int GetUseCount(Part part)
        {
            if (!Available)
            {
                return 0;
            }
            return (int)invokeMethod("GetUseCount_Part", part);
        }

        /// <summary>
        /// Gets the number of total uses of a part
        /// </summary>
        /// <param name="partNode">The ConfigNode of the part to check</param>
        /// <returns>Number of uses of the part</returns>
        public static int GetUseCount(ConfigNode part)
        {
            if (!Available)
            {
                return 0;
            }
            return (int)invokeMethod("GetUseCount_Node", part);
        }

        #region Private Methods
        /// <summary>
        /// The static instance of the APIManager within ScrapYard
        /// </summary>
        private static object Instance
        {
            get
            {
                if (Available && _instance == null)
                {
                    _instance = SYType.GetProperty("Instance").GetValue(null, null);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Invokes a method on the ScrapYard API
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="parameters">Parameters to pass to the method</param>
        /// <returns>The response</returns>
        private static object invokeMethod(string methodName, params object[] parameters)
        {
            MethodInfo method = SYType.GetMethod(methodName);
            return method?.Invoke(Instance, parameters);
        }
        #endregion Private Methods
    }
}
