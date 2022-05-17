using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dAccounting.Common.Interfaces
{
    public interface ISecretVault
    {
        Dictionary<string, ISecretVaultEntry>? SecretVaultStore { get; }
        void AddSecret( string key, string secret );
        string? this[ string key ] { get; }
    }
}
