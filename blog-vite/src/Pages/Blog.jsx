import React, { useState } from "react";
import Navbar from "../components/Navbar.jsx";
import CardList from "../components/CardList.jsx";

function Blog({ blogs, deleteBlog, savedBlogIds, saveBlog, unsaveBlog }) {
  const [search, setSearch] = useState("");
  const [showSaved, setShowSaved] = useState(false);

  // ⭐ Updated to handle guest users
  const currentUser =
    localStorage.getItem("username") || localStorage.getItem("guestId");

  // My Blogs → only blogs created by current user
  const myBlogs = blogs.filter(
    (b) => b.isUserCreated && b.author === currentUser
  );

  // My Feed → blogs from other users + default blogs
  const myFeed = blogs.filter(
    (b) => !b.isUserCreated || (b.isUserCreated && b.author !== currentUser)
  );

  return (
    <div className="page-wrapper">
      <Navbar
        search={search}
        setSearch={setSearch}
        setShowSaved={setShowSaved}
      />

      {!showSaved && (
        <>
          <section style={{ marginTop: "30px" }}>
            <h2 className="section-title">My Blogs</h2>
            <CardList
              items={myBlogs}
              search={search}
              savedBlogIds={savedBlogIds}
              deleteBlog={deleteBlog}
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
              deleteBlog={deleteBlog}
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
            items={blogs}
            search={search}
            showSaved={true}
            savedBlogIds={savedBlogIds}
            deleteBlog={deleteBlog}
            saveBlog={saveBlog}
            unsaveBlog={unsaveBlog}
          />
        </section>
      )}
    </div>
  );
}

export default Blog;