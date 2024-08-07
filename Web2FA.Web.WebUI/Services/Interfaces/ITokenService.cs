using Web2FA.Backend.Shared.Payload.Derived;

namespace Web2FA.Web.WebUI.Services.Interfaces
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public interface ITokenService
    {
        Task<string> ControlTokenAsync();
        Task<TokenPayload?> GetClaimAsync();
        Task RemoveTokenAsync();
    }
}
