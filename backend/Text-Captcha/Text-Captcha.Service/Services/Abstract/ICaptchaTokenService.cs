using Text_Captcha.Infrastructure.Entities;

namespace Text_Captcha.Service.Services.Abstract;

public interface ICaptchaTokenService
{
    Task<CaptchaTokenResponse> GenerateTokenAsync(string ipAddress);
}