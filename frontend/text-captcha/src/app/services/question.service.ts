import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateQuestionDTO } from '../models/question.model';
import { Observable } from 'rxjs';


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

  constructor(private http: HttpClient) {}

  createQuestion(question: CreateQuestionDTO): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/CreateQuestion`, question);
  }


  getQuestion(): Observable<QuestionResponseDTO> {
    return this.http.get<QuestionResponseDTO>(`${this.apiUrl}/GetQuestion`);
  }
}
