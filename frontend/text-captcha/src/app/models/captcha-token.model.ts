export interface CaptchaTokenResponse{
    token: string;
    expiresIn: Date;
    questionId: number;
    isUsed: boolean;
}