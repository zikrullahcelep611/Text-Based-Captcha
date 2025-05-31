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

  private azureKey = environment.azureKey;
  private azureRegion = environment.azureRegion;

  constructor(private questionService: QuestionService){}

  //component açılır açılmazilk çalışacak metottur.
  ngOnInit(): void {
    this.questionService.generateToken().subscribe({
      next: (res) => {
        this.token = res,
        this.loadQuestionWithId();
      },
      error: (err) => console.error('Token alınamadı:', err)
    });
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

  getToken(){
    this.questionService.generateToken().subscribe({
      next: (res) => this.token = res,
      error: (err) => console.error('Token alınamadı:', err)
    });
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
      const audioConfig = SpeechSDK.AudioConfig.fromDefaultSpeakerOutput();
      const synthesizer = new SpeechSDK.SpeechSynthesizer(speechConfig, audioConfig);

      synthesizer.speakTextAsync(
        text,
        result => {
          synthesizer.close();
          resolve();
        },
        error => {
          console.error('Azure TTS Hatası:', error);
          synthesizer.close();
          reject(error);
        }
      );
    });
  }

  async readQuestionAndOptions(){
    if(!this.question) return;

    // Eğer konuşma devam ediyorsa, işlemi durdur
    if(this.isSpeaking) return;

    try{
      this.isSpeaking = true;
      
      // Önce soruyu oku ve bitirmesini bekle
      await this.speakWithAzure(this.question.questionText);
      
      // Soru bittikten sonra 1 saniye bekle
      await new Promise(resolve => setTimeout(resolve, 5000));

      // Şıkları sırayla oku
      for(let i = 0; i < this.question.options.length; i++){
        const option = this.question.options[i];
        // Her şık için await kullan - bir şık bitene kadar diğerine geçme
        await this.speakWithAzure(`Şık ${i + 1}: ${option.optionText}`);
        // Şıklar arası kısa bir duraklama
        await new Promise(resolve => setTimeout(resolve, 800));
      }
    }catch(error){
      console.error('Azure ile sesli okuma sırasında hata:', error);
    }finally{
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
