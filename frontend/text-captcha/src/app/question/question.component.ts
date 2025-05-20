import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { QuestionService } from '../services/question.service';
import { CreateQuestionDTO } from '../models/question.model';
@Component({
  selector: 'app-question-form',
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css'],
  standalone: false
})

export class QuestionComponent {
  questionForm: FormGroup;

  constructor(private fb: FormBuilder, private questionService: QuestionService) {
    this.questionForm = this.fb.group({
      questionText: ['', Validators.required],
      options: this.fb.array([...Array(5)].map(() =>
        this.fb.group({
          optionText: ['', Validators.required],
          isCorrect: [false]
        })
      ))
    });
  }

  get options(): FormArray {
    return this.questionForm.get('options') as FormArray;
  }

  onSubmit(): void {
  console.log('onSubmit çalıştı');
  if (this.questionForm.valid) {
    // Form değerlerini kontrol edin
    console.log('Form değerleri:', this.questionForm.value);
    
    // API'ye gönderilecek modeli oluşturun
    const questionData: CreateQuestionDTO = {
      questionText: this.questionForm.value.questionText,
      options: this.questionForm.value.options.map((option: { optionText: any; isCorrect: any; }) => ({
        optionText: option.optionText,
        isCorrect: Boolean(option.isCorrect) // Boolean değere çevirin
      }))
    };
    
    console.log('API\'ye gönderilecek veri:', questionData);
    
    this.questionService.createQuestion(questionData).subscribe({
      next: () => alert('Soru başarıyla eklendi'),
      error: err => {
        console.error('Hata:', err);
        alert(`Hata: ${err.error || err.message}`);
      }
    });
  }
}

  setCorrectOption(index: number): void {
    this.options.controls.forEach((control, i) => {
      control.get('isCorrect')?.setValue(i === index);
    });
  }
}
