import {
  ChangeDetectionStrategy,
  Component,
  computed,
  ElementRef,
  HostListener,
  inject,
  input,
  model,
  signal,
} from '@angular/core';

/**
 * Lightweight, reusable text autocomplete driven by a fixed option list.
 * Two-way binds the selected text via `value` and filters options as the
 * user types. Supports keyboard navigation (arrows / enter / escape).
 */
@Component({
  selector: 'app-autocomplete',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './autocomplete.html',
  styleUrl: './autocomplete.scss',
})
export class AutocompleteComponent {
  private readonly host = inject(ElementRef<HTMLElement>);

  readonly options = input<string[]>([]);
  readonly placeholder = input<string>('');
  readonly inputId = input<string>('');
  readonly invalid = input<boolean>(false);

  readonly value = model<string>('');

  protected readonly open = signal(false);
  protected readonly activeIndex = signal(-1);

  protected readonly filtered = computed(() => {
    const query = this.value().trim().toLowerCase();
    const options = this.options();
    if (!query) {
      return options;
    }
    return options.filter((o) => o.toLowerCase().includes(query));
  });

  protected onInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value.set(target.value);
    this.open.set(true);
    this.activeIndex.set(-1);
  }

  protected onFocus(): void {
    this.open.set(true);
  }

  protected select(option: string): void {
    this.value.set(option);
    this.open.set(false);
    this.activeIndex.set(-1);
  }

  protected onKeydown(event: KeyboardEvent): void {
    const items = this.filtered();

    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.open.set(true);
        this.activeIndex.set(Math.min(this.activeIndex() + 1, items.length - 1));
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.activeIndex.set(Math.max(this.activeIndex() - 1, 0));
        break;
      case 'Enter': {
        const index = this.activeIndex();
        if (this.open() && index >= 0 && index < items.length) {
          event.preventDefault();
          this.select(items[index]);
        }
        break;
      }
      case 'Escape':
        this.open.set(false);
        break;
    }
  }

  @HostListener('document:click', ['$event'])
  protected onDocumentClick(event: MouseEvent): void {
    if (!this.host.nativeElement.contains(event.target)) {
      this.open.set(false);
    }
  }
}
