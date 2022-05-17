using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class dAccountingServiceInstanceManifest : IdAccountingServiceInstanceManifest
    {
        #region Field Members
        #endregion

        #region Constructors
        public dAccountingServiceInstanceManifest() // required for serialization
        {
            CultureInfo = new CultureInfo("en-US"); // default
            //// Instance IDs
        }
        public dAccountingServiceInstanceManifest(  string daccountingserviceid,
                                                    string jurisdictionid, 
                                                    string culturename,
                                                    string daccountingservicecoastemplateid,
                                                    string jurisdictioncoastemplateid,
                                                    string jurisdictionmembercoastemplateid,
                                                    KeyVault jurisdictiontokenadminkeyvault,
                                                    //KeyVault jurisdictiontokengrantkyckeyvault,
                                                    //KeyVault jurisdictiontokensuspendkeyvault,
                                                    //KeyVault jurisdictiontokenconfiscatekeyvault,
                                                    //KeyVault jurisdictiontokensuplykeyvault,
                                                    DLTAddress daccountingserviceaddress,
                                                    DLTAddress jurisdictionpayoraddress,
                                                    DLTAddress jurisdictiontokentreasuryaddress,
                                                    DLTToken jurisidctiontoken,
                                                    DLTAddress jurisdictionjournalentrypostcontractaddress,
                                                    DLTCurrency jurisdictiondefaultaccountingcurrency,
                                                    FeeSchedule jurisdictionservicebuyertransactionfeeschedule,
                                                    FeeSchedule jurisdictionservicesellertransactionfeeschedule,
                                                    FeeSchedule jurisdictionservicemembercashinfeeschedule,
                                                    FeeSchedule jurisdictionservicemembercashoutfeeschedule,
                                                    FeeSchedule jurisdictionservicecashoutfeeschedule,
                                                    GeneralLedger jurisdictiongeneralleger,
                                                    GeneralLedger daccountingservicegeneralledger,
                                                    string testmember1ID,
                                                    string testmember2ID
                                                 )  : this()
        {
            // IDs
            JurisdictionID = daccountingserviceid;
            dAccountingServiceID = jurisdictionid;

            // Culture            
            CultureInfo = new CultureInfo(culturename); // default

            // IDs
            TestMember1ID = testmember1ID;
            TestMember2ID = testmember2ID;

            // Chart of Accounts Template
            dAccountingServiceCOAsTemplateID = daccountingservicecoastemplateid;
            JurisdictionCOAsTemplateID = jurisdictioncoastemplateid;
            JurisdictionMemberCOAsTemplateID = jurisdictioncoastemplateid;

            // Keys
            JurisdictionTokenAdminKeyVault = jurisdictiontokenadminkeyvault;
            //JurisdictionTokenGrantKycKeyVault = jurisdictiontokengrantkyckeyvault;
            //JurisdictionTokenSuspendKeyVault = jurisdictiontokensuspendkeyvault;
            //JurisdictionTokenConfiscateKeyVault = jurisdictiontokenconfiscatekeyvault;
            //JurisdictionTokenSupplyKeyVault = jurisdictiontokensuplykeyvault;

            // Addresses
            dAccountingServiceAddress = daccountingserviceaddress;
            JurisdictionPayorAddress = jurisdictionpayoraddress;
            JurisdictionTokenTreasuryAddress = jurisdictiontokentreasuryaddress;
            JursidictionJournalEntryPostContractAddress = jurisdictionjournalentrypostcontractaddress;

            // Fee Schedule
            JurisdictionServiceBuyerTransactionFeeSchedule = jurisdictionservicebuyertransactionfeeschedule;
            JurisdictionServiceSellerTransactionFeeSchedule = jurisdictionservicesellertransactionfeeschedule;
            JurisdictionServiceMemberCashInFeeSchedule = jurisdictionservicemembercashinfeeschedule;
            JurisdictionServiceMemberCashOutFeeSchedule = jurisdictionservicemembercashoutfeeschedule;
            JurisdictionServiceCashOutFeeSchedule = jurisdictionservicecashoutfeeschedule;
            

            // Jurisdiction General Ledger
            JurisdictionGeneralLedger = jurisdictiongeneralleger;

            // dAccounting Service General Ledger
            dAccountingServiceGeneralLedger = daccountingservicegeneralledger;

            // Jurisidction Token
            JurisdictionToken = jurisidctiontoken;

            // Jurisdiction Currency
            JurisdictionDefaultAccountngCurrency = jurisdictiondefaultaccountingcurrency;
        }
        #endregion

        #region Public Interface
        // Culture
        public string? CultureName
        {
            get
            {
                return CultureInfo.Name;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    CultureInfo = new CultureInfo(value);
                }
            }
        }

        [JsonIgnore]
        public CultureInfo CultureInfo { get; set; }

        // Instance IDs
        public string? JurisdictionID { get; set; }
        public string? dAccountingServiceID { get; set; }
        public string? TestMember1ID { get; set; }
        public  string? TestMember2ID { get; set; }


        // Chart of Accounts Template
        public string? dAccountingServiceCOAsTemplateID { get; set; }
        public string? JurisdictionCOAsTemplateID { get; set; }
        public string? JurisdictionMemberCOAsTemplateID { get; set; }
        // Keys
        //public Dictionary<GLAccountCode, KeyVault>? dAccountingServiceCoaKeys { get;  set; }
        //public Dictionary<GLAccountCode, KeyVault>? JurisdictionCoaKeys { get;  set; }
        public KeyVault? JurisdictionTokenAdminKeyVault { get; set; }
        //public KeyVault? JurisdictionTokenGrantKycKeyVault { get; set; }
        //public KeyVault? JurisdictionTokenSuspendKeyVault { get; set; }
        //public KeyVault? JurisdictionTokenConfiscateKeyVault { get; set; }
        //public KeyVault? JurisdictionTokenSupplyKeyVault { get; set; }

        // Jurisdiction Addresses
        public DLTAddress? JurisdictionPayorAddress { get; set; }       // NOTE:  Supplied externally by Jurisdiction - used as Payor for all transactions
        public DLTAddress? JurisdictionTokenTreasuryAddress { get; set; }
        public DLTAddress? JursidictionJournalEntryPostContractAddress { get; set; }

        // Fee Schedule
        public FeeSchedule? JurisdictionServiceBuyerTransactionFeeSchedule { get; set; }
        public FeeSchedule? JurisdictionServiceSellerTransactionFeeSchedule { get; set; }
        public FeeSchedule? JurisdictionServiceMemberCashInFeeSchedule { get; set; }
        public FeeSchedule? JurisdictionServiceMemberCashOutFeeSchedule { get; set; }
        public FeeSchedule? JurisdictionServiceCashOutFeeSchedule { get; set; }

        // Jurisdiction General Ledger
        public GeneralLedger? JurisdictionGeneralLedger { get; set; }

        // dAccounting Service General Ledger
        public GeneralLedger? dAccountingServiceGeneralLedger { get; set; }

        // Jurisdiction Token
        public DLTToken? JurisdictionToken { get; set; }

        // Jurisdiction Currency
        public DLTCurrency? JurisdictionDefaultAccountngCurrency { get; set; }

        // dAccounting Service Address
        public DLTAddress? dAccountingServiceAddress { get; set; }


        #endregion

        #region Helpers
        #endregion


    }
}
