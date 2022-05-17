using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IJurisdictionMemberGeneralLedger
    {
        GeneralLedger? GeneralLedger { get; set; }
        JurisdictionMember? JurisdictionMember { get; set; }
    }
}
