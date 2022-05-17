using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class FeeSchedule : IFeeSchedule
    {
        #region Field Members
        #endregion

        #region Constructors
        public FeeSchedule() { } // required for serialization
        public FeeSchedule( decimal feerate, ulong minfee, ulong maxfee )
        {
            FeeRate = feerate;
            MinimumFee = minfee;
            MaximumFee = maxfee;
        }
        #endregion

        #region Public Interface
        public decimal FeeRate { get; set; }
        public ulong MinimumFee { get; set; }
        public ulong MaximumFee { get; set; }
        #endregion

        #region Helpers
        #endregion

    }
}
