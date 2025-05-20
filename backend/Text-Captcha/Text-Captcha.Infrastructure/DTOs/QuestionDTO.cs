namespace Text_Captcha.Infrastucture.DTOs;

public class QuestionDTO
{
    public string QuestionText { get; set; }
    public int QuestionId { get; set; }
    public List<OptionDTO> Options { get; set; }
}