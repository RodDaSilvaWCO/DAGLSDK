using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class JurisdictionMember : IJurisdictionMember
    {
        #region Field Members
        #endregion

        #region Constructors
        public JurisdictionMember() { } // required for serialization
        public JurisdictionMember( string id, string glid, string description, DLTAddress membercryptoaddress )
        {
            ID = id;
            GeneralLedgerID = glid;
            Description = description;
            CryptoAddress = membercryptoaddress;    
        }
        #endregion

        #region Public Interface
        public string? ID { get; set; }
        public string? GeneralLedgerID { get; set; }
        public string? Description { get; set; }
        public DLTAddress? CryptoAddress { get; set; }
        #endregion

        #region Helpers
        #endregion
    }
}
