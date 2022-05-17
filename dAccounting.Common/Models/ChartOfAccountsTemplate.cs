using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;
using dAccounting.Common.Utilities;

namespace dAccounting.Common.Models
{
    public class ChartOfAccountsTemplate : IChartOfAccountsTemplate
    {
        #region Field Members
        #endregion

        #region Constructors
        public ChartOfAccountsTemplate() { } // required for serialization

        public ChartOfAccountsTemplate( string name, string description, List<GLAccountCode> coas )
        {
            ID = Guid.NewGuid().ToString("N");
            Name = name;
            Description = description;
            ChartOfAccounts = coas; 
        }
        #endregion

        #region Public Interface
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<GLAccountCode>? ChartOfAccounts { get; set; }

        public string DumpChartOfAccounts(IGeneralLedgerAccountsCatalog glaccountcatalog)
        {
            const int tableWidth = 40;
            const int codeWidth = 8;
            StringBuilder sb = new StringBuilder();
            foreach (GLAccountCode coa in ChartOfAccounts!)
            {
                if ((int)coa % (int)GLAccountCode.ASSETS == 0)
                {
                    
                    sb.Append(Environment.NewLine + glaccountcatalog[coa]!.GetLocalizedName().ToUpper() + Environment.NewLine);
                }
                var coaProperties = glaccountcatalog[coa];
                if (coaProperties != null)
                {
                    string accountName = coaProperties.GetLocalizedName();
                    switch ((GLAccountType)coaProperties.Type)
                    {
                        case GLAccountType.GROUP_HEADING:
                            {
                                string line = "\t" + StringUtilities.FormatFixedLength(((int)coaProperties.Code).ToString(), codeWidth, ' ') + " " +
                                                        StringUtilities.LeftJustifiedFormatFixedLength(accountName, tableWidth, '.') + "\tGroup Heading" + Environment.NewLine;
                                sb.Append(line);
                                break;
                            }
                        case GLAccountType.POSTABLE_GROUP_ACCOUNT:
                            {
                                string line = "\t " + StringUtilities.FormatFixedLength(((int)coaProperties.Code).ToString(), codeWidth, ' ') + " " +
                                                        StringUtilities.LeftJustifiedFormatFixedLength(accountName, (tableWidth-1), '.') + "\tPostable Account" + Environment.NewLine;
                                sb.Append(line);
                                break;
                            }
                        case GLAccountType.GROUP_TOTAL:
                            {
                                string line = "\t" + StringUtilities.FormatFixedLength(((int)coaProperties.Code).ToString(), codeWidth, ' ') + " " +
                                        StringUtilities.LeftJustifiedFormatFixedLength(accountName, (tableWidth - 1), '.') + "\tGroup Total" + Environment.NewLine;
                                sb.Append(line);
                                break;
                            }
                        default:
                            {
                                throw new InvalidOperationException("Invalid COA Template.");
                            }
                    }
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Helpers
        #endregion
    }
}
