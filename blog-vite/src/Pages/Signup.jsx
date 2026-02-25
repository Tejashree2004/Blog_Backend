import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { signupUser } from "../api/api"; // ✅ make sure to create this API call

function Signup() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSignup = async (e) => {
    e.preventDefault();
    setError("");

    if (!email || !password) {
      setError("Please fill all fields");
      return;
    }

    try {
      setLoading(true);
      const res = await signupUser({ email, password });
      // Save user info if needed (like userId)
      localStorage.setItem("signupEmail", email); // optional for verify
      navigate("/verify-email");
    } catch (err) {
      console.error(err);
      setError(err.response?.data?.message || "Signup failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <form onSubmit={handleSignup} autoComplete="off">
        <h2 style={{ textAlign: "center", marginBottom: "20px" }}>Signup</h2>

        {error && (
          <p style={{ color: "#f87171", textAlign: "center", marginBottom: "10px" }}>
            {error}
          </p>
        )}

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          disabled={loading}
          required
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={loading}
          required
        />

        <button className="auth-btn" type="submit" disabled={loading}>
          {loading ? "Signing up..." : "Signup"}
        </button>

        <p style={{ textAlign: "center", fontSize: "14px", marginTop: "12px" }}>
          Already have an account?{" "}
          <span
            onClick={() => navigate("/login")}
            style={{ color: "#3b82f6", cursor: "pointer" }}
          >
            Login
          </span>
        </p>
      </form>
    </div>
  );
}

export default Signup;