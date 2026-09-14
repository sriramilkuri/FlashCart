"use client";

import { useState } from "react";

interface AddToCartButtonProps {
  productId: number;
}

export default function AddToCartButton({
  productId,
}: AddToCartButtonProps) {
  const [isAdding, setIsAdding] = useState(false);
  const [message, setMessage] = useState("");

  const addToCart = async () => {
    try {
      setIsAdding(true);
      setMessage("");

      const response = await fetch(
        "/api/auth/Cart/Items",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            productId: productId,
            quantity: 1,
          }),
        }
      );

      const data = await response.json();

      if (!response.ok) {
        throw new Error(
          data.message ??
            "Failed to add product to cart."
        );
      }

      setMessage("Added to cart");
    } catch (error) {
      setMessage(
        error instanceof Error
          ? error.message
          : "Something went wrong."
      );
    } finally {
      setIsAdding(false);
    }
  };

  const isSuccess =
    message === "Added to cart";

  return (
    <div className="fc-add-to-cart">

      <button
        type="button"
        onClick={addToCart}
        disabled={isAdding}
        className="fc-button fc-button-primary fc-button-full"
      >
        {isAdding
          ? "Adding..."
          : "Add to Cart"}
      </button>

      {message && (
        <p
          className={
            isSuccess
              ? "fc-add-to-cart-message fc-add-to-cart-success"
              : "fc-add-to-cart-message fc-add-to-cart-error"
          }
          role="status"
        >
          {message}
        </p>
      )}

    </div>
  );
}