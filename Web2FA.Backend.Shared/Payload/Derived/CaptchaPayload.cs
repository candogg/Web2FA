using Web2FA.Backend.Shared.Payload.Base;

namespace Web2FA.Backend.Shared.Payload.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public sealed class CaptchaPayload : PayloadBase
    {
        public string? CaptchaImage { get; set; }
        public string CaptchaId { get; set; } = null!;
        public int CaptchaResult { get; set; }
    }
}
