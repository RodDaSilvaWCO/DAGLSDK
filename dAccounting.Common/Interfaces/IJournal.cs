using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IJournal
    {
        string? ID { get; set; }    
        Dictionary<string, JournalEntryRecord>?  JournalEntries { get; set; }
        void AddJournalEntryRecord(JournalEntryRecord jer);
    }
}
