import { Routes, Route, Navigate } from "react-router-dom";
import { useState, useEffect, useCallback } from "react";
import axiosInstance from "./api/axiosInstance";
import PrivateRoute from "./components/PrivateRoute";

import Blog from "./Pages/Blog";
import CreateBlog from "./Pages/CreateBlog";
import Landing from "./Pages/Landing";
import Login from "./Pages/Login";
import Signup from "./Pages/Signup";
import VerifyEmail from "./Pages/VerifyEmail";

import { deleteBlogApi } from "./api/api";

function App() {
  const [blogs, setBlogs] = useState([]);
  const [savedBlogIds, setSavedBlogIds] = useState([]);

  const token = localStorage.getItem("jwtToken");
  const username = localStorage.getItem("username");
  const userId = localStorage.getItem("userId"); // ✅ Correct way

  // ✅ Fetch Blogs
  const fetchBlogs = useCallback(async () => {
    try {
      const res = await axiosInstance.get("/blogs");
      setBlogs(res.data);
    } catch (err) {
      console.error("Error fetching blogs:", err);
    }
  }, []);

  // ✅ Fetch Saved Blogs (Only if logged in)
  const fetchSavedBlogs = useCallback(async () => {
    if (!userId) return;

    try {
      const res = await axiosInstance.get(`/blogs/saved?userId=${userId}`);
      const ids = res.data.map((b) => b.id);
      setSavedBlogIds(ids);
    } catch (err) {
      console.error("Error fetching saved blogs:", err);
    }
  }, [userId]);

  // ✅ Initial Load
  useEffect(() => {
    fetchBlogs();
    if (token && userId) {
      fetchSavedBlogs();
    }
  }, [fetchBlogs, fetchSavedBlogs, token, userId]);

  // ✅ Delete Blog
  const deleteBlog = async (id) => {
    try {
      await deleteBlogApi(id);
      setBlogs((prev) => prev.filter((b) => b.id !== id));
      setSavedBlogIds((prev) => prev.filter((bid) => bid !== id));
    } catch (err) {
      console.error("Error deleting blog:", err);
    }
  };

  // ✅ Save Blog
  const saveBlog = async (blogId) => {
    if (!userId) return;

    try {
      await axiosInstance.post(`/blogs/save/${blogId}?userId=${userId}`);
      setSavedBlogIds((prev) =>
        prev.includes(blogId) ? prev : [...prev, blogId]
      );
    } catch (err) {
      console.error("Error saving blog:", err);
    }
  };

  // ✅ Unsave Blog
  const unsaveBlog = async (blogId) => {
    if (!userId) return;

    try {
      await axiosInstance.delete(`/blogs/save/${blogId}?userId=${userId}`);
      setSavedBlogIds((prev) => prev.filter((id) => id !== blogId));
    } catch (err) {
      console.error("Error unsaving blog:", err);
    }
  };

  return (
    <Routes>
      {/* Default Route */}
      <Route
        path="/"
        element={
          token ? <Navigate to="/landing" replace /> : <Navigate to="/login" replace />
        }
      />

      {/* Public Routes */}
      <Route path="/login" element={<Login />} />
      <Route path="/signup" element={<Signup />} />
      <Route path="/verify-email" element={<VerifyEmail />} />

      {/* Public Blog View */}
      <Route
        path="/landing"
        element={
          <Landing
            blogs={blogs}
            deleteBlog={deleteBlog}
            savedBlogIds={savedBlogIds}
            saveBlog={saveBlog}
            unsaveBlog={unsaveBlog}
          />
        }
      />

      <Route
        path="/blog"
        element={
          <Blog
            blogs={blogs}
            deleteBlog={deleteBlog}
            savedBlogIds={savedBlogIds}
            saveBlog={saveBlog}
            unsaveBlog={unsaveBlog}
          />
        }
      />

      {/* 🔐 Protected Route */}
      <Route element={<PrivateRoute />}>
        <Route
          path="/create-blog"
          element={<CreateBlog fetchBlogs={fetchBlogs} />}
        />
      </Route>

      {/* Fallback */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;