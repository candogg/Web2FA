namespace Web2FA.Backend.Shared.Payload.Derived
{
    public class TokenPayload
    {
        public string UserId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Roles { get; set; } = null!;
    }
}
