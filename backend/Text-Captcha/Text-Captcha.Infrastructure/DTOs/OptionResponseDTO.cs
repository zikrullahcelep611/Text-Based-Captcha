namespace Text_Captcha.Infrastucture.DTOs;


//DTO for returning option information
public class OptionResponseDTO
{
    public int OptionId { get; set; }
    public int QuestionId { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}