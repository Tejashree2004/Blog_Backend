import axios from "axios";

const API = axios.create({
  baseURL: "http://localhost:5111/api",
});

// ✅ GET all blogs (optional)
export const getBlogs = () => API.get("/blogs");

// ✅ NEW - My Blogs only
export const getMyBlogs = () => API.get("/blogs/myblogs");

// ✅ NEW - My Feed only
export const getMyFeed = () => API.get("/blogs/myfeed");

// GET single blog
export const getBlogById = (id) => API.get(`/blogs/${id}`);

// CREATE blog
export const createBlog = (data) => API.post("/blogs", data);

// DELETE blog
export const deleteBlogApi = (id) => API.delete(`/blogs/${id}`);

// ✅ LOGIN
export const loginUser = (data) => API.post("/users/login", data);

// ✅ SIGNUP
export const signupUser = (data) => API.post("/users/signup", data);