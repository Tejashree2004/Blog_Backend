import axios from "axios";

// ✅ Create axios instance
const axiosInstance = axios.create({
  baseURL: "http://localhost:5111/api",
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10000,
});

// ================= REQUEST INTERCEPTOR ================= //
axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("jwtToken");

    // 🔹 Debug token
    console.log("Token being sent:", token);

    if (token) {
      // Attach JWT if available
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);

// ================= RESPONSE INTERCEPTOR ================= //
axiosInstance.interceptors.response.use(
  (response) => response, // success

  (error) => {
    console.error("🔥 FULL ERROR OBJECT:", error);

    const status = error.response?.status;

    // 🔹 Extract message safely
    const message =
      error.response?.data?.message ||
      error.response?.data?.title ||
      error.message ||
      "Something went wrong";

    console.error("🚨 API Error:", status, message);

    // 🔴 401 Unauthorized
    if (status === 401) {
      console.warn("Unauthorized - invalid or expired token");
      localStorage.clear();
      if (!window.location.pathname.includes("/login")) {
        window.location.href = "/login";
      }
    }

    // 🔴 403 Forbidden
    else if (status === 403) {
      alert("Access denied. You don’t have permission.");
    }

    // 🔴 404 Not Found
    else if (status === 404) {
      console.error("API endpoint not found:", error.config?.url);
    }

    // 🔴 400 Bad Request
    else if (status === 400) {
      console.error("Bad Request:", message);
    }

    // 🔴 500+ Server Error
    else if (status >= 500) {
      console.error("Server error:", message);
    }

    return Promise.reject({
      status,
      message,
      originalError: error,
    });
  }
);

export default axiosInstance;