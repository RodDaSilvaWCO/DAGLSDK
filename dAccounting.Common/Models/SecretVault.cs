using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class SecretVault : ISecretVault
    {
        #region Field Members
        #endregion

        #region Constructors
        public SecretVault() 
        {

            SecretVaultStore = new Dictionary<string, ISecretVaultEntry>();
        } 
        #endregion

        #region Public Interface
        public Dictionary<string, ISecretVaultEntry>? SecretVaultStore { get; }

        public void AddSecret(string key, string secret)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));   
            }
            SecretVaultStore!.Add(key, new SecretVaultEntry(secret));
        }

        public string? this[string key] 
        { 
            get 
            {
                string? result = null!;
                if(string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (SecretVaultStore!.ContainsKey(key))
                {
                    result = SecretVaultStore![key]!.Secret;
                }
                return result;
            } 
        }
        #endregion

        #region Helpers
        #endregion
    }
}
