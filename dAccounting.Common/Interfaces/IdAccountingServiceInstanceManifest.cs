using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IdAccountingServiceInstanceManifest
    {
        // Instance ID
        string? JurisdictionID { get; set; }
        string? dAccountingServiceID { get; set; }
        string? TestMember1ID { get; set; }
        string? TestMember2ID { get; set; }

        // Culture
        string? CultureName { get; set; }

        [JsonIgnore]
        CultureInfo CultureInfo { get; set; }

        // Chart of Accounts Template
        string? dAccountingServiceCOAsTemplateID { get; set; }
        string? JurisdictionCOAsTemplateID { get; set; }
        string? JurisdictionMemberCOAsTemplateID { get; set; }

        // Required Keys
        KeyVault? JurisdictionTokenAdminKeyVault { get; set; }
        //KeyVault? JurisdictionTokenGrantKycKeyVault { get; set; }
        //KeyVault? JurisdictionTokenSuspendKeyVault { get; set; }
        //KeyVault? JurisdictionTokenConfiscateKeyVault { get; set; }
        //KeyVault? JurisdictionTokenSupplyKeyVault { get; }

        // Jurisidiction Addresses
        DLTAddress? JurisdictionPayorAddress { get; set; }  // NOTE:  Supplied externally by Jurisdiction - used as Payor for all transactions
        DLTAddress? JurisdictionTokenTreasuryAddress { get; set; }
        DLTAddress? JursidictionJournalEntryPostContractAddress { get; set; }

        // Fee Schedules
        FeeSchedule? JurisdictionServiceBuyerTransactionFeeSchedule { get; set; }
        FeeSchedule? JurisdictionServiceSellerTransactionFeeSchedule { get; set; }
        FeeSchedule? JurisdictionServiceMemberCashInFeeSchedule { get; set; }
        FeeSchedule? JurisdictionServiceMemberCashOutFeeSchedule { get; set; }
        FeeSchedule? JurisdictionServiceCashOutFeeSchedule { get; set; }
        
        // Jurisdiction General Ledger
        GeneralLedger? JurisdictionGeneralLedger { get; set; }

        // dAccounting Service General Ledger
        GeneralLedger? dAccountingServiceGeneralLedger { get; set; }

        // Jurisdiction Token
        DLTToken? JurisdictionToken { get; set; }

        // Jurisidiction Currency 
        DLTCurrency? JurisdictionDefaultAccountngCurrency { get; set; }

        // dAccountingService Address
        DLTAddress? dAccountingServiceAddress { get; set; } // NOTE:  Supplied externally by dAccounting Service - used to post paid royalties 




    }
}
