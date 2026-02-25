import React from "react";
import Blog from "./Blog.jsx";

function Landing({ blogs, deleteBlog, savedBlogIds, saveBlog, unsaveBlog }) {
  return (
    <div className="page-wrapper">
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