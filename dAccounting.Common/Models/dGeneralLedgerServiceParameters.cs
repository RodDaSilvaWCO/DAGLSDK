using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class dGeneralLedgerServiceParameters : IdGeneralLedgerServiceParameters
    {
        #region Field Members
        #endregion

        #region Constructors
        public dGeneralLedgerServiceParameters( string? daccountingServiceID,
                                                string? jurisdictionID,              
                                                int maxNumberOfAmountsInJournalEntry,
                                                int maxNumberOfAtomicJournalEntrySwapsInSingleTransaction,
                                                int maxMemberIdSize,
                                                int maxDLTAddressSize,
                                                int maxTransactionIdSize,
                                                int maxDLTTransactionReceiptIdSize,
                                                int maxMemoSize,
                                                string? jurisdictionDescription,
                                                string? daccountingServiceChartOfAccountTemplateId,
                                                string? jurisdictionChartOfAccountTemplateId,
                                                string? jurisdictionMemberChartOfAccountTemplateId,
                                                string? jurisdictionBaseCurrencySymbol,
                                                int jurisdictionBaseCurrencyDecimals,
                                                decimal jurisdictionBuyerTransactionFeeRate,
                                                ulong jurisdictionBuyerTransactionFeeMinimum,
                                                ulong jurisdictionBuyerTransactionFeeMaximum,
                                                decimal jurisdictionSellerTransactionFeeRate,
                                                ulong jurisdictionSellerTransactionFeeMinimum,
                                                ulong jurisdictionSellerTransactionFeeMaximum,
                                                decimal jurisdictionMemberCashInFeeRate,
                                                ulong jurisdictionMemberCashInFeeMinimum,
                                                ulong jurisdictionMemberCashInFeeMaximum,
                                                decimal jurisdictionMemberCashOutFeeRate,
                                                ulong jurisdictionMemberCashOutMinimum,
                                                ulong jurisdictionMemberCashOutFeeMaximum,
                                                decimal jurisdictionCashOutFeeRate,
                                                ulong jurisdictionCashOutFeeMinimum,
                                                ulong jurisdictionCashOutFeeMaximum,
                                                string? testMemberId1,
                                                string? testMemberId2,
                                                DLTAddress? testMemberCryptoAddress1,
                                                DLTAddress? testMemberCryptoAddress2,
                                                DLTAddress? daccountingServiceCryptoAddress,
                                                string? persistenceStorePath

            )
        {
            dAccountingServiceID = daccountingServiceID;
            JurisdictionID = jurisdictionID;
            MaxNumberOfAmountsInJournalEntry = maxNumberOfAmountsInJournalEntry;
            MaxNumberOfAtomicJournalEntrySwapsInSingleTransaction = maxNumberOfAtomicJournalEntrySwapsInSingleTransaction;
            MaxMemberIdSize = maxMemberIdSize;
            MaxDLTAddressSize = maxDLTAddressSize;
            MaxTransactionIdSize = maxTransactionIdSize;
            MaxDLTTransactionReceiptIdSize = maxDLTTransactionReceiptIdSize;
            MaxMemoSize = maxMemoSize;
            JurisdictionDescription = jurisdictionDescription;
            dAccountingServiceChartOfAccountTemplateId = daccountingServiceChartOfAccountTemplateId;
            JurisdictionChartOfAccountTemplateId = jurisdictionChartOfAccountTemplateId;
            JurisdictionMemberChartOfAccountTemplateId = jurisdictionMemberChartOfAccountTemplateId;
            JurisdictionBaseCurrencySymbol = jurisdictionBaseCurrencySymbol;
            JurisdictionBaseCurrencyDecimals = jurisdictionBaseCurrencyDecimals;
            JurisdictionBuyerTransactionFeeRate = jurisdictionBuyerTransactionFeeRate;
            JurisdictionBuyerTransactionFeeMinimum = jurisdictionBuyerTransactionFeeMinimum;
            JurisdictionBuyerTransactionFeeMaximum = jurisdictionBuyerTransactionFeeMaximum;
            JurisdictionSellerTransactionFeeRate = jurisdictionSellerTransactionFeeRate;
            JurisdictionSellerTransactionFeeMinimum = jurisdictionSellerTransactionFeeMinimum;
            JurisdictionSellerTransactionFeeMaximum = jurisdictionSellerTransactionFeeMaximum;
            JurisdictionMemberCashInFeeRate = jurisdictionMemberCashInFeeRate;
            JurisdictionMemberCashInFeeMinimum = jurisdictionMemberCashInFeeMinimum;
            JurisdictionMemberCashInFeeMaximum = jurisdictionMemberCashInFeeMaximum;
            JurisdictionMemberCashOutFeeRate = jurisdictionMemberCashOutFeeRate;
            JurisdictionMemberCashOutFeeMinimum = jurisdictionMemberCashOutMinimum;
            JurisdictionMemberCashOutFeeMaximum = jurisdictionMemberCashOutFeeMaximum;
            JurisdictionCashOutFeeRate = jurisdictionCashOutFeeRate;
            JurisdictionCashOutFeeMinimum = jurisdictionCashOutFeeMinimum;
            JurisdictionCashOutFeeMaximum = jurisdictionCashOutFeeMaximum;
            TestMemberId1 = testMemberId1;
            TestMemberId2 = testMemberId2;
            TestMemberCryptoAddress1 = testMemberCryptoAddress1;
            TestMemberCryptoAddress2 = testMemberCryptoAddress2;
            dAccountingServiceCryptoAddress = daccountingServiceCryptoAddress;
            PersistenceStorePath = persistenceStorePath;
    }
        #endregion

        #region Public Interface
        public string? dAccountingServiceID { get; }
        public string? JurisdictionID { get; }
        public int MaxNumberOfAmountsInJournalEntry { get; }
        public int MaxNumberOfAtomicJournalEntrySwapsInSingleTransaction { get; }
        public int MaxMemberIdSize { get; }
        public int MaxDLTAddressSize { get; }
        public int MaxTransactionIdSize { get; }
        public int MaxDLTTransactionReceiptIdSize { get; }
        public int MaxMemoSize { get; }
        public string? JurisdictionDescription { get; }
        public string? dAccountingServiceChartOfAccountTemplateId { get; }
        public string? JurisdictionChartOfAccountTemplateId { get; }
        public string? JurisdictionMemberChartOfAccountTemplateId { get; }
        public string? JurisdictionBaseCurrencySymbol { get; }
        public int JurisdictionBaseCurrencyDecimals { get; }
        public decimal JurisdictionBuyerTransactionFeeRate { get; }
        public ulong JurisdictionBuyerTransactionFeeMinimum { get; }
        public ulong JurisdictionBuyerTransactionFeeMaximum { get; }
        public decimal JurisdictionSellerTransactionFeeRate { get; }
        public ulong JurisdictionSellerTransactionFeeMinimum { get; }
        public ulong JurisdictionSellerTransactionFeeMaximum { get; }
        public decimal JurisdictionMemberCashInFeeRate { get; }
        public ulong JurisdictionMemberCashInFeeMinimum { get; }
        public ulong JurisdictionMemberCashInFeeMaximum { get; }
        public decimal JurisdictionMemberCashOutFeeRate { get; }
        public ulong JurisdictionMemberCashOutFeeMinimum { get; }
        public ulong JurisdictionMemberCashOutFeeMaximum { get; }
        public decimal JurisdictionCashOutFeeRate { get; }
        public ulong JurisdictionCashOutFeeMinimum { get; }
        public ulong JurisdictionCashOutFeeMaximum { get; }
        public string? TestMemberId1 { get; }
        public string? TestMemberId2 { get; }
        public DLTAddress? TestMemberCryptoAddress1 { get; }
        public DLTAddress? TestMemberCryptoAddress2 { get; }
        public DLTAddress? dAccountingServiceCryptoAddress { get; }
        public string? PersistenceStorePath { get; }
        #endregion

        #region Helpers
        #endregion
    }
}
