using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerbal_Construction_Time
{
    public class KCT_Recon_Rollout : IKCTBuildItem
    {
        [Persistent] private string name;
        [Persistent] public double BP, progress;
        [Persistent] public string associatedID;
        public enum RolloutReconType { Reconditioning, Rollout, Rollback, None };
        public RolloutReconType RRType;

        public KCT_Recon_Rollout()
        {
            name = "LaunchPad Reconditioning";
            progress = 0;
            BP = 0;
            RRType = RolloutReconType.None;
            associatedID = "";
        }

        public KCT_Recon_Rollout(Vessel vessel, RolloutReconType type, string id)
        {
            RRType = type;
            associatedID = id;
            if (type == RolloutReconType.Reconditioning) 
            {
                BP = vessel.GetTotalMass() * KCT_GameStates.timeSettings.ReconditioningEffect * KCT_GameStates.timeSettings.OverallMultiplier; //1 day per 50 tons (default) * overall multiplier
                if (BP > KCT_GameStates.timeSettings.MaxReconditioning) BP = KCT_GameStates.timeSettings.MaxReconditioning;
                BP *= (1 - KCT_GameStates.timeSettings.RolloutReconSplit);
                name = "LaunchPad Reconditioning";
            }
            else if (type == RolloutReconType.Rollout)
            {
                BP = vessel.GetTotalMass() * KCT_GameStates.timeSettings.ReconditioningEffect * KCT_GameStates.timeSettings.OverallMultiplier; //1 day per 50 tons (default) * overall multiplier
                if (BP > KCT_GameStates.timeSettings.MaxReconditioning) BP = KCT_GameStates.timeSettings.MaxReconditioning;
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Rollout";
            }
            else if (type == RolloutReconType.Rollback)
            {
                BP = vessel.GetTotalMass() * KCT_GameStates.timeSettings.ReconditioningEffect * KCT_GameStates.timeSettings.OverallMultiplier; //1 day per 50 tons (default) * overall multiplier
                if (BP > KCT_GameStates.timeSettings.MaxReconditioning) BP = KCT_GameStates.timeSettings.MaxReconditioning;
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Rollback";
            }
            progress = 0;
        }

        public void SwapRolloutType()
        {
            if (RRType == RolloutReconType.Rollout)
                RRType = RolloutReconType.Rollback;
            else if (RRType == RolloutReconType.Rollback)
                RRType = RolloutReconType.Rollout;
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

        IKCTBuildItem AsBuildItem()
        {
            return (IKCTBuildItem)this;
        }
    }
}
