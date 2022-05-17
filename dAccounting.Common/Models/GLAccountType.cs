using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Models
{
    public enum GLAccountType
    {
        UNINITIALIZED = -1,
        GROUP_HEADING = 1,
        SUB_GROUP_HEADING = 2,
        SUB_GROUP_TOTAL = 3,
        POSTABLE_GROUP_ACCOUNT = 4,
        GROUP_TOTAL = 5
    }
}
