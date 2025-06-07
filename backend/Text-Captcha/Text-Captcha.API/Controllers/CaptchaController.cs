using Microsoft.AspNetCore.Mvc;
using Text_Captcha.Infrastucture.DTOs;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaptchaController : ControllerBase
{

    private readonly ICaptchaTokenService _tokenService;
    private readonly IIpAddressService _ipAddressService;


    public CaptchaController(ICaptchaTokenService tokenService, IIpAddressService ipAddressService)
    {
        _tokenService = tokenService;
        _ipAddressService = ipAddressService;
    }

    [HttpGet("generate")]
    public async Task<IActionResult> GenerateToken()
    {
        string ipAddress = _ipAddressService.GetCurrentIpAddress();
        if (string.IsNullOrEmpty(ipAddress))
        {
            return BadRequest("Invalid ip address");
        }
        
        try
        {
            var tokenResponse = await _tokenService.GenerateTokenAsync(ipAddress);

            return Ok(tokenResponse);
        }
        catch(Exception ex)
        {
            return StatusCode(403, "Ip address is banned, please try again later");
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyCaptchaAnswer([FromBody]CaptchaRequestAnswerDTO captchaRequestAnswerDto)
    {
        bool isCorrect = await _tokenService.VerifyCaptchaAnswerAsync(captchaRequestAnswerDto);

        var response = new CaptchaVerificationResponse
        {
            Success = isCorrect,
            Message = isCorrect ? "Captcha verification is success" : "Invalid captcha answer"
        };

        return Ok(response);
    }
}
