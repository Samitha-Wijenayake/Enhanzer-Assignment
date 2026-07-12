import { ChangeDetectionStrategy, Component, input } from '@angular/core';

export type AlertVariant = 'error' | 'success' | 'info' | 'warning';

@Component({
  selector: 'app-alert',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="alert" [class]="'alert--' + variant()" role="alert">
      <span class="alert__icon" aria-hidden="true">{{ icon }}</span>
      <span class="alert__message">{{ message() }}</span>
    </div>
  `,
  styleUrl: './alert.scss',
})
export class AlertComponent {
  readonly variant = input<AlertVariant>('info');
  readonly message = input<string>('');

  get icon(): string {
    switch (this.variant()) {
      case 'error':
        return '⚠';
      case 'success':
        return '✓';
      case 'warning':
        return '!';
      default:
        return 'ℹ';
    }
  }
}
