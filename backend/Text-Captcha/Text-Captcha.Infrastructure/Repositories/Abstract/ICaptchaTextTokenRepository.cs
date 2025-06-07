using Text_Captcha.Infrastructure.Entities;

namespace Text_Captcha.Infrastructure.Repositories.Abstract;

public interface ICaptchaTextTokenRepository<CaptchaTextToken>
{
    Task SaveTokenAsync(CaptchaTextToken token);
    Task<CaptchaTextToken> GetTokenByIdAsync(string tokenId);
    Task UpdateTokenAsync(CaptchaTextToken token);
}