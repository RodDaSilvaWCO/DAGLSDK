using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using dAccounting.Common.Interfaces;
using dAccounting.Common.Utilities;
using System.Text.Json.Serialization;
using System.Globalization;

namespace dAccounting.Common.Models
{
    public class JournalEntryAccounts : IJournalEntryAccounts
    {
        #region Field Members
        //private static IdGeneralLedgerServiceParameters GLSParameters = null!;
        #endregion

        #region Constructors
        public JournalEntryAccounts(/*IdGeneralLedgerServiceParameters glsparams */)
        {
            //GLSParameters = glsparams;
            DebtAccountList = new List<DLTGLAccount>();
            CreditAccountList = new List<DLTGLAccount>();
        }

        public JournalEntryAccounts(/*IdGeneralLedgerServiceParameters glsparams,*/ List<DLTGLAccount> debitAccountList, List<DLTGLAccount> creditAccountList ) : this(/*glsparams*/)
        {
            DebtAccountList = debitAccountList;
            CreditAccountList = creditAccountList;
        }

        #endregion

        #region Public Interface
        public List<DLTGLAccount> DebtAccountList { get; set; }
        public List<DLTGLAccount> CreditAccountList { get; set; }


        public void AddDoubleEntry( List<DLTGLAccount> debitaccounts, List<DLTGLAccount> creditaccounts, bool assertBalanced = true)
        {
            Debug.Assert(debitaccounts != null!);
            Debug.Assert(creditaccounts != null!);
            if (assertBalanced)
            {
                Debug.Assert(debitaccounts.Count > 0);  // Must have at least one debit  - accept if assertBalanced = false (used for reset)
                Debug.Assert(creditaccounts.Count > 0); // Must have at least one credit - accept if assertBalanced = false (used for reset)
            }

            // Ensure accounts balance
            ulong? debitBalance = 0L;
            foreach( var debitaccount in debitaccounts)
            {
                debitBalance += debitaccount.Amount;
            }
            ulong? creditBalance = 0L;
            foreach (var creditaccount in creditaccounts)
            {
                creditBalance += creditaccount.Amount;
            }
            if (assertBalanced)
            {
                Debug.Assert(debitBalance == creditBalance);  // Debits must balance with credits - accept if assertBalanced = false (used for reset)
            }

            // Add the debit and credit accounts to the list of debit and credit accounts being built up in this journal entry
            foreach (var debitaccount in debitaccounts)
            {
                DebtAccountList.Add(debitaccount);
            }
            foreach (var creditaccount in creditaccounts)
            {
                CreditAccountList.Add(creditaccount);
            }

        }


