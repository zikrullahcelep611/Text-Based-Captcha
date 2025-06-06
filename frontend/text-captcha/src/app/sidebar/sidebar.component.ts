import { Component } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  standalone: false,
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  isSidebarOpen = false;

  toggleSidebar(){
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  closeSidebar(){
    this.isSidebarOpen = false;
  }
}
