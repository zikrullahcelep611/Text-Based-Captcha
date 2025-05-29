using Microsoft.EntityFrameworkCore;
using Text_Captcha.Infrastructure.DbContext;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;

namespace Text_Captcha.Infrastructure.Repositories.Concrete;

public class CaptchaTokenRepository<CaptchaToken> : ICaptchaTokenRepository<CaptchaToken> where CaptchaToken : class
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<CaptchaToken> _dbSet;

    public CaptchaTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<CaptchaToken>();
    }
    
    public async Task SaveTokenAsync(CaptchaToken token)
    {
        _dbSet.AddAsync(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTokenAsync(CaptchaToken token)
    {
        
    }

    public async Task<CaptchaToken> GetTokenByIdAsync(string tokenId)
    {
        return await _dbSet.FindAsync(tokenId);
    }

    public async Task UpdateTokenAsync(CaptchaToken token)
    {
        _dbSet.Update(token);
        await _dbContext.SaveChangesAsync();
    }
    
}