import { useCallback, useEffect, useState } from "react";
import type { FormEvent } from "react";

import { getAuthorizationHeaders } from "../auth";

type EventInfo = {
    id: number;
    name: string;
    startsAt: string;
    isActive: boolean;
    isCompleted: boolean;
};

type Role = {
    id: number;
    name: string;
};

type UserInfo = {
    id: number;
    username: string;
    roleId: number;
    roleName: string;
    isActive: boolean;
};

type NamedResource = {
    id: number;
    name: string;
};

type ServingStation = NamedResource & {
    eventId: number;
};

type MenuItemType = NamedResource & {
    servingStationId: number;
};

const baseUrl = import.meta.env.VITE_API_BASE_URL || "/api";

async function request<T>(path: string, method = "GET", body?: unknown): Promise<T> {
    const response = await fetch(`${baseUrl}${path}`, {
        method,
        headers: {
            ...getAuthorizationHeaders(),
            ...(body === undefined ? {} : { "Content-Type": "application/json" }),
        },
        body: body === undefined ? undefined : JSON.stringify(body),
    });

    if (!response.ok) {
        const detail = await response.text();
        throw new Error(detail || `Request failed with status ${response.status}.`);
    }

    return response.status === 204 ? undefined as T : response.json() as Promise<T>;
}

