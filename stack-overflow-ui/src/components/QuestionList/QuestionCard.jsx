import React from "react";
import { Link } from "react-router-dom";

const QuestionCard = ({ question }) => {
  return (
    <div className="border p-4 rounded shadow-md bg-white">
      <h2 className="text-xl font-semibold">{question.Title}</h2>
      <p className="mt-1 text-gray-700">{question.Description}</p>

      {question.PictureUrl && (
        <img
          src={question.PictureUrl}
          alt="Question"
          className="mt-2 w-full max-h-64 object-cover rounded"
        />
      )}

      <Link
        to={`/question/${question.Id}/answers`}
        className="mt-4 inline-block text-blue-600 hover:underline"
      >
        Answers
      </Link>
    </div>
  );
};

export default QuestionCard;
