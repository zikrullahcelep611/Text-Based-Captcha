using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastucture.DTOs;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{

    private readonly IRepository<Question> _questionRepository;
    private readonly IQuestionService _questionService;
    private readonly IRepository<Option> _optionRepository;
    private readonly IVisitorScoreService _visitorScoreService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIpAddressService _ipAddressService;
    
    public QuestionController(IIpAddressService ipAddressService, IHttpContextAccessor httpContextAccessor, IVisitorScoreService visitorScoreService, IQuestionService questionService, IRepository<Option> optionRepository, IRepository<Question> questionRepository)
    {
        _questionService = questionService;
        _optionRepository = optionRepository;
        _questionRepository = questionRepository;
        _visitorScoreService = visitorScoreService;
        _httpContextAccessor = httpContextAccessor;
        _ipAddressService = ipAddressService;
    }

    [HttpGet("Index")]
    public async Task<ActionResult?> Index()
    {
        try
        {
            var ipAddress = _ipAddressService.GetCurrentIpAddress();
            bool isCaptchaRequired = await _visitorScoreService.NeedsCaptchaAsync(ipAddress);

            if (isCaptchaRequired)
            {
                var questionResponse = await GetQuestion();
                return questionResponse.Result; //ActionResult'ı dön
            }

            return Ok(new
            {
                requiresCaptcha = false,
                message = "Capctcha doğrulaması gerekir."
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Hata oluştu: {ex.Message}");
        }
    }
    
    [HttpGet("GetQuestion")]
    public async Task<ActionResult<QuestionResponseDTO>>GetQuestion()
    {
        try
        {
            var question = await _questionService.GetRandomQuestionAsync();

            if (question == null)
                return NotFound("Soru bulunamadı");

            var questionDto = new QuestionResponseDTO
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
        
            return questionDto;
        }
        
        catch (Exception ex)
        {
            return BadRequest($"Hata oluştu:{ex.Message}");
        }
    }

    /* Once Question nesnesi olustur.
     * Sonra bu question nesnesi icerisine Optionslari doldur
     * Options nesnelerini foreach kullanarak kaydet.
     * Sonra QuestionResponseDTO icerisini doldur.
     */
    
    [Authorize]
    [HttpPost("CreateQuestion")]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDTO model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var correctOptionsCount = model.Options.Count(o => o.IsCorrect);

            if (correctOptionsCount != 1)
                return BadRequest("Tam olarak 1 doğru cevap olmalıdır.");

            var questionResponse = await _questionService.CreateQuestion(model);

            return CreatedAtAction(nameof(GetQuestion), new { id = questionResponse.QuestionId }, questionResponse);
        }

        catch (Exception ex)
        {
            return BadRequest($"Soru oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    [HttpPost("CheckAnswer")]
    public async Task<IActionResult> CheckAnswer([FromBody] AnswerDTO model, int questionId)
    {
        var ipAddress = _ipAddressService.GetCurrentIpAddress();
        bool isCorrect = await _questionService.CheckAnswer(model, questionId);
        
        if (isCorrect)
        {
            await _visitorScoreService.UpdateScoreOnSuccessAsync(ipAddress);
            return Ok(new { success = true, message = "Doğrulama başarılı!" });
        }
        else
        {
            await _visitorScoreService.UpdateScoreOnFailureAsync(ipAddress);
            return BadRequest(new { success = false, message = "Yanlış cavap lütfen tekrar deneyin." });
        }
    }
}
