using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;
using dAccounting.Common.Utilities;

namespace dAccounting.Common.Models
{
    public class TrialBalance : ITrialBalance
    {
        #region Field Members
        private CultureInfo cultureInfo = null!;
        private DLTCurrency Currency = null!;
        #endregion

        #region Constructors
        public TrialBalance() { }  // required for serialization

        public TrialBalance( DateTime balanceDateTime, List<DLTGLAccount> debitcoa, List<DLTGLAccount> creditcoa, 
                                CultureInfo cultureinfo, DLTCurrency currency, GeneralLedgerAccountsCatalog glcatalog )
        {
            BalanceDateTime = balanceDateTime;
            DebitChartOfAccounts = debitcoa;
            CreditChartOfAccounts = creditcoa;
            Currency = currency;
            cultureInfo = cultureinfo;
            GeneralLedgerCatalog = glcatalog;
        }
        #endregion

        #region Public Interface
        public DateTime BalanceDateTime { get; set; }
        public List<DLTGLAccount>? DebitChartOfAccounts { get; set; }
        public List<DLTGLAccount>? CreditChartOfAccounts { get; set; }
        public GeneralLedgerAccountsCatalog? GeneralLedgerCatalog { get; set; }


        public string DumpTrialBalance()
        {
            int currencyDecimals = Currency.Decimals;
            string currencySymbol = Currency?.Symbol!;
            const string TAB = "   ";  // 3 spaces
            const int accountWidth = 30;
            const int codeWidth = 8;
            const int DbCrWidth = 12;
            const int reportHeaderPrefixWidth = (accountWidth+codeWidth + 1);
            StringBuilder sb = new StringBuilder();
            long totalDebits = 0L;
            long totalCredits = 0L;

            //var topLine = StringUtilities.LeftJustifiedFormatFixedLength("As at " + BalanceDateTime.ToString("MMM-dd-yyyy"), codeWidth + accountWidth, ' ') + "\t" +
            //                          StringUtilities.FormatFixedLength("Debits", DbCrWidth) + "\t" +
            //                          StringUtilities.FormatFixedLength("Credits", DbCrWidth) + "\t";
            var totalSepLine = StringUtilities.FormatFixedLength("", codeWidth, ' ') + " " +
                                       StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB +
                                       StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + TAB +
                                       StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + TAB;
            var reportHeaderPrefix = "As at " + DateTime.UtcNow.ToString("MMM-dd-yyyy") + " (utc)";
            //var topSepLine = StringUtilities.FormatFixedLength("", reportHeaderPrefix.Length, ' ') +
            //      StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t" +
            //      StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t";
            
            var topLine = StringUtilities.LeftJustifiedFormatFixedLength(reportHeaderPrefix, reportHeaderPrefixWidth) + TAB +
                                        StringUtilities.FormatFixedLength("Debits", DbCrWidth) + TAB +
                                        StringUtilities.FormatFixedLength("Credits", DbCrWidth) ;
            var topSepLine = new string('-', topLine.Length );
            sb.AppendLine(topLine);
            //var topSepLine = new string('-', codeWidth + accountWidth + DbCrWidth + DbCrWidth + "\t\t\t\t\t\t\t".Length);
            sb.AppendLine(topSepLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            //sb.Append($"|\t{StringUtilities.FormatFixedLength("ACCOUNT", codeWidth)}\t|\t{StringUtilities.FormatFixedLength("DB", DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength("CR", DbCrWidth)}\t|" + Environment.NewLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            int currentAccount = 0;
            // Loop through trail balance accounts procesing them in DB/CR pairs
            foreach (var acc in DebitChartOfAccounts!) // doesn't matter whether we use DebitChartOfAccounts or CreditChartOfAccounts here as they both have the same list of account codes
            {
                var coaProperties = GeneralLedgerCatalog![acc.DLTGLAccountInfo!.Code];
                if (coaProperties != null)
                {
                    string accountName = coaProperties.GetLocalizedName();
                    var code = StringUtilities.FormatFixedLength(((int)acc.DLTGLAccountInfo!.Code).ToString(), codeWidth, ' ') + " " +
                                        StringUtilities.LeftJustifiedFormatFixedLength(accountName, accountWidth, ' ');
                    var db = "";
                    var cr = "";
                    long debitbal = 0;
                    long creditbal = 0;
                    bool isDebitAccount;
                    // Is this a Debit Class Account?
                    (debitbal, creditbal, isDebitAccount) = GLAccountUtilities.DetermineDebitCreditBalance(acc, CreditChartOfAccounts![currentAccount]);
                    if (debitbal == 0)
                    {
                        db = StringUtilities.FormatFixedLength((isDebitAccount? "0" : "-"), DbCrWidth);
                    }
                    else
                    {
                        db = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(debitbal), DbCrWidth);
                        totalDebits += debitbal;
                    }
                    if (creditbal == 0)
                    {
                        cr = StringUtilities.FormatFixedLength((!isDebitAccount ? "0" : "-"), DbCrWidth);
                    }
                    else
                    {
                        cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(creditbal), DbCrWidth);
                        totalCredits += creditbal;
                    }
                    sb.AppendLine($"{code}   {db}   {cr}" );
                    currentAccount++;
                }
            }
           
