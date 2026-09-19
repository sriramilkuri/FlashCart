import { apiFetch } from "./client";

const AUTH_API_URL =
  process.env.NEXT_PUBLIC_AUTH_API_URL!;

export type RegisterRequest = {
  name: string;
  email: string;
  password: string;
};

export type RegisterResponse = {
  userId: number;
  name: string;
  email: string;
  role: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  userId: number;
  name: string;
  email: string;
  role: string;
  token: string;
};

export type CurrentUser = {
  id: number;
  name: string;
  email: string;
  role: string;
};

export async function register(
  request: RegisterRequest
): Promise<RegisterResponse> {
  return apiFetch<RegisterResponse>(
    `${AUTH_API_URL}/api/Auth/register`,
    {
      method: "POST",
      body: JSON.stringify(request),
    }
  );
}

export async function login(
  request: LoginRequest
): Promise<LoginResponse> {
  return apiFetch<LoginResponse>(
    `${AUTH_API_URL}/api/Auth/login`,
    {
      method: "POST",
      body: JSON.stringify(request),
    }
  );
}

export async function getCurrentUser(
  token: string
): Promise<CurrentUser> {
  return apiFetch<CurrentUser>(
    `${AUTH_API_URL}/api/Auth/me`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );
}