using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class AccountingContainer : IAccountingContainer
    {
        #region Field Members
        #endregion

        #region Constructors
        public AccountingContainer() { } 
        #endregion

        #region Public Interface
        public IdAccountingServiceInstanceManifest? InstanceManifest { get; set; }
        #endregion

        #region Helpers
        #endregion
    }
}
