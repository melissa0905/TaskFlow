import { readApiError } from "./ApiError";


export interface CurrentUser {
    id: string;
    fullName: string;
    email: string;
    roles: string[];
}
export interface LoginInput {
    email: string;
    password: string;
}

export interface RegisterInput extends LoginInput {
    fullName: string;
}

async function postWithCsrf(path: string, fallbackMessage: string, body?: LoginInput | RegisterInput): Promise<void> {
    const csrfResponse = await fetch('/api/auth/csrf', {
        credentials: 'same-origin',
        cache: 'no-store',
    });
    if (!csrfResponse.ok) {
        throw await readApiError(
            csrfResponse,
            'Oturum isteği hazırlanamadı.',
        );
    }
    const { token } = (await csrfResponse.json()) as { token: string };
    const response = await fetch(`/api/auth/${path}`, {
        method: 'POST',
        credentials: 'same-origin',
        headers: {
            'X-CSRF-TOKEN': token,
            ...(body ? { 'Content-Type': 'application/json' } : {}),
        },
        ...(body ? { body: JSON.stringify(body) } : {}),
    });
    if (!response.ok) {
        throw await readApiError(response, fallbackMessage);
    }
}
export function registerAccount(input: RegisterInput): Promise<void> {
    return postWithCsrf('register', 'Hesap oluşturulamadı.', input);
}

export function loginAccount(input: LoginInput): Promise<void> {
    return postWithCsrf('login', 'Giriş yapılamadı.', input);
}

export function logoutAccount(): Promise<void> {
    return postWithCsrf('logout', 'Çıkış yapılamadı.');
}

export async function getCurrentUser(
    signal?: AbortSignal,
): Promise<CurrentUser | null> {
    const response = await fetch('/api/auth/me', {
        credentials: 'same-origin',
        cache: 'no-store',
        signal,
    });

    if (response.status === 401) {
        return null;
    }

    if (!response.ok) {
        throw await readApiError(response, 'Oturum kontrol edilemedi.');
    }

    return response.json();
}