using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class GeneralLedgerAccountsCatalog : IGeneralLedgerAccountsCatalog
    {
        #region Field Members
        
        #endregion

        #region Constructors
        public GeneralLedgerAccountsCatalog() 
        {
            GeneralLedgerAccountCatalog = new List<GeneralLedgerAccountProperties>();
        }  
        #endregion

        #region Public Interface
        public List<GeneralLedgerAccountProperties>? GeneralLedgerAccountCatalog { get; set; }

        public void AddGeneralLedgerAccount( GeneralLedgerAccountProperties accountproperties)
        {
            GeneralLedgerAccountCatalog!.Add(accountproperties);
        }

        public GeneralLedgerAccountProperties? this[GLAccountCode code] 
        { 
            get
            {
                GeneralLedgerAccountProperties result = null!;
                foreach( var glaccountProp in GeneralLedgerAccountCatalog!)
                {
                    if( (GLAccountCode)glaccountProp.Code == code )
                    {
                        result = glaccountProp;
                    }
                }
                return result;
            }
        }
        #endregion

        #region Helpers
        #endregion
    }
}
