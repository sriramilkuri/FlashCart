"use client";

import { useEffect, useState } from "react";

import CartItem from "./CartItem";
import CartSummary from "./Summary";

import { Cart } from "./Cart.Types";

export default function CartPage() {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadCart = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await fetch("/api/auth/Cart");

      if (!response.ok) {
        throw new Error("Failed to load cart.");
      }

      const data: Cart = await response.json();

      setCart(data);
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Something went wrong."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCart();
  }, []);

  const updateQuantity = async (
    cartItemId: number,
    quantity: number
  ) => {
    try {
      const response = await fetch(
        `/api/auth/Cart/Items/${cartItemId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            quantity,
          }),
        }
      );

      if (!response.ok) {
        const errorData = await response.json();

        throw new Error(
          errorData.message ||
            "Failed to update quantity."
        );
      }

      await loadCart();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Something went wrong."
      );
    }
  };

  const removeItem = async (
    cartItemId: number
  ) => {
    try {
      const response = await fetch(
        `/api/auth/Cart/Items/${cartItemId}`,
        {
          method: "DELETE",
        }
      );

      if (!response.ok) {
        throw new Error(
          "Failed to remove item."
        );
      }

      await loadCart();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Something went wrong."
      );
    }
  };

  /* Loading state */
  if (loading) {
    return (
      <main className="fc-page">
        <div className="fc-container">

          <div className="fc-page-header">
            <div>
              <h1 className="fc-page-title">
                Shopping Cart
              </h1>

              <p className="fc-page-description">
                Loading your cart...
              </p>
            </div>
          </div>

          <div className="fc-card">
            <div className="fc-card-body">
              <div className="fc-loading">
                Loading cart...
              </div>
            </div>
          </div>

        </div>
      </main>
    );
  }

  /* Error state */
  if (error) {
    return (
      <main className="fc-page">
        <div className="fc-container">

          <div className="fc-page-header">
            <div>
              <h1 className="fc-page-title">
                Shopping Cart
              </h1>

              <p className="fc-page-description">
                There was a problem loading your cart.
              </p>
            </div>
          </div>

          <div className="fc-alert fc-alert-error">
            {error}
          </div>

        </div>
      </main>
    );
  }

  /* Empty cart */
  if (!cart || cart.items.length === 0) {
    return (
      <main className="fc-page">
        <div className="fc-container">

          <div className="fc-page-header">
            <div>
              <h1 className="fc-page-title">
                Shopping Cart
              </h1>

              <p className="fc-page-description">
                Review the products you want to purchase.
              </p>
            </div>
          </div>

          <div className="fc-empty-state">
            <h2 className="fc-empty-title">
              Your cart is empty
            </h2>

            <p className="fc-empty-description">
              Add some products to your cart and they
              will appear here.
            </p>
          </div>

        </div>
      </main>
    );
  }

  /* Cart with items */
  return (
    <main className="fc-page">
      <div className="fc-container">

        <div className="fc-page-header">
          <div>
            <h1 className="fc-page-title">
              Shopping Cart
            </h1>

            <p className="fc-page-description">
              Review your items before checkout.
            </p>
          </div>

          <span className="fc-badge fc-badge-primary">
            {cart.items.length}{" "}
            {cart.items.length === 1
              ? "item"
              : "items"}
          </span>
        </div>


        <div className="fc-cart-layout">

          {/* Cart items */}
          <section className="fc-cart-items">

            {cart.items.map((item) => (
              <CartItem
                key={item.cartItemId}
                item={item}
                onQuantityChange={updateQuantity}
                onRemove={removeItem}
              />
            ))}

          </section>


          {/* Cart summary */}
          <aside>
            <CartSummary cart={cart} />
          </aside>

        </div>

      </div>
    </main>
  );
}