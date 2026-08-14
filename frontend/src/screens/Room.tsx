import { useEffect } from "react";
import { Navigate, useParams } from "react-router-dom";
import { useGameStore } from "../store/gameStore";
import { Lobby } from "./Lobby";
import { Night } from "./Night";
import { Day } from "./Day";
import { End } from "./End";

export function Room() {
  const { code } = useParams<{ code: string }>();
  const phase = useGameStore((s) => s.phase);
  const players = useGameStore((s) => s.players);
  const error = useGameStore((s) => s.error);
  const enterRoom = useGameStore((s) => s.enterRoom);
  const stopSync = useGameStore((s) => s.stopSync);

  // هم‌گام‌سازی را با mount شروع و با unmount متوقف می‌کنیم.
  // چون code از URL می‌آید، refresh صفحه هم بازی را از دست نمی‌دهد.
  useEffect(() => {
    if (!code) return;
    enterRoom(code);
    return () => stopSync();
  }, [code, enterRoom, stopSync]);

  if (!code) return <Navigate to="/" replace />;

  // اولین بارگذاری، هنوز چیزی از سرور نیامده
  if (players.length === 0) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center gap-3 p-6 text-center">
        <p style={{ color: "var(--parchment-dim)" }}>
          {error ? "اتصال برقرار نشد" : "در حال اتصال به روم..."}
        </p>
        {error && (
          <p className="text-sm" style={{ color: "var(--blood-bright)" }} role="alert">
            {error}
          </p>
        )}
      </div>
    );
  }

  switch (phase) {
    case "lobby":
      return <Lobby />;
    case "night":
      return <Night />;
    case "day":
      return <Day />;
    case "end":
      return <End />;
  }
}
