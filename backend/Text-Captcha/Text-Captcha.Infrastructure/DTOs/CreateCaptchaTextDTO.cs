namespace Text_Captcha.Infrastucture.DTOs;

public class CreateCaptchaTextDTO
{
    public string QuestionText { get; set; }
    public string ContentText { get; set; }
    public List<string> AnswerWords { get; set; }
}