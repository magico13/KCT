using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalConstructionTime
{
    public class CrewQ_Integrator
    {
        private static bool? available = null;
        private static Type CrewQType = null;
        private static object instance_;


        /* Call this to see if the addon is available. If this returns false, no additional API calls should be made! */
        public static bool CrewQ_Available
        {
            get
            {
                if (available == null)
                {
                    CrewQType = AssemblyLoader.loadedAssemblies
                        .Select(a => a.assembly.GetExportedTypes())
                        .SelectMany(t => t)
                        .FirstOrDefault(t => t.FullName == "CrewQ.CrewQ");
                    available = CrewQType != null;
                }
                return (bool)available;
            }
        }


        private static object Instance
        {
            get
            {
                if (CrewQ_Available && instance_ == null)
                {
                    instance_ = CrewQType.GetProperty("Instance").GetValue(null, null);
                }

                return instance_;
            }
        }

        public static void SuppressCrew()
        {
            if (CrewQ_Available)
            {
                System.Reflection.MethodInfo suppressMethod = CrewQType.GetMethod("SuppressCrew");
                suppressMethod.Invoke(Instance, new object[] { });
            }
        }

        public static void ReleaseCrew()
        {
            if (CrewQ_Available)
            {
                System.Reflection.MethodInfo releaseMethod = CrewQType.GetMethod("ReleaseCrew");
                releaseMethod.Invoke(Instance, new object[] { });
            }
        }
    }
}
