using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerbal_Construction_Time
{
    public interface IKCTBuildItem
    {
        string GetItemName();
        double GetBuildRate();
        double GetTimeLeft();
        KCT_BuildListVessel.ListType GetListType();
        bool IsComplete();
    }
}
