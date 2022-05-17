using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public  interface IdGeneralLedgerServiceParameters
    {
        string? JurisdictionID { get; }
        string? dAccountingServiceID { get; }
        int MaxNumberOfAmountsInJournalEntry { get; }
        int MaxNumberOfAtomicJournalEntrySwapsInSingleTransaction { get; }
        int MaxMemberIdSize { get; }
        int MaxDLTAddressSize { get; }
        int MaxTransactionIdSize { get; }
        int MaxDLTTransactionReceiptIdSize { get; }
        int MaxMemoSize { get; }
        string? JurisdictionDescription { get; }
        string? dAccountingServiceChartOfAccountTemplateId { get; }
        string? JurisdictionChartOfAccountTemplateId { get; }
        string? JurisdictionMemberChartOfAccountTemplateId { get; }
        string? JurisdictionBaseCurrencySymbol { get; }
        int JurisdictionBaseCurrencyDecimals { get; }
        decimal JurisdictionBuyerTransactionFeeRate { get; }
        ulong JurisdictionBuyerTransactionFeeMinimum { get; }
        ulong JurisdictionBuyerTransactionFeeMaximum { get; }
        decimal JurisdictionSellerTransactionFeeRate { get; }
        ulong JurisdictionSellerTransactionFeeMinimum { get; }
        ulong JurisdictionSellerTransactionFeeMaximum { get; }
        decimal JurisdictionMemberCashInFeeRate { get; }
        ulong JurisdictionMemberCashInFeeMinimum { get; }
        ulong JurisdictionMemberCashInFeeMaximum { get; }
        decimal JurisdictionMemberCashOutFeeRate { get; }
        ulong JurisdictionMemberCashOutFeeMinimum { get; }
        ulong JurisdictionMemberCashOutFeeMaximum { get; }
        decimal JurisdictionCashOutFeeRate { get; }
        ulong JurisdictionCashOutFeeMinimum { get; }
        ulong JurisdictionCashOutFeeMaximum { get; }
        string? TestMemberId1 { get; }
        string? TestMemberId2 { get; }
        DLTAddress? TestMemberCryptoAddress1 { get; }
        DLTAddress? TestMemberCryptoAddress2 { get; }
        DLTAddress? dAccountingServiceCryptoAddress { get; }
        string? PersistenceStorePath { get; }
    }
}
