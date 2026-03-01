import React, { useState } from "react";
import Navbar from "../components/Navbar.jsx";
import CardList from "../components/CardList.jsx";

function Blog({
  blogs,
  deleteBlog,
  savedBlogIds,
  saveBlog,
  unsaveBlog,
}) {
  const [search, setSearch] = useState("");
  const [showSaved, setShowSaved] = useState(false);

  // Only show blogs created by the user in "My Blogs"
  const myBlogs = blogs.filter((b) => b.isUserCreated);

  // Show all active blogs that are NOT user-created in "My Feed"
  const myFeed = blogs.filter((b) => b.isActive && !b.isUserCreated);

  return (
    <div className="page-wrapper">
      <Navbar
        search={search}
        setSearch={setSearch}
        setShowSaved={setShowSaved}
      />

      {/* My Blogs Section */}
      <section style={{ marginTop: "30px" }}>
        <h2 className="section-title">My Blogs</h2>
        <CardList
          items={myBlogs}
          search={search}
          showSaved={showSaved}
          savedBlogIds={savedBlogIds}
          deleteBlog={deleteBlog}
          saveBlog={saveBlog}
          unsaveBlog={unsaveBlog}
        />
      </section>

      {/* Feed Section */}
      <section style={{ marginTop: "50px" }}>
        <h2 className="section-title">My Feed</h2>
        <CardList
          items={myFeed}
          search={search}
          showSaved={showSaved}
          savedBlogIds={savedBlogIds}
          deleteBlog={deleteBlog}
          saveBlog={saveBlog}
          unsaveBlog={unsaveBlog}
        />
      </section>
    </div>
  );
}

export default Blog;