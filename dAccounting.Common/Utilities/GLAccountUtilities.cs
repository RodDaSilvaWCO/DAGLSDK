using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Utilities
{
    public class GLAccountUtilities
    {
        public static  int AccountClass(GLAccountCode code)
        {
            const int ASSETS = 0;
            const int LIABILITIES = 1;
            const int EQUITY = 2;
            const int REVENUE = 3;
            const int EXPENSE = 4;
            int intCode = (int)code;
            int accountClass = -1;
            if (intCode >= (int)GLAccountCode.ASSETS && intCode <= (int)GLAccountCode.ASSETS_TOTAL)
            {
                accountClass = ASSETS;
            }
            else if (intCode >= (int)GLAccountCode.LIABILITIES && intCode <= (int)GLAccountCode.LIABILITIES_TOTAL)
            {
                accountClass = LIABILITIES;
            }
            else if (intCode >= (int)GLAccountCode.EQUITY && intCode <= (int)GLAccountCode.EQUITY_TOTAL)
            {
                accountClass = EQUITY;
            }
            else if (intCode >= (int)GLAccountCode.REVENUE && intCode <= (int)GLAccountCode.REVENUE_TOTAL)
            {
                accountClass = REVENUE;
            }
            else if (intCode >= (int)GLAccountCode.EXPENSES && intCode <= (int)GLAccountCode.EXPENSES_TOTAL)
            {
                accountClass = EXPENSE;
            }
            return accountClass;
        }


        public static  bool IsDebitAccount(GLAccountCode code)
        {
            int intCode = (int)code;
            bool result = false;
            if ((intCode >= (int)GLAccountCode.ASSETS && intCode <= (int)GLAccountCode.ASSETS_TOTAL) ||
                (intCode >= (int)GLAccountCode.EXPENSES && intCode <= (int)GLAccountCode.EXPENSES_TOTAL))
            {
                result = true;
            }
            return result;
        }

        public static bool IsRevenueAccount(GLAccountCode code )
        {
            int intCode = (int)code;
            return (intCode >= (int)GLAccountCode.REVENUE && intCode <= (int)GLAccountCode.REVENUE_TOTAL);
        }

        public static bool IsExpenseAccount(GLAccountCode code)
        {
            int intCode = (int)code;
            return (intCode >= (int)GLAccountCode.EXPENSES && intCode <= (int)GLAccountCode.EXPENSES_TOTAL);
        }

        public static bool IsAssetAccount(GLAccountCode code)
        {
            int intCode = (int)code;
            return (intCode >= (int)GLAccountCode.ASSETS && intCode <= (int)GLAccountCode.ASSETS_TOTAL);
        }

        public static bool IsLiabilityAccount(GLAccountCode code)
        {
            int intCode = (int)code;
            return (intCode >= (int)GLAccountCode.LIABILITIES && intCode <= (int)GLAccountCode.LIABILITIES_TOTAL);
        }

        public static bool IsEquityAccount(GLAccountCode code)
        {
            int intCode = (int)code;
            return (intCode >= (int)GLAccountCode.EQUITY && intCode <= (int)GLAccountCode.EQUITY_TOTAL);
        }

        public static ChartOfAccountsTemplate LookupCoaTemplate(List<ChartOfAccountsTemplate> coaTemplateList, string coaTemplateId)
        {
            ChartOfAccountsTemplate coaTemplate = null!;
            foreach (var coatemp in coaTemplateList)
            {
                if (new Guid(coatemp.ID!).Equals(new Guid(coaTemplateId)))
                {
                    coaTemplate = coatemp;
                    break;
                }
            }
            return coaTemplate;
        }

        public static  (long, long, bool) DetermineDebitCreditBalance(DLTGLAccount debitAccount, DLTGLAccount creditAccount)
        {
            long debitbal = 0;
            long creditbal = 0;
            bool isDebitAccount = GLAccountUtilities.IsDebitAccount(debitAccount.DLTGLAccountInfo!.Code);
            if (isDebitAccount)
            {
                if (debitAccount.Amount.HasValue)
                {
                    if (creditAccount.Amount.HasValue)
                    {
                        var diff = Convert.ToInt64(debitAccount.Amount.Value) - Convert.ToInt64(creditAccount.Amount!.Value);
                        if (diff > 0)
                        {
                            debitbal = diff;
                        }
                        else if (diff < 0)
                        {
                            creditbal = Math.Abs(diff);
                        }
                    }
                    else
                    {
                        debitbal = Convert.ToInt64(debitAccount.Amount.Value);
                    }
                }
                else
                {
                    if (creditAccount.Amount.HasValue)
                    {
                        creditbal = Convert.ToInt64(creditAccount.Amount!.Value);
                    }
                    else
                    {
                        // Should never get here
                        throw new ArgumentException("Invalid trial balance");
                    }
                }
            }
            else  // We are dealing with a Credit class Account
            {
                if (creditAccount.Amount.HasValue)
                {
                    if (debitAccount.Amount.HasValue)
                    {
                        var diff = Convert.ToInt64(creditAccount.Amount!.Value) - Convert.ToInt64(debitAccount.Amount.Value);
                        if (diff > 0)
                        {
                            creditbal = diff;
                        }
                        else if (diff < 0)
                        {
                            debitbal = Math.Abs(diff);
                        }
                    }
                    else
                    {
                        creditbal = Convert.ToInt64(creditAccount.Amount!.Value);
                    }
                }
                else
                {
                    if (debitAccount.Amount.HasValue)
                    {
                        debitbal = Convert.ToInt64(debitAccount.Amount.Value);
                    }
                    else
                    {
                        // Should never get here
                        throw new ArgumentException("Invalid trial balance");
                    }
                }
            }
            return (debitbal, creditbal, isDebitAccount);
        }


        public static DLTGeneralLedgerAccountInfo LookupGLAccountInChartOfAccountsByCode(List<DLTGeneralLedgerAccountInfo> coa, GLAccountCode glaccountcode)
        {
            DLTGeneralLedgerAccountInfo result = null!;
            foreach (var acc in coa)
            {
                if (acc.Code == glaccountcode)
                {
                    result = acc;
                    break;
                }
            }
            return result;
        }
    }
}
