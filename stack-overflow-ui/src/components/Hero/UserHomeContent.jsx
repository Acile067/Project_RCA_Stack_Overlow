import React, { useEffect, useState } from "react";
import { getToken } from "../../services/authService";
import { testAuthorization } from "../../services/testService";
import UserHero from "./UserHero";
import QuestionList from "../QuestionList/QuestionList";

const UserHomeContent = () => {
  return (
    <>
      <UserHero />
      <QuestionList />
    </>
  );
};

export default UserHomeContent;
