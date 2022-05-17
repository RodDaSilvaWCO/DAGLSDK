namespace dAccounting.Common.Models
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Reflection;
	using System.Security.Authentication;
	using System.Security.Claims;
	using System.Security.Cryptography;
	using System.Text;
	using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    public class HostCryptology
	{
		//static HttpClient client = null!;
		//static object clientLock = new object();
		//static RSA rsa = RSA.Create();
		#region Key Generation
		//#region Symmetric
		//public static void SymmetricKeyGeneration(out byte[] Key, out byte[] IV, bool use32ByteIV = false)
		//{
		//	Key = null!;
		//	IV = null!;

		//	using (RijndaelManaged provider = new RijndaelManaged())
		//	{
		//		provider.KeySize = 256;
		//		if (use32ByteIV)
		//			provider.BlockSize = 256;

		//		provider.GenerateKey();
		//		provider.GenerateIV();
		//		Key = provider.Key;
		//		IV = provider.IV;
		//	}
		//}
  //      #endregion

  //      #region Asymmetric
  //      //public static void AsymmetricKeyGeneration(out string publickey, out string privatekey)
  //      //{
  //      //    publickey = null!;
  //      //    privatekey = null!;
  //      //    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
  //      //    {
  //      //        publickey = rsa.ToXmlString2(false);
  //      //        privatekey = rsa.ToXmlString2(true);
  //      //        //System.Diagnostics.Debug.Print( "KeySize:{0}", rsa.KeySize );
  //      //    }
  //      //}

  //      public static void AsymmetricKeyGeneration(out string publickey, out string privatekey)
  //      {
  //          publickey = null!;
  //          privatekey = null!;

  //          using (RSA rsa = RSA.Create())
  //          {
  //              rsa.KeySize = 1024;  // uses the default keysize of the RSACryptoServiceProvider class
		//		publickey = rsa.ToXmlString2(false);
  //              privatekey = rsa.ToXmlString2(true);
  //          }
  //      }

  //      public static void AsymmetricKeyGeneration(int keySizeInBits, out string publickey, out string privatekey)
		//{
		//	publickey = null!;
		//	privatekey = null!;
		//	using (RSA rsa = RSA.Create())
		//	{
		//		rsa.KeySize = keySizeInBits;
		//		publickey = rsa.ToXmlString2(false);
		//		privatekey = rsa.ToXmlString2(true);
		//		//System.Diagnostics.Debug.Print( "KeySize:{0}", rsa.KeySize );
		//	}
		//}
		//#endregion
		#endregion

		#region CallerAuthentication
		//static public async Task<SecureApiCallResponse> SecureApiCall(string callUriTemplate, GridOperation op, string payload, Guid sourceID, string targetAsymmetricPublicKey, string sourceAsymmetricPrivateKey)
		//{
		//	SecureApiCallResponse output = new SecureApiCallResponse();
		//	try
		//	{
		//		//Debug.Print($"UnoSys.Common.SecureApiCall(A) - absoluteUri={callUriTemplate}, op={op}");
		//		byte[] sourceid = sourceID.ToByteArray();
		//		// Generate the symmetric key to use for this call
		//		byte[] key = null!;
		//		byte[] iv = null!;
		//		HostCryptology.SymmetricKeyGeneration(out key, out iv);
		//		// Combine sourceid + key + iv to form the SessionToken
		//		byte[] sessionToken = new byte[64];

		//		Buffer.BlockCopy(sourceid, 0, sessionToken, 0, 16);
		//		Buffer.BlockCopy(key, 0, sessionToken, 16, 32);
		//		Buffer.BlockCopy(iv, 0, sessionToken, 48, 16);
		//		// Encrypt the sessionToken with the Grid's  asymmetric public key
		//		string encryptedHexSessionToken = HostCryptology.ConvertBytesToHexString(
		//				HostCryptology.AsymmetricEncryptionWithoutCertificate(sessionToken, targetAsymmetricPublicKey));
		//		// Sign the SourceID with the Node's asymmetric private key
		//		byte[] signature = HostCryptology.AsymmetricSigningWithoutCertificate(sourceid, sourceAsymmetricPrivateKey);
		//		// Encrypt signature + payload using generated symmetric key
		//		string encryptedPayLoad = HostCryptology.SymmetricEncryptAndHexEncode(HostCryptology.ConvertBytesToHexString(signature) + payload, ref key, ref iv);
		//		// Now send it
		//		string absoluteUri = string.Format(callUriTemplate, encryptedHexSessionToken);
		//		HttpRequestMessage request = new HttpRequestMessage();
		//		HttpResponseMessage response = null!;
		//		// Note:  Quotes are required!!
		//		StringContent stringContent = new StringContent("\"" + ((int)op).ToString("X4") + encryptedPayLoad + "\"", new UTF8Encoding(), "application/json");
		//		lock (client)
		//		{
		//			client.DefaultRequestHeaders.Accept.Clear();
		//			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		//			response = client.PostAsync(absoluteUri, stringContent).Result;
		//		}
		//		output.Status = response.StatusCode;
		//		if (response.IsSuccessStatusCode)
		//		{
		//			output.Response = await response.Content.ReadAsStringAsync();
		//			string rsp = output.Response.Substring(1, output.Response.Length - 2);

		//			if (string.IsNullOrEmpty(rsp))
		//			{
		//				output.Response = "";
		//			}
		//			else
		//			{
		//				//Debug.Print($"UnoSys.Common.SecureApiCall() - absoluteUri={absoluteUri}, rsp={rsp}");
		//				output.Response = HostCryptology.SymmetricHexDecodeAndDecrypt(rsp, key, iv);
		//			}
		//		}
		//		output.Status = response.StatusCode;
		//		//Debug.Print($"DiskBounty.Common.SecureApiCall() output.Status={output.Status}, output.Response={output.Response} ");
		//	}
		//	catch (Exception /*ex*/)
		//	{
		//		//System.Diagnostics.Debug.Print("Error in HostCryptology.SecureApiCall() call: {0}", ex.ToString() );
		//		// NOTE:  We choose PartialContent as the Status here so that we can differentiate between a typical HTTP error code (i.e.; Bad_Request(400) and an error in our logic above)
		//		output.Status = System.Net.HttpStatusCode.PartialContent;  // If we got here then the call returned but there was a problem with the interpreting the data
		//	}
		//	return output;
		//}

		//static public async Task<SecureApiCallResponse> SecureApiCall(string callUriTemplate, /*GridOperation*/ int op, string payload, Guid sourceID, string targetAsymmetricPublicKey, string sourceAsymmetricPrivateKey)
		//{
		//	SecureApiCallResponse output = new SecureApiCallResponse();
		//	try
		//	{
		//		byte[] key = null!;
		//		byte[] iv = null!;
		//		HttpResponseMessage response = null!;
		//		int count = 0;
		//		int conflictCount = 0;
		//		// API Call in retry loop 
		//		while (true)
		//		{
		//			count++;
		//			response = RawSecureApiCall(callUriTemplate, op, payload, sourceID, targetAsymmetricPublicKey, sourceAsymmetricPrivateKey, ref key, ref iv);
		//			output.Status = response.StatusCode;
		//			if (response.IsSuccessStatusCode)
		//			{
		//				output.Response = await response.Content.ReadAsStringAsync();
		//				string rsp = output.Response.Substring(1, output.Response.Length - 2);
		//				if (string.IsNullOrEmpty(rsp))
		//				{
		//					output.Response = "";
		//				}
		//				else
		//				{
		//					//Debug.Print($"UnoSys.Common.SecureApiCall() - absoluteUri={absoluteUri}, rsp={rsp}");
		//					output.Response = HostCryptology.SymmetricHexDecodeAndDecrypt(rsp, key, iv);
		//				}
		//				break;  // Exit loop as call made it to the API and we received a response of some sort
		//			}
		//			else
		//			{
		//				// Do not retry when we get Unauthorized respones - avoiding possible denial of service
		//				if (output.Status == HttpStatusCode.Unauthorized)
		//				{
		//					break;
		//				}
		//				// Conflicts are expected when resources can't be accessed - we don't want to infinitely try them
		//				if (output.Status == HttpStatusCode.Conflict || output.Status == HttpStatusCode.BadRequest)
		//				{
		//					conflictCount++;
		//					if (conflictCount > 10)
		//					{
		//						break;
		//					}
		//				}

		//				if (count % 10 == 0)
		//				{
		//					PAL.Log($"2222222222 HostCryptology.SecureApiCall(1) FAILED - response={output.Status} reason={response.ReasonPhrase}, op={op} - retry....\n{Environment.StackTrace}");
		//					break;  // exit retry loop
		//				}
		//				await Task.Delay(500).ConfigureAwait(false);
		//			}
		//		}
		//		output.Status = response.StatusCode;
		//		//Debug.Print($"DiskBounty.Common.SecureApiCall() output.Status={output.Status}, output.Response={output.Response} ");
		//	}
		//	catch (Exception ex)
		//	{
		//		PAL.Log($"2222222222 HostCryptology.SecureApiCall(2) FAILED - response={ex.ToString()}");
		//		//System.Diagnostics.Debug.Print("Error in HostCryptology.SecureApiCall() call: {0}", ex.ToString() );
		//		// NOTE:  We choose PartialContent as the Status here so that we can differentiate between a typical HTTP error code (i.e.; Bad_Request(400) and an error in our logic above)
		//		output.Status = System.Net.HttpStatusCode.PartialContent;  // If we got here then the call returned but there was a problem with interpreting the data
		//	}
		//	return output;
		//}


		//static public HttpResponseMessage RawSecureApiCall(string callUriTemplate, /*GridOperation*/ int op, string payload, Guid sourceID,
		//			string targetAsymmetricPublicKey, string sourceAsymmetricPrivateKey, ref byte[] key, ref byte[] iv)
		//{
		//	HttpResponseMessage response = null!;
		//	//Debug.Print($"UnoSys.Core.RawSecureApiCall(A) - absoluteUri={callUriTemplate}, op={op}");
		//	byte[] sourceid = sourceID.ToByteArray();
		//	// Generate the symmetric key to use for this call
		//	HostCryptology.SymmetricKeyGeneration(out key, out iv);
		//	// Combine sourceid + key + iv to form the SessionToken
		//	byte[] sessionToken = new byte[64];
		//	Buffer.BlockCopy(sourceid, 0, sessionToken, 0, 16);
		//	Buffer.BlockCopy(key, 0, sessionToken, 16, 32);
		//	Buffer.BlockCopy(iv, 0, sessionToken, 48, 16);
		//	// Encrypt the sessionToken with the Grid's  asymmetric public key
		//	string encryptedHexSessionToken = HostCryptology.ConvertBytesToHexString(
		//			HostCryptology.AsymmetricEncryptionWithoutCertificate(sessionToken, targetAsymmetricPublicKey));
		//	// Sign the SourceID with the Node's asymmetric private key
		//	byte[] signature = HostCryptology.AsymmetricSigningWithoutCertificate(sourceid, sourceAsymmetricPrivateKey);
		//	// Encrypt signature + payload using generated symmetric key
		//	string encryptedPayLoad = HostCryptology.SymmetricEncryptAndHexEncode(HostCryptology.ConvertBytesToHexString(signature) + payload, ref key, ref iv);
		//	// Now send it
		//	string absoluteUri = string.Format(callUriTemplate, encryptedHexSessionToken);
		//	HttpRequestMessage request = new HttpRequestMessage();
		//	// Note:  Quotes are required!!
		//	StringContent stringContent = new StringContent("\"" + ((int)op).ToString("X4") + encryptedPayLoad + "\"", new UTF8Encoding(), "application/json");
		//	//lock (clientLock)
		//	//{
		//	//	if(client == null!)  // lazy instantiation
		//	//             {
		//	//		client = new HttpClient(GetHandler());
		//	//             }
		//	//	client.DefaultRequestHeaders.Accept.Clear();
		//	//	client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		//	//	//Debug.Print($"***** UnoSys.Core.HostCryptology.RawSecureApiCall - absoluteUri={absoluteUri}");
		//	//	response = client.PostAsync(absoluteUri, stringContent).Result;
		//	//	//Debug.Print($"UnoSys.Core.RawSecureApiCall(C) - response status={(response==null ? "null" : response.StatusCode.ToString() )}, op={op}");
		//	//}
		//	//lock (clientLock)
		//	//{
		//	// %TODO% - Use HttpClientFactory...
		//	using (var handler = GetHandler())
		//	{
		//		using (var httpClient = new HttpClient(handler))
		//		{
		//			httpClient.DefaultRequestHeaders.Accept.Clear();
		//			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		//			//Debug.Print($"***** UnoSys.Core.HostCryptology.RawSecureApiCall - absoluteUri={absoluteUri}");
		//			response = httpClient.PostAsync(absoluteUri, stringContent).Result;
		//			//Debug.Print($"UnoSys.Core.RawSecureApiCall(C) - response status={(response==null ? "null" : response.StatusCode.ToString() )}, op={op}");
		//		}
		//	}
		//	//}
		//	return response;
		//}

		//public static string ComputeDID( Guid salt, string publicKey, out byte[] hash)
  //      {
		//	// First Generate a sufficiently strong 16-byte hash of the public key using ID as the salt
		//	hash = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(publicKey), salt.ToByteArray(), 2000).GetBytes(16);
		//	// Finally generate the DID from the encrypted multiformat base64 (padded) string version of the computed hash
		//	return  Base64Padded.Encode(MultibaseEncoding.Base64Padded, hash);
		//}

		//#if WORLDCOMPUTER
		//		static public async Task<SecureApiCallResponse> SecureOverlayCall(string callUriTemplate, GridOperation op, string payload, Guid sourceID, string targetAsymmetricPublicKey, string sourceAsymmetricPrivateKey)
		//		{
		//			SecureApiCallResponse output = new SecureApiCallResponse();
		//			try
		//			{
		//				byte[] key = null!;
		//				byte[] iv = null!;
		//				HttpResponseMessage response = null!;
		//				int count = 0;
		//				int conflictCount = 0;
		//				// Overlay Call retry loop - we stay in here until the Overlay accepts the call (i.e.; the Overlay is reachable)
		//				while (true)
		//				{
		//					count++;
		//					response = RawSecureOverlayCall(callUriTemplate, op, payload, sourceID, targetAsymmetricPublicKey, sourceAsymmetricPrivateKey, ref key, ref iv);
		//					output.Status = response.StatusCode;
		//					if (response.IsSuccessStatusCode)
		//					{
		//						output.Response = await response.Content.ReadAsStringAsync();
		//						string rsp = output.Response.Substring(1, output.Response.Length - 2);
		//						if (string.IsNullOrEmpty(rsp))
		//						{
		//							output.Response = "";
		//						}
		//						else
		//						{
		//							//Debug.Print($"UnoSys.Common.SecureApiCall() - absoluteUri={absoluteUri}, rsp={rsp}");
		//							output.Response = HostCryptology.SymmetricHexDecodeAndDecrypt(rsp, key, iv);
		//						}
		//						break;  // Exit loop as call made it to the API and we received a response of some sort
		//					}
		//					else
		//					{
		//						// Conflicts are expected when resources can't be accessed - we don't want to infinitely try them
		//						if (output.Status == HttpStatusCode.Conflict || output.Status == HttpStatusCode.BadRequest)
		//						{
		//							conflictCount++;
		//							if (conflictCount > 10)
		//							{
		//								break;
		//							}
		//						}

		//						if (count % 10 == 0)
		//						{
		//							PAL.Log($"2222222222 HostCryptology.SecureApiCall(1) FAILED - response={output.Status} reason={response.ReasonPhrase}, op={op} - retry....\n{Environment.StackTrace}");
		//						}
		//						await Task.Delay(100).ConfigureAwait(false);
		//					}
		//				}
		//				output.Status = response.StatusCode;
		//				//Debug.Print($"DiskBounty.Common.SecureApiCall() output.Status={output.Status}, output.Response={output.Response} ");
		//			}
		//			catch (Exception ex)
		//			{
		//				PAL.Log($"2222222222 HostCryptology.SecureApiCall(2) FAILED - response={ex.ToString()}");
		//				//System.Diagnostics.Debug.Print("Error in HostCryptology.SecureApiCall() call: {0}", ex.ToString() );
		//				// NOTE:  We choose PartialContent as the Status here so that we can differentiate between a typical HTTP error code (i.e.; Bad_Request(400) and an error in our logic above)
		//				output.Status = System.Net.HttpStatusCode.PartialContent;  // If we got here then the call returned but there was a problem with the interpreting the data
		//			}
		//			return output;
		//		}

		//		static public HttpResponseMessage RawSecureOverlayCall(string callUriTemplate, GridOperation op, string payload, Guid sourceID,
		//					string targetAsymmetricPublicKey, string sourceAsymmetricPrivateKey, ref byte[] key, ref byte[] iv)
		//		{
		//			HttpResponseMessage response = null!;
		//			//Debug.Print($"UnoSys.Common.SecureApiCall(A) - absoluteUri={callUriTemplate}, op={op}");
		//			byte[] sourceid = sourceID.ToByteArray();
		//			// Generate the symmetric key to use for this call
		//			HostCryptology.SymmetricKeyGeneration(out key, out iv);
		//			// Combine sourceid + key + iv to form the SessionToken
		//			byte[] sessionToken = new byte[64];
		//			Buffer.BlockCopy(sourceid, 0, sessionToken, 0, 16);
		//			Buffer.BlockCopy(key, 0, sessionToken, 16, 32);
		//			Buffer.BlockCopy(iv, 0, sessionToken, 48, 16);
		//			// Encrypt the sessionToken with the Grid's  asymmetric public key
		//			string encryptedHexSessionToken = HostCryptology.ConvertBytesToHexString(
		//					HostCryptology.AsymmetricEncryptionWithoutCertificate(sessionToken, targetAsymmetricPublicKey));
		//			// Sign the SourceID with the Node's asymmetric private key
		//			byte[] signature = HostCryptology.AsymmetricSigningWithoutCertificate(sourceid, sourceAsymmetricPrivateKey);
		//			// Encrypt signature + payload using generated symmetric key
		//			string encryptedPayLoad = HostCryptology.SymmetricEncryptAndHexEncode(HostCryptology.ConvertBytesToHexString(signature) + payload, ref key, ref iv);
		//			// Now send it
		//			string absoluteUri = string.Format(callUriTemplate, encryptedHexSessionToken);
		//			HttpRequestMessage request = new HttpRequestMessage();
		//			// Note:  Quotes are required!!
		//			StringContent stringContent = new StringContent("\"" + ((int)op).ToString("X4") + encryptedPayLoad + "\"", new UTF8Encoding(), "application/json");
		//			lock (client)
		//			{
		//				client.DefaultRequestHeaders.Accept.Clear();
		//				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		//				response = client.PostAsync(absoluteUri, stringContent).Result;
		//			}
		//			return response;
		//		}
		//#endif

		//public delegate bool ValidateCaller(Guid ID, ref string IDPublicKey);


		//static public bool ValidateXmlPayLoad<T, TS>(string sessionToken, string privateDecryptionKey, string input, ref T payLoad, ref byte[] key, ref byte[] iv, ref Guid nodeID, ref string publicVerificationKey, ValidateCaller validateCaller)
		//{
		//	bool result = false;
		//	try
		//	{
		//		//// Authenticate caller
		//		string strPayLoad = null!;
		//		if (ValidateStringPayLoad(sessionToken, privateDecryptionKey, input, ref strPayLoad, ref key, ref iv, ref nodeID, ref publicVerificationKey, validateCaller))
		//		{
		//			// Process Xml payload
		//			XmlSerializer xs = (XmlSerializer)Activator.CreateInstance(typeof(TS));
		//			using (TextReader reader = new StringReader(strPayLoad))
		//			{
		//				payLoad = (T)xs.Deserialize(reader);
		//			}
		//			result = true;
		//		}
		//		else
		//		{
		//			Debug.Print($"[API] *** HostCryptology.ValidateXmlPayLoad () - ValidateStringPayLoad failed...");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Debug.Print("[ERR] HostHttp.ValidateXmlPayLoad<T,TS>() -> {0}", ex.ToString());
		//		result = false;
		//	}
		//	return result;
		//}



		//static public bool ValidateJsonPayLoad<T>(string sessionToken, string privateDecryptionKey, string input, ref T payLoad, ref byte[] key, ref byte[] iv, ref Guid nodeID, ref string publicVerificationKey, ValidateCaller validateCaller)
		//{
		//	bool result = false;
		//	try
		//	{
		//		//// Authenticate caller
		//		string strPayLoad = null!;
		//		if (ValidateStringPayLoad(sessionToken, privateDecryptionKey, input, ref strPayLoad, ref key, ref iv, ref nodeID, ref publicVerificationKey, validateCaller))
		//		{
		//			// Process Xml payload
		//			//XmlSerializer xs = (XmlSerializer)Activator.CreateInstance(typeof(TS));
		//			//using (TextReader reader = new StringReader(strPayLoad))
		//			//{
		//			//    payLoad = (T)xs.Deserialize(reader);
		//			//}
		//			payLoad = HostUtilities.JsonDeserialize<T>(strPayLoad);
		//			result = true;
		//		}
		//		else
		//		{
		//			Debug.Print($"[API] *** HostCryptology.ValidateJsonPayLoad () - ValidateStringPayLoad failed...");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Debug.Print("[ERR] Unsys.Common.ValidateJsonPayLoad<T>() -> {0}", ex.ToString());
		//		result = false;
		//	}
		//	return result;
		//}

		//static public bool ValidateStringPayLoad(string sessionToken, string gridAsymmetricPrivateKey, string input, ref string payLoad, ref byte[] key, ref byte[] iv, ref Guid nodeID, ref string publicVerificationKey, ValidateCaller validateCaller)
		//{
		//	bool result = false;
		//	try
		//	{
		//		// Authenticate caller
		//		key = null!;
		//		iv = null!;
		//		if (AuthenticateCaller(sessionToken, gridAsymmetricPrivateKey, ref nodeID, ref key, ref iv, ref publicVerificationKey, validateCaller))
		//		{
		//			// Decrypt signature + payload using key/iv symmetric key
		//			string signedpayload = HostCryptology.SymmetricHexDecodeAndDecrypt(input, key, iv);
		//			// verify signature (first 256 bytes)
		//			string signature = signedpayload.Substring(0, 256);
		//			if (HostCryptology.AsymmetricVerificationWithoutCertificate(nodeID.ToByteArray(),
		//						HostCryptology.ConvertHexStringToBytes(signature), publicVerificationKey))
		//			{
		//				payLoad = signedpayload.Substring(256);
		//				result = true;
		//			}
		//		}
		//		else
		//		{
		//			Debug.Print($"[API] *** HostCryptology.ValidateStringPayLoad () - AuthenticateCaller failed...");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Debug.Print("[ERR] UnoSys.Core.ValidateStringPayLoad() -> {0}", ex.ToString());
		//		result = false;
		//	}

		//	return result;
		//}




		//static private bool AuthenticateCaller(string encryptedHexSessionToken, string gridAsymmetricPrivateKey, ref Guid ID, ref byte[] key, ref byte[] iv, ref string IDPublicKey, ValidateCaller validateCaller)
		//{
		//	bool result = true;
		//	try
		//	{
		//		byte[] encryptedSessionToken = HostCryptology.ConvertHexStringToBytes(encryptedHexSessionToken);
		//		byte[] sessionToken = HostCryptology.AsymmetricDecryptionWithoutCertificate(encryptedSessionToken, gridAsymmetricPrivateKey);
		//		// Extract the 3 parts of the sessionToken:  node + generated symm key + generated symm iv
		//		byte[] node = new byte[16];
		//		key = new byte[32];
		//		iv = new byte[16];
		//		Buffer.BlockCopy(sessionToken, 0, node, 0, 16);
		//		Buffer.BlockCopy(sessionToken, 16, key, 0, 32);
		//		Buffer.BlockCopy(sessionToken, 48, iv, 0, 16);
		//		ID = new Guid(node);
		//		result = validateCaller(ID, ref IDPublicKey);
		//	}
		//	catch (Exception ex)
		//	{
		//		Debug.Print("[ERR] UnoSys.Core.AuthenticateCaller() -> {0}", ex.ToString());
		//		result = false;
		//	}
		//	return result;
		//}

		#endregion

		#region Asymmetric Encryption/Decryption
		public static byte[] AsymmetricEncryptionWithoutCertificate(byte[] dataToEncrypted, string publickey)
		{
			byte[] encryptedData = null!;
			using (RSA rsa = RSA.Create())
			{
				rsa.FromXmlString2(publickey); // encrypt using public key
				encryptedData = rsa.Encrypt(dataToEncrypted, RSAEncryptionPadding.OaepSHA1);
			}
			return encryptedData;
		}

		public static byte[] AsymmetricDecryptionWithoutCertificate(byte[] dataToDecrypt, string privatekey)
		{
			byte[] decryptedData = null!;
			using (RSA rsa = RSA.Create())
			{
				rsa.FromXmlString2(privatekey); // decrypt using private key
				decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA1);
			}
			return decryptedData;
		}
		#endregion

		#region Asymmetric Signing/Verification
		public static byte[] AsymmetricSigningWithoutCertificate(byte[] dataToSign, string privateKey)
		{
			byte[] signature = null!;

			// Sender's end
			//using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
			using (RSA rsa = RSA.Create())
			{
				rsa.FromXmlString2(privateKey); // sign using private key
												//signature = rsa.SignData(dataToSign, "SHA256");
				signature = rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
			}
			return signature;
		}

		public static bool AsymmetricVerificationWithoutCertificate(byte[] dataToVerify, byte[] signature, string publicKey)
		{
			bool result = false;
			// Receiver's end
			//using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
			using (RSA rsa = RSA.Create())
			{
				rsa.FromXmlString2(publicKey); // validate using public key
											   //result = rsa.VerifyData(dataToVerify, "SHA256", signature); // Outputs True, if signature is valid
				result = rsa.VerifyData(dataToVerify, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1); // Outputs True, if signature is valid
			}
			return result;
		}
		#endregion

		#region Symmetric Encryption/Decryption
		// This function assumes the key and IV passed in are 64Base encoded 
		//public static string EncryptForGridHost(string value, string key, string initVector)
		//{
		//	byte[] bytes = Encoding.UTF8.GetBytes(value);
		//	return EncryptForGridHost(bytes, key, initVector);
		//}


		//public static string EncryptForGridHost(byte[] bytes, string key, string initVector)
		//{
		//	byte[] encryptedBytes = null!;
		//	using (RijndaelManaged provider = new RijndaelManaged())
		//	{
		//		provider.KeySize = 256;
		//		provider.BlockSize = 256;
		//		byte[] Key = Convert.FromBase64String(key);
		//		byte[] InitVector = Convert.FromBase64String(initVector);
		//		encryptedBytes = Transform(bytes, 0, bytes.Length, provider.CreateEncryptor(Key, InitVector));
		//	}
		//	string result = Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);
		//	return result;
		//}

		//public static string SymmetricEncryptAndHexEncode(string dataToEncrypt, ref byte[] key, ref byte[] iv)
		//{
		//	byte[] bytes = Encoding.UTF8.GetBytes(dataToEncrypt);
		//	return SymmetricEncryptAndHexEncode(bytes, ref key, ref iv);
		//}

		public static string SymmetricEncryptAndBase64Encode(string dataToEncrypt, ref byte[] key, ref byte[] iv)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(dataToEncrypt);
			return SymmetricEncryptAndBase64Encode(bytes, ref key, ref iv);
		}


		//public static string SymmetricEncryptAndHexEncode(byte[] bytesToEncrypt, ref byte[] key, ref byte[] iv)
		//{
		//	byte[] foggyBytes = null!;
		//	using (AesManaged provider = new AesManaged())
		//	{
		//		if (key == null!)
		//		{
		//			provider.GenerateKey();
		//			key = provider.Key;
		//		}
		//		if (iv == null!)
		//		{
		//			provider.GenerateIV();
		//			iv = provider.IV;
		//		}
		//		foggyBytes = Transform(bytesToEncrypt, 0, bytesToEncrypt.Length, provider.CreateEncryptor(key, iv));
		//	}
		//	return ConvertBytesToHexString(foggyBytes);
		//}


		public static string SymmetricEncryptAndBase64Encode(byte[] bytesToEncrypt, ref byte[] key, ref byte[] iv)
		{
			byte[] foggyBytes = null!;
			//using (AesManaged provider = new AesManaged())
			using (Aes provider = Aes.Create())
			{
				if (key == null!)
				{
					provider.GenerateKey();
					key = provider.Key;
				}
				if (iv == null!)
				{
					provider.GenerateIV();
					iv = provider.IV;
				}
				foggyBytes = Transform(bytesToEncrypt, 0, bytesToEncrypt.Length, provider.CreateEncryptor(key, iv));
			}
			return Convert.ToBase64String(foggyBytes);
		}

		//public static string DecryptFromGridHost(string value, string key, string initVector, bool isJson = true)
		//{
		//	if (isJson)
		//	{
		//		value = value.Substring(1, value.Length - 2);  // Strip off enclosing quotes (not sure why they are there at all)
		//	}
		//	byte[] encryptedValue = System.Convert.FromBase64String(value);
		//	byte[] rawBytes = null!;

		//	using (RijndaelManaged provider = new RijndaelManaged())
		//	{
		//		provider.KeySize = 256;
		//		provider.BlockSize = 256;
		//		byte[] Key = Convert.FromBase64String(key);
		//		byte[] InitVector = Convert.FromBase64String(initVector);
		//		rawBytes = Transform(encryptedValue, 0, encryptedValue.Length, provider.CreateDecryptor(Key, InitVector));
		//	}
		//	return Encoding.UTF8.GetString(rawBytes);
		//}


		//public static string SymmetricHexDecodeAndDecrypt(string hexEncodedAndEncryptedData, byte[] key, byte[] iv)
		//{
		//	byte[] clearBytes = null!;
		//	using (AesManaged provider = new AesManaged())
		//	{
		//		byte[] foggyBytes = ConvertHexStringToBytes(hexEncodedAndEncryptedData);
		//		clearBytes = Transform(foggyBytes, 0, foggyBytes.Length, provider.CreateDecryptor(key, iv));
		//	}
		//	return Encoding.UTF8.GetString(clearBytes);
		//}

		public static byte[] SymmetricBase64DecodeAndDecrypt(string base64EncodedAndEncryptedData, byte[] key, byte[] iv)
		{
			byte[] clearBytes = null!;
			//using (AesManaged provider = new AesManaged())
			using (Aes provider = Aes.Create())
			{
				byte[] foggyBytes = Convert.FromBase64String(base64EncodedAndEncryptedData);
				clearBytes = Transform(foggyBytes, 0, foggyBytes.Length, provider.CreateDecryptor(key, iv));
			}
			//return Encoding.UTF8.GetString(clearBytes);
			return clearBytes;
		}


		//public static byte[] Encrypt2(byte[] dataToEncrypt, ref byte[] key, ref byte[] iv, bool use32ByteIV = true)
		//{
		//	return Encrypt2(dataToEncrypt, 0, dataToEncrypt.Length, ref key, ref iv, use32ByteIV);
		//}

		//public static byte[] Encrypt2(byte[] dataToEncrypt, int offset, int count, ref byte[] key, ref byte[] iv, bool use32ByteIV = true)
		//{
		//	byte[] foggyBytes = null!;
		//	if (use32ByteIV)
		//	{
		//		byte[] skipBytes = new byte[count];
		//		Buffer.BlockCopy(dataToEncrypt, offset, skipBytes, 0, count);
		//		return skipBytes;
		//	}
		//	else
		//	{
		//		using (RijndaelManaged provider = new RijndaelManaged())
		//		{
		//			provider.KeySize = 256;
		//			if (use32ByteIV)
		//				provider.BlockSize = 256;

		//			if (key == null!)
		//			{
		//				provider.GenerateKey();
		//				key = provider.Key;
		//			}
		//			if (iv == null!)
		//			{
		//				provider.GenerateIV();
		//				iv = provider.IV;
		//			}
		//			foggyBytes = Transform(dataToEncrypt, offset, count, provider.CreateEncryptor(key, iv));
		//		}
		//	}
		//	return foggyBytes;
		//}

		//public static byte[] Decrypt2(byte[] foggyBytes, byte[] key, byte[] iv, bool use32ByteIV = true)
		//{
		//	return Decrypt2(foggyBytes, 0, foggyBytes.Length, key, iv, use32ByteIV);
		//}

		//public static byte[] Decrypt2(byte[] foggyBytes, int offset, int count, byte[] key, byte[] iv, bool use32ByteIV = true)
		//{
		//	byte[] clearBytes = null!;
		//	if (use32ByteIV)
		//	{
		//		return foggyBytes;
		//	}
		//	else
		//	{
		//		using (RijndaelManaged provider = new RijndaelManaged())
		//		{
		//			provider.KeySize = 256;
		//			if (use32ByteIV)
		//				provider.BlockSize = 256;

		//			clearBytes = Transform(foggyBytes, offset, count, provider.CreateDecryptor(key, iv));
		//		}
		//	}
		//	return clearBytes;
		//}


		public static bool EncryptBuffer(byte[] plainBytes, int bufferOffset, int byteOperationCount, byte[] password)
		{
			#region Validate inputs            
			int passwordBytesLength = password.Length;
			if (plainBytes == null!) throw new ArgumentException(" Must provide a buffer to encrypt.");
			if (password == null || passwordBytesLength == 0) throw new ArgumentException("Must provide a password to encrypt with.");
			int plainBytesLength = plainBytes.Length;
			if (plainBytesLength == 0)
				return false;  // nothing to do.
			#endregion
			byte currentPasswordByte = 0;
			for (int i = 0; i < passwordBytesLength; i++)
			{
				currentPasswordByte += password[i];
			}
			int index = 0;
			for (int i = bufferOffset; i < bufferOffset + byteOperationCount; i++)
			{
				byte currentPlainByte = plainBytes[i];
				byte encryptedByte = Multiply257((byte)(currentPlainByte + currentPasswordByte), currentPasswordByte);
				currentPasswordByte = Multiply257((byte)(currentPlainByte + encryptedByte), currentPasswordByte);
				plainBytes[i] = encryptedByte;
				index = (index + 1) % passwordBytesLength;
				currentPasswordByte = Multiply257(password[index], currentPasswordByte);
			}
			//PAL.Log($"&&&&&&&&&&&&&&&&& Inside UnoSysEncryption.EncryptBuffer() plain.Length={plainBytes.Length}, bufferoffset={bufferOffset}, byteOperationCount={byteOperationCount}, {password[0]},{password[1]},{password[2]},{password[3]}  &&&&&&&&&&&");
			return true;
		}

		public static bool DecryptBuffer(byte[] encryptedBytes, int bufferOffset, int byteOperationCount, byte[] password)
		{
			#region Validate inputs            
			int passwordBytesLength = password.Length;
			if (encryptedBytes == null!) throw new ArgumentException(" Must provide a buffer to decrypt.");
			if (password == null || passwordBytesLength == 0) throw new ArgumentException("Must provide a password to decrypt with.");
			int plainBytesLength = encryptedBytes.Length;
			if (plainBytesLength == 0)
				return false;  // nothing to do.
			#endregion
			byte currentPasswordByte = 0;
			for (int i = 0; i < passwordBytesLength; i++)
			{
				currentPasswordByte += password[i];
			}
			int index = 0;
			for (int i = bufferOffset; i < bufferOffset + byteOperationCount; i++)
			{
				byte currentEncryptedByte = encryptedBytes[i];
				byte plainByte = (byte)(Multiply257(currentEncryptedByte, Complement257(currentPasswordByte)) - currentPasswordByte);
				currentPasswordByte = Multiply257((byte)(currentEncryptedByte + plainByte), currentPasswordByte);
				encryptedBytes[i] = plainByte;
				index = (index + 1) % passwordBytesLength;
				currentPasswordByte = Multiply257(password[index], currentPasswordByte);
			}
			//PAL.Log($"^^^^^^^^^^^^^^^^^  Inside UnoSysEncryption.DecryptBuffer() encryptedBytes.Length={encryptedBytes.Length}, bufferoffset={bufferOffset}, byteOperationCount={byteOperationCount}, {password[0]},{password[1]},{password[2]},{password[3]} ^^^^^^^");
			return true;
		}

		private static byte Multiply257(byte a, byte b)
		{
			return (byte)((((a + 1) * (b + 1)) % 257) - 1);
		}

		private static byte[] complements = {
												  0,128,85,192,102,42,146,224,199,179,186,149,177,201,119,240,
												  120,99,229,89,48,221,189,74,71,88,237,100,194,59,198,248,
												  147,188,234,49,131,114,144,44,162,152,5,110,39,94,174,165,
												  20,35,125,172,96,118,242,178,247,225,60,29,58,227,101,252,
												  86,73,233,222,148,245,180,24,168,65,23,185,246,200,243,150,
												  164,209,95,204,126,2,64,183,25,19,208,175,151,215,45,82,
												  52,138,134,17,27,62,4,214,163,176,244,187,223,249,43,217,
												  115,123,37,112,133,158,53,14,16,157,139,113,219,50,84,254,
												  1,171,205,36,142,116,98,239,241,202,97,122,143,218,132,140,
												  38,212,6,32,68,11,79,92,41,251,193,228,238,121,117,203,
												  173,210,40,104,80,47,236,230,72,191,253,129,51,160,46,91,
												  105,12,55,9,70,232,190,87,231,75,10,107,33,22,182,169,
												  3,154,28,197,226,195,30,8,77,13,137,159,83,130,220,235,
												  90,81,161,216,145,250,103,93,211,111,141,124,206,21,67,108,
												  7,57,196,61,155,18,167,184,181,66,34,207,166,26,156,135,
												  15,136,54,78,106,69,76,56,31,109,213,153,63,170,127,255
											};

		private static byte Complement257(byte b) { return complements[b]; }
		#endregion


		//#region StrongNamePair
		//public static byte[] StrongNamePublickKey(byte[] SNKFileContents)
		//{
		//	StrongNameKeyPair snk = new StrongNameKeyPair(SNKFileContents);
		//	return snk.PublicKey;
		//}

		//#endregion


		#region Hashing
		public static string Rfc2898Hash(string password)
		{
			//Returns an RFC 2898 hash value for the specified password.</summary>
			if (password == null!)
				throw new ArgumentNullException("password");
			byte[] salt;
			byte[] bytes;
			using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, 16, 1000))
			{
				salt = rfc2898DeriveBytes.Salt;
				bytes = rfc2898DeriveBytes.GetBytes(32);
			}
			byte[] inArray = new byte[49];
			Buffer.BlockCopy((Array)salt, 0, (Array)inArray, 1, 16);
			Buffer.BlockCopy((Array)bytes, 0, (Array)inArray, 17, 32);
			return Convert.ToBase64String(inArray);
		}

		public static byte[] ComputeKeyedHashSHA512(byte[] bytesToHash, int bytesOffset, int bytesCount, byte[] hashKey)
		{
			HMACSHA512 hashAlgorithm = new HMACSHA512(hashKey);
			return hashAlgorithm.ComputeHash(bytesToHash, bytesOffset, bytesCount);
		}

		public static ulong Fnv1a_hash(byte[] bytesToHash, int offset, int count)
		{
			ulong hash = 0x811c9dc5;
			for (int i = offset; i < offset + count; i++)
			{
				hash ^= bytesToHash[i];
				hash *= 0x01000193;
			}
			return hash;
		}

		public static ulong Fnv1a_hash(byte[] bytesToHash, int offset)
		{
			return Fnv1a_hash(bytesToHash, offset, bytesToHash.Length - offset);
		}

		public static ulong Fnv1a_hash(byte[] bytesToHash)
		{
			return Fnv1a_hash(bytesToHash, 0, bytesToHash.Length);
			//ulong hash = 0x811c9dc5;
			//for (int i = 0; i < bytesToHash.Length; i++)
			//{
			//	//hash ^= (unsigned char) *cp++;
			//	hash ^= bytesToHash[i];
			//	hash *= 0x01000193;
			//}
			//return hash;
		}
		#endregion


//		#region JWT Token Support
//		public static string GetClaim(string token, string claimType)
//		{
//			var tokenHandler = new JwtSecurityTokenHandler();
//			var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
//			return GetClaim(securityToken, claimType);
//		}

//		public static string GetClaim(JwtSecurityToken securityToken, string claimType)
//		{
//			var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
//			return stringClaimValue;
//		}

//		public static bool ValidateAccessToken(string tokenId, string token, string publicKey, out List<Claim> claimList, out JwtSecurityToken securityToken)
//		{
//			SecurityToken? validatedToken = null!;
//			securityToken = null!;
//			claimList = null!;
//			bool result = true;
//			using (RSA rsa = RSA.Create())
////			lock(rsa)
//			{
//				rsa.FromXmlString2(publicKey);
//				var tokenHandler = new JwtSecurityTokenHandler();
//				try
//				{
//					var rsaKey = new RsaSecurityKey(rsa);
//					var tokenValidationParams = new TokenValidationParameters
//					{
//						ValidateIssuerSigningKey = true,
//						ValidateIssuer = true,
//						ValidateAudience = true,
//						ValidateLifetime = true,
//						ValidIssuer = "http://worldcomputer.org",
//						ValidAudience = $"http://worldcomputer.org/{tokenId}",
//						IssuerSigningKey = rsaKey,
//						CryptoProviderFactory = new CryptoProviderFactory()
//						{
//							CacheSignatureProviders = false
//						}

//					};
//					tokenHandler.ValidateToken(token, tokenValidationParams, out validatedToken);
//					claimList = new List<Claim>();
//					securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
//					if (securityToken != null)
//					{
//						foreach (var claim in securityToken.Claims)
//						{
//							claimList.Add(claim);
//						}
//					}
//				}
//				catch (Exception ex)
//				{
//					Debug.Print($"HostCryptology.ValidateToken() - Error:  {ex.ToString()}");
//					return false;
//				}
//			}
//			return result;
//		}


//		public static string GenerateAccessToken(string tokenId, string privateKey, List<Claim> claimList, DateTime expiration = default)
//		{
//			JwtSecurityTokenHandler tokenHandler = null!;
//			string result = null!;
//			using (RSA rsa = RSA.Create())
//			{
//				rsa.FromXmlString2(privateKey);
//				var rsaKey = new RsaSecurityKey(rsa);
//				var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha512Signature)
//				{
//					CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
//				};
//				tokenHandler = new JwtSecurityTokenHandler();
//				DateTime maxAllowedExpirationDate = new DateTime(2038, 1, 15);  // Max allowed date
//				if (expiration == default || expiration > maxAllowedExpirationDate)
//				{
//					expiration = maxAllowedExpirationDate;

//				}
//				var tokenDescriptor = new SecurityTokenDescriptor
//				{
//					Expires = expiration,
//					Issuer = "http://worldcomputer.org",
//					Audience = $"http://worldcomputer.org/{tokenId}",
//					SigningCredentials = signingCredentials
//				};
//				ClaimsIdentity claimsIdentity = null!;
//				if (claimList != null && claimList.Count > 0)
//				{
//					Claim[] claims = new Claim[claimList.Count];
//					int i = 0;
//					foreach (var claim in claimList)
//					{
//						claims[i++] = claim;
//					}
//					claimsIdentity = new ClaimsIdentity(claims);
//				}
//				else
//				{
//					claimsIdentity = new ClaimsIdentity();
//				}
//				tokenDescriptor.Subject = claimsIdentity;
//				result = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
//			}
//			return result;
//		}
//		#endregion



		#region Helpers
		private static HttpClientHandler GetHandler()
		{
			var handler = new HttpClientHandler();
			handler.ClientCertificateOptions = ClientCertificateOption.Manual;
			handler.SslProtocols = SslProtocols.None;
			handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
			return handler;
		}

		public static string ConvertBytesToHexString(byte[] bytes)
		{
			string sbytes = BitConverter.ToString(bytes);       // Convert to hyphen delimited string of hex characters
			return sbytes.Replace("-", "");
		}

		public static string ConvertBytesToHexString(byte[] buffer, int offset, int byteCount)
		{
			byte[] bytes = new byte[byteCount];
			Buffer.BlockCopy(buffer, offset, bytes, 0, byteCount);
			string sbytes = BitConverter.ToString(bytes);       // Convert to hyphen delimited string of hex characters
			return sbytes.Replace("-", "");
		}

		public static byte[] ConvertHexStringToBytes(string hexString)
		{
			// Convert Hex string to byte[]
			byte[] HexAsBytes = new byte[hexString.Length / 2];
			for (int index = 0; index < HexAsBytes.Length; index++)
			{
				string byteValue = hexString.Substring(index * 2, 2);
				HexAsBytes[index] = byte.Parse(byteValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
			}
			return HexAsBytes;
		}

		public static byte[] ConvertStringToBytes(string String)
		{
			// Convert string to byte[]
			byte[] AsBytes = new byte[String.Length];
			for (int index = 0; index < AsBytes.Length; index++)
			{
				AsBytes[index] = (byte)String[index];
			}
			return AsBytes;
		}

		//private static byte[] Transform( byte[] textBytes, ICryptoTransform transform )
		//{
		//	using (MemoryStream buf = new MemoryStream())
		//	{
		//		using (CryptoStream stream = new CryptoStream( buf, transform, CryptoStreamMode.Write ))
		//		{
		//			stream.Write( textBytes, 0, textBytes.Length );
		//			stream.FlushFinalBlock();
		//			return buf.ToArray();
		//		}
		//	}
		//}

		private static byte[] Transform(byte[] textBytes, int offset, int count, ICryptoTransform transform)
		{
			using (MemoryStream buf = new MemoryStream())
			{
				using (CryptoStream stream = new CryptoStream(buf, transform, CryptoStreamMode.Write))
				{
					stream.Write(textBytes, offset, count);
					stream.FlushFinalBlock();
					return buf.ToArray();
				}
			}
		}
		#endregion
	}
	public static class RSAKeyExtensions
	{
		#region JSON
		//internal static void FromJsonString(this RSA rsa, string jsonString)
		//{
		//    Check.Argument.IsNotEmpty(jsonString, nameof(jsonString));
		//    try
		//    {
		//        var paramsJson = JsonConvert.DeserializeObject<RSAParametersJson>(jsonString);

		//        RSAParameters parameters = new RSAParameters();

		//        parameters.Modulus = paramsJson.Modulus != null ? Convert.FromBase64String(paramsJson.Modulus) : null;
		//        parameters.Exponent = paramsJson.Exponent != null ? Convert.FromBase64String(paramsJson.Exponent) : null;
		//        parameters.P = paramsJson.P != null ? Convert.FromBase64String(paramsJson.P) : null;
		//        parameters.Q = paramsJson.Q != null ? Convert.FromBase64String(paramsJson.Q) : null;
		//        parameters.DP = paramsJson.DP != null ? Convert.FromBase64String(paramsJson.DP) : null;
		//        parameters.DQ = paramsJson.DQ != null ? Convert.FromBase64String(paramsJson.DQ) : null;
		//        parameters.InverseQ = paramsJson.InverseQ != null ? Convert.FromBase64String(paramsJson.InverseQ) : null;
		//        parameters.D = paramsJson.D != null ? Convert.FromBase64String(paramsJson.D) : null;
		//        rsa.ImportParameters(parameters);
		//    }
		//    catch
		//    {
		//        throw new Exception("Invalid JSON RSA key.");
		//    }
		//}

		//internal static string ToJsonString(this RSA rsa, bool includePrivateParameters)
		//{
		//    RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

		//    var parasJson = new RSAParametersJson()
		//    {
		//        Modulus = parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null!,
		//        Exponent = parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null!,
		//        P = parameters.P != null ? Convert.ToBase64String(parameters.P) : null!,
		//        Q = parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null!,
		//        DP = parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null!,
		//        DQ = parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null!,
		//        InverseQ = parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null!,
		//        D = parameters.D != null ? Convert.ToBase64String(parameters.D) : null
		//    };

		//    return JsonConvert.SerializeObject(parasJson);
		//}
		#endregion

		#region XML

		public static void FromXmlString2(this RSA rsa, string xmlString)
		{
			RSAParameters parameters = new RSAParameters();

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlString);

			if (xmlDoc.DocumentElement != null && xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
			{
				foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
				{
					switch (node.Name)
					{
						case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
						case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
						case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
						case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
						case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
						case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
						case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
						case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
					}
				}
			}
			else
			{
				throw new Exception("Invalid XML RSA key.");
			}

			rsa.ImportParameters(parameters);
		}

		public static string ToXmlString2(this RSA rsa, bool includePrivateParameters)
		{
			RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

			return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
				  parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null!,
				  parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null!,
				  parameters.P != null ? Convert.ToBase64String(parameters.P) : null!,
				  parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null!,
				  parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null!,
				  parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null!,
				  parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null!,
				  parameters.D != null ? Convert.ToBase64String(parameters.D) : null);
		}

		#endregion
	}
	//public class SecureApiCallResponse
	//{
	//	public string Response = null!;
	//	public HttpStatusCode Status = HttpStatusCode.BadRequest;
	//}
}
