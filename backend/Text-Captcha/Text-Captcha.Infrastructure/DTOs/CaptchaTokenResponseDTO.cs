namespace Text_Captcha.Infrastucture.DTOs;

public class CaptchaTokenResponseDTO
{
    public string Token { get; set; }
    public DateTime ExpiresIn { get; set; }
    public int CaptchaTextId { get; set; }
    public bool IsUsed { get; set; }
}