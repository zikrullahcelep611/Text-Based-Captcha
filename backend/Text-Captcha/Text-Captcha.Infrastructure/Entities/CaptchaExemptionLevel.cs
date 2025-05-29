namespace Text_Captcha.Infrastructure.Entities;

public enum CaptchaExemptionLevel
{
    NoExemption = 0,
    SessionBased = 1,
    WeeklyExemption = 2,
    MonthlyExemption = 3
}