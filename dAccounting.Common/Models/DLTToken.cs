using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class DLTToken : IDLTToken
    {
        #region Field Members
        #endregion

        #region Constructors
        public DLTToken() { }  // required for serialization
        public DLTToken( string name, DLTAddress address, string status )
        {
            Name = name;
            Address = address;
            Status = status;
        }
        #endregion

        #region Public Interface
        public string? Name { get;  set; }
        public DLTAddress? Address { get; set; }
        [JsonIgnore]
        public string? Status { get; set; }
        #endregion

        #region Helpers
        #endregion
    }
}
