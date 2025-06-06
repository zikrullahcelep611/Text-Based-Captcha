import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CaptchaRequestAnswerDTO } from '../models/captcha-answer.model';
import { CaptchaTokenResponse } from '../models/captcha-token.model';
import { CaptchaVerificationResponse } from '../models/captcha-verification-model';
import { CreateQuestionDTO } from '../models/question.model';
import { BannedIps } from '../models/banned-ip.model';


export interface OptionResponseDTO{
  optionId: number,
  questionId: number,
  optionText: string,
  isCorrect: boolean
}

export interface QuestionResponseDTO{
  questionId: number,
  questionText: string,
  options: OptionResponseDTO[]
}

@Injectable({ providedIn: 'root' })
export class QuestionService {
  private apiUrl = 'http://localhost:5085/api/Question';
  private captchaControllerUrl = 'http://localhost:5085/api/Captcha';
  private reportControllerUrl = 'http://localhost:5085/api/Report';

  constructor(private http: HttpClient) {}

  getQuestion(): Observable<QuestionResponseDTO> {
    return this.http.get<QuestionResponseDTO>(`${this.apiUrl}/GetQuestion`);
  }

  getQuestionWithId(questionId: number): Observable<QuestionResponseDTO> {
    return this.http.get<QuestionResponseDTO>(`${this.apiUrl}/GetQuestionWithId/${questionId}`);
  }

  generateToken(): Observable<CaptchaTokenResponse> {
    return this.http.get<CaptchaTokenResponse>(`${this.captchaControllerUrl}/Generate`);
  }
  
  verifyCaptchaAnswer(captchaRequestAnswerDto: CaptchaRequestAnswerDTO):Observable<CaptchaVerificationResponse>{
    return this.http.post<CaptchaVerificationResponse>(`${this.captchaControllerUrl}/Verify`, captchaRequestAnswerDto);
  }

  createQuestion(question: CreateQuestionDTO): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/CreateQuestion`, question);
  }

  getBannedIpAddresses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.reportControllerUrl}/Get`);
  }
}
