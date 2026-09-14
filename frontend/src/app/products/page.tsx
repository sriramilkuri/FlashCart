import Link from "next/link";

import { ProductCursorResponse } from "./types";
import AddToCartButton from "./AddToCartButton";
import { getAccessToken } from "../lib/auth";

import "./products.css";

const API_URL = process.env.API_URL;

async function getProducts(
  cursor: number | null,
  pageSize: number,
  direction: number,
  categoryId: number | null,
  minPrice: number | null,
  maxPrice: number | null
): Promise<ProductCursorResponse> {
  const token = await getAccessToken();

  const params = new URLSearchParams();

  params.set("pageSize", pageSize.toString());
  params.set("direction", direction.toString());

  if (cursor !== null) {
    params.set("cursor", cursor.toString());
  }

  if (categoryId !== null) {
    params.set("categoryId", categoryId.toString());
  }

  if (minPrice !== null) {
    params.set("minPrice", minPrice.toString());
  }

  if (maxPrice !== null) {
    params.set("maxPrice", maxPrice.toString());
  }

  const response = await fetch(
    `${API_URL}/api/Products?${params.toString()}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    }
  );

  if (!response.ok) {
    throw new Error("Failed to fetch products");
  }

  return response.json();
}

interface ProductsPageProps {
  searchParams: Promise<{
    cursor?: string;
    direction?: string;
    categoryId?: string;
    minPrice?: string;
    maxPrice?: string;
  }>;
}

export default async function ProductsPage({
  searchParams,
}: ProductsPageProps) {
  const params = await searchParams;

  // Cursor
  const cursor =
    params.cursor !== undefined
      ? Number(params.cursor)
      : null;

  // Direction
  const direction =
    params.direction !== undefined
      ? Number(params.direction)
      : 1;

  // Filters
  const categoryId =
    params.categoryId !== undefined
      ? Number(params.categoryId)
      : null;

  const minPrice =
    params.minPrice !== undefined
      ? Number(params.minPrice)
      : null;

  const maxPrice =
    params.maxPrice !== undefined
      ? Number(params.maxPrice)
      : null;

  const pageSize = 20;

  // Fetch products
  const data = await getProducts(
    cursor,
    pageSize,
    direction,
    categoryId,
    minPrice,
    maxPrice
  );

  // Create pagination URL
  function createPaginationUrl(
    newCursor: number,
    newDirection: number
  ) {
    const searchParams = new URLSearchParams();

    searchParams.set(
      "cursor",
      newCursor.toString()
    );

    searchParams.set(
      "direction",
      newDirection.toString()
    );

    // Preserve category filter
    if (params.categoryId) {
      searchParams.set(
        "categoryId",
        params.categoryId
      );
    }

    // Preserve minimum price
    if (params.minPrice) {
      searchParams.set(
        "minPrice",
        params.minPrice
      );
    }

    // Preserve maximum price
    if (params.maxPrice) {
      searchParams.set(
        "maxPrice",
        params.maxPrice
      );
    }

    return `/products?${searchParams.toString()}`;
  }

  return (
    <main className="fc-page">
      <div className="fc-container">

        {/* Page Header */}
        <div className="fc-page-header">
          <div>
            <h1 className="fc-page-title">
              Products
            </h1>

            <p className="fc-page-description">
              Browse our collection of products.
            </p>
          </div>

          <span className="fc-badge fc-badge-primary">
            {data.items.length} products
          </span>
        </div>


        {/* Products */}
        {data.items.length > 0 ? (
          <div className="fc-product-grid">

            {data.items.map((product) => (
              <article
                className="fc-product-card"
                key={product.id}
              >

                <div className="fc-product-card-body">

                  <h2 className="fc-product-name">
                    {product.name}
                  </h2>

                  <p className="fc-product-description">
                    {product.description}
                  </p>

                  <p className="fc-product-price">
                    ₹{product.price.toFixed(2)}
                  </p>

                  <div className="fc-product-card-actions">

                    <Link
                      href={`/products/${product.id}`}
                      className="fc-button fc-button-secondary"
                    >
                      View Product
                    </Link>

                    <AddToCartButton
                      productId={product.id}
                    />

                  </div>

                </div>

              </article>
            ))}

          </div>
        ) : (
          <div className="fc-empty-state">
            <h2 className="fc-empty-title">
              No products found
            </h2>

            <p className="fc-empty-description">
              Try changing your filters or check back
              later for new products.
            </p>
          </div>
        )}


        {/* Pagination */}
        {(data.hasPreviousPage ||
          data.hasNextPage) && (
          <div className="fc-pagination">

            {data.hasPreviousPage &&
              data.previousCursor !== null && (
                <Link
                  href={createPaginationUrl(
                    data.previousCursor,
                    -1
                  )}
                  className="fc-pagination-button"
                >
                  ← Previous
                </Link>
              )}

            {data.hasNextPage &&
              data.nextCursor !== null && (
                <Link
                  href={createPaginationUrl(
                    data.nextCursor,
                    1
                  )}
                  className="fc-pagination-button"
                >
                  Next →
                </Link>
              )}

          </div>
        )}

      </div>
    </main>
  );
}