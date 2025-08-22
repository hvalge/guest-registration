const Header = () => {
  return (
    <header className="bg-white shadow-sm py-3">
      <div className="container d-flex justify-content-between align-items-center">
        <h1 className="h4 m-0 text-muted">NULLAM</h1>
        <nav className="d-flex align-items-center">
          <a href="#" className="nav-link text-muted px-2">AVALEHT</a>
          <a href="#" className="btn btn-primary mx-2">ÜRITUSE LISAMINE</a>
          <svg width="40" height="40" viewBox="0 0 100 100" className="ms-2">
            <circle cx="50" cy="50" r="45" fill="#004a99" />
            <text x="50" y="68" fontFamily="Arial, sans-serif" fontSize="50" fill="white" textAnchor="middle">¥</text>
          </svg>
        </nav>
      </div>
    </header>
  );
};

export default Header;