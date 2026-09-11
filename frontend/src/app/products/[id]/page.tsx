import { notFound } from "next/navigation";
import { Product } from "../types";
import "./product-details.css";

interface ProductDetailsPageProps {
  params: Promise<{
    id: string;
  }>;
}

async function getProductById(
  id: string
): Promise<Product | null> {
  const response = await fetch(
    `http://localhost:5011/api/products/${id}`
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
    <main className="product-details-page">
      <div className="product-details-card">
        <h1 className="product-details-name">
          {product.name}
        </h1>

        <p className="product-details-description">
          {product.description}
        </p>

        <p className="product-details-price">
          ₹{product.price}
        </p>

        <p className="product-details-category">
          Category ID: {product.categoryId}
        </p>
      </div>
    </main>
  );
}