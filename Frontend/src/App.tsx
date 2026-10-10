import { Routes, Route } from "react-router-dom";
import MainLayout from "./layout/MainLayout";
import Home from "./pages/Home";
import Serve from "./pages/Serve";
import Login from "./pages/Login";
import Admin from "./pages/Admin";
import Tickets from "./pages/Tickets";
import RequireAuthentication from "./components/RequireAuthentication";

function App() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route
          path="/serve"
          element={<RequireAuthentication><Serve /></RequireAuthentication>}
        />
        <Route
          path="/admin"
          element={<RequireAuthentication><Admin /></RequireAuthentication>}
        />
        <Route
          path="/tickets"
          element={<RequireAuthentication><Tickets /></RequireAuthentication>}
        />
      </Route>
    </Routes>
  );
}

export default App;
