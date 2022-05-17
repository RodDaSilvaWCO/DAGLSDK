using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;
namespace dAccounting.Common.Interfaces
{
    public interface IGeneralLedgerAccountProperties
    {
        int Code { get; set; }
        int Type { get; set; }
        //string? GetSemanticName();
        string GetLocalizedName(CultureInfo culture = null!);
        bool OmitFromReportingIfZero { get; set; }
    }
}
