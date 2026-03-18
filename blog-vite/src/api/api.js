import axiosInstance from "./axiosInstance";

// ===================== BLOG ROUTES ===================== //

export const getBlogs = async () => {
  const res = await axiosInstance.get("/blogs");
  return res.data;
};

export const getMyBlogs = async () => {
  const res = await axiosInstance.get("/blogs/myblogs");
  return res.data;
};

export const getMyFeed = async () => {
  const res = await axiosInstance.get("/blogs/feed");
  return res.data;
};

export const getBlogById = async (id) => {
  const res = await axiosInstance.get(`/blogs/${id}`);
  return res.data;
};

export const createBlog = async (data) => {
  const res = await axiosInstance.post("/blogs", data);
  return res.data;
};

export const deleteBlogApi = async (id) => {
  const res = await axiosInstance.delete(`/blogs/${id}`);
  return res.data;
};

// ===================== AUTH ===================== //

export const loginUser = async (data) => {
  const res = await axiosInstance.post("/auth/login", data);

  console.log("🔥 LOGIN RESPONSE:", res.data);

  if (res.data?.token) {
    localStorage.setItem("jwtToken", res.data.token);
    localStorage.setItem("username", res.data.username);
  } else {
    console.error("❌ Token missing in response");
  }

  return res.data;
};

export const signupUser = async (data) => {
  const res = await axiosInstance.post("/auth/signup", data);
  return res.data;
};

// 🔥 FIXED
export const verifyEmailOtp = async (data) => {
  const res = await axiosInstance.post("/auth/verify-email", data);
  return res.data; // ✅ important
};