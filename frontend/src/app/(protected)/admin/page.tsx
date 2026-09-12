import { redirect } from "next/navigation";
import { getCurrentUser } from "@/app/lib/api";

export default async function AdminPage() {
  const user = await getCurrentUser();

  if (!user) {
    redirect("/login");
  }

  if (user.role !== "Admin") {
    return (
      <main>
        <h1>Forbidden</h1>
        <p>You do not have permission to access this page.</p>
      </main>
    );
  }

  return (
    <main>
      <h1>Admin Dashboard</h1>

      <p>Welcome, {user.name}.</p>
    </main>
  );
}