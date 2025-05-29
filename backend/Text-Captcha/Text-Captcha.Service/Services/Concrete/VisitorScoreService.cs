using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastucture.Helpers;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.Service.Services.Concrete;

public class VisitorScoreService : IVisitorScoreService
{
    private readonly IRepository<VisitorScore> _repository;
    private readonly IConfiguration _configuration;
    private const int INITIAL_SCORE = 50;
    private const int SUCCESS_POINTS = 10;
    private const int FAILURE_POINTS = -15;
    private const int ACTIVITY_POINTS = 1;
    private const int MAX_SCORE = 100;

    public VisitorScoreService(IRepository<VisitorScore> repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<VisitorScore> GetOrCreateScoreAsync(string ipAddress)
    {
        var score = await _repository.GetAsync(x => x.IpAddress == ipAddress);

        if (score == null)
        {
            score = new VisitorScore
            {
                IpAddress = ipAddress,
                Score = INITIAL_SCORE,
                LastActivityTime = DateTime.UtcNow,
                LastCaptchaAttempt = DateTime.UtcNow,
                ExemptionEndDate = DateTime.UtcNow
            };
            await _repository.CreateAsync(score);
        }

        return score;
    }

    public async Task UpdateScoreOnSuccessAsync(string ipAddress)
    {
        var score = await GetOrCreateScoreAsync(ipAddress);
        //Math.Min 2 sayıyı karşılaştırır ve küçük olanı döndürür.
        score.Score = Math.Min(score.Score + SUCCESS_POINTS, MAX_SCORE);
        score.LastCaptchaSuccess = DateTime.UtcNow;
        score.FailedAttempts = 0;

        score.ExemptionEndDate = GetExemptionEndDate(score.Score);

        await _repository.UpdateAsync(score);
    }

    public async Task UpdateScoreOnFailureAsync(string ipAddress)
    {
        var score = await GetOrCreateScoreAsync(ipAddress);
        score.Score = Math.Max(0, score.Score + FAILURE_POINTS);
        score.FailedAttempts++;
        score.LastCaptchaAttempt = DateTime.UtcNow;
        
        await _repository.UpdateAsync(score);
    }

    public async Task UpdateActivityTimeAsync(string ipAddress)
    {
        var score = await GetOrCreateScoreAsync(ipAddress);
        var timeDiff = DateTime.UtcNow - score.LastActivityTime;
        
        if (timeDiff.TotalMinutes >= 30)
        {
            score.Score = Math.Min(score.Score + ACTIVITY_POINTS, MAX_SCORE);
            score.LastActivityTime = DateTime.UtcNow;
            await _repository.UpdateAsync(score);
        }
    }

    public async Task<bool> NeedsCaptchaAsync(string ipAddress)
    {
        var score = await GetOrCreateScoreAsync(ipAddress);
        
        if (DateTime.UtcNow <= score.ExemptionEndDate)
            return false;

        return score.Score switch
        {
            < 20 => true,
            < 40 => !HasValidSessionCaptcha(score),
            _ => false
        };

    }

    public async Task<CaptchaExemptionLevel> GetExemptionLevelAsync(string ipAddress)
    {
        var score = await GetOrCreateScoreAsync(ipAddress);
        
        return score.Score switch
        {
            >= 70 => CaptchaExemptionLevel.MonthlyExemption,
            >= 40 => CaptchaExemptionLevel.WeeklyExemption,
            >= 20 => CaptchaExemptionLevel.SessionBased,
            _ => CaptchaExemptionLevel.NoExemption
        };

    }
    
    private DateTime GetExemptionEndDate(int score)
    {
        return score switch
        {
            >= 70 => DateTime.UtcNow.AddDays(30),
            >= 40 => DateTime.UtcNow.AddDays(7),
            _ => DateTime.UtcNow
        };
    }

    private bool HasValidSessionCaptcha(VisitorScore score)
    {
        // Session bazlı kontrol için son başarılı captcha'nın aynı oturumda olup olmadığını kontrol et
        return (DateTime.UtcNow - score.LastCaptchaSuccess).TotalHours < 24;
    }

}
