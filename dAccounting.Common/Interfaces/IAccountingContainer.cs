using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Interfaces
{
    public interface IAccountingContainer
    {
        IdAccountingServiceInstanceManifest? InstanceManifest { get; set; }
    }
}
