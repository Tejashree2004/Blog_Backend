import React, { useState, useRef } from "react";
import { createBlog } from "../api/api";
import { useNavigate } from "react-router-dom";

function CreateBlog() {
  const [title, setTitle] = useState("");
  const [desc, setDesc] = useState("");
  const [file, setFile] = useState(null);

  const fileRef = useRef(null);
  const navigate = useNavigate();

  // Go Back
  const handleGoBack = () => {
    navigate("/blog");
  };

  // Open file picker
  const openFilePicker = () => {
    fileRef.current.click();
  };

  // Clear file
  const clearFile = () => {
    setFile(null);
    fileRef.current.value = "";
  };

  // Submit
  const handleSubmit = async (e) => {
    e.preventDefault();

    const newBlog = {
      title,
      desc,
      image: file
        ? URL.createObjectURL(file)
        : "https://picsum.photos/300/200?random",
      category: "blog",
    };

    try {
      await createBlog(newBlog);
      navigate("/blog");
    } catch (err) {
      console.error(err);
      alert("Error creating blog");
    }
  };

  return (
    <div className="create-page" style={{ position: "relative" }}>
      <button className="go-back-btn" onClick={handleGoBack}>
        ⮌
      </button>

      <form className="create-blog-form" onSubmit={handleSubmit}>
        <h2 className="create-title">Create a New Blog</h2>

        <input
          type="text"
          placeholder="Blog Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          required
        />

        <div style={{ marginTop: "20px" }}>
          <input
            type="file"
            ref={fileRef}
            style={{ display: "none" }}
            onChange={(e) => setFile(e.target.files[0])}
          />

          <div
            onClick={openFilePicker}
            style={{
                     
              display: "flex",
              alignItems: "center",
              gap: "10px",
              width: "100%",
              padding: "12px",
              borderRadius: "6px",
              border: "1px dashed #3b82f6",
              cursor: "pointer",
              background: "transparent",
              color: "white",
            }}
          >
            <img
              src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT95rKJldDyjAtvUJXQ4RpytJGo5QT8yooACQ&s"
              alt="attach"
              style={{
                width: "18px",
                height: "18px",
                filter: "invert(1)",
                opacity: "0.6",
              }}
            />

            <span style={{ opacity: file ? 1 : 0.6 }}>
              {file ? file.name : "Attach file"}
            </span>
          </div>

          {file && (
            <button
              type="button"
              onClick={clearFile}
              style={{
                marginTop: "10px",
                padding: "6px 12px",
                borderRadius: "6px",
                cursor: "pointer",
                background: "#1e293b",
                color: "#f87171",
                border: "1px solid #374151",
              }}
            >
              Remove File
            </button>
          )}
        </div>

        <textarea
          placeholder="Description"
          rows="6"
          value={desc}
          onChange={(e) => setDesc(e.target.value)}
          required
          style={{ marginTop: "25px" }}
        ></textarea>

        <div className="btn-center" style={{ marginTop: "25px" }}>
          <button type="submit" className="create-btn">
            Create
          </button>
        </div>
      </form>
    </div>
  );
}

export default CreateBlog;
