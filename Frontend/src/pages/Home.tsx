import { useNavigate } from "react-router-dom";

export default function Home() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen flex flex-col gap-6 p-6">

      <h1 className="text-3xl font-bold mb-6">
        POS System
      </h1>

      <button
        onClick={() => navigate("/serve")}
        className="w-full max-w-sm p-6 text-xl rounded-xl"
      >
        Bedienung
      </button>

      <button
        className="w-full max-w-sm p-6 text-xl rounded-xl"
        onClick={() => navigate("/login")}
      >
        Login
      </button>

      <button
        className="w-full max-w-sm p-6 text-xl rounded-xl"
        onClick={() => navigate("/admin")}
      >
        Admin
      </button>

    </div>
  );
}
