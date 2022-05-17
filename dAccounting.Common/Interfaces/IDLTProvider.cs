using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IDLTProvider
    {
        bool IsValidDLTKeys(string publickey, string privatekey);
        bool IsValidDLTAddress( string address);
        Task<IDLTAddress?> CreateCryptoAccountAsync(IDLTAddress payorAddress);

        Task<IDLTAddress?> CreateTokenAccountAsync(IDLTAddress payorAddress, IDLTToken token, IDLTAddress treasury /*, IKeyVault kycKeyVault*/);

        IKeyVault CreateKeys( bool _public, bool _private );

        IKeyVault LoadKeys(string base64EncryptedPublicKey, string base64EncryptedPrivateKey);


        Task<IDLTToken> CreateTokenAsync( 
                IDLTAddress payorAddress,                      // pays for the transaction as well as token account renewals
                IDLTAddress jurisditionTreasuryAddress,        // token treasure as well as a DLT account that holds the jurisdiction "reserves" in esgrow
                //string currency,
                IKeyVault tokenAdminPublicKey
                //IKeyVault tokenGrantKycPublicKey,
                //IKeyVault tokenSuspendPublicKey,
                //IKeyVault tokenConfiscatePublicKey,
                //IKeyVault tokenSupplyPublicKey
                                                );

        Task<IDLTTransactionConfirmation?> CreateContractAsync(IDLTAddress payorAddress);
                                                                       


        Task<ulong?> GetCryptoAccountBalanceAsync(IDLTAddress account);

        Task<ulong?> GetTokenAccountBalanceAsync(IDLTToken token, IDLTAddress account );

        Task<IDLTTransactionConfirmation?> SimpleTokenTransferAsync(IDLTAddress payor, IDLTToken token, IDLTAddress fromAccount, IDLTAddress toAccount, long amount);

        Task<IDLTTransactionConfirmation?> SimpleCryptoTransferAsync(IDLTAddress payor, IDLTAddress fromAccount, IDLTAddress toAccount, long amount, IKeyVault signatoryKeys = null!);

        Task<IDLTTransactionConfirmation?> JournalEntryTokenTransferAsync(  JournalEntryRecord journalEntryRecord,
                                                                            //bool isReversing,
                                                                            IDLTAddress externalCryptoAddress = null!,
                                                                            bool transferOutToExternalAccount = false, 
                                                                            long externalCryptoAmount = 0, 
                                                                            object signatory = null!,
                                                                            bool clearaccounts = false );

        Task<IDLTTransactionConfirmation?> SmartContractJournalEntryTokenTransferAsync( JournalEntryRecord journalEntryRecord,
                                                                                        bool isReversing,
                                                                                        bool clearaccounts = false
                                                                                      );

    }
}
