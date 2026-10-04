import { IoArrowBackOutline } from "react-icons/io5";
import { useNavigate } from "react-router-dom";

type HomeButtonProps = {
    onBeforeNavigate?: () => boolean;
};

export default function HomeButton({ onBeforeNavigate }: HomeButtonProps) {
    const navigate = useNavigate();

    function handleClick() {
        if (onBeforeNavigate && !onBeforeNavigate()) {
            return;
        }
        navigate("/");
    }

    return (<button
        onClick={handleClick}
        className="px-4 py-2 bg-gray-200 rounded"
    >
        <IoArrowBackOutline />
    </button>);
}
