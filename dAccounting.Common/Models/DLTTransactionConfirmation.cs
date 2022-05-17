using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class DLTTransactionConfirmation : IDLTTransactionConfirmation
    {
        #region Field Members
        #endregion

        #region Constructors
        public DLTTransactionConfirmation( string? status )
        {
            Status = status;
        }

        public DLTTransactionConfirmation(string? status, string? transactionid )
        {
            Status = status;
            TransactionId = transactionid;
        }

        #endregion

        #region Public Interface
        public string? Status { get; }
        public string? TransactionId { get; }
        public long TransactionTimeSeconds { get; set; }
        public int TransactionTimeNanoSeconds { get; set; }
        public bool IsPending { get; set; }
        public int ExchangeRateUSDCentEquivalent { get; set; }
        public int NextExchangeRateUSDCentEquivalent { get; set; }
        public int ExchangeRateNativeCryptoEquivalent { get; set; }
        public int NextExchangeRateNativeCryptoEquivalent { get; set; }
        public DateTime ExchangeRateExpiration { get; set; }
        public DateTime NextExchangeRateExpiration { get; set; }
        public DLTAddress? ContractId { get; set; }
        #endregion

        #region Helpers
        #endregion
    }
}
