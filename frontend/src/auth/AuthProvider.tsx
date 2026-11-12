
import { useEffect, useState } from 'react';
import type { ReactNode } from 'react';

import {
  getCurrentUser,
  loginAccount,
  logoutAccount,
} from '../api/auth';

import type { CurrentUser, LoginInput } from '../api/auth';
import { AuthContext } from './authcontext';

export default function AuthProvider({
  children,
}: {
  children: ReactNode;
}) {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [checking, setChecking] = useState(true);
  const [checkError, setCheckError] = useState('');

  useEffect(() => {
    const controller = new AbortController();

    async function checkSession() {
      try {
        const currentUser = await getCurrentUser(controller.signal);

        if (!controller.signal.aborted) {
          setUser(currentUser);
        }
      } catch (error: unknown) {
        if (!controller.signal.aborted) {
          setCheckError(
            error instanceof Error
              ? error.message
              : 'Oturum kontrol edilemedi.',
          );
        }
      } finally {
        if (!controller.signal.aborted) {
          setChecking(false);
        }
      }
    }

    void checkSession();

    return () => controller.abort();
  }, []);

  async function login(input: LoginInput) {
    await loginAccount(input);

    const currentUser = await getCurrentUser();

    if (!currentUser) {
      throw new Error('Oturum doğrulanamadı. Lütfen tekrar giriş yap.');
    }

    setUser(currentUser);
  }

  async function logout() {
    await logoutAccount();
    setUser(null);
  }

  if (checking) {
    return <p role="status">Oturum kontrol ediliyor...</p>;
  }

  if (checkError) {
    return (
      <div>
        <p role="alert">{checkError}</p>
        <button type="button" onClick={() => window.location.reload()}>
          Tekrar dene
        </button>
      </div>
    );
  }

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}