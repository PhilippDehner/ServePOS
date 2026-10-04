export type PendingOrder = {
    clientOrderId: string;
    staffId: number;
    tableId: string;
    items: Array<{
        menuItemId: number;
        specialInstructions: string | null;
    }>;
    queuedAt: string;
};

const databaseName = "servepos";
const storeName = "pending-orders";
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || "/api";

function openDatabase(): Promise<IDBDatabase> {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(databaseName, 1);

        request.onerror = () => reject(request.error);
        request.onupgradeneeded = () => {
            request.result.createObjectStore(storeName, { keyPath: "clientOrderId" });
        };
        request.onsuccess = () => resolve(request.result);
    });
}

async function withStore<T>(
    mode: IDBTransactionMode,
    operation: (store: IDBObjectStore) => IDBRequest<T>,
): Promise<T> {
    const database = await openDatabase();

    return new Promise((resolve, reject) => {
        const transaction = database.transaction(storeName, mode);
        const request = operation(transaction.objectStore(storeName));

        request.onerror = () => reject(request.error);
        transaction.onabort = () => reject(transaction.error);
        transaction.oncomplete = () => {
            database.close();
            resolve(request.result);
        };
    });
}

export function createClientOrderId(): string {
    return crypto.randomUUID();
}

export async function getPendingOrderCount(): Promise<number> {
    return withStore("readonly", store => store.count());
}

export async function queueOrder(order: PendingOrder): Promise<void> {
    await withStore("readwrite", store => store.put(order));
}

async function sendOrder(order: PendingOrder): Promise<void> {
    const response = await fetch(`${apiBaseUrl}/serve/order`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(order),
    });

    if (!response.ok) {
        throw new Error(`Bestellung konnte nicht übertragen werden (${response.status}).`);
    }
}

export async function submitOrder(order: PendingOrder): Promise<"submitted" | "queued"> {
    if (!navigator.onLine) {
        await queueOrder(order);
        return "queued";
    }

    try {
        await sendOrder(order);
        return "submitted";
    } catch {
        await queueOrder(order);
        return "queued";
    }
}

export async function syncPendingOrders(): Promise<number> {
    if (!navigator.onLine) {
        return getPendingOrderCount();
    }

    const pendingOrders = await withStore<PendingOrder[]>("readonly", store => store.getAll());
    for (const order of pendingOrders) {
        try {
            await sendOrder(order);
            await withStore("readwrite", store => store.delete(order.clientOrderId));
        } catch {
            break;
        }
    }

    return getPendingOrderCount();
}
