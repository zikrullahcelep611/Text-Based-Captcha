namespace Text_Captcha.Infrastructure.Entities;

public class VisitorScore
{
    public int Id { get; set; }
    public string IpAddress { get; set; }
    public int Score { get; set; }
    public DateTime LastCaptchaSuccess { get; set; }
    public DateTime LastCaptchaAttempt { get; set; }
    public DateTime LastActivityTime { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime ExemptionEndDate { get; set; }
}