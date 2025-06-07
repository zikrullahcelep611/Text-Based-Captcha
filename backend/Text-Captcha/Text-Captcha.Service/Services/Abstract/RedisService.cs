using Microsoft.EntityFrameworkCore.Storage;

namespace Text_Captcha.Service.Services.Abstract;

public interface RedisService
{

    public Task SetValue(string key, string value, int expireSeconds = 3600);
    public Task<string> GetValue(string key);

}