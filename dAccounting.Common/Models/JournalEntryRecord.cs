using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class JournalEntryRecord : IJournalEntryRecord
    {
        #region Field Members
        #endregion

        #region Constructors
        public JournalEntryRecord() { } // Required for serialization
        #endregion

        #region Public Interface
        public string? TransactionID { get; set; }

        public string? DLTTransactionReceiptID { get; set; }

        //[JsonPropertyName("M")]
        public string? Memo { get; set; }
        //[JsonPropertyName("D")]
        public DateTime PostDate { get; set; }
        //[JsonPropertyName("R")]
        public bool IsAutoReversal { get; set; }
        //[JsonPropertyName("JE")]
        public JournalEntryAccounts? JournalEntry { get; set; }

        
        #endregion

        #region Helpers
        #endregion

    }
}
