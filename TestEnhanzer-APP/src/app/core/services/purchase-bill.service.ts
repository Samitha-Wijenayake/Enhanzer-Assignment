import { Injectable } from '@angular/core';
import {
  PurchaseBillLine,
  PurchaseBillLineResult,
  PurchaseBillSummary,
} from '../models/purchase-bill.model';

/**
 * Encapsulates the purchase-bill calculation rules from the assignment.
 * The same formulas are implemented on the backend; keeping a client-side
 * copy gives instant feedback without a round-trip per line.
 */
@Injectable({ providedIn: 'root' })
export class PurchaseBillService {
  calculateLine(line: PurchaseBillLine): PurchaseBillLineResult {
    const grossCost = line.standardCost * line.quantity;
    const discountFactor = 1 - line.discountPercentage / 100;
    const totalCost = this.round(grossCost * discountFactor);
    const totalSelling = this.round(line.standardPrice * line.quantity);

    return { ...line, totalCost, totalSelling };
  }

  summarize(lines: PurchaseBillLineResult[]): PurchaseBillSummary {
    return {
      totalItems: lines.length,
      totalQuantity: lines.reduce((sum, l) => sum + l.quantity, 0),
      totalCost: this.round(lines.reduce((sum, l) => sum + l.totalCost, 0)),
      totalSelling: this.round(lines.reduce((sum, l) => sum + l.totalSelling, 0)),
    };
  }

  private round(value: number): number {
    return Math.round((value + Number.EPSILON) * 100) / 100;
  }
}
