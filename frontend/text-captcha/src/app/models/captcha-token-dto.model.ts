export interface CaptchaTokenResponseDTO{
    token: string;
    expiresIn: Date;
    captchaTextId: number;
    isUsed: boolean;
}