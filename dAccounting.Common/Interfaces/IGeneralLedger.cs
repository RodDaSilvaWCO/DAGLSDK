using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IGeneralLedger
    {
        string? ID { get; set; }
        string? Description { get; set;}
        List<DLTGeneralLedgerAccountInfo>? DebitChartOfAccounts { get; set; }
        List<DLTGeneralLedgerAccountInfo>? CreditChartOfAccounts { get; set; }
        string? COATemplateID { get; set; }
    }
}
