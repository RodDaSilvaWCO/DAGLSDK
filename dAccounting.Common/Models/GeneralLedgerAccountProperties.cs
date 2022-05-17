using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace dAccounting.Common.Models
{
    public class GeneralLedgerAccountProperties : IGeneralLedgerAccountProperties
    {
        #region Field Members
        #endregion

        #region Constructors
        public GeneralLedgerAccountProperties() { } // required for searialization

        public GeneralLedgerAccountProperties( GLAccountCode code, GLAccountType type, bool omitfromreportingifzero = false)
        {
            Code = (int)code;
            Type = (int)type;
            OmitFromReportingIfZero = omitfromreportingifzero;
        }
        #endregion

        #region Public Interface
        [JsonPropertyName("C")]
        public int Code { get; set; }
        [JsonPropertyName("T")]
        public int Type { get; set; }

        [JsonPropertyName("O")]
        public bool OmitFromReportingIfZero { get; set; }


        //public string? GetSemanticName() 
        //{ 
        //   return Type.ToString();
        //}


        public string GetLocalizedName( CultureInfo culture = null! )
        {
            return LocalizedStringTable.LookUpLocalizedString( ( culture != null ? (int)culture.LCID :1033 ), Code );
        }

        #endregion

        #region Helpers
        #endregion
    }
}