        public static string GenerateJournalEntriesReport(List<IJournalEntryRecord> journalEntriesToReport,
                                                            CultureInfo cultureinfo,
                                                            DLTCurrency currency,
                                                            IGeneralLedgerAccountsCatalog glcatalog,
                                                            bool fullDltAudit = false,
                                                            int startingOrdinal = 1 /*NOTE:  startingOrdinal is 1's based */ )
        {
            const int dltAddressWidth = 14;
            const int postDateWidth = 11;
            const int contextWidth = 32;
            const int accountWidth = 30;
            const int codeWidth = 8;
            const int DbCrWidth = 12;
            const int journalEntryOrdinalWidth = 12;
            const int memoWidth = 40;
            StringBuilder sb = new StringBuilder();
            long totalDebits = 0L;
            long totalCredits = 0L;
            var reportHeaderPrefix = StringUtilities.LeftJustifiedFormatFixedLength("As at " + DateTime.UtcNow.ToString("MMM-dd-yyyy") + " (utc)", accountWidth + postDateWidth + journalEntryOrdinalWidth + memoWidth, ' ') + " ";
            var totalSepLine = StringUtilities.FormatFixedLength("", reportHeaderPrefix.Length, ' ') +
                  StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t" +
                  StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t";
            var topLine = reportHeaderPrefix + StringUtilities.FormatFixedLength("Debits", DbCrWidth) + "\t" +
                                      StringUtilities.FormatFixedLength("Credits", DbCrWidth) + "\t";
            sb.AppendLine(topLine);
            var topSepLine = new string('-', totalSepLine.Length + 2);
            sb.AppendLine(topSepLine);
            // Main Loop through Journal Entries to report on starting from the startingOrdinal
            for( int je = 0 + (startingOrdinal-1); je < journalEntriesToReport.Count; je++ )
            {
                sb.AppendLine(StringUtilities.LeftJustifiedFormatFixedLength(journalEntriesToReport[je].PostDate.ToString("MMM-dd-yyyy"), postDateWidth) + "\t" +
                                StringUtilities.LeftJustifiedFormatFixedLength("J" + (je + 1).ToString(), journalEntryOrdinalWidth) + "\t" +
                                StringUtilities.LeftJustifiedFormatFixedLength(journalEntriesToReport[je].Memo! + $" - {journalEntriesToReport[je].TransactionID!}", memoWidth + 36));
                // Sort the account lists in place
                //List<DLTGLAccount>? dbacclist = journalEntriesToReport[je]?.JournalEntry?.DebtAccountList!.OrderBy(o => o.JurisdictionMember!.Description + o.DLTGLAccountInfo!.Code).ToList();
                //List<DLTGLAccount>? cracclist = journalEntriesToReport[je]?.JournalEntry?.CreditAccountList!.OrderBy(o => o.JurisdictionMember!.Description + o.DLTGLAccountInfo!.Code).ToList();
                journalEntriesToReport[je]?.JournalEntry?.DebtAccountList!.Sort((x, y) =>
                            (((int)(x.DLTGLAccountInfo!.Code)).ToString() + x.JurisdictionMember!.Description).CompareTo(((int)(y.DLTGLAccountInfo!.Code)).ToString() + y.JurisdictionMember!.Description));

                journalEntriesToReport[je]?.JournalEntry?.CreditAccountList!.Sort((x, y) =>
                            ( ((int)(x.DLTGLAccountInfo!.Code)).ToString() + x.JurisdictionMember!.Description ).CompareTo(((int)(y.DLTGLAccountInfo!.Code)).ToString() + y.JurisdictionMember!.Description ));

                //journalEntriesToReport[je]?.JournalEntry?.DebtAccountList!.Sort((x, y) =>
                //            (x.JurisdictionMember!.Description + ((int)(x.DLTGLAccountInfo!.Code)).ToString()).CompareTo(y.JurisdictionMember!.Description + ((int)(y.DLTGLAccountInfo!.Code)).ToString()));
                //journalEntriesToReport[je]?.JournalEntry?.CreditAccountList!.Sort((x, y) =>
                //            (x.JurisdictionMember!.Description + ((int)(x.DLTGLAccountInfo!.Code)).ToString()).CompareTo(y.JurisdictionMember!.Description + ((int)(y.DLTGLAccountInfo!.Code)).ToString()));

                // Loop through Debits of current journal entry
                foreach (var acc in journalEntriesToReport[je]?.JournalEntry?.DebtAccountList!)
                {
                    var coaProperties = glcatalog![acc.DLTGLAccountInfo!.Code];
                    if (coaProperties != null)
                    {
                        string accountName = coaProperties.GetLocalizedName();
                        var context = StringUtilities.FormatFixedLength((fullDltAudit ? acc.JurisdictionMember?.Description! : " "), contextWidth);
                        var code = StringUtilities.LeftJustifiedFormatFixedLength(fullDltAudit ? acc.DLTGLAccountInfo!.DLTAddress! +":" : " ", dltAddressWidth) + 
                                    StringUtilities.FormatFixedLength(((int)acc.DLTGLAccountInfo!.Code).ToString(), codeWidth, ' ') + " " +
                                     StringUtilities.LeftJustifiedFormatFixedLength(accountName, accountWidth, ' ');
                        
                        //var code = StringUtilities.FormatFixedLength(entry.DLTGLAccountInfo!.Code.ToString() + ":" + entry.DLTGLAccountInfo!.DLTAddress!, codeWidth);
                        var db = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount((long)acc.Amount!.Value), DbCrWidth);
                        var cr = StringUtilities.FormatFixedLength("-", DbCrWidth);
                        sb.AppendLine($"{context}\t{code}\t{db}\t{cr}");
                    }
                    totalDebits += (long)acc.Amount!;
                }
                // Loop through Credits of current journal entry
                foreach (var acc in journalEntriesToReport[je]?.JournalEntry?.CreditAccountList!)
                {
                    var coaProperties = glcatalog![acc.DLTGLAccountInfo!.Code];
                    if (coaProperties != null)
                    {
                        string accountName = coaProperties.GetLocalizedName();
                        var context = StringUtilities.FormatFixedLength((fullDltAudit ? acc.JurisdictionMember?.Description! : " "), contextWidth);
                        var code = StringUtilities.LeftJustifiedFormatFixedLength(fullDltAudit ? acc.DLTGLAccountInfo!.DLTAddress! + ":" : " ", dltAddressWidth) +
                                    StringUtilities.FormatFixedLength(((int)acc.DLTGLAccountInfo!.Code).ToString(), codeWidth, ' ') + " " +
                                     StringUtilities.LeftJustifiedFormatFixedLength(accountName, accountWidth, ' ');

                        var db = StringUtilities.FormatFixedLength("-", DbCrWidth);
                        var cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount((long)acc.Amount!.Value), DbCrWidth);
                        sb.AppendLine($"{context}\t{code}\t{db}\t{cr}");
                    }
                    totalCredits += (long)acc.Amount!;
                }
                sb.AppendLine();
            }

