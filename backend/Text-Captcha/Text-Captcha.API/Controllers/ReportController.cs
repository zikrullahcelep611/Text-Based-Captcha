using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Text_Captcha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{

    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _redis;

    public ReportController(IDatabase database, IConnectionMultiplexer redis)
    {
        _database = database;
        _redis = redis;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetBannedIpAddresses()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var bannedIps = new List<string>();

        await foreach (var key in server.KeysAsync(pattern: "banned:*"))
        {
            var ip = key.ToString().Substring("banned:".Length);
            bannedIps.Add(ip);
        }

        if (!bannedIps.Any())
        {
            return NotFound("No banned IP addresses found");
        }

        return Ok(bannedIps);
    }
}