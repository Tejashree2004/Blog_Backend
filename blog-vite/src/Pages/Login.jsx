import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { loginUser } from "../api/api"; // ✅ Import login API

function Login() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    setUsername("");
    setPassword("");
  }, []);

  // ✅ Handle real login via backend
  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");
    if (!username || !password) return setError("Please fill all fields");

    try {
      setLoading(true);
      const res = await loginUser({ username, password });
      const user = res.data;

      // Save user info in localStorage for auth
      localStorage.setItem("userId", user.id);
      localStorage.setItem("username", user.username);
      localStorage.removeItem("userType"); // remove guest flag if any

      navigate("/blog");
    } catch (err) {
      console.error(err);
      setError(err.response?.data?.message || "Login failed");
    } finally {
      setLoading(false);
    }
  };

  // ✅ Guest login
  const handleGuest = () => {
    localStorage.setItem("userType", "guest");
    localStorage.removeItem("userId");
    localStorage.removeItem("username");
    navigate("/blog");
  };

  return (
    <div className="login-container">
      <form onSubmit={handleLogin} autoComplete="off">
        <h2 style={{ textAlign: "center", marginBottom: "20px" }}>Login</h2>

        {error && (
          <p style={{ color: "#f87171", textAlign: "center", marginBottom: "10px" }}>
            {error}
          </p>
        )}

        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          disabled={loading}
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={loading}
        />

        <button className="auth-btn" type="submit" disabled={loading}>
          {loading ? "Logging in..." : "Login"}
        </button>

        <p style={{ textAlign: "center", fontSize: "14px", marginTop: "10px" }}>
          Or{" "}
          <span
            onClick={handleGuest}
            style={{ color: "#3b82f6", cursor: "pointer", fontWeight: "500" }}
          >
            Continue as Guest
          </span>
        </p>

        <p style={{ textAlign: "center", fontSize: "14px", marginTop: "12px" }}>
          Don't have an account?{" "}
          <span
            onClick={() => navigate("/signup")}
            style={{ color: "#3b82f6", cursor: "pointer" }}
          >
            Signup
          </span>
        </p>
      </form>
    </div>
  );
}

export default Login;