import React, { useState, useRef, useEffect } from "react";
import axiosInstance from "../api/axiosInstance";
import { useNavigate } from "react-router-dom";

function CreateBlog() {
  const [title, setTitle] = useState("");
  const [desc, setDesc] = useState("");
  const [file, setFile] = useState(null);

  const [popup, setPopup] = useState({
    show: false,
    message: "",
    type: "",
  });

  const fileRef = useRef(null);
  const navigate = useNavigate();
  const timerRef = useRef(null);

  const handleGoBack = () => {
    navigate("/blog");
  };

  const openFilePicker = () => {
    fileRef.current.click();
  };

  const clearFile = () => {
    setFile(null);
    fileRef.current.value = "";
  };

  const closePopup = () => {
    setPopup({ show: false, message: "", type: "" });
    if (timerRef.current) clearTimeout(timerRef.current);
  };

  useEffect(() => {
    return () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, []);

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
      await axiosInstance.post("/blogs", newBlog);

      setPopup({
        show: true,
        message: "Your blog has been created successfully.",
        type: "success",
      });

      // ✅ Professional success timing (1 sec)
      timerRef.current = setTimeout(() => {
        closePopup();
        navigate("/blog");
      }, 1000);

    } catch (err) {
      console.error("Create error:", err.response?.data || err.message);

      setPopup({
        show: true,
        message: "Something went wrong while creating the blog.",
        type: "error",
      });

      // ✅ Professional error timing (8 sec)
      timerRef.current = setTimeout(() => {
        closePopup();
      }, 8000);
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
                background: "#0f172a",
                color: "#3b82f6",
                border: "1px solid #1e293b",
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

      {popup.show && (
        <div
          onClick={closePopup}
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            width: "100%",
            height: "100%",
            background: "rgba(0,0,0,0.35)",
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            zIndex: 999,
          }}
        >
          <div
            onClick={(e) => e.stopPropagation()}
            style={{
              width: "400px",
              background: "#111827",
              borderRadius: "12px",
              border: "1px solid #1f2937",
              boxShadow: "0 10px 30px rgba(0,0,0,0.4)",
              overflow: "hidden",
            }}
          >
            <div
              style={{
                padding: "16px",
                background: "#1e3a8a",
                color: "white",
                fontWeight: "500",
                textAlign: "center",
                position: "relative",
              }}
            >
              {popup.type === "success" ? "Success" : "Error"}

              <span
                onClick={closePopup}
                style={{
                  position: "absolute",
                  right: "15px",
                  top: "10px",
                  cursor: "pointer",
                  fontSize: "16px",
                }}
              >
                ✕
              </span>
            </div>

            <div
              style={{
                padding: "25px",
                textAlign: "center",
                color: "#e5e7eb",
                fontSize: "14px",
              }}
            >
              {popup.message}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default CreateBlog;