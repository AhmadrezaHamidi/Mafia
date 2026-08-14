import { useGameStore } from "../store/gameStore";
import { Table } from "../components/Table";
import { MicToggle } from "../components/MicToggle";
import { ChatPanel } from "../components/ChatPanel";

export function Lobby() {
  const roomCode = useGameStore((s) => s.roomCode);
  const capacity = useGameStore((s) => s.capacity);
  const players = useGameStore((s) => s.players);
  const startGame = useGameStore((s) => s.startGame);
  const me = useGameStore((s) => s.me());

  const isFull = players.length >= capacity;

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col gap-3 p-6 pb-10 screen-enter">
      <div
        className="mx-auto rounded-lg border border-dashed px-4 py-2 font-mono text-lg tracking-[0.25em]"
        style={{ background: "var(--table)", borderColor: "var(--rule)" }}
      >
        {roomCode}
      </div>
      <p className="text-center text-sm" style={{ color: "var(--parchment-dim)" }}>
        {players.length} از {capacity} نفر
        {isFull ? " — همه اومدن، می‌تونی شروع کنی" : " — در انتظار بقیه…"}
      </p>

      <Table
        players={players}
        center={
          <div className="text-sm" style={{ color: "var(--parchment-dim)" }}>
            {isFull ? "همه حاضرن" : "در انتظار بازیکنان…"}
          </div>
        }
      />

      <MicToggle />

      {me?.isHost ? (
        <button
          onClick={startGame}
          disabled={!isFull}
          className="rounded-lg px-5 py-3 font-bold transition active:scale-[0.98] disabled:cursor-not-allowed disabled:opacity-40"
          style={{ background: "var(--blood)", color: "var(--parchment)" }}
        >
          شروع بازی
        </button>
      ) : (
        <p className="text-center text-sm" style={{ color: "var(--muted)" }}>
          منتظر بمون تا Host بازی رو شروع کنه
        </p>
      )}
      <p className="text-center text-xs" style={{ color: "var(--muted)" }}>
        بازی تا پر نشدن ظرفیت روم شروع نمی‌شه
      </p>

      <ChatPanel />
    </div>
  );
}
