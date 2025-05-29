namespace Text_Captcha.Infrastructure.Entities;

public class CaptchaToken
{
    public string TokenId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public string IpAddress { get; set; }
    public int QuestionId { get; set; }
    public CaptchaStatus Status { get; set; }
}