export default function EventAdministration() {
    const [events, setEvents] = useState<EventInfo[]>([]);
    const [roles, setRoles] = useState<Role[]>([]);
    const [users, setUsers] = useState<UserInfo[]>([]);
    const [tables, setTables] = useState<NamedResource[]>([]);
    const [stations, setStations] = useState<ServingStation[]>([]);
    const [itemTypes, setItemTypes] = useState<MenuItemType[]>([]);
    const [selectedEventId, setSelectedEventId] = useState<number | null>(null);
    const [eventName, setEventName] = useState("");
    const [eventStartsAt, setEventStartsAt] = useState("");
    const [username, setUsername] = useState("");
    const [pin, setPin] = useState("");
    const [roleId, setRoleId] = useState("");
    const [tableName, setTableName] = useState("");
    const [stationName, setStationName] = useState("");
    const [itemTypeName, setItemTypeName] = useState("");
    const [stationId, setStationId] = useState("");
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const selectedEvent = events.find((event) => event.id === selectedEventId) ?? null;
    const activeEvent = events.find((event) => event.isActive) ?? null;
    const eventStations = stations.filter((station) => station.eventId === selectedEventId);

    const loadData = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const [loadedEvents, loadedRoles, loadedUsers, loadedStations, loadedItemTypes] = await Promise.all([
                request<EventInfo[]>("/administration/events"),
                request<Role[]>("/administration/roles"),
                request<UserInfo[]>("/administration/users"),
                request<ServingStation[]>("/administration/serving-stations"),
                request<MenuItemType[]>("/administration/menu-item-types"),
            ]);

            setEvents(loadedEvents);
            setRoles(loadedRoles);
            setUsers(loadedUsers);
            setStations(loadedStations);
            setItemTypes(loadedItemTypes);
            setSelectedEventId((current) => current ?? loadedEvents.find((event) => event.isActive)?.id ?? loadedEvents[0]?.id ?? null);
            setRoleId((current) => current || loadedRoles[0]?.id.toString() || "");
        } catch (loadError) {
            setError(loadError instanceof Error ? loadError.message : "Verwaltungsdaten konnten nicht geladen werden.");
        } finally {
            setLoading(false);
        }
    }, []);

    const loadTables = useCallback(async () => {
        if (selectedEventId === null) {
            setTables([]);
            return;
        }

        try {
            setTables(await request<NamedResource[]>(`/administration/events/${selectedEventId}/tables`));
        } catch (loadError) {
            setError(loadError instanceof Error ? loadError.message : "Tische konnten nicht geladen werden.");
        }
    }, [selectedEventId]);

    useEffect(() => {
        void loadData();
    }, [loadData]);

    useEffect(() => {
        void loadTables();
    }, [loadTables]);

    async function submit(action: () => Promise<void>) {
        try {
            setSubmitting(true);
            setError(null);
            await action();
            await loadData();
        } catch (submitError) {
            setError(submitError instanceof Error ? submitError.message : "Die Änderung konnte nicht gespeichert werden.");
        } finally {
            setSubmitting(false);
        }
    }

    function addEvent(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (!eventName.trim() || !eventStartsAt) {
            setError("Name und Beginn der Veranstaltung sind erforderlich.");
            return;
        }

        void submit(async () => {
            await request<EventInfo>("/administration/events", "POST", {
                name: eventName.trim(),
                startsAt: new Date(eventStartsAt).toISOString(),
            });
            setEventName("");
            setEventStartsAt("");
        });
    }

    function addUser(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (!username.trim() || pin.length < 4 || !roleId) {
            setError("Benutzername, mindestens vierstellige PIN und Rolle sind erforderlich.");
            return;
        }

        void submit(async () => {
            await request<UserInfo>("/administration/users", "POST", {
                username: username.trim(),
                pin,
                roleId: Number(roleId),
            });
            setUsername("");
            setPin("");
        });
    }

    function addTable(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (selectedEventId === null || !tableName.trim()) {
            setError("Wählen Sie eine Veranstaltung und geben Sie einen Tischnamen ein.");
            return;
        }

        void submit(async () => {
            await request<void>(`/administration/events/${selectedEventId}/tables`, "POST", { name: tableName.trim() });
            setTableName("");
            await loadTables();
        });
    }

    function addStation(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (selectedEventId === null || !stationName.trim()) {
            setError("Wählen Sie eine Veranstaltung und geben Sie einen Namen ein.");
            return;
        }

        void submit(async () => {
            await request<void>("/administration/serving-stations", "POST", {
                name: stationName.trim(),
                eventId: selectedEventId,
            });
            setStationName("");
        });
    }

    function addItemType(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (!itemTypeName.trim() || !stationId) {
            setError("Name und Ausgabestelle sind erforderlich.");
            return;
        }

        void submit(async () => {
            await request<void>("/administration/menu-item-types", "POST", {
                name: itemTypeName.trim(),
                servingStationId: Number(stationId),
            });
            setItemTypeName("");
        });
    }

    if (loading) {
        return <div className="border-t p-4">Lade Veranstaltungsverwaltung...</div>;
    }

    return (
        <section className="border-t p-4 space-y-8">
            <h1>Veranstaltungsverwaltung</h1>
            {error && <p className="text-red-600">{error}</p>}

            <div>
                <h2 className="text-lg font-semibold">Veranstaltungen</h2>
                <form onSubmit={addEvent} className="mt-2 flex flex-wrap items-end gap-3">
                    <label className="grid gap-1 text-sm">Name
                        <input className="border rounded px-3 py-2" value={eventName} onChange={(event) => setEventName(event.target.value)} />
                    </label>
                    <label className="grid gap-1 text-sm">Beginn
                        <input className="border rounded px-3 py-2" type="datetime-local" value={eventStartsAt} onChange={(event) => setEventStartsAt(event.target.value)} />
                    </label>
                    <button className="rounded bg-blue-600 px-4 py-2 text-white disabled:opacity-50" disabled={submitting} type="submit">Anlegen</button>
                </form>
                <ul className="mt-3 space-y-2">
                    {events.map((event) => (
                        <li className="flex flex-wrap items-center gap-2" key={event.id}>
                            <button className="text-left underline" onClick={() => setSelectedEventId(event.id)} type="button">
                                {event.name} ({new Date(event.startsAt).toLocaleString()})
                            </button>
                            {event.isActive && <span className="text-green-700">aktiv</span>}
                            {event.isCompleted && <span>abgeschlossen</span>}
                            {!event.isActive && !event.isCompleted && (
                                <button className="rounded bg-green-600 px-2 py-1 text-white disabled:opacity-50" disabled={submitting} onClick={() => void submit(() => request<void>(`/administration/events/${event.id}/activate`, "POST"))} type="button">
                                    Aktivieren
                                </button>
                            )}
                            {!event.isCompleted && (
                                <button className="rounded bg-gray-700 px-2 py-1 text-white disabled:opacity-50" disabled={submitting} onClick={() => void submit(() => request<void>(`/administration/events/${event.id}/complete`, "POST"))} type="button">
                                    Abschließen
                                </button>
                            )}
                        </li>
                    ))}
                </ul>
            </div>

            <div>
                <h2 className="text-lg font-semibold">Benutzer</h2>
                <form onSubmit={addUser} className="mt-2 flex flex-wrap items-end gap-3">
                    <label className="grid gap-1 text-sm">Benutzername
                        <input className="border rounded px-3 py-2" value={username} onChange={(event) => setUsername(event.target.value)} />
                    </label>
                    <label className="grid gap-1 text-sm">PIN
                        <input className="border rounded px-3 py-2" inputMode="numeric" minLength={4} type="password" value={pin} onChange={(event) => setPin(event.target.value)} />
                    </label>
                    <label className="grid gap-1 text-sm">Rolle
                        <select className="border rounded px-3 py-2" value={roleId} onChange={(event) => setRoleId(event.target.value)}>
                            {roles.map((role) => <option key={role.id} value={role.id}>{role.name}</option>)}
                        </select>
                    </label>
                    <button className="rounded bg-blue-600 px-4 py-2 text-white disabled:opacity-50" disabled={submitting} type="submit">Anlegen</button>
                </form>
                <ul className="mt-3 space-y-1">
                    {users.map((user) => (
                        <li key={user.id}>
                            {user.username} ({user.roleName}) {!user.isActive && "– deaktiviert"}
                            {user.isActive && <button className="ml-2 rounded bg-gray-700 px-2 py-1 text-white disabled:opacity-50" disabled={submitting} onClick={() => void submit(() => request<void>(`/administration/users/${user.id}/deactivate`, "POST"))} type="button">Deaktivieren</button>}
                        </li>
                    ))}
                </ul>
            </div>

            <div>
                <h2 className="text-lg font-semibold">Aktive Veranstaltung: {activeEvent?.name ?? "keine"}</h2>
                {selectedEvent && <p className="text-sm">Ausgewählt: {selectedEvent.name}</p>}
                <div className="mt-2 grid gap-6 md:grid-cols-3">
                    <form onSubmit={addTable} className="grid content-start gap-2">
                        <h3 className="font-semibold">Tische</h3>
                        <input className="border rounded px-3 py-2" placeholder="Tischnummer" value={tableName} onChange={(event) => setTableName(event.target.value)} />
                        <button className="rounded bg-blue-600 px-4 py-2 text-white disabled:opacity-50" disabled={submitting || !selectedEvent?.isActive} type="submit">Hinzufügen</button>
                        <ul>{tables.map((table) => <li key={table.id}>{table.name}</li>)}</ul>
                    </form>
                    <form onSubmit={addStation} className="grid content-start gap-2">
                        <h3 className="font-semibold">Ausgabestellen</h3>
                        <input className="border rounded px-3 py-2" placeholder="Ausgabestelle" value={stationName} onChange={(event) => setStationName(event.target.value)} />
                        <button className="rounded bg-blue-600 px-4 py-2 text-white disabled:opacity-50" disabled={submitting || !selectedEvent?.isActive} type="submit">Hinzufügen</button>
                        <ul>{eventStations.map((station) => <li key={station.id}>{station.name}</li>)}</ul>
                    </form>
                    <form onSubmit={addItemType} className="grid content-start gap-2">
                        <h3 className="font-semibold">Artikeltypen</h3>
                        <input className="border rounded px-3 py-2" placeholder="Artikeltyp" value={itemTypeName} onChange={(event) => setItemTypeName(event.target.value)} />
                        <select className="border rounded px-3 py-2" value={stationId} onChange={(event) => setStationId(event.target.value)}>
                            <option value="">Ausgabestelle wählen</option>
                            {eventStations.map((station) => <option key={station.id} value={station.id}>{station.name}</option>)}
                        </select>
                        <button className="rounded bg-blue-600 px-4 py-2 text-white disabled:opacity-50" disabled={submitting || !selectedEvent?.isActive} type="submit">Hinzufügen</button>
                        <ul>{itemTypes.filter((type) => eventStations.some((station) => station.id === type.servingStationId)).map((type) => <li key={type.id}>{type.name}</li>)}</ul>
                    </form>
                </div>
            </div>
        </section>
    );
}
