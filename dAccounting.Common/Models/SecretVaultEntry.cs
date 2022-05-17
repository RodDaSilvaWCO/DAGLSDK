using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class SecretVaultEntry : ISecretVaultEntry
    {
        #region Field Members
        private const string secretpassphrase = "%TODO%-secret pass phrase";
        private byte[] encryptedSecretBytes = null!;
        #endregion

        #region Constructors
        public SecretVaultEntry( string secret ) 
        {
            if (secret != null)
            {
                encryptedSecretBytes = Encoding.UTF8.GetBytes(secret);
                HostCryptology.EncryptBuffer(encryptedSecretBytes, 0, encryptedSecretBytes.Length, Encoding.UTF8.GetBytes(secretpassphrase));
                unsafe
                {
                    // Fill the passed in secret with zeros to remove it from memory as we are finished with it
                    fixed (char* ptr = secret)
                    {
                        for (int i = 0; i < secret.Length; i++)
                        {
                            ptr[i++] = '0';
                        }
                    }
                }
            }
        }
        #endregion

        #region Public Interface
        public string? Secret
        {
            get
            {
                string? result = null!;
                if (encryptedSecretBytes != null)
                {
                    // Make a copy of the encrypted secret
                    byte[] buffer = new byte[encryptedSecretBytes.Length];
                    encryptedSecretBytes.CopyTo(buffer, 0);
                    // Decrypt the copy in-place
                    HostCryptology.DecryptBuffer(buffer, 0, buffer.Length, Encoding.UTF8.GetBytes(secretpassphrase));
                    // Convert key to string
                    result = Encoding.ASCII.GetString(buffer);
                    // Zero buffer holding sensitive private key information as we are finished with it now.
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }
                    buffer = null!;
                }
                return result;
            }
        }
        #endregion

        #region Helpers
        #endregion
    }
}
