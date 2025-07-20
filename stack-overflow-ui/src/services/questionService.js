import { getToken } from "./authService"; // napravi ako nemaš, vraća JWT iz localStorage-a
const API_URL = import.meta.env.VITE_BACKEND_API_URL;

export const getAllQuestions = async () => {
  const res = await fetch(`${API_URL}/questions/get-all`, {
    headers: {
      Authorization: `Bearer ${getToken()}`,
    },
  });

  if (!res.ok) return [];
  return await res.json();
};

export const createQuestion = async (data) => {
  const formData = new FormData();
  formData.append("title", data.title);
  formData.append("description", data.description);
  if (data.questionImage) {
    formData.append("questionImage", data.questionImage);
  }

  const res = await fetch(`${API_URL}/questions/create`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${getToken()}`,
    },
    body: formData,
  });

  return res.ok;
};

export const getQuestionById = async (id) => {
  const res = await fetch(`${API_URL}/questions/${id}`, {
    headers: {
      Authorization: `Bearer ${getToken()}`,
    },
  });

  if (!res.ok) return null;
  return await res.json();
};
