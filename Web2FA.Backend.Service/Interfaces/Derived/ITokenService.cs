using Web2FA.Backend.Shared.Payload.Derived;

namespace Web2FA.Backend.Service.Interfaces.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public interface ITokenService
    {
        Task<LoginResponsePayload> GenerateTokenAsync(LoginRequestPayload pLoginPayload, long pUserId);
    }
}
