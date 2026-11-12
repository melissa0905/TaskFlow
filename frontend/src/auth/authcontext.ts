import { createContext, useContext } from "react";
import type { CurrentUser, LoginInput } from "../api/auth";

interface AuthContextValue {
    user: CurrentUser | null;
    login: (input: LoginInput) => Promise<void>;
    logout: () => Promise<void>;
}
export const AuthContext = createContext<AuthContextValue | null>(null);
export function useAuth(): AuthContextValue {
    const context = useContext(AuthContext);
    if(!context){
        throw new Error("useAuth, AuthProvider içinde kullanılmalıdır.");
    }
    return context;
}