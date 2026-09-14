import { getAccessToken } from "@/app/lib/auth";

const API_URL = process.env.API_URL;

interface RouteContext {
  params: Promise<{
    cartItemId: string;
  }>;
}

export async function PUT(
  request: Request,
  context: RouteContext
) {
  console.log("🔥 PUT route called");

  const params = await context.params;

  console.log("🔥 params:", params);
  console.log("🔥 cartItemId:", params.cartItemId);

  const token = await getAccessToken();

  if (!token) {
    return Response.json(
      { message: "Unauthorized" },
      { status: 401 }
    );
  }

  const body = await request.json();

  console.log("🔥 body:", body);

  const response = await fetch(
    `${API_URL}/api/Cart/items/${params.cartItemId}`,
    {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(body),
    }
  );

  console.log(
    "🔥 Backend status:",
    response.status
  );

  const data = await response.json();

  console.log(
    "🔥 Backend response:",
    data
  );

  return Response.json(data, {
    status: response.status,
  });
}

export async function DELETE(
  request: Request,
  context: RouteContext
) {
  console.log("🔥 DELETE route called");

  const params = await context.params;

  console.log("🔥 params:", params);
  console.log("🔥 cartItemId:", params.cartItemId);

  const token = await getAccessToken();

  if (!token) {
    return Response.json(
      { message: "Unauthorized" },
      { status: 401 }
    );
  }

  const response = await fetch(
    `${API_URL}/api/Cart/items/${params.cartItemId}`,
    {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  console.log(
    "🔥 Backend status:",
    response.status
  );

  if (response.status === 204) {
    return new Response(null, {
      status: 204,
    });
  }

  const data = await response.json();

  console.log(
    "🔥 Backend response:",
    data
  );

  return Response.json(data, {
    status: response.status,
  });
}