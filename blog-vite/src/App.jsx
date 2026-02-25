import { Routes, Route, Navigate } from "react-router-dom";
import { useState, useEffect } from "react";
import axiosInstance from "./api/axiosInstance";

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
  const userId = 1; // ✅ Replace with actual logged-in user ID

  useEffect(() => {
    fetchBlogs();
    fetchSavedBlogs();
  }, []);

  // ✅ Fetch all active blogs
  const fetchBlogs = async () => {
    try {
      const res = await axiosInstance.get("/blogs");
      setBlogs(res.data);
    } catch (err) {
      console.error("Error fetching blogs:", err);
    }
  };

  // ✅ Fetch saved blogs for the user
  const fetchSavedBlogs = async () => {
    try {
      const res = await axiosInstance.get(`/blogs/saved?userId=${userId}`);
      const ids = res.data.map((b) => b.id);
      setSavedBlogIds(ids);
    } catch (err) {
      console.error("Error fetching saved blogs:", err);
    }
  };

  // ✅ Delete blog
  const deleteBlog = async (id) => {
    try {
      await deleteBlogApi(id);
      setBlogs((prev) => prev.filter((b) => b.id !== id));
      setSavedBlogIds((prev) => prev.filter((bid) => bid !== id));
    } catch (err) {
      console.error("Error deleting blog:", err);
    }
  };

  // ✅ Save a blog
  const saveBlog = async (blogId) => {
    try {
      await axiosInstance.post(`/blogs/save/${blogId}?userId=${userId}`);
      setSavedBlogIds((prev) => [...prev, blogId]);
    } catch (err) {
      console.error("Error saving blog:", err);
    }
  };

  // ✅ Unsave a blog
  const unsaveBlog = async (blogId) => {
    try {
      await axiosInstance.delete(`/blogs/save/${blogId}?userId=${userId}`);
      setSavedBlogIds((prev) => prev.filter((id) => id !== blogId));
    } catch (err) {
      console.error("Error unsaving blog:", err);
    }
  };

  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />

      <Route path="/login" element={<Login />} />
      <Route path="/signup" element={<Signup />} />
      <Route path="/verify-email" element={<VerifyEmail />} />

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

      <Route
        path="/create-blog"
        element={<CreateBlog fetchBlogs={fetchBlogs} />}
      />

      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;