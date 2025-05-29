using System.Linq.Expressions;

namespace Text_Captcha.Infrastructure.Repositories.Abstract;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T> GetAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task UpdateAsync(T entity);
    Task CreateAsync(T entity);
    
}
