import type { ReactNode } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { getAccessToken } from "../auth";

type Props = {
    children: ReactNode;
};

export default function RequireAuthentication({ children }: Props) {
    const location = useLocation();

    if (!getAccessToken()) {
        return <Navigate to="/login" replace state={{ from: location }} />;
    }

    return <>{children}</>;
}
