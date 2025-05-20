import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  standalone: false
})
export class DashboardComponent {
  isSidebarOpen = false; // Sidebar’ın açık/kapalı durumunu kontrol eder

  toggleSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
}