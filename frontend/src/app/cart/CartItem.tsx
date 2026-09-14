"use client";

import { CartItem as CartItemType } from "./Cart.Types";

interface CartItemProps {
  item: CartItemType;
  onQuantityChange: (
    cartItemId: number,
    quantity: number
  ) => void;
  onRemove: (cartItemId: number) => void;
}

export default function CartItem({
  item,
  onQuantityChange,
  onRemove,
}: CartItemProps) {
  const decreaseQuantity = () => {
    if (item.quantity <= 1) {
      return;
    }

    onQuantityChange(
      item.cartItemId,
      item.quantity - 1
    );
  };

  const increaseQuantity = () => {
    if (item.quantity >= 100) {
      return;
    }

    onQuantityChange(
      item.cartItemId,
      item.quantity + 1
    );
  };

  return (
    <article className="fc-cart-item">

      {/* Product information */}
      <div className="fc-cart-item-info">
        <h2 className="fc-cart-item-name">
          {item.productName}
        </h2>

        <p className="fc-cart-item-price">
          ₹{item.price.toFixed(2)} each
        </p>
      </div>


      {/* Quantity control */}
      <div className="fc-quantity-control">

        <button
          type="button"
          className="fc-quantity-button"
          onClick={decreaseQuantity}
          disabled={item.quantity <= 1}
          aria-label={`Decrease quantity of ${item.productName}`}
        >
          −
        </button>

        <span className="fc-quantity-value">
          {item.quantity}
        </span>

        <button
          type="button"
          className="fc-quantity-button"
          onClick={increaseQuantity}
          disabled={item.quantity >= 100}
          aria-label={`Increase quantity of ${item.productName}`}
        >
          +
        </button>

      </div>


      {/* Subtotal and remove */}
      <div className="fc-cart-item-actions">

        <p className="fc-cart-item-subtotal">
          ₹{item.subtotal.toFixed(2)}
        </p>

        <button
          type="button"
          className="fc-button fc-button-ghost fc-remove-button"
          onClick={() =>
            onRemove(item.cartItemId)
          }
        >
          Remove
        </button>

      </div>

    </article>
  );
}