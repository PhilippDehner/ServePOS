import { useState, useEffect } from "react";

import { MenuApi, Configuration } from "../api";
import { StaffApi } from "../api";
import type { MenuItem, MenuItemTypeEnum, StaffInfo } from "../api";
import HomeButton from "../components/HomeButton";
import MenuItemsGrid from "../components/MenuItemsGrid";
import OrderItem from "../components/OrderItem";
import RefreshButton from "../components/RefreshButton";
import { RiDeleteBin6Line } from "react-icons/ri";
import ButtonGroup from "../components/ButtonGroup";
import { BiSolidDrink } from "react-icons/bi";
import { BsForkKnife } from "react-icons/bs";
import { LiaCommentDotsSolid } from "react-icons/lia";
import { CgCoffee } from "react-icons/cg";
import {
    createClientOrderId,
    getPendingOrderCount,
    submitOrder,
    syncPendingOrders,
} from "../offlineOrders";

const config = new Configuration({
    basePath: import.meta.env.VITE_API_BASE_URL || "/api"
});
const api = new MenuApi(config);
const staffApi = new StaffApi(config);

export default function Order() {
    const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
    const [staff, setStaff] = useState<StaffInfo[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [selectedType, setSelectedType] = useState<MenuItemTypeEnum | null>(null);
    const [orderLines, setOrderLines] = useState<Array<{ lineId: number; itemId: number; comment: string; }>>([]);
    const [nextLineId, setNextLineId] = useState(1);
    const [tableId, setTableId] = useState(() => localStorage.getItem("servepos.table-id") ?? "");
    const [staffId, setStaffId] = useState(() => localStorage.getItem("servepos.staff-id") ?? "");
    const [pendingOrderCount, setPendingOrderCount] = useState(0);
    const [submitting, setSubmitting] = useState(false);
    const [orderStatus, setOrderStatus] = useState<string | null>(null);

    useEffect(() => {
        loadMenuItems();
        void loadStaff();
    }, []);

    useEffect(() => {
        let isMounted = true;

        async function synchronizePendingOrders() {
            const count = await syncPendingOrders();
            if (isMounted) {
                setPendingOrderCount(count);
            }
        }

        void synchronizePendingOrders();
        window.addEventListener("online", synchronizePendingOrders);
        return () => {
            isMounted = false;
            window.removeEventListener("online", synchronizePendingOrders);
        };
    }, []);

    useEffect(() => {
        localStorage.setItem("servepos.table-id", tableId);
    }, [tableId]);

    useEffect(() => {
        localStorage.setItem("servepos.staff-id", staffId);
    }, [staffId]);

    async function loadMenuItems() {
        try {
            setLoading(true);
            setError(null);
            const items = await api.menuGet();
            setMenuItems(items);
            // Set the first type as selected
            if (items.length > 0) {
                setSelectedType("Drink");
            }
        } catch (err) {
            setError("Backend nicht erreichbar. Bitte stellen Sie sicher, dass der Server läuft.");
            console.error("Failed to load menu items:", err);
        } finally {
            setLoading(false);
        }
    }

    async function loadStaff() {
        try {
            setStaff(await staffApi.staffGet());
        } catch (err) {
            console.error("Failed to load staff:", err);
        }
    }

    // Get unique types from menu items
    // const types = Array.from(new Set(menuItems.map(item => item.type)));
    const types = ["Drink", "Food", "Dessert", "Special"] as MenuItemTypeEnum[];

    // Filter items by selected type
    const filteredItems = selectedType !== null
        ? menuItems.filter(item => item.type === selectedType)
        : [];

    const orderItems = orderLines.reduce<Record<number, number>>((acc, line) => {
        acc[line.itemId] = (acc[line.itemId] || 0) + 1;
        return acc;
    }, {});

    // Decrease shown availability by selected quantity, but only for non-null stock values.
    const filteredItemsWithRemainingAvailability = filteredItems.map(item => {
        if (item.availableQuantity == null) {
            return item;
        }

        const selectedCount = orderItems[item.id] || 0;
        return {
            ...item,
            availableQuantity: Math.max(0, item.availableQuantity - selectedCount),
        };
    });

    // Add item to order
    function addItem(itemId: number) {
        const lineId = nextLineId;
        setNextLineId(prev => prev + 1);
        setOrderLines(prev => ([
            ...prev,
            { lineId, itemId, comment: "" }
        ]));
    }

    function removeOrderLine(lineId: number) {
        setOrderLines(prev => prev.filter(line => line.lineId !== lineId));
    }

    function removeGroupedOrderItem(itemId: number) {
        const line = orderLines.find(x => x.itemId === itemId && !x.comment.trim());
        if (!line) {
            return;
        }
        removeOrderLine(line.lineId);
    }

    function updateOrderLineComment(lineId: number) {
        const line = orderLines.find(x => x.lineId === lineId);
        if (!line) {
            return;
        }

        const current = line.comment ?? "";
        const updated = window.prompt("Kommentar / Sonderwunsch", current);
        if (updated === null) {
            return;
        }

        setOrderLines(prev => prev.map(x =>
            x.lineId === lineId ? { ...x, comment: updated.trim() } : x
        ));
    }

    function updateGroupedOrderItemComment(itemId: number) {
        const line = orderLines.find(x => x.itemId === itemId && !x.comment.trim());
        if (!line) {
            return;
        }
        updateOrderLineComment(line.lineId);
    }

    // Calculate total price
    const totalPrice = orderLines.reduce((sum, line) => {
        const item = menuItems.find(m => m.id === line.itemId);
        return sum + (item?.price ?? 0);
    }, 0);

    // Type for grouped items without comment
    type GroupedNoCommentItem = {
        itemId: number;
        item: MenuItem;
        quantity: number;
    };

    type GroupedCommentItem = {
        itemId: number;
        item: MenuItem;
        comment: string;
    };

    // Group items without comment by itemId, keep commented lines separate
    const groupedNoCommentMap = orderLines.reduce<Record<number, { itemId: number; count: number; }>>((acc, line) => {
        if (line.comment.trim()) {
            return acc;
        }

        if (!acc[line.itemId]) {
            acc[line.itemId] = { itemId: line.itemId, count: 0 };
        }
        acc[line.itemId].count += 1;
        return acc;
    }, {});

    const groupedNoCommentDetails: GroupedNoCommentItem[] = Object.values(groupedNoCommentMap)
        .map(group => {
            const item = menuItems.find(m => m.id === group.itemId);
            return { itemId: group.itemId, item, quantity: group.count };
        })
        .filter(x => x.item !== undefined) as GroupedNoCommentItem[];

    const commentedOrderDetails: GroupedCommentItem[] = orderLines
        .filter(line => !!line.comment.trim())
        .map((line) => {
            const item = menuItems.find(m => m.id === line.itemId);
            return { itemId: line.lineId, item, comment: line.comment };
        })
        .filter(x => x.item !== undefined) as GroupedCommentItem[];

    const hasOrderItems = groupedNoCommentDetails.length > 0 || commentedOrderDetails.length > 0;

    useEffect(() => {
        function handleBeforeUnload(event: BeforeUnloadEvent) {
            if (!hasOrderItems) {
                return;
            }

            event.preventDefault();
            event.returnValue = "";
        }

        window.addEventListener("beforeunload", handleBeforeUnload);
        return () => {
            window.removeEventListener("beforeunload", handleBeforeUnload);
        };
    }, [hasOrderItems]);

    function confirmLeaveIfOrderExists() {
        if (!hasOrderItems) {
            return true;
        }

        return window.confirm("Die Bestellung ist nicht leer. Wirklich zur Startseite wechseln?");
    }

    function resetCurrentOrder() {
        const confirm = window.confirm("Möchtest Du die aktuelle Bestellung wirklich zurücksetzen?");
        if (confirm) {
            setOrderLines([]);
            setNextLineId(1);
        }
    }

    async function sendCurrentOrder() {
        const normalizedTableId = tableId.trim();
        const parsedStaffId = Number(staffId);

        if (!normalizedTableId) {
            setOrderStatus("Bitte einen Tisch angeben.");
            return;
        }

        if (!Number.isInteger(parsedStaffId) || parsedStaffId <= 0) {
            setOrderStatus("Bitte einen Bediener auswählen.");
            return;
        }

        if (!hasOrderItems) {
            setOrderStatus("Bitte mindestens einen Artikel hinzufügen.");
            return;
        }

        setSubmitting(true);
        setOrderStatus(null);
        try {
            const result = await submitOrder({
                clientOrderId: createClientOrderId(),
                staffId: parsedStaffId,
                tableId: normalizedTableId,
                items: orderLines.map(line => ({
                    menuItemId: line.itemId,
                    specialInstructions: line.comment.trim() || null,
                })),
                queuedAt: new Date().toISOString(),
            });

            setOrderLines([]);
            setNextLineId(1);
            setPendingOrderCount(await getPendingOrderCount());
            setOrderStatus(result === "submitted"
                ? "Bestellung wurde gesendet."
                : "Bestellung wurde offline gespeichert und wird automatisch übertragen.");
        } catch (err) {
            console.error("Failed to submit order:", err);
            setOrderStatus("Bestellung konnte nicht gespeichert werden.");
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <div className="min-h-screen flex flex-col">
            <div id="header" className="flex gap-4 mb-4 overflow-x-auto pb-2 [&>*]:shrink-0">
                <ButtonGroup>
                    <HomeButton onBeforeNavigate={confirmLeaveIfOrderExists} />
                    <RefreshButton onRefresh={() => void loadMenuItems()} />

                    <button
                        onClick={() => void resetCurrentOrder()}
                        className="px-4 py-2 bg-gray-200 rounded"
                        title="Bestellung zurücksetzen"
                        disabled={!hasOrderItems}>
                        <RiDeleteBin6Line />
                    </button>

                </ButtonGroup>

                <ButtonGroup>
                    {types.map((type) => (
                        <button
                            key={type}
                            onClick={() => setSelectedType(type)}
                            disabled={type === selectedType}
                            className={`h-14 px-4 py-2 rounded transition-colors`}
                        >
                            {(() => {
                                switch(type){
                                    case "Food": return <BsForkKnife />;
                                    case "Drink": return <BiSolidDrink />;
                                    case "Dessert": return <CgCoffee />
                                    case "Special": return <LiaCommentDotsSolid />;
                                    default: return type;
                                }
                            })()}
                        </button>
                    ))}
                </ButtonGroup>
            </div>

            {loading && <div className="p-4">Lade Menü...</div>}
            {error && <div className="p-4 text-red-500">{error}</div>}

            {!loading && !error && (
                <>
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                        <div className="lg:col-span-2">
                            <MenuItemsGrid
                                items={filteredItemsWithRemainingAvailability}
                                orderItems={orderItems}
                                onAddItem={addItem}
                            />
                        </div>

                        <div className="border-t-2 lg:col-span-1">
                            <div className="rounded sticky top-6 h-fit">
                                <h2>Vorschau</h2>
                                <div className="my-4 space-y-3">
                                    <label className="block">
                                        <span className="mb-1 block font-medium">Tisch</span>
                                        <input
                                            value={tableId}
                                            onChange={event => setTableId(event.target.value)}
                                            className="w-full rounded border p-2"
                                            placeholder="z. B. 12"
                                        />
                                    </label>
                                    <label className="block">
                                        <span className="mb-1 block font-medium">Bediener</span>
                                        <select
                                            value={staffId}
                                            onChange={event => setStaffId(event.target.value)}
                                            className="w-full rounded border p-2"
                                        >
                                            <option value="">Bediener auswählen</option>
                                            {staff.map(member => (
                                                <option key={member.id} value={member.id}>
                                                    {member.name ?? `Bediener ${member.id}`}
                                                </option>
                                            ))}
                                        </select>
                                    </label>
                                </div>
                                {hasOrderItems === false ? (
                                    <p className="text-gray-500">Keine Artikel hinzugefügt</p>
                                ) : (
                                    <div>
                                        <div className="space-y-2 mb-4 max-h-96 overflow-y-auto">
                                            {groupedNoCommentDetails.map(({ itemId, item, quantity }) => (
                                                <OrderItem key={`group-${itemId}`} item={{ Id: itemId, item, quantity, comment: "" }} onRemove={removeGroupedOrderItem} onAddComment={updateGroupedOrderItemComment} />
                                            ))}

                                            {commentedOrderDetails.map(({ itemId, item, comment }) => (
                                                <OrderItem key={`group-${itemId}`} item={{ Id: itemId, item, quantity: 1, comment }} onRemove={removeOrderLine} onAddComment={updateOrderLineComment} />
                                            ))}
                                        </div>
                                        <div className="pt-4">
                                            <div className="flex justify-between text-xl font-bold">
                                                <span>Summe:</span>
                                                <span>{totalPrice.toFixed(2)} €</span>
                                            </div>
                                        </div>
                                    </div>
                                )}
                                <button
                                    type="button"
                                    onClick={() => void sendCurrentOrder()}
                                    disabled={!hasOrderItems || submitting}
                                    className="mt-4 w-full bg-blue-700 text-white"
                                >
                                    {submitting ? "Wird gesendet..." : "Bestellung senden"}
                                </button>
                                {orderStatus && <p className="mt-2" role="status">{orderStatus}</p>}
                                {pendingOrderCount > 0 && (
                                    <p className="mt-2 text-amber-700" role="status">
                                        {pendingOrderCount} Bestellung{pendingOrderCount === 1 ? "" : "en"} warten auf Übertragung.
                                    </p>
                                )}
                            </div>
                        </div>
                    </div>
                </>
            )}
        </div>

    );
}
