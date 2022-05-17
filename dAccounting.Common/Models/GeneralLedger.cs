using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;
using dAccounting.Common.Models;

namespace dAccounting.Common.Models
{
    public class GeneralLedger : IGeneralLedger
    {
        #region Field Members
        #endregion

        #region Constructors
        public GeneralLedger() { }  // for serialization
        #endregion

        #region Public Interface
        public string? ID { get; set; }
        public string? Description { get; set; }
        public List<DLTGeneralLedgerAccountInfo>? DebitChartOfAccounts { get; set; }
        public List<DLTGeneralLedgerAccountInfo>? CreditChartOfAccounts { get; set; }
        public string? COATemplateID { get; set; }
        #endregion

        #region Helpers
        #endregion

    }
}
