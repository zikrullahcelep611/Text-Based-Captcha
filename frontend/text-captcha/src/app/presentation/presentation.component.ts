import { HttpClient } from '@angular/common/http';
import { Component, Injectable, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { QuestionResponseDTO, QuestionService } from '../services/question.service';

@Component({
  selector: 'app-presentation',
  templateUrl: './presentation.component.html',
  styleUrls: ['./presentation.component.css'],
  standalone: false
})


export class PresentationComponent implements OnInit{
  
    question!: QuestionResponseDTO;
    selectedOptionId: number | null = null;
    
    constructor(private questionService: QuestionService) {}

    ngOnInit(): void {
      this.loadQuestion();
    }


    loadQuestion() {
      this.questionService.getQuestion().subscribe({
      next: (res) => this.question = res,
      error: (err) => console.error('Soru alınamadı:', err)
      });
    }

    selectOption(optionId: number) {
      this.selectedOptionId = optionId;
      console.log('Seçilen Şık:', optionId);
      // Eğer cevabı kontrol etmek istersen:
      const selectedOption = this.question.options.find(opt => opt.optionId === optionId);
      if (selectedOption?.isCorrect) {
        alert('Doğru cevap!');
        } else {
        alert('Yanlış cevap!');
      }
    }  
}
