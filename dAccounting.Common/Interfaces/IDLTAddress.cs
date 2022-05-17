using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IDLTAddress
    {
        string? AddressID { get; set; }
        KeyVault? KeyVault { get; set; }
        string? Status { get; set; }
    }
}
