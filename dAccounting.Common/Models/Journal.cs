using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class Journal : IJournal
    {
        #region Field Members
        #endregion

        #region Constructors
        public Journal()  // required for serialization
        {
            JournalEntries = new Dictionary<string, JournalEntryRecord>();
        } 

        public Journal( string generalLedgerID ) : this()
        {
            ID = generalLedgerID.ToUpper();
        }
        #endregion

        #region Public Interface
        public string? ID { get; set; }
        public Dictionary<string, JournalEntryRecord>? JournalEntries { get; set; }

        public void AddJournalEntryRecord(JournalEntryRecord jer)
        {
            JournalEntries!.Add(jer.TransactionID!, jer);
        }

        
        #endregion

        #region Helpers
        #endregion
    }
}
