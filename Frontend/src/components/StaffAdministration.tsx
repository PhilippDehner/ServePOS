import { useEffect, useState } from "react";
import type { FormEvent } from "react";

import { Configuration, StaffApi } from "../api";
import type { StaffInfo } from "../api";
import { MdOutlineAddCircleOutline } from "react-icons/md";
import { MdOutlineModeEditOutline, MdSaveAlt } from "react-icons/md";
import { TbPencilCancel } from "react-icons/tb";

const config = new Configuration({
    basePath: import.meta.env.VITE_API_BASE_URL || "/api",
});
const api = new StaffApi(config);

type StaffAdministrationProps = {
    onRegisterRefresh?: (refreshFn: () => Promise<void>) => void;
};

export default function StaffAdministration({ onRegisterRefresh }: StaffAdministrationProps) {
    const [staffMembers, setStaffMembers] = useState<StaffInfo[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [newStaffName, setNewStaffName] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [editingStaffId, setEditingStaffId] = useState<number | null>(null);
    const [editingStaffName, setEditingStaffName] = useState("");
    const [isUpdating, setIsUpdating] = useState(false);

    useEffect(() => {
        void loadStaffMembers();
    }, []);

    async function loadStaffMembers() {
        try {
            setLoading(true);
            setError(null);
            const staff = await api.staffGet();
            setStaffMembers(staff);
        } catch (err) {
            setError("Fehler beim Laden des Mitarbeiters.");
            console.error("Failed to load staff:", err);
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        if (!onRegisterRefresh) {
            return;
        }

        onRegisterRefresh(async () => {
            await loadStaffMembers();
        });
    }, [onRegisterRefresh]);

    async function addStaffMember(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (!newStaffName.trim()) {
            setError("Bitte einen Namen eingeben.");
            return;
        }

        try {
            setIsSubmitting(true);
            setError(null);

            await api.staffPost({
                staffUpsert: {
                    name: newStaffName.trim()
                },
            });

            setNewStaffName("");
            await loadStaffMembers();
        } catch (err) {
            setError("Fehler beim Anlegen der Mitarbeiter.");
            console.error("Failed to create staff:", err);
        } finally {
            setIsSubmitting(false);
        }
    }

    function startEdit(staff: StaffInfo) {
        setEditingStaffId(staff.id);
        setEditingStaffName(staff.name ?? "");
    }

    function cancelEdit() {
        setEditingStaffId(null);
        setEditingStaffName("");
    }

    async function saveEdit(staffId: number) {
        if (!editingStaffName.trim()) {
            setError("Bitte einen Namen eingeben.");
            return;
        }

        try {
            setIsUpdating(true);
            setError(null);
            await api.staffIdPut({
                id: staffId,
                staffUpsert: {
                    name: editingStaffName.trim(),
                },
            });

            cancelEdit();
            await loadStaffMembers();
        } catch (err) {
            setError("Fehler beim Speichern des Mitarbeiters.");
            console.error("Failed to update staff:", err);
        } finally {
            setIsUpdating(false);
        }
    }

    return (
        <div className="border-t p-4">
            <h1>Personenverwaltung</h1>

            <form onSubmit={addStaffMember} className="flex flex-wrap items-end gap-3 mb-4">
                <div>
                    <label className="block text-sm mb-1">Name</label>
                    <input
                        type="text"
                        value={newStaffName}
                        onChange={(e) => setNewStaffName(e.target.value)}
                        className="border rounded px-3 py-2"
                        placeholder="Neuer Mitarbeiter"
                        disabled={isSubmitting}
                    />
                </div>
                <button
                    type="submit"
                    className="px-4 py-2 rounded bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-50"
                    disabled={isSubmitting}
                >
                    <MdOutlineAddCircleOutline />

                </button>
            </form>

            {loading && <div>Lade Mitarbeiter...</div>}
            {error && <div className="text-red-500 mb-3">{error}</div>}

            {!loading && !error && (
                <table className="w-full border-collapse">
                    <thead>
                        <tr>
                            <th className="px-4 py-2 text-left">ID</th>
                            <th className="px-4 py-2 text-left">Name</th>
                            <th className="px-4 py-2 text-left">Letzte Aktivität</th>
                            <th className="px-4 py-2 text-left">Aktionen</th>
                        </tr>
                    </thead>
                    <tbody>
                        {staffMembers.map((staff) => (
                            <tr key={staff.id}>
                                <td className="px-4 py-2">{staff.id}</td>
                                <td className="px-4 py-2">
                                    {editingStaffId === staff.id ? (
                                        <input
                                            type="text"
                                            className="border rounded px-2 py-1"
                                            value={editingStaffName}
                                            onChange={(e) => setEditingStaffName(e.target.value)}
                                            disabled={isUpdating}
                                        />
                                    ) : (
                                        staff.name ?? "-"
                                    )}
                                </td>
                                <td className="px-4 py-2">
                                    {Number.isNaN(staff.lastOperation.getTime())
                                        ? "-"
                                        : staff.lastOperation.toLocaleString()}
                                </td>
                                <td className="px-4 py-2">
                                    {editingStaffId === staff.id ? (
                                        <div className="flex items-center gap-2">
                                            <button
                                                className="px-2 py-1 bg-green-600 text-white rounded disabled:opacity-50"
                                                onClick={() => void saveEdit(staff.id)}
                                                disabled={isUpdating}
                                            >
                                                <MdSaveAlt />
                                            </button>
                                            <button
                                                className="px-2 py-1 bg-gray-500 text-white rounded disabled:opacity-50"
                                                onClick={cancelEdit}
                                                disabled={isUpdating}
                                            >
                                                <TbPencilCancel />
                                            </button>
                                        </div>
                                    ) : (
                                        <button
                                            className="px-2 py-1 bg-blue-500 text-white rounded"
                                            onClick={() => startEdit(staff)}
                                        >
                                            <MdOutlineModeEditOutline />
                                        </button>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            {!loading && !error && staffMembers.length === 0 && (
                <p className="text-sm text-gray-600 mt-2">Noch keine Mitarbeiter vorhanden.</p>
            )}
        </div>
    );
}
