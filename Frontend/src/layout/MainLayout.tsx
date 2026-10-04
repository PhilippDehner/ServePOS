import { Outlet } from "react-router-dom";
import InstallPrompt from "../components/InstallPrompt";

export default function MainLayout() {
  return (
    <div className="min-h-screen p-2 md:p-8">
      <Outlet />
      <InstallPrompt />
    </div>
  );
}
