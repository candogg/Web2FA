using Web2FA.Backend.Shared.Payload.Base;

namespace Web2FA.Backend.Shared.Payload.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public sealed class LoginRequestPayload : PayloadBase
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string CaptchaId { get; set; } = null!;
        public int CaptchaResult { get; set; }
        public string? TFACode { get; set; }
    }
}
