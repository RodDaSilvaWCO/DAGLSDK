using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class JurisdictionMemberGeneralLedger : IJurisdictionMemberGeneralLedger
    {
        #region Field Members
        #endregion

        #region Constructors
        public JurisdictionMemberGeneralLedger() { }
        //public JurisdictionMemberGeneralLedger( JurisdictionMember member, GeneralLedger gl )
        //{
        //    JurisdictionMember = member;
        //    GeneralLedger = gl;
        //}
        #endregion

        #region Public Interface
        public GeneralLedger? GeneralLedger { get; set; }
        public JurisdictionMember? JurisdictionMember { get; set; }
        #endregion

        #region Helpers
        #endregion


    }
}
