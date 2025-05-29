using Microsoft.AspNetCore.Http;

namespace Text_Captcha.Infrastucture.Helpers;

public class IpHelper
{
    public static string GetIpAddress(HttpContext context)
    {
        string ip = string.Empty;

        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            ip = forwardedHeader.Split(',')[0].Trim();
        }

        if (string.IsNullOrEmpty(ip) && context.Connection?.RemoteIpAddress != null)
        {
            ip = context.Connection.RemoteIpAddress.ToString();
        }

        if (ip == "::1")
            ip = "127.0.0.1";

        return ip;
    }
}