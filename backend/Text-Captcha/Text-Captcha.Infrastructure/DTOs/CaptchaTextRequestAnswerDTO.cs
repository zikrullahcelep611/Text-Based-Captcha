namespace Text_Captcha.Infrastucture.DTOs;

public class CaptchaTextRequestAnswerDTO
{
    public List<string> AnswerWords { get; set; }
    public string TokenId { get; set; }
}