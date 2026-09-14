export interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
  categoryId: number;
}

export interface ProductPagedResponse {
  items : Product[];
  page: number,
  pageSize: number,
  totalItems: number,
  totalPages: number
}

export interface ProductCursorResponse {
  items: Product[];
  nextCursor: number | null;
  previousCursor: number | null;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  pageSize: number;
}