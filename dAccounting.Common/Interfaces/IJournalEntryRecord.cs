using dAccounting.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Interfaces
{
    public interface IJournalEntryRecord
    {
        string? TransactionID { get; set; }
        string? DLTTransactionReceiptID { get; set; }
        string? Memo { get; set; }
        DateTime PostDate { get; set; }
        bool IsAutoReversal { get; set; }
        JournalEntryAccounts? JournalEntry { get; set; }
    }
}
