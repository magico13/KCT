/********************************************************************************
 * Mission Controller Extended Wrapper class originally written by magico13.    *
 * You must still reference MissionLibrary.dll in your mod, but need not        *
 * include it in your release.                                                  *
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerbal_Construction_Time //Change this to your mod's namespace
{
    /********************************************************
     * You should not change anything below this line!      *
     *******************************************************/
    class MCEWrapper
    {
        public static bool MCEAvailable
        {
            get
            {
                Type MCE = AssemblyLoader.loadedAssemblies
                .Select(a => a.assembly.GetExportedTypes())
                .SelectMany(t => t)
                .FirstOrDefault(t => t.FullName == "MissionController.MissionController");

                if (MCE == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static int ModCost(int value, string description)
        {
            return MissionController.ManagerAccessor.get.ModCost(value, description);
        }

        public static int modReward(int value, string description)
        {
            return MissionController.ManagerAccessor.get.modReward(value, description);
        }

        public static int CleanReward(int value)
        {
            return MissionController.ManagerAccessor.get.CleanReward(value);
        }

        public static int recyclereward(int value)
        {
            return MissionController.ManagerAccessor.get.recyclereward(value);
        }

        public static float sciencereward(float value)
        {
            return MissionController.ManagerAccessor.get.sciencereward(value);
        }

        public static int kerbCost(int value)
        {
            return MissionController.ManagerAccessor.get.kerbCost(value);
        }

        public static int IgetBudget()
        {
            return MissionController.ManagerAccessor.get.IgetBudget();
        }

        public static int Itotalbudget()
        {
            return MissionController.ManagerAccessor.get.Itotalbudget();
        }

        public static int ItotalSpentVehicles()
        {
            return MissionController.ManagerAccessor.get.ItotalSpentVehicles();
        }

        public static int ItotalRecycleMoney()
        {
            return MissionController.ManagerAccessor.get.ItotalRecycleMoney();
        }

        public static int ItotalHiredKerbCost()
        {
            return MissionController.ManagerAccessor.get.ItotalHiredKerbCost();
        }

        public static int ItotalModPayment()
        {
            return MissionController.ManagerAccessor.get.ItotalModPayment();
        }

        public static int ItotalModCost()
        {
            return MissionController.ManagerAccessor.get.ItotalModCost();
        }

        public static void IloadMCEbackup()
        {
            MissionController.ManagerAccessor.get.IloadMCEbackup();
        }

        public static void IloadMCESave()
        {
            MissionController.ManagerAccessor.get.IloadMCESave();
        }

        public static void IsaveMCE()
        {
            MissionController.ManagerAccessor.get.IsaveMCE();
        }
    }
}
