"use client";

import { FormEvent, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { login } from "@/app/lib/api/Auth";

export default function LoginPage() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    setError("");

    if (!email || !password) {
      setError("Please enter email and password.");
      return;
    }

    try {
      setLoading(true);

      const data = await login({
        email,
        password,
      });

      // Store JWT for authenticated API calls
      localStorage.setItem(
        "accessToken",
        data.token
      );

      // Store basic user information
      localStorage.setItem(
        "currentUser",
        JSON.stringify({
          userId: data.userId,
          name: data.name,
          email: data.email,
          role: data.role,
        })
      );

      router.push("/products?page=1");
    } catch (error) {
      console.error("Login error:", error);

      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError(
          "Something went wrong. Please try again."
        );
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="fc-auth-page">
      <div className="fc-auth-card">

        {/* Header */}
        <div className="fc-auth-header">
          <h1 className="fc-auth-title">
            Welcome back
          </h1>

          <p className="fc-auth-description">
            Login to your FlashCart account
          </p>
        </div>

        {/* Form */}
        <form
          onSubmit={handleSubmit}
          className="fc-form"
        >

          {/* Email */}
          <div className="fc-form-group">
            <label
              htmlFor="email"
              className="fc-label"
            >
              Email
            </label>

            <input
              id="email"
              type="email"
              value={email}
              onChange={(event) =>
                setEmail(event.target.value)
              }
              placeholder="Enter your email"
              className="fc-input"
              disabled={loading}
            />
          </div>

          {/* Password */}
          <div className="fc-form-group">
            <label
              htmlFor="password"
              className="fc-label"
            >
              Password
            </label>

            <input
              id="password"
              type="password"
              value={password}
              onChange={(event) =>
                setPassword(event.target.value)
              }
              placeholder="Enter your password"
              className="fc-input"
              disabled={loading}
            />
          </div>

          {/* Error */}
          {error && (
            <div
              className="fc-alert fc-alert-error"
              role="alert"
            >
              {error}
            </div>
          )}

          {/* Submit */}
          <button
            type="submit"
            disabled={loading}
            className="fc-button fc-button-primary fc-button-full"
          >
            {loading
              ? "Logging in..."
              : "Login"}
          </button>

        </form>

        {/* Register */}
        <p className="fc-auth-footer">
          Don&apos;t have an account?{" "}

          <Link
            href="/register"
            className="fc-auth-link"
          >
            Register
          </Link>
        </p>

      </div>
    </main>
  );
}

