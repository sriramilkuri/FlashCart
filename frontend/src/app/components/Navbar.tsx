import Link from "next/link";

export default function Navbar() {
  return (
    <header className="fc-header">
      <div className="fc-header-inner">

        <Link
          href="/"
          className="fc-logo"
        >
          FlashCart
        </Link>

        <nav className="fc-nav">
          <Link
            href="/"
            className="fc-nav-link"
          >
            Home
          </Link>

          <Link
            href="/products"
            className="fc-nav-link"
          >
            Products
          </Link>

          <Link
            href="/cart"
            className="fc-nav-link"
          >
            Cart
          </Link>
        </nav>

      </div>
    </header>
  );
}