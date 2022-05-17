using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class DLTConfig : IDLTConfig
    {
        #region Field Members
        
        #endregion

        #region Constructors
        public DLTConfig()  // requried for serialization
        {
            
        } 
        #endregion

        #region Public Interface
       
        public string? dAccoutingServiceRoyaltyAddress { get; set; }
        public string? JurisdictionServicePayorAddress { get; set; }
        public string? JurisdictionServicePayorEncryptedPublicKey { get; set; }
        public string? JurisdictionServicePayorEncryptedPrivateKey { get; set; }
        public string? Gateways { get; set; }
        #endregion

        #region Helpers
        #endregion
    }
}
