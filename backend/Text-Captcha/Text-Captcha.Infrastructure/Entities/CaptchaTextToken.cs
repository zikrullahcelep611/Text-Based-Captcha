namespace Text_Captcha.Infrastructure.Entities;

public class CaptchaTextToken
{
    public string CaptchaTextTokenId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public string IpAddress { get; set; }
    public int CaptchaTextId { get; set; }
    public CaptchaStatus Status { get; set; }
}