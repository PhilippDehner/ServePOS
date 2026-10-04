import { useCallback, useEffect, useState } from "react";
import type { FormEvent } from "react";

import { Configuration, MenuApi } from "../api";
import { MenuItemInsertInformationTypeEnum } from "../api";
import type { MenuItem } from "../api";
import AdminMenuRow, { type RowSavePayload } from "./AdminMenuRow";
import { MdOutlineAddCircleOutline } from "react-icons/md";

const config = new Configuration({
	basePath: import.meta.env.VITE_API_BASE_URL || "/api",
});
const api = new MenuApi(config);

type MenuAdministrationProps = {
	onRegisterRefresh?: (refreshFn: () => Promise<void>) => void;
	onAnyRowEditingChange?: (isEditing: boolean) => void;
};

export default function MenuAdministration({
	onRegisterRefresh,
	onAnyRowEditingChange,
}: MenuAdministrationProps) {
	const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);
	const [editingRowIds, setEditingRowIds] = useState<number[]>([]);
	const [newName, setNewName] = useState("");
	const [newShortName, setNewShortName] = useState<string | undefined>(undefined);
	const [newPrice, setNewPrice] = useState("0");
	const [newType, setNewType] = useState<MenuItemInsertInformationTypeEnum>(MenuItemInsertInformationTypeEnum.Food);
	const [newAvailableQuantity, setNewAvailableQuantity] = useState("");

	const isAnyRowEditing = editingRowIds.length > 0;

	const loadMenuItems = useCallback(async () => {
		try {
			setLoading(true);
			setError(null);
			const items = await api.menuGet();
			setMenuItems(items);
		} catch (err) {
			setError("Backend nicht erreichbar. Bitte stellen Sie sicher, dass der Server läuft.");
			console.error("Failed to load menu items:", err);
		} finally {
			setLoading(false);
		}
	}, []);

	useEffect(() => {
		void loadMenuItems();
	}, [loadMenuItems]);

	useEffect(() => {
		if (!onRegisterRefresh) {
			return;
		}

		onRegisterRefresh(async () => {
			await loadMenuItems();
		});
	}, [onRegisterRefresh, loadMenuItems]);

	useEffect(() => {
		onAnyRowEditingChange?.(isAnyRowEditing);
	}, [isAnyRowEditing, onAnyRowEditingChange]);

	const handleRowEditModeChange = useCallback((id: number, isEditing: boolean) => {
		setEditingRowIds(prev => {
			if (isEditing) {
				if (prev.includes(id)) {
					return prev;
				}
				return [...prev, id];
			}

			return prev.filter(rowId => rowId !== id);
		});
	}, []);

	async function saveItem(id: number, payload: RowSavePayload): Promise<boolean> {
		try {
			await api.menuItemIdPut({
				id,
				menuItemUpdateInformation: {
					name: payload.name,
					shortName: payload.shortName,
					price: payload.price,
					type: payload.type,
					availableQuantity: payload.availableQuantity,
				},
			});
			await loadMenuItems();
			return true;
		} catch (err) {
			setError("Fehler beim Speichern des Menuepunkts.");
			console.error("Failed to save menu item:", err);
			return false;
		}
	}

	async function moveItem(id: number, down: boolean) {
		try {
			await api.menuItemIdPushSortOrderPut({ id, down });
			await loadMenuItems();
		} catch (err) {
			setError("Fehler beim Verschieben des Menuepunkts.");
			console.error("Failed to move menu item:", err);
		}
	}

	async function deleteMenuItem(id: number) {
		try {
			await api.menuItemIdDelete({ id });
			await loadMenuItems();
		} catch (err) {
			setError("Fehler beim Loeschen des Menuepunkts.");
			console.error("Failed to delete menu item:", err);
		}
	}

	async function addMenuItem(event: FormEvent<HTMLFormElement>) {
		event.preventDefault();

		const parsedPrice = Number(newPrice);
		if (!newName.trim()) {
			setError("Bitte einen Namen fuer den Menuepunkt eingeben.");
			return;
		}

		if (!Number.isFinite(parsedPrice) || parsedPrice < 0) {
			setError("Bitte einen gueltigen Preis eingeben.");
			return;
		}

		const hasQuantity = newAvailableQuantity.trim() !== "";
		const parsedQuantity = hasQuantity ? Number(newAvailableQuantity) : undefined;
		if (hasQuantity && (!Number.isInteger(parsedQuantity) || (parsedQuantity ?? 0) < 0)) {
			setError("Verfuegbare Menge muss eine ganze Zahl >= 0 sein oder leer bleiben.");
			return;
		}

		try {
			setError(null);
			await api.menuItemPost({
				menuItemInsertInformation: {
					name: newName.trim(),
					shortName: newShortName === undefined ? null : newShortName.trim() === "" ? null : newShortName.trim(),
					price: parsedPrice,
					type: newType,
					availableQuantity: hasQuantity ? parsedQuantity : null,
				},
			});

			setNewName("");
			setNewShortName("");
			setNewPrice("0");
			setNewType(MenuItemInsertInformationTypeEnum.Food);
			setNewAvailableQuantity("");
			await loadMenuItems();
		} catch (err) {
			setError("Fehler beim Anlegen des Menuepunkts.");
			console.error("Failed to create menu item:", err);
		}
	}

	return (
		<div>
			<h1> Menüverwaltung </h1>
			<form onSubmit={addMenuItem} className="p-4 border-b flex flex-wrap gap-3 items-end">
				<div>
					<label className="block text-sm mb-1">Name</label>
					<input
						type="text"
						value={newName}
						onChange={(e) => setNewName(e.target.value)}
						className="border rounded px-3 py-2"
						placeholder="Neuer Menüpunkt"
					/>
					<input
						type="text"
						value={newShortName}
						onChange={(e) => setNewShortName(e.target.value)}
						className="border rounded px-3 py-2"
						placeholder="Abkürzung (optional)"
					/>
				</div>
				<div>
					<label className="block text-sm mb-1">Preis</label>
					<input
						type="number"
						min="0"
						step="0.01"
						value={newPrice}
						onChange={(e) => setNewPrice(e.target.value)}
						className="border rounded px-3 py-2 w-28"
					/>
				</div>
				<div>
					<label className="block text-sm mb-1">Typ</label>
					<select
						value={newType}
						onChange={(e) => setNewType(e.target.value as MenuItemInsertInformationTypeEnum)}
						className="border rounded px-3 py-2"
					>
						{Object.values(MenuItemInsertInformationTypeEnum).map((type) => (
							<option key={type} value={type}>
								{type}
							</option>
						))}
					</select>
				</div>
				<div>
					<label className="block text-sm mb-1">Verfügbar (optional)</label>
					<input
						type="number"
						min="0"
						step="1"
						value={newAvailableQuantity}
						onChange={(e) => setNewAvailableQuantity(e.target.value)}
						className="border rounded px-3 py-2 w-50"
						placeholder="leer = unbegrenzt"
					/>
				</div>
				<button type="submit" className="px-4 py-2 rounded bg-blue-600 text-white hover:bg-blue-700">
					<MdOutlineAddCircleOutline />
				</button>
			</form>

			{loading && <div className="p-4">Lade Menü...</div>}
			{error && <div className="p-4 text-red-500">{error}</div>}

			{!loading && !error && (
				<table className="w-full border-collapse">
					<thead>
						<tr>
							<th className="px-4 py-2 text-left">Name (Abkürzung)</th>
							<th className="px-4 py-2 text-left">Preis</th>
							<th className="px-4 py-2 text-left">Typ</th>
							<th className="px-4 py-2 text-left">Aktuell verfügbar</th>
							<th className="px-4 py-2 text-left">Aktionen</th>
						</tr>
					</thead>
					<tbody>
						{menuItems.map((item, index) => (
							<AdminMenuRow
								key={item.id}
								item={item}
								index={index}
								total={menuItems.length}
								onMove={moveItem}
								onDelete={deleteMenuItem}
								onSave={saveItem}
								onEditModeChange={handleRowEditModeChange}
							/>
						))}
					</tbody>
				</table>
			)}
		</div>
	);
}
