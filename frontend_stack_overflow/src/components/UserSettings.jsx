import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const UserSettings = ({ id }) => {
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

  const [profileImagePreview, setProfileImagePreview] = useState(null);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const API_URL = import.meta.env.VITE_BACKEND_STACKOVERFLOW_API_URL;
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("access_token");
    if (!token) return;

    fetch(`${API_URL}/user/profileimage`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then(async (res) => {
        if (!res.ok) throw new Error("Failed to fetch profile image");
        const blob = await res.blob();
        const url = URL.createObjectURL(blob);
        setProfileImagePreview(url);
      })
      .catch((err) => {
        console.error("Failed to load profile image:", err);
      });
  }, []);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const token = localStorage.getItem("access_token");

        const response = await fetch(`${API_URL}/user/${id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        const data = await response.json();

        if (!response.ok) {
          setErrorMessage(data?.ExceptionMessage || "Failed to load user");
          return;
        }

        setFormData({
          name: data.Name || "",
          lastName: data.LastName || "",
          gender: data.Gender || "",
          country: data.Country || "",
          city: data.City || "",
          address: data.Address || "",
          email: data.Email || "",
        });
      } catch (err) {
        setErrorMessage("Network error");
      }
    };

    fetchUser();
  }, [id]);

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

    try {
      const token = localStorage.getItem("access_token");

      const form = new FormData();
      for (let key in formData) {
        if (formData[key]) {
          form.append(key, formData[key]);
        }
      }

      const response = await fetch(`${API_URL}/user/${id}`, {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: form,
      });

      let data;
      try {
        data = await response.json();
      } catch {
        data = { ExceptionMessage: await response.text() };
      }

      if (!response.ok) {
        setErrorMessage(data?.ExceptionMessage || "Update failed");
        return;
      }

      setSuccessMessage("User updated successfully");
    } catch (err) {
      setErrorMessage("Failed to update user");
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100 px-4 mt-10">
      <form
        onSubmit={handleSubmit}
        className="w-full max-w-lg bg-white rounded-lg shadow-md p-8 space-y-6"
      >
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
              alt="Profile"
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

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Input
            label="First Name"
            name="name"
            value={formData.name}
            onChange={handleChange}
          />
          <Input
            label="Last Name"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
          />
          <Select
            label="Gender"
            name="gender"
            value={formData.gender}
            onChange={handleChange}
          />
          <Input
            label="Country"
            name="country"
            value={formData.country}
            onChange={handleChange}
          />
          <Input
            label="City"
            name="city"
            value={formData.city}
            onChange={handleChange}
          />
          <Input
            label="Address"
            name="address"
            value={formData.address}
            onChange={handleChange}
          />
          <Input
            label="Email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
          />
          <Input
            label="Password"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleChange}
          />
        </div>

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 transition-colors duration-200"
        >
          Save Changes
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

const Input = ({ label, name, type = "text", value, onChange }) => (
  <div>
    <label htmlFor={name} className="block mb-2 font-medium text-gray-700">
      {label}
    </label>
    <input
      type={type}
      id={name}
      name={name}
      value={value}
      onChange={onChange}
      placeholder={label}
      className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
    />
  </div>
);

const Select = ({ label, name, value, onChange }) => (
  <div>
    <label htmlFor={name} className="block mb-2 font-medium text-gray-700">
      {label}
    </label>
    <select
      id={name}
      name={name}
      value={value}
      onChange={onChange}
      required
      className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
    >
      <option value="" disabled>
        Select {label.toLowerCase()}
      </option>
      <option value="male">Male</option>
      <option value="female">Female</option>
    </select>
  </div>
);

export default UserSettings;
