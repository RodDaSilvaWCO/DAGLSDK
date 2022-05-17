using Hashgraph;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;
using dAccounting.Common.Models;
using dAccounting.Common.Utilities;
using System.Text;
using System.Collections.ObjectModel;
//using UnoSys.Core;
//using WorldComputer.Api.Configuration;
//using WorldComputer.DAL;

namespace DLTProviders.Hedera.Hedera
{
    public class HederaManager
    {
        // NOTE:  This API should not contain any references to dAccountingService.Common types except for ISecretVault and IDLTConfig -
        //        only HederaManager and Hashgraph types.
        #region Members
        const int DAGL_FILE_CHUNK_SIZE = 1024 * 4;
        const int SOLIDITY_ADDRESS_SIZE = 20;  // aka 160-bits
        const string DAGL_SMART_CONTRACT_CONTRACT_BYTECODE = "608060405234801561001057600080fd5b5061148b806100206000396000f3fe608060405234801561001057600080fd5b506004361061002b5760003560e01c8063110d0dcd14610030575b600080fd5b61004a60048036038101906100459190610b1f565b610060565b6040516100579190610db3565b60405180910390f35b60008061006e8360386109bd565b67ffffffffffffffff16905060006100878460306109bd565b67ffffffffffffffff16905060006002836100a29190610f99565b67ffffffffffffffff1667ffffffffffffffff8111156100c5576100c461136f565b5b6040519080825280602002602001820160405280156100f35781602001602082028036833780820191505090505b50905060006002846101059190610f99565b67ffffffffffffffff1667ffffffffffffffff8111156101285761012761136f565b5b6040519080825280602002602001820160405280156101565781602001602082028036833780820191505090505b509050610164866000610a24565b8260008151811061017857610177611340565b5b602002602001019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff16815250506101bd866014610a24565b826001815181106101d1576101d0611340565b5b602002602001019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff16815250506102168660286109bd565b8160008151811061022a57610229611340565b5b602002602001019060070b908160070b8152505060008160018151811061025457610253611340565b5b602002602001019060070b908160070b8152505060006040905060028260008151811061028457610283611340565b5b602002602001015160070b12156102a65760a360030b955050505050506109b8565b60005b85811015610382576102bb8883610a24565b846002836102c99190610f99565b815181106102da576102d9611340565b5b602002602001019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff16815250506014826103219190610f99565b915061032d88836109bd565b8360028361033b9190610f99565b8151811061034c5761034b611340565b5b602002602001019060070b908160070b8152505060088261036d9190610f99565b9150808061037a90611268565b9150506102a9565b5060006001808460008151811061039c5761039b611340565b5b602002602001015167ffffffffffffffff166103b89190611161565b6001886103c59190611161565b6103cf91906112b1565b6103d99190610f99565b905060006001846000815181106103f3576103f2611340565b5b602002602001015167ffffffffffffffff1661040f9190611161565b8761041a9190610fef565b90508660018560008151811061043357610432611340565b5b602002602001015167ffffffffffffffff1661044f9190611161565b8261045a9190611107565b101561046f57808061046b90611268565b9150505b60006002905060005b828110156109a75760008660018151811061049657610495611340565b5b602002602001019060070b908160070b8152505060006001846104b99190611161565b82146104ea57866000815181106104d3576104d2611340565b5b602002602001015167ffffffffffffffff166104f8565b6001856104f79190610f99565b5b905060008167ffffffffffffffff8111156105165761051561136f565b5b6040519080825280602002602001820160405280156105445781602001602082028036833780820191505090505b50905060008267ffffffffffffffff8111156105635761056261136f565b5b6040519080825280602002602001820160405280156105915781602001602082028036833780820191505090505b50905060005b6001846105a49190611161565b8110156107035760018c146105ba5760016105dc565b7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff5b8a87815181106105ef576105ee611340565b5b60200260200101516106019190611020565b82828151811061061457610613611340565b5b602002602001019060070b908160070b815250508a868151811061063b5761063a611340565b5b602002602001015183828151811061065657610655611340565b5b602002602001019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff16815250508986815181106106a3576106a2611340565b5b60200260200101518a6001815181106106bf576106be611340565b5b602002602001018181516106d39190610f1d565b91509060070b908160070b8152505085806106ed90611268565b96505080806106fb90611268565b915050610597565b8a60018151811061071757610716611340565b5b602002602001015183828151811061073257610731611340565b5b602002602001019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff168152505060018c1461079a577fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff61079d565b60015b8a6001815181106107b1576107b0611340565b5b60200260200101516107c39190611020565b8282815181106107d6576107d5611340565b5b602002602001019060070b908160070b8152505060008061016773ffffffffffffffffffffffffffffffffffffffff166382bba49360e01b8e60008151811061082257610821611340565b5b6020026020010151878760405160240161083e93929190610d6e565b604051602081830303815290604052907bffffffffffffffffffffffffffffffffffffffffffffffffffffffff19166020820180517bffffffffffffffffffffffffffffffffffffffffffffffffffffffff83818316178352505050506040516108a89190610d57565b6000604051808303816000865af19150503d80600081146108e5576040519150601f19603f3d011682016040523d82523d6000602084013e6108ea565b606091505b5091509150601660030b82610900576015610915565b818060200190518101906109149190610b68565b5b60030b1461098e576000871415610953578080602001905181019061093a9190610b68565b60030b9f505050505050505050505050505050506109b8565b6040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161098590610e0e565b60405180910390fd5b505050505050808061099f90611268565b915050610478565b50601660030b985050505050505050505b919050565b60006008826109cc9190610f99565b83511015610a0f576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610a0690610dce565b60405180910390fd5b60008260088501015190508091505092915050565b6000601482610a339190610f99565b83511015610a76576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610a6d90610dee565b60405180910390fd5b60006c01000000000000000000000000836020860101510490508091505092915050565b6000610aad610aa884610e53565b610e2e565b905082815260208101848484011115610ac957610ac86113a3565b5b610ad48482856111f5565b509392505050565b600082601f830112610af157610af061139e565b5b8135610b01848260208601610a9a565b91505092915050565b600081519050610b198161143e565b92915050565b600060208284031215610b3557610b346113ad565b5b600082013567ffffffffffffffff811115610b5357610b526113a8565b5b610b5f84828501610adc565b91505092915050565b600060208284031215610b7e57610b7d6113ad565b5b6000610b8c84828501610b0a565b91505092915050565b6000610ba18383610bc5565b60208301905092915050565b6000610bb98383610cdf565b60208301905092915050565b610bce81611195565b82525050565b610bdd81611195565b82525050565b6000610bee82610ea4565b610bf88185610edf565b9350610c0383610e84565b8060005b83811015610c34578151610c1b8882610b95565b9750610c2683610ec5565b925050600181019050610c07565b5085935050505092915050565b6000610c4c82610eaf565b610c568185610ef0565b9350610c6183610e94565b8060005b83811015610c92578151610c798882610bad565b9750610c8483610ed2565b925050600181019050610c65565b5085935050505092915050565b6000610caa82610eba565b610cb48185610f01565b9350610cc4818560208601611204565b80840191505092915050565b610cd9816111a7565b82525050565b610ce8816111be565b82525050565b6000610cfb601483610f0c565b9150610d06826113c3565b602082019050919050565b6000610d1e601583610f0c565b9150610d29826113ec565b602082019050919050565b6000610d41600f83610f0c565b9150610d4c82611415565b602082019050919050565b6000610d638284610c9f565b915081905092915050565b6000606082019050610d836000830186610bd4565b8181036020830152610d958185610be3565b90508181036040830152610da98184610c41565b9050949350505050565b6000602082019050610dc86000830184610cd0565b92915050565b60006020820190508181036000830152610de781610cee565b9050919050565b60006020820190508181036000830152610e0781610d11565b9050919050565b60006020820190508181036000830152610e2781610d34565b9050919050565b6000610e38610e49565b9050610e448282611237565b919050565b6000604051905090565b600067ffffffffffffffff821115610e6e57610e6d61136f565b5b610e77826113b2565b9050602081019050919050565b6000819050602082019050919050565b6000819050602082019050919050565b600081519050919050565b600081519050919050565b600081519050919050565b6000602082019050919050565b6000602082019050919050565b600082825260208201905092915050565b600082825260208201905092915050565b600081905092915050565b600082825260208201905092915050565b6000610f28826111be565b9150610f33836111be565b925081677fffffffffffffff03831360008312151615610f5657610f556112e2565b5b817fffffffffffffffffffffffffffffffffffffffffffffffff8000000000000000038312600083121615610f8e57610f8d6112e2565b5b828201905092915050565b6000610fa4826111eb565b9150610faf836111eb565b9250827fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff03821115610fe457610fe36112e2565b5b828201905092915050565b6000610ffa826111eb565b9150611005836111eb565b92508261101557611014611311565b5b828204905092915050565b600061102b826111be565b9150611036836111be565b925082677fffffffffffffff048211600084136000841316161561105d5761105c6112e2565b5b817fffffffffffffffffffffffffffffffffffffffffffffffff8000000000000000058312600084126000841316161561109a576110996112e2565b5b827fffffffffffffffffffffffffffffffffffffffffffffffff800000000000000005821260008413600084121616156110d7576110d66112e2565b5b82677fffffffffffffff05821260008412600084121616156110fc576110fb6112e2565b5b828202905092915050565b6000611112826111eb565b915061111d836111eb565b9250817fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff0483118215151615611156576111556112e2565b5b828202905092915050565b600061116c826111eb565b9150611177836111eb565b92508282101561118a576111896112e2565b5b828203905092915050565b60006111a0826111cb565b9050919050565b6000819050919050565b60008160030b9050919050565b60008160070b9050919050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000819050919050565b82818337600083830152505050565b60005b83811015611222578082015181840152602081019050611207565b83811115611231576000848401525b50505050565b611240826113b2565b810181811067ffffffffffffffff8211171561125f5761125e61136f565b5b80604052505050565b6000611273826111eb565b91507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8214156112a6576112a56112e2565b5b600182019050919050565b60006112bc826111eb565b91506112c7836111eb565b9250826112d7576112d6611311565b5b828206905092915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601260045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052603260045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b600080fd5b600080fd5b600080fd5b600080fd5b6000601f19601f8301169050919050565b7f746f55696e7436345f6f75744f66426f756e6473000000000000000000000000600082015250565b7f746f416464726573735f6f75744f66426f756e64730000000000000000000000600082015250565b7f5472616e73666572204661696c65640000000000000000000000000000000000600082015250565b611447816111b1565b811461145257600080fd5b5056fea264697066735822122058605d22aafff01316ba53fc2690af9e1b596e9f11c9089e2cf5ec29c081e6ce64736f6c63430008070033";
        static internal ISecretVault MasterSecretVault = null!;
        static internal IDLTConfig? HederaConfig;
        #endregion

