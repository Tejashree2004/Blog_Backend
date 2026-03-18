import axios from "axios";

// ✅ Create instance
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

    console.log("Token being sent:", token);

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    } else {
      console.warn("No JWT token found in localStorage");
    }

    return config;
  },
  (error) => Promise.reject(error)
);

// ================= RESPONSE INTERCEPTOR ================= //

axiosInstance.interceptors.response.use(
  (response) => response,

  (error) => {
    console.error("🔥 FULL ERROR OBJECT:", error);

    const status = error.response?.status;

    // 🔥 FIX: handle all possible message formats
    let message =
      error.response?.data?.message ||
      error.response?.data ||
      error.message ||
      "Something went wrong";

    console.error("🚨 API Error:", status, message);

    // 🔴 401 Unauthorized
    if (status === 401) {
      console.warn("Unauthorized - invalid/expired token");

      if (!window.location.pathname.includes("/login")) {
        localStorage.clear();
        window.location.href = "/login";
      }
    }

    // 🔴 403 Forbidden
    else if (status === 403) {
      alert("Access denied. You don’t have permission.");
    }

    // 🔴 404 Not Found
    else if (status === 404) {
      console.error("API Not Found:", error.config?.url);
    }

    // 🔴 500 Server Error
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