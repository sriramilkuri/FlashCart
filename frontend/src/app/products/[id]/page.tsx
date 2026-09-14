import { notFound } from "next/navigation";

import { Product } from "../types";
import { getAccessToken } from "@/app/lib/auth";

import "./product-details.css";

interface ProductDetailsPageProps {
  params: Promise<{
    id: string;
  }>;
}

async function getProductById(
  id: string
): Promise<Product | null> {
  const token = await getAccessToken();

  const response = await fetch(
    `http://localhost:5011/api/products/${id}`,
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  if (response.status === 404) {
    return null;
  }

  if (!response.ok) {
    throw new Error("Failed to fetch product");
  }

  return response.json();
}

export default async function ProductDetailsPage({
  params,
}: ProductDetailsPageProps) {
  const { id } = await params;

  const productId = Number(id);

  if (Number.isNaN(productId)) {
    notFound();
  }

  const product = await getProductById(id);

  if (!product) {
    notFound();
  }

  return (
    <main className="fc-page">
      <div className="fc-container">

        <div className="fc-page-header">
          <div>
            <h1 className="fc-page-title">
              Product Details
            </h1>

            <p className="fc-page-description">
              View product information and pricing.
            </p>
          </div>
        </div>

        <article className="fc-product-details-card">

          <div className="fc-product-details-content">

            <div className="fc-product-details-info">

              <h2 className="fc-product-details-name">
                {product.name}
              </h2>

              <p className="fc-product-details-description">
                {product.description}
              </p>

            </div>

            <div className="fc-product-details-purchase">

              <p className="fc-product-details-price">
                ₹{product.price.toFixed(2)}
              </p>

              <span className="fc-badge fc-badge-primary">
                Category {product.categoryId}
              </span>

            </div>

          </div>

        </article>

      </div>
    </main>
  );
}