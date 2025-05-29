using Microsoft.AspNetCore.Http;
using Text_Captcha.Infrastucture.Helpers;
using Text_Captcha.Service.Services.Abstract;

namespace Text_Captcha.Service.Services.Concrete;
 
public class IpAddressService : IIpAddressService
{
    private readonly IHttpContextAccessor _httpContextAccessor;


    public IpAddressService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
        
    
    public string GetCurrentIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;

        if (context == null)
            return string.Empty;

        return IpHelper.GetIpAddress(context);
    }
}