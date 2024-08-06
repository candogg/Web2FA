using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web2FA.Backend.Repository.Interfaces.Base;
using Web2FA.Backend.Service.Interfaces.Derived;
using Web2FA.Backend.Service.Services.Base;
using Web2FA.Backend.Shared.Constants;
using Web2FA.Backend.Shared.Payload.Derived;
using Web2FA.Backend.Shared.Services.Derived;

namespace Web2FA.Backend.Service.Services.Derived
{
    public sealed class TokenService : ServiceBase, ITokenService
    {
        public TokenService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public async Task<LoginResponsePayload> GenerateTokenAsync(LoginRequestPayload pLoginPayload, long pUserId)
        {
            try
            {
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ApplicationConstants.ApplicationKey));

                var createDate = DateTime.UtcNow;
                var expireDate = createDate.AddDays(15);

                var claims = new List<Claim>
                {
                    new("Username", pLoginPayload.Username),
                    new("Name", pLoginPayload.Name ?? string.Empty),
                    new("Surname", pLoginPayload.Surname ?? string.Empty),
                    new("UserId", pUserId.ToString())
                };

                var jwt = new JwtSecurityToken(
                        issuer: "Centech Bilişim",
                        audience: "Web2FA",
                        claims: claims,
                        notBefore: createDate,
                        expires: expireDate,
                        signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
                    );

                return await Task.FromResult(new LoginResponsePayload
                {
                    AuthenticationToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                    TokenExpireDate = expireDate,
                    IsAuthenticated = true
                });
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return await Task.FromResult(new LoginResponsePayload());
        }
    }
}
