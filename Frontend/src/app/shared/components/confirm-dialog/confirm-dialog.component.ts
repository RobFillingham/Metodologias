import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="modal-backdrop" *ngIf="show" (click)="onBackdropClick($event)">
      <div class="modal-content">
        <div class="modal-header">
          <h3>{{ title }}</h3>
        </div>
        <div class="modal-body">
          <p>{{ message }}</p>
        </div>
        <div class="modal-footer">
          <button
            class="btn btn-secondary"
            (click)="onCancel()"
            [disabled]="loading"
          >
            {{ cancelText }}
          </button>
          <button
            class="btn btn-danger"
            (click)="onConfirm()"
            [disabled]="loading"
          >
            <span *ngIf="loading">Processing...</span>
            <span *ngIf="!loading">{{ confirmText }}</span>
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-backdrop {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }

    .modal-content {
      background: white;
      border-radius: 12px;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
      max-width: 400px;
      width: 90%;
      max-height: 90vh;
      overflow-y: auto;
    }

    .modal-header {
      padding: 1.5rem 1.5rem 1rem;
      border-bottom: 1px solid #e1e5e9;
    }

    .modal-header h3 {
      margin: 0;
      color: #333;
      font-size: 1.25rem;
    }

    .modal-body {
      padding: 1.5rem;
    }

    .modal-body p {
      margin: 0;
      color: #666;
      line-height: 1.5;
    }

    .modal-footer {
      padding: 1rem 1.5rem 1.5rem;
      border-top: 1px solid #e1e5e9;
      display: flex;
      gap: 0.75rem;
      justify-content: flex-end;
    }

    .btn {
      padding: 0.5rem 1rem;
      border: 2px solid transparent;
      border-radius: 6px;
      font-size: 0.9rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.3s ease;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
      border-color: #6c757d;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #5a6268;
      border-color: #5a6268;
    }

    .btn-danger {
      background: #dc3545;
      color: white;
      border-color: #dc3545;
    }

    .btn-danger:hover:not(:disabled) {
      background: #c82333;
      border-color: #c82333;
    }
  `]
})
export class ConfirmDialogComponent {
  @Input() show = false;
  @Input() title = 'Confirm Action';
  @Input() message = 'Are you sure you want to proceed?';
  @Input() confirmText = 'Confirm';
  @Input() cancelText = 'Cancel';
  @Input() loading = false;

  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  onConfirm(): void {
    this.confirm.emit();
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onBackdropClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.onCancel();
    }
  }
}