import React from "react";
import MyQuestions from "./MyQuestions"; // putanja prilagodi gde si smestio fajl

const Profile = () => {
  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">My Questions</h1>
      <MyQuestions />
    </div>
  );
};

export default Profile;
