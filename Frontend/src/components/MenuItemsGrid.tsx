import type { MenuItem } from "../api";
import { useRef, useState } from "react";

type MenuItemsGridProps = {
    items: MenuItem[];
    orderItems: Record<number, number>;
    onAddItem: (itemId: number) => void;
};

type MenuItemCardProps = {
    item: MenuItem;
    quantity: number;
    onAdd: () => void;
};

function MenuItemCard({ item, quantity, onAdd }: MenuItemCardProps) {
    const longPressTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
    const [showToast, setShowToast] = useState(false);

    const handleTouchStart = () => {
        longPressTimeoutRef.current = setTimeout(() => {
            setShowToast(true);
            setTimeout(() => setShowToast(false), 2000);
        }, 500);
    };

    const handleTouchEnd = () => {
        if (longPressTimeoutRef.current) {
            clearTimeout(longPressTimeoutRef.current);
            longPressTimeoutRef.current = null;
        }
    };

    return (
        <>
            <button
                className={`p-4 border rounded shadow text-left ${item.availableQuantity === 0 ? "opacity-50" : ""}`}
                onClick={onAdd}
                disabled={item.availableQuantity === 0}
                onTouchStart={handleTouchStart}
                onTouchEnd={handleTouchEnd}
            >
                <h3>
                    <span title={item.name ?? ""}>{item.shortName ?? item.name}</span>    
                    <span className="font-normal"> | {item.price.toFixed(2)} €</span>
                </h3>

                <p className="mt-2">
                    {item.availableQuantity != null && item.availableQuantity < 20 ? (
                        <span className="text-red-600">Verfügbar: {item.availableQuantity} | </span>
                    ) : null}
                    {quantity === 0
                        ? <></>
                        : <span className={quantity === 0 ? "text-gray-400" : ""}>Ausgewählt: {quantity}x</span>
                    }
                </p>
            </button>

            {showToast && (
                <div className="fixed bottom-8 left-1/2 transform -translate-x-1/2 bg-gray-800 text-white px-6 py-3 rounded-lg shadow-lg z-50 pointer-events-none">
                    {item.name}
                </div>
            )}
        </>
    );
}

export default function MenuItemsGrid({ items, orderItems, onAddItem }: MenuItemsGridProps) {
    return (
        <div className="grid grid-cols-2 md:grid-cols-3 gap-2">
            {items.map((item) => (
                <MenuItemCard
                    key={item.id}
                    item={item}
                    quantity={orderItems[item.id] || 0}
                    onAdd={() => onAddItem(item.id)}
                />
            ))}
        </div>
    );
}
