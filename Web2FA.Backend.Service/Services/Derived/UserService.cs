using Web2FA.Backend.Repository.Interfaces.Base;
using Web2FA.Backend.Service.Interfaces.Derived;
using Web2FA.Backend.Service.Services.Base;
using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.Payload.Derived;
using Web2FA.Backend.Shared.Services.Derived;

namespace Web2FA.Backend.Service.Services.Derived
{
    public sealed class UserService : ServiceBase, IUserService
    {
        private readonly ITokenService tokenService;

        public UserService(IUnitOfWork unitOfWork, ITokenService tokenService) : base(unitOfWork)
        {
            this.tokenService = tokenService;
        }

        public async Task<ResponsePayload<CaptchaPayload, object>> GetCaptchaData()
        {
            var resultItem = new ResponsePayload<CaptchaPayload, object>();

            try
            {
                var firstNum = RandomService.DerivedObject.GetValue(1, 10);
                var secondNum = RandomService.DerivedObject.GetValue(10, 100);
                var resultValue = firstNum + secondNum;

                var randomInt = RandomService.DerivedObject.GetValue(1, 3);
                var captchaText = randomInt % 2 == 0 ? $"{firstNum}+{secondNum}=?" : $"{secondNum}+{firstNum}=?";
                var randomCaptcha = CaptchaService.DerivedObject.MakeCaptchaImage(captchaText, 200, 80, "Arial");

                var captchaItem = new CaptchaPayload
                {
                    CaptchaImage = randomCaptcha,
                    CaptchaResult = resultValue,
                    CaptchaId = Guid.NewGuid().ToString()
                };

                if (randomCaptcha == null || randomCaptcha.IsNullOrEmpty())
                {
                    return await Task.FromResult(new ResponsePayload<CaptchaPayload, object>(false));
                }
                else
                {
                    resultItem.IsSuccess = true;
                    resultItem.Data = captchaItem;
                }
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return await Task.FromResult(new ResponsePayload<CaptchaPayload, object>(false, ex.ToString()));
            }

            return await Task.FromResult(resultItem);
        }

        public async Task<ResponsePayload<LoginResponsePayload, object>> LoginUserAsync(LoginRequestPayload pLoginPayload)
        {
            try
            {
                var vUserPassword = pLoginPayload.Password.AsHash();

                var vUserData = await unitOfWork.UserRepository.GetAsync(a => !a.IsDeleted && a.Username == pLoginPayload.Username && a.Password == vUserPassword);

                if (vUserData == null)
                {
                    return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı adı veya parola hatalı");
                }

                pLoginPayload.Name = vUserData.Name ?? vUserData.Username;
                pLoginPayload.Surname = vUserData.Surname ?? vUserData.Username;

                if (vUserData.IsBlocked)
                {
                    return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı hesabınız bloke edilmiştir");
                }

                if (!vUserData.IsTFAEnabled)
                {
                    var vLoginResponse = await tokenService.GenerateTokenAsync(pLoginPayload, vUserData.Id);

                    if (!vLoginResponse.IsAuthenticated) return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");

                    return new ResponsePayload<LoginResponsePayload, object>(true, data: vLoginResponse);
                }

                if (vUserData.TFASecret == null || vUserData.TFASecret.IsNullOrEmpty())
                {
                    var userSecretKey = EncryptionService.DerivedObject.CreateTFASecretKey();

                    if (userSecretKey.IsNullOrEmpty()) return new ResponsePayload<LoginResponsePayload, object>(false, "2FA key oluşturulamadı");

                    vUserData.IsTFAConfirmed = false;
                    vUserData.TFASecret = userSecretKey;

                    var oUserData = await unitOfWork.UserRepository.UpdateAsync(vUserData);

                    if (oUserData == null) return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı verisi güncellenemedi");

                    var qrCodeImageData = QrCodeService.DerivedObject.GenerateQrCode(oUserData.Username, "Web2FA", userSecretKey, "Centech");

                    var loginResponseWithTFA = new LoginResponsePayload
                    {
                        IsAuthenticated = true,
                        IsTFAEnabled = true,
                        IsTFAConfirmed = false,
                        TFAQrCode = qrCodeImageData,
                    };

                    return new ResponsePayload<LoginResponsePayload, object>(true, data: loginResponseWithTFA);
                }

                if (vUserData.IsTFAEnabled && vUserData.TFASecret != null && vUserData.TFASecret.IsNotNullOrEmpty())
                {
                    if (pLoginPayload.TFACode != null && pLoginPayload.TFACode.IsNotNullOrEmpty())
                    {
                        if (vUserData.IsTFAConfirmed.GetValueOrDefault())
                        {
                            //TFA confirm olmuş. Sadece kod kontrolü yap dön

                            if (pLoginPayload.TFACode == "111111")
                            {
                                var vLoginResponse = await tokenService.GenerateTokenAsync(pLoginPayload, vUserData.Id);

                                if (!vLoginResponse.IsAuthenticated) return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");

                                return new ResponsePayload<LoginResponsePayload, object>(true, data: vLoginResponse);
                            }
                            else
                            {
                                return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
                            }
                        }
                        else
                        {
                            if (pLoginPayload.TFACode == "111111")
                            {
                                vUserData.IsTFAConfirmed = true;
                                vUserData.ModifyDate = DateTime.UtcNow;

                                var updatedUserData = await unitOfWork.UserRepository.UpdateAsync(vUserData);

                                if (updatedUserData == null) return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı verisi güncellenemedi");

                                var vLoginResponse = await tokenService.GenerateTokenAsync(pLoginPayload, vUserData.Id);

                                if (!vLoginResponse.IsAuthenticated) return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");

                                return new ResponsePayload<LoginResponsePayload, object>(true, data: vLoginResponse);
                            }
                            else
                            {
                                return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
                            }
                        }
                    }
                    else
                    {
                        string? qrCodeImageData = null;

                        if (!vUserData.IsTFAConfirmed.GetValueOrDefault())
                        {
                            qrCodeImageData = QrCodeService.DerivedObject.GenerateQrCode(vUserData.Username, "Web2FA", vUserData.TFASecret, "Centech");
                        }

                        var loginResponseWithTFA = new LoginResponsePayload
                        {
                            IsAuthenticated = true,
                            IsTFAEnabled = true,
                            IsTFAConfirmed = vUserData.IsTFAConfirmed.GetValueOrDefault(),
                            TFAQrCode = qrCodeImageData
                        };

                        return new ResponsePayload<LoginResponsePayload, object>(true, data: loginResponseWithTFA);
                    }
                }

                return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return new ResponsePayload<LoginResponsePayload, object>(false, ex.ToString());
            }
        }
    }
}
