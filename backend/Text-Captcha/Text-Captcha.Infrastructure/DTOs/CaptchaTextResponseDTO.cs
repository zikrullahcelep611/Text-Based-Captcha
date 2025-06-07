namespace Text_Captcha.Infrastucture.DTOs;

//Front tarafina yollanacak soru ve metin
public class CaptchaTextResponseDTO
{
    public int CaptchaTextId { get; set; }
    public string CaptchaTextContent { get; set; }
    public string CaptchaTextQuestion { get; set; }
}