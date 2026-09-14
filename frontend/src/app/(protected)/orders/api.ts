
import { OrderDetails, OrderHistory } from "./types";

import { getAccessToken } from "@/app/lib/auth";

const API_BASE_URL = process.env.API_URL;


export async function getOrders(): Promise<OrderHistory[]> {

const token = await getAccessToken();

  const response = await fetch(
    `${API_BASE_URL}/api/Orders`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    }
  );

  if (!response.ok) {
   if (!response.ok) {
  const errorBody = await response.text();

  console.log("Orders API status:", response.status);
  console.log("Orders API response:", errorBody);

  throw new Error(
    `Failed to fetch orders. Status: ${response.status}`
  );
}
  }

  return response.json();
}

export async function getOrderDetails(
  orderId: number
): Promise<OrderDetails> {
const token = await getAccessToken();

console.log(orderId, "orderId inside");
  const response = await fetch(
    `${API_BASE_URL}/api/orders/${orderId}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    }
  );
console.log("response", response);
  if (!response.ok) {
    if (response.status === 404) {
      throw new Error("ORDER_NOT_FOUND");
    }

    throw new Error("Failed to fetch order details.");
  }

  return response.json();
}