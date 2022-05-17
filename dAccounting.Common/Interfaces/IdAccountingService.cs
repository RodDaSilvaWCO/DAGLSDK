using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IdAccountingService
    {
        Task<dAccountingServiceInstanceManifest> GenerateInstanceManifestAsync(     string dAccountingServiceID,
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


    }
}
