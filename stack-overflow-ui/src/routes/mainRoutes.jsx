import React from "react";
import { Route } from "react-router-dom";
import MainLayout from "../layouts/MainLayout";
const HomePage = React.lazy(() => import("../pages/HomePage"));

export const mainRoutes = (
  <Route path="/" element={<MainLayout />}>
    <Route index element={<HomePage />} />
  </Route>
);
