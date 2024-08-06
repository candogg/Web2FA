using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.Payload.Derived;
using Web2FA.Web.WebUI.Services.Interfaces;

namespace Web2FA.Web.WebUI.Services.Derived
{
    public sealed class TokenService : ITokenService
    {
        private readonly ILocalStorageService localStorageService;

        public TokenService(ILocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;
        }

        public async Task<string> ControlTokenAsync()
        {
            var loginStorageData = await localStorageService.GetItemAsync<LoginResponsePayload>("Web2FAData");

            if (loginStorageData == null || !loginStorageData.IsAuthenticated || loginStorageData.TokenExpireDate < DateTime.UtcNow || loginStorageData.AuthenticationToken == null || loginStorageData.AuthenticationToken.IsNullOrEmpty())
            {
                await localStorageService.RemoveItemAsync("MedLOGData");

                return string.Empty;
            }

            return loginStorageData.AuthenticationToken;
        }

        public async Task RemoveTokenAsync()
        {
            var loginStorageData = await localStorageService.GetItemAsync<LoginResponsePayload>("Web2FAData");

            if (loginStorageData != null)
            {
                await localStorageService.RemoveItemAsync("Web2FAData");
            }
        }

        public async Task<TokenPayload?> GetClaimAsync()
        {
            var loginStorageData = await localStorageService.GetItemAsync<LoginResponsePayload>("Web2FAData");

            if (loginStorageData == null) return await Task.FromResult<TokenPayload?>(null);

            var tokenHandler = new JwtSecurityTokenHandler();
            var payload = tokenHandler.ReadJwtToken(loginStorageData.AuthenticationToken).Payload;
            var jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            var tokenInfo = JsonSerializer.Deserialize<TokenPayload>(jsonPayload);

            return await Task.FromResult(tokenInfo ?? new TokenPayload());
        }
    }
}