            var dummyContext = StringUtilities.FormatFixedLength(" ", contextWidth);
            var dummyCode = StringUtilities.LeftJustifiedFormatFixedLength(" ", dltAddressWidth) +
                        StringUtilities.FormatFixedLength(" ", codeWidth, ' ') + " " +
                         StringUtilities.LeftJustifiedFormatFixedLength(" ", accountWidth, ' ');
            var totalDb = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalDebits), DbCrWidth);
            var totalCr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalCredits), DbCrWidth);

            //var totalSepLine = StringUtilities.FormatFixedLength("", codeWidth, ' ') + " " +
            //                  StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + "\t" +
            //                  StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t" +
            //                  StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t";

            sb.AppendLine(totalSepLine);
            sb.AppendLine();
            sb.AppendLine($"{dummyContext}\t{dummyCode}\t{totalDb}\t{totalCr}");
            //var totalLine = StringUtilities.FormatFixedLength("", codeWidth, ' ') + " " +
            //                            StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + "\t" +
            //                            StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalDebits), DbCrWidth) + "\t" +
            //                            StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalCredits), DbCrWidth) + "\t";
            //sb.AppendLine(totalLine);
            var totalDoubleSepLine = StringUtilities.FormatFixedLength("", reportHeaderPrefix.Length, ' ') +
                            StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth) + "\t" +
                            StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth) + "\t";
            //var totalDoubleSepLine = StringUtilities.FormatFixedLength("", codeWidth, ' ') + " " +
            //                StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + "\t" +
            //                StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth) + "\t" +
            //                StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth) + "\t";
            sb.AppendLine(totalDoubleSepLine);

            //int accCount = 0;
            //foreach (var entry in DebtAccountList)
            //{
            //    var context = StringUtilities.FormatFixedLength(entry.JurisdictionMember?.Description!, contextWidth);
            //    var code = StringUtilities.FormatFixedLength(entry.DLTGLAccountInfo!.Code.ToString() + ":" + entry.DLTGLAccountInfo!.DLTAddress!, codeWidth);
            //    var db = StringUtilities.FormatFixedLength(entry.Amount!.Value.ToString(), DbCrWidth);
            //    var cr = StringUtilities.FormatFixedLength(null!, DbCrWidth);
            //    sb.Append($"|\t{context}\t|\t{code}\t|\t{db}\t|\t{cr}\t|" + Environment.NewLine);
            //    totalDebits += entry.Amount;
            //    accCount++;
            //}
            //accCount = 0;
            //foreach (var entry in CreditAccountList)
            //{
            //    var context = StringUtilities.FormatFixedLength(entry.JurisdictionMember?.Description!, contextWidth);
            //    var code = StringUtilities.FormatFixedLength(entry.DLTGLAccountInfo!.Code.ToString() + ":" + entry.DLTGLAccountInfo!.DLTAddress!, codeWidth);
            //    var db = StringUtilities.FormatFixedLength(null!, DbCrWidth);
            //    var cr = StringUtilities.FormatFixedLength(entry.Amount!.Value.ToString(), DbCrWidth);

            //    sb.Append($"|\t{context}\t|\t{code}\t|\t{db}\t|\t{cr}\t|" + Environment.NewLine);
            //    totalCredits += entry.Amount;
            //    accCount++;
            //}
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            //sb.Append($"|\t{StringUtilities.FormatFixedLength(" ", contextWidth)}\t|\t{StringUtilities.FormatFixedLength(" ", codeWidth)}\t|\t{StringUtilities.FormatFixedLength(totalDebits.Value!.ToString(), DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength(totalCredits.Value!.ToString(), DbCrWidth)}\t|" + Environment.NewLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            //sb.Append(Environment.NewLine);
            //if (totalCredits != totalDebits)
            //{
            //    sb.Append($" *** E R R O R: @ {DateTime.UtcNow.ToString()} (utc)  ***  Invalid Journal Entry - total Debits do NOT match total Credits.");
            //}
            //else
            //{
            //    sb.Append($" Valid Journal Entry @ {DateTime.UtcNow.ToString()} (utc)." + Environment.NewLine + Environment.NewLine);
            //}
            return sb.ToString();
        }

        public string DumpJournalEntry()
        {
            const int contextWidth = 32;
            const int tableWidth = 144;
            const int codeWidth = 38;
            const int DbCrWidth = 12;
            StringBuilder sb = new StringBuilder();
            ulong? totalDebits = 0L;
            ulong? totalCredits = 0L;
            sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            sb.Append($"|\t{StringUtilities.FormatFixedLength("CONTEXT", contextWidth)}\t|\t{StringUtilities.FormatFixedLength("ACCOUNT", codeWidth)}\t|\t{StringUtilities.FormatFixedLength("DB", DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength("CR", DbCrWidth)}\t|" + Environment.NewLine);
            sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            int accCount = 0;
            foreach (var entry in DebtAccountList)
            {
                var context = StringUtilities.FormatFixedLength(entry.JurisdictionMember?.Description!, contextWidth);
                var code = StringUtilities.FormatFixedLength(entry.DLTGLAccountInfo!.Code.ToString() + ":" + entry.DLTGLAccountInfo!.DLTAddress!, codeWidth);
                var db = StringUtilities.FormatFixedLength(entry.Amount!.Value.ToString(), DbCrWidth);
                var cr = StringUtilities.FormatFixedLength(null!, DbCrWidth);
                sb.Append($"|\t{context}\t|\t{code}\t|\t{db}\t|\t{cr}\t|" + Environment.NewLine);
                totalDebits += entry.Amount;
                accCount++;
            }
            accCount = 0;
            foreach (var entry in CreditAccountList)
            {
                var context = StringUtilities.FormatFixedLength(entry.JurisdictionMember?.Description!, contextWidth);
                var code = StringUtilities.FormatFixedLength(entry.DLTGLAccountInfo!.Code.ToString() + ":" + entry.DLTGLAccountInfo!.DLTAddress!, codeWidth);
                var db = StringUtilities.FormatFixedLength(null!, DbCrWidth);
                var cr = StringUtilities.FormatFixedLength(entry.Amount!.Value.ToString(), DbCrWidth);

                sb.Append($"|\t{context}\t|\t{code}\t|\t{db}\t|\t{cr}\t|" + Environment.NewLine);
                totalCredits += entry.Amount;
                accCount++;
            }
            sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            sb.Append($"|\t{StringUtilities.FormatFixedLength(" ", contextWidth)}\t|\t{StringUtilities.FormatFixedLength(" ", codeWidth)}\t|\t{StringUtilities.FormatFixedLength(totalDebits.Value!.ToString(), DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength(totalCredits.Value!.ToString(), DbCrWidth)}\t|" + Environment.NewLine);
            sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            sb.Append(Environment.NewLine);
            if (totalCredits != totalDebits)
            {
                sb.Append($" *** E R R O R: @ {DateTime.UtcNow.ToString()} (utc)  ***  Invalid Journal Entry - total Debits do NOT match total Credits.");
            }
            else
            {
                sb.Append($" Valid Journal Entry @ {DateTime.UtcNow.ToString()} (utc)." + Environment.NewLine + Environment.NewLine);
            }
            return sb.ToString();
        }
        #endregion

        #region Static Publics
        public static int MaxSerializedSize(IdGeneralLedgerServiceParameters glsparams ) 
        {
            // .
            return (2 * sizeof(byte)) +
                   glsparams.MaxNumberOfAmountsInJournalEntry * (sizeof(byte) + glsparams.MaxMemberIdSize + sizeof(long) + sizeof(byte)  + glsparams.MaxDLTTransactionReceiptIdSize + sizeof(int)); 
        }


        public static byte[] Serialize(IdGeneralLedgerServiceParameters glsparams, IJournalEntryAccounts journalEntryToSerialize)
        {
            byte[] rawBytes = new byte[ MaxSerializedSize(glsparams)];
            int pos = 0;
            
            // 1st Serialize list of Debit Accounts
            Buffer.BlockCopy( BitConverter.GetBytes(Convert.ToByte(journalEntryToSerialize.DebtAccountList.Count)),0,rawBytes,pos,sizeof(byte));
            pos += sizeof(byte);
            foreach( var acc in journalEntryToSerialize.DebtAccountList)
            {
                // Member ID
                byte[] memberIdBytes = Encoding.ASCII.GetBytes(acc.JurisdictionMember!.ID!);
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToByte(memberIdBytes.Length)), 0, rawBytes, pos, sizeof(byte));
                pos += sizeof(byte);
                Buffer.BlockCopy(memberIdBytes, 0, rawBytes, pos, memberIdBytes.Length);
                pos += memberIdBytes.Length;
                // Amount
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToInt64(acc.Amount)), 0, rawBytes, pos, sizeof(long));
                pos += sizeof(long);
                // DLT Address
                byte[] addressBytes = Encoding.ASCII.GetBytes(acc.DLTGLAccountInfo!.DLTAddress!);
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToByte(addressBytes.Length)), 0, rawBytes, pos, sizeof(byte));
                pos += sizeof(byte);
                Buffer.BlockCopy(addressBytes, 0, rawBytes, pos, addressBytes.Length);
                pos += addressBytes.Length;
                // GL Account Code
                Buffer.BlockCopy(BitConverter.GetBytes((int)acc.DLTGLAccountInfo.Code), 0, rawBytes, pos, sizeof(int));
                pos += sizeof(int);
            }
            // 2nd, Serialize list of Credit Accounts
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToByte(journalEntryToSerialize.CreditAccountList.Count)), 0, rawBytes, pos, sizeof(byte));
            pos += sizeof(byte);
            foreach (var acc in journalEntryToSerialize.CreditAccountList)
            {
                // Member Id
                byte[] memberIdBytes = Encoding.ASCII.GetBytes(acc.JurisdictionMember!.ID!);
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToByte(memberIdBytes.Length)), 0, rawBytes, pos, sizeof(byte));
                pos += sizeof(byte);
                Buffer.BlockCopy(memberIdBytes, 0, rawBytes, pos, memberIdBytes.Length);
                pos += memberIdBytes.Length;
                // Amount
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToInt64(acc.Amount)), 0, rawBytes, pos, sizeof(long));
                pos += sizeof(long);
                // DLT Address
                byte[] addressBytes = Encoding.ASCII.GetBytes(acc.DLTGLAccountInfo!.DLTAddress!);
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToByte(addressBytes.Length)), 0, rawBytes, pos, sizeof(byte));
                pos += sizeof(byte);
                Buffer.BlockCopy(addressBytes, 0, rawBytes, pos, addressBytes.Length);
                pos += addressBytes.Length;
                // GL Account Code
                Buffer.BlockCopy(BitConverter.GetBytes((int)acc.DLTGLAccountInfo.Code), 0, rawBytes, pos, sizeof(int));
                pos += sizeof(int);
            }
            byte[] finalBytes = new byte[pos];
            Buffer.BlockCopy(rawBytes, 0, finalBytes, 0, pos);
            return finalBytes;
        }


        public static IJournalEntryAccounts Deserialize( byte[] serializedJournalEntry, int offset, Func<string, Task<JurisdictionMemberGeneralLedger>> MemberLookUp)
        {
            int pos = offset;
            // 1st read Debit Accounts List
            byte debitAccountListCount = (byte)BitConverter.ToChar(serializedJournalEntry, pos);
            pos += sizeof(byte);
            List<DLTGLAccount> debitAccountList = new List<DLTGLAccount>(); 
            for (int i = 0; i < debitAccountListCount; i++)
            {
                // Member Id
                byte memberIdBytesLen = (byte)BitConverter.ToChar(serializedJournalEntry, pos);
                pos += sizeof(byte);
                byte[] memberIdBytes = new byte[memberIdBytesLen];
                Buffer.BlockCopy(serializedJournalEntry, pos, memberIdBytes, 0, memberIdBytesLen);
                pos += memberIdBytesLen;
                string memberId = Encoding.ASCII.GetString(memberIdBytes);
                // Amount
                long amount = BitConverter.ToInt64(serializedJournalEntry, pos);
                pos += sizeof(long);
                // DLT Address
                byte addressBytesLen = (byte)BitConverter.ToChar(serializedJournalEntry, pos);
                pos += sizeof(byte);
                byte[] addressBytes = new byte[addressBytesLen];
                Buffer.BlockCopy(serializedJournalEntry, pos, addressBytes, 0, addressBytesLen);
                pos+= addressBytesLen;
                string dltAddress = Encoding.ASCII.GetString( addressBytes );
                // GL Account Code
                GLAccountCode code = (GLAccountCode)BitConverter.ToInt32(serializedJournalEntry, pos);
                pos += sizeof(int);
                // Lookup the JurisdictionMemberGeneralLedger
                var jmgl = MemberLookUp(memberId).Result;
                // Now have everything we need to create a DLTGLAccount and add it to the debit accounts list
                debitAccountList.Add(new DLTGLAccount
                {
                    Amount = Convert.ToUInt64(amount),
                    DLTGLAccountInfo = new DLTGeneralLedgerAccountInfo(code, dltAddress),
                    JurisdictionMember = jmgl.JurisdictionMember!
                });
            }
            // 2nd read Credit Accounts List
            byte creditAccountListCount = (byte)BitConverter.ToChar(serializedJournalEntry, pos);
            pos += sizeof(byte);
            List<DLTGLAccount> creditAccountList = new List<DLTGLAccount>();
            for (int i = 0; i < creditAccountListCount; i++)
            {
                // Member Id
                byte memberIdBytesLen = (byte)BitConverter.ToChar(serializedJournalEntry, pos);
                pos += sizeof(byte);
                byte[] memberIdBytes = new byte[memberIdBytesLen];
                Buffer.BlockCopy(serializedJournalEntry, pos, memberIdBytes, 0, memberIdBytesLen);
                pos += memberIdBytesLen;
                string memberId = Encoding.ASCII.GetString(memberIdBytes);
                // Amount
                long amount = BitConverter.ToInt64(serializedJournalEntry, pos);
                pos += sizeof(long);
                // DLT Address
                byte addressBytesLen = (byte)BitConverter.ToChar(serializedJournalEntry, pos);
                pos += sizeof(byte);
                byte[] addressBytes = new byte[addressBytesLen];
                Buffer.BlockCopy(serializedJournalEntry, pos, addressBytes, 0, addressBytesLen);
                pos += addressBytesLen;
                string dltAddress = Encoding.ASCII.GetString(addressBytes);
                // GL Account Code
                GLAccountCode code = (GLAccountCode)BitConverter.ToInt32(serializedJournalEntry, pos);
                pos += sizeof(int);
                // Lookup the JurisdictionMemberGeneralLedger
                var jmgl = MemberLookUp(memberId).Result;
                // Now have everything we need to create a DLTGLAccount and add it to the credt accounts list
                creditAccountList.Add(new DLTGLAccount
                {
                    Amount = Convert.ToUInt64(amount),
                    DLTGLAccountInfo = new DLTGeneralLedgerAccountInfo(code, dltAddress),
                    JurisdictionMember = jmgl.JurisdictionMember!
                });
            }
            return new JournalEntryAccounts( /*GLSParameters,*/ debitAccountList, creditAccountList );
        }
        #endregion

        #region Helpers
       
        #endregion
    }
}
