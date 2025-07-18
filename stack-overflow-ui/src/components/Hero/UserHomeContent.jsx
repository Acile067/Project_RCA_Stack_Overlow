import React, { useEffect, useState } from "react";
import { getToken } from "../../services/authService";
import { testAuthorization } from "../../services/testService";

const UserHomeContent = () => {
  const [profile, setProfile] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const token = getToken();
        const userData = await testAuthorization(token);
        setProfile(userData);
      } catch (err) {
        setError("Failed to fetch user info.");
      }
    };

    fetchProfile();
  }, []);

  return (
    <div className="max-w-xl mx-auto text-center px-4 py-10">
      <h1 className="text-5xl font-extrabold text-gray-900 mb-6 leading-tight">
        Welcome back, <span className="text-blue-400">StackOverflower</span>
      </h1>

      {profile && (
        <div className="mt-6 text-left">
          <p className="text-gray-700">
            <strong>Email:</strong> {profile.email}
          </p>
          <p className="text-gray-700">
            <strong>ID:</strong> {profile.id}
          </p>
        </div>
      )}

      {error && <p className="text-red-500 mt-4">{error}</p>}
    </div>
  );
};

export default UserHomeContent;
