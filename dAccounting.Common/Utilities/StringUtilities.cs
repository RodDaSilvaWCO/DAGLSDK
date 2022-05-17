using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Utilities
{
    public class StringUtilities
    {
        public static  string FormatFixedLength(string val, int fixedLength, char chr = ' ')
        {
            return RightFormatFixedLength( val, fixedLength, chr); 
        }

        public static string RightFormatFixedLength(string val, int fixedLength, char chr = ' ')
        {
            string result = new string(chr, fixedLength);
            if (!string.IsNullOrEmpty(val))
            {
                result = (result + val);
                result = result.Substring(result.Length - fixedLength);
            }
            return result;
        }

        public static string LeftJustifiedFormatFixedLength(string val, int fixedLength, char chr = ' ')
        {
            string result = new string(chr, fixedLength);
            if (!string.IsNullOrEmpty(val))
            {
                result = (val + result);
                result = result.Substring( 0, fixedLength);
            }
            return result;
        }

        public static string FormatCurrencyAmount( long amount, int currencyDecimals = 0 )
        {
            // %TODO% - Below code assume currencyDecimals = 2 - need to fix for arbitrary currencyDecimals values
            //string result = "";
            //if( amount < 0)
            //{
            //    throw new InvalidOperationException("Amount must be >= 0.");
            //}
            //if( currencyDecimals < 2 || currencyDecimals > 8)
            //{
            //    throw new InvalidOperationException("Decimals must >= 2 and <= 8.");
            //}
            //if( amount <= 9)
            //{
            //    result = "0.0" + amount.ToString();  // 0.01 to 0.09
            //}
            //else if( amount <= 99)
            //{
            //    result = "0." + amount.ToString();  // 0.10 to 0.99
            //}
            //else
            //{
            //    result = (amount / 100).ToString() + "." + ("00" + (amount % 100).ToString()).Substring(-2, 2);
            //}
            //return result;
            return amount.ToString("N0");
        }
    }
}
