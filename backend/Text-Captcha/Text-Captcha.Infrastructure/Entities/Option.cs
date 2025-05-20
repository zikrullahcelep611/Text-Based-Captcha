namespace Text_Captcha.Infrastructure.Entities;

public class Option
{
    public virtual int OptionId { get; set; }
    public virtual int QuestionId { get; set; }
    public virtual string OptionText { get; set; }
    public virtual bool IsCorrect {get; set; }
    public virtual Question Question { get; set; } //Navigation property ekleyin
}