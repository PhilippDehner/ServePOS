const accessTokenKey = "servepos.access-token";

export function getAccessToken(): string | null {
    return localStorage.getItem(accessTokenKey);
}

export function setAccessToken(accessToken: string): void {
    localStorage.setItem(accessTokenKey, accessToken);
}

export function clearAccessToken(): void {
    localStorage.removeItem(accessTokenKey);
}

export function getAuthorizationHeaders(): Record<string, string> {
    const accessToken = getAccessToken();
    return accessToken ? { Authorization: `Bearer ${accessToken}` } : {};
}
