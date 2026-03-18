import React, { useState, useEffect } from "react";
import Navbar from "../components/Navbar.jsx";
import CardList from "../components/CardList.jsx";
import axiosInstance from "../api/axiosInstance"; // ✅ axios with JWT

function Blog() {
  const [blogs, setBlogs] = useState([]);
  const [savedBlogIds, setSavedBlogIds] = useState([]);
  const [search, setSearch] = useState("");
  const [showSaved, setShowSaved] = useState(false);

  // ✅ Current user: username or guestId
  const currentUser = localStorage.getItem("username") || localStorage.getItem("guestId");

  // ✅ Fetch all blogs from backend
  const fetchBlogs = async () => {
    try {
      const res = await axiosInstance.get("/blogs");
      setBlogs(res.data);
    } catch (err) {
      console.error("Fetch blogs error:", err.response?.data || err.message);
    }
  };

  // ✅ Fetch saved blog IDs for current user
  const fetchSavedBlogs = async () => {
    if (!currentUser) return;

    try {
      const res = await axiosInstance.get(`/savedblogs/${currentUser}`);
      // ⚡ Convert all IDs to numbers to match blog.id type
      const ids = res.data.map((id) => Number(id));
      setSavedBlogIds(ids);
    } catch (err) {
      console.error("Fetch saved blogs error:", err.response?.data || err.message);
    }
  };

  useEffect(() => {
    fetchBlogs();
    fetchSavedBlogs();
  }, []);

  // ✅ Save a blog
  const saveBlog = async (blogId) => {
    if (!currentUser) return alert("Login to save blogs");

    try {
      await axiosInstance.post("/savedblogs/save", { userId: currentUser, blogId });
      setSavedBlogIds((prev) => [...prev, Number(blogId)]);
    } catch (err) {
      console.error("Save blog error:", err.response?.data || err.message);
    }
  };

  // ✅ Unsave a blog
  const unsaveBlog = async (blogId) => {
    if (!currentUser) return;

    try {
      await axiosInstance.post("/savedblogs/unsave", { userId: currentUser, blogId });
      setSavedBlogIds((prev) => prev.filter((id) => id !== Number(blogId)));
    } catch (err) {
      console.error("Unsave blog error:", err.response?.data || err.message);
    }
  };

  // ✅ My Blogs → only blogs created by current user
  const myBlogs = blogs.filter((b) => b.isUserCreated && b.author === currentUser);

  // ✅ My Feed → blogs from other users + default blogs
  const myFeed = blogs.filter((b) => !b.isUserCreated || (b.isUserCreated && b.author !== currentUser));

  // ✅ My Saved Blogs → only blogs whose IDs are in savedBlogIds
  const mySavedBlogs = blogs.filter((b) => savedBlogIds.includes(Number(b.id)));

  return (
    <div className="page-wrapper">
      <Navbar search={search} setSearch={setSearch} setShowSaved={setShowSaved} />

      {!showSaved && (
        <>
          <section style={{ marginTop: "30px" }}>
            <h2 className="section-title">My Blogs</h2>
            <CardList
              items={myBlogs}
              search={search}
              savedBlogIds={savedBlogIds}
              saveBlog={saveBlog}
              unsaveBlog={unsaveBlog}
            />
          </section>

          <section style={{ marginTop: "50px" }}>
            <h2 className="section-title">My Feed</h2>
            <CardList
              items={myFeed}
              search={search}
              savedBlogIds={savedBlogIds}
              saveBlog={saveBlog}
              unsaveBlog={unsaveBlog}
            />
          </section>
        </>
      )}

      {showSaved && (
        <section style={{ marginTop: "30px" }}>
          <h2 className="section-title">My Saved Blogs</h2>
          <CardList
            items={mySavedBlogs}   // ✅ Only show saved blogs
            search={search}
            showSaved={true}
            savedBlogIds={savedBlogIds}
            saveBlog={saveBlog}
            unsaveBlog={unsaveBlog}
          />
        </section>
      )}
    </div>
  );
}

export default Blog;