import React, { useState } from "react";

function CardList({
  items = [],
  search = "",
  showSaved = false,
  deleteBlog,
  initialSavedIds = [],
  savedBlogIds = [],
  saveBlog,
  unsaveBlog,
}) {
  const [selectedCard, setSelectedCard] = useState(null);

  const currentUser = localStorage.getItem("username");

  // use savedBlogIds from props
  const [savedIds, setSavedIds] = useState(
    savedBlogIds.length ? savedBlogIds : initialSavedIds
  );

  const [copyMessage, setCopyMessage] = useState("");

  const query = search.toLowerCase();

  // Copy to clipboard
  const copyData = (item, e) => {
    if (e) e.stopPropagation();
    navigator.clipboard.writeText(
      `Title: ${item.title}\nDescription: ${item.desc}`
    );
    setCopyMessage("Copied successfully ✓");
    setTimeout(() => setCopyMessage(""), 2000);
  };

  // Save/Unsave
  const toggleSave = (item, e) => {
    e.stopPropagation();

    if (savedIds.includes(item.id)) {
      setSavedIds((prev) => prev.filter((id) => id !== item.id));
      unsaveBlog && unsaveBlog(item.id);
    } else {
      setSavedIds((prev) => [...prev, item.id]);
      saveBlog && saveBlog(item.id);
    }
  };

  // Filter logic
  const filteredItems = items.filter((item) => {
    if (showSaved && !savedIds.includes(item.id)) return false;

    if (query && !item.title.toLowerCase().includes(query)) return false;

    return true;
  });

  const renderCards = (data) =>
    data.map((item) => {
      const isSaved = savedIds.includes(item.id);

      return (
        <div
          key={item.id}
          className="card"
          onClick={() => setSelectedCard(item)}
          style={{ position: "relative" }}
        >
          {/* Save icon */}
          <svg
            onClick={(e) => toggleSave(item, e)}
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            width="22"
            height="22"
            style={{
              position: "absolute",
              top: "10px",
              right: "10px",
              cursor: "pointer",
              fill: isSaved ? "#ffffff" : "none",
              stroke: "#ffffff",
              strokeWidth: "2",
              zIndex: 10,
            }}
          >
            <path d="M6 3h12a1 1 0 0 1 1 1v17l-7-4-7 4V4a1 1 0 0 1 1-1z" />
          </svg>

          <img src={item.image} alt={item.title} />

          <div className="card-content">
            <div className="text-copy-wrapper">
              <div className="text-wrapper">
                <h3>{item.title}</h3>
                <p>{item.desc}</p>
              </div>

              {/* Copy icon */}
              <span className="copy-icon" onClick={(e) => copyData(item, e)}>
                🗍
              </span>
            </div>
          </div>
        </div>
      );
    });

  // Full page view
  if (selectedCard) {
    return (
      <>
        <div className="fullpage-card">
          <button className="go-back-btn" onClick={() => setSelectedCard(null)}>
            ⮌
          </button>

          <div className="fullpage-content">
            <h2>{selectedCard.title}</h2>
            <img src={selectedCard.image} alt={selectedCard.title} />
            <p>{selectedCard.desc}</p>

            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                marginTop: "20px",
              }}
            >
              {/* Delete button */}
              <button
                style={{
                  padding: "6px 12px",
                  borderRadius: "8px",
                  border: "1px solid #374151",
                  background: "#1f2933",
                  color: "#f87171",
                  cursor:
                    selectedCard?.author === currentUser
                      ? "pointer"
                      : "not-allowed",
                }}
                onClick={() => {
                  if (selectedCard?.author === currentUser && deleteBlog) {
                    deleteBlog(selectedCard.id);
                    setSelectedCard(null);
                  }
                }}
                disabled={selectedCard?.author !== currentUser}
              >
                Delete
              </button>

              <span className="copy-icon" onClick={() => copyData(selectedCard)}>
                🗍
              </span>
            </div>
          </div>
        </div>

        {/* Copy toast */}
        {copyMessage && <div className="copy-toast">{copyMessage}</div>}
      </>
    );
  }

  // Default card view
  return (
    <>
      <div className="card-container">
        {filteredItems.length > 0 ? (
          renderCards(filteredItems)
        ) : (
          <div
            style={{
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
              width: "100%",
              minHeight: "200px",
            }}
          >
            <p
              style={{
                color: "#9ca3af",
                fontSize: "16px",
                letterSpacing: "0.5px",
              }}
            >
              No blogs found.
            </p>
          </div>
        )}
      </div>

      {copyMessage && <div className="copy-toast">{copyMessage}</div>}
    </>
  );
}

export default CardList;