using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface ITrialBalance
    {
        DateTime BalanceDateTime { get; set; }
        List<DLTGLAccount>? DebitChartOfAccounts { get; set; }
        List<DLTGLAccount>? CreditChartOfAccounts { get; set; }
        GeneralLedgerAccountsCatalog? GeneralLedgerCatalog { get; set; }
        string DumpTrialBalance( /*GLReportOptions reportOptions = null!*/);
        string IncomeStatementReport();
        string BalanceSheetReport();
    }
}
