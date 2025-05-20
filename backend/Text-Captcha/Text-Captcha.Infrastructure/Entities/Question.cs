namespace Text_Captcha.Infrastructure.Entities;

public class Question
{
    public virtual string QuestionText { get; set; }
    public virtual int QuestionId { get; set; }
    public virtual ICollection<Option> Options { get; set; } = new List<Option>();  
}