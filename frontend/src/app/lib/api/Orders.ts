import {
  authenticatedFetch,
} from "./AuthenticatedCLient";

const ORDER_API_URL =
  process.env.NEXT_PUBLIC_ORDER_API_URL!;

export type CreateOrderItemRequest = {
  productId: number;
  quantity: number;
};

export type CreateOrderRequest = {
  items: CreateOrderItemRequest[];
};

export type OrderItem = {
  productId: number;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
};

export type Order = {
  orderId: number;
  userId: number;
  orderDate: string;
  total: number;
  status: string;
  items: OrderItem[];
};

export async function createOrder(
  token: string,
  request: CreateOrderRequest
): Promise<Order> {
  return authenticatedFetch<Order>(
    `${ORDER_API_URL}/api/Orders`,
    token,
    {
      method: "POST",
      body: JSON.stringify(request),
    }
  );
}

export async function getOrders(
  token: string
): Promise<Order[]> {
  return authenticatedFetch<Order[]>(
    `${ORDER_API_URL}/api/Orders`,
    token
  );
}

export async function getOrder(
  token: string,
  orderId: number
): Promise<Order> {
  return authenticatedFetch<Order>(
    `${ORDER_API_URL}/api/Orders/${orderId}`,
    token
  );
}

export async function cancelOrder(
  token: string,
  orderId: number
): Promise<{ message: string }> {
  return authenticatedFetch<{ message: string }>(
    `${ORDER_API_URL}/api/Orders/${orderId}/cancel`,
    token,
    {
      method: "POST",
    }
  );
}