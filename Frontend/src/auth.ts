const accessTokenKey = "servepos.access-token";
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || "/api";

export function getAccessToken(): string | null {
    return localStorage.getItem(accessTokenKey);
}

export function isAuthenticated(): boolean {
    const accessToken = getAccessToken();
    if (!accessToken) {
        return false;
    }

    try {
        const payload = accessToken.split(".")[1];
        if (!payload) {
            throw new Error("JWT payload is missing.");
        }

        const decodedPayload = JSON.parse(
            atob(payload.replace(/-/g, "+").replace(/_/g, "/")),
        ) as { exp?: number };
        if (!decodedPayload.exp || decodedPayload.exp * 1000 <= Date.now()) {
            clearAccessToken();
            return false;
        }

        return true;
    } catch {
        clearAccessToken();
        return false;
    }
}

export function setAccessToken(accessToken: string): void {
    localStorage.setItem(accessTokenKey, accessToken);
}

export function clearAccessToken(): void {
    localStorage.removeItem(accessTokenKey);
}

export function getAuthorizationHeaders(): Record<string, string> {
    const accessToken = getAccessToken();
    return accessToken && isAuthenticated() ? { Authorization: `Bearer ${accessToken}` } : {};
}

export function authorizedFetch(input: RequestInfo | URL, init?: RequestInit): Promise<Response> {
    const headers = new Headers(init?.headers);
    for (const [name, value] of Object.entries(getAuthorizationHeaders())) {
        headers.set(name, value);
    }

    return fetch(input, {
        ...init,
        headers,
    }).then(response => {
        if (response.status === 401) {
            clearAccessToken();
            const from = `${window.location.pathname}${window.location.search}${window.location.hash}`;
            window.location.assign(`/login?from=${encodeURIComponent(from)}`);
        }

        return response;
    });
}

export async function logout(): Promise<void> {
    try {
        await fetch(`${apiBaseUrl}/auth/logout`, {
            method: "POST",
            headers: getAuthorizationHeaders(),
        });
    } finally {
        clearAccessToken();
    }
}
