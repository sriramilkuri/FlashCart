import { apiFetch } from "./client";

const PRODUCT_API_URL =
  process.env.NEXT_PUBLIC_PRODUCT_API_URL!;

export type Product = {
  id: number;
  name: string;
  price: number;
  description: string;
  categoryId: number;
};

export type ProductCursorResponse = {
  items: Product[];
  nextCursor: number | null;
  previousCursor: number | null;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  pageSize: number;
};

export type GetProductsParams = {
  cursor?: number;
  pageSize?: number;
  direction?: 1 | -1;
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
};

export async function getProducts(
  params: GetProductsParams = {}
): Promise<ProductCursorResponse> {
  const searchParams = new URLSearchParams();

  if (params.cursor !== undefined) {
    searchParams.set(
      "cursor",
      params.cursor.toString()
    );
  }

  if (params.pageSize !== undefined) {
    searchParams.set(
      "pageSize",
      params.pageSize.toString()
    );
  }

  if (params.direction !== undefined) {
    searchParams.set(
      "direction",
      params.direction.toString()
    );
  }

  if (params.categoryId !== undefined) {
    searchParams.set(
      "categoryId",
      params.categoryId.toString()
    );
  }

  if (params.minPrice !== undefined) {
    searchParams.set(
      "minPrice",
      params.minPrice.toString()
    );
  }

  if (params.maxPrice !== undefined) {
    searchParams.set(
      "maxPrice",
      params.maxPrice.toString()
    );
  }

  const queryString =
    searchParams.toString();

  const url =
    `${PRODUCT_API_URL}/api/Products` +
    (queryString
      ? `?${queryString}`
      : "");

  return apiFetch<ProductCursorResponse>(
    url
  );
}

export async function getProduct(
  productId: number
): Promise<Product> {
  return apiFetch<Product>(
    `${PRODUCT_API_URL}/api/Products/${productId}`
  );
}