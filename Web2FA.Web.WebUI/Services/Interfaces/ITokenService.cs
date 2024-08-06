using Web2FA.Backend.Shared.Payload.Derived;

namespace Web2FA.Web.WebUI.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> ControlTokenAsync();
        Task<TokenPayload?> GetClaimAsync();
        Task RemoveTokenAsync();
    }
}
