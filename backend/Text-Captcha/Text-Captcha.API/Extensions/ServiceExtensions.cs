using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Text_Captcha.Infrastructure.DbContext;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastructure.Repositories.Concrete;
using Text_Captcha.Service.Services.Abstract;
using Text_Captcha.Service.Services.Concrete;

namespace Text_Captcha.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        
        //Repository registrations
        services.AddScoped<IRepository<Question>, Repository<Question>>();
        services.AddScoped<IRepository<Option>, Repository<Option>>();
        services.AddScoped<IRepository<Answer>, Repository<Answer>>();
        services.AddScoped<IRepository<VisitorScore>, Repository<VisitorScore>>();
        services.AddScoped<IRepository<CaptchaText>, Repository<CaptchaText>>();
        services
            .AddScoped<ICaptchaTextTokenRepository<CaptchaTextToken>, CaptchaTextTokenRepository<CaptchaTextToken>>();
        // Service registrations
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IOptionRepository<Option>, OptionRepository<Option>>();
        services.AddScoped<IVisitorScoreService, VisitorScoreService>();
        services.AddScoped<ICaptchaTokenRepository<CaptchaToken>, CaptchaTokenRepository<CaptchaToken>>();
        services.AddScoped<IIpAddressService, IpAddressService>();
        services.AddScoped<ICaptchaTokenService, CaptchaTokenService>();
        services.AddScoped<ICaptchaTextTokenService, CaptchaTextTokenService>();
        services.AddScoped<ICaptchaTextService, CaptchaTextService>();
        
        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseLazyLoadingProxies().UseNpgsql(connectionString));

        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
    public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var connectionStrings = configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(connectionStrings);
        });

        services.AddScoped<IDatabase>(provider =>
        {
            var connectionMultiplexer = provider.GetService<IConnectionMultiplexer>();
            return connectionMultiplexer.GetDatabase();
        });

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });

        return services;
    }
}

