import React from "react";
import { Navigate, Outlet } from "react-router-dom";

function PrivateRoute() {
  const token = localStorage.getItem("jwtToken");
  const userType = localStorage.getItem("userType");

  // If JWT token exists → allow
  if (token) {
    return <Outlet />;
  }

  // If guest → allow only viewing pages (not create)
  if (userType === "guest") {
    return <Outlet />;
  }

  // Otherwise redirect to login
  return <Navigate to="/login" replace />;
}

export default PrivateRoute;