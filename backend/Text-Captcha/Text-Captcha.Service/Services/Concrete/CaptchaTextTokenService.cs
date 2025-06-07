using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Text_Captcha.Infrastructure.DbContext;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastucture.DTOs;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.Service.Services.Concrete;

public class CaptchaTextTokenService : ICaptchaTextTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ICaptchaTextTokenRepository<CaptchaTextToken> _captchaTextTokenRepository;
    private readonly ICaptchaTextService _captchaTextService;
    private readonly IIpAddressService _ipAddressService;
    private readonly IDatabase _redisDatabase;

    public CaptchaTextTokenService(IDatabase redisDatabase, IIpAddressService ipAddressService,
        ICaptchaTextTokenRepository<CaptchaTextToken> captchaTextTokenRepository, ICaptchaTextService captchaTextService)
    {
        _captchaTextTokenRepository = captchaTextTokenRepository;
        _captchaTextService = captchaTextService;
        _ipAddressService = ipAddressService;
        _redisDatabase = redisDatabase;
    }

    /*
    public class CaptchaTokenResponseDTO
    {
        public string Token { get; set; }
        public DateTime ExpiresIn { get; set; }
        public int CaptchaTextId { get; set; }
        public bool IsUsed { get; set; }
    }
    */
    
    public async Task<CaptchaTokenResponseDTO> GenerateTokenAsync(string ipAddress)
    {
        if (await _redisDatabase.KeyExistsAsync($"banned:{ipAddress}"))
        {
            throw new InvalidOperationException("IP address is banned. Please try again later.");
        }

        try
        {
            if (!await _redisDatabase.KeyExistsAsync($"refresh:{ipAddress}"))
            {
                await _redisDatabase.StringSetAsync($"refresh:{ipAddress}", 0, TimeSpan.FromHours(24));
            }

            int refreshCount = (int)await _redisDatabase.StringIncrementAsync($"refresh:{ipAddress}");

            if (refreshCount >= 50)
            {
                await _redisDatabase.StringSetAsync($"banned:{ipAddress}", "banned", TimeSpan.FromHours(2));
                throw new InvalidOperationException("Too many refresh attempts. IP banned for 24 hours.");
            }

            var textCaptcha = await _captchaTextService.GetRandomCaptchaTextAsync();


            var token = new CaptchaTextToken
            {
                CaptchaTextTokenId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                IpAddress = ipAddress,
                Status = CaptchaStatus.Active,
                CaptchaTextId = textCaptcha.Id
            };

            await _captchaTextTokenRepository.SaveTokenAsync(token);

            var tokenResponse = new CaptchaTokenResponseDTO
            {
                Token = token.CaptchaTextTokenId,
                ExpiresIn = token.ExpiresAt,
                CaptchaTextId = token.CaptchaTextId,
                IsUsed = false
            };

            return tokenResponse;
        }
        catch (Exception ex)
        {
            throw new Exception("Token oluşturma sırasında hata oluştu.", ex);
        }
    }

    public async Task<bool> VerifyCaptchaAnswerAsync(CaptchaTextRequestAnswerDTO model)
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

        var token = await _captchaTextTokenRepository.GetTokenByIdAsync(model.TokenId);
        
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

        List<string> correctAnswers = await _captchaTextService.GetCaptchaAnswerWithCaptchaId(token.CaptchaTextId);

        bool areEqual = correctAnswers.OrderBy(x => x).SequenceEqual(model.AnswerWords.OrderBy((x => x)));

        if (areEqual)
        {
            token.IsUsed = true;
            token.Status = CaptchaStatus.Completed;
            await _captchaTextTokenRepository.UpdateTokenAsync(token);
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
}