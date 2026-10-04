import { Routes, Route } from "react-router-dom";
import MainLayout from "./layout/MainLayout";
import Home from "./pages/Home";
import Serve from "./pages/Serve";
import Login from "./pages/Login";
import Admin from "./pages/Admin";
import Tickets from "./pages/Tickets";

function App() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/" element={<Home />} />
        <Route path="/serve" element={<Serve />} />
        <Route path="/login" element={<Login />} />
        <Route path="/admin" element={<Admin />} />
        <Route path="/tickets" element={<Tickets />} />
      </Route>
    </Routes>
  );
}

export default App;
