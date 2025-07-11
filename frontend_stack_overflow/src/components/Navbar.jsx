import React from "react";
import { useState, useEffect } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { animateScroll, scroller } from "react-scroll";
import { FaSearch } from "react-icons/fa";
import { isAuthenticated } from "../services/authService";
import { getUserIdFromToken } from "../services/authService";

const Navbar = () => {
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const [authenticated, setAuthenticated] = useState(false);
  const [profileImage, setProfileImage] = useState(null);
  const [userId, setUserId] = useState(null);
  const API_URL = import.meta.env.VITE_BACKEND_STACKOVERFLOW_API_URL;

  const handleHeroScroll = () => {
    if (location.pathname === "/") {
      // Ako smo već na HomePage-u, skroluj do vrha
      animateScroll.scrollToTop({ duration: 500, smooth: true });
    } else {
      // Ako nismo na HomePage-u, prvo navigiraj, pa zatim skroluj
      navigate("/");
      setTimeout(() => {
        animateScroll.scrollToTop({ duration: 500, smooth: true });
      }, 200); // Sačekaj dok se stranica učita
    }
  };

  const handleMobileMenuToggle = () => {
    setIsMobileMenuOpen((prev) => !prev);
  };

  const handleMouseEnter = () => {
    setIsDropdownOpen(true);
  };

  const handleMouseLeave = () => {
    setIsDropdownOpen(false);
  };

  const handleScroll = () => {
    const offset = window.scrollY;
    setIsScrolled(offset > 0);
  };

  useEffect(() => {
    window.addEventListener("scroll", handleScroll);
    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, []);

  useEffect(() => {
    setAuthenticated(isAuthenticated());
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("access_token");
    setAuthenticated(false);
    navigate("/login");
  };

  useEffect(() => {
    console.log("Fetching profile image...");
    const token = localStorage.getItem("access_token");
    if (!token) return;

    fetch(`${API_URL}/user/profileimage`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then(async (res) => {
        if (!res.ok) throw new Error("Image fetch failed");
        const blob = await res.blob();
        const url = URL.createObjectURL(blob);
        setProfileImage(url);
      })
      .catch((err) => {
        console.error("Failed to load profile image:", err);
      });
  }, []);

  useEffect(() => {
    setAuthenticated(isAuthenticated());

    if (isAuthenticated()) {
      const id = getUserIdFromToken();
      setUserId(id);
    } else {
      setUserId(null);
    }
  }, []);

  return (
    <nav
      className={`bg-white bg-opacity-70 backdrop-blur-md fixed w-full z-20 top-0 start-0 transition-shadow duration-300 ${
        isScrolled ? "shadow-md" : ""
      }`}
    >
      <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto p-4">
        <button
          onClick={handleHeroScroll}
          className="flex items-center space-x-3 rtl:space-x-reverse"
        >
          <span className="self-center text-2xl font-semibold whitespace-nowrap transition duration-200 ease-in-out hover:text-secondary">
            StackOverflow
          </span>
        </button>

        <div className="flex md:order-2 space-x-3 md:space-x-0 rtl:space-x-reverse">
          {!authenticated ? (
            <>
              <button
                type="button"
                className="hidden md:inline-flex text-gray-900 border-gray-900 border-2 focus:outline-none font-medium rounded-lg text-base px-4 py-2 text-center group mr-1"
              >
                <Link to="/login">
                  <span className="inline-block transition-transform duration-150 ease-in-out group-hover:-translate-y-1 font-medium">
                    Login
                  </span>
                </Link>
              </button>
              <button
                type="button"
                className="hidden md:inline-flex text-gray-900 border-gray-900 border-2 focus:outline-none font-medium rounded-lg text-base px-4 py-2 text-center group"
              >
                <Link to="/register">
                  <span className="inline-block transition-transform duration-150 ease-in-out group-hover:-translate-y-1 font-medium">
                    Register
                  </span>
                </Link>
              </button>
            </>
          ) : (
            <div className="relative hidden md:block">
              <button
                type="button"
                onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                className="w-11 h-11 rounded-full overflow-hidden border-2 border-gray-300 focus:outline-none cursor-pointer"
              >
                {profileImage ? (
                  <img
                    src={profileImage}
                    alt="Profile"
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <div className="w-full h-full bg-gray-400 animate-pulse"></div>
                )}
              </button>

              {isDropdownOpen && (
                <div
                  onMouseLeave={() => setIsDropdownOpen(false)}
                  className="absolute right-0 mt-2 w-40 bg-white rounded-md shadow-lg z-50"
                >
                  <Link
                    to={`/profile/${userId}`}
                    className="block px-4 py-2 text-gray-700 hover:bg-gray-100"
                    onClick={() => setIsDropdownOpen(false)}
                  >
                    Profile
                  </Link>
                  <Link
                    to={`/settings/${userId}`}
                    className="block px-4 py-2 text-gray-700 hover:bg-gray-100"
                    onClick={() => setIsDropdownOpen(false)}
                  >
                    Settings
                  </Link>
                  <button
                    onClick={handleLogout}
                    className="w-full text-left px-4 py-2 text-gray-700 hover:bg-gray-100"
                  >
                    Logout
                  </button>
                </div>
              )}
            </div>
          )}

          <button
            onClick={handleMobileMenuToggle}
            type="button"
            className="inline-flex items-center p-2 w-10 h-10 justify-center text-sm text-gray-500 rounded-lg md:hidden hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-gray-200"
            aria-controls="navbar-sticky"
            aria-expanded={isMobileMenuOpen}
          >
            <span className="sr-only">Open main menu</span>
            <svg
              className="w-5 h-5"
              aria-hidden="true"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 17 14"
            >
              <path
                stroke="currentColor"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M1 1h15M1 7h15M1 13h15"
              />
            </svg>
          </button>
        </div>
        <div
          className={`items-center justify-between w-full md:flex md:w-auto md:order-1 ${
            isMobileMenuOpen ? "block" : "hidden"
          }`}
          id="navbar-sticky"
        >
          <ul className="flex flex-col p-4 md:p-0 mt-4 font-medium border  md:space-x-8 rtl:space-x-reverse md:flex-row md:mt-0 md:border-0">
            {!authenticated ? (
              <>
                <li>
                  <Link
                    to="/login"
                    className="block mt-1 md:hidden py-2 px-3 text-gray-900 border-2 border-gray-900 rounded-lg hover:bg-gray-100 md:hover:bg-transparent md:p-0 text-base font-medium text-center"
                  >
                    Login
                  </Link>
                </li>
                <li>
                  <Link
                    to="/register"
                    className="block mt-1 md:hidden py-2 px-3 text-gray-900 border-2 border-gray-900 rounded-lg hover:bg-gray-100 md:hover:bg-transparent md:p-0 text-base font-medium text-center"
                  >
                    Register
                  </Link>
                </li>
              </>
            ) : (
              <>
                <li>
                  <Link
                    to={`/profile/${userId}`}
                    className="block mt-1 md:hidden py-2 px-3 text-white bg-gray-900 rounded-lg hover:bg-gray-800 text-base font-medium text-center"
                  >
                    Profile
                  </Link>
                </li>
                <li>
                  <Link
                    to={`/settings/${userId}`}
                    className="block mt-1 md:hidden py-2 px-3 text-white bg-gray-900 rounded-lg hover:bg-gray-800 text-base font-medium text-center"
                  >
                    Settings
                  </Link>
                </li>
                <li>
                  <button
                    onClick={handleLogout}
                    className="block w-full mt-1 py-2 px-3 text-red-600 border-2 border-red-600 rounded-lg hover:bg-red-100 md:hidden text-base font-medium text-center"
                  >
                    Logout
                  </button>
                </li>
              </>
            )}
            <li>
              <form
                onSubmit={(e) => {
                  e.preventDefault();
                  const query = e.target.elements.search.value.trim();
                  if (query)
                    navigate(`/search?query=${encodeURIComponent(query)}`);
                }}
                className="flex items-center gap-2 py-2"
              >
                <input
                  type="text"
                  name="search"
                  placeholder="Search..."
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-secondary"
                />
                <button
                  type="submit"
                  className="px-3 py-2 bg-gray-900 rounded-md hover:bg-gray-800"
                >
                  <FaSearch className="text-white w-4 h-4" />
                </button>
              </form>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
