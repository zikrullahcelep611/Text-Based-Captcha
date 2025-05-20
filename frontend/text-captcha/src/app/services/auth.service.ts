import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { RegisterDto } from '../models/register-dto.model';
import { LoginDTO } from '../models/login-dto.model';

interface AuthResponse {
  token: string
}

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  private apiUrl = 'http://localhost:5085/api/Account'; // Backend API URL’nizi buraya ekleyin

  constructor(private http: HttpClient) {}

  register(model: RegisterDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, model, { responseType: 'json' }).pipe(
      catchError(this.handleError)
    );
  }

  login(loginData: LoginDTO): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/Login`, loginData)
      .pipe(
        tap(response => {
          // Token'ı localStorage'a kaydet
          localStorage.setItem('token', response.token);
        })
      );
  }

  logout(): void{
    if (typeof window !== 'undefined') {
      localStorage.removeItem('token');
    }
  }

  isLoggedIn() {
    return typeof window !== 'undefined' && !!localStorage.getItem('token');
  }
  
  getToken(): string | null {
      return typeof window !== 'undefined' ? localStorage.getItem('token') : null;
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Bilinmeyen bir hata oluştu!';
    if (error.error instanceof ErrorEvent) {
      // İstemci tarafı hata
      errorMessage = `Hata: ${error.error.message}`;
    } else {
      // Sunucu tarafı hata
      if (error.status === 400 && error.error) {
        // Backend’den gelen hata mesajlarını topla
        errorMessage = Array.isArray(error.error) ? error.error.join(', ') : error.error.Message || errorMessage;
      } else if (error.status === 500) {
        errorMessage = error.error.Message || 'Sunucu hatası oluştu.';
      }
    }
    return throwError(() => new Error(errorMessage));
  }
}
