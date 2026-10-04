import { FiRefreshCw } from "react-icons/fi";

type RefreshButtonProps = {
    onRefresh: () => void;
};

export default function RefreshButton({ onRefresh }: RefreshButtonProps) {
    return (
        <button
            onClick={onRefresh}
            className="px-4 py-2 bg-gray-200 rounded"
            title="Neu laden"
        >
            <FiRefreshCw />
        </button>
    );
}
