import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-alert',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="alert alert-error" *ngIf="show">
      <div class="alert-icon">⚠️</div>
      <div class="alert-content">
        <h4 *ngIf="title">{{ title }}</h4>
        <p>{{ message }}</p>
        <ul *ngIf="errors && errors.length > 0">
          <li *ngFor="let error of errors">{{ error }}</li>
        </ul>
      </div>
      <button class="alert-close" (click)="onClose()" *ngIf="dismissible">×</button>
    </div>
  `,
  styles: [`
    .alert {
      display: flex;
      align-items: flex-start;
      gap: 1rem;
      padding: 1rem;
      border-radius: 8px;
      margin-bottom: 1rem;
      border: 1px solid;
    }

    .alert-error {
      background-color: #fee;
      border-color: #fcc;
      color: #c66;
    }

    .alert-icon {
      font-size: 1.5rem;
      flex-shrink: 0;
    }

    .alert-content {
      flex: 1;
    }

    .alert-content h4 {
      margin: 0 0 0.5rem 0;
      font-size: 1rem;
      font-weight: 600;
    }

    .alert-content p {
      margin: 0 0 0.5rem 0;
    }

    .alert-content ul {
      margin: 0;
      padding-left: 1.5rem;
    }

    .alert-content li {
      margin-bottom: 0.25rem;
    }

    .alert-close {
      background: none;
      border: none;
      font-size: 1.5rem;
      cursor: pointer;
      color: inherit;
      padding: 0;
      width: 24px;
      height: 24px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .alert-close:hover {
      opacity: 0.7;
    }
  `]
})
export class ErrorAlertComponent {
  @Input() show = false;
  @Input() title = 'Error';
  @Input() message = '';
  @Input() errors: string[] = [];
  @Input() dismissible = true;

  onClose(): void {
    this.show = false;
  }
}