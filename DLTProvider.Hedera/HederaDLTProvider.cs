using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hashgraph;
using dAccounting.Common.Interfaces;
using dAccounting.Common.Models;
using System.Diagnostics;
using DLTProviders.Hedera.Hedera;

namespace DLTProviders.Hedera
{
    public class HederaDLTProvider : IDLTProvider
    {
        #region Field Members
        private HederaManager manager = null!;
        private IAccountingContainer accountingContainer = null!;
        private IdGeneralLedgerServiceParameters glParameters = null!;
        #endregion

        #region Constructors
        public HederaDLTProvider( ISecretVault mastersecretvault, IdGeneralLedgerServiceParameters glparams, IAccountingContainer accountingcontainer, IDLTConfig hederaconfig )
        {
            glParameters = glparams;
            accountingContainer = accountingcontainer;
            manager = new HederaManager(mastersecretvault, hederaconfig);
        }
        #endregion

        #region Public Interface
        public bool IsValidDLTKeys(string publickey, string privatekey)
        {
            bool result = true;
            try
            {
                var validKeys = new HederaDLTKeyPair(publickey, privatekey);
            }
            catch (Exception)
            {

                result = false;
            }
            return result;
        }

        public bool IsValidDLTAddress(string address)
        {
            bool result = true;
            try
            {
                var validAddress = HederaManager.ParseAddress(address);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public async Task<IDLTTransactionConfirmation?> CreateContractAsync(IDLTAddress payorAddress)
        {
            IDLTTransactionConfirmation? trxConfirm = null!;
            try
            {
                var createContractRecord = await manager.CreateContractAsync(   HederaManager.ParseAddress(payorAddress.AddressID),
                                                                                HederaManager.CreateEndorsement(new HederaDLTKeyPair(payorAddress.KeyVault!.Base64EncryptedPublicKey!, null!)),
                                                                                HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, payorAddress.KeyVault!.Base64EncryptedPrivateKey!))
                                                              ).ConfigureAwait(false);
                if( createContractRecord == null || createContractRecord.Status != ResponseCode.Success )
                {
                    throw new InvalidOperationException("Unable to create DAGL smart contract.");
                }
                else
                {
                    trxConfirm = ConvertCreateContractRecordToDLTTransactionConfirmation(createContractRecord);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return trxConfirm;
        }


        public async Task<IDLTAddress?> CreateTokenAccountAsync(    IDLTAddress payorAddress, IDLTToken token, IDLTAddress treasury /*, IKeyVault kycKeyVault*/ )
        {
            IDLTAddress tokenAccountAddress = null!;
            try
            {
                var tokenAcountReceipt = await manager.CreateTokenAccountAddressAsync(
                                    HederaManager.ParseAddress(payorAddress.AddressID),
                                    HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, payorAddress.KeyVault!.Base64EncryptedPrivateKey!)),
                                    HederaManager.ParseAddress(token.Address!.AddressID),
                                    HederaManager.ParseAddress(treasury.AddressID),
                                    HederaManager.CreateEndorsement(new HederaDLTKeyPair(treasury.KeyVault!.Base64EncryptedPublicKey!, null!)),
                                    HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, treasury.KeyVault!.Base64EncryptedPrivateKey!))//,
                                    //HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, kycKeyVault!.Base64EncryptedPrivateKey!))
                                    ).ConfigureAwait(false);
                if (tokenAcountReceipt != null && tokenAcountReceipt.Status == ResponseCode.Success)
                {
                    tokenAccountAddress = new DLTAddress($"{tokenAcountReceipt.Address.ShardNum}.{tokenAcountReceipt.Address.RealmNum}.{tokenAcountReceipt.Address.AccountNum}", 
                        treasury.KeyVault, tokenAcountReceipt.Status.ToString());
                }
                else
                {
                    throw new InvalidOperationException("Error ");
                }

            }
            catch (Exception)
            {

                throw;
            }



