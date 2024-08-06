using QRCoder;
using Web2FA.Backend.Shared.Services.Base;

namespace Web2FA.Backend.Shared.Services.Derived
{
    public class QrCodeService : ServiceSingularBase<QrCodeService>
    {
        public string GenerateQrCode(string userName, string appName, string secretKey, string issuer)
        {
            var authUrl = $"otpauth://totp/{appName}\\{userName}?secret={secretKey}&issuer={issuer}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(authUrl, QRCodeGenerator.ECCLevel.L);
            using var qrCode = new PngByteQRCode(qrCodeData);

            try
            {
                var qrCodeImageData = qrCode.GetGraphic(10);

                return Convert.ToBase64String(qrCodeImageData);
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return string.Empty;
        }
    }
}
