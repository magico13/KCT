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

        public KCT_Reconditioning(double buildPoints)
        {
            name = "LaunchPad Reconditioning";
            BP = buildPoints;
            progress = 0;
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
