using Microsoft.EntityFrameworkCore;
using Text_Captcha.Infrastructure.DbContext;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastucture.DTOs;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.Service.Services.Concrete;

public class CaptchaTextService : ICaptchaTextService
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<CaptchaText> _captchaTextRepository;

    public CaptchaTextService(IRepository<CaptchaText> captchaTextRepository, ApplicationDbContext context)
    {
        _captchaTextRepository = captchaTextRepository;
        _context = context;
    }

    public async Task<CaptchaText> GetRandomCaptchaTextAsync()
    {
        return await _context.CaptchaTexts.OrderBy(t => EF.Functions.Random())
            .FirstOrDefaultAsync();
    }

    public async Task<CaptchaTextResponseDTO> GetCaptchaTextWithIdAsync(int captchaTextId)
    {
        var captchaText = await _captchaTextRepository.GetByIdAsync(captchaTextId);

        var captchaTextResponseDTO = new CaptchaTextResponseDTO
        {
            CaptchaTextId = captchaText.Id,
            CaptchaTextContent = captchaText.ContentText,
            CaptchaTextQuestion = captchaText.QuestionText
        };

        return captchaTextResponseDTO;
    }

    public async Task<CaptchaTextResponseDTO> CreateCaptchaText(CreateCaptchaTextDTO model)
    {
        if (string.IsNullOrWhiteSpace(model.QuestionText) || string.IsNullOrWhiteSpace(model.QuestionText) ||
           model.AnswerWords == null)
        {
            throw new ArgumentNullException("Required properties cannot be null");
        }

        var captchaText = new CaptchaText
        {
            QuestionText = model.QuestionText,
            ContentText = model.ContentText,
            AnswerWords = model.AnswerWords
        };

        await _captchaTextRepository.CreateAsync(captchaText);

        var captchaTextResponseDTO = new CaptchaTextResponseDTO
        {
            CaptchaTextId = captchaText.Id,
            CaptchaTextQuestion = captchaText.QuestionText,
            CaptchaTextContent = captchaText.ContentText
        };

        return captchaTextResponseDTO;
    }

    public async Task<List<string>> GetCaptchaAnswerWithCaptchaId(int captchaTextId)
    {
        var captchaText = await _captchaTextRepository.GetByIdAsync(captchaTextId);

        return captchaText.AnswerWords;
    }
}