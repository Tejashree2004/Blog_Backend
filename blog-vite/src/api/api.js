import axiosInstance from "./axiosInstance";

// ✅ GET all blogs (Public)
export const getBlogs = () => axiosInstance.get("/blogs");

// ✅ My Blogs (Public in your backend)
export const getMyBlogs = () => axiosInstance.get("/blogs/myblogs");

// ✅ Feed (⚠️ Your backend route is /feed, not /myfeed)
export const getMyFeed = () => axiosInstance.get("/blogs/feed");

// ✅ GET single blog
export const getBlogById = (id) => axiosInstance.get(`/blogs/${id}`);

// 🔐 CREATE blog (Protected)
export const createBlog = (data) => axiosInstance.post("/blogs", data);

// 🔐 DELETE blog (Protected)
export const deleteBlogApi = (id) => axiosInstance.delete(`/blogs/${id}`);

// 🔐 LOGIN
export const loginUser = (data) =>
  axiosInstance.post("/users/login", data);

// 🔐 SIGNUP
export const signupUser = (data) =>
  axiosInstance.post("/users/signup", data);