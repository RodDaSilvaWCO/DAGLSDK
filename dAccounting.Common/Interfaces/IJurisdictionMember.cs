using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IJurisdictionMember
    {
        string? ID { get;set;}
        string? GeneralLedgerID { get; set; }
        string? Description { get; set; }
        DLTAddress? CryptoAddress { get; set; }
    }
}
