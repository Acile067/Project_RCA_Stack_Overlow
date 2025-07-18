import React, { useState } from "react";
import { registerUser } from "../../services/userService";
import { createEmptyFieldErrors } from "../../models/fieldErrorModel";
import ProfileImageUploader from "./ProfileImageUploader";
import { Link } from "react-router-dom";

const Register = () => {
  const [formData, setFormData] = useState({
    fullName: "",
    gender: "",
    country: "",
    city: "",
    address: "",
    email: "",
    password: "",
    profileImage: null,
  });

  const [errors, setErrors] = useState(createEmptyFieldErrors());
  const [message, setMessage] = useState({ type: "", text: "" });

  const validateFields = () => {
    const newErrors = createEmptyFieldErrors();
    let isValid = true;

    for (const key in formData) {
      if (!formData[key]) {
        newErrors[key] = "This field is required.";
        isValid = false;
      }
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleChange = (e) => {
    const { name, value, files } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: files ? files[0] : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage({ type: "", text: "" });

    if (!validateFields()) return;

    try {
      const response = await registerUser(formData);

      if (!response.ok) {
        const result = await response.json();
        if (result.Errors) {
          const formattedErrors = createEmptyFieldErrors();
          result.Errors.forEach((err) => {
            if (formattedErrors.hasOwnProperty(err.Field)) {
              formattedErrors[err.Field] = err.Message;
            }
          });
          setErrors(formattedErrors);
        } else {
          setMessage({
            type: "error",
            text: result.message || "Registration failed.",
          });
        }
      } else {
        setMessage({ type: "success", text: "Registration successful!" });
        setFormData({
          fullName: "",
          gender: "",
          country: "",
          city: "",
          address: "",
          email: "",
          password: "",
          profileImage: null,
        });
        setErrors(createEmptyFieldErrors());
      }
    } catch (error) {
      setMessage({
        type: "error",
        text: "An error occurred. Please try again.",
      });
    }
  };

  return (
    <div className="max-w-xl mx-auto p-8 bg-white shadow-lg rounded-xl mt-10">
      <Link
        to="/"
        className="block w-full text-2xl font-semibold mb-6 text-center"
      >
        StackOverflow
      </Link>
      <form
        onSubmit={handleSubmit}
        encType="multipart/form-data"
        className="space-y-5"
      >
        <ProfileImageUploader
          preview={formData.profileImage}
          onChange={handleChange}
          error={errors.profileImage}
        />

        {/* Full Name */}
        <div>
          <label className="block font-semibold">Full Name</label>
          <input
            name="fullName"
            className="w-full p-2 border rounded-md"
            value={formData.fullName}
            onChange={handleChange}
          />
          <p className="text-red-500 text-sm mt-1">{errors.fullName}</p>
        </div>

        {/* Gender */}
        <div>
          <label className="block font-semibold">Gender</label>
          <select
            name="gender"
            className="w-full p-2 border rounded-md"
            value={formData.gender}
            onChange={handleChange}
          >
            <option value="">Select gender</option>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </select>
          <p className="text-red-500 text-sm mt-1">{errors.gender}</p>
        </div>

        {/* Country */}
        <div>
          <label className="block font-semibold">Country</label>
          <input
            name="country"
            className="w-full p-2 border rounded-md"
            value={formData.country}
            onChange={handleChange}
          />
          <p className="text-red-500 text-sm mt-1">{errors.country}</p>
        </div>

        {/* City */}
        <div>
          <label className="block font-semibold">City</label>
          <input
            name="city"
            className="w-full p-2 border rounded-md"
            value={formData.city}
            onChange={handleChange}
          />
          <p className="text-red-500 text-sm mt-1">{errors.city}</p>
        </div>

        {/* Address */}
        <div>
          <label className="block font-semibold">Address</label>
          <input
            name="address"
            className="w-full p-2 border rounded-md"
            value={formData.address}
            onChange={handleChange}
          />
          <p className="text-red-500 text-sm mt-1">{errors.address}</p>
        </div>

        {/* Email */}
        <div>
          <label className="block font-semibold">Email</label>
          <input
            name="email"
            type="email"
            className="w-full p-2 border rounded-md"
            value={formData.email}
            onChange={handleChange}
          />
          <p className="text-red-500 text-sm mt-1">{errors.email}</p>
        </div>

        {/* Password */}
        <div>
          <label className="block font-semibold">Password</label>
          <input
            name="password"
            type="password"
            className="w-full p-2 border rounded-md"
            value={formData.password}
            onChange={handleChange}
          />
          <p className="text-red-500 text-sm mt-1">{errors.password}</p>
        </div>

        <button
          type="submit"
          className="w-full bg-blue-600 text-white p-2 rounded-md hover:bg-blue-700 transition"
        >
          Register
        </button>

        {message.text && (
          <p
            className={`text-sm mt-4 text-center ${
              message.type === "success" ? "text-green-600" : "text-red-600"
            }`}
          >
            {message.text}
          </p>
        )}
      </form>
    </div>
  );
};

export default Register;
