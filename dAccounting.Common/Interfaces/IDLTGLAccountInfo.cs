using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Models;

namespace dAccounting.Common.Interfaces
{
    public interface IDLTGLAccountInfo
    {
        GLAccountCode Code { get; }
        string? DLTAddress { get;  }
    }
}
