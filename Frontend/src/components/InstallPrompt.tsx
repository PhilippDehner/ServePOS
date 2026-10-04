import { useEffect, useState } from "react";

type BeforeInstallPromptEvent = Event & {
    prompt: () => Promise<void>;
    userChoice: Promise<{ outcome: "accepted" | "dismissed" }>;
};

export default function InstallPrompt() {
    const [installPrompt, setInstallPrompt] = useState<BeforeInstallPromptEvent | null>(null);

    useEffect(() => {
        function handleBeforeInstallPrompt(event: Event) {
            event.preventDefault();
            setInstallPrompt(event as BeforeInstallPromptEvent);
        }

        function handleAppInstalled() {
            setInstallPrompt(null);
        }

        window.addEventListener("beforeinstallprompt", handleBeforeInstallPrompt);
        window.addEventListener("appinstalled", handleAppInstalled);
        return () => {
            window.removeEventListener("beforeinstallprompt", handleBeforeInstallPrompt);
            window.removeEventListener("appinstalled", handleAppInstalled);
        };
    }, []);

    async function install() {
        if (!installPrompt) {
            return;
        }

        await installPrompt.prompt();
        await installPrompt.userChoice;
        setInstallPrompt(null);
    }

    if (!installPrompt) {
        return null;
    }

    return (
        <button
            type="button"
            onClick={() => void install()}
            className="fixed bottom-4 right-4 z-50 bg-blue-700 text-white shadow-lg"
        >
            ServePOS installieren
        </button>
    );
}
