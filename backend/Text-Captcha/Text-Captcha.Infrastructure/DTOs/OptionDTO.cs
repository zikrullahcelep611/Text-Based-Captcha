namespace Text_Captcha.Infrastucture.DTOs;

//DTO for creating options
public class OptionDTO
{
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}