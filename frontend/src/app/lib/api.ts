import { getAccessToken } from "@/app/lib/auth";
import { CurrentUser } from "./types";

const API_URL = process.env.API_URL;

export async function getCurrentUser() : Promise<CurrentUser | null> {
  const token = await getAccessToken();

  if (!token) {
    return null;
  }

  const response = await fetch(
    `${API_URL}/api/users/me`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    }
  );

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getOrders() {
  const token = await getAccessToken();

  if (!token) {
    return null;
  }

  const response = await fetch(
    `${API_URL}/api/orders`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    }
  );

  if (!response.ok) {
    return null;
  }

  return response.json();
}

