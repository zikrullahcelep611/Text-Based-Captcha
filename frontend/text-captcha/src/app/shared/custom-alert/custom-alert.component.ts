import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-custom-alert',
  templateUrl: './custom-alert.component.html',
  styleUrls: ['./custom-alert.component.css'],
  standalone: false
})
export class CustomAlertComponent {
  @Input() message: string = '';
  @Input() show: boolean = false;
  @Output() closed = new EventEmitter<void>();

  closeAlert() {
    this.closed.emit();
  }
}
