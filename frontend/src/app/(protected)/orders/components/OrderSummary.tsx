import { OrderDetails } from "../types";

interface OrderSummaryProps {
  order: OrderDetails;
}

export default function OrderSummary({
  order,
}: OrderSummaryProps) {
  return (
    <section className="order-summary">
      <h2>Order Summary</h2>

      <div className="summary-row">
        <span>Items</span>

        <span>{order.items.length}</span>
      </div>

      <div className="summary-row summary-total">
        <strong>Total</strong>

        <strong>
          ₹{order.total.toFixed(2)}
        </strong>
      </div>
    </section>
  );
}