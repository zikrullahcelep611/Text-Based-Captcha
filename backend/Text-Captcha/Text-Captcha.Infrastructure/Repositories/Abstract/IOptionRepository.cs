using Text_Captcha.Infrastructure.Entities;

namespace Text_Captcha.Infrastructure.Repositories.Abstract;

public interface IOptionRepository<T> : IRepository<T> where T : class
{
    Task CreateRangeAsync(IEnumerable<Option> options);
}