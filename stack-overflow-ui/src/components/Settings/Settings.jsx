import React, { useEffect, useState } from "react";
import {
  fetchUserProfile,
  updateUserProfile,
} from "../../services/userService";
import { getToken } from "../../services/authService";
import ImageUpload from "./ImageUpload";

const Settings = () => {
  const [profile, setProfile] = useState({
    fullName: "",
    gender: "",
    country: "",
    city: "",
    address: "",
    profilePictureUrl: "",
  });
  const [password, setPassword] = useState("");
  const [imageFile, setImageFile] = useState(null);
  // message: { text: string, type: 'success' | 'error' | '' }
  const [message, setMessage] = useState({ text: "", type: "" });

  useEffect(() => {
    const loadProfile = async () => {
      try {
        const token = getToken();
        const data = await fetchUserProfile(token);

        setProfile({
          fullName: data.FullName || "",
          gender: data.Gender || "",
          country: data.Country || "",
          city: data.City || "",
          address: data.Address || "",
          profilePictureUrl: data.ProfilePictureUrl || "",
        });

        if (data.ProfilePictureUrl) {
          setImageFile(data.ProfilePictureUrl);
        }
      } catch (err) {
        console.error(err);
        setMessage({ text: "Failed to load profile.", type: "error" });
      }
    };

    loadProfile();
  }, []);

  const handleChange = (e) => {
    setProfile({ ...profile, [e.target.name]: e.target.value });
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setImageFile(file);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const token = getToken();
    const formData = new FormData();
    formData.append("fullName", profile.fullName);
    formData.append("gender", profile.gender);
    formData.append("country", profile.country);
    formData.append("city", profile.city);
    formData.append("address", profile.address);
    if (password) formData.append("password", password);
    if (imageFile && typeof imageFile !== "string") {
      formData.append("profileImage", imageFile);
    }

    try {
      await updateUserProfile(formData, token);
      setMessage({ text: "Profile updated successfully.", type: "success" });
    } catch (err) {
      console.error(err);
      setMessage({ text: "Failed to update profile.", type: "error" });
    }
  };

  return (
    <div className="max-w-xl mx-auto mt-24 p-6">
      <h2 className="text-2xl text-center font-bold mb-4">Edit Profile</h2>
      {message.text && (
        <p
          className={`mb-4 text-sm ${
            message.type === "error" ? "text-red-600" : "text-green-600"
          }`}
        >
          {message.text}
        </p>
      )}
      <form onSubmit={handleSubmit} className="space-y-4">
        <ImageUpload
          preview={imageFile}
          onChange={handleFileChange}
          error={null}
        />

        <div>
          <label htmlFor="fullName" className="block mb-1 font-medium">
            Full Name
          </label>
          <input
            id="fullName"
            name="fullName"
            placeholder="Full Name"
            value={profile.fullName}
            onChange={handleChange}
            className="w-full p-2 border rounded"
          />
        </div>

        <div>
          <label htmlFor="gender" className="block mb-1 font-medium">
            Gender
          </label>
          <select
            id="gender"
            name="gender"
            value={profile.gender}
            onChange={handleChange}
            className="w-full p-2 border rounded"
          >
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </select>
        </div>

        <div>
          <label htmlFor="country" className="block mb-1 font-medium">
            Country
          </label>
          <input
            id="country"
            name="country"
            placeholder="Country"
            value={profile.country}
            onChange={handleChange}
            className="w-full p-2 border rounded"
          />
        </div>

        <div>
          <label htmlFor="city" className="block mb-1 font-medium">
            City
          </label>
          <input
            id="city"
            name="city"
            placeholder="City"
            value={profile.city}
            onChange={handleChange}
            className="w-full p-2 border rounded"
          />
        </div>

        <div>
          <label htmlFor="address" className="block mb-1 font-medium">
            Address
          </label>
          <input
            id="address"
            name="address"
            placeholder="Address"
            value={profile.address}
            onChange={handleChange}
            className="w-full p-2 border rounded"
          />
        </div>

        <div>
          <label htmlFor="password" className="block mb-1 font-medium">
            New Password
          </label>
          <input
            id="password"
            name="password"
            placeholder="New Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full p-2 border rounded"
          />
        </div>

        <button
          type="submit"
          className="px-4 py-2 bg-blue-600 text-white rounded"
        >
          Save Changes
        </button>
      </form>
    </div>
  );
};

export default Settings;
