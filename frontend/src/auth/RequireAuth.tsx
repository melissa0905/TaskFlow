import { Navigate, Outlet } from "react-router";
import { useAuth } from "./authcontext";

export default function RequireAuth(){
    const { user } = useAuth();

    return user ? <Outlet /> : <Navigate to="/login" replace />;}