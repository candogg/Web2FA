using OtpNet;
using Web2FA.Backend.Shared.Services.Base;

namespace Web2FA.Backend.Shared.Services.Derived
{
    public sealed class OtpService : ServiceSingularBase<OtpService>
    {
        public bool ConfirmOtpCode(string userKey, string code, int validSeconds = 60, int totpSize = 6)
        {
            try
            {
                var sBytes = Base32Encoding.ToBytes(userKey);

                var totp = new Totp(secretKey: sBytes, totpSize: totpSize, mode: OtpHashMode.Sha256, step: validSeconds);

                return totp.VerifyTotp(DateTime.Now, code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return false;
        }
    }
}
