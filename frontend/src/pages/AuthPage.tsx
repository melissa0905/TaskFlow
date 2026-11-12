import { useState } from 'react';
import { Link, Navigate, useNavigate } from 'react-router';

import { ApiError } from '../api/ApiError';
import { registerAccount } from '../api/auth';
import { PageHeader } from '../components/PageHeader';
import { useAuth } from '../auth/authcontext';

type AuthPageProps = {
  mode: 'login' | 'register';
};

export default function AuthPage({ mode }: AuthPageProps) {
  const navigate = useNavigate();
  const { user, login } = useAuth();
  const registering = mode === 'register';

  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [pending, setPending] = useState(false);
  const [error, setError] = useState('');

  async function submit() {
    if (pending) return;

    setPending(true);
    setError('');

    try {
      if (registering) {
        await registerAccount({
          fullName: fullName.trim(),
          email: email.trim(),
          password,
        });

        navigate('/login', { replace: true });
      } else {
        await login({ email: email.trim(), password });
        navigate('/dashboard', { replace: true });
      }
    } catch (error: unknown) {
      if (error instanceof ApiError) {
        const messages = Object.values(error.fieldErrors).flat();
        setError(messages.join(' ') || error.message);
      } else {
        setError(
          error instanceof Error ? error.message : 'İşlem tamamlanamadı.',
        );
      }
    } finally {
      setPending(false);
    }
  }

  if (user) {
    return <Navigate to="/dashboard" replace />;
  }

  return (
    <main className="auth-page">
      <PageHeader title={registering ? 'Hesap oluştur' : 'Giriş yap'} description={''} />

      <form
        className="auth-form"
        onSubmit={(event) => {
          event.preventDefault();
          void submit();
        }}>
        {registering && (
          <label>
            Ad soyad
            <input
              value={fullName}
              onChange={(event) => setFullName(event.target.value)}
              autoComplete="name"
              maxLength={150}
              required
              disabled={pending}
            />
          </label>
        )}

        <label>
          E-posta
          <input
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            autoComplete="username"
            maxLength={256}
            required
            disabled={pending}
          />
        </label>

        <label>
          Parola
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete={registering ? 'new-password' : 'current-password'}
            minLength={registering ? 12 : undefined}
            maxLength={128}
            required
            disabled={pending}
          />
        </label>

        {error && <p className="form-error" role="alert">{error}</p>}

        <button type="submit" disabled={pending}>
          {pending ? 'Bekleniyor...' : registering ? 'Kayıt ol' : 'Giriş yap'}
        </button>
      </form>

      <p>
        <Link to={registering ? '/login' : '/register'}>
          {registering ? 'Zaten hesabın var mı? Giriş yap' : 'Hesabın yok mu? Kayıt ol'}
        </Link>
      </p>
    </main>
  );
}