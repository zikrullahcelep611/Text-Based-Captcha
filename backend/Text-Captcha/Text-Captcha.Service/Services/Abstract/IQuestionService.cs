using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastucture.DTOs;

namespace Text_Captcha.Service.Services.Abstract;

public interface IQuestionService
{
    Task<Question> GetQuestion();
    Task<bool> CheckAnswer(AnswerDTO model, int questionId);
    Task<QuestionResponseDTO> CreateQuestion(CreateQuestionDTO model);
}