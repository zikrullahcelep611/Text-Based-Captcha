namespace Text_Captcha.Infrastructure.Entities;

public class Answer
{
    public int AnswerId { get; set; }
    public int QuestionId { get; set; }
    public string AnswerText { get; set; } 
}