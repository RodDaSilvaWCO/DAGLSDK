using dAccounting.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Interfaces
{
    public interface IJurisdictiondAccountingServiceConfig
    {
        decimal FeeRate { get; }
        long    MinimumFee { get; }
        long    MaximumFee { get; }
        List<DLTGeneralLedgerAccountInfo>? DLTGeneralLedgerChartOfAccounts { get;  }

        
    }
}
