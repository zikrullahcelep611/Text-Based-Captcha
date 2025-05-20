namespace Text_Captcha.Infrastucture.DTOs;

//DTO for returning question information
public class QuestionResponseDTO
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public List<OptionResponseDTO> Options { get; set; }
}