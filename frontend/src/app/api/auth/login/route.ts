import { NextRequest, NextResponse } from "next/server";

const API_URL = process.env.API_URL;

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

    const response = await fetch(`${API_URL}/api/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
    });

    const data = await response.json();

    if (!response.ok) {
      return NextResponse.json(
        {
          message: data.message ?? "Login failed.",
        },
        {
          status: response.status,
        }
      );
    }

    const nextResponse = NextResponse.json({
      id: data.id,
      name: data.name,
      email: data.email,
    });

    nextResponse.cookies.set("accessToken", data.token, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      maxAge: 60 * 60,
    });

    return nextResponse;
  } catch {
    return NextResponse.json(
      {
        message: "Unable to login.",
      },
      {
        status: 500,
      }
    );
  }
}