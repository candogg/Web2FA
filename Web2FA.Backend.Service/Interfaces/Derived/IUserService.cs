using Web2FA.Backend.Shared.Payload.Derived;

namespace Web2FA.Backend.Service.Interfaces.Derived
{
    public interface IUserService
    {
        Task<ResponsePayload<LoginResponsePayload, object>> LoginUserAsync(LoginRequestPayload pLoginPayload);
        Task<ResponsePayload<CaptchaPayload, object>> GetCaptchaData();
    }
}