        #region Constructor
        public HederaManager(ISecretVault mastersecretvault, IDLTConfig hederaconfig)
        {
            MasterSecretVault = mastersecretvault;
            HederaConfig = hederaconfig;
        }
        #endregion


        #region Public Static Interface
        public static Signatory CreateSignatory(HederaDLTKeyPair privateKey)
        {
            return new Signatory(privateKey.PrivateHederaKeyBytes);
        }

        public static Endorsement CreateEndorsement(HederaDLTKeyPair publicKey)
        {
            return new Endorsement(publicKey.PublicHederaKeyBytes);
        }

        //public static async Task<ulong> GetAccountHBARBalanceAsync(string accountAddress)
        //{
        //    return await GetAccountHBARBalanceAsync(ParseAddress(accountAddress));
        //}
        //public static async Task<ulong> GetAccountHBARBalanceAsync( Address accountAddress )
        //{
        //    try
        //    {
        //        await using var client = new Client(ctx =>
        //        {
        //            ctx.Gateway = DetermineGatewayToUse(HederaConfig.HeaderTestNetGateways);    // Random Hedera Gateway
        //        });
        //        var balances = await client.GetAccountBalancesAsync(accountAddress).ConfigureAwait(false);
        //        return balances.Crypto;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Print($"Error in WCOHederaManager.GetAccountHBARBalanceAsync() - {ex.ToString()}");
        //        throw;
        //    }
        //}


        //public static async Task<ulong> GetAccountATTNBalanceAsync(string accountAddress)
        //{
        //    return await GetAccountATTNBalanceAsync(ParseAddress( accountAddress) );
        //}

        //public static async Task<ulong[]> GetAccountBalancesAsync(string accountAddress)
        //{
        //    ulong[] twoBalances = new ulong[2];
        //    try
        //    {
        //        await using var client = new Client(ctx =>
        //        {
        //            ctx.Gateway = DetermineGatewayToUse(HederaConfig.HeaderTestNetGateways);    // Random Hedera Gateway
        //        });
        //        var balances = await client.GetAccountBalancesAsync(ParseAddress(accountAddress)).ConfigureAwait(false);
        //        twoBalances[0] = balances.Crypto;
        //        foreach (var tkn in balances.Tokens)
        //        {
        //            if (tkn.Key.AccountNum == TokenAddress.AccountNum)
        //            {
        //                twoBalances[1] = tkn.Value;
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Print($"Error in WCOHederaManager.GetAccountBalancesAsync() - {ex.ToString()}");
        //        throw;
        //    }
        //    return twoBalances;
        //}
        //public static async Task<ulong> GetAccountATTNBalanceAsync(Address accountAddress)
        //{
        //    try
        //    {
        //        await using var client = new Client(ctx =>
        //        {
        //            ctx.Gateway = DetermineGatewayToUse(HederaConfig.HeaderTestNetGateways);    // Random Hedera Gateway
        //        });
        //        var balances = await client.GetAccountBalancesAsync(accountAddress).ConfigureAwait(false);
        //        foreach (var tkn in balances.Tokens)
        //        {
        //            if (tkn.Key.AccountNum == TokenAddress.AccountNum)
        //            {
        //                return tkn.Value;
        //            }
        //        }
        //        throw new InvalidOperationException("Unkonwn token");
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Print($"Error in WCOHederaManager.GetAccountATTNBalanceAsync() - {ex.ToString()}");
        //        throw;
        //    }
        //}

        //public static async Task<TransactionReceipt> TransferATTNAsync(string senderAddress,  string receiverAddress, long amountToTransfer, string senderEncryptedPrivateKey )
        //{

        //    return await TransferATTNAsync(ParseAddress( senderAddress ), ParseAddress(receiverAddress), amountToTransfer, senderEncryptedPrivateKey).ConfigureAwait(false);
        //}


        //public static async Task<TransactionReceipt> TreasuryTransferATTNAsync( string subjectAddress, bool sendFromTreasury, long amountToTransfer, string senderEncryptedPrivateKey)
        //{
        //    if (sendFromTreasury)
        //    {
        //        // Send from Treasury to subjectAddress
        //        return await TransferATTNAsync(JurisdictionGLTreasuryAccountAddress, ParseAddress(subjectAddress), amountToTransfer, JurisdictionGLTreasurySignatory).ConfigureAwait(false);
        //    }
        //    else
        //    {
        //        // Send subjectAddress to Treasury
        //        return await TransferATTNAsync(ParseAddress(subjectAddress), JurisdictionGLTreasuryAccountAddress, amountToTransfer, senderEncryptedPrivateKey).ConfigureAwait(false);
        //    }
        //}

        //public static async Task<TransactionReceipt> TransferATTNAsync(Address senderAddress, Address receiverAddress, long amountToTransfer, string senderEncryptedPrivateKey )
        //{
        //    TransactionReceipt receipt = null!;
        //    try
        //    {
        //        await using var client = new Client(ctx =>
        //        {
        //            ctx.Gateway = DetermineGatewayToUse(HederaConfig.HeaderTestNetGateways);    // Random Hedera Gateway
        //            ctx.Payer = JurisdictionGLTreasuryAccountAddress;                                      // WCO Treasury pays for the transfer
        //            ctx.Signatory = JurisdictionGLTreasurySignatory;                                       // WCO Treasury authorizes the transfer
        //        });
        //        var keyPair = new HederaDLTKeyPair( null, senderEncryptedPrivateKey );  // Signatory(new HederaDLTKeyPair(null, HederaConfig.HederaTestNetJurisdictionGLTreasuryEncryptedPrivateKey).PrivateHederaKeyBytes);
        //        receipt = await client.TransferTokensAsync(TokenAddress, senderAddress, receiverAddress, amountToTransfer, new Signatory(keyPair.PrivateKeyBytes) );
        //    }

        //    catch (Exception ex)
        //    {
        //        Debug.Print($"Error in WCOHederaManager.TransferATTNAsync(1) - {ex.ToString()}");
        //    }
        //    return await Task.FromResult<TransactionReceipt>(receipt);

        //}

        //public static async Task<TransactionReceipt> TransferATTNAsync(Address senderAddress, Address receiverAddress, long amountToTransfer, Signatory signatory )
        //{
        //    TransactionReceipt receipt = null!;
        //    try
        //    {
        //        await using var client = new Client(ctx =>
        //        {
        //            ctx.Gateway = DetermineGatewayToUse(HederaConfig.HeaderTestNetGateways);    // Random Hedera Gateway
        //            ctx.Payer = JurisdictionGLTreasuryAccountAddress;                                      // WCO Treasury pays for the transfer
        //            ctx.Signatory = JurisdictionGLTreasurySignatory;                                       // WCO Treasury authorizes the transfer
        //        });
        //        receipt = await client.TransferTokensAsync(TokenAddress, senderAddress, receiverAddress, amountToTransfer, signatory );
        //    }

        //    catch (Exception ex)
        //    {
        //        Debug.Print($"Error in WCOHederaManager.TransferATTNAsync(2) - {ex.ToString()}");
        //    }
        //    return await Task.FromResult<TransactionReceipt>(receipt);

        //}
        #endregion


        #region Public Interface
        public HederaDLTKeyPair GenerateDLTKey()
        {
            return new HederaDLTKeyPair();
        }

        public HederaDLTKeyPair LoadDLTKey(string base64EncryptedPublicKey, string base64EncryptedPrivateKey)
        {
            return new HederaDLTKeyPair(base64EncryptedPublicKey, base64EncryptedPrivateKey);
        }

