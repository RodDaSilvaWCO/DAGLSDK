using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IDLTGLAccount
    {
        JurisdictionMember? JurisdictionMember { get; set; }
        DLTGeneralLedgerAccountInfo? DLTGLAccountInfo{ get; }
        ulong? Amount { get;  }
    }
}
