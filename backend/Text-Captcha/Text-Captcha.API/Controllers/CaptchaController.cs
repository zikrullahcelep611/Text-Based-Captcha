using Microsoft.AspNetCore.Mvc;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.API.Controllers;

[ApiController]
[Route("api/{controller}")]
public class CaptchaController : ControllerBase
{

    private readonly ICaptchaTokenService _tokenService;
    private readonly IIpAddressService _ipAddressService;


    public CaptchaController(ICaptchaTokenService tokenService, IIpAddressService ipAddressService)
    {
        _tokenService = tokenService;
        _ipAddressService = ipAddressService;
    }

    [HttpPost("generate")]
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
}
