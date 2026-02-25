import React, { useState, useEffect } from "react";
import axios from "axios";
import Navbar from "../components/Navbar.jsx";
import CardList from "../components/CardList.jsx";

function Blog({ userId }) {
  const [myBlogs, setMyBlogs] = useState([]);
  const [myFeed, setMyFeed] = useState([]);
  const [savedBlogIds, setSavedBlogIds] = useState([]);
  const [search, setSearch] = useState("");
  const [showSaved, setShowSaved] = useState(false);

  const baseURL = "http://localhost:5111/api/blogs";

  // Fetch blogs + saved blogs
  const fetchData = async () => {
    try {
      const [blogsRes, feedRes, savedRes] = await Promise.all([
        axios.get(`${baseURL}/myblogs`),
        axios.get(`${baseURL}/feed`),
        axios.get(`${baseURL}/saved`, { params: { userId } }),
      ]);
      setMyBlogs(blogsRes.data);
      setMyFeed(feedRes.data);
      setSavedBlogIds(savedRes.data.map((b) => b.Id));
    } catch (err) {
      console.error("Fetch error:", err);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  // Delete blog
  const handleDelete = async (id) => {
    try {
      const res = await axios.delete(`${baseURL}/${id}`);
      if (res.status === 200) {
        setMyBlogs((prev) => prev.filter((b) => b.Id !== id));
      }
    } catch (err) {
      console.error("Delete failed:", err);
    }
  };

  return (
    <div className="page-wrapper">
      <Navbar search={search} setSearch={setSearch} setShowSaved={setShowSaved} />

      {/* My Blogs Section */}
      <section style={{ marginTop: "30px" }}>
        <h2 className="section-title">My Blogs</h2>
        <CardList
          items={myBlogs}
          search={search}
          showSaved={showSaved}
          savedBlogIds={savedBlogIds}
          deleteBlog={handleDelete} // ✅ add delete feature
        />
      </section>

      {/* My Feed Section */}
      <section style={{ marginTop: "50px" }}>
        <h2 className="section-title">My Feed</h2>
        <CardList
          items={myFeed}
          search={search}
          showSaved={showSaved}
          savedBlogIds={savedBlogIds}
          deleteBlog={handleDelete} // ✅ add delete feature
        />
      </section>
    </div>
  );
}

export default Blog;