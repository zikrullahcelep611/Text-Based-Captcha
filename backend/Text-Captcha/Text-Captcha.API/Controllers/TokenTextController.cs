using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Text_Captcha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenTextController : ControllerBase
{
    private readonly IDatabase _database;

    public TokenTextController(IDatabase database)
    {
        _database = database;
    }

    [HttpPost("set")]
    public async Task<IActionResult> SetValue(string key, string value, int expireSeconds = 3600)
    {
        await _database.StringSetAsync(key, value, TimeSpan.FromSeconds(expireSeconds));
        return Ok($"Key '{key}' set successfully");
    }

    [HttpGet("get/{key}")]
    public async Task<IActionResult> GetValue(string key)
    {
        var value = await _database.StringGetAsync(key);

        if (!value.HasValue)
        {
            return NotFound($"Key '{key} not found");
        }

        return Ok(new { Key = key, Value = value.ToString() });
    }

    [HttpDelete("delete/{key}")]
    public async Task<IActionResult> DeleteValue(string key)
    {
        var deleted = await _database.KeyDeleteAsync(key);
        return Ok($"Key {key} deleted: {deleted}");
    }

    [HttpGet("ttl/{key}")]
    public async Task<IActionResult> GetTTL(string key)
    {
        var ttl = await _database.KeyTimeToLiveAsync(key);
        return Ok(new { Key = key, TTL = ttl?.TotalSeconds ?? -1 });
    }
}
