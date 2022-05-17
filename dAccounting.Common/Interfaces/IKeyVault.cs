using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Interfaces
{
    public interface IKeyVault
    {
        string? Base64EncryptedPublicKey { get; set; }
        string? Base64EncryptedPrivateKey { get; set;  }
        bool    HasPublicKey { get;  }   
        bool    HasPrivateKey { get; }
    }
}
