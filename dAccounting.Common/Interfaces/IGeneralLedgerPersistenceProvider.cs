using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IGeneralLedgerPersistenceProvider
    {
        string? ServiceInstancePath { get; set; }
        //void Initialize(IAccountingContainer accountingContainer, string connectionString = null! );

        // RWD for General Ledger Table
        Task<IGeneralLedger> ReadGeneralLedgerAsync(string id);
        Task WriteGeneralLedgerAsync( IGeneralLedger gltowrite );
        void DeleteGeneralLedger(string id);

        // RWD for Member Table
        Task<IJurisdictionMember> ReadMemberAsync(string id);
        Task WriteMemberAsync(IJurisdictionMember membertowrite);
        void DeleteMember(string id);

        // CAD for Journal Table
        void CreateJournal(string generalLedgerId );
        Task<List<IJournalEntryRecord>> ReadJournalAsync( string generalLedgerId, Func<string, Task<JurisdictionMemberGeneralLedger>> MemberLookUp);
        Task AppendJournalAsync( IJournalEntryRecord journalentryrecortdtoappend);
        void DeleteJournal(string id);

        // CRD for Jurisdiction Instance Manifest Table
        Task CreateJurisdictionInstanceManifestAsync(IdAccountingServiceInstanceManifest serviceInstanceManifest);
        IdAccountingServiceInstanceManifest ReadJurisdictionInstanceManifest();
        void DeleteJurisdictionInstanceManifest();
    }
}
