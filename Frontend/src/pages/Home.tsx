import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { clearAccessToken, isAuthenticated } from "../auth";

export default function Home() {
  const navigate = useNavigate();
  const [authenticated, setAuthenticated] = useState(isAuthenticated);

  function logout() {
    clearAccessToken();
    setAuthenticated(false);
  }

  return (
    <div className="min-h-screen flex flex-col gap-6 p-6">

      <h1 className="text-3xl font-bold mb-6">
        POS System
      </h1>

      {authenticated ? (
        <>
          <button
            onClick={() => navigate("/serve")}
            className="w-full max-w-sm p-6 text-xl rounded-xl"
          >
            Bedienung
          </button>

          <button
            className="w-full max-w-sm p-6 text-xl rounded-xl"
            onClick={() => navigate("/admin")}
          >
            Admin
          </button>

          <button
            className="w-full max-w-sm p-6 text-xl rounded-xl"
            onClick={logout}
          >
            Abmelden
          </button>
        </>
      ) : (
        <button
          className="w-full max-w-sm p-6 text-xl rounded-xl"
          onClick={() => navigate("/login")}
        >
          Login
        </button>
      )}

    </div>
  );
}
