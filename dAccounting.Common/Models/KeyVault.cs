using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using dAccounting.Common.Interfaces;

namespace dAccounting.Common.Models
{
    public class KeyVault : IKeyVault
    {
        #region Field Members
        // %TODO% ****
        //const string pukey = "i8mV3yX8Sw1HlTX4+Cwuo7jdj0rruD/rF/VH+1HeG7UFtqwluyt9MzOU0LsCIhBp/sz5hKaMon9/GH/R06w3vqX6ZTmrcOV9DtxT8woO4mVypAa/CPLGnPgrzH+9C5tn8+8fGjGzwK8Q6h6C6oflYfZM9AjX8DmWwoG379RWWJ5SFoPOocChpDDkuHvnDF7S4yV93epls+r8Fhch3yPcXNr4IWVh4st4jk15lmJzgQ+bnFBd9zxEft8CFKu5RZnzYJk4l+YaNd/orM0tae1ADOH6i+xcy/aEh9ogSDoMVGYG6PbXeoU9wYFajJMaX+8L/7wiDNAktd09+Jz7YXWs3A==";
        //const string privatekey = "INOZgTwuB75hpn4zcjvRphS5WLU6tj7tTe5j2Jn9R31/e/KBCfnQqV/JjC3rDiuGF/B4T/5HbAygdONoidBQHi1LQ4BacjHaPYiUv6yOUd7Tfa8GjN22IKuXfOkGpwjZiQM4N6+rN1bzPTy9ff35MISBq5stnAsQNPe1BXtUqGl5+fnLW847VmwKHox+zeUXivNiUK4Q7M6se/9Ke1S8yk9clCb4FhbFOBFNtBPQvh7ZVCWlPJhUXw1HyZnJq9CoeChYDMi+8tEOisjcVqtL/txFX0MNgBOnB53g0fcK0CxTZkLisliqJ6hm3nDEb+7dhA8ASNVnN9v/B5AwyiviXQ==";
        //                         publicKey = HostCryptology.AsymmetricDecryptionWithoutCertificate(Convert.FromBase64String(publickey), WCOHederaManager.HederaConfig.WorldComputerAsymmetricPrivateKey);

        private const string secretpublickey = "%TODO%-secret #2";
        private const string secretprivatekey = "%TODO%-secret #2";
        private byte[] encryptedPublicKeyBytes = null!;
        private byte[] encryptedPrivateKeyBytes = null!;
        #endregion

        #region Constructors
        public KeyVault() { } // Required for serialization
        public KeyVault( string? base64encryptedpublickey, string? base64encryptedprivatekey )
        {
            if (base64encryptedpublickey != null)
            {
                encryptedPublicKeyBytes = Encoding.UTF8.GetBytes(base64encryptedpublickey);
                HostCryptology.EncryptBuffer(encryptedPublicKeyBytes, 0, encryptedPublicKeyBytes.Length, Encoding.UTF8.GetBytes(secretpublickey));
                unsafe 
                { 
                    // Fill the passed in public key with zeros to remove it from memory as we are finished with it
                    fixed (char* ptr = base64encryptedpublickey)
                    {
                        for (int i = 0; i < base64encryptedpublickey.Length; i++)
                        {
                            ptr[i++] = '0';
                        }
                    }
                }
            }
            if (base64encryptedprivatekey != null)
            {
                encryptedPrivateKeyBytes = Encoding.UTF8.GetBytes(base64encryptedprivatekey);
                HostCryptology.EncryptBuffer(encryptedPrivateKeyBytes, 0, encryptedPrivateKeyBytes.Length, Encoding.UTF8.GetBytes(secretprivatekey));
                unsafe
                {
                    // Fill the passed in private key with zeros to remove it from memory as we are finished with it
                    fixed (char* ptr = base64encryptedprivatekey)
                    {
                        for (int i = 0; i < base64encryptedprivatekey.Length; i++)
                        {
                            ptr[i++] = '0';
                        }
                    }
                }
            }
        }

        //public KeyVault(byte[]? publickey, byte[]? privatekey)
        //{
        //    if (publickey != null)
        //    {
        //        encryptedPublicKeyBytes = (byte[])publickey.Clone();
        //        HostCryptology.EncryptBuffer(encryptedPublicKeyBytes, 0, encryptedPublicKeyBytes.Length, Encoding.UTF8.GetBytes(secretpublickey));
        //    }
        //    if (privatekey != null)
        //    {
        //        encryptedPrivateKeyBytes = (byte[])privatekey.Clone();
        //        HostCryptology.EncryptBuffer(encryptedPrivateKeyBytes, 0, encryptedPrivateKeyBytes.Length, Encoding.UTF8.GetBytes(secretprivatekey));
        //    }
        //}
        #endregion

