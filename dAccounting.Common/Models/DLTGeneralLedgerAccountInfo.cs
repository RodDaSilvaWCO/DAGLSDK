using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models 
{
    public class DLTGeneralLedgerAccountInfo : IDLTGLAccountInfo
    {
        #region Field Members
        #endregion

        #region Constructors
        public DLTGeneralLedgerAccountInfo( GLAccountCode code, string dltaddress )
        {
            Code = code;
            DLTAddress = dltaddress;
        }
        #endregion

        #region Public Interface
        public GLAccountCode Code { get; set; }

        public string? DLTAddress { get; set; }
        #endregion

        #region Helpers
        #endregion

    }
}
