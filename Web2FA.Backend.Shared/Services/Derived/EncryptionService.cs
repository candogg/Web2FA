using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Web2FA.Backend.Shared.Constants;
using Web2FA.Backend.Shared.Services.Base;

namespace Web2FA.Backend.Shared.Services.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public sealed class EncryptionService : ServiceSingularBase<EncryptionService>
    {
        public string EncryptString(string input)
        {
            try
            {
                using var aesAlg = Aes.Create();

                aesAlg.Key = Encoding.UTF8.GetBytes(ApplicationConstants.ApplicationKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(ApplicationConstants.ApplicationIv);

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using var msEncrypt = new MemoryStream();
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(input);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return string.Empty;
        }

        public string DecryptString(string input)
        {
            try
            {
                using var aesAlg = Aes.Create();

                aesAlg.Key = Encoding.UTF8.GetBytes(ApplicationConstants.ApplicationKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(ApplicationConstants.ApplicationIv);

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using var msDecrypt = new MemoryStream(Convert.FromBase64String(input));
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);

                return srDecrypt.ReadToEnd();
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return string.Empty;
        }

        public string GetSignature(string input)
        {
            var signature = string.Empty;

            try
            {
                using var hSigner = new HMACSHA256(Encoding.UTF8.GetBytes(ApplicationConstants.ApplicationKey));

                var dataBytes = Encoding.UTF8.GetBytes(input);

                signature = BitConverter.ToString(hSigner.ComputeHash(dataBytes)).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return signature;
        }

        public string CreateTFASecretKey(int length = 32)
        {
            try
            {
                using var rng = RandomNumberGenerator.Create();

                var codeBytes = new byte[length];

                rng.GetBytes(codeBytes);

                return Base32Encode(codeBytes);
            }
            catch
            { }

            return string.Empty;
        }

        private string Base32Encode(byte[] pData)
        {
            var oBase32Char = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var oBitCount = 0;
            var oCurrentIndex = 0;
            var oBase32 = new StringBuilder();

            while (oCurrentIndex < pData.Length)
            {
                var oCurrentByte = pData[oCurrentIndex];
                var oNextByte = (oCurrentIndex < pData.Length - 1) ? pData[oCurrentIndex + 1] : 0;
                var oCombinedBytes = (oCurrentByte << 8) | oNextByte;
                var oBitsToConsume = Math.Min(5, 8 - oBitCount);
                var oIndex = (oCombinedBytes >> (8 - oBitCount - oBitsToConsume)) & 0x1F;

                oBase32.Append(oBase32Char[oIndex]);
                oBitCount += oBitsToConsume;

                if (oBitCount >= 5)
                {
                    oCurrentIndex++;
                    oBitCount -= 5;
                }
            }

            return oBase32.ToString();
        }
    }
}
