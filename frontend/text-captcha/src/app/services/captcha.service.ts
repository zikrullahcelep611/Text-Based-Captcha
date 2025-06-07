import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { CaptchaTokenResponseDTO } from "../models/captcha-token-dto.model";
import { CaptchaTextResponseDTO } from "../models/captcha-text-response-dto.model";
import { CaptchaTextRequestAnswerDTO } from "../models/captcha-text-request-answer-dto.model";
import { CaptchaVerificationResponse } from "../models/captcha-verification-model";
import { CreateCaptchaTextDTO } from "../models/create-text-captcha-dto.model";


@Injectable({providedIn: 'root'})

export class CaptchaService{

    private captchaUrl = 'http://localhost:5085/api/QuestionText';
    private tokenUrl = 'http://localhost:5085/api/Token';

    constructor(private http: HttpClient) {}

    getCaptchaWithId(captchaId: number): Observable<CaptchaTextResponseDTO>{
        return this.http.get<CaptchaTextResponseDTO>(`${this.captchaUrl}/getcaptchatextwithid/${captchaId}`);
    }

    generateToken(): Observable<CaptchaTokenResponseDTO> {
        return this.http.get<CaptchaTokenResponseDTO>(`${this.tokenUrl}/generate`);
    }

    verifyCaptchaAnswer(captchaTextAnswerRequestDto: CaptchaTextRequestAnswerDTO):Observable<CaptchaVerificationResponse>{
        return this.http.post<CaptchaVerificationResponse>(`${this.tokenUrl}/verify`, captchaTextAnswerRequestDto);
    }

    createCaptcha(captcha: CreateCaptchaTextDTO): Observable<void>{
        return this.http.post<void>(`${this.captchaUrl}/createCaptchaText`,captcha);
    }
}
   