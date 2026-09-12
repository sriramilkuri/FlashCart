import { getCurrentUser } from "../../lib/api";

export default async function AccountPage()
{
    const user = await getCurrentUser();
 

  return (
    <main>
      <h1>My Account</h1>

      <p>User ID: {user?.userId}</p>
      <p>Name: {user?.name}</p>
      <p>Email: {user?.email}</p>
    </main>
  );
}