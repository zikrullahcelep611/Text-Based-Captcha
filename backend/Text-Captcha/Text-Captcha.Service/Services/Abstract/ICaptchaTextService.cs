using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastucture.DTOs;

namespace Text_Captcha.Service.Services.Abstract;

public interface ICaptchaTextService
{
    Task<CaptchaTextResponseDTO> CreateCaptchaText(CreateCaptchaTextDTO model);
    Task<CaptchaTextResponseDTO> GetCaptchaTextWithIdAsync(int captchaTextId);
    Task<CaptchaText> GetRandomCaptchaTextAsync();
    Task<List<string>> GetCaptchaAnswerWithCaptchaId(int captchaTextId);
}