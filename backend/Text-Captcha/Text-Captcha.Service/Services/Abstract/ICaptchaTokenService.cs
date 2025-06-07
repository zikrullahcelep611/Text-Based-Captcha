using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastucture.DTOs;

namespace Text_Captcha.Service.Services.Abstract;

public interface ICaptchaTokenService
{
    Task<CaptchaTokenResponse> GenerateTokenAsync(string ipAddress);
    Task<bool> VerifyCaptchaAnswerAsync(CaptchaRequestAnswerDTO captchaRequestAnswerDto);
}