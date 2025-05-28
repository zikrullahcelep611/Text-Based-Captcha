import { Component } from '@angular/core';
import { Router } from '@angular/router';


@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})

export class HomeComponent {
  constructor(private router: Router){}

  goToPresentationComponent(){
    this.router.navigate(['/presentation']);
  }

  goToLoginComponent(){
    this.router.navigate(['/login']);
  }

  goToDashboardComponent(){
    this.router.navigate(['/dashboard']);
  }
}
