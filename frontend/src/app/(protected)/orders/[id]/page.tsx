import Link from "next/link";
import { notFound } from "next/navigation";

import { getOrderDetails } from "../api";

import OrderHeader from "../components/OrderHeader";
import OrderItem from "../components/OrderItem";
import OrderSummary from "../components/OrderSummary";

interface OrderDetailsPageProps {
  params: Promise<{
    id: string;
  }>;
}

export default async function OrderDetailsPage({
  params,
}: OrderDetailsPageProps) {
  const { id } = await params;

  const orderId = Number(id);

  if (!Number.isInteger(orderId) || orderId <= 0) {
    notFound();
  }

  let order;
  try {
    order = await getOrderDetails(orderId);
  } catch (error) {
    console.log(error);
    if (
      error instanceof Error &&
      error.message === "ORDER_NOT_FOUND"
    ) {
      notFound();
    }

    throw error;
  }

  return (
    <main className="orders-page">
      <div className="orders-container">

        <Link href="/orders" className="back-link">
          ← Back to Orders
        </Link>

        <OrderHeader order={order} />

        <section className="order-items-section">
          <div className="section-heading">
            <h2>Order Items</h2>

            <span>
              {order.items.length}{" "}
              {order.items.length === 1
                ? "item"
                : "items"}
            </span>
          </div>

          <div className="order-items">
            {order.items.map((item) => (
              <OrderItem
                key={item.productId}
                item={item}
              />
            ))}
          </div>
        </section>

        <OrderSummary order={order} />

      </div>
    </main>
  );
}