            sb.AppendLine(totalSepLine);
            sb.AppendLine();
            var totalLine = StringUtilities.FormatFixedLength("", codeWidth, ' ') + " " +
                                        StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB +
                                        StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalDebits), DbCrWidth) + TAB +
                                        StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalCredits), DbCrWidth) ;
            sb.AppendLine(totalLine);
            var totalDoubleSepLine = StringUtilities.FormatFixedLength("", codeWidth, ' ') + " " +
                            StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB +
                            StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth) + TAB +
                            StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth) ;
            sb.AppendLine(totalDoubleSepLine);
            
            if (totalCredits != totalDebits)
            {
                sb.AppendLine($" *** E R R O R: @ {DateTime.UtcNow.ToString()} (utc)  ***  Invalid Trial Balance - total Debits do NOT match total Credits.");
            }
            //else
            //{
            //    sb.Append($" Valid Trial Balance @ {DateTime.UtcNow.ToString()} (utc).");
            //}
            return sb.ToString();
        }


        public string IncomeStatementReport()
        {
            int currencyDecimals = Currency.Decimals;
            string currencySymbol = Currency?.Symbol!;
            const string TAB = "   ";  // 3 spaces
            const int accountWidth = 30;
            const int codeWidth = 8;
            const int DbCrWidth = 12;
            const int reportHeaderPrefixWidth = (accountWidth + codeWidth + 1);
            StringBuilder sb = new StringBuilder();

            //var topLine = StringUtilities.LeftJustifiedFormatFixedLength("As at " + BalanceDateTime.ToString("MMM-dd-yyyy"), codeWidth + accountWidth, ' ') + "\t" +
            //                          StringUtilities.FormatFixedLength("Debits", DbCrWidth) + "\t" +
            //                          StringUtilities.FormatFixedLength("Credits", DbCrWidth) + "\t";
            
                               
            var reportHeaderPrefix = "As at " + DateTime.UtcNow.ToString("MMM-dd-yyyy") + " (utc)";
            //var topSepLine = StringUtilities.FormatFixedLength("", reportHeaderPrefix.Length, ' ') +
            //      StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t" +
            //      StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t";

            var topLine = StringUtilities.LeftJustifiedFormatFixedLength(reportHeaderPrefix, reportHeaderPrefixWidth);// + TAB +
                                        //StringUtilities.FormatFixedLength("Debits", DbCrWidth) + TAB +
                                        //StringUtilities.FormatFixedLength("Credits", DbCrWidth);

            //var topSepLine = new string('-', topLine.Length);
            sb.AppendLine(topLine);
            sb.AppendLine();
            sb.AppendLine("REVENUE");
            sb.AppendLine();
            sb.AppendLine($"{TAB}REVENUE");

            //var topSepLine = new string('-', codeWidth + accountWidth + DbCrWidth + DbCrWidth + "\t\t\t\t\t\t\t".Length);
            //sb.AppendLine(topSepLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            //sb.Append($"|\t{StringUtilities.FormatFixedLength("ACCOUNT", codeWidth)}\t|\t{StringUtilities.FormatFixedLength("DB", DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength("CR", DbCrWidth)}\t|" + Environment.NewLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            long totalRevenue = 0;
            // Loop through coa revenue accounts - NOTE:  this call only sees "Postable" accounts
            int currentAccount = 0;
            foreach (var acc in DebitChartOfAccounts!) // doesn't matter whether we use DebitChartOfAccounts or CreditChartOfAccounts here as they both have the same list of account codes
            {
                var coaProperties = GeneralLedgerCatalog![acc.DLTGLAccountInfo!.Code];
                if (coaProperties != null)
                {
                    // Only process Revenue Accounts
                    if (GLAccountUtilities.IsRevenueAccount(acc.DLTGLAccountInfo.Code!) )
                    {
                        var accountName = StringUtilities.LeftJustifiedFormatFixedLength(coaProperties.GetLocalizedName(), accountWidth, ' ');
                        var cr = "";
                        long debitbal = 0;
                        long creditbal = 0;
                        bool isDebitAccount;
                        // Is this a Debit Class Account?
                        (debitbal, creditbal, isDebitAccount) = GLAccountUtilities.DetermineDebitCreditBalance(acc, CreditChartOfAccounts![currentAccount]);
                        if( creditbal == 0)
                        {
                            if( debitbal == 0)
                            {
                                cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(0), DbCrWidth);
                            }
                            else
                            {
                                cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(debitbal * -1), DbCrWidth);
                                totalRevenue -= debitbal;
                            }
                        }
                        else
                        {
                            cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(creditbal), DbCrWidth );
                            totalRevenue += creditbal;
                        }
                        sb.AppendLine($"{TAB}{TAB}{accountName}{cr}");
                    }
                    currentAccount++;
                }
            }
            var totalSepLine = StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB + TAB +
                                       StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth);
            sb.AppendLine(totalSepLine);
            var revSubTotalLine = StringUtilities.LeftJustifiedFormatFixedLength($"{TAB}TOTAL REVENUE", accountWidth, ' ') + TAB + TAB +
                                        StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalRevenue), DbCrWidth) ;
            sb.AppendLine(revSubTotalLine);
            sb.AppendLine(totalSepLine);
            sb.AppendLine();
            sb.AppendLine();
            //var revTotalLine = StringUtilities.LeftJustifiedFormatFixedLength("TOTAL REVENUE", accountWidth, ' ') + TAB + TAB +
            //                           StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalRevenue), DbCrWidth);
            //sb.AppendLine(revTotalLine);

            sb.AppendLine();
            sb.AppendLine("EXPENSE");
            sb.AppendLine();
            sb.AppendLine($"{TAB}EXPENSES");

            //var topSepLine = new string('-', codeWidth + accountWidth + DbCrWidth + DbCrWidth + "\t\t\t\t\t\t\t".Length);
            //sb.AppendLine(topSepLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            //sb.Append($"|\t{StringUtilities.FormatFixedLength("ACCOUNT", codeWidth)}\t|\t{StringUtilities.FormatFixedLength("DB", DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength("CR", DbCrWidth)}\t|" + Environment.NewLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            long totalExpenses = 0;
            // Loop through coa revenue accounts - NOTE:  this call only sees "Postable" accounts
            currentAccount = 0;
            foreach (var acc in DebitChartOfAccounts!) // doesn't matter whether we use DebitChartOfAccounts or CreditChartOfAccounts here as they both have the same list of account codes
            {
                var coaProperties = GeneralLedgerCatalog![acc.DLTGLAccountInfo!.Code];
                if (coaProperties != null)
                {
                    // Only process Revenue Accounts
                    if (GLAccountUtilities.IsExpenseAccount(acc.DLTGLAccountInfo.Code!))
                    {
                        var accountName = StringUtilities.LeftJustifiedFormatFixedLength(coaProperties.GetLocalizedName(), accountWidth, ' ');
                        var db = "";
                        long debitbal = 0;
                        long creditbal = 0;
                        bool isDebitAccount;
                        // Is this a Debit Class Account?
                        (debitbal, creditbal, isDebitAccount) = GLAccountUtilities.DetermineDebitCreditBalance(acc, CreditChartOfAccounts![currentAccount]);
                        if (debitbal == 0)
                        {
                            if (creditbal == 0)
                            {
                                db = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(0), DbCrWidth);
                            }
                            else
                            {
                                db = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(creditbal * -1), DbCrWidth);
                                totalExpenses -= creditbal;
                            }
                        }
                        else
                        {
                            db = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(debitbal), DbCrWidth);
                            totalExpenses += debitbal;
                        }
                        sb.AppendLine($"{TAB}{TAB}{accountName}{db}");
                    }
                    currentAccount++;
                }
            }
            totalSepLine = StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB + TAB +
                                       StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth);
            sb.AppendLine(totalSepLine);
            var expSubTotalLine = StringUtilities.LeftJustifiedFormatFixedLength($"{TAB}TOTAL EXPENSES", accountWidth, ' ') + TAB + TAB +
                                        StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalExpenses), DbCrWidth);
            sb.AppendLine(expSubTotalLine);
            sb.AppendLine(totalSepLine);
            sb.AppendLine();
            //sb.AppendLine();
            //var expTotalLine = StringUtilities.LeftJustifiedFormatFixedLength("TOTAL EXPENSE", accountWidth, ' ') + TAB + TAB +
            //                           StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalExpenses), DbCrWidth);
            //sb.AppendLine(expTotalLine);
            sb.AppendLine();
            var netIncomeTotalLine = StringUtilities.LeftJustifiedFormatFixedLength("NET INCOME", accountWidth, ' ') + TAB + TAB +
                                       StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalRevenue - totalExpenses), DbCrWidth);
            sb.AppendLine(netIncomeTotalLine);
            var doubleTotalSepLine = StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB + TAB +
                                       StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth);
            sb.AppendLine(doubleTotalSepLine);
            //var totalDoubleSepLine = StringUtilities.FormatFixedLength("", codeWidth, ' ') + " " +
            //                StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB +
            //                StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth) + TAB +
            //                StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth);
            //sb.AppendLine(totalDoubleSepLine);

            //if (totalCredits != totalDebits)
            //{
            //    sb.AppendLine($" *** E R R O R: @ {DateTime.UtcNow.ToString()} (utc)  ***  Invalid Trial Balance - total Debits do NOT match total Credits.");
            //}
            //else
            //{
            //    sb.Append($" Valid Trial Balance @ {DateTime.UtcNow.ToString()} (utc).");
            //}
            return sb.ToString();
        }


        public string BalanceSheetReport()
        {
            int currencyDecimals = Currency.Decimals;
            string currencySymbol = Currency?.Symbol!;
            const string TAB = "   ";  // 3 spaces
            const int accountWidth = 30;
            const int codeWidth = 8;
            const int DbCrWidth = 12;
            const int reportHeaderPrefixWidth = (accountWidth + codeWidth + 1);
            StringBuilder sb = new StringBuilder();

            //var topLine = StringUtilities.LeftJustifiedFormatFixedLength("As at " + BalanceDateTime.ToString("MMM-dd-yyyy"), codeWidth + accountWidth, ' ') + "\t" +
            //                          StringUtilities.FormatFixedLength("Debits", DbCrWidth) + "\t" +
            //                          StringUtilities.FormatFixedLength("Credits", DbCrWidth) + "\t";


            var reportHeaderPrefix = "As at " + DateTime.UtcNow.ToString("MMM-dd-yyyy") + " (utc)";
            //var topSepLine = StringUtilities.FormatFixedLength("", reportHeaderPrefix.Length, ' ') +
            //      StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t" +
            //      StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth) + "\t";

            var topLine = StringUtilities.LeftJustifiedFormatFixedLength(reportHeaderPrefix, reportHeaderPrefixWidth);// + TAB +
                                                                                                                      //StringUtilities.FormatFixedLength("Debits", DbCrWidth) + TAB +
                                                                                                                      //StringUtilities.FormatFixedLength("Credits", DbCrWidth);

            //var topSepLine = new string('-', topLine.Length);
            sb.AppendLine(topLine);
            sb.AppendLine();

            //var revTotalLine = StringUtilities.LeftJustifiedFormatFixedLength("TOTAL REVENUE", accountWidth, ' ') + TAB + TAB +
            //                           StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalRevenue), DbCrWidth);
            //sb.AppendLine(revTotalLine);
            var totalSepLine = StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB + TAB +
                           StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth);

            sb.AppendLine();
            sb.AppendLine("ASSETS");
            sb.AppendLine();
            sb.AppendLine($"{TAB}ASSETS");

            //var topSepLine = new string('-', codeWidth + accountWidth + DbCrWidth + DbCrWidth + "\t\t\t\t\t\t\t".Length);
            //sb.AppendLine(topSepLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            //sb.Append($"|\t{StringUtilities.FormatFixedLength("ACCOUNT", codeWidth)}\t|\t{StringUtilities.FormatFixedLength("DB", DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength("CR", DbCrWidth)}\t|" + Environment.NewLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            long totalAssets = 0;
            // Loop through coa asset accounts - NOTE:  this call only sees "Postable" accounts
            var currentAccount = 0;
            foreach (var acc in DebitChartOfAccounts!) // doesn't matter whether we use DebitChartOfAccounts or CreditChartOfAccounts here as they both have the same list of account codes
            {
                var coaProperties = GeneralLedgerCatalog![acc.DLTGLAccountInfo!.Code];
                if (coaProperties != null)
                {
                    // Only process Asset Accounts
                    if (GLAccountUtilities.IsAssetAccount(acc.DLTGLAccountInfo.Code!))
                    {
                        var accountName = StringUtilities.LeftJustifiedFormatFixedLength(coaProperties.GetLocalizedName(), accountWidth, ' ');
                        var db = "";
                        long debitbal = 0;
                        long creditbal = 0;
                        bool isDebitAccount;
                        // Is this a Debit Class Account?
                        (debitbal, creditbal, isDebitAccount) = GLAccountUtilities.DetermineDebitCreditBalance(acc, CreditChartOfAccounts![currentAccount]);
                        if (debitbal == 0)
                        {
                            if (creditbal == 0)
                            {
                                db = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(0), DbCrWidth);
                            }
                            else
                            {
                                db = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(creditbal * -1), DbCrWidth);
                                totalAssets -= creditbal;
                            }
                        }
                        else
                        {
                            db = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(debitbal), DbCrWidth);
                            totalAssets += debitbal;
                        }
                        sb.AppendLine($"{TAB}{TAB}{accountName}{db}");
                    }
                    currentAccount++;
                }
            }
            totalSepLine = StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB + TAB +
                                       StringUtilities.FormatFixedLength(new string('_', DbCrWidth), DbCrWidth);
            sb.AppendLine(totalSepLine);
            var expSubTotalLine = StringUtilities.LeftJustifiedFormatFixedLength($"{TAB}TOTAL ASSETS", accountWidth, ' ') + TAB + TAB +
                                        StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalAssets), DbCrWidth);
            sb.AppendLine(expSubTotalLine);
            var doubleTotalSepLine = StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB + TAB +
                                       StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth);
            sb.AppendLine(doubleTotalSepLine);
            sb.AppendLine();

            sb.AppendLine("LIABILITIES");
            sb.AppendLine();
            sb.AppendLine($"{TAB}LIABILITIES");

            //var topSepLine = new string('-', codeWidth + accountWidth + DbCrWidth + DbCrWidth + "\t\t\t\t\t\t\t".Length);
            //sb.AppendLine(topSepLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            //sb.Append($"|\t{StringUtilities.FormatFixedLength("ACCOUNT", codeWidth)}\t|\t{StringUtilities.FormatFixedLength("DB", DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength("CR", DbCrWidth)}\t|" + Environment.NewLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            long totalLiabilities = 0;
            // Loop through coa asset accounts - NOTE:  this call only sees "Postable" accounts
            currentAccount = 0;
            foreach (var acc in DebitChartOfAccounts!) // doesn't matter whether we use DebitChartOfAccounts or CreditChartOfAccounts here as they both have the same list of account codes
            {
                var coaProperties = GeneralLedgerCatalog![acc.DLTGLAccountInfo!.Code];
                if (coaProperties != null)
                {
                    // Only process Revenue Accounts
                    if (GLAccountUtilities.IsLiabilityAccount(acc.DLTGLAccountInfo.Code!))
                    {
                        var accountName = StringUtilities.LeftJustifiedFormatFixedLength(coaProperties.GetLocalizedName(), accountWidth, ' ');
                        var cr = "";
                        long debitbal = 0;
                        long creditbal = 0;
                        bool isDebitAccount;
                        // Is this a Debit Class Account?
                        (debitbal, creditbal, isDebitAccount) = GLAccountUtilities.DetermineDebitCreditBalance(acc, CreditChartOfAccounts![currentAccount]);
                        if (creditbal == 0)
                        {
                            if (debitbal == 0)
                            {
                                cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(0), DbCrWidth);
                            }
                            else
                            {
                                cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(debitbal * -1), DbCrWidth);
                                totalLiabilities -= debitbal;
                            }
                        }
                        else
                        {
                            cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(creditbal), DbCrWidth);
                            totalLiabilities += creditbal;
                        }
                        sb.AppendLine($"{TAB}{TAB}{accountName}{cr}");
                    }
                    currentAccount++;
                }
            }
            sb.AppendLine(totalSepLine);
            var revSubTotalLine = StringUtilities.LeftJustifiedFormatFixedLength($"{TAB}TOTAL LIABILITIES", accountWidth, ' ') + TAB + TAB +
                                        StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalLiabilities), DbCrWidth);
            sb.AppendLine(revSubTotalLine);
            sb.AppendLine(totalSepLine);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("EQUITY");
            sb.AppendLine();
            sb.AppendLine($"{TAB}EQUITY");

            //var topSepLine = new string('-', codeWidth + accountWidth + DbCrWidth + DbCrWidth + "\t\t\t\t\t\t\t".Length);
            //sb.AppendLine(topSepLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            //sb.Append($"|\t{StringUtilities.FormatFixedLength("ACCOUNT", codeWidth)}\t|\t{StringUtilities.FormatFixedLength("DB", DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength("CR", DbCrWidth)}\t|" + Environment.NewLine);
            //sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
            long totalEquities = 0;
            // Loop through coa equity accounts - NOTE:  this call only sees "Postable" accounts
            currentAccount = 0;
            foreach (var acc in DebitChartOfAccounts!) // doesn't matter whether we use DebitChartOfAccounts or CreditChartOfAccounts here as they both have the same list of account codes
            {
                var coaProperties = GeneralLedgerCatalog![acc.DLTGLAccountInfo!.Code];
                if (coaProperties != null)
                {
                    // Only process Revenue Accounts
                    if (GLAccountUtilities.IsEquityAccount(acc.DLTGLAccountInfo.Code!))
                    {
                        var accountName = StringUtilities.LeftJustifiedFormatFixedLength(coaProperties.GetLocalizedName(), accountWidth, ' ');
                        var cr = "";
                        long debitbal = 0;
                        long creditbal = 0;
                        bool isDebitAccount;
                        // Is this a Debit Class Account?
                        (debitbal, creditbal, isDebitAccount) = GLAccountUtilities.DetermineDebitCreditBalance(acc, CreditChartOfAccounts![currentAccount]);
                        if (creditbal == 0)
                        {
                            if (debitbal == 0)
                            {
                                cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(0), DbCrWidth);
                            }
                            else
                            {
                                cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(debitbal * -1), DbCrWidth);
                                totalEquities -= debitbal;
                            }
                        }
                        else
                        {
                            cr = StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(creditbal), DbCrWidth);
                            totalEquities += creditbal;
                        }
                        sb.AppendLine($"{TAB}{TAB}{accountName}{cr}");
                    }
                    currentAccount++;
                }
            }
            sb.AppendLine(totalSepLine);
            var eqSubTotalLine = StringUtilities.LeftJustifiedFormatFixedLength($"{TAB}TOTAL EQUITY", accountWidth, ' ') + TAB + TAB +
                                        StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalEquities), DbCrWidth);
            sb.AppendLine(eqSubTotalLine);
            sb.AppendLine(totalSepLine);
            sb.AppendLine();
            sb.AppendLine();
            //sb.AppendLine();
            //var expTotalLine = StringUtilities.LeftJustifiedFormatFixedLength("TOTAL EXPENSE", accountWidth, ' ') + TAB + TAB +
            //                           StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalExpenses), DbCrWidth);
            //sb.AppendLine(expTotalLine);

            var netIncomeTotalLine = StringUtilities.LeftJustifiedFormatFixedLength("LIABILITIES AND EQUITIES", accountWidth, ' ') + TAB + TAB +
                                       StringUtilities.FormatFixedLength(StringUtilities.FormatCurrencyAmount(totalLiabilities + totalEquities), DbCrWidth);
            sb.AppendLine(netIncomeTotalLine);
            sb.AppendLine(doubleTotalSepLine);
            //var totalDoubleSepLine = StringUtilities.FormatFixedLength("", codeWidth, ' ') + " " +
            //                StringUtilities.LeftJustifiedFormatFixedLength("", accountWidth, ' ') + TAB +
            //                StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth) + TAB +
            //                StringUtilities.FormatFixedLength(new string('=', DbCrWidth), DbCrWidth);
            //sb.AppendLine(totalDoubleSepLine);

            //if (totalCredits != totalDebits)
            //{
            //    sb.AppendLine($" *** E R R O R: @ {DateTime.UtcNow.ToString()} (utc)  ***  Invalid Trial Balance - total Debits do NOT match total Credits.");
            //}
            //else
            //{
            //    sb.Append($" Valid Trial Balance @ {DateTime.UtcNow.ToString()} (utc).");
            //}
            return sb.ToString();
        }

        //public string DumpTrialBalance( )
        //{
        //    const int tableWidth = 89;
        //    const int codeWidth = 25;
        //    const int DbCrWidth = 12;
        //    StringBuilder sb = new StringBuilder();
        //    long totalDebits = 0L;
        //    long totalCredits = 0L;
        //    sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
        //    sb.Append($"|\t{StringUtilities.FormatFixedLength("ACCOUNT", codeWidth)}\t|\t{StringUtilities.FormatFixedLength("DB", DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength("CR", DbCrWidth)}\t|" + Environment.NewLine);
        //    sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
        //    int currentAccount = 0;
        //    // Loop through trail balance accounts procesing them in DB/CR pairs
        //    foreach (var acc in DebitChartOfAccounts!) // doesn't matter whether we use DebitChartOfAccounts or CreditChartOfAccounts here as they both have the same list of account codes
        //    {
        //        var code = StringUtilities.FormatFixedLength(acc.DLTGLAccountInfo!.Code.ToString(), codeWidth);
        //        var db = "";
        //        var cr = "";
        //        long debitbal = 0;
        //        long creditbal = 0;
        //        // Is this a Debit Class Account?
        //        (debitbal, creditbal) = GLAccountUtilities.DetermineDebitCreditBalance(acc, CreditChartOfAccounts![currentAccount]);
        //        if( debitbal == 0 )
        //        {
        //            db = StringUtilities.FormatFixedLength("-", DbCrWidth);
        //        }
        //        else
        //        {
        //            db = StringUtilities.FormatFixedLength(debitbal.ToString(), DbCrWidth);
        //            totalDebits += debitbal;
        //        }
        //        if( creditbal == 0)
        //        {
        //            cr = StringUtilities.FormatFixedLength("-", DbCrWidth);
        //        }
        //        else
        //        {
        //            cr = StringUtilities.FormatFixedLength(creditbal.ToString(), DbCrWidth);
        //            totalCredits += creditbal;
        //        }
        //        sb.Append($"|\t{code}\t|\t{db}\t|\t{cr}\t|" + Environment.NewLine);
        //        currentAccount++;
        //    }
        //    sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
        //    sb.Append($"|\t{StringUtilities.FormatFixedLength(" ", codeWidth)}\t|\t{StringUtilities.FormatFixedLength(totalDebits.ToString(), DbCrWidth)}\t|\t{StringUtilities.FormatFixedLength(totalCredits.ToString(), DbCrWidth)}\t|" + Environment.NewLine);
        //    sb.Append($"{StringUtilities.FormatFixedLength("=", tableWidth, '=')}" + Environment.NewLine);
        //    sb.Append(Environment.NewLine);
        //    if (totalCredits != totalDebits)
        //    {
        //        sb.Append($" *** E R R O R: @ {DateTime.UtcNow.ToString()} (utc)  ***  Invalid Trial Balance - total Debits do NOT match total Credits.");
        //    }
        //    else
        //    {
        //        sb.Append($" Valid Trial Balance @ {DateTime.UtcNow.ToString()} (utc).");
        //    }
        //    return sb.ToString();
        //}
        #endregion

        #region Helpers

        #endregion
    }
}
