import { getOrders } from "@/app/lib/api";

interface OrderItem {
  id: number;
  productId: number;
  quantity: number;
  price: number;
}

interface Order {
  id: number;
  orderDate: string;
  total: number;
  orderItems: OrderItem[];
}

export default async function OrdersPage() {
  const orders = (await getOrders()) as Order[] | null;

  return (
    <main>
      <h1>My Orders</h1>

      {!orders || orders.length === 0 ? (
        <p>No orders yet.</p>
      ) : (
        orders.map((order) => (
          <div key={order.id}>
            <h2>Order #{order.id}</h2>

            <p>
              Date: {order.orderDate}
            </p>

            <p>
              Total: ₹{order.total}
            </p>

            <p>
              Items: {order.orderItems.length}
            </p>
          </div>
        ))
      )}
    </main>
  );
}