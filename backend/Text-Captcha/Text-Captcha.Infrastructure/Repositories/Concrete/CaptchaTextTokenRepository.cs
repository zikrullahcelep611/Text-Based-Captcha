using Microsoft.EntityFrameworkCore;
using Text_Captcha.Infrastructure.DbContext;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;

namespace Text_Captcha.Infrastructure.Repositories.Concrete;

public class CaptchaTextTokenRepository<CaptchaTextToken> : ICaptchaTextTokenRepository<CaptchaTextToken> where CaptchaTextToken : class 
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<CaptchaTextToken> _dbSet;
    
    public CaptchaTextTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<CaptchaTextToken>();
    }
    
    public async Task SaveTokenAsync(CaptchaTextToken token)
    {
        _dbSet.AddAsync(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<CaptchaTextToken> GetTokenByIdAsync(string tokenId)
    {
        try
        {
            return await _dbSet.FindAsync(tokenId);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred when taking a token from the database.", ex);
        }
    }

    public async Task UpdateTokenAsync(CaptchaTextToken token)
    {
        _dbSet.Update(token);
        await _dbContext.SaveChangesAsync();
    }
}