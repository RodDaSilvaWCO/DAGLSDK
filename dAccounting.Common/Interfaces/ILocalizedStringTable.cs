using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface ILocalizedStringTable
    {
        static Dictionary<int, Dictionary<int,string>>? StringTable { get; set; }
        static void AddLocalizedStringSet(int locale, Dictionary<int, string> stringset)
        {
            StringTable?.Add(locale, stringset);
        }
        static string LookUpLocalizedString(int locale, int stringtableindex)
        {
            return StringTable?[locale][stringtableindex]!;
        }
    }
}
