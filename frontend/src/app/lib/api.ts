import { getAccessToken } from "./auth";

const API_URL = process.env.API_URL;

export async function getCurrentUser()
{
    const token = await getAccessToken();

    if(!token) return null;

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

    if(!response) return null;

    return response.json();
}