using dAccounting.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Interfaces
{
    public interface IDLTTransactionConfirmation
    {
        string? Status { get; }
        string? TransactionId { get; }
        long TransactionTimeSeconds { get; set; }
        int TransactionTimeNanoSeconds { get; set; }
        bool IsPending { get; set; }
        int ExchangeRateUSDCentEquivalent { get; set; }
        int NextExchangeRateUSDCentEquivalent { get; set; }
        int ExchangeRateNativeCryptoEquivalent { get; set; }
        int NextExchangeRateNativeCryptoEquivalent { get; set; }
        DateTime ExchangeRateExpiration { get; set; }
        DateTime NextExchangeRateExpiration { get; set; }
        DLTAddress? ContractId { get; set; }


    }
}
