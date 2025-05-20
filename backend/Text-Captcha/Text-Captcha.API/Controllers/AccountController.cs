using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastucture.DTOs;

namespace Text_Captcha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{

    private readonly UserManager<User> _userManager;
    //private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<User> userManager,  IConfiguration configuration)
    {
        _userManager = userManager;
       // _signInManager = signInManager;
        _configuration = configuration;
    }
    

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "Parolalar Eşleşmiyor");
            return BadRequest(ModelState);
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.UserName,
            Email = model.Email
        };

        try
        {
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { Message = "Veritabanı hatası:", Detail = ex.Message });
        }

        catch (Exception ex)
        {
            return StatusCode(500, new {Message = "Bilinmeyen hata:", Detail = ex.Message});
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized();
        }

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims:claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}