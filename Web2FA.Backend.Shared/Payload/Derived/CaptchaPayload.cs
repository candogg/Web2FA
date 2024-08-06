using Web2FA.Backend.Shared.Payload.Base;

namespace Web2FA.Backend.Shared.Payload.Derived
{
    public sealed class CaptchaPayload : PayloadBase
    {
        public string? CaptchaImage { get; set; }
        public string CaptchaId { get; set; } = null!;
        public int CaptchaResult { get; set; }
    }
}
