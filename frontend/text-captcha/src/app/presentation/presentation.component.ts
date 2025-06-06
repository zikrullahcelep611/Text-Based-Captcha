import { Component, OnInit } from "@angular/core";
import { QuestionComponent } from "../question/question.component";
import { QuestionResponseDTO, QuestionService } from "../services/question.service";
import { resolve } from "path";
import { rejects } from "assert";
import * as SpeechSDK from 'microsoft-cognitiveservices-speech-sdk';
import { error } from "console";
import { environment } from "../../environments/environment";
import { CaptchaRequestAnswerDTO } from "../models/captcha-answer.model";
import { CaptchaTokenResponse } from "../models/captcha-token.model";
import { generate } from "rxjs";
import { CaptchaVerificationResponse } from "../models/captcha-verification-model";
import { Router } from '@angular/router';


@Component({
  selector: 'app-presentation',
  templateUrl: './presentation.component.html',
  styleUrls: ['./presentation.component.css'],
  standalone: false
})

export class PresentationComponent implements OnInit{
  question!: QuestionResponseDTO;
  selectedOptionId: number | null = null;
  isSpeaking: boolean = false;
  token!: CaptchaTokenResponse;
  captchaVerificationResponse!: CaptchaVerificationResponse;
  showAlert = false;
  alertMessage = '';
  isBanned = false;

  private azureKey = environment.azureKey;
  private azureRegion = environment.azureRegion;

  constructor(private questionService: QuestionService, private router: Router){}

  ngOnInit(): void {
    this.questionService.generateToken().subscribe({
      next: (res) => {
        this.token = res,
        this.loadQuestionWithId();
      },
      error: (err) => {
        if(err.status === 403){
          this.alertMessage = "Çok fazla deneme yaptınız, daha sonra tekrar deneyin.";
          this.showAlert = true;
          this.isBanned = true
        }
        console.error('Token alınamadı:', err)
      }
    });
  }

  onAlertClosed() {
    this.showAlert = false;
    this.router.navigate(['/dashboard']);
  }

  loadQuestion(){
    this.questionService.getQuestion().subscribe({
      next:(res) => this.question = res,
      error: (err) => console.error('Soru alınamadı:', err)
    });
  }

  loadQuestionWithId(){
    this.questionService.getQuestionWithId(this.token.questionId).subscribe({
      next: (res) => this.question = res,
      error: (err) => console.error('Id ile soru alınamadı:', err)
    });
  }

  selectOption(optionId: number){
    this.selectedOptionId = optionId;
    // const selectedOption = this.question.options.find(opt => opt.optionId === optionId);
    // if(selectedOption?.isCorrect){
    //   alert('Doğru cevap');
    // }
    // else{
    //   alert('Yanlış cevap');
    // }
  }

  getSelectedOption(optionId: number){
    this.selectedOptionId = optionId;
    const selectedOption = this.question.options.find(opt => opt.optionId === optionId);

    return selectedOption;
  }


  verifyCaptchaAnswer(){
    if(!this.question){
      alert('Soru yüklenmeden cevaplama yapılamaz.');
      return;
    }
    const selectedOption = this.getSelectedOption(this.selectedOptionId!);
    const captchaAnswerRequestDto: CaptchaRequestAnswerDTO = {
      tokenId: this.token.token,
      answer: selectedOption ? selectedOption.optionText : ''
    };

    //Burayi yeniden dusunecegim.
    this.questionService.verifyCaptchaAnswer(captchaAnswerRequestDto).subscribe({
      next: (res) => {
        this.captchaVerificationResponse = res;
        alert(res.message);
      },
      error: (err) => console.error("Backend tarafında cevap doğrulama yapılamadı.", err)
    });
  }
  
  speakWithAzure(text: string): Promise<void> {
      return new Promise((resolve, reject) => {
          const speechConfig = SpeechSDK.SpeechConfig.fromSubscription(this.azureKey, this.azureRegion);
          speechConfig.speechSynthesisLanguage = 'tr-TR';
          speechConfig.speechSynthesisVoiceName = 'tr-TR-EmelNeural';
          
          const audioConfig = SpeechSDK.AudioConfig.fromDefaultSpeakerOutput();
          const synthesizer = new SpeechSDK.SpeechSynthesizer(speechConfig, audioConfig);

          synthesizer.speakTextAsync(
              text,
              result => {
                  if (result.reason === SpeechSDK.ResultReason.SynthesizingAudioCompleted) {
                      synthesizer.close();
                      resolve();
                  } else {
                      console.error('Konuşma tamamlanmadı:', result.errorDetails);
                      synthesizer.close();
                      reject(new Error(result.errorDetails));
                  }
              },
              error => {
                  console.error('Azure TTS Hatası:', error);
                  synthesizer.close();
                  reject(error);
              }
          );
      });
  }


  async readQuestionAndOptionsAsOneText(){
    if(!this.question){
      console.warn('Soru bulunamadı');
      return;
    }

    if(this.isSpeaking){
      console.warn('Konuşma zaten devam ediyor.');
      return;
    }

    try{
      this.isSpeaking = true;

      let fullText = this.question.questionText;

      fullText += ". ";

      for(let i = 0; i < this.question.options.length; i++){
        const option = this.question.options[i];
        const optionLetter = String.fromCharCode(65 + i);

        fullText += `Şık ${i + 1}: ${option.optionText}.`;
      }

      console.log('Okucak tam metin:', fullText);

      await this.speakWithAzure(fullText);
    }catch(error){
      console.error('Sesli okuma sırasında hata:', error);
    } finally {
        this.isSpeaking = false;
    }
  }

  listenWithAzure(){
    const speechConfig = SpeechSDK.SpeechConfig.fromSubscription(this.azureKey, this.azureRegion);
    speechConfig.speechRecognitionLanguage = 'tr-TR';
    const audioConfig = SpeechSDK.AudioConfig.fromDefaultMicrophoneInput();
    const recognizer = new SpeechSDK.SpeechRecognizer(speechConfig, audioConfig);

    recognizer.recognizeOnceAsync(result => {
      if(result.reason === SpeechSDK.ResultReason.RecognizedSpeech){
        this.checkAnswer(result.text);
      }else{
        alert('Ses algılanamadı.');
      }

      recognizer.close();
    }, error => {
      console.error('Azure STT hatası', error);
      recognizer.close();
    });
  }

  checkAnswer(userAnswer: string) {
    if (!this.question) return;
    const normalizedAnswer = userAnswer.toLowerCase().trim();
    const found = this.question.options.find((opt, idx) => {
      const optionNumber = (idx + 1).toString();
      const optionText = opt.optionText.toLowerCase();
      return normalizedAnswer.includes(optionText) ||
             normalizedAnswer.includes(optionNumber) ||
             normalizedAnswer.includes(`şık ${optionNumber}`) ||
             (optionNumber === '1' && normalizedAnswer.includes('bir')) ||
             (optionNumber === '2' && normalizedAnswer.includes('iki')) ||
             (optionNumber === '3' && normalizedAnswer.includes('üç')) ||
             (optionNumber === '4' && normalizedAnswer.includes('dört'));
    });

    if (found) {
      this.selectedOptionId = found.optionId;
      if (found.isCorrect) {
        this.speakWithAzure('Tebrikler, doğru cevap!');
      } else {
        this.speakWithAzure('Maalesef, yanlış cevap.');
      }
    } else {
      this.speakWithAzure('Cevabınızı anlayamadım, lütfen tekrar deneyin.');
    }
  }
}
