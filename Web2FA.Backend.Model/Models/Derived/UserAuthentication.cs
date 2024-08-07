using Web2FA.Backend.Model.Models.Base;

namespace Web2FA.Backend.Model.Models.Derived
{
    public partial class UserAuthentication : EntityBase
    {
        public long UserId { get; set; }
        public string IpAddress { get; set; } = null!;
        public bool? IsAuthenticated { get; set; }

        public virtual User? User { get; set; }
    }
}
