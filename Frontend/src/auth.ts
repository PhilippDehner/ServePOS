const accessTokenKey = "servepos.access-token";

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
