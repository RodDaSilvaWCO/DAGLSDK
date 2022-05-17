using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Models
{
    public enum GLAccountCode
    {
        UNINITIALIZED = -1,

        ASSETS                      = 10000000,
                WALLET                  = 10070000,
                ACCOUNTS_RECEIVABLE     = 11200000,
        ASSETS_TOTAL                = 19999999,


        LIABILITIES                 = 20000000,
                ACCOUNTS_PAYABLE        = 22000000,
                DUE_OWNER               = 22001000,
        LIABILITIES_TOTAL           = 29999999,


        EQUITY                      = 30000000,
                RETAINED_EARNINGS       = 33560000,
        EQUITY_TOTAL                = 39999999,


        REVENUE                     = 40000000,
                EARNED_TRX_FEES         = 40000010,
                MISCLLANEOUS_SALES      = 40000020,
        REVENUE_TOTAL               = 49999999,


        EXPENSES                    = 50000000,
                dACCOUNTING_TRX_FEES    = 50000010,
                JURISDICTION_TRX_FEES   = 50000011,
                MISCLLANEOUS_PURCHASES  = 50000020,
        EXPENSES_TOTAL              = 59999999


    }
}
