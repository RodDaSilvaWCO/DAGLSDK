using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class DLTAddress : IDLTAddress
    {
        #region Field Members
        #endregion

        #region Constructors
        public DLTAddress( string addressid)
        {
            AddressID = addressid;
            Status = "UNINITALIZED";
        }

        public DLTAddress(string addressid, KeyVault keyvault) : this(addressid)
        {
            KeyVault = keyvault;
        }

        public DLTAddress(string addressid, KeyVault keyvault, string status ) : this(addressid, keyvault)
        {
            Status = status;
        }

        public DLTAddress( string addressid, string? publickey, string? privatekey )
        {
            AddressID = addressid;
            if( publickey == null && privatekey == null)
            {
                throw new ArgumentNullException("A DLTAddress cannot have both its publickey and its privatekey null.");
            }
            KeyVault = new KeyVault(publickey, privatekey);
        }
        public DLTAddress() { }   // needed for searilzation
        #endregion

        #region Public Interface
        public string? AddressID { get; set; }
        public KeyVault? KeyVault { get; set; }
        [JsonIgnore]
        public string? Status { get; set; }
        #endregion

        #region Helpers
        #endregion
    }
}