        public async Task<CreateAccountReceipt> CreateAccountAddressAsync(Address payor, Signatory payorSignatory, Endorsement accountEndorsement)
        {
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);                           // Random Hedera Gateway
                    ctx.Payer = payor;
                    ctx.Signatory = payorSignatory;
                });

                var createParams = new CreateAccountParams
                {
                    Endorsement = accountEndorsement,                                                       // account endorses itself
                    InitialBalance = 0,                                                                     // Zero the balance on creation
                    AutoRenewPeriod = TimeSpan.FromDays(90),
                    RequireReceiveSignature = false,
                    AutoAssociationLimit = 1
                };
                //CreateAccountReceipt accountReceipt = await client.CreateAccountAsync(createParams).ConfigureAwait(false);
                CreateAccountReceipt accountReceipt = await client.RetryKnownNetworkIssuesForReceipt(async client =>
                {
                    return await client.CreateAccountAsync(createParams);
                });
                if (accountReceipt != null && accountReceipt.Status == ResponseCode.Success)
                {

                    //address = accountReceipt.Address;   // Record the Address in the account
                    return accountReceipt;
                }
                else
                {
                    if (accountReceipt != null)
                    {
                        throw new InvalidOperationException($"Hedera failed to create account reporting status: {accountReceipt.Status}");
                    }
                    else
                    {
                        throw new InvalidOperationException("Hedera failed to create account for unknown reason.");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<CreateAccountReceipt?> CreateTokenAccountAddressAsync(
                                                                           Address payor,
                                                                           Signatory payorSignatory,
                                                                           Address tokenAddress,
                                                                           Address treasury,
                                                                           Endorsement accountEndorsement,
                                                                           Signatory accountSignatory//,
                                                                                                     //Signatory grantkycSignatory
                                                                        )
        {
            CreateAccountReceipt accountReceipt = null!;
            try
            {
                // Step #1:  Create an Hedera DLT Account
                accountReceipt = await CreateAccountAddressAsync(payor, payorSignatory, accountEndorsement).ConfigureAwait(false);
                if (accountReceipt != null && accountReceipt.Status == ResponseCode.Success)
                {
                    // Step #2:  Associate Account with the Token
                    /*var trxReceipt = await AssociateAccountWithTokenAsync(payor, payorSignatory, tokenAddress,
                                             accountReceipt.Address, accountSignatory).ConfigureAwait(false);
                    if (trxReceipt != null && trxReceipt.Status == ResponseCode.Success)
                    {
                        // Step #3:  Grant KYC on Account for the Token
                        var grantStatus = await GrantTokenKycOnAccountAsync(payor, payorSignatory, tokenAddress, accountReceipt.Address!, grantkycSignatory).ConfigureAwait(false);
                        if (grantStatus == ResponseCode.Success)
                        {
                            // If we make it here the account is fully created
                        }
                        else
                        {
                            throw new Exception($"Failed GrantTokenKycOnAccountAsync() call - resultCode={grantStatus}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed AssociateAccountWithTokenAsync() call - resultCode={trxReceipt}");
                    }*/
                }
                else
                {
                    throw new Exception($"Failed CreateAccountAddressAsync() call");
                }
            }
            catch (Exception ex)
            {
                // NOP - Call made on "best effort" so ignore any errors
                Debug.Print($"[API] ERROR: HederaManager.CreateTokenAccountAddressAsync() - {ex.ToString()}");
                if (accountReceipt != null && accountReceipt.Status == ResponseCode.Success)
                {
                    try
                    {
                        // Delete any partially created Account
                        var deleteTrxReceipt = await DeleteAccountAsync(payor, payorSignatory, accountReceipt.Address!, treasury, accountSignatory).ConfigureAwait(false);
                        if (deleteTrxReceipt != null && deleteTrxReceipt.Status == ResponseCode.Success)
                        {
                            Debug.Print($"CreateTokenAccountAddressAsync() created address was deleted.");
                        }
                    }
                    catch (Exception)
                    {
                        // NOP - Best effort
                    }
                }
            }
            return accountReceipt;
        }



        public async Task<CreateTokenReceipt> CreateTokenAsync(
                        Address payor,
                        Signatory payorSignatory,
                        Address tokenTreasuryAddress,
                        Signatory treasurySignatory,
                        string tokenNameAndSymbol,
                        Signatory tokenAdminSignatory,
                        Endorsement tokeAdminEndorsement
                        //Endorsement tokenGrantKycEndorsement,
                        //Endorsement tokenSuspendEndorsement,
                        //Endorsement tokenConfiscateEndorsement,
                        //Endorsement tokenSupplyEndorsement
                                                                        )
        {
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);  // Random Hedera Gateway
                    ctx.Payer = payor;
                    ctx.Signatory = new Signatory(treasurySignatory, payorSignatory);
                    ctx.SignaturePrefixTrimLimit = int.MaxValue;
                });
                var createParams = new CreateTokenParams
                {
                    Name = tokenNameAndSymbol,
                    Symbol = tokenNameAndSymbol,
                    Circulation = 90_000_000_000_000,
                    Decimals = 0,
                    Treasury = tokenTreasuryAddress,
                    //Treasury = payor,
                    Administrator = tokeAdminEndorsement,
                    Signatory = tokenAdminSignatory,
                    //GrantKycEndorsement = tokenGrantKycEndorsement,
                    //SuspendEndorsement = tokeAdminEndorsement,//tokenSuspendEndorsement,
                    //ConfiscateEndorsement = tokeAdminEndorsement, //tokenConfiscateEndorsement,
                    //SupplyEndorsement = tokeAdminEndorsement, //tokenSupplyEndorsement,
                    //InitializeSuspended = false,
                    Expiration = DateTime.UtcNow.AddDays(90),
                    RenewAccount = payor,
                    RenewPeriod = TimeSpan.FromDays(90),
                };

                //var tokenReceipt = await client.CreateTokenAsync(createParams).ConfigureAwait(false);
                var tokenReceipt = await client.RetryKnownNetworkIssuesForReceipt(async client =>
                {
                    return await client.CreateTokenAsync(createParams).ConfigureAwait(false);
                }).ConfigureAwait(false);
                return tokenReceipt;
            }
            catch (Exception ex)
            {
                Debug.Print($"Error in HederaManager.CreateTokenAsync() - {ex.ToString()}");
                throw;
            }
        }

        public async Task<CreateContractRecord> CreateContractAsync(    Address payor,
                                                                        Endorsement payorEndorsement,
                                                                        Signatory payorSignatory
                                                                    )
        {
            Client client = new Client( ctx =>
            {
                ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);
                ctx.Payer = payor;
                ctx.Signatory = payorSignatory;
            });
            
            #region Create an "empty" File to hold the DAGL Smart Contract byte code contents
            var fileParams = new CreateFileParams
            {
                Expiration = DateTime.UtcNow.AddSeconds(7890000),
                Endorsements = new Endorsement[] { payorEndorsement }
                              
            };
            
            var fileRecord = await client.RetryKnownNetworkIssues(async client =>
            {
                return await client.CreateFileWithRecordAsync(fileParams, ctx =>
                {
                    ctx.Memo = "Create DAGL Smart Contract File";
                });
            });
            //Assert.Equal(ResponseCode.Success, fx.FileRecord.Status);
            if (fileRecord.Status != ResponseCode.Success)
            {
                throw new InvalidOperationException("Unable to create Hedera File.");
            }
            #endregion

            #region Append the DAGL smart contract byte code contents to the file just created in DAGL_FILE_CHUNK_SIZE chunks
            var smartContractByteCodeBytes = Encoding.UTF8.GetBytes(DAGL_SMART_CONTRACT_CONTRACT_BYTECODE);
            var sizeOfFinalChunk = smartContractByteCodeBytes.Length % DAGL_FILE_CHUNK_SIZE;
            int chunksRequired = smartContractByteCodeBytes.Length / DAGL_FILE_CHUNK_SIZE;
            if (chunksRequired * DAGL_FILE_CHUNK_SIZE < smartContractByteCodeBytes.Length)
            {
                chunksRequired++;
            }
            for (int c = 0; c < chunksRequired; c++)
            {
                byte[] chunk = null!;
                if (c == chunksRequired - 1)
                {
                    chunk = new byte[sizeOfFinalChunk];
                }
                else
                {
                    chunk = new byte[ DAGL_FILE_CHUNK_SIZE];
                }
                Buffer.BlockCopy(smartContractByteCodeBytes, c * DAGL_FILE_CHUNK_SIZE, chunk, 0, chunk.Length);
                // NOTE:  We do it this way since the byte code is likely larger than the 6K limit on transaction sizes.
                //        this approach will "chunk" the code
                var fileAppendParams = new AppendFileParams
                {
                    Contents = chunk,
                    File = fileRecord.File
                };
                var trxRecord = await client.RetryKnownNetworkIssues(async client =>
                {
                    return await client.AppendFileWithRecordAsync(fileAppendParams, ctx =>
                    {
                        ctx.Memo = "Append Byte Code to Contract File ";
                        ctx.Signatory = payorSignatory;
                    });
                });
                //Assert.Equal(ResponseCode.Success, trxRecord.Status);
                if (trxRecord.Status != ResponseCode.Success)
                {
                    throw new InvalidOperationException("Unable to append to Hedera File.");
                }
            }
            #endregion

            #region Create the Smart Contract from the File
            var contractParams = new CreateContractParams
            {
                /// The address of the file containing the bytecode for the contract. 
                /// The bytecode is encoded as a hexadecimal string in the file 
                /// (not directly as the bytes of the bytescode).
                File = fileRecord.File,
                /// An optional endorsement that can be used to modify the contract details.  
                /// If left null, the contract is immutable once created.
                /// NOTE:  Needs to be set to be able to Delete the contract later.
                //Administrator = fx.PublicKey,       
                Administrator = payorEndorsement,
                //Administrator =new Endorsement( fx.PayorEndorsement, fx.AlternatePayorEndorsement),
                /// Additional private key, keys or signing callback method 
                /// required to create this contract.  Typically matches the
                /// Administrator endorsement assigned to this new contract.
                /// Keys/callbacks added here will be combined with those already
                /// identified in the client object's context when signing this 
                /// transaction to change the state of this account.  They will 
                /// not be asked to sign transactions to retrieve the record
                /// if the "WithRecord" form of the method call is made.  The
                /// client will rely on the Signatory from the context to sign
                /// the transaction requesting the record.
                //Signatory = fx.PrivateKey,
                //Signatory = fx.PayorSignatory,
                //Signatory = new Signatory( fx.PayorSignatory, fx.AlternatePayorSignatory),
                /// Short description of the contract, limit to 100 bytes.
                Memo = "DAGL Contract ",
                /// Maximum gas to pay for the constructor, unused gas will be 
                /// refunded to the paying account.
                Gas = 150000,
                /// The renewal period for maintaining the contract bytecode and state.  
                /// The contract instance will be charged at this interval as appropriate.
                RenewPeriod = TimeSpan.FromSeconds(7890000)  // approx 91 days
                /// The arguments to pass to the smart contract constructor method.
                //Arguments = new object[] { "Hello from .NET. " + DateTime.UtcNow.ToLongDateString() }
            };
            //customize?.Invoke(fx);
            var contractRecord = await client.RetryKnownNetworkIssues(async client =>
            {
                return await client.CreateContractWithRecordAsync( contractParams, ctx =>
                {
                    ctx.Payer = payor;
                    ctx.Memo = "DAGL Contract Create";
                    //ctx.Signatory = new Signatory(fx.PayorSignatory, fx.AlternatePayorSignatory);
                    ctx.Signatory = payorSignatory;
                });
            });
            #endregion
            return contractRecord;
        }


        public async Task<TransactionReceipt> SimpleCryptoTransferAsync(Address payor, Signatory[] signatories, Address fromAccount, Address toAccount, long amount)
        {
            TransactionReceipt tokenReceipt = null!;
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);    // Random Hedera Gateway
                    ctx.Payer = payor;
                    ctx.SignaturePrefixTrimLimit = int.MaxValue;
                    ctx.Signatory = new Signatory(signatories);
                });

                tokenReceipt = await client.TransferAsync(fromAccount, toAccount, amount);
                if (tokenReceipt == null || tokenReceipt.Status != ResponseCode.Success)
                {
                    throw new InvalidOperationException("Error transfering crypto.");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error in SimpleCryptoTransferAsync() - {ex}");
                throw;
            }
            return await Task.FromResult(tokenReceipt);
        }


        public async Task<TransactionReceipt> SimpleTokenTransferAsync(
                                                                        Address payor,
                                                                        Signatory payorSignatory,
                                                                        Address tokenAddress,
                                                                        Signatory treasurySignatory,
                                                                        Address fromAccount,
                                                                        Address toAccount,
                                                                        long amount)
        {
            TransactionReceipt tokenReceipt = null!;
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);    // Random Hedera Gateway
                    ctx.Payer = payor;
                    ctx.Signatory = payorSignatory;
                });


                tokenReceipt = await client.TransferTokensAsync(tokenAddress, fromAccount, toAccount, amount, treasurySignatory);


                if (tokenReceipt == null || tokenReceipt.Status != ResponseCode.Success)
                {
                    throw new InvalidOperationException("Error transfering token.");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error HederaManaer.SimpleTokenTransferAsync() - {ex}");
                throw;
            }
            return await Task.FromResult(tokenReceipt);
        }



        public async Task<TransactionReceipt> JournalEntryTokenTransferAsync(
                                                                      Address payor,
                                                                      Signatory payorSignatory,
                                                                      Address tokenAddress,
                                                                      Address treasuryAddress,
                                                                      Signatory treasurySignatory,
                                                                      JournalEntryRecord journalEntryRecord,
                                                                      //int transactionBatchSize,
                                                                      //bool isReversing,
                                                                      Address externalCryptoAddress,
                                                                      Signatory externalCryptSignatory,
                                                                      bool transferOutToExternalAccount,
                                                                      long externalCryptoAmount,
                                                                      bool clearAccounts = false)
        {
            TransactionReceipt trxReceipt = null!;
            Debug.Print($"Journal Entry To Post:");
            Debug.Print(journalEntryRecord.JournalEntry!.DumpJournalEntry());
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);    // Random Hedera Gateway
                    ctx.Payer = payor;
                    ctx.Signatory = payorSignatory;
                    ctx.Memo = journalEntryRecord!.TransactionID;  // NOTE:  We make the Memo of the this transaction the Journal Entry's generated transaction ID
                });


                List<TokenTransfer> tokenTransfers = new List<TokenTransfer>();
                TransferParams transferParams = null!;

                //List<TokenTransfer> debitTokenTransfers = new List<TokenTransfer>();
                ulong totalTokensToTransferFromTreasury = 0;
                //long halfOfCryptoAmount = 0;
                //List<Task<TransactionReceipt>> transferTasks = new List<Task<TransactionReceipt>>();
                //TransferParams debitsTransferParams = null!;
                //TransferParams creditsTransferParams = null!;

                // Determine an optimized set of debit and credit accounts from the original journalEntryRecord
                var optimizedJournalEntryRecord = OptimizedJournalEntryRecord(journalEntryRecord, clearAccounts);

                foreach (var dbacc in optimizedJournalEntryRecord.JournalEntry!.DebtAccountList)
                {
                    if (dbacc.Amount > 0)
                    {
                        tokenTransfers.Add(new TokenTransfer(tokenAddress, ParseAddress(dbacc.DLTGLAccountInfo!.DLTAddress), (clearAccounts ? -1 : 1) * Convert.ToInt64(dbacc.Amount)));
                        totalTokensToTransferFromTreasury += dbacc.Amount!.Value;
                    }
                }
                foreach (var cracc in optimizedJournalEntryRecord.JournalEntry!.CreditAccountList)
                {
                    if (cracc.Amount > 0)
                    {
                        tokenTransfers.Add(new TokenTransfer(tokenAddress, ParseAddress(cracc.DLTGLAccountInfo!.DLTAddress), (clearAccounts ? -1 : 1) * Convert.ToInt64(cracc.Amount)));
                        totalTokensToTransferFromTreasury += cracc.Amount!.Value;
                    }
                }
                if (tokenTransfers.Count > 0)
                {
                    // Add the balancing amount to remove/add from/to the treasury based on the transferOutToExternalAccount flag
                    tokenTransfers.Add(new TokenTransfer(tokenAddress, treasuryAddress, (clearAccounts ? -1 : 1) * Convert.ToInt64(totalTokensToTransferFromTreasury) * -1));
                    if (externalCryptoAddress == null)
                    {
                        // If we make it here the journal entry only affects GL Accounts
                        transferParams = new TransferParams
                        {
                            TokenTransfers = tokenTransfers,
                            Signatory = treasurySignatory
                        };
                    }
                    else
                    {
                        // If we make it here the journal entry affects GL Accounts and Crypto accounts (i.e. its a Cash In or Cash Out journal entry)
                        //halfOfCryptoAmount = externalCryptoAmount / 2;
                        Dictionary<AddressOrAlias, long> ctl = new Dictionary<AddressOrAlias, long>();
                        if (transferOutToExternalAccount)
                        {
                            // To external account from treasury account
                            ctl.Add(externalCryptoAddress, externalCryptoAmount);
                            ctl.Add(treasuryAddress, -1 * externalCryptoAmount);
                        }
                        else
                        {
                            // From external account to treasury account
                            ctl.Add(externalCryptoAddress, -1 * externalCryptoAmount);
                            ctl.Add(treasuryAddress, externalCryptoAmount);
                        }
                        transferParams = new TransferParams
                        {
                            TokenTransfers = tokenTransfers,
                            CryptoTransfers = ctl,
                            Signatory = new Signatory(treasurySignatory, externalCryptSignatory)
                        };
                    }

                    trxReceipt = await client.TransferAsync(transferParams).ConfigureAwait(false);
                    if (trxReceipt == null || trxReceipt.Status != ResponseCode.Success)
                    {
                        throw new InvalidOperationException("Error transfering tokens.");
                    }
                    //transferTasks.Add(client.TransferAsync(debitsTransferParams));

                    //// Verify all calls
                    //foreach (var tr in trxReceipts)
                    //{
                    //    if (tr.Status != ResponseCode.Success)
                    //    {
                    //        throw new InvalidOperationException("Error transfering tokens.");
                    //    }
                    //}
                    //trxReceipt = trxReceipts[0];  // Hack because we have to split the journal entry into two separate calls - so use first call's trxReceipt

                    //if (trxReceipt == null || trxReceipt.Status != ResponseCode.Success)
                    //{
                    //    throw new InvalidOperationException("Error transfering tokens.");
                    //}
                }

                // If we make it here...do the same as above for Credits
                //totalTokensToTransferFromTreasury = 0;
                //List<TokenTransfer> creditTokenTransfers = new List<TokenTransfer>();  // Reset for credits
                //foreach (var cracc in optimizedJournalEntryRecord.JournalEntry!.CreditAccountList)
                //{
                //    if (cracc.Amount > 0)
                //    {
                //        creditTokenTransfers.Add(new TokenTransfer(tokenAddress, ParseAddress(cracc.DLTGLAccountInfo!.DLTAddress), (clearAccounts ? -1 : 1) * Convert.ToInt64(cracc.Amount)));
                //        totalTokensToTransferFromTreasury += cracc.Amount!.Value;
                //    }
                //}
                //if (creditTokenTransfers.Count > 0)
                //{
                //    // Add the balancing amount to remove/add from/to the treasury based on the transferOutToExternalAccount flag
                //    creditTokenTransfers.Add(new TokenTransfer(tokenAddress, treasuryAddress, (clearAccounts ? -1 : 1) * Convert.ToInt64(totalTokensToTransferFromTreasury) * -1));
                //    if (externalCryptoAddress == null)
                //    {
                //        creditsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = creditTokenTransfers,
                //            Signatory = treasurySignatory,
                //        };
                //    }
                //    else
                //    {
                //        halfOfCryptoAmount = Convert.ToInt64(totalTokensToTransferFromTreasury) - halfOfCryptoAmount;  // Other half of fund amount accouting for rounding
                //        Dictionary<AddressOrAlias, long> ctl = new Dictionary<AddressOrAlias, long>();
                //        if (transferOutToExternalAccount)
                //        {
                //            // To external account from treasury account
                //            ctl.Add(externalCryptoAddress, halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, -1 * halfOfCryptoAmount);
                //        }
                //        else
                //        {
                //            // From external account to treasury account
                //            ctl.Add(externalCryptoAddress, -1 * halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, halfOfCryptoAmount);
                //        }
                //        creditsTransferParams = new TransferParams
                //       {
                //            TokenTransfers = creditTokenTransfers,
                //            CryptoTransfers = ctl,
                //            Signatory = new Signatory(treasurySignatory, externalCryptSignatory)
                //        };
                //    }
                //    transferTasks.Add(client.TransferAsync(creditsTransferParams));
                //    //if (trxReceipt == null || trxReceipt.Status != ResponseCode.Success)
                //    //{
                //    //    throw new InvalidOperationException("Error transfering tokens.");
                //    //}
                //}

                #region OLD Prepare Transfer Transaction
                //foreach (var dbacc in optimizedJournalEntryRecord.JournalEntry!.DebtAccountList)
                //{
                //    if (dbacc.Amount > 0)
                //    {
                //        debitTokenTransfers.Add(new TokenTransfer(tokenAddress, ParseAddress(dbacc.DLTGLAccountInfo!.DLTAddress), (clearAccounts ? -1 : 1) * Convert.ToInt64(dbacc.Amount)));
                //        totalTokensToTransferFromTreasury += dbacc.Amount!.Value;
                //    }
                //}
                //if (debitTokenTransfers.Count > 0)
                //{
                //    // Add the balancing amount to remove/add from/to the treasury based on the transferOutToExternalAccount flag
                //    debitTokenTransfers.Add(new TokenTransfer(tokenAddress, treasuryAddress, (clearAccounts ? -1 : 1) * Convert.ToInt64(totalTokensToTransferFromTreasury) * -1));
                //    if (externalCryptoAddress == null)
                //    {
                //        // If we make it here the journal entry only affects GL Accounts
                //        debitsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = debitTokenTransfers,
                //            Signatory = treasurySignatory
                //        };
                //    }
                //    else
                //    {
                //        // If we make it here the journal entry affects GL Accounts and Crypto accounts (i.e. its a Cash In or Cash Out journal entry)
                //        halfOfCryptoAmount = externalCryptoAmount / 2;
                //        Dictionary<AddressOrAlias, long> ctl = new Dictionary<AddressOrAlias, long>();
                //        if (transferOutToExternalAccount)
                //        {
                //            // To external account from treasury account
                //            ctl.Add(externalCryptoAddress, halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, -1 * halfOfCryptoAmount);
                //        }
                //        else
                //        {
                //            // From external account to treasury account
                //            ctl.Add(externalCryptoAddress, -1 * halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, halfOfCryptoAmount);
                //        }
                //        debitsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = debitTokenTransfers,
                //            CryptoTransfers = ctl,
                //            Signatory = new Signatory(treasurySignatory, externalCryptSignatory)
                //        };
                //    }
                //    transferTasks.Add(client.TransferAsync(debitsTransferParams));
                //    //if (trxReceipt == null || trxReceipt.Status != ResponseCode.Success)
                //    //{
                //    //    throw new InvalidOperationException("Error transfering tokens.");
                //    //}
                //}

                //// If we make it here...do the same as above for Credits
                //totalTokensToTransferFromTreasury = 0;
                //List<TokenTransfer> creditTokenTransfers = new List<TokenTransfer>();  // Reset for credits
                //foreach (var cracc in optimizedJournalEntryRecord.JournalEntry!.CreditAccountList)
                //{
                //    if (cracc.Amount > 0)
                //    {
                //        creditTokenTransfers.Add(new TokenTransfer(tokenAddress, ParseAddress(cracc.DLTGLAccountInfo!.DLTAddress), (clearAccounts ? -1 : 1) * Convert.ToInt64(cracc.Amount)));
                //        totalTokensToTransferFromTreasury += cracc.Amount!.Value;
                //    }
                //}
                //if (creditTokenTransfers.Count > 0)
                //{
                //    // Add the balancing amount to remove/add from/to the treasury based on the transferOutToExternalAccount flag
                //    creditTokenTransfers.Add(new TokenTransfer(tokenAddress, treasuryAddress, (clearAccounts ? -1 : 1) * Convert.ToInt64(totalTokensToTransferFromTreasury) * -1));
                //    if (externalCryptoAddress == null)
                //    {
                //        creditsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = creditTokenTransfers,
                //            Signatory = treasurySignatory,
                //        };
                //    }
                //    else
                //    {
                //        halfOfCryptoAmount = Convert.ToInt64(totalTokensToTransferFromTreasury) - halfOfCryptoAmount;  // Other half of fund amount accouting for rounding
                //        Dictionary<AddressOrAlias, long> ctl = new Dictionary<AddressOrAlias, long>();
                //        if (transferOutToExternalAccount)
                //        {
                //            // To external account from treasury account
                //            ctl.Add(externalCryptoAddress, halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, -1 * halfOfCryptoAmount);
                //        }
                //        else
                //        {
                //            // From external account to treasury account
                //            ctl.Add(externalCryptoAddress, -1 * halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, halfOfCryptoAmount);
                //        }
                //        creditsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = creditTokenTransfers,
                //            CryptoTransfers = ctl,
                //            Signatory = new Signatory(treasurySignatory, externalCryptSignatory)
                //        };
                //    }
                //    transferTasks.Add(client.TransferAsync(creditsTransferParams));
                //    //if (trxReceipt == null || trxReceipt.Status != ResponseCode.Success)
                //    //{
                //    //    throw new InvalidOperationException("Error transfering tokens.");
                //    //}
                //}
                #endregion

                // Execute tasks in parallel
                //TransactionReceipt[] trxReceipts = await Task.WhenAll(transferTasks).ConfigureAwait(false);
                //if (trxReceipts.Length != transferTasks.Count) //|| trxReceipts[0] == null || trxReceipts[1] == null || 
                //                                               //         trxReceipts[0].Status != ResponseCode.Success || trxReceipts[1].Status != ResponseCode.Success)
                //{
                //    throw new InvalidOperationException("Error transfering tokens.");
                //}
                //else
                //{
                //    // Verify all calls
                //    foreach (var tr in trxReceipts)
                //    {
                //        if (tr.Status != ResponseCode.Success)
                //        {
                //            throw new InvalidOperationException("Error transfering tokens.");
                //        }
                //    }
                //    trxReceipt = trxReceipts[0];  // Hack because we have to split the journal entry into two separate calls - so use first call's trxReceipt
                //}
                //#endregion 
            }
            catch (Exception ex)
            {
                Debug.Print($"Error HederaManager.JournalEntryTokenTransferAsync() - {ex}");
                throw;
            }
            return trxReceipt;
        }



        public async Task<ReadOnlyCollection<TransactionRecord>> SmartContractJournalEntryTokenTransferAsync( Address contract,
                                                                      Address payor,
                                                                      Signatory payorSignatory,
                                                                      Address tokenAddress,
                                                                      Address treasuryAddress,
                                                                      Signatory treasurySignatory,
                                                                      JournalEntryRecord journalEntryRecord,
                                                                      int transactionBatchSize,
                                                                      bool isReversing,
                                                                      //Address externalCryptoAddress,
                                                                      //Signatory externalCryptSignatory,
                                                                      //bool transferOutToExternalAccount,
                                                                      //long externalCryptoAmount,
                                                                      bool clearAccounts = false)
        {
            ReadOnlyCollection<TransactionRecord> trxRecords = null!;
            Debug.Print($"Journal Entry To Post:");
            Debug.Print(journalEntryRecord.JournalEntry!.DumpJournalEntry());
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);    // Random Hedera Gateway
                    
                });

                //List<TokenTransfer> debitTokenTransfers = new List<TokenTransfer>();
                //ulong totalTokensToTransferFromTreasury = 0;
                //long halfOfCryptoAmount = 0;
                //List<Task<TransactionReceipt>> transferTasks = new List<Task<TransactionReceipt>>();
                //TransferParams debitsTransferParams = null!;
                //TransferParams creditsTransferParams = null!;

                // Determine an optimized set of debit and credit accounts from the original journalEntryRecord
                var optimizedJournalEntryRecord = OptimizedJournalEntryRecord(journalEntryRecord, clearAccounts);
                Debug.Print(optimizedJournalEntryRecord.JournalEntry!.DumpJournalEntry());
                byte[]? smParamBytes = CreateSmartContractParameterBytesFromJournalEntry(tokenAddress,
                                                                                            treasuryAddress,
                                                                                            optimizedJournalEntryRecord,
                                                                                            isReversing,
                                                                                            transactionBatchSize);
                TestSmartContractLogic(smParamBytes!);
                long maxGas = 2_044_000;
                var contractCallReceipt = await client.CallContractAsync(new CallContractParams
                {
                    Contract = contract,
                    Gas = maxGas,
                    FunctionName = "AtomicJournalEntryPost",
                    FunctionArgs = new object[] { smParamBytes! },
                }, ctx =>
                {
                    ctx.Payer = payor;
                    //ctx.Signatory = payorSignatory;
                    ctx.Signatory = new Signatory(treasurySignatory, payorSignatory);
                    ctx.SignaturePrefixTrimLimit = int.MaxValue;
                    ctx.Memo = journalEntryRecord!.TransactionID;  // NOTE:  We make the Memo of the this transaction the Journal Entry's generated transaction ID
                    //ctx.Payer = AlternatePayorAddress;
                    //ctx.Signatory = new Signatory(JurisdictionTreasurySignatory, AlternatePayorSignatory);
                }).ConfigureAwait(false);

                if( contractCallReceipt == null || contractCallReceipt.Status != ResponseCode.Success )
                {
                    throw new InvalidOperationException("Unable to call contract.");
                }
                trxRecords = await client.GetAllTransactionRecordsAsync(contractCallReceipt.Id, ctx =>
                    {
                        ctx.Payer = payor;
                        ctx.Signatory = payorSignatory;
                        //ctx.Payer = AlternatePayorAddress;
                        //ctx.Signatory = AlternatePayorSignatory;
                    }).ConfigureAwait(false);
                //if (record.Count > 0)
                //{
                //    callContractRecord = (CallContractRecord)record[0];
                //    //var result = contractCallRecord?.CallResult?.Result.As<int>();
                //    //callContractRecord = contractCallRecord!;
                //}
                //else
                //{
                //    throw new InvalidOperationException("Unable to retrieve contract record.");
                //}
                #region Ignore
                //foreach (var dbacc in optimizedJournalEntryRecord.JournalEntry!.DebtAccountList)
                //{
                //    if (dbacc.Amount > 0)
                //    {
                //        debitTokenTransfers.Add(new TokenTransfer(tokenAddress, ParseAddress(dbacc.DLTGLAccountInfo!.DLTAddress), (clearAccounts ? -1 : 1) * Convert.ToInt64(dbacc.Amount)));
                //        totalTokensToTransferFromTreasury += dbacc.Amount!.Value;
                //    }
                //}
                //if (debitTokenTransfers.Count > 0)
                //{
                //    // Add the balancing amount to remove/add from/to the treasury based on the transferOutToExternalAccount flag
                //    debitTokenTransfers.Add(new TokenTransfer(tokenAddress, treasuryAddress, (clearAccounts ? -1 : 1) * Convert.ToInt64(totalTokensToTransferFromTreasury) * -1));
                //    if (externalCryptoAddress == null)
                //    {
                //        // If we make it here the journal entry only affects GL Accounts
                //        debitsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = debitTokenTransfers,
                //            Signatory = treasurySignatory
                //        };
                //    }
                //    else
                //    {
                //        // If we make it here the journal entry affects GL Accounts and Crypto accounts (i.e. its a Cash In or Cash Out journal entry)
                //        halfOfCryptoAmount = externalCryptoAmount / 2;
                //        Dictionary<AddressOrAlias, long> ctl = new Dictionary<AddressOrAlias, long>();
                //        if (transferOutToExternalAccount)
                //        {
                //            // To external account from treasury account
                //            ctl.Add(externalCryptoAddress, halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, -1 * halfOfCryptoAmount);
                //        }
                //        else
                //        {
                //            // From external account to treasury account
                //            ctl.Add(externalCryptoAddress, -1 * halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, halfOfCryptoAmount);
                //        }
                //        debitsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = debitTokenTransfers,
                //            CryptoTransfers = ctl,
                //            Signatory = new Signatory(treasurySignatory, externalCryptSignatory)
                //        };
                //    }
                //    transferTasks.Add(client.TransferAsync(debitsTransferParams));
                //    //if (trxReceipt == null || trxReceipt.Status != ResponseCode.Success)
                //    //{
                //    //    throw new InvalidOperationException("Error transfering tokens.");
                //    //}
                //}

                //// If we make it here...do the same as above for Credits
                //totalTokensToTransferFromTreasury = 0;
                //List<TokenTransfer> creditTokenTransfers = new List<TokenTransfer>();  // Reset for credits
                //foreach (var cracc in optimizedJournalEntryRecord.JournalEntry!.CreditAccountList)
                //{
                //    if (cracc.Amount > 0)
                //    {
                //        creditTokenTransfers.Add(new TokenTransfer(tokenAddress, ParseAddress(cracc.DLTGLAccountInfo!.DLTAddress), (clearAccounts ? -1 : 1) * Convert.ToInt64(cracc.Amount)));
                //        totalTokensToTransferFromTreasury += cracc.Amount!.Value;
                //    }
                //}
                //if (creditTokenTransfers.Count > 0)
                //{
                //    // Add the balancing amount to remove/add from/to the treasury based on the transferOutToExternalAccount flag
                //    creditTokenTransfers.Add(new TokenTransfer(tokenAddress, treasuryAddress, (clearAccounts ? -1 : 1) * Convert.ToInt64(totalTokensToTransferFromTreasury) * -1));
                //    if (externalCryptoAddress == null)
                //    {
                //        creditsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = creditTokenTransfers,
                //            Signatory = treasurySignatory,
                //        };
                //    }
                //    else
                //    {
                //        halfOfCryptoAmount = Convert.ToInt64(totalTokensToTransferFromTreasury) - halfOfCryptoAmount;  // Other half of fund amount accouting for rounding
                //        Dictionary<AddressOrAlias, long> ctl = new Dictionary<AddressOrAlias, long>();
                //        if (transferOutToExternalAccount)
                //        {
                //            // To external account from treasury account
                //            ctl.Add(externalCryptoAddress, halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, -1 * halfOfCryptoAmount);
                //        }
                //        else
                //        {
                //            // From external account to treasury account
                //            ctl.Add(externalCryptoAddress, -1 * halfOfCryptoAmount);
                //            ctl.Add(treasuryAddress, halfOfCryptoAmount);
                //        }
                //        creditsTransferParams = new TransferParams
                //        {
                //            TokenTransfers = creditTokenTransfers,
                //            CryptoTransfers = ctl,
                //            Signatory = new Signatory(treasurySignatory, externalCryptSignatory)
                //        };
                //    }
                //    transferTasks.Add(client.TransferAsync(creditsTransferParams));
                //    //if (trxReceipt == null || trxReceipt.Status != ResponseCode.Success)
                //    //{
                //    //    throw new InvalidOperationException("Error transfering tokens.");
                //    //}
                //}
                //// Execute tasks in parallel
                //TransactionReceipt[] trxReceipts = await Task.WhenAll(transferTasks).ConfigureAwait(false);
                //if (trxReceipts.Length != transferTasks.Count) //|| trxReceipts[0] == null || trxReceipts[1] == null || 
                //                                               //         trxReceipts[0].Status != ResponseCode.Success || trxReceipts[1].Status != ResponseCode.Success)
                //{
                //    throw new InvalidOperationException("Error transfering tokens.");
                //}
                //else
                //{
                //    // Verify all calls
                //    foreach (var tr in trxReceipts)
                //    {
                //        if (tr.Status != ResponseCode.Success)
                //        {
                //            throw new InvalidOperationException("Error transfering tokens.");
                //        }
                //    }
                //    trxReceipt = trxReceipts[0];  // Hack because we have to split the journal entry into two separate calls - so use first call's trxReceipt
                //}
                #endregion 
            }
            catch (Exception ex)
            {
                Debug.Print($"Error HederaManaer.SmartContractJournalEntryTokenTransferAsync() - {ex}");
                throw;
            }
            return await Task.FromResult(trxRecords);
        }


 
        public async Task<ulong> GetAccountCryptoBalanceAsync(Address accountAddress)
        {
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);  // Random Hedera Gateway
                });
                var balances = await client.GetAccountBalancesAsync(accountAddress).ConfigureAwait(false);
                if (balances != null)
                {
                    return balances.Crypto;
                }
                else
                {
                    throw new InvalidOperationException("Error retrieving crypto balance.");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error in HederaManager.GetAccountCryptoBalanceAsync() - {ex.ToString()}");
                throw;
            }
        }


        public async Task<ulong> GetAccountTokenBalanceAsync(Address accountAddress, Address tokenAddress)
        {
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);    // Random Hedera Gateway
                });
                var balances = await client.GetAccountBalancesAsync(accountAddress).ConfigureAwait(false);
                if (balances != null)
                {
                    foreach (var tkn in balances.Tokens)
                    {
                        if (tkn.Key.AccountNum == tokenAddress.AccountNum)
                        {
                            Debug.Print($"Token {tokenAddress.ToString()} balance for address {accountAddress.ToString()} is {tkn.Value}");
                            return tkn.Value;
                        }
                    }
                    // If we make it here we didn't find the token associated with the account because the account hasn't been posted to yet
                    // so default to a zero balance
                    return 0UL;
                    //throw new InvalidOperationException("Unkonwn token");
                }
                else
                {
                    throw new InvalidOperationException("Error retrieving token balance.");
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error in HederaManager.GetAccountTokenBalanceAsync() - {ex.ToString()}");
                throw;
            }
        }


        public static Address ParseAddress(string? address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Invalid Address");
            string[] addressParts = address.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (addressParts.Length != 3)
                throw new ArgumentException("Invalid Address");
            return new Address(long.Parse(addressParts[0]), long.Parse(addressParts[1]), long.Parse(addressParts[2]));
        }
        #endregion

        #region Helper
        private ResponseCode TestSmartContractLogic(byte[] memBytes)
        {
            long accountTransferCount = DotNetInt64(memBytes, 56);
            long isReversing = DotNetInt64(memBytes, 48);
            Address[] transferAddresses = new Address[accountTransferCount + 2];   // NOTE:  +2 - see comments below
            long[] transferAmounts = new long[accountTransferCount + 2];          // NOTE:  +2 - see comments below  
                                                                                  // First 2 slots in transferAddresses used to store tokenId and treasuryId - hack to reduce number of variables to avoid "Stack too deep" error
            transferAddresses[0] = SolidityAddressToHederaAddress(new ReadOnlyMemory<byte>(memBytes, 0, 20).ToArray());  // Token Id
            transferAddresses[1] = SolidityAddressToHederaAddress(new ReadOnlyMemory<byte>(memBytes, 20, 20).ToArray());  // Treasury Id
                                                                                                                          // First 2 slots in transferAmounts used to store maxTransactionBatchSize and totalTokensToTransferFromTreasury - hack to reduce number of variables to avoid "Stack too deep" error
            transferAmounts[0] = DotNetInt64(memBytes, 40);       // maxTransactionBatchSize
            transferAmounts[1] = 0;                           // totalTokensToTransferFromTreasury
            int index = 64;                               // We have consumed the first 56 bytes of memBytes, position index read to next byte

            // Loop to parse the GL account address/amount pair from memBytes and store in parallel transferAddresses/transferAmounts arrays
            for (long i = 0; i < accountTransferCount; i++)
            {
                transferAddresses[i + 2] = SolidityAddressToHederaAddress(new ReadOnlyMemory<byte>(memBytes, index, 20).ToArray());
                //transferAddresses[i + 2] = toAddress(memBytes, index);      // NOTE: +2 to skip first two reserved slots - see comments above
                index += 20; // size of an EMV address in bytes
                             //transferAmounts[i + 2] = toInt64(memBytes, index);            // NOTE: +2 to skip first two reserved slots - see comments above
                transferAmounts[i + 2] = DotNetInt64(memBytes, index);            // NOTE: +2 to skip first two reserved slots - see comments above
                index += 8;  // size of an int64 in bytes
            }


            // The request number of account transfer swaps may exceed the maximum allowable at one time (as specifed by the maxTransactionBatchSize)
            // So we may need to batch the transfers up into maxTransactionBatchSize chunks and process them individually.  The "posting" of a complete
            // Journal Entry therefore entails posting all of the necessary individually chunked transactions in an all or nothing process.  If a Revert()
            // occurs at any time during the processing of these individual transactions, the entire state is rolled back and no state change will take palce.
            //
            // Determine the number of unique transactions it will take to complete this journal entry's entire "atomic" swap based on 
            // passed in accountTransferCount and maxTransactionBatchSize
            //
            // NOTE:  We substract one from the maxTransactionBatchSize in the following calculations because we 
            //        have to reserve one entry in the transaction for the Treasury account which will be used to offset all the other 
            //        account transfers.  That is, the Treasury is assumed to supply the tokens for all the other account transfers. 
            long lastUniqueTransactionSize = ((accountTransferCount - 1) % (transferAmounts[0] - 1)) + 1;
            long uniqueTransactionCount = accountTransferCount / (transferAmounts[0] - 1);
            if (uniqueTransactionCount * (transferAmounts[0] - 1) < accountTransferCount)
            {
                uniqueTransactionCount++;
            }
            // Initialize a variable to track which account transfer is being processed at a given loop iteration        
            int currentAccount = 2;  // NOTE:  2 because we skip the first two slots in the  transferAddresses/transferAmounts arrays 

            // Loop through the number of unique transactions we need to create to complete this "atomic" swap
            for (long uniqueTransaction = 0; uniqueTransaction < uniqueTransactionCount; uniqueTransaction++)
            {
                transferAmounts[1] = 0;  // Reset totalTokensToTransferFromTreasury for each unique transaction
                                         // Determine the size of this unique transaction
                long currentUniqueTransactionSize = (
                        uniqueTransaction == uniqueTransactionCount - 1 ?
                            lastUniqueTransactionSize + 1 :      // Partial unique Transaction (i.e.; lastUniqueTransactionSize in size) - +1 for the offsetting Treasury account
                            transferAmounts[0] // Fullsize unique Transaction (i.e.; maxTransactionBatchSize in size)            
                                                        );

                // Allocate the parallel addresses and amounts arrays needed for this unique transaction
                //address[] memory addresses = new address[](currentUniqueTransactionSize);
                Address[] addresses = new Address[currentUniqueTransactionSize];
                //int64[] memory amounts = new int64[](currentUniqueTransactionSize);
                long[] amounts = new long[currentUniqueTransactionSize];
                // Loop to initialize above parallel arrays and total the number of treasury tokens we need to offset the total account amounts in the unique transaction
                int accountAddressAmountPair;
                for (accountAddressAmountPair = 0; accountAddressAmountPair < (currentUniqueTransactionSize - 1); accountAddressAmountPair++)
                {
                    amounts[accountAddressAmountPair] = transferAmounts[currentAccount] * (isReversing == 1 ? -1 : 1);
                    addresses[accountAddressAmountPair] = transferAddresses[currentAccount];
                    transferAmounts[1] += transferAmounts[currentAccount];
                    Debug.Print($"uniqueTRX:{uniqueTransaction}, currentAccount:{currentAccount}/{accountAddressAmountPair}, ({addresses[accountAddressAmountPair].ToString()}->{amounts[accountAddressAmountPair]})");
                    currentAccount++;
                }

                // Now add the required extra offseting Treasury account address/amount in the final slot of the parallel arrays allocated above
                addresses[accountAddressAmountPair] = transferAddresses[1];                             // Treasury Id
                amounts[accountAddressAmountPair] = transferAmounts[1] * (isReversing == 1 ? 1 : -1); // totalTokensToTransferFromTreasury - * -1 as tokens are subtracted from Treasury 

                Debug.Print($"uniqueTRX:{uniqueTransaction}, TREASURY/{accountAddressAmountPair}, ({addresses[accountAddressAmountPair].ToString()}->{amounts[accountAddressAmountPair]})");

                // Perform atomic swap for this unique transaction only
                /*(bool success, bytes memory result) = precompileAddress.call(
                             abi.encodeWithSelector(IHederaTokenService.transferTokens.selector,
                             transferAddresses[0], addresses, amounts));*/

                //return  int(abi.decode(result, (int32))) ;
                //if( (success ? abi.decode(result, (int32)) : HederaResponseCodes.UNKNOWN) != HederaResponseCodes.SUCCESS )
                //if (!success)
                //{
                //    if (uniqueTransaction == 0)  // Were we attempting the very first unique transaction in this batch?
                //    {
                //        // NOTE: we do not need to Revert as the above call returned false and therefore no state has been changed to this point in the contract executiion
                //        //       so we can simply return the actual reason for the failure to the caller.
                //        return int(abi.decode(result, (int32)));
                //    }
                //    else
                //    {
                //        revert("Transfer Failed");        // NOTE:  We revert here to roll back any previously successful unique transactions, ensure no state change in overall transaction 
                //    }
                //}
                //return addresses[2];            
            } // for( uint256 uniqueTransaction = 0; uniqueTransaction < uniqueTransactionCount; i++)

            // If we make it here we have successfully posted the entire journal entry as an atomic step (potentiall using multiple individual calls to HTS) 
            // so return success
            return 0;
        }


        //private static byte[] EncodeInt64Part(object value)
        //{
        //    var bytes = new byte[32];
        //    WriteInt256(bytes.AsSpan(), Convert.ToInt64(value));
        //    return bytes;
        //}

        //private static void WriteInt256(Span<byte> buffer, long value)
        //{
        //    var valueAsBytes = BitConverter.GetBytes(value);
        //    if (BitConverter.IsLittleEndian)
        //    {
        //        Array.Reverse(valueAsBytes);
        //    }
        //    valueAsBytes.CopyTo(buffer.Slice(24));
        //}

        //private byte[] SolidityInt64(long val)
        //{
        //    var bytes = BitConverter.GetBytes(val);
        //    if (BitConverter.IsLittleEndian)
        //    {
        //        Array.Reverse(bytes);
        //    }
        //    return bytes;
        //}

        private long DotNetInt64(byte[] _bytes, int offset)
        {
            byte[] bytes = new byte[sizeof(long)];
            Buffer.BlockCopy(_bytes, offset, bytes, 0, sizeof(long));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt64(bytes); ;
        }

        private Address SolidityAddressToHederaAddress(byte[] _arg)
        {
            var bytes = new byte[32];
            Buffer.BlockCopy(_arg, 0, bytes, 12, _arg.Length);
            ReadOnlyMemory<byte> arg = new ReadOnlyMemory<byte>(bytes);
            // See EncodeAddressPart for packing notes
            var shardAsBytes = arg.Slice(12, 4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(shardAsBytes);
            }
            var shard = BitConverter.ToInt32(shardAsBytes);

            var realmAsBytes = arg.Slice(16, 8).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(realmAsBytes);
            }
            var realm = BitConverter.ToInt64(realmAsBytes);

            var numAsBytes = arg.Slice(24, 8).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(numAsBytes);
            }
            var num = BitConverter.ToInt64(numAsBytes);

            return new Address(shard, realm, num);
        }



        private IJournalEntryRecord OptimizedJournalEntryRecord(IJournalEntryRecord journalEntryRecord, bool clearAccounts)
        {
            IJournalEntryRecord optimizedJournalEntryRecord = null!;
            ulong totalDebits = 0;
            ulong totalCredits = 0;
            Dictionary<string, DLTGLAccount> UniqueDebitAccounts = new Dictionary<string, DLTGLAccount>();
            Dictionary<string, DLTGLAccount> UniqueCreditAccounts = new Dictionary<string, DLTGLAccount>();
            foreach (var dbacc in journalEntryRecord.JournalEntry!.DebtAccountList)
            {
                // Sum all DB accounts with amounts > 0
                if (dbacc.Amount.HasValue && dbacc.Amount.Value > 0)
                {
                    totalDebits += dbacc.Amount.HasValue ? dbacc.Amount.Value : 0;
                    // Add the account and its amount to the accountAmounts dictionary if not already there, otherwise combine same-account entries
                    if (!UniqueDebitAccounts.ContainsKey(dbacc.DLTGLAccountInfo?.DLTAddress!))
                    {
                        UniqueDebitAccounts.Add(dbacc.DLTGLAccountInfo?.DLTAddress!, dbacc);
                    }
                    else
                    {
                        UniqueDebitAccounts[dbacc.DLTGLAccountInfo?.DLTAddress!].Amount += dbacc.Amount.Value;
                    }
                }
            }
            foreach (var cracc in journalEntryRecord.JournalEntry!.CreditAccountList)
            {
                // Sum all CR accounts with amounts > 0
                if (cracc.Amount > 0)
                {
                    totalCredits += cracc.Amount.HasValue ? cracc.Amount.Value : 0;
                    // Add the account and its amount to the accountAmounts dictionary if not already there, otherwise combine same-account entries
                    if (!UniqueCreditAccounts.ContainsKey(cracc.DLTGLAccountInfo?.DLTAddress!))
                    {
                        UniqueCreditAccounts.Add(cracc.DLTGLAccountInfo?.DLTAddress!, cracc);
                    }
                    else
                    {
                        UniqueCreditAccounts[cracc.DLTGLAccountInfo?.DLTAddress!].Amount += cracc.Amount.Value;
                    }
                }
            }
            if (!clearAccounts && totalDebits != totalCredits)
            {
                throw new InvalidOperationException("Invalid journal entry - total debits don't equal total credits.");
            }
            // Now need to merge same-named DB and CR accounts if any in order to reduce the number of amount transfers that need to be made
            // and to avoid the Hedera 'AccountRepeatedInAccountAmounts' error
            List<string> sameNamedAccounts = new List<string>();
            foreach (var dbacc in UniqueDebitAccounts)
            {
                if (UniqueCreditAccounts.ContainsKey(dbacc.Key))
                {
                    sameNamedAccounts.Add(dbacc.Key);
                }
            }
            // Now process all samed-named accounts by 1) combining their values, 2) replacing the appropriate orignal DB/CR value, and 3) removing the other appropraite DB/CR value
            foreach (var snacc in sameNamedAccounts)
            {
                // Combine the DB and CR account 
                (long debitbal, long creditbal, bool isDebitAccount) = GLAccountUtilities.DetermineDebitCreditBalance(UniqueDebitAccounts[snacc], UniqueCreditAccounts[snacc]);
                if (debitbal == creditbal)
                {
                    // The DB and CR cancel each other out so remove them from both UniqueDebitAccounts and UniqueCreditAccounts
                    UniqueDebitAccounts.Remove(snacc);
                    UniqueCreditAccounts.Remove(snacc);
                }
                else if (debitbal == 0)
                {
                    // CR has the value so remove the account from UniqueDebitAccounts and update its value in UniqueCreditAccounts
                    UniqueDebitAccounts.Remove(snacc);
                    UniqueCreditAccounts[snacc].Amount = Convert.ToUInt64(creditbal);
                }
                else
                {
                    // DB has the value so remove the account from UniqueCreditAccounts and update its value in UniqueDebitAccounts
                    UniqueCreditAccounts.Remove(snacc);
                    UniqueDebitAccounts[snacc].Amount = Convert.ToUInt64(debitbal);
                }
            }
            // Check edge case where we have nothing to post due to account removals from the above processing loop
            if (UniqueCreditAccounts.Count == 0 || UniqueDebitAccounts.Count == 0)
            {
                throw new InvalidOperationException("Invalid journal entry - total debits cancel out total credits.");
            }


            //
            // **** %TODO% - Must check for edge case where the DBs and CRs balance overall, but not within a given stakeholder context
            //


            // If we make it here we have a valid journal entry (as far as debits and credits balancing goes) and we know how many unique account transfers we need

            List<DLTGLAccount> debitAccounts = new List<DLTGLAccount>();
            foreach (var dbacc in UniqueDebitAccounts)
            {
                debitAccounts.Add(dbacc.Value);
            }
            List<DLTGLAccount> creditAccounts = new List<DLTGLAccount>();
            foreach (var cracc in UniqueCreditAccounts)
            {
                creditAccounts.Add(cracc.Value);
            }
            optimizedJournalEntryRecord = new JournalEntryRecord
            {
                DLTTransactionReceiptID = journalEntryRecord.DLTTransactionReceiptID,
                Memo = journalEntryRecord.Memo,
                PostDate = journalEntryRecord.PostDate,
                IsAutoReversal = journalEntryRecord.IsAutoReversal,
                TransactionID = journalEntryRecord.TransactionID,
                JournalEntry = new JournalEntryAccounts
                {
                    CreditAccountList = creditAccounts,
                    DebtAccountList = debitAccounts
                }
            };
            return optimizedJournalEntryRecord;
        }


        private byte[]? CreateSmartContractParameterBytesFromJournalEntry(Address tokenAddress,
                                                                            Address tokenTreasuryAddress,
                                                                            IJournalEntryRecord journalEntryRecord,
                                                                            bool isReversal,
                                                                            int transactionBatchSize)
        {
            byte[] smParamBytes = null!;
            var uniqueAccountCount = journalEntryRecord.JournalEntry!.DebtAccountList!.Count + journalEntryRecord.JournalEntry!.CreditAccountList!.Count;
            // Allocate a byte[] big enough to contain all of the data need by the dAccounting Service Post Journal Entry smart contract
            smParamBytes = new byte[SOLIDITY_ADDRESS_SIZE +                         // Token address
                                        SOLIDITY_ADDRESS_SIZE +                         // Treasury address
                                        sizeof(long) +                                  // Max Transaction Batch Size
                                        sizeof(long) +                                  // Is reversing journal entry (i.e.; tokens flow into Treasury NOT out of Treasury)
                                        sizeof(long) +                                  // Number of address/amount pairings
                                        SOLIDITY_ADDRESS_SIZE * uniqueAccountCount +  // Addresses for each account involved in transfer 
                                        sizeof(long) * uniqueAccountCount];           // Amounts for each account involved in transfer
            // Now fill the byte[]
            int pos = 0;
            Buffer.BlockCopy(HederaAddressToSolidityAddress(tokenAddress), 0, smParamBytes, pos, SOLIDITY_ADDRESS_SIZE);         // Token Address
            pos += SOLIDITY_ADDRESS_SIZE;
            Buffer.BlockCopy(HederaAddressToSolidityAddress(tokenTreasuryAddress), 0, smParamBytes, pos, SOLIDITY_ADDRESS_SIZE); // Token Treasury Address
            pos += SOLIDITY_ADDRESS_SIZE;
            Buffer.BlockCopy(SolidityInt64(Convert.ToInt64(transactionBatchSize)), 0, smParamBytes, pos, sizeof(long));          // Transaction Batch Size
            pos += sizeof(long);
            Buffer.BlockCopy(SolidityInt64(Convert.ToInt64(isReversal ? 1 : 0)), 0, smParamBytes, pos, sizeof(long));          // Is a revering journal entry
            pos += sizeof(long);
            Buffer.BlockCopy(SolidityInt64(Convert.ToInt64(uniqueAccountCount)), 0, smParamBytes, pos, sizeof(long));            // Number of account/amount pairings
            pos += sizeof(long);
            foreach (var dbacc in journalEntryRecord.JournalEntry!.DebtAccountList!)
            {
                if (dbacc.Amount!.Value <= 0)
                {
                    throw new InvalidOperationException("Invalid account amount - all values must be > 0");
                }
                // DB account address
                Buffer.BlockCopy(HederaAddressToSolidityAddress(ParseAddress(dbacc.DLTGLAccountInfo?.DLTAddress!)), 0, smParamBytes, pos, SOLIDITY_ADDRESS_SIZE);
                pos += SOLIDITY_ADDRESS_SIZE;
                // DB account amount
                Buffer.BlockCopy(SolidityInt64(Convert.ToInt64(dbacc.Amount!.Value)), 0, smParamBytes, pos, sizeof(long));
                pos += sizeof(long);
            }
            foreach (var cracc in journalEntryRecord.JournalEntry!.CreditAccountList!)
            {
                if (cracc.Amount!.Value <= 0)
                {
                    throw new InvalidOperationException("Invalid account amount - all values must be > 0");
                }
                // CR account address
                Buffer.BlockCopy(HederaAddressToSolidityAddress(ParseAddress(cracc.DLTGLAccountInfo?.DLTAddress!)), 0, smParamBytes, pos, SOLIDITY_ADDRESS_SIZE);
                pos += SOLIDITY_ADDRESS_SIZE;
                // DB account amount
                Buffer.BlockCopy(SolidityInt64(Convert.ToInt64(cracc.Amount!.Value)), 0, smParamBytes, pos, sizeof(long));
                pos += sizeof(long);
            }
            return smParamBytes;
        }

        private async Task<TransactionReceipt> DeleteAccountAsync(Address payor, Signatory payorSignatory, Address addressToDelete, Address addressToTransferBalanceTo, Signatory accountSignatory)
        {
            TransactionReceipt trxReceipt = null!;
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);                           // Random Hedera Gateway
                    ctx.Payer = payor;
                    ctx.Signatory = payorSignatory;
                });
                trxReceipt = await client.DeleteAccountAsync(addressToDelete, addressToTransferBalanceTo, accountSignatory).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.Print($"Error in HederaManager.DeleteAccountAsync() - {ex.ToString()}");
                throw;
            }
            return trxReceipt;
        }


        //private async Task<ResponseCode> GrantTokenKycOnAccountAsync(Address payor, Signatory payorSignatory, Address tokenAddress, Address addressToGrantKycTo, Signatory accountSignatory)
        //{
        //    ResponseCode result = ResponseCode.AuthorizationFailed;
        //    try
        //    {
        //        await using var client = new Client(ctx =>
        //        {
        //            ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);                           // Random Hedera Gateway
        //            ctx.Payer = payor;                                                
        //            ctx.Signatory = payorSignatory;                                        
        //        });

        //        var receipt = await client.GrantTokenKycAsync(tokenAddress, addressToGrantKycTo, accountSignatory);
        //        result = receipt.Status;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Print($"Error in HederaManager.GrantTokenKycOnAccountAsync() - {ex.ToString()}");
        //        throw;
        //    }
        //    return result;
        //}

        //private async Task<TransactionReceipt> AssociateAccountWithTokenAsync(Address payor, Signatory payourSignatory, Address tokenAddress,  Address addressToAssociateToToken,  Signatory accountSignatory )
        //{
        //    TransactionReceipt trxReceipt = null!;
        //    try
        //    {
        //        await using var client = new Client(ctx =>
        //        {
        //            ctx.Gateway = DetermineGatewayToUse(HederaConfig?.Gateways!);                 // Random Hedera Gateway
        //            ctx.Payer = payor;                          
        //            ctx.Signatory = payourSignatory;                              
        //        });
        //        trxReceipt = await client.AssociateTokenAsync(tokenAddress, addressToAssociateToToken, accountSignatory).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Print($"Error in HederaManager.AssociateAccountWithTokenAsync() - {ex.ToString()}");
        //        throw;
        //    }
        //    return trxReceipt;
        //}



        static private Gateway DetermineGatewayToUse(string possibleGateways)
        {
            Gateway result = null!;
            // eg: "0.0.3|34.94.106.61:50211;"
            string[] candidateGateways = possibleGateways.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var r = new Random(DateTime.Now.Second);
            if (candidateGateways != null && candidateGateways.Length > 0)
            {
                long[] shards = new long[candidateGateways.Length];
                long[] realms = new long[candidateGateways.Length];
                long[] accounts = new long[candidateGateways.Length];
                string[] urls = new string[candidateGateways.Length];
                int index = 0;
                foreach (var g in candidateGateways)
                {
                    string[] parts = g.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        string[] accountParts = parts[0].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (accountParts.Length == 3)
                        {
                            shards[index] = long.Parse(accountParts[0]);
                            realms[index] = long.Parse(accountParts[1]);
                            accounts[index] = long.Parse(accountParts[2]);
                            urls[index] = parts[1];
                            index++;
                        }
                    }
                }
                var randomGateway = r.Next(0, index);
                result = new Gateway(urls[randomGateway], shards[randomGateway], realms[randomGateway], accounts[randomGateway]);
            }
            return result;
        }

        private static byte[] EncodeInt64Part(object value)
        {
            var bytes = new byte[32];
            WriteInt256(bytes.AsSpan(), Convert.ToInt64(value));
            return bytes;
        }

        private static void WriteInt256(Span<byte> buffer, long value)
        {
            var valueAsBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueAsBytes);
            }
            valueAsBytes.CopyTo(buffer.Slice(24));
        }

        private byte[] SolidityInt64(long val)
        {
            var bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }


        private byte[] HederaAddressToSolidityAddress(Address hederaAddress)
        {
            byte[] solidityAddress = new byte[20];

            var bytes = new byte[32];
            var shard = BitConverter.GetBytes(hederaAddress.ShardNum);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(shard);
            }
            shard[^4..^0].CopyTo(bytes, 12);
            var realm = BitConverter.GetBytes(hederaAddress.RealmNum);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(realm);
            }
            realm.CopyTo(bytes, 16);
            var num = BitConverter.GetBytes(hederaAddress.AccountNum);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(num);
            }
            num.CopyTo(bytes, 24);
            Buffer.BlockCopy(bytes, 12, solidityAddress, 0, 20);
            return solidityAddress;
        }
        #endregion
    }


    public class HederaDLTKeyPair
    {
        #region Field Members
        private byte[] publicKey = null!;
        private byte[] privateKey = null!;
        private byte[] publicKeyPrefix = Hex.ToBytes("302a300506032b6570032100").ToArray();
        private byte[] privateKeyPrefix = Hex.ToBytes("302e020100300506032b657004220420").ToArray();
        #endregion

        public HederaDLTKeyPair()
        {
            var keyPairGenerator = new Ed25519KeyPairGenerator();
            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            var keyPair = keyPairGenerator.GenerateKeyPair();
            var privateKeyParameter = keyPair.Private as Ed25519PrivateKeyParameters;
            var publicKeyParameter = keyPair.Public as Ed25519PublicKeyParameters;
            if (publicKeyParameter != null)
            {
                publicKey = publicKeyParameter.GetEncoded().ToArray();
            }
            if (privateKeyParameter != null)
            {
                privateKey = privateKeyParameter.GetEncoded().ToArray();
            }
        }

        public HederaDLTKeyPair(byte[] publickey, byte[] privatekey)
        {
            if (publickey == null)
            {
                throw new ArgumentException("Invalid Public and/or Public keys.");
            }
            else if (privatekey == null)
            {
                throw new ArgumentException("Invalid Public and/or Public keys.");
            }
            else if (publickey.Length == 44 && privatekey.Length == 48 || privatekey != null && privatekey.Length == 48 & publickey == null || publickey != null && publickey.Length == 44 & privatekey == null)
            {
                if (publickey != null)
                {
                    publicKey = new byte[32];
                    for (int i = 12; i < 44; i++)
                    {
                        publicKey[i - 12] = publickey[i];
                    }
                }
                if (privatekey != null)
                {
                    privateKey = new byte[32];
                    for (int i = 16; i < 48; i++)
                    {
                        privateKey[i - 16] = privatekey[i];
                    }
                }
            }
            else
            {
                if (publickey != null && publickey.Length == 32 && privatekey != null && privatekey.Length == 32 || publickey != null && publickey.Length == 32 & privatekey == null || privatekey != null && privatekey.Length == 32 & publickey == null)
                {
                    publicKey = publickey!;
                    privateKey = privatekey!;
                }
                else
                {
                    throw new ArgumentException("Invalid Public and/or Public keys.");
                }
            }
        }


        public HederaDLTKeyPair(string base64encryptedpublickey, string base64encryptedprivatekey)
        {
            if (base64encryptedpublickey == null || base64encryptedpublickey.Length == 88 || base64encryptedpublickey.Length == 344 || base64encryptedprivatekey == null || base64encryptedprivatekey.Length == 96 || base64encryptedprivatekey.Length == 344)
            {
                if (base64encryptedpublickey != null)
                {
                    if (base64encryptedpublickey.Length == 88)
                    {
                        publicKey = Hex.ToBytes(base64encryptedpublickey.Substring(24)).ToArray();
                    }
                    else
                    {
                        publicKey = HostCryptology.AsymmetricDecryptionWithoutCertificate(
                                    Convert.FromBase64String(base64encryptedpublickey), HederaManager.MasterSecretVault["MASTER_PRIVATE_KEY"]!);

                    }
                }
                if (base64encryptedprivatekey != null)
                {
                    if (base64encryptedprivatekey.Length == 96)
                    {
                        privateKey = Hex.ToBytes(base64encryptedprivatekey.Substring(32)).ToArray();
                    }
                    else
                    {
                        privateKey = HostCryptology.AsymmetricDecryptionWithoutCertificate(
                                    Convert.FromBase64String(base64encryptedprivatekey), HederaManager.MasterSecretVault["MASTER_PRIVATE_KEY"]!);
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid Public and/or Public keys.");
            }

        }

        public void Clear()
        {
            if (publicKey != null)
            {
                for (int i = 0; i < publicKey.Length; i++)
                {
                    publicKey[i] = 0;
                }
            }
            if (privateKey != null)
            {
                for (int i = 0; i < privateKey.Length; i++)
                {
                    privateKey[i] = 0;
                }
            }
        }


        public byte[] PublicKeyBytes
        {
            get { return publicKey; }
        }


        public byte[] PublicHederaKeyBytes
        {
            get
            {
                if (publicKey.Length == 44) // key already contains prefix?
                {
                    return publicKey;  // return as is
                }
                else
                {
                    // add the prefix
                    return publicKeyPrefix.Concat(publicKey).ToArray();
                }
            }
        }

        public byte[] PrivateKeyBytes
        {
            get { return privateKey; }
        }


        public byte[] PrivateHederaKeyBytes
        {
            get
            {
                if (privateKey.Length == 48)  // key already contains prefix?
                {
                    return privateKey;  // return as is
                }
                else
                {
                    // add the prefix
                    return privateKeyPrefix.Concat(privateKey).ToArray();
                }
            }
        }


        public string PublicKeyString
        {
            get
            {
                if (publicKey == null)
                    return null!;
                return Hex.FromBytes(publicKeyPrefix.Concat(publicKey).ToArray());
            }
        }

        public string PrivateKeyString
        {
            get
            {
                if (privateKey == null)
                    return null!;
                return Hex.FromBytes(privateKeyPrefix.Concat(privateKey).ToArray());
            }
        }

        public string Base64EncryptedPublicKeyString
        {
            get
            {
                if (publicKey == null)
                    return null!;
                return Convert.ToBase64String(HostCryptology.AsymmetricEncryptionWithoutCertificate(publicKeyPrefix.Concat(publicKey).ToArray(),
                                                HederaManager.MasterSecretVault["MASTER_PUBLIC_KEY"]!));
            }
        }

        public string Base64EncryptedPrivateKeyString
        {
            get
            {
                if (privateKey == null)
                    return null!;
                return Convert.ToBase64String(HostCryptology.AsymmetricEncryptionWithoutCertificate(privateKeyPrefix.Concat(privateKey).ToArray(),
                                                HederaManager.MasterSecretVault["MASTER_PUBLIC_KEY"]!));
            }
        }

        //private static string ByteArrayToHex(byte[] barray)
        //{
        //    char[] c = new char[barray.Length * 2];
        //    byte b;
        //    for (int i = 0; i < barray.Length; ++i)
        //    {
        //        b = ((byte)(barray[i] >> 4));
        //        c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
        //        b = ((byte)(barray[i] & 0xF));
        //        c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
        //    }
        //    return new string(c);
        //}
    }

}