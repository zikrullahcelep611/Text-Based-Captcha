using Text_Captcha.Infrastructure.Entities;

namespace Text_Captcha.Infrastucture.DTOs;

public class CreateQuestionDTO
{
    public string QuestionText { get; set; }
    public List<OptionDTO> Options { get; set; }
}