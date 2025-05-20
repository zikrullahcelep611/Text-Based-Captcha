using Microsoft.AspNetCore.Identity;

namespace Text_Captcha.Infrastructure.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Initials { get; set; }
}