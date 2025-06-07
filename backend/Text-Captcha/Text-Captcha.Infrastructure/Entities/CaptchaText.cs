namespace Text_Captcha.Infrastructure.Entities;

public class CaptchaText
{
    public int Id { get; set; }
    public string QuestionText { get; set; } // metinle alakalı soru
    public string ContentText { get; set; } // metnin kendisi
    public List<string> AnswerWords { get; set; }
}