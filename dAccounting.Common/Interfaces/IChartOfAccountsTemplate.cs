using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IChartOfAccountsTemplate
    {
        string? ID { get; }
        string? Name { get; set; }
        string? Description { get; set; }
        List<GLAccountCode>? ChartOfAccounts { get; set; }
        string DumpChartOfAccounts(IGeneralLedgerAccountsCatalog glaccountcatalog );
        
    }
}
