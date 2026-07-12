export interface PurchaseBillLine {
  item: string;
  batch: string;
  standardCost: number;
  standardPrice: number;
  quantity: number;
  discountPercentage: number;
}

export interface PurchaseBillLineResult extends PurchaseBillLine {
  totalCost: number;
  totalSelling: number;
}

export interface PurchaseBillSummary {
  totalItems: number;
  totalQuantity: number;
  totalCost: number;
  totalSelling: number;
}