            return await Task.FromResult(tokenAccountAddress);
        }


        public async Task<IDLTAddress?> CreateCryptoAccountAsync(IDLTAddress payorAddress )
        {
            IDLTAddress? accountAddress = null!;
            try
            {
                #region Generate keys for new account
                var acckeys = new HederaDLTKeyPair();
                var accKeyVault = new KeyVault(acckeys.Base64EncryptedPublicKeyString, acckeys.Base64EncryptedPrivateKeyString);
                acckeys.Clear();
                #endregion
                var accountReceipt = await manager.CreateAccountAddressAsync(HederaManager.ParseAddress(payorAddress.AddressID),
                                    HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, payorAddress.KeyVault!.Base64EncryptedPrivateKey!)),
                                    HederaManager.CreateEndorsement(new HederaDLTKeyPair(accKeyVault.Base64EncryptedPublicKey!, null!))).ConfigureAwait(false);
                if(accountReceipt != null  && accountReceipt.Status == ResponseCode.Success  )
                {
                    accountAddress = new DLTAddress($"{accountReceipt.Address.ShardNum}.{accountReceipt.Address.RealmNum}.{accountReceipt.Address.AccountNum}", 
                                                        accKeyVault, accountReceipt.Status.ToString());
                }
                else
                {
                    throw new InvalidOperationException("Error creating crypto account.");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return await Task.FromResult(accountAddress);
        }



        public async Task<ulong?> GetCryptoAccountBalanceAsync(IDLTAddress account)
        {
            ulong? balance = null!;
            try
            {
                balance = await manager.GetAccountCryptoBalanceAsync(HederaManager.ParseAddress(account.AddressID)).ConfigureAwait(false);                
            }
            catch (Exception)
            {
                // NOP
            }
            return await Task.FromResult(balance);
        }



        public async Task<ulong?> GetTokenAccountBalanceAsync( IDLTToken token, IDLTAddress account)
        {
            //var x = accountingContainer.InstanceManifest?.JurisdictionTokenTreasuryAddress!;
            //var y = new HederaDLTKeyPair(x.KeyVault?.Base64EncryptedPublicKey!, x.KeyVault?.Base64EncryptedPrivateKey!).PublicKeyString;
            //var z = new HederaDLTKeyPair(x.KeyVault?.Base64EncryptedPublicKey!, x.KeyVault?.Base64EncryptedPrivateKey!).PrivateKeyString;
            ulong? balance = null!;
            try
            {
                balance = await manager.GetAccountTokenBalanceAsync(HederaManager.ParseAddress(account.AddressID), HederaManager.ParseAddress(token.Address!.AddressID)).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // NOP
            }
            return await Task.FromResult(balance);
        }


        public async Task<IDLTTransactionConfirmation?> SimpleCryptoTransferAsync(IDLTAddress payor, IDLTAddress fromAccount, IDLTAddress toAccount, long amount, IKeyVault signatory = null!)
        {
            IDLTTransactionConfirmation? trxConfirmation = null!;
            try
            {
                Signatory[] signatories = null!;
                if(signatory == null!)
                {
                    // If we make it here then only the payor is the signatory
                    signatories = new Signatory[1];
                    signatories[0] = HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, payor.KeyVault!.Base64EncryptedPrivateKey!));
                }
                else
                {
                    // A Signatory was specified so must sign with both the payor and the signatory
                    signatories = new Signatory[2];
                    signatories[0] = HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, payor.KeyVault!.Base64EncryptedPrivateKey!));
                    signatories[1] = HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, signatory.Base64EncryptedPrivateKey!));
                }
                var trxReceipt = await manager.SimpleCryptoTransferAsync(
                                        HederaManager.ParseAddress(payor.AddressID),
                                        signatories,
                                        HederaManager.ParseAddress(fromAccount.AddressID),
                                        HederaManager.ParseAddress(toAccount.AddressID),
                                        amount
                                        ).ConfigureAwait(false);
                if (trxReceipt != null)
                {
                    trxConfirmation = ConvertTransactionReceiptToDLTTransactionConfirmation(trxReceipt);
                }
                else
                {
                    throw new InvalidOperationException("");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error in HederaDLTProvider.SimpleCryptoTransferAsync() - {ex}");
            }
            return await Task.FromResult(trxConfirmation);
        }


        public async Task<IDLTTransactionConfirmation?> SimpleTokenTransferAsync(IDLTAddress payor, IDLTToken token, IDLTAddress fromAccount, IDLTAddress toAccount, long amount )
        {
            IDLTTransactionConfirmation? trxConfirmation = null!;
            try
            {
                var trxReceipt = await manager.SimpleTokenTransferAsync(
                                        HederaManager.ParseAddress(payor.AddressID),
                                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, payor.KeyVault!.Base64EncryptedPrivateKey!)),
                                        HederaManager.ParseAddress(token.Address!.AddressID),
                                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, fromAccount.KeyVault!.Base64EncryptedPrivateKey!)),
                                        HederaManager.ParseAddress(fromAccount.AddressID),
                                        HederaManager.ParseAddress(toAccount.AddressID),
                                        amount
                                        ).ConfigureAwait(false);
                if( trxReceipt != null )
                {
                    trxConfirmation = ConvertTransactionReceiptToDLTTransactionConfirmation(trxReceipt);
                }
                else
                {
                    throw new InvalidOperationException("");
                }
            }
            catch (Exception)
            {
                // NOP
            }
            return await Task.FromResult(trxConfirmation);
        }


        public async Task<IDLTTransactionConfirmation?> SmartContractJournalEntryTokenTransferAsync( JournalEntryRecord journalEntryRecord,
                                                                                                     bool isReversing,
                                                                                                     bool clearaccounts = false
                                                                                                   )
        {
            IDLTTransactionConfirmation? trxConfirmation = null!;
            try
            {
                var trxRecords = await manager.SmartContractJournalEntryTokenTransferAsync( 
                                        HederaManager.ParseAddress(accountingContainer.InstanceManifest?.JursidictionJournalEntryPostContractAddress?.AddressID!),
                                        HederaManager.ParseAddress(accountingContainer.InstanceManifest?.JurisdictionPayorAddress?.AddressID!),
                                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, accountingContainer.InstanceManifest?.JurisdictionPayorAddress?.KeyVault!.Base64EncryptedPrivateKey!)),
                                        HederaManager.ParseAddress(accountingContainer.InstanceManifest?.JurisdictionToken?.Address!.AddressID),
                                        HederaManager.ParseAddress(accountingContainer.InstanceManifest?.JurisdictionTokenTreasuryAddress?.AddressID),
                                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, accountingContainer.InstanceManifest?.JurisdictionTokenTreasuryAddress?.KeyVault!.Base64EncryptedPrivateKey!)),
                                        journalEntryRecord,
                                        glParameters.MaxNumberOfAtomicJournalEntrySwapsInSingleTransaction,
                                        isReversing,
                                        clearaccounts
                                                                       ).ConfigureAwait(false);
                if (trxRecords != null)
                {
                    if (trxRecords.Count > 0)
                    {
                        var callContractRecord = (CallContractRecord)trxRecords[0];
                        var result = (ResponseCode)(callContractRecord!.CallResult!.Result!.As<int>());
                        if (result != ResponseCode.Success)
                        {
                            throw new InvalidOperationException($"Contract call failed with status: {result}");
                        }
                        else
                        {
                            trxConfirmation = ConvertCallContractRecordToDLTTransactionConfirmation(callContractRecord);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Contract record results are empty");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Failed to create journal entry for unknown reason.");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error in HederaDLTProvider.SmartContractJournalEntryTokenTransferAsync() - {ex}");
            }
            return await Task.FromResult(trxConfirmation);
        }


        public async Task<IDLTTransactionConfirmation?> JournalEntryTokenTransferAsync( JournalEntryRecord journalEntryRecord, 
                                                                                        //bool isReversing,
                                                                                        IDLTAddress externalAccount = null!,
                                                                                        bool transferOutToExternalAccount = false, 
                                                                                        long externalAmount = 0, 
                                                                                        object signatory = null!,
                                                                                        bool clearaccounts = false 
                                                                                      )
        {
            IDLTTransactionConfirmation? trxConfirmation = null!;
            try
            {
                var trxReceipt = await manager.JournalEntryTokenTransferAsync(
                                        HederaManager.ParseAddress(accountingContainer.InstanceManifest?.JurisdictionPayorAddress?.AddressID!),
                                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, accountingContainer.InstanceManifest?.JurisdictionPayorAddress?.KeyVault!.Base64EncryptedPrivateKey!)),
                                        HederaManager.ParseAddress(accountingContainer.InstanceManifest?.JurisdictionToken?.Address!.AddressID),
                                        HederaManager.ParseAddress(accountingContainer.InstanceManifest?.JurisdictionTokenTreasuryAddress?.AddressID ),
                                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, accountingContainer.InstanceManifest?.JurisdictionTokenTreasuryAddress?.KeyVault!.Base64EncryptedPrivateKey!)),
                                        journalEntryRecord,
                                        //glParameters.MaxNumberOfAtomicJournalEntrySwapsInSingleTransaction,
                                        //isReversing,
                                        (externalAccount != null ? HederaManager.ParseAddress(externalAccount.AddressID) : null!),
                                        (externalAccount != null ? ( signatory != null ? (Signatory)signatory : HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, externalAccount.KeyVault!.Base64EncryptedPrivateKey!))) : null!),
                                        transferOutToExternalAccount,
                                        externalAmount,                                        
                                        clearaccounts
                                                                       ).ConfigureAwait(false);
                if (trxReceipt != null)
                {
                    if (trxReceipt.Status != ResponseCode.Success)
                    {
                        throw new InvalidOperationException($"Failed to create journal entry - reason: {trxReceipt.Status.ToString()}.");
                    }
                    else
                    {
                        trxConfirmation = ConvertTransactionReceiptToDLTTransactionConfirmation(trxReceipt);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Failed to create journal entry for unknown reason.");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error in HederaDLTProvider.JournalEntryTokenTransferAsync() - {ex}");
            }
            return await Task.FromResult(trxConfirmation);
        }


        public IKeyVault CreateKeys(bool _public, bool _private)
        {
            KeyVault keyVault = null!;
            if( !_public && !_private)
            {
                throw new ArgumentException("Must specifiy to create at least one of the public or private keys.");
            }
            var dltKey = manager.GenerateDLTKey();
            if (_public && !_private)
            {
                keyVault = new KeyVault(dltKey.Base64EncryptedPublicKeyString, null!);
            }
            else if( !_public && _private)
            {
                keyVault = new KeyVault(null!, dltKey.Base64EncryptedPrivateKeyString);
            }
            else
            {
                keyVault = new KeyVault(dltKey.Base64EncryptedPublicKeyString, dltKey.Base64EncryptedPrivateKeyString);
            }
            return keyVault;
        }


        public IKeyVault LoadKeys(string base64EncryptedPublicKey, string base64EncryptedPrivateKey)
        {
            KeyVault keyVault = null!;
            if (string.IsNullOrEmpty(base64EncryptedPublicKey) && string.IsNullOrEmpty(base64EncryptedPrivateKey))
            {
                throw new ArgumentException("Must specifiy to create at least one of the public or private keys.");
            }
            var dltKey = manager.LoadDLTKey( base64EncryptedPublicKey, base64EncryptedPrivateKey);
            if (!string.IsNullOrEmpty(base64EncryptedPublicKey) && string.IsNullOrEmpty(base64EncryptedPrivateKey))
            {
                keyVault = new KeyVault(dltKey.Base64EncryptedPublicKeyString, null!);
            }
            else if (string.IsNullOrEmpty(base64EncryptedPublicKey) && !string.IsNullOrEmpty(base64EncryptedPrivateKey))
            {
                keyVault = new KeyVault(null!, dltKey.Base64EncryptedPrivateKeyString);
            }
            else
            {
                keyVault = new KeyVault(dltKey.Base64EncryptedPublicKeyString, dltKey.Base64EncryptedPrivateKeyString);
            }
            return keyVault;
        }


        public async Task<IDLTToken> CreateTokenAsync(
                                                        IDLTAddress payorAddress,                      // pays for the transaction as well as token account renewals
                                                        IDLTAddress jurisditionTokenTreasuryAddress,   // token treasure as well as a DLT account that holds the jurisdiction "reserves" in esgrow
                                                        //string currency,
                                                        IKeyVault tokenAdminKeyVault
                                                        //IKeyVault tokenGrantKycKeyVault,
                                                        //IKeyVault tokenSuspendKeyVault,
                                                        //IKeyVault tokenConfiscateKeyVault,
                                                        //IKeyVault tokenSupplyKeyVault
                                                     )
        {
            IDLTToken token = null!;
            try
            {
                string currency = "DAGL";
                var hederatoken = await manager.CreateTokenAsync(
                        HederaManager.ParseAddress(payorAddress.AddressID),
                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, payorAddress.KeyVault!.Base64EncryptedPrivateKey!)),
                        //new Signatory(new HederaDLTKeyPair(null!, payorAddress.KeyVault!.Base64EncryptedPrivateKey!).PrivateHederaKeyBytes),
                        HederaManager.ParseAddress(jurisditionTokenTreasuryAddress.AddressID),
                        //HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, tokenAdminKeyVault.Base64EncryptedPrivateKey!)),
                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, jurisditionTokenTreasuryAddress.KeyVault!.Base64EncryptedPrivateKey!)),
                        //HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, tokenAdminKeyVault.Base64EncryptedPrivateKey!)),
                        currency,
                        HederaManager.CreateSignatory(new HederaDLTKeyPair(null!, tokenAdminKeyVault.Base64EncryptedPrivateKey!)),
                        HederaManager.CreateEndorsement(new HederaDLTKeyPair(tokenAdminKeyVault.Base64EncryptedPublicKey!, null!))
                        //null!,  // no kyc
                        //HederaManager.CreateEndorsement(new HederaDLTKeyPair(tokenGrantKycKeyVault.Base64EncryptedPublicKey!, null!)),
                        //HederaManager.CreateEndorsement(new HederaDLTKeyPair(tokenSuspendKeyVault.Base64EncryptedPublicKey!, null!)),
                        //HederaManager.CreateEndorsement(new HederaDLTKeyPair(tokenConfiscateKeyVault.Base64EncryptedPublicKey!, null!)),
                        //HederaManager.CreateEndorsement(new HederaDLTKeyPair(tokenSupplyKeyVault.Base64EncryptedPublicKey!, null!))
                                                                ).ConfigureAwait(false);
                if (hederatoken != null)
                {
                    if (hederatoken.Status == ResponseCode.Success)
                    {
                        token = new DLTToken(
                                                currency, 
                                                new DLTAddress($"{hederatoken.Token.ShardNum}.{hederatoken.Token.RealmNum}.{hederatoken.Token.AccountNum}"), 
                                                hederatoken.Status.ToString()
                                            );
                    }
                    else
                    {
                        token = new DLTToken(currency, null!, hederatoken.Status.ToString());
                    }
                }
                else
                {
                    throw new InvalidOperationException("Error calling HederaManager.CreateTokenAsync()");
                }

            }
            catch (Exception ex)
            {
                Debug.Print($"Error in HederaDLTProvider.CreateTokenAsync() - {ex}");
                // NOP
            }
            return await Task.FromResult(token);
        }

        #endregion

        #region Helpers
        private DLTTransactionConfirmation ConvertTransactionReceiptToDLTTransactionConfirmation( TransactionReceipt trxReceipt)
        {
            DLTTransactionConfirmation trxConfirm = new DLTTransactionConfirmation(trxReceipt.Status.ToString(), trxReceipt.Id.Address.ToString())
            {
                ExchangeRateUSDCentEquivalent = trxReceipt.CurrentExchangeRate!.USDCentEquivalent,
                NextExchangeRateUSDCentEquivalent = trxReceipt.NextExchangeRate!.USDCentEquivalent,
                ExchangeRateNativeCryptoEquivalent = trxReceipt.CurrentExchangeRate!.HBarEquivalent,
                NextExchangeRateNativeCryptoEquivalent = trxReceipt.NextExchangeRate!.HBarEquivalent,
                ExchangeRateExpiration = trxReceipt.CurrentExchangeRate.Expiration,
                NextExchangeRateExpiration = trxReceipt.NextExchangeRate.Expiration,
                IsPending = (trxReceipt.Pending != null),
                TransactionTimeNanoSeconds = trxReceipt.Id.ValidStartNanos,
                TransactionTimeSeconds = trxReceipt.Id.ValidStartSeconds
                
            };

            return trxConfirm;

        }

        private DLTTransactionConfirmation ConvertCreateContractRecordToDLTTransactionConfirmation(CreateContractRecord createContractRecord )
        {
            DLTTransactionConfirmation trxConfirm = new DLTTransactionConfirmation(createContractRecord.Status.ToString(), createContractRecord.Id.Address.ToString())
            {
                ExchangeRateUSDCentEquivalent = createContractRecord.CurrentExchangeRate!.USDCentEquivalent,
                NextExchangeRateUSDCentEquivalent = createContractRecord.NextExchangeRate!.USDCentEquivalent,
                ExchangeRateNativeCryptoEquivalent = createContractRecord.CurrentExchangeRate!.HBarEquivalent,
                NextExchangeRateNativeCryptoEquivalent = createContractRecord.NextExchangeRate!.HBarEquivalent,
                ExchangeRateExpiration = createContractRecord.CurrentExchangeRate.Expiration,
                NextExchangeRateExpiration = createContractRecord.NextExchangeRate.Expiration,
                IsPending = (createContractRecord.Pending != null),
                TransactionTimeNanoSeconds = createContractRecord.Id.ValidStartNanos,
                TransactionTimeSeconds = createContractRecord.Id.ValidStartSeconds,
                ContractId = new DLTAddress($"{createContractRecord.Contract.ShardNum}.{createContractRecord.Contract.RealmNum}.{createContractRecord.Contract.AccountNum}")
            };

            return trxConfirm;

        }

        private DLTTransactionConfirmation ConvertCallContractRecordToDLTTransactionConfirmation(CallContractRecord callContractRecord)
        {
            ResponseCode result = (ResponseCode)(callContractRecord!.CallResult!.Result!.As<int>());
            DLTTransactionConfirmation trxConfirm = new DLTTransactionConfirmation(result.ToString(), callContractRecord.Id.Address.ToString())
            {
                ExchangeRateUSDCentEquivalent = callContractRecord.CurrentExchangeRate!.USDCentEquivalent,
                NextExchangeRateUSDCentEquivalent = callContractRecord.NextExchangeRate!.USDCentEquivalent,
                ExchangeRateNativeCryptoEquivalent = callContractRecord.CurrentExchangeRate!.HBarEquivalent,
                NextExchangeRateNativeCryptoEquivalent = callContractRecord.NextExchangeRate!.HBarEquivalent,
                ExchangeRateExpiration = callContractRecord.CurrentExchangeRate.Expiration,
                NextExchangeRateExpiration = callContractRecord.NextExchangeRate.Expiration,
                IsPending = (callContractRecord.Pending != null),
                TransactionTimeNanoSeconds = callContractRecord.Id.ValidStartNanos,
                TransactionTimeSeconds = callContractRecord.Id.ValidStartSeconds,
            };

            return trxConfirm;

        }
        #endregion
    }
}
