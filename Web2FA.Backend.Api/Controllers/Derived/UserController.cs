using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web2FA.Backend.Api.Controllers.Base;
using Web2FA.Backend.Service.Interfaces.Derived;
using Web2FA.Backend.Shared.Payload.Derived;
using Web2FA.Backend.Shared.Services.Derived;

namespace Web2FA.Backend.Api.Controllers.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public class UserController : ApiBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginRequestPayload pLoginPayload)
        {
            var captchaItem = CacheService<CaptchaPayload>.DerivedObject.GetObject(pLoginPayload.CaptchaId);

            if (captchaItem == null || captchaItem.CaptchaResult != pLoginPayload.CaptchaResult)
            {
                if (captchaItem != null)
                {
                    CacheService<CaptchaPayload>.DerivedObject.DeleteObject(pLoginPayload.CaptchaId);
                }

                return BadRequest(new ResponsePayload<LoginResponsePayload, object>(false, "Hatalı captcha"));
            }

            var vResult = await userService.LoginUserAsync(pLoginPayload);

            if (captchaItem != null)
            {
                CacheService<CaptchaPayload>.DerivedObject.DeleteObject(pLoginPayload.CaptchaId);
            }

            return vResult.IsSuccess ? Ok(vResult) : BadRequest(vResult);
        }

        [HttpPost("LoginWithTFA")]
        [AllowAnonymous]
        public async Task<ActionResult> LoginWithTFA(LoginRequestPayload pLoginPayload)
        {
            var captchaItem = CacheService<CaptchaPayload>.DerivedObject.GetObject(pLoginPayload.CaptchaId);

            if (captchaItem == null || captchaItem.CaptchaResult != pLoginPayload.CaptchaResult)
            {
                if (captchaItem != null)
                {
                    CacheService<CaptchaPayload>.DerivedObject.DeleteObject(pLoginPayload.CaptchaId);
                }

                return BadRequest(new ResponsePayload<LoginResponsePayload, object>(false, "Hatalı captcha"));
            }

            var vResult = await userService.LoginUserAsync(pLoginPayload);

            if (captchaItem != null)
            {
                CacheService<CaptchaPayload>.DerivedObject.DeleteObject(pLoginPayload.CaptchaId);
            }

            return vResult.IsSuccess ? Ok(vResult) : BadRequest(vResult);
        }

        [HttpGet("GetCaptcha")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCaptcha()
        {
            var cResult = await userService.GetCaptchaData();

            if (cResult.IsSuccess && cResult.Data != null)
            {
                var cachedCaptcha = new CaptchaPayload
                {
                    CaptchaId = cResult.Data.CaptchaId,
                    CaptchaResult = cResult.Data.CaptchaResult,
                };

                CacheService<CaptchaPayload>.DerivedObject.SetObject(cachedCaptcha.CaptchaId, cachedCaptcha, 5);

                cResult.Data.CaptchaResult = 0;
            }

            return cResult.IsSuccess ? Ok(cResult) : BadRequest(cResult);
        }

        [HttpGet("ResetAuthenticator")]
        [Authorize]
        public async Task<ActionResult> ResetAuthenticator()
        {
            var userIdClaim = User.FindFirst(x => x.Type == "UserId")?.Value;

            if (userIdClaim == null || !long.TryParse(userIdClaim, out var userId))
            {
                return BadRequest();
            }

            var vResult = await userService.ResetAuthenticatorAsync(userId);

            return vResult.IsSuccess ? Ok(vResult) : BadRequest(vResult);
        }
    }
}
