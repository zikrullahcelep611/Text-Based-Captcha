using Text_Captcha.Infrastructure.Entities;

namespace Text_Captcha.Infrastructure.Repositories.Abstract;

public interface ICaptchaTokenRepository<CaptchaToken> 
{
    Task SaveTokenAsync(CaptchaToken token);
    Task<CaptchaToken> GetTokenByIdAsync(string tokenId);
    Task UpdateTokenAsync(CaptchaToken token);
}