import { OrderDetails } from "@/app/(protected)/orders/types";

interface OrderHeaderProps {
  order: OrderDetails;
}

export default function OrderHeader({
  order,
}: OrderHeaderProps) {
  return (
    <section>
      <h1>Order #{order.orderId}</h1>

      <p>
        Status: {order.status}
      </p>

      <p>
        Date:{" "}
        {new Date(order.orderDate).toLocaleDateString()}
      </p>
    </section>
  );
}