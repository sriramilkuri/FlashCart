import Link from "next/link";

import {
  getProducts,
} from "@/app/lib/api/Products";

import AddToCartButton from "./AddToCartButton";

import { getAccessToken } from "../lib/auth";

import "./products.css";

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
      : undefined;

  // Direction
  const direction =
    params.direction !== undefined
      ? Number(params.direction) as 1 | -1
      : 1;

  // Filters
  const categoryId =
    params.categoryId !== undefined
      ? Number(params.categoryId)
      : undefined;

  const minPrice =
    params.minPrice !== undefined
      ? Number(params.minPrice)
      : undefined;

  const maxPrice =
    params.maxPrice !== undefined
      ? Number(params.maxPrice)
      : undefined;

  const pageSize = 20;

  /*
   * Get JWT token.
   *
   * ProductService will eventually validate this token.
   */
  const token = await getAccessToken();

  /*
   * Fetch products from ProductService.
   */
  const data = await getProducts({
    cursor,
    pageSize,
    direction,
    categoryId,
    minPrice,
    maxPrice,
  });

  /*
   * Create pagination URL.
   */
  function createPaginationUrl(
    newCursor: number,
    newDirection: 1 | -1
  ) {
    const searchParams =
      new URLSearchParams();

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