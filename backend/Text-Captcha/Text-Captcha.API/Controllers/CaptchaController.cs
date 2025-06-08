using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastucture.DTOs;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaptchaController : ControllerBase
{
    private readonly IRepository<CaptchaText> _captchaTextRepository;
    private readonly ICaptchaTextService _captchaTextService; 
    private readonly IIpAddressService _ipAddressService;

    public CaptchaController(IRepository<CaptchaText> captchaTextRepository, ICaptchaTextService captchaTextService
        ,IIpAddressService ipAddressService)
    {
        _captchaTextService = captchaTextService;
        _ipAddressService = ipAddressService;
        _captchaTextRepository = captchaTextRepository;
    }
    
    [HttpGet("GetCaptchaWithId/{captchaTextId}")]
    public async Task<ActionResult<CaptchaTextResponseDTO>> GetQuestionWithId(int captchaTextId)
    {
        
        var captchaResponse = await _captchaTextService.GetCaptchaTextWithIdAsync(captchaTextId);
        return Ok(captchaResponse);
    } 
    
    [Authorize]
    [HttpPost("CreateCaptcha")]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateCaptchaTextDTO model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var questionResponse = await _captchaTextService.CreateCaptchaText(model);

            return Ok(new {message = "Captcha başarılı bir şekilde oluşturuldu."});
        }

        catch (Exception ex)
        {
            return BadRequest($"Soru oluşturulurken hata oluştu: {ex.Message}");
        }
    }
}
