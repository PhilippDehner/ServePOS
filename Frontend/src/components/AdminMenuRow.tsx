import { useEffect, useState } from "react";

import type { MenuItem } from "../api";
import { RiDeleteBin6Line } from "react-icons/ri";
import { MdOutlineModeEditOutline, MdSaveAlt } from "react-icons/md";
import { TbPencilCancel } from "react-icons/tb";

export type RowSavePayload = {
    name: string | null;
    shortName: string | null;
    price: number;
    type: MenuItem["type"];
    availableQuantity: number | null;
};

type Draft = {
    name: string;
    shortName: string;
    price: string;
    type: MenuItem["type"];
    availableQuantity: string;
};

type AdminMenuRowProps = {
    item: MenuItem;
    index: number;
    total: number;
    onMove: (id: number, down: boolean) => Promise<void>;
    onDelete: (id: number) => Promise<void>;
    onSave: (id: number, payload: RowSavePayload) => Promise<boolean>;
    onEditModeChange?: (id: number, isEditing: boolean) => void;
};

function toDraft(item: MenuItem): Draft {
    return {
        name: item.name ?? "",
        shortName: item.shortName ?? "",
        price: item.price.toString(),
        type: item.type,
        availableQuantity: item.availableQuantity?.toString() ?? "",
    };
}

export default function AdminMenuRow({
    item,
    index,
    total,
    onMove,
    onDelete,
    onSave,
    onEditModeChange,
}: AdminMenuRowProps) {
    const [isEditing, setIsEditing] = useState(false);
    const [isSaving, setIsSaving] = useState(false);
    const [draft, setDraft] = useState<Draft>(toDraft(item));
    const original = toDraft(item);
    const hasChanges =
        draft.name !== original.name ||
        draft.shortName !== original.shortName ||
        draft.price !== original.price ||
        draft.type !== original.type ||
        draft.availableQuantity !== original.availableQuantity;

    const priceNumber = Number.parseFloat(draft.price);
    const availableQuantityNumber =
        draft.availableQuantity.trim() === ""
            ? null
            : Number.parseInt(draft.availableQuantity, 10);

    const isValid =
        Number.isFinite(priceNumber) &&
        (availableQuantityNumber === null || Number.isFinite(availableQuantityNumber));

    useEffect(() => {
        onEditModeChange?.(item.id, isEditing);
    }, [item.id, isEditing, onEditModeChange]);

    useEffect(() => {
        return () => {
            onEditModeChange?.(item.id, false);
        };
    }, [item.id, onEditModeChange]);

    async function handleSave() {
        if (!hasChanges || !isValid || isSaving) {
            return;
        }

        setIsSaving(true);
        const ok = await onSave(item.id, {
            name: draft.name.trim() === "" ? null : draft.name,
            shortName: draft.shortName.trim() === "" ? null : draft.shortName,
            price: priceNumber,
            type: draft.type,
            availableQuantity: availableQuantityNumber,
        });

        if (!ok) {
            setIsSaving(false);
        }
    }

    function cancelEdit() {
        setDraft(original);
        setIsEditing(false);
    }

    return (
        <tr>
            <td className="px-4 py-2">
                {isEditing ? (
                    <div>
                        <input
                            className="border rounded px-2 py-1 w-full bg-transparent"
                            value={draft.name}
                            onChange={e => setDraft(prev => ({ ...prev, name: e.target.value }))}
                            placeholder="Name"
                        />
                        <input
                            className="border rounded px-2 py-1 w-full bg-transparent"
                            value={draft.shortName}
                            onChange={e => setDraft(prev => ({ ...prev, shortName: e.target.value }))}
    						placeholder="Abkürzung (optional)"
                        />
                    </div>
                ) : (
                        <span>{item.name}{item.shortName ? ` (${item.shortName})` : ""}</span>
                )}
            </td>
            <td className="px-4 py-2">
                {isEditing ? (
                    <input
                        className="border rounded px-2 py-1 w-24 bg-transparent"
                        type="number"
                        step="0.01"
                        value={draft.price}
                        onChange={e => setDraft(prev => ({ ...prev, price: e.target.value }))}
                    />
                ) : (
                    <span>{item.price.toFixed(2)} €</span>
                )}
            </td>
            <td className="px-4 py-2">
                {isEditing ? (
                    <select
                        className="border rounded px-2 py-1 bg-transparent"
                        value={draft.type}
                        onChange={e => setDraft(prev => ({ ...prev, type: e.target.value as MenuItem["type"] }))}
                    >
                        <option value="Food">Food</option>
                        <option value="Drink">Drink</option>
                        <option value="Dessert">Dessert</option>
                        <option value="Special">Special</option>
                    </select>
                ) : (
                    <span>{item.type}</span>
                )}
            </td>
            <td className="px-4 py-2">
                {isEditing ? (
                    <input
                        className="border rounded px-2 py-1 w-24 bg-transparent"
                        type="number"
                        value={draft.availableQuantity}
                        onChange={e => setDraft(prev => ({ ...prev, availableQuantity: e.target.value }))}
                    />
                ) : (
                    <span>{item.availableQuantity ?? "-"}</span>
                )}
            </td>
            <td className="px-4 py-2 flex items-center gap-2">
                <button
                    className="leading-none disabled:opacity-30"
                    onClick={() => onMove(item.id, true)}
                    disabled={index === 0 || isEditing}
                    title="Nach oben"
                >▲</button>
                <button
                    className="leading-none disabled:opacity-30"
                    onClick={() => onMove(item.id, false)}
                    disabled={index === total - 1 || isEditing}
                    title="Nach unten"
                >▼</button>

                {isEditing ? (
                    <>
                        <button
                            className="px-2 py-1 bg-green-600 text-white rounded disabled:opacity-50"
                            onClick={handleSave}
                            disabled={!hasChanges || !isValid || isSaving}
                        >
                            <MdSaveAlt />
                        </button>
                        <button
                            className="px-2 py-1 bg-gray-500 text-white rounded"
                            onClick={cancelEdit}
                            disabled={isSaving}
                            title="Cancel"
                        >
                            <TbPencilCancel />
                        </button>
                    </>
                ) : (
                    <>
                        <button
                            className="px-2 py-1 bg-blue-500 text-white rounded"
                            onClick={() => setIsEditing(true)}>
                            <MdOutlineModeEditOutline />
                        </button>
                        <button
                            className="px-2 py-1 bg-red-500 text-white rounded"
                            onClick={() => onDelete(item.id)}
                            disabled={isSaving}>
                            <RiDeleteBin6Line />
                        </button>
                    </>
                )}
            </td>
        </tr>
    );
}
