using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class DLTCurrency : IDLTCurrency
    {
        #region Field Members
        #endregion

        #region Constructors
        public DLTCurrency() { } // required for serialization

        public DLTCurrency( string? symbol, int decimals)
        {
            Symbol = symbol;    
            Decimals = decimals;
        }
        #endregion

        #region Public Interface
        public int Decimals { get; set; }
        public string? Symbol { get; set; }

        #endregion

        #region Helpers
        #endregion
    }
}
