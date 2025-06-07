import { Component } from '@angular/core';
import { CaptchaService } from '../services/captcha.service';
import { CreateCaptchaTextDTO } from '../models/create-text-captcha-dto.model';

@Component({
  selector: 'app-create-captcha',
  standalone: false,
  templateUrl: './create-captcha.component.html',
  styleUrls: ['./create-captcha.component.css']
})

export class CreateCaptchaComponent {

 captchaData: CreateCaptchaTextDTO = {
    questionText: '',
    contentText: '',
    answerWords: []
  };

  words: string[] = [];
  selectedWordIndices: Set<number> = new Set();
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  constructor(private captchaService: CaptchaService) {}

  onContentTextChange() {
    // Metni kelimelere böl
    this.words = this.captchaData.contentText
      .split(/\s+/)
      .filter(word => word.trim().length > 0);
    
    // Seçimleri temizle
    this.selectedWordIndices.clear();
    this.captchaData.answerWords = [];
    console.log('words', this.words);
  }

  toggleWordSelection(word: string, index: number) {
    const cleanWord = word.replace(/[.,!?;:]/, ''); // Noktalama işaretlerini temizle
    
    if (this.selectedWordIndices.has(index)) {
      // Seçimi kaldır
      this.selectedWordIndices.delete(index);
      this.captchaData.answerWords = this.captchaData.answerWords
        .filter(answer => answer !== cleanWord);
    } else {
      // Seçim ekle
      this.selectedWordIndices.add(index);
      if (!this.captchaData.answerWords.includes(cleanWord)) {
        this.captchaData.answerWords.push(cleanWord);
      }
    }
  }

  isWordSelected(word: string, index: number): boolean {
    return this.selectedWordIndices.has(index);
  }

  removeAnswer(index: number) {
    const removedWord = this.captchaData.answerWords[index];
    this.captchaData.answerWords.splice(index, 1);
    
    // Kelime seçimini de kaldır
    this.words.forEach((word, wordIndex) => {
      if (word.replace(/[.,!?;:]/, '') === removedWord) {
        this.selectedWordIndices.delete(wordIndex);
      }
    });
  }

  onSubmit() {
    if (this.captchaData.answerWords.length === 0) {
      this.errorMessage = 'En az bir doğru cevap seçmelisiniz!';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.captchaService.createCaptcha(this.captchaData).subscribe({
      next: () => {
        this.successMessage = 'CAPTCHA başarıyla oluşturuldu!';
        this.resetForm();
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'CAPTCHA oluşturulurken bir hata oluştu!';
        console.error('Error creating captcha:', error);
        this.isLoading = false;
      }
    });
  }
 
  resetForm() {
    this.captchaData = {
      questionText: '',
      contentText: '',
      answerWords: []
    };
    this.words = [];
    this.selectedWordIndices.clear();
    
    // 3 saniye sonra başarı mesajını temizle
    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
  }

}
