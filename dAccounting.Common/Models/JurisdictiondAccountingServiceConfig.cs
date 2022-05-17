using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class JurisdictiondAccountingServiceConfig : IJurisdictiondAccountingServiceConfig
    {
        #region Field Members
        #endregion

        #region Constructors
        public JurisdictiondAccountingServiceConfig( decimal feerate, long minfee, long maxfee, List<DLTGeneralLedgerAccountInfo> dltglaccountinfo )
        {
            FeeRate = feerate;
            MinimumFee = (minfee < 0 ? 1 : minfee);
            MaximumFee = (maxfee < 0 ? long.MaxValue : maxfee);
            DLTGeneralLedgerChartOfAccounts = dltglaccountinfo;
        }

        public JurisdictiondAccountingServiceConfig()  { }  // required for serialization
        #endregion

        #region Public Interface
        public decimal FeeRate { get; set; }

        public long MinimumFee { get; set; }

        public long MaximumFee { get; set; }

        public List<DLTGeneralLedgerAccountInfo>? DLTGeneralLedgerChartOfAccounts { get; set; }

        #endregion

        #region Helpers
        #endregion
    }
}
