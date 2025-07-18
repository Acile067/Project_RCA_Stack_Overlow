import React from "react";

const GuestHomeContent = () => {
  return (
    <div className="max-w-xl mx-auto text-center px-4 py-20">
      <h1 className="text-5xl font-extrabold text-gray-900 mb-6 leading-tight">
        Welcome to <span className="text-blue-400">StackOverflow</span>!
      </h1>
      <p className="text-lg text-gray-700 tracking-wide">
        Create an account or login to start asking questions and answering.
      </p>
    </div>
  );
};

export default GuestHomeContent;
