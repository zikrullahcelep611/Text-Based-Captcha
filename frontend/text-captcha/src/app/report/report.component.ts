import { NgClass } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { QuestionService } from '../services/question.service';

@Component({
  selector: 'app-report',
  standalone: false,
  templateUrl: './report.component.html',
  styleUrl: './report.component.css'
})
export class ReportComponent implements OnInit{
  
  bannedIps?: string[];
  noBannedIps = false

  constructor(private questionService: QuestionService){}
  
  ngOnInit(): void {

    console.log('burasi calisti');

    this.questionService.getBannedIpAddresses().subscribe({
      next: (res) =>{
        this.bannedIps = res,
        this.noBannedIps = false;
      },error: (err) => {
        if(err.status === 404){
          this.noBannedIps = true;
        }
      }
    })
  }
}
