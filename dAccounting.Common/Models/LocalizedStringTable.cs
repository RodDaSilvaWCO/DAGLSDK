using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public static class LocalizedStringTable //: ILocalizedStringTable
    {
        #region Field Members
        
        #endregion

        #region Constructors
        static LocalizedStringTable() 
        {
            StringTable = new Dictionary<int, Dictionary<int,string>>();
        } 


        #endregion

        #region Public Interface
        public static  Dictionary<int, Dictionary<int,string>>? StringTable { get; set; }

        public static void AddLocalizedStringSet(int locale, Dictionary<int, string> stringset)
        {
            StringTable?.Add(locale, stringset);
        }

        public static string LookUpLocalizedString(int locale, int stringtableindex )
        {
            return StringTable?[locale][stringtableindex]!;
        }
        #endregion

        #region Helpers
        #endregion
    }
}
