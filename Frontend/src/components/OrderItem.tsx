import { RiDeleteBin6Line } from "react-icons/ri";
import { LiaCommentDotsSolid } from "react-icons/lia";
import type { MenuItem } from "../api";

type Item = {
    Id: number;
    item: MenuItem;
    quantity: number;
    comment: string;
};

type Props = {
    item: Item;
    onRemove: (itemId: number) => void;
    onAddComment: (itemId: number) => void;
};

export default function OrderSummary({
    item,
    onRemove,
    onAddComment,
}: Props) {
    return (
        <div key={`group-${item.Id}`} className="p-3 rounded border">
            <div className="flex justify-between items-start gap-2">
                <p className="flex-1 font-medium text-sm">{item.quantity}x {item.item.name}</p>
                <p className="font-bold ml-2">{(item.item.price * item.quantity).toFixed(2)} €</p>
            </div>
            <div className="flex items-center gap-2">
                <button
                    type="button"
                    onClick={() => onRemove(item.Id)}
                    className="px-3 py-1 rounded hover:bg-red-600"
                    title="Artikel entfernen">
                    <RiDeleteBin6Line />
                </button>
                <button
                    type="button"
                    onClick={() => onAddComment(item.Id)}
                    className="px-3 py-1 rounded hover:bg-gray-300"
                    title="Kommentar">
                    <LiaCommentDotsSolid />
                </button>
                {item.comment && <span className="ml-1 font-semibold">{item.comment}</span>}
            </div>
        </div>
    )
}
