import { useCallback, useEffect, useRef, useState } from "react";

import HomeButton from "../components/HomeButton";
import MenuAdministration from "../components/MenuAdministration";
import RefreshButton from "../components/RefreshButton";
import StaffAdministration from "../components/StaffAdministration";
import ButtonGroup from "../components/ButtonGroup";

export default function Admin() {
    const menuRefreshRef = useRef<(() => Promise<void>) | null>(null);
    const staffRefreshRef = useRef<(() => Promise<void>) | null>(null);
    const [isAnyMenuRowEditing, setIsAnyMenuRowEditing] = useState(false);

    useEffect(() => {
        function handleBeforeUnload(event: BeforeUnloadEvent) {
            if (!isAnyMenuRowEditing) {
                return;
            }

            event.preventDefault();
            event.returnValue = "";
        }

        window.addEventListener("beforeunload", handleBeforeUnload);
        return () => {
            window.removeEventListener("beforeunload", handleBeforeUnload);
        };
    }, [isAnyMenuRowEditing]);

    function confirmLeaveIfInEditModeExists() {
        if (!isAnyMenuRowEditing) {
            return true;
        }

        return window.confirm("Mindestens ein Menuepunkt wird bearbeitet. Wirklich zur Startseite wechseln?");
    }

    const handleRegisterMenuRefresh = useCallback((refreshFn: () => Promise<void>) => {
        menuRefreshRef.current = refreshFn;
    }, []);

    const handleRegisterStaffRefresh = useCallback((refreshFn: () => Promise<void>) => {
        staffRefreshRef.current = refreshFn;
    }, []);

    const handleRefreshAll = useCallback(async () => {
        const tasks: Promise<void>[] = [];

        if (menuRefreshRef.current) {
            tasks.push(menuRefreshRef.current());
        }

        if (staffRefreshRef.current) {
            tasks.push(staffRefreshRef.current());
        }

        await Promise.all(tasks);
    }, []);

    return (
        <div>
            <ButtonGroup>
                <HomeButton onBeforeNavigate={confirmLeaveIfInEditModeExists} />
                <RefreshButton onRefresh={() => void handleRefreshAll()} />
            </ButtonGroup>

            <MenuAdministration
                onRegisterRefresh={handleRegisterMenuRefresh}
                onAnyRowEditingChange={setIsAnyMenuRowEditing}
            />

            <StaffAdministration onRegisterRefresh={handleRegisterStaffRefresh} />
        </div>
    );
}
