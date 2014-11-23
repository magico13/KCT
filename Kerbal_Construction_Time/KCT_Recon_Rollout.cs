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
        private RolloutReconType RRTypeInternal = RolloutReconType.None;
        public RolloutReconType RRType
        {
            get
            {
                if (RRTypeInternal != RolloutReconType.None)
                    return RRTypeInternal;
                else
                {
                    if (name == "LaunchPad Reconditioning")
                        RRTypeInternal = RolloutReconType.Reconditioning;
                    if (name == "Vessel Rollout")
                        RRTypeInternal = RolloutReconType.Rollout;
                    if (name == "Vessel Rollback")
                        RRTypeInternal = RolloutReconType.Rollback;
                    return RRTypeInternal;
                }
            }
            set
            {
                RRTypeInternal = value;
            }
        }

        public KCT_KSC KSC { get { return KCT_GameStates.KSCs.FirstOrDefault(k => k.GetReconRollout(RRType).associatedID == this.associatedID);} }

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
            BP = vessel.GetTotalMass() * KCT_GameStates.timeSettings.ReconditioningEffect * KCT_GameStates.timeSettings.OverallMultiplier; //1 day per 50 tons (default) * overall multiplier
            if (BP > KCT_GameStates.timeSettings.MaxReconditioning) BP = KCT_GameStates.timeSettings.MaxReconditioning;
            if (type == RolloutReconType.Reconditioning) 
            {
                BP *= (1 - KCT_GameStates.timeSettings.RolloutReconSplit);
                name = "LaunchPad Reconditioning";
            }
            else if (type == RolloutReconType.Rollout)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Rollout";
            }
            else if (type == RolloutReconType.Rollback)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Rollback";
            }
            progress = 0;
        }

        public KCT_Recon_Rollout(KCT_BuildListVessel vessel, RolloutReconType type, string id)
        {
            RRType = type;
            associatedID = id;
            BP = vessel.GetTotalMass() * KCT_GameStates.timeSettings.ReconditioningEffect * KCT_GameStates.timeSettings.OverallMultiplier; //1 day per 50 tons (default) * overall multiplier
            if (BP > KCT_GameStates.timeSettings.MaxReconditioning) BP = KCT_GameStates.timeSettings.MaxReconditioning;
            if (type == RolloutReconType.Reconditioning)
            {
                BP *= (1 - KCT_GameStates.timeSettings.RolloutReconSplit);
                name = "LaunchPad Reconditioning";
            }
            else if (type == RolloutReconType.Rollout)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Rollout";
            }
            else if (type == RolloutReconType.Rollback)
            {
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
            List<double> rates = KCT_Utilities.BuildRatesVAB(KSC);
            double buildRate = 0;
            foreach (double rate in rates)
                buildRate += rate;
            if (RRType == RolloutReconType.Rollback)
                buildRate *= -1;
            return buildRate;
        }

        double IKCTBuildItem.GetTimeLeft()
        {
            double timeLeft = (BP - progress) / ((IKCTBuildItem)this).GetBuildRate();
            if (RRType == RolloutReconType.Rollback)
                timeLeft = (-progress) / ((IKCTBuildItem)this).GetBuildRate();
            return timeLeft;
        }

        KCT_BuildListVessel.ListType IKCTBuildItem.GetListType()
        {
            return KCT_BuildListVessel.ListType.Reconditioning;
        }

        bool IKCTBuildItem.IsComplete()
        {
            bool complete = progress >= BP;
            if (RRType == RolloutReconType.Rollback)
                complete = progress <= 0;
            return complete;
        }

        public IKCTBuildItem AsBuildItem()
        {
            return (IKCTBuildItem)this;
        }
    }
}
