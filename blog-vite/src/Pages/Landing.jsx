import React from "react";
import { useNavigate } from "react-router-dom";
import Blog from "./Blog.jsx";

function Landing({ blogs, deleteBlog, savedBlogIds, saveBlog, unsaveBlog }) {
  const navigate = useNavigate();

  const handleCreateBlog = () => {
    navigate("/create-blog");
  };

  return (
    <div className="page-wrapper">
      <div style={{ margin: "20px 0", textAlign: "center" }}>
        <button
          onClick={handleCreateBlog}
          style={{
            padding: "10px 20px",
            backgroundColor: "#3b82f6",
            color: "#fff",
            border: "none",
            borderRadius: "6px",
            cursor: "pointer",
            fontSize: "16px"
          }}
        >
          Create Blog
        </button>
      </div>

      <Blog
        blogs={blogs}
        deleteBlog={deleteBlog}
        savedBlogIds={savedBlogIds}
        saveBlog={saveBlog}
        unsaveBlog={unsaveBlog}
      />
    </div>
  );
}

export default Landing;