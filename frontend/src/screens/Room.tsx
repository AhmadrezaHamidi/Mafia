import { Navigate } from "react-router-dom";
import { useGameStore } from "../store/gameStore";
import { Lobby } from "./Lobby";
import { Night } from "./Night";
import { Day } from "./Day";
import { End } from "./End";

export function Room() {
  const roomCode = useGameStore((s) => s.roomCode);
  const phase = useGameStore((s) => s.phase);

  if (!roomCode) return <Navigate to="/" replace />;

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
