import { getAccessToken } from "@/app/lib/auth";

const API_URL = process.env.API_URL;

export async function GET() {
  console.log("🔥 Next.js GET /api/auth/Cart called");

  const token = await getAccessToken();

  console.log("🔥 Token exists:", !!token);
  console.log("🔥 API_URL:", API_URL);

  if (!token) {
    return Response.json(
      { message: "Unauthorized" },
      { status: 401 }
    );
  }

  const response = await fetch(`${API_URL}/api/Cart`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  console.log("🔥 Backend response status:", response.status);

  const data = await response.json();

  console.log("🔥 Backend cart response:", data);

  return Response.json(data, {
    status: response.status,
  });
}