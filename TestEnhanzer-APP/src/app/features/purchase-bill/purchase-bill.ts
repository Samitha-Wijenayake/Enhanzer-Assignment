import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';

import { AuthService } from '../../core/services/auth.service';
import { ItemService } from '../../core/services/item.service';
import { LocationService } from '../../core/services/location.service';
import { PurchaseBillService } from '../../core/services/purchase-bill.service';
import { LocationDto } from '../../core/models/location.model';
import { PurchaseBillLineResult } from '../../core/models/purchase-bill.model';
import { AutocompleteComponent } from '../../shared/components/autocomplete/autocomplete';
import { SpinnerComponent } from '../../shared/components/spinner/spinner';
import { AlertComponent } from '../../shared/components/alert/alert';

@Component({
  selector: 'app-purchase-bill',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, DecimalPipe, AutocompleteComponent, SpinnerComponent, AlertComponent],
  templateUrl: './purchase-bill.html',
  styleUrl: './purchase-bill.scss',
})
export class PurchaseBillComponent {
  private readonly fb = inject(FormBuilder);
  private readonly itemService = inject(ItemService);
  private readonly locationService = inject(LocationService);
  private readonly billService = inject(PurchaseBillService);
  private readonly auth = inject(AuthService);

  protected readonly items = signal<string[]>([]);
  protected readonly locations = signal<LocationDto[]>([]);
  protected readonly lines = signal<PurchaseBillLineResult[]>([]);

  protected readonly loadingItems = signal(true);
  protected readonly loadingLocations = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly formError = signal<string | null>(null);

  protected readonly itemValue = signal('');
  protected readonly itemTouched = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    batch: ['', [Validators.required]],
    standardCost: [0, [Validators.required, Validators.min(0)]],
    standardPrice: [0, [Validators.required, Validators.min(0)]],
    quantity: [1, [Validators.required, Validators.min(1)]],
    discountPercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
  });

  private readonly formValue = toSignal(this.form.valueChanges, {
    initialValue: this.form.getRawValue(),
  });

  /** Live preview of the totals for the line currently being entered. */
  protected readonly preview = computed(() => {
    const v = this.formValue();
    return this.billService.calculateLine({
      item: this.itemValue(),
      batch: v.batch ?? '',
      standardCost: Number(v.standardCost) || 0,
      standardPrice: Number(v.standardPrice) || 0,
      quantity: Number(v.quantity) || 0,
      discountPercentage: Number(v.discountPercentage) || 0,
    });
  });

  protected readonly summary = computed(() => this.billService.summarize(this.lines()));

  constructor() {
    this.loadItems();
    this.loadLocations();
  }

  protected addLine(): void {
    this.formError.set(null);
    this.itemTouched.set(true);

    const itemInvalid = !this.itemValue().trim();
    if (itemInvalid || this.form.invalid) {
      this.form.markAllAsTouched();
      this.formError.set('Please complete all required fields before adding the item.');
      return;
    }

    const line = this.billService.calculateLine({
      item: this.itemValue().trim(),
      ...this.form.getRawValue(),
    });

    this.lines.update((current) => [...current, line]);
    this.resetLineForm();
  }

  protected removeLine(index: number): void {
    this.lines.update((current) => current.filter((_, i) => i !== index));
  }

  protected clearAll(): void {
    this.lines.set([]);
  }

  protected onItemChange(value: string): void {
    this.itemValue.set(value);
    this.itemTouched.set(true);
  }

  private resetLineForm(): void {
    this.itemValue.set('');
    this.itemTouched.set(false);
    this.form.reset({
      batch: this.form.controls.batch.value,
      standardCost: 0,
      standardPrice: 0,
      quantity: 1,
      discountPercentage: 0,
    });
  }

  private loadItems(): void {
    this.loadingItems.set(true);
    this.itemService.getItems().subscribe({
      next: (items) => {
        this.items.set(items);
        this.loadingItems.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loadingItems.set(false);
        this.loadError.set(this.resolveError(err));
      },
    });
  }

  private loadLocations(): void {
    this.loadingLocations.set(true);
    this.locationService.getLocations().subscribe({
      next: (locations) => {
        // Fall back to the locations captured at login if the DB call is empty.
        this.locations.set(locations );
        this.loadingLocations.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loadingLocations.set(false);
      },
    });
  }

  private resolveError(err: HttpErrorResponse): string {
    if (err.status === 0) {
      return 'Cannot reach the server. Please make sure the API is running.';
    }
    return err.error?.message ?? 'Failed to load data. Please try again.';
  }
}
