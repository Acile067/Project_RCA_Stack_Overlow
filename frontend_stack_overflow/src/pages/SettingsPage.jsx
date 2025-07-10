import React from "react";
import UserSettings from "../components/UserSettings";
import { useParams } from "react-router-dom";

const SettingsPage = () => {
  const { id } = useParams();

  return (
    <div>
      <UserSettings id={id} />
    </div>
  );
};

export default SettingsPage;
