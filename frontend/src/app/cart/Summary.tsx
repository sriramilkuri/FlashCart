import { Cart } from "./Cart.Types";

interface CartSummaryProps {
  cart: Cart;
}

export default function CartSummary({
  cart,
}: CartSummaryProps) {
  return (
    <div className="fc-summary">
      <h2 className="fc-summary-title">
        Cart Summary
      </h2>

      <div className="fc-summary-row">
        <span>Subtotal</span>

        <span>
          ₹{cart?.subtotal?.toFixed(2)}
        </span>
      </div>

      <div className="fc-summary-row">
        <span>
          Tax ({cart.taxRate}%)
        </span>

        <span>
          ₹{cart?.tax?.toFixed(2)}
        </span>
      </div>

      <div className="fc-divider" />

      <div className="fc-summary-row fc-summary-total">
        <span>Total</span>

        <span>
          ₹{cart?.total?.toFixed(2)}
        </span>
      </div>

      <button
        type="button"
        className="fc-button fc-button-primary fc-button-full"
      >
        Checkout
      </button>
    </div>
  );
}