import React from "react";
import { Navigate, Outlet } from "react-router-dom";

function PrivateRoute() {
  const token = localStorage.getItem("jwtToken");
  const username = localStorage.getItem("username");
  let userType = localStorage.getItem("userType");

  // Assign guest automatically if nothing exists
  if (!token && !username && !userType) {
    const guestId = "guest_" + Date.now();
    localStorage.setItem("userId", guestId);
    localStorage.setItem("userType", "guest");
    userType = "guest";
  }

  // Logged-in user or guest allowed
  if (username || userType === "guest") return <Outlet />;

  return <Navigate to="/login" replace />;
}

export default PrivateRoute;