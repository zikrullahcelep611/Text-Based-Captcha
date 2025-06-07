import { Component, OnInit } from '@angular/core';
import { CaptchaService } from '../services/captcha.service';
import { Router } from '@angular/router';
import { CaptchaTokenResponseDTO } from '../models/captcha-token-dto.model';
import { CaptchaTextResponseDTO } from '../models/captcha-text-response-dto.model';
import { CaptchaTextRequestAnswerDTO } from '../models/captcha-text-request-answer-dto.model';

@Component({
  selector: 'app-captcha',
  standalone: false,
  templateUrl: './captcha.component.html',
  styleUrls: ['./captcha.component.css']
})

export class CaptchaComponent implements OnInit{

  captcha?: CaptchaTextResponseDTO;
  token!: CaptchaTokenResponseDTO;
  alertMessage = '';
  showAlert = false;
  isBanned = false;
  answerWords: string[] = [];

  words: string[] = [];
  selectedWordIndices: Set<number> = new Set();
  isVerifying = false;
  verificationResult?: {success:boolean, message: string}

  constructor(private captchaService: CaptchaService, private router: Router){}
  
  ngOnInit(): void {
    this.captchaService.generateToken().subscribe({
      next: (res) => {
        this.token = res,
        this.loadCaptchaWithId();
      },
      error: (err) => {
        if(err.status === 403){
          this.alertMessage = "Çok fazla deneme yaptınız, daha sonra tekrar deneyin.";
          this.showAlert = true;
          this.isBanned = true;
        }
        console.error('Token alınamadı:', err);
      }
    })
  }

  onAlertClosed() {
    this.showAlert = false;
    this.router.navigate(['/dashboard']);
  }

  loadCaptchaWithId(){
    this.captchaService.getCaptchaWithId(this.token.captchaTextId).subscribe({
      next: (res) => {
        this.captcha = res
        this.prepareTextForSelection();
      },
      error: (err) => console.error('Id ile captcha alınamadı:', err)
    });
  }

  prepareTextForSelection(){
    if(this.captcha?.captchaTextContent){
      this.words = this.captcha.captchaTextContent
        .split(/\s+/)
        .filter(word => word.trim().length > 0);
    }
  }

  toggleWordSelection(word: string, index: number){
    const cleanWord = word.replace(/[.,!?;:]/, '');

    if(this.selectedWordIndices.has(index)){
      this.selectedWordIndices.delete(index);
      this.answerWords = this.answerWords?.filter(answer => answer !== cleanWord);
    }else{
      this.selectedWordIndices.add(index);
      if(!this.answerWords?.includes(cleanWord)){
        this.answerWords?.push(cleanWord);
      }
    }
  }

  isWordSelected(word: string, index: number): boolean {
    return this.selectedWordIndices.has(index);
  }

  removeSelectedWord(wordToRemove: string) {
    this.answerWords = this.answerWords?.filter(word => word !== wordToRemove);
    
    // Kelime seçimini de kaldır
    this.words.forEach((word, index) => {
      if (word.replace(/[.,!?;:]/, '') === wordToRemove) {
        this.selectedWordIndices.delete(index);
      }
    });
  }

  verifyCaptchaAnswer() {
    if (!this.captcha) {
      alert('Soru yüklenmeden cevaplama yapılamaz.');
      return;
    }

    if (this.answerWords?.length === 0) {
      alert('Lütfen en az bir kelime seçin.');
      return;
    }

    this.isVerifying = true;
    this.verificationResult = undefined;

    const requestDto: CaptchaTextRequestAnswerDTO = {
      answerWords: this.answerWords,
      tokenId: this.token.token
    };

    this.captchaService.verifyCaptchaAnswer(requestDto).subscribe({
      next: (response) => {
        this.verificationResult = response;
        this.isVerifying = false;
        
        if (response.success) {
          // Başarılı doğrulama sonrası yönlendirme
          setTimeout(() => {
            this.router.navigate(['/dashboard']);
          }, 2000);
        }
      },
      error: (err) => {
        console.error('Captcha doğrulama hatası:', err);
        this.verificationResult = {
          success: false,
          message: 'Doğrulama sırasında bir hata oluştu.'
        };
        this.isVerifying = false;
      }
    });
  }

  resetCaptcha() {
    this.answerWords = [];
    this.selectedWordIndices.clear();
    this.verificationResult = undefined;
    this.ngOnInit(); // Yeni captcha yükle
  }
 
}
