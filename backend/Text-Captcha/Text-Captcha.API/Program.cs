using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Text_Captcha.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Text_Captcha.API.Extensions;
using Text_Captcha.Infrastructure.Entities;
using Text_Captcha.Infrastructure.Repositories.Abstract;
using Text_Captcha.Infrastructure.Repositories.Concrete;
using Text_Captcha.Service.Services.Abstract;
using Text_Captcha.Service.Services.Concrete;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddDatabaseServices(builder.Configuration)
    .AddRedisServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");
app.MapControllers();

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();