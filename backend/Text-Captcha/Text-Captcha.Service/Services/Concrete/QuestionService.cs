using Microsoft.EntityFrameworkCore;
using Text_Captcha.Infrastructure.DbContext;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastucture.DTOs;
using Text_Captcha.Service.Services.Abstract;

namespace  Text_Captcha.Service.Services.Concrete;

public class QuestionService : IQuestionService
{

    private readonly ApplicationDbContext _context;
    private readonly IRepository<Question> _questionRepository;
    private readonly IRepository<Answer> _answerRepository;
    private readonly IOptionRepository<Option> _optionRepository;

    public QuestionService(ApplicationDbContext context, IRepository<Answer> answerRepository,
        IRepository<Question> questionRepository, IOptionRepository<Option> optionRepository)
    {
        _context = context;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
        _optionRepository = optionRepository;
    }

    public async Task<Question?> GetRandomQuestionAsync()
    {
        return await _context.Questions.OrderBy(q => EF.Functions.Random())
            .FirstOrDefaultAsync();
    }

    public async Task<QuestionResponseDTO> GetQuestionWithIdAsync(int questionId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);

        var questionResponseDto = new QuestionResponseDTO
        {

            QuestionId = question.QuestionId,
            QuestionText = question.QuestionText,
            Options = question.Options.Select(o => new OptionResponseDTO
            {
                OptionId = o.OptionId,
                QuestionId = o.QuestionId,
                OptionText = o.OptionText,
                IsCorrect = o.IsCorrect
            }).ToList()
        };

        return questionResponseDto;
    }
    
    public async Task<bool> CheckAnswer(AnswerDTO model, int questionId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

        //Büyük/küçük harf duyarsız karşılaştırma
        bool checkMatch = string.Equals(
            model.AnswerText.Trim(),
            correctOption.OptionText.Trim(),
            StringComparison.OrdinalIgnoreCase
        );

        return checkMatch;
    }

    public async Task<QuestionResponseDTO> CreateQuestion(CreateQuestionDTO model)
    {
        // Input doğrulaması
        if (string.IsNullOrWhiteSpace(model.QuestionText) || model.Options == null || !model.Options.Any())
        {
            throw new ArgumentException("Soru metni ve en az bir seçenek gereklidir.");
        }
        
        var question = new Question
        {
            QuestionText = model.QuestionText,
        };

        // Save the question first to get a QuestionId
        await _questionRepository.CreateAsync(question);

        // Now create options with the proper QuestionId
        var options = model.Options.Select(o => new Option
        {
            QuestionId = question.QuestionId,
            OptionText = o.OptionText,
            IsCorrect = o.IsCorrect
        }).ToList();

        // Add options to the question
        question.Options = options;

        await _optionRepository.CreateRangeAsync(options);

        // Map to response DTO
        var responseDTO = new QuestionResponseDTO
        {
            QuestionId = question.QuestionId,
            QuestionText = question.QuestionText,
            Options = options.Select(o => new OptionResponseDTO
            {
                OptionId = o.OptionId,
                QuestionId = o.QuestionId,
                OptionText = o.OptionText,
                IsCorrect = o.IsCorrect
            }).ToList()
        };
        return responseDTO;
    }

    public async Task<string> GetQuestionAnswerWithQuestionId(int questionId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        
        foreach (var option in question.Options)
        {
            if (option.IsCorrect)
            {
                return option.OptionText;
            }
        }

        //If there isn't true option return null
        return null;
    }
}