        #region Public Interface
        public string? Base64EncryptedPublicKey
        { 
            get 
            {
                string? result = null!;
                if (encryptedPublicKeyBytes != null)
                {
                    // Make a copy of the encrypted public key 
                    byte[] buffer = new byte[encryptedPublicKeyBytes.Length];
                    encryptedPublicKeyBytes.CopyTo(buffer, 0);
                    // Decrypt the copy in-place
                    HostCryptology.DecryptBuffer(buffer, 0, buffer.Length, Encoding.UTF8.GetBytes(secretpublickey));
                    // Convert key to Xml string
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
            set
            {
                byte[] valueBytes = Encoding.ASCII.GetBytes(value!);
                encryptedPublicKeyBytes = new byte[valueBytes!.Length];
                valueBytes.CopyTo(encryptedPublicKeyBytes, 0);
                HostCryptology.EncryptBuffer(encryptedPublicKeyBytes, 0, encryptedPublicKeyBytes.Length, Encoding.UTF8.GetBytes(secretpublickey));
            }
        }

        public string? Base64EncryptedPrivateKey
        {
            get
            {
                string? result = null!;
                if (encryptedPrivateKeyBytes != null)
                {
                    // Make a copy of the encrypted private key 
                    byte[] buffer = new byte[encryptedPrivateKeyBytes.Length];
                    encryptedPrivateKeyBytes.CopyTo(buffer, 0);
                    // Decrypt the copy in-place
                    HostCryptology.DecryptBuffer(buffer, 0, buffer.Length, Encoding.UTF8.GetBytes(secretprivatekey));
                    // Convert key to Xml string
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
            set
            {
                byte[] valueBytes = Encoding.ASCII.GetBytes(value!);
                encryptedPrivateKeyBytes = new byte[valueBytes!.Length];
                valueBytes.CopyTo(encryptedPrivateKeyBytes, 0);
                HostCryptology.EncryptBuffer(encryptedPrivateKeyBytes, 0, encryptedPrivateKeyBytes.Length, Encoding.UTF8.GetBytes(secretpublickey));

            }
        }


        [JsonIgnore]
        public bool HasPublicKey 
        { 
            get
            {
                return (encryptedPublicKeyBytes != null);
            }
        }
        [JsonIgnore]
        public bool HasPrivateKey
        {
            get
            {
                return (encryptedPrivateKeyBytes != null);
            }
        }

    //[JsonIgnore]
    //public string? PublicKeyBase64Encrypted
    //{
    //    get
    //    {
    //        string? result = null!;
    //        if (encryptedPublicKeyBytes != null)
    //        {
    //            // Make a copy of the encrypted public key 
    //            byte[] buffer = new byte[encryptedPublicKeyBytes.Length];
    //            encryptedPublicKeyBytes.CopyTo(buffer, 0);
    //            // Decrypt the copy in-place
    //            HostCryptology.DecryptBuffer(buffer, 0, buffer.Length, Encoding.UTF8.GetBytes(secretpublickey));
    //            // Convert key to base64 string of an Xml
    //            result = Convert.ToBase64String(buffer);
    //            // Zero buffer holding sensitive private key information as we are finished with it now.
    //            for (int i = 0; i < buffer.Length; i++)
    //            {
    //                buffer[i] = 0;
    //            }
    //            buffer = null!;
    //        }
    //        return result;
    //    }
    //}

    //[JsonIgnore]
    //public string? PrivateKeyBase64Encrypted
    //{
    //    get
    //    {
    //        string? result = null!;
    //        if (encryptedPrivateKeyBytes != null)
    //        {
    //            // Make a copy of the encrypted private key 
    //            byte[] buffer = new byte[encryptedPrivateKeyBytes.Length];
    //            encryptedPrivateKeyBytes.CopyTo(buffer, 0);
    //            // Decrypt the copy in-place
    //            HostCryptology.DecryptBuffer(buffer, 0, buffer.Length, Encoding.UTF8.GetBytes(secretprivatekey));
    //            // Convert key to base64 string of an Xml
    //            result = Convert.ToBase64String(buffer);
    //            // Zero buffer holding sensitive private key information as we are finished with it now.
    //            for (int i = 0; i < buffer.Length; i++)
    //            {
    //                buffer[i] = 0;
    //            }
    //            buffer = null!;
    //        }
    //        return result;
    //    }
    //}
    #endregion

    #region Helpers
    #endregion
}
}
