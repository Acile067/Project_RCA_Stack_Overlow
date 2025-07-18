const API_URL = import.meta.env.VITE_BACKEND_API_URL;

export const testAuthorization = async (token) => {
  const response = await fetch(`${API_URL}/users/test/autorize`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error("Authorization test failed");
  }

  return await response.json();
};
