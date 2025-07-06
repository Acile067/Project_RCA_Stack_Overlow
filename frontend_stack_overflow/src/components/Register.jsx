import React, { useState } from "react";
import { Link } from "react-router-dom";

const Register = () => {
  const [formData, setFormData] = useState({
    name: "",
    lastName: "",
    gender: "",
    country: "",
    city: "",
    address: "",
    email: "",
    password: "",
    profileImage: null,
  });

  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [profileImagePreview, setProfileImagePreview] = useState(null);

  const handleChange = (e) => {
    const { name, value, files, type } = e.target;
    if (type === "file") {
      const file = files[0];
      setFormData((prev) => ({ ...prev, [name]: file }));
      setProfileImagePreview(URL.createObjectURL(file));
    } else {
      setFormData((prev) => ({ ...prev, [name]: value }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage("");
    setSuccessMessage("");

    if (!formData.profileImage) {
      setErrorMessage("Profile image is required.");
      return;
    }

    try {
      const form = new FormData();
      for (let key in formData) {
        if (formData[key]) {
          form.append(key, formData[key]);
        }
      }

      const response = await fetch("http://localhost:5050/user/register", {
        method: "POST",
        body: form,
      });

      let data;
      try {
        data = await response.json();
      } catch {
        data = { ExceptionMessage: await response.text() };
      }

      if (!response.ok) {
        setErrorMessage(data.ExceptionMessage || "Registration failed");
        return;
      }

      setSuccessMessage(data || "User registered successfully");
    } catch (error) {
      setErrorMessage("Network error or server unavailable");
      console.error("Registration failed:", error);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100 px-4">
      <form
        onSubmit={handleSubmit}
        className="w-full max-w-lg bg-white rounded-lg shadow-md p-8 space-y-6"
      >
        <Link
          to="/"
          className="block w-full text-2xl font-semibold mb-6 text-center"
        >
          StackOverflow
        </Link>

        <div className="flex justify-center">
          <label
            htmlFor="profileImage"
            className="relative cursor-pointer group"
          >
            <img
              src={
                profileImagePreview ||
                "https://avatars.githubusercontent.com/u/583231?v=4"
              }
              alt="Profile Preview"
              className="w-32 h-32 rounded-full object-cover border-4 border-gray-300 group-hover:opacity-80 transition"
            />
            <input
              type="file"
              id="profileImage"
              name="profileImage"
              accept="image/*"
              onChange={handleChange}
              className="absolute inset-0 opacity-0 cursor-pointer"
            />
            <div className="absolute bottom-0 w-full text-center text-sm text-white bg-black bg-opacity-50 rounded-b-full py-1 hidden group-hover:block">
              Change
            </div>
          </label>
        </div>
        {!formData.profileImage &&
          errorMessage === "Profile image is required." && (
            <p className="text-red-500 text-sm text-center mt-2">
              Please upload a profile image.
            </p>
          )}

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label
              htmlFor="firstName"
              className="block mb-2 font-medium text-gray-700"
            >
              First Name
            </label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
              placeholder="First Name"
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label
              htmlFor="lastName"
              className="block mb-2 font-medium text-gray-700"
            >
              Last Name
            </label>
            <input
              type="text"
              id="lastName"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              required
              placeholder="Last Name"
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label
              htmlFor="gender"
              className="block mb-2 font-medium text-gray-700"
            >
              Gender
            </label>
            <select
              id="gender"
              name="gender"
              value={formData.gender}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="" disabled>
                Select gender
              </option>
              <option value="male">Male</option>
              <option value="female">Female</option>
            </select>
          </div>

          <div>
            <label
              htmlFor="country"
              className="block mb-2 font-medium text-gray-700"
            >
              Country
            </label>
            <input
              type="text"
              id="country"
              name="country"
              value={formData.country}
              onChange={handleChange}
              required
              placeholder="Country"
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label
              htmlFor="city"
              className="block mb-2 font-medium text-gray-700"
            >
              City
            </label>
            <input
              type="text"
              id="city"
              name="city"
              value={formData.city}
              onChange={handleChange}
              required
              placeholder="City"
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label
              htmlFor="address"
              className="block mb-2 font-medium text-gray-700"
            >
              Address
            </label>
            <input
              type="text"
              id="address"
              name="address"
              value={formData.address}
              onChange={handleChange}
              required
              placeholder="Address"
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label
              htmlFor="email"
              className="block mb-2 font-medium text-gray-700"
            >
              Email
            </label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              placeholder="you@example.com"
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label
              htmlFor="password"
              className="block mb-2 font-medium text-gray-700"
            >
              Password
            </label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required
              placeholder="Password"
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 transition-colors duration-200"
        >
          Register
        </button>
        {errorMessage && (
          <p className="text-red-600 mt-4 text-center font-semibold">
            {errorMessage}
          </p>
        )}

        {successMessage && (
          <p className="text-green-600 mt-4 text-center font-semibold">
            {successMessage}
          </p>
        )}
      </form>
    </div>
  );
};

export default Register;
