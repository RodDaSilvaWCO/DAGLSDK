using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Interfaces
{
    public interface IFeeSchedule
    {
        decimal FeeRate { get; set; }
        ulong MinimumFee { get; set; }
        ulong MaximumFee { get; set; }
    }
}
