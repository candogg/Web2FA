using Web2FA.Backend.Model.Models.Base;

namespace Web2FA.Backend.Model.Models.Derived
{
    public partial class User : EntityBase
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
