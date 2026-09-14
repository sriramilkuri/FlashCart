import Link from "next/link";

import { getOrders } from "./api";


export default async function OrdersPage() {
  const orders = await getOrders();

  return (
    <main className="orders-page">
      <div className="orders-container">
        <h1>My Orders</h1>

        {orders.length === 0 ? (
          <div className="order-not-found">
            <h2>No orders found</h2>

            <p>
              You havent placed any orders yet.
            </p>
          </div>
        ) : (
          <section className="orders-list">
            {orders.map((order) => (
              <article
                key={order.orderId}
                className="order-card"
              >
                <div className="order-card-header">
                  <div>
                    <p className="order-label">
                      Order
                    </p>

                    <h2>
                      #{order.orderId}
                    </h2>
                  </div>

                  <div className="order-status">
                    {order.status}
                  </div>
                </div>

                <div className="order-card-details">
                  <div>
                    <span>Date</span>

                    <strong>
                      {new Date(
                        order.orderDate
                      ).toLocaleDateString("en-IN", {
                        day: "2-digit",
                        month: "short",
                        year: "numeric",
                      })}
                    </strong>
                  </div>

                  <div>
                    <span>Items</span>

                    <strong>
                      {order.items.length}
                    </strong>
                  </div>

                  <div>
                    <span>Total</span>

                    <strong>
                      ₹{order.total.toFixed(2)}
                    </strong>
                  </div>
                </div>

                <div className="order-card-footer">
                  <Link
                    href={`/orders/${order.orderId}`}
                    className="view-order-link"
                  >
                    View Order
                  </Link>
                </div>
              </article>
            ))}
          </section>
        )}
      </div>
    </main>
  );
}