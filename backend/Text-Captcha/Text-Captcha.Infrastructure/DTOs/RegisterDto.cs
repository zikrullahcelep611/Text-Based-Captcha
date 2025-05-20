using System.ComponentModel.DataAnnotations;

namespace Text_Captcha.Infrastucture.DTOs;

public class RegisterDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Password { get;set; }
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string UserName { get; set; }
}