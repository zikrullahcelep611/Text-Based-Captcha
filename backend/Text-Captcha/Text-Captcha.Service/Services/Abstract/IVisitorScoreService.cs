using Text_Captcha.Infrastructure.Entities;

namespace Text_Captcha.Service.Services.Abstract;

public interface IVisitorScoreService
{
    Task<VisitorScore> GetOrCreateScoreAsync(string ipAddress);
    Task UpdateScoreOnSuccessAsync(string ipAddress);
    Task UpdateScoreOnFailureAsync(string ipAddress);
    Task UpdateActivityTimeAsync(string ipAddress);
    Task<bool> NeedsCaptchaAsync(string ipAddress);
    Task<CaptchaExemptionLevel> GetExemptionLevelAsync(string ipAddress);
}