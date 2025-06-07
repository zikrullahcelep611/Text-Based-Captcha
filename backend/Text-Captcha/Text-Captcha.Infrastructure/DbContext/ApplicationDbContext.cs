using Text_Captcha.Infrastructure.Entities;

namespace Text_Captcha.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User>
{
    string connectionString =
        "Host=localhost;Port=5432;Database=captcha_Db;Username=postgres;Password=password;SSL Mode=Prefer;";
    
    public ApplicationDbContext()
    {
            
    }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<CaptchaToken> CaptchaTokens { get; set; }
    public DbSet<CaptchaTextToken> CaptchaTextTokens { get; set; }
    public DbSet<CaptchaText> CaptchaTexts { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /*builder.Entity<Question>()
            .HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId);*/
        
        builder.Entity<User>().Property(u => u.Initials).HasMaxLength(5);

        builder.HasDefaultSchema("identity");
    }
}
