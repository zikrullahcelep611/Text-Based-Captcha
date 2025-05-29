using Microsoft.AspNetCore.Http;

namespace Text_Captcha.Service.Services.Abstract;

public interface IIpAddressService
{
    string GetCurrentIpAddress();
}