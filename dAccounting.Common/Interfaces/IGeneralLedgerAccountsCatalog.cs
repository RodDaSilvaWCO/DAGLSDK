using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IGeneralLedgerAccountsCatalog
    {
        List<GeneralLedgerAccountProperties>? GeneralLedgerAccountCatalog { get; set; }
        GeneralLedgerAccountProperties? this[GLAccountCode code] { get; }

        void AddGeneralLedgerAccount(GeneralLedgerAccountProperties accountproperties);
    }
}
