using Web2FA.Backend.Shared.Payload.Derived;

namespace Web2FA.Backend.Service.Interfaces.Derived
{
    public interface ITokenService
    {
        Task<LoginResponsePayload> GenerateTokenAsync(LoginRequestPayload pLoginPayload, long pUserId);
    }
}
