import { OrderDetailsItem } from "../types";

interface OrderItemProps {
  item: OrderDetailsItem;
}

export default function OrderItem({
  item,
}: OrderItemProps) {
  return (
    <article>
      <h3>{item.productName}</h3>

      <p>Quantity: {item.quantity}</p>

      <p>
        Unit Price: ₹{item.unitPrice.toFixed(2)}
      </p>

      <p>
        Line Total: ₹{item.lineTotal.toFixed(2)}
      </p>
    </article>
  );
}