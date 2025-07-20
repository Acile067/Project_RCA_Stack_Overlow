import React from "react";
import { Route } from "react-router-dom";
import MainLayout from "../layouts/MainLayout";
import PrivateRoute from "../components/Route/PrivateRoute";
const HomePage = React.lazy(() => import("../pages/HomePage"));
const SettingsPage = React.lazy(() => import("../pages/SettingsPage"));
const AnswersPage = React.lazy(() => import("../pages/AnswersPage"));

export const mainRoutes = (
  <Route path="/" element={<MainLayout />}>
    <Route index element={<HomePage />} />
    <Route
      path="settings/:id"
      element={
        <PrivateRoute>
          <SettingsPage />
        </PrivateRoute>
      }
    />
    <Route
      path="question/:id/answers"
      element={
        <PrivateRoute>
          <AnswersPage />
        </PrivateRoute>
      }
    />
  </Route>
);
