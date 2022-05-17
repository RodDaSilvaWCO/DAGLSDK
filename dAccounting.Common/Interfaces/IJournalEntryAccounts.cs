using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IJournalEntryAccounts
    {
        
        void AddDoubleEntry( List<DLTGLAccount> debtAccounts, List<DLTGLAccount> creditAccounts, bool assertBalanced = true);
        string DumpJournalEntry();
        // string GenerateJournalEntriesReport(List<IJournalEntryRecord> journalEntriesToReport, int startingOrdinal = 1);

        List<DLTGLAccount> DebtAccountList { get; set; }
        List<DLTGLAccount> CreditAccountList { get; set; }

    }
}
