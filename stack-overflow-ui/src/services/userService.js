import { createRegisterRequestForm } from "../models/registerRequestModel";

const API_URL = import.meta.env.VITE_BACKEND_API_URL;

export const registerUser = async (formData) => {
  const form = createRegisterRequestForm(formData);
  return await fetch(`${API_URL}/users/create`, {
    method: "POST",
    body: form,
  });
};

export const loginUser = async (loginRequest) => {
  return await fetch(`${API_URL}/users/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(loginRequest),
  });
};

export const fetchUserProfilePicture = async (token) => {
  const API_URL = import.meta.env.VITE_BACKEND_API_URL;

  const response = await fetch(`${API_URL}/users/profilepicture`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  const data = await response.json();

  if (!data.success || !data.profilePictureUrl) {
    throw new Error("Failed to fetch profile picture URL");
  }

  return data.profilePictureUrl;
};
