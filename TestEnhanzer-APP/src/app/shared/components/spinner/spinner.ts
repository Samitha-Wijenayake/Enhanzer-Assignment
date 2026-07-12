import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-spinner',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <span class="spinner" [class.spinner--inline]="inline()" [attr.aria-label]="label()" role="status">
      <span class="spinner__circle"></span>
      @if (!inline()) {
        <span class="spinner__label">{{ label() }}</span>
      }
    </span>
  `,
  styleUrl: './spinner.scss',
})
export class SpinnerComponent {
  readonly inline = input<boolean>(false);
  readonly label = input<string>('Loading…');
}
