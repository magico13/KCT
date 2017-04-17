using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KerbalConstructionTime
{
    //DO NOT CHANGE ANYTHING BELOW THIS LINE
    public sealed class ScrapYardWrapper
    {
        private static bool? available;
        private static Type SYType;
        private static object _instance;

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
        /// <param name="applyInventory">If true, applies inventory parts.</param>
        /// <returns>True if processed, false otherwise</returns>
        public static bool ProcessVessel(List<Part> parts, bool applyInventory)
        {
            if (!Available)
            {
                return false;
            }
            MethodInfo method = SYType.GetMethod("ProcessVessel_Parts");
            object result = method.Invoke(Instance, new object[] { parts, applyInventory });
            return (bool)result;
        }

        /// <summary>
        /// Removes inventory parts, refunds funds, marks it as tracked
        /// </summary>
        /// <param name="parts">The vessel as a List of part ConfigNodes</param>
        /// <param name="applyInventory">If true, applies inventory parts.</param>
        /// <returns>True if processed, false otherwise</returns>
        public static bool ProcessVessel(List<ConfigNode> parts, bool applyInventory)
        {
            if (!Available)
            {
                return false;
            }
            MethodInfo method = SYType.GetMethod("ProcessVessel_Nodes");
            object result = method.Invoke(Instance, new object[] { parts, applyInventory });
            return (bool)result;
        }

        /// <summary>
        /// Takes a List of Parts and returns the Parts that are present in the inventory. 
        /// Assumes the default strictness.
        /// </summary>
        /// <param name="sourceParts">Source list of parts</param>
        /// <returns>List of Parts that are in the inventory</returns>
        public static List<Part> GetPartsInInventory(List<Part> sourceParts)
        {
            if (!Available)
            {
                return null;
            }
            MethodInfo method = SYType.GetMethod("GetPartsInInventory_Parts");
            object result = method.Invoke(Instance, new object[] { sourceParts });
            return (List<Part>)result;
        }

        /// <summary>
        /// Takes a List of part ConfigNodes and returns the ConfigNodes that are present in the inventory. 
        /// Assumes the default strictness.
        /// </summary>
        /// <param name="sourceParts">Source list of parts</param>
        /// <returns>List of part ConfigNodes that are in the inventory</returns>
        public static List<ConfigNode> GetPartsInInventory(List<ConfigNode> sourceParts)
        {
            if (!Available)
            {
                return null;
            }
            MethodInfo method = SYType.GetMethod("GetPartsInInventory_ConfigNodes");
            object result = method.Invoke(Instance, new object[] { sourceParts });
            return (List<ConfigNode>)result;
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
            MethodInfo method = SYType.GetMethod("GetBuildCount_Part");
            object result = method.Invoke(Instance, new object[] { part });
            return (int)result;
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
            MethodInfo method = SYType.GetMethod("GetBuildCount_Node");
            object result = method.Invoke(Instance, new object[] { part });
            return (int)result;
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
            MethodInfo method = SYType.GetMethod("GetUseCount_Part");
            object result = method.Invoke(Instance, new object[] { part });
            return (int)result;
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
            MethodInfo method = SYType.GetMethod("GetUseCount_Node");
            object result = method.Invoke(Instance, new object[] { part });
            return (int)result;
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
        #endregion Private Methods
    }
}
