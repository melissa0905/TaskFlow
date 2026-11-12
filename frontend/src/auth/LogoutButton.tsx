import { useState } from "react";
import { useAuth } from "./authcontext";

export default function LogoutButton() {
    const { user, logout } = useAuth();
    const [pending, setPending] = useState(false);
    const [error, setError] = useState('');

    async function handleLogout() {
        setPending(true);
        setError('');
        try {
            await logout();
        } catch (error: unknown) {
            setError(error instanceof Error ? error.message : 'Oturum kapatılamadı.');
        } finally {
            setPending(false);
        }
    }
    return(
        <>
        <span>{user?.fullName}</span>
        <button onClick={handleLogout} disabled={pending}>
            {pending ? 'Çıkış yapılıyor...' : 'Çıkış yap'}
        </button>
        {error && <p className="error">{error}</p>}
        </>
    );
}