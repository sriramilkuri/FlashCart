import {
  authenticatedFetch,
} from "./AuthenticatedCLient";

const CART_API_URL =
  process.env.NEXT_PUBLIC_CART_API_URL!;

export type CartItem = {
  cartItemId: number;
  productId: number;
  quantity: number;
};

export type Cart = {
  cartId: number;
  userId: number;
  updatedAt: string;
  items: CartItem[];
};

export type AddCartItemRequest = {
  productId: number;
  quantity: number;
};

export type UpdateCartItemRequest = {
  quantity: number;
};

export async function getCart(
  token: string
): Promise<Cart> {
  return authenticatedFetch<Cart>(
    `${CART_API_URL}/api/Cart`,
    token
  );
}

export async function addToCart(
  token: string,
  request: AddCartItemRequest
): Promise<{ message: string }> {
  return authenticatedFetch<{ message: string }>(
    `${CART_API_URL}/api/Cart/items`,
    token,
    {
      method: "POST",
      body: JSON.stringify(request),
    }
  );
}

export async function updateCartItem(
  token: string,
  productId: number,
  request: UpdateCartItemRequest
): Promise<{ message: string }> {
  return authenticatedFetch<{ message: string }>(
    `${CART_API_URL}/api/Cart/items/${productId}`,
    token,
    {
      method: "PUT",
      body: JSON.stringify(request),
    }
  );
}

export async function removeCartItem(
  token: string,
  productId: number
): Promise<{ message: string }> {
  return authenticatedFetch<{ message: string }>(
    `${CART_API_URL}/api/Cart/items/${productId}`,
    token,
    {
      method: "DELETE",
    }
  );
}

export async function clearCart(
  token: string
): Promise<{ message: string }> {
  return authenticatedFetch<{ message: string }>(
    `${CART_API_URL}/api/Cart`,
    token,
    {
      method: "DELETE",
    }
  );
}