import { apiFetch } from "./client";

export async function authenticatedFetch<T>(
  url: string,
  token: string,
  options: RequestInit = {}
): Promise<T> {
  return apiFetch<T>(url, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
      ...(options.headers || {}),
    },
  });
}