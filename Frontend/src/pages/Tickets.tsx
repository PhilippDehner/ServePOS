import { useEffect, useState } from "react";
import { getAuthorizationHeaders } from "../auth";
import HomeButton from "../components/HomeButton";

type Station = "Kitchen" | "Drinks";
type Ticket = {
    id: number;
    orderId: number;
    tableId: string;
    station: Station;
    printCount: number;
    items: Array<{ orderItemId: number; name: string; specialInstructions?: string }>;
};

const baseUrl = import.meta.env.VITE_API_BASE_URL || "/api";

export default function Tickets() {
    const [station, setStation] = useState<Station>("Kitchen");
    const [tickets, setTickets] = useState<Ticket[]>([]);
    const [error, setError] = useState<string | null>(null);

    async function loadTickets() {
        const response = await fetch(`${baseUrl}/tickets/open/${station}`, { headers: getAuthorizationHeaders() });
        if (!response.ok) {
            setError("Bons konnten nicht geladen werden.");
            return;
        }
        setTickets(await response.json() as Ticket[]);
        setError(null);
    }

    useEffect(() => { void loadTickets(); }, [station]);

    async function issue(ticketId: number, orderItemId: number) {
        const response = await fetch(`${baseUrl}/tickets/${ticketId}/issue`, {
            method: "POST",
            headers: { "Content-Type": "application/json", ...getAuthorizationHeaders() },
            body: JSON.stringify({ orderItemIds: [orderItemId] }),
        });
        if (!response.ok) {
            setError("Position konnte nicht als ausgegeben markiert werden.");
            return;
        }
        void loadTickets();
    }

    async function reprint(ticketId: number) {
        const response = await fetch(`${baseUrl}/tickets/${ticketId}/reprint`, {
            method: "POST",
            headers: getAuthorizationHeaders(),
        });
        if (!response.ok) {
            setError("Nachdruck konnte nicht erstellt werden.");
            return;
        }
        void loadTickets();
    }

    return <main className="space-y-4">
        <div className="flex gap-2">
            <HomeButton />
            <button onClick={() => setStation("Kitchen")} disabled={station === "Kitchen"}>Küche</button>
            <button onClick={() => setStation("Drinks")} disabled={station === "Drinks"}>Getränke</button>
        </div>
        <h1>{station === "Kitchen" ? "Küchenbons" : "Getränkebons"}</h1>
        {error && <p className="text-red-600">{error}</p>}
        {tickets.map(ticket => <section key={ticket.id} className="rounded border p-4">
            <div className="flex justify-between"><strong>Tisch {ticket.tableId}</strong><span>Bon #{ticket.id}</span></div>
            {ticket.printCount > 1 && <p className="text-amber-700">Nachdruck {ticket.printCount - 1}</p>}
            {ticket.items.map(item => <div key={item.orderItemId} className="mt-2 flex justify-between gap-2">
                <span>{item.name}{item.specialInstructions ? ` - ${item.specialInstructions}` : ""}</span>
                <button onClick={() => void issue(ticket.id, item.orderItemId)}>Ausgegeben</button>
            </div>)}
            <button className="mt-3" onClick={() => void reprint(ticket.id)}>Nachdruck</button>
        </section>)}
        {tickets.length === 0 && <p>Keine offenen Bons.</p>}
    </main>;
}
