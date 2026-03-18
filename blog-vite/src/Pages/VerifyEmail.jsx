import React, { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { verifyEmailOtp } from "../api/api";

function VerifyEmail() {
  const [otp, setOtp] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();

  // ✅ FIX: always lowercase
  const email = location.state?.email?.trim().toLowerCase() || "";

  const handleVerify = async (e) => {
    e.preventDefault();
    setError("");

    if (!email) {
      setError("Email not found. Please signup again.");
      return;
    }

    const trimmedOtp = otp.trim();

    if (!trimmedOtp) {
      setError("Please enter the OTP sent to your email.");
      return;
    }

    try {
      setLoading(true);

      const res = await verifyEmailOtp({
        email: email,
        otp: trimmedOtp,
      });

      console.log("VERIFY RESPONSE:", res);

      // ✅ FIXED
      if (res?.message) {
        alert(res.message);
        navigate("/login");
      } else {
        setError("OTP verification failed: Invalid response");
      }
    } catch (err) {
      console.error("OTP verification error:", err);

      setError(
        err.response?.data?.message ||
        "OTP verification failed. Try again."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <form onSubmit={handleVerify} autoComplete="off">
        <h2 style={{ textAlign: "center", marginBottom: "20px" }}>
          Verify your email
        </h2>

        <input
          type="text"
          placeholder="Enter OTP"
          value={otp}
          onChange={(e) => setOtp(e.target.value)}
          disabled={loading}
          required
        />

        {error && (
          <p
            style={{
              color: "#f87171",
              textAlign: "center",
              fontSize: "14px",
              marginTop: "10px",
            }}
          >
            {error}
          </p>
        )}

        <button className="auth-btn" type="submit" disabled={loading}>
          {loading ? "Verifying..." : "Verify and Continue"}
        </button>
      </form>
    </div>
  );
}

export default VerifyEmail;