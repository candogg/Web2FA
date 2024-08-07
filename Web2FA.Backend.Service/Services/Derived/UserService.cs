using System.Transactions;
using Web2FA.Backend.Repository.Interfaces.Base;
using Web2FA.Backend.Service.Interfaces.Derived;
using Web2FA.Backend.Service.Services.Base;
using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.Payload.Derived;
using Web2FA.Backend.Shared.Services.Derived;

namespace Web2FA.Backend.Service.Services.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
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
            using TransactionScope transaction = new(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted,
            }, TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var vUserPassword = pLoginPayload.Password.AsHash();

                var vUserData = await unitOfWork.UserRepository.GetAsync(a => !a.IsDeleted && a.Username == pLoginPayload.Username && a.Password == vUserPassword);

                if (vUserData == null)
                {
                    transaction.Dispose();

                    return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı adı veya parola hatalı");
                }

                pLoginPayload.Name = vUserData.Name ?? vUserData.Username;
                pLoginPayload.Surname = vUserData.Surname ?? vUserData.Username;

                //Kullanıcı bloklanmış ise hiç işlem yapmadan geri döner

                if (vUserData.IsBlocked)
                {
                    transaction.Dispose();

                    return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı hesabınız bloke edilmiştir");
                }

                //Kullanıcı için tfa doğrulama aktif değilse sadece token oluşturup döner

                if (!vUserData.IsTFAEnabled)
                {
                    var vLoginResponse = await tokenService.GenerateTokenAsync(pLoginPayload, vUserData.Id);

                    if (!vLoginResponse.IsAuthenticated)
                    {
                        transaction.Dispose();

                        return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
                    }

                    transaction.Complete();

                    return new ResponsePayload<LoginResponsePayload, object>(true, data: vLoginResponse);
                }

                //Kullanıcı için tfa aktif fakat secret key yoksa oluşturup ilk kayıt için qr kod oluşturur

                if (vUserData.TFASecret == null || vUserData.TFASecret.IsNullOrEmpty())
                {
                    var userSecretKey = EncryptionService.DerivedObject.CreateTFASecretKey();

                    if (userSecretKey.IsNullOrEmpty())
                    {
                        transaction.Dispose();

                        return new ResponsePayload<LoginResponsePayload, object>(false, "2FA key oluşturulamadı");
                    }

                    vUserData.IsTFAConfirmed = false;
                    vUserData.TFASecret = userSecretKey;

                    var oUserData = await unitOfWork.UserRepository.UpdateAsync(vUserData);

                    if (oUserData == null)
                    {
                        transaction.Dispose();

                        return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı verisi güncellenemedi");
                    }

                    var qrCodeImageData = QrCodeService.DerivedObject.GenerateQrCode(oUserData.Username, "Web2FA", userSecretKey, "Centech");

                    if (qrCodeImageData == null || qrCodeImageData.IsNullOrEmpty())
                    {
                        transaction.Dispose();

                        return new ResponsePayload<LoginResponsePayload, object>(false, "QR kod oluşturulamadı");
                    }

                    var loginResponseWithTFA = new LoginResponsePayload
                    {
                        IsAuthenticated = true,
                        IsTFAEnabled = true,
                        IsTFAConfirmed = false,
                        TFAQrCode = qrCodeImageData,
                    };

                    transaction.Complete();

                    return new ResponsePayload<LoginResponsePayload, object>(true, data: loginResponseWithTFA);
                }

                //Kullanıcı için tfa aktif ve secret key varsa bu doğrulama adımına girer

                if (vUserData.IsTFAEnabled && vUserData.TFASecret != null && vUserData.TFASecret.IsNotNullOrEmpty())
                {
                    //Kullanıcı tarafından gelen payload'da tfa kodu varsa doğrulama adımına geçer

                    if (pLoginPayload.TFACode != null && pLoginPayload.TFACode.IsNotNullOrEmpty())
                    {
                        //Kullanıcı tarafından gelen payload'da tfa kodu varsa ve kullanıcı QR kodu önceden doğrulanmışsa bu aşamaya girer

                        if (vUserData.IsTFAConfirmed.GetValueOrDefault())
                        {
                            //TFA confirm olmuş. Sadece kod kontrolü yap dön

                            if (OtpService.DerivedObject.ConfirmOtpCode(vUserData.TFASecret, pLoginPayload.TFACode))
                            {
                                var vLoginResponse = await tokenService.GenerateTokenAsync(pLoginPayload, vUserData.Id);

                                if (!vLoginResponse.IsAuthenticated)
                                {
                                    transaction.Dispose();

                                    return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
                                }

                                transaction.Complete();

                                return new ResponsePayload<LoginResponsePayload, object>(true, data: vLoginResponse);
                            }
                            else
                            {
                                transaction.Dispose();

                                return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
                            }
                        }
                        else
                        {
                            //Kullanıcı tarafından gelen payload'da tfa kodu varsa ve kullanıcı QR kodu önceden doğrulanmamışsa bu aşamaya girer ve doğrulama yapar

                            if (OtpService.DerivedObject.ConfirmOtpCode(vUserData.TFASecret, pLoginPayload.TFACode))
                            {
                                vUserData.IsTFAConfirmed = true;
                                vUserData.ModifyDate = DateTime.UtcNow;

                                var updatedUserData = await unitOfWork.UserRepository.UpdateAsync(vUserData);

                                if (updatedUserData == null)
                                {
                                    transaction.Dispose();

                                    return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı verisi güncellenemedi");
                                }

                                var vLoginResponse = await tokenService.GenerateTokenAsync(pLoginPayload, vUserData.Id);

                                if (!vLoginResponse.IsAuthenticated)
                                {
                                    transaction.Dispose();

                                    return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
                                }

                                transaction.Complete();

                                return new ResponsePayload<LoginResponsePayload, object>(true, data: vLoginResponse);
                            }
                            else
                            {
                                transaction.Dispose();

                                return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
                            }
                        }
                    }
                    else
                    {
                        //Kullanıcı için tfa aktif ve secret key yoksa bu doğrulama adımına girer ve secret key ile QR kod oluşturur

                        string? qrCodeImageData = null;

                        if (!vUserData.IsTFAConfirmed.GetValueOrDefault())
                        {
                            var userSecretKey = EncryptionService.DerivedObject.CreateTFASecretKey();

                            if (userSecretKey.IsNullOrEmpty())
                            {
                                transaction.Dispose();

                                return new ResponsePayload<LoginResponsePayload, object>(false, "2FA key oluşturulamadı");
                            }

                            vUserData.IsTFAConfirmed = false;
                            vUserData.TFASecret = userSecretKey;

                            var oUserData = await unitOfWork.UserRepository.UpdateAsync(vUserData);

                            if (oUserData == null)
                            {
                                transaction.Dispose();

                                return new ResponsePayload<LoginResponsePayload, object>(false, "Kullanıcı verisi güncellenemedi");
                            }

                            qrCodeImageData = QrCodeService.DerivedObject.GenerateQrCode(vUserData.Username, "Web2FA", vUserData.TFASecret, "Centech Bilişim");

                            if (qrCodeImageData == null || qrCodeImageData.IsNullOrEmpty())
                            {
                                transaction.Dispose();

                                return new ResponsePayload<LoginResponsePayload, object>(false, "QR kod oluşturulamadı");
                            }
                        }

                        var loginResponseWithTFA = new LoginResponsePayload
                        {
                            IsAuthenticated = true,
                            IsTFAEnabled = true,
                            IsTFAConfirmed = vUserData.IsTFAConfirmed.GetValueOrDefault(),
                            TFAQrCode = qrCodeImageData
                        };

                        transaction.Complete();

                        return new ResponsePayload<LoginResponsePayload, object>(true, data: loginResponseWithTFA);
                    }
                }

                transaction.Dispose();

                return new ResponsePayload<LoginResponsePayload, object>(false, "Giriş başarısız");
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                transaction.Dispose();

                return new ResponsePayload<LoginResponsePayload, object>(false, ex.ToString());
            }
        }

        public async Task<ResponsePayload<object, object>> ResetAuthenticatorAsync(long userId)
        {
            using TransactionScope transaction = new(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted,
            }, TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var vUserData = await unitOfWork.UserRepository.GetAsync(a => !a.IsDeleted && a.Id == userId);

                if (vUserData == null)
                {
                    transaction.Dispose();

                    return new ResponsePayload<object, object>(false, "Kullanıcı bulunamadı");
                }

                vUserData.IsTFAConfirmed = false;
                vUserData.TFASecret = null;

                var oUserData = await unitOfWork.UserRepository.UpdateAsync(vUserData);

                if (oUserData == null)
                {
                    transaction.Dispose();

                    return new ResponsePayload<object, object>(false, "Kullanıcı verisi güncellenemedi");
                }

                transaction.Complete();

                return new ResponsePayload<object, object>(true);
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                transaction.Dispose();

                return new ResponsePayload<object, object>(false, ex.ToString());
            }
        }
    }
}
