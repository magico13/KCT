using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerbal_Construction_Time
{
    public class KCT_Reconditioning : IKCTBuildItem
    {
        [Persistent] private string name;
        [Persistent] public double BP, progress;


        public KCT_Reconditioning()
        {
            name = "LaunchPad Reconditioning";
            progress = 0;
            BP = 0;
        }

        public KCT_Reconditioning(Vessel vessel)
        {
            BP = vessel.GetTotalMass() * KCT_GameStates.timeSettings.ReconditioningEffect * KCT_GameStates.timeSettings.OverallMultiplier; //1 day per 25 tons (default) * overall multiplier
            name = "LaunchPad Reconditioning";
            progress = 0;
        }

        public double ProgressPercent()
        {
            return Math.Round(100 * (progress / BP), 2);
        }

        string IKCTBuildItem.GetItemName()
        {
            return name;
        }

        double IKCTBuildItem.GetBuildRate()
        {
            List<double> rates = KCT_Utilities.BuildRatesVAB();
            double buildRate = 0;
            foreach (double rate in rates)
                buildRate += rate;
            return buildRate;
        }

        double IKCTBuildItem.GetTimeLeft()
        {
            return (BP - progress) / ((IKCTBuildItem)this).GetBuildRate();
        }

        KCT_BuildListVessel.ListType IKCTBuildItem.GetListType()
        {
            return KCT_BuildListVessel.ListType.Reconditioning;
        }

        bool IKCTBuildItem.IsComplete()
        {
            return progress >= BP;
        }
    }
}
