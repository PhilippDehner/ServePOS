import { useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import ButtonGroup from "../components/ButtonGroup";
import HomeButton from "../components/HomeButton";
import { setAccessToken } from "../auth";

export default function Login() {
    const navigate = useNavigate();
    const [username, setUsername] = useState("");
    const [pin, setPin] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    async function login(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setIsSubmitting(true);
        setError(null);

        try {
            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || "/api"}/auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, pin }),
            });
            if (!response.ok) {
                setError("Benutzername oder PIN sind nicht korrekt.");
                return;
            }

            const { accessToken } = await response.json() as { accessToken: string };
            setAccessToken(accessToken);
            navigate("/admin");
        } catch (err) {
            console.error("Failed to log in:", err);
            setError("Die Anmeldung konnte nicht durchgeführt werden.");
        } finally {
            setIsSubmitting(false);
        }
    }

    return <div>
        <ButtonGroup>
            <HomeButton />
        </ButtonGroup>
        <form onSubmit={login} className="mx-auto mt-12 flex max-w-sm flex-col gap-4">
            <h1 className="text-2xl font-bold">Anmelden</h1>
            <label className="flex flex-col gap-1">
                Benutzername
                <input required value={username} onChange={(event) => setUsername(event.target.value)} className="border rounded px-3 py-2" />
            </label>
            <label className="flex flex-col gap-1">
                PIN
                <input required type="password" inputMode="numeric" value={pin} onChange={(event) => setPin(event.target.value)} className="border rounded px-3 py-2" />
            </label>
            {error && <p className="text-red-600">{error}</p>}
            <button type="submit" disabled={isSubmitting} className="rounded bg-blue-600 px-4 py-2 text-white disabled:opacity-50">
                Anmelden
            </button>
        </form>
    </div>;
}
