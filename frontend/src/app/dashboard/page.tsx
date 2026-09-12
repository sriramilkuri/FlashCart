import { getCurrentUser } from "@/app/lib/api";
import LogoutButton from "@/app/components/LogOutButton";

export default async function DashboardPage() {
  const user = await getCurrentUser();

  if (!user) {
    return <h1>You are not logged in.</h1>;
  }

  return (
    <main>
      <h1>Dashboard</h1>

      <p>User ID: {user.userId}</p>
      <p>Name: {user.name}</p>
      <p>Email: {user.email}</p>

      <LogoutButton />
    </main>
  );
}