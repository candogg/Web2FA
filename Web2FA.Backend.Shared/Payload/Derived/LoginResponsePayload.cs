using Web2FA.Backend.Shared.Payload.Base;

namespace Web2FA.Backend.Shared.Payload.Derived
{
    public sealed class LoginResponsePayload : PayloadBase
    {
        public bool IsAuthenticated { get; set; }
        public bool IsTFAEnabled { get; set; }
        public bool IsTFAConfirmed { get; set; }
        public string? TFAQrCode { get; set; }
        public string? AuthenticationToken { get; set; }
        public DateTime? TokenExpireDate { get; set; }
    }
}
