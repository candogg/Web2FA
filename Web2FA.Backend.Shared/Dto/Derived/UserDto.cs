using Web2FA.Backend.Shared.Dto.Base;

namespace Web2FA.Backend.Shared.Dto.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public sealed class UserDto : DtoBase
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsTFAEnabled { get; set; }
        public bool? IsTFAConfirmed { get; set; }
        public string? TFASecret { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntil { get; set; }
    }
}
