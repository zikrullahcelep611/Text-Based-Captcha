using Text_Captcha.Infrastucture.DTOs;

namespace Text_Captcha.Service.Services.Abstract;

public interface ICaptchaTextTokenService
{
    Task<CaptchaTokenResponseDTO> GenerateTokenAsync(string ipAddress);
    Task<bool> VerifyCaptchaAnswerAsync(CaptchaTextRequestAnswerDTO model);
}