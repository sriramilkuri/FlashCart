import { NextRequest, NextResponse } from "next/server";

import { getAccessToken } from "@/app/lib/auth";

const API_URL = process.env.API_URL;

export async function POST(
  request: NextRequest
) {
  try {
    const body = await request.json();

    const token = await getAccessToken();

    const response = await fetch(
      `${API_URL}/api/Cart/Items`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(body)
      }
    );
    console.log(response);
    const data = await response.json();

    return NextResponse.json(
      data,
      {
        status: response.status
      }
    );
  } catch {
    return NextResponse.json(
      {
        message: "Failed to add product to cart."
      },
      {
        status: 500
      }
    );
  }
}