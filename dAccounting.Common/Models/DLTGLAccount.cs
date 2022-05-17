using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class DLTGLAccount : IDLTGLAccount
    {
        #region Field Members
        #endregion

        #region Constructors
        public DLTGLAccount( JurisdictionMember member, DLTGeneralLedgerAccountInfo dltglaccountinfo, ulong? amount = 0 )
        {
            JurisdictionMember = member;
            DLTGLAccountInfo = dltglaccountinfo;
            Amount = amount;
        }

        public DLTGLAccount() { }  // Required for serialization
        #endregion

        #region Public Interface
        public JurisdictionMember? JurisdictionMember { get; set; }
        public DLTGeneralLedgerAccountInfo? DLTGLAccountInfo { get; set; }

        public ulong? Amount { get; set; }
        #endregion

        #region Helpers
        #endregion

    }
}
