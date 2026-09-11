import { Product } from "./types";
import "./products.css";
import Link from "next/link";

async function getProducts(): Promise<Product[]> {
  const response = await fetch(
    "http://localhost:5011/api/Products"
  );

  if (!response.ok) {
    throw new Error("Failed to fetch products");
  }

  return response.json();
}

export default async function ProductsPage() {
  const products = await getProducts();

  return (
    <main className="products-page">
      <h1 className="products-title">Products</h1>

      <div className="products-grid">
        {products.map((product) => (
        <div className="product-card" key={product.id}>
  <h2 className="product-name">
    {product.name}
  </h2>

  <p className="product-description">
    {product.description}
  </p>

  <p className="product-price">
    ₹{product.price}
  </p>

  <Link href={`/products/${product.id}`}>
    View Product
  </Link>
</div>
        ))}
      </div>
    </main>
  );
}