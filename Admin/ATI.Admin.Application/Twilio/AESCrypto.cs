using ATI.Configuration;
using Newtonsoft.Json;
using RestSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using Org.BouncyCastle.Tls.Crypto;
using System.Web;

namespace ATI.Admin.Application
{
    /// <summary>
    ///
    /// AES Utility Class to generate XST security token
    ///
    /// 
    ///   string xst = AXCM.CM.Util.XAesUtility.GenerateSecurityTokenUrl(
    ///   "Username","AppKey","EncKey", "EncInitVector", true);
    ///
    /// </summary>
    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    /// <summary>
    /// This class provides utilities for symmetric key encryption using Rijndael/AES.
    ///
    /// </summary>
    public static class AESCrypto
    {
        /// <summary>
        /// AES block size is constant of 128 bits, or 16 bytes.
        /// </summary>
        public const int AesBlockSizeBits = 128;
        /// <summary>
        /// Default padding byte character for keys and init vectors that are too short.
        /// </summary>
        public const byte DefaultBytePadCharacter = 0x00;
        /// <summary>
        /// Industry standard default test init vector bytes.
        /// </summary>
        public static readonly byte[] DefaultInitVectorBytes = new byte[] {
         0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07, 0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F
      };
        /// <summary>
        /// Industry standard default test init vector string representation.
        /// </summary>
        public static readonly String DefaultInitVector =
           Encoding.UTF8.GetString(DefaultInitVectorBytes);
        /// <summary>
        /// Generate securyt token in XST/URL format for web service calls.
        /// </summary>
        /// <param name="pTokenContext"></param>
        /// <param name="pTokenAppId"></param>
        /// <param name="pTokenAppKey"></param>
        /// <param name="pTokenClient"></param>
        /// <param name="pEncryptionKey"></param>
        /// <param name="pEncryptionInitVector"></param>
        /// <param name="pUrlEncode"></param>
        /// <returns></returns>
        public static string GenerateSecurityTokenUrl(
           string pUsername,
           string pApiKey,
           string pEncryptionKey,
           string pEncryptionInitVector,
           bool pUrlEncode)
        {
            string tokenDataInput;
            string tokenDataEncoded;
            string tokenGenDT;
            tokenGenDT = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            tokenDataInput =
                 "Username=" + pUsername +
               "&ApiKey=" + pApiKey +
               "&GenDT=" + tokenGenDT +
               "";
            {
            }
            tokenDataEncoded = EncodeBase64(
               Encrypt(
                  tokenDataInput,
                  pEncryptionKey,
                  pEncryptionInitVector,
                  256,
                  System.Security.Cryptography.CipherMode.CBC,
                  System.Security.Cryptography.PaddingMode.PKCS7)
               );
            if (pUrlEncode)
            {
                tokenDataEncoded = tokenDataEncoded.Replace("+", "%2b");
                tokenDataEncoded = tokenDataEncoded.Replace("/", "%2f");
                tokenDataEncoded = tokenDataEncoded.Replace("=", "%3d");
            }
            return tokenDataEncoded;
        }
        /// <summary>
        /// Encrypt the input data using the specified encryption settings
        /// and return the result base64 encoded.
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static string EncryptToString(
           string pData,
           string pKey,
           string pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            return EncodeBase64(
               Encrypt(
                  pData,
                  pKey,
                  pInitVector,
                  pKeySizeBits,
                  pCipherMode,
                  pPaddingMode)
               );
        }
        /// <summary>
        /// Encrypt the input data using the specified encryption settings
        /// and return the result base64 encoded.
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <returns></returns>
        public static string EncryptToString(
           string pData,
           string pKey,
           string pInitVector)
        {
            return EncodeBase64(
               Encrypt(
                  pData,
                  pKey,
                  pInitVector,
                  256,
                  System.Security.Cryptography.CipherMode.CBC,
                  System.Security.Cryptography.PaddingMode.PKCS7)
               );
        }
        /// <summary>
        /// Encrypt the input data using the specified encryption settings
        /// and return the result base64 encoded.
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static string EncryptToString(
           string pData,
           byte[] pKey,
           byte[] pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            return EncodeBase64(
               Encrypt(
                  pData,
                  pKey,
                  pInitVector,
                  pKeySizeBits,
                  pCipherMode,
                  pPaddingMode)
            );
        }
        /// <summary>
        /// Encrypt the input data using the specified encryption settings.
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static byte[] Encrypt(
           string pData,
           string pKey,
           string pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            if (String.IsNullOrEmpty(pData))
            {
                return new byte[0];
            }
            byte[] keyBytes;
            byte[] vectorBytes;
            ParseKeyAndVector(pKey, pKeySizeBits, pInitVector, out keyBytes, out vectorBytes);
            return Encrypt(
               pData,
               keyBytes,
               vectorBytes,
               pKeySizeBits,
               pCipherMode,
               pPaddingMode);
        }
        /// <summary>
        /// Encrypt the input data using the specified encryption settings.
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static byte[] Encrypt(
           string pData,
           byte[] pKey,
           byte[] pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            if (String.IsNullOrEmpty(pData))
            {
                return new byte[0];
            }
            return Encrypt(
               DecodeUTF8(pData),
               pKey,
               pInitVector,
               pKeySizeBits,
               pCipherMode,
               pPaddingMode);
        }
        /// <summary>
        /// Encrypt the input data using the specified encryption settings.
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static byte[] Encrypt(
           byte[] pData,
           byte[] pKey,
           byte[] pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            if (pData.Length < 1)
            {
                return new byte[0];
            }
            // Convert our plaintext into a byte array.
            // Assume that plaintext contains UTF8-encoded characters.
            byte[] dataBytes = pData;
            // Create uninitialized Rijndael encryption object.
            using (RijndaelManaged cipher = new RijndaelManaged())
            {
                cipher.Mode = pCipherMode;
                cipher.KeySize = pKeySizeBits;
                cipher.BlockSize = AesBlockSizeBits;
                cipher.Padding = pPaddingMode;
                // Generate encryptor from the existing key bytes and initialization vector.
                try
                {
                    using (ICryptoTransform encryptor = cipher.CreateEncryptor(pKey, pInitVector))
                    {
                        // Define cryptographic stream (always use Write mode for encryption).
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream =
                               new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                // Start encrypting.
                                cryptoStream.Write(dataBytes, 0, dataBytes.Length);
                                // Finish encrypting.
                                cryptoStream.FlushFinalBlock();
                                // Convert our encrypted data from a memory stream into a byte array, then base64.
                                return memoryStream.ToArray();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(
                      "Cipher.CreateEncryptorError|"
                    + "KeySize=" + cipher.KeySize + "|"
                    + "BlockSize=" + cipher.BlockSize + "|"
                    + "CipherMode=" + cipher.Mode + "|"
                    + "PaddingMode=" + cipher.Padding + "|"
                    + "KeyLength=" + pKey.Length + "|"
                    + "InitVectorLength=" + pInitVector.Length + "|"
                    , e);
                }
            }
        }
        /// <summary>
        /// Decrypt the base64 encoded input data using the specified encryption settings
        /// and return the result as a UTF8 string.
        /// </summary>
        /// <param name="pBase64Data"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static string DecryptToString(
           string pBase64Data,
           string pKey,
           string pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            return EncodeUTF8(
               Decrypt(
                  pBase64Data,
                  pKey,
                  pInitVector,
                  pKeySizeBits,
                  pCipherMode,
                  pPaddingMode)
               );
        }
        /// <summary>
        /// Decrypt the base64 encoded input data using the specified encryption settings
        /// and return the result as a UTF8 string.
        /// </summary>
        /// <param name="pBase64Data"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static string DecryptToString(
           string pBase64Data,
           byte[] pKey,
           byte[] pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            return EncodeUTF8(
               Decrypt(
                  pBase64Data,
                  pKey,
                  pInitVector,
                  pKeySizeBits,
                  pCipherMode,
                  pPaddingMode)
               );
        }
        /// <summary>
        /// Decrypt the base64 encoded input data using the specified encryption settings.
        /// </summary>
        /// <param name="pBase64Data"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static byte[] Decrypt(
           string pBase64Data,
           string pKey,
           string pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            if (String.IsNullOrEmpty(pBase64Data))
            {
                return new byte[0];
            }
            byte[] keyBytes;
            byte[] vectorBytes;
            ParseKeyAndVector(pKey, pKeySizeBits, pInitVector, out keyBytes, out vectorBytes);
            return Decrypt(
               DecodeBase64(pBase64Data),
               keyBytes,
               vectorBytes,
               pKeySizeBits,
               pCipherMode,
               pPaddingMode);
        }
        /// <summary>
        /// Decrypt the base64 encoded input data using the specified encryption settings.
        /// </summary>
        /// <param name="pBase64Data"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static byte[] Decrypt(
           string pBase64Data,
           byte[] pKey,
           byte[] pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            if (String.IsNullOrEmpty(pBase64Data))
            {
                return new byte[0];
            }
            return Decrypt(
               DecodeBase64(pBase64Data),
               pKey,
               pInitVector,
               pKeySizeBits,
               pCipherMode,
               pPaddingMode);
        }
        /// <summary>
        /// Decrypt the input data using the specified encryption settings.
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pKey"></param>
        /// <param name="pInitVector"></param>
        /// <param name="pKeySizeBits"></param>
        /// <param name="pCipherMode"></param>
        /// <param name="pPaddingMode"></param>
        /// <returns></returns>
        public static byte[] Decrypt(
           byte[] pData,
           byte[] pKey,
           byte[] pInitVector,
           int pKeySizeBits,
           CipherMode pCipherMode,
           PaddingMode pPaddingMode)
        {
            if (pData.Length < 1)
            {
                return new byte[0];
            }
            // Convert our encrypted data into a byte array.
            byte[] cipherTextBytes = pData;
            // Create uninitialized Rijndael encryption object.
            using (RijndaelManaged cipher = new RijndaelManaged())
            {
                cipher.Mode = pCipherMode;
                cipher.KeySize = pKeySizeBits;
                cipher.BlockSize = AesBlockSizeBits;
                cipher.Padding = pPaddingMode;
                try
                {
                    // Generate decryptor from the existing key bytes and initialization
                    // vector. Key size will be defined based on the number of the key bytes.
                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(pKey, pInitVector))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            // Define cryptographic stream (always use Read mode for encryption).
                            using (CryptoStream cryptoStream =
                               new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                // Since at this point we don't know what the size of decrypted data
                                // will be, allocate the buffer long enough to hold ciphertext;
                                // plaintext is never longer than ciphertext.
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                // Start decrypting.
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                // create and return the resulting array
                                byte[] resultBytes = new byte[decryptedByteCount];
                                Array.Copy(plainTextBytes, resultBytes, decryptedByteCount);
                                return resultBytes;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(
                      "Cipher.CreateEncryptorError|"
                    + "KeySize=" + cipher.KeySize + "|"
                    + "BlockSize=" + cipher.BlockSize + "|"
                    + "CipherMode=" + cipher.Mode + "|"
                    + "PaddingMode=" + cipher.Padding + "|"
                    + "KeyLength=" + pKey.Length + "|"
                    + "InitVectorLength=" + pInitVector.Length + "|"
                    , e);
                }
            }
        }
        /// <summary>
        /// Get the UTF8 string representation of the data.
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public static string EncodeUTF8(byte[] pData)
        {
            if (pData.Length == 0)
            {
                return String.Empty;
            }
            return Encoding.UTF8.GetString(pData, 0, pData.Length);
        }
        /// <summary>
        /// Decode the UTF8 string to bytes.
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public static byte[] DecodeUTF8(string pData)
        {
            if (String.IsNullOrEmpty(pData))
            {
                return new byte[0];
            }
            return Encoding.UTF8.GetBytes(pData);
        }
        /// <summary>
        /// Baes64 encode the data.
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public static string EncodeBase64(byte[] pData)
        {
            if (pData.Length == 0)
            {
                return String.Empty;
            }
            return Convert.ToBase64String(pData);
        }
        /// <summary>
        /// Decode the Base64 encoded string.
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public static byte[] DecodeBase64(string pData)
        {
            if (String.IsNullOrEmpty(pData))
            {
                return new byte[0];
            }
            return Convert.FromBase64String(pData);
        }
        /// <summary>
        /// Parses the cipher mode into it's enum equivalent.
        /// </summary>
        /// <param name="pMode"></param>
        /// <returns></returns>
        public static CipherMode ParseCipherMode(string pMode)
        {
            return (CipherMode)Enum.Parse(typeof(CipherMode), pMode, true);
        }
        /// <summary>
        /// Parses the padding mode into it's enum equivalent.
        /// </summary>
        /// <param name="pMode"></param>
        /// <returns></returns>
        public static PaddingMode ParsePaddingMode(string pMode)
        {
            return (PaddingMode)Enum.Parse(typeof(PaddingMode), pMode, true);
        }
        /// <summary>
        /// Parse the key size in bits to a supported AES value:  128, 192, 256.
        /// </summary>
        /// <param name="pKeySizeBits"></param>
        /// <returns></returns>
        public static int ParseKeySize(int pKeySizeBits)
        {
            if (pKeySizeBits <= 128)
            {
                return 128;
            }
            if (pKeySizeBits <= 192)
            {
                return 192;
            }
            return 256;
        }
        /// <summary>
        /// Parse and normalize the key and vector to their byte equivalents.
        /// </summary>
        /// <param name="pInputKey"></param>
        /// <param name="pInputInitVector"></param>
        /// <param name="pInputKeySizeBits"></param>
        /// <param name="pOutputKeyBytes"></param>
        /// <param name="pOutputInitVectorBytes"></param>
        public static void ParseKeyAndVector(
           string pInputKey,
           int pInputKeySizeBits,
           string pInputInitVector,
           out byte[] pOutputKeyBytes,
           out byte[] pOutputInitVectorBytes)
        {
            string tmp;
            ParseKey(pInputKey, pInputKeySizeBits, out tmp, out pOutputKeyBytes);
            ParseInitVector(pInputInitVector, out tmp, out pOutputInitVectorBytes);
        }
        /// <summary>
        /// Creates the corresponding byte array for the input key, truncating it,
        /// or padding it with null characters (0x00) based on the input key size in bits.
        /// </summary>
        /// <param name="pInputKey"></param>
        /// <param name="pInputKeySizeBits">key size in bits</param>
        /// <param name="pOutputKey"></param>
        /// <param name="pInputKeySize">key size in bits</param>
        /// <returns></returns>
        public static void ParseKey(
           string pInputKey,
           int pInputKeySizeBits,
           out string pOutputKey,
           out byte[] pOutputKeyBytes)
        {
            // working variables
            byte[] tempBytes;
            int tempBytesSize = 0;
            byte[] finalBytes;
            int finalBytesRequiredLength = 0;
            // Password Key Config
            finalBytesRequiredLength = pInputKeySizeBits / 8;
            string key = pInputKey + String.Empty;
            if (key.Length > finalBytesRequiredLength)
            {
                key = key.Substring(0, finalBytesRequiredLength);
            }
            // Password Bytes Config
            finalBytes = new byte[finalBytesRequiredLength];
            tempBytes = Encoding.UTF8.GetBytes(key);
            tempBytesSize = tempBytes.Length;
            for (int i = 0; i < tempBytesSize; i++)
            {
                finalBytes[i] = tempBytes[i];
            }
            if (tempBytesSize < finalBytesRequiredLength)
            {
                for (int i = tempBytesSize; i < finalBytesRequiredLength; i++)
                {
                    finalBytes[i] = DefaultBytePadCharacter;
                }
            }
            pOutputKey = key;
            pOutputKeyBytes = finalBytes;
        }
        /// <summary>
        /// Parses and normalizes the init vector. For AES encryption, the
        /// init vector is always 128 bits, or 16 bytes. If the passed in
        /// init vector is empty, the DefaultInitVector will be used. If
        /// the passed in init vector is too long, it will get truncated.
        /// If too short, it will be padded with the null byte character
        /// (0x00).
        /// </summary>
        /// <param name="pInputInitVector"></param>
        /// <param name="pOutputInitVector"></param>
        /// <param name="pOutputInitVectorBytes"></param>
        public static void ParseInitVector(
           string pInputInitVector,
           out string pOutputInitVector,
           out byte[] pOutputInitVectorBytes)
        {
            // working variables
            byte[] tempBytes;
            int tempBytesSize = 0;
            byte[] finalBytes;
            int finalBytesRequiredLength = 0;
            // Init Vector Config
            // NOTE: fixed size for AES.
            finalBytesRequiredLength = AesBlockSizeBits / 8;
            string initVector = pInputInitVector + String.Empty;
            if (String.IsNullOrEmpty(initVector))
            {
                initVector = DefaultInitVector;
                finalBytes = new byte[DefaultInitVectorBytes.Length];
                DefaultInitVectorBytes.CopyTo(finalBytes, 0);
            }
            else
            {
                // init vector config
                if (initVector.Length > finalBytesRequiredLength)
                {
                    initVector = initVector.Substring(0, finalBytesRequiredLength);
                }
                // init vector bytes config
                finalBytes = new byte[finalBytesRequiredLength];
                tempBytes = Encoding.UTF8.GetBytes(initVector);
                tempBytesSize = tempBytes.Length;
                for (int i = 0; i < tempBytesSize; i++)
                {
                    finalBytes[i] = tempBytes[i];
                }
                if (tempBytesSize < finalBytesRequiredLength)
                {
                    for (int i = tempBytesSize; i < finalBytesRequiredLength; i++)
                    {
                        finalBytes[i] = DefaultBytePadCharacter;
                    }
                }
            }
            pOutputInitVector = initVector;
            pOutputInitVectorBytes = finalBytes;
        }

    }
}
