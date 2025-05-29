using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastucture.DTOs;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.Service.Services.Concrete;

public class CaptchaTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ICaptchaTokenRepository<CaptchaToken> _tokenRepository;
    private readonly IQuestionService _questionService;
    private readonly IpAddressService _ipAddressService;
    private readonly IDatabase _redisDatabase;

    public CaptchaTokenService(IDatabase redisDatabase, IpAddressService ipAddressService,
        ICaptchaTokenRepository<CaptchaToken> tokenRepository, IQuestionService questionService)
    {
        _tokenRepository = tokenRepository;
        _questionService = questionService;
        _ipAddressService = ipAddressService;
        _redisDatabase = redisDatabase;
    }

    public async Task<CaptchaTokenResponse> GenerateTokenAsync(string ipAddress)
    {
        
        if (await _redisDatabase.KeyExistsAsync($"banned:{ipAddress}"))
        {
            throw new InvalidOperationException("IP address is banned. Please try again later.");
        }
        
        try
        {
            if(!await _redisDatabase.KeyExistsAsync($"refresh:{ipAddress}"))
            {
                await _redisDatabase.StringSetAsync($"refresh:{ipAddress}", 0, TimeSpan.FromHours(24));
            }
           
            int refreshCount = (int) await _redisDatabase.StringIncrementAsync($"refresh:{ipAddress}");

            if (refreshCount >= 4)
            {
                await _redisDatabase.StringSetAsync($"banned:{ipAddress}", "banned", TimeSpan.FromHours(2));
                throw new InvalidOperationException("Too many refresh attempts. IP banned for 24 hours.");
            }

            var question = await _questionService.GetRandomQuestionAsync();
            var token = new CaptchaToken
            {
                TokenId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                IpAddress = ipAddress,
                Status = CaptchaStatus.Active,
                QuestionId = question.QuestionId
            };

            await _tokenRepository.SaveTokenAsync(token);

            var tokenResponse = new CaptchaTokenResponse
            {
                Token = token.TokenId,
                ExpiresIn = token.ExpiresAt,
                QuestionId = token.QuestionId,
                IsUsed = false
            };

            return tokenResponse;
        }

        catch (Exception ex)
        {
            throw new Exception("Token oluşturma sırasında hata oluştu.", ex);
        }
    }

    public async Task<bool> VerifyCaptchaAnswer(CaptchaRequestAnswerDTO captchaRequestAnswerDto)
    {
        var ipAddress = _ipAddressService.GetCurrentIpAddress();
        
        if (string.IsNullOrEmpty(ipAddress))
        {
            throw new ArgumentException("IP address cannot be null or empty.");
        }
        
        if (await _redisDatabase.KeyExistsAsync($"banned:{ipAddress}"))
        {
            throw new InvalidOperationException("IP address is banned. Please try again later.");
        }
        
        var token = await _tokenRepository.GetTokenByIdAsync(captchaRequestAnswerDto.TokenId);
        
        if (token == null)
        {
            throw new InvalidOperationException("Token not found");
        }
        
        if (token.IpAddress != ipAddress)
        {
            throw new InvalidOperationException("IP adderss mismatch");
        }

        if (token.IsUsed)
        {
            throw new InvalidOperationException("Token has already been used");
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Token has expired");
        }
        
        string correctAnswer = await _questionService.GetQuestionAnswerWithQuestionId(token.QuestionId);
        if (captchaRequestAnswerDto.Answer == correctAnswer)
        {
            token.IsUsed = true;
            token.Status = CaptchaStatus.Completed;
            await _tokenRepository.UpdateTokenAsync(token);
            return true;
        }

        if (!await _redisDatabase.KeyExistsAsync($"answer:{ipAddress}"))
        {
            await _redisDatabase.StringSetAsync($"answer:{ipAddress}", 0, TimeSpan.FromHours(24));
        }

        int answerAttemptCount = (int) await _redisDatabase.StringIncrementAsync($"answer:{ipAddress}");

        if (answerAttemptCount >= 4)
        {
            await _redisDatabase.StringSetAsync($"banned:{ipAddress}", "banned", TimeSpan.FromHours(1));
        }
        
        return false;
    }

    public async Task<int> GetRefreshCount(string refreshKey)
    {
        var refreshCountValue = await _redisDatabase.StringGetAsync(refreshKey);
        int refreshCount = 0;
        if (refreshCountValue.HasValue && int.TryParse(refreshCountValue.ToString(), out var count))
        {
            refreshCount = count;
        }

        return refreshCount;
    }
}
