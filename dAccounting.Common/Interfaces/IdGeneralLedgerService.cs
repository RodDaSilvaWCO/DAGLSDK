using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;


namespace dAccounting.Common.Interfaces
{
    public interface IdGeneralLedgerService
    {
        #region Offical API
        Task<dAccountingServiceInstanceManifest> GenerateInstanceManifestAsync(string dAccountingServiceID,
                                                                                   string jurisdictionID,
                                                                                   DLTAddress dAccountingServiceCryptoAddress,
                                                                                   string jurisdictionDescription,
                                                                                   string jurisdictionpayorAddress,
                                                                                   string jurisdictionpayorBase64EncryptedPublicKey,
                                                                                   string jurisdictionpayorBase64EncryptedPrivateKey,
                                                                                   string daccountingservicecoatemplateId,
                                                                                   string jurisdictioncoatemplateId,
                                                                                   string jurisdictionmembercoatemplateId,
                                                                                   DLTCurrency jurisdictiondefaultaccountingcurrency,
                                                                                   FeeSchedule jurisdictionservicebuyertransactionFeeSchedule,
                                                                                   FeeSchedule jurisdictionservicesellertransactionFeeSchedule,
                                                                                   FeeSchedule jurisdictionservicemembercashinFeeSchedule,
                                                                                   FeeSchedule jurisdictionservicemembercashoutFeeSchedule,
                                                                                   FeeSchedule jurisdictionservicecashoutFeeSchedule,
                                                                                   DLTAddress testMember1CryptoAddress,
                                                                                   DLTAddress testMember2CryptoAddress,
                                                                                   string testMember1ID,
                                                                                   string testMember2ID
                                                                              );

        bool IsDAGLInitialized();

        Task<IJurisdictionMember> RegisterJurisdictionMemeber(string? jurisdictionmemberid, IDLTAddress membercryptoaddress);

        ulong NetPurchaseAmountIncludingFees(ulong grossAmount);

        Task<long> GetJurisdictionMemberWalletBalanceAsync( string jurisdictionmemberid );

        Task<ulong> GetJurisdictionDltBalanceAsync();

        Task<JurisdictionMemberGeneralLedger> GetMemberGeneralLedgerAsync(string jurisdictionMemberId);

        //Task<JurisdictionMemberGeneralLedger> GetJurisdictionGeneralLedgerAsync();

        Task<string> GetMemberChartOfAccountsAsync(string jurisdictionmemberid);

        //Task<string> GetJurisdictionChartOfAccountsAsync(string jurisdictionmemberid);
        #endregion


        Task<IDLTTransactionConfirmation> PostSynchronousMemberPurchaseAsync(   string buyerMemberId, 
                                                                                string sellerMemberId, 
                                                                                ulong? amount
                                                                            );


        Task<IDLTTransactionConfirmation> ResetAsync( string buyerMemberId, string sellerMemberId );


        Task<ITrialBalance> GetMemberTrialBalanceAsync(IJurisdictionMemberGeneralLedger jurisdictionMemberGeneralLedger);

        //Task<ITrialBalance> GetJurisdictionTrialBalanceAsync();

        //Task<ITrialBalance> GetdAccountingServiceTrialBalanceAsync();

        Task<List<IJournalEntryRecord>> GetMemberJournalEntriesAsync(IJurisdictionMemberGeneralLedger jurisdictionMemberGeneralLedger);


        Task<List<IJournalEntryRecord>> GetJurisdictionJournalEntriesAsync();

        Task<List<IJournalEntryRecord>> GetJurisdictionAuditAsync();

        Task<List<IJournalEntryRecord>> GetdAccountingServiceJournalEntriesAsync();




        Task<bool> UnregisterJurisdictionMemeber(IJurisdictionMember member);





        Task<IDLTTransactionConfirmation> FundWalletAsync(  string stakeholderid,
                                                            IDLTAddress fundingAccount,
                                                            ulong? amount,
                                                            object signatory = null!);


        Task<IDLTTransactionConfirmation> CashOutSellerWalletAsync(IJurisdictionMemberGeneralLedger jurisdictionMemberGeneralLedgerToCashOut, IDLTAddress sellerAccount, ulong? amount);


        Task<IDLTTransactionConfirmation> CashOutJurisdictionWalletAsync( ulong? amount);


        Task<IDLTAddress?> CreateCryptoAccountAsync();


        Task<IDLTTransactionConfirmation> TransferCryptoAsync( IDLTAddress receivingAccount, long amount);

    }
}
