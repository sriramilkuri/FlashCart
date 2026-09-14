export interface OrderHistoryItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}

export interface OrderHistory {
  orderId: number;
  orderDate: string;
  total: number;
  status: string;
  items: OrderHistoryItem[];
}

export interface OrderDetailsItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface OrderDetails {
  orderId: number;
  orderDate: string;
  total: number;
  status: string;
  items: OrderDetailsItem[];
}