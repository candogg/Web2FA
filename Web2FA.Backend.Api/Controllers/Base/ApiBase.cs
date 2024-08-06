using Microsoft.AspNetCore.Mvc;

namespace Web2FA.Backend.Api.Controllers.Base
{
    [Route("api/Web2FA/[controller]")]
    [ApiController]
    public abstract class ApiBase : ControllerBase
    {
        protected async Task<string> GetIPAddressAsync()
        {
            var remoteAddr = string.Empty;

            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                remoteAddr = HttpContext.Connection.RemoteIpAddress.ToString();
            }

            return await Task.FromResult(remoteAddr);
        }
    }
}
