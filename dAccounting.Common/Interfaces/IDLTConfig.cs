using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Interfaces
{
    public interface IDLTConfig
    {
        
        string? dAccoutingServiceRoyaltyAddress { get; set; }
        string? JurisdictionServicePayorAddress { get; set;}
        string? JurisdictionServicePayorEncryptedPublicKey { get; set;}
        string? JurisdictionServicePayorEncryptedPrivateKey { get; set; }
        string? Gateways { get; set;}
  
    }
}
