namespace Text_Captcha.Infrastructure.Entities;

public class CaptchaTokenResponse
{
    public string Token { get; set; }
    public DateTime ExpiresIn { get; set; }
    public int QuestionId { get; set; }
    public bool IsUsed { get; set; }
}