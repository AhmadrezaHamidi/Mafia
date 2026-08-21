import { useState } from "react";
import { useGameStore } from "../store/gameStore";
import { Table } from "../components/Table";
import { MicToggle } from "../components/MicToggle";
import { ChatPanel } from "../components/ChatPanel";
import { RolesGallery } from "../components/RolesGallery";

// روی سرور زیر /Mafia سرو می‌شود؛ لینک دعوت هم باید همون base رو داشته باشه
// وگرنه لینکی که Host کپی می‌کنه برای مهمون‌ها 404 می‌ده.
const basename = import.meta.env.BASE_URL.replace(/\/$/, "");

export function Lobby() {
  const roomCode = useGameStore((s) => s.roomCode);
  const capacity = useGameStore((s) => s.capacity);
  const visibility = useGameStore((s) => s.visibility);
  const players = useGameStore((s) => s.players);
  const startGame = useGameStore((s) => s.startGame);
  const me = useGameStore((s) => s.me());
  const [copied, setCopied] = useState(false);
  const [rolesOpen, setRolesOpen] = useState(false);

  const isFull = players.length >= capacity;
  const isPublic = visibility === "Public";
  const joinLink = roomCode ? `${location.origin}${basename}/room/${roomCode}` : "";

  async function copyLink() {
    try {
      await navigator.clipboard.writeText(joinLink);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch {
      // مرورگرهای قدیمی/بدون HTTPS — کاربر خودش کد رو کپی می‌کنه
    }
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col gap-3 p-6 pb-10 screen-enter">
      {isPublic ? (
        <div
          className="mx-auto rounded-lg border border-dashed px-4 py-2 text-center text-sm"
          style={{ background: "var(--table)", borderColor: "var(--rule)", color: "var(--parchment-dim)" }}
        >
          🎲 صف بازی عمومی — به محض پر شدن ظرفیت خودکار شروع می‌شه
        </div>
      ) : (
        <div className="flex flex-col items-center gap-2">
          <div
            className="rounded-lg border border-dashed px-4 py-2 font-mono text-lg tracking-[0.25em]"
            style={{ background: "var(--table)", borderColor: "var(--rule)" }}
          >
            {roomCode}
          </div>
          <button
            onClick={copyLink}
            className="w-full truncate rounded-lg border px-3 py-2 text-xs"
            style={{ background: "var(--table-edge)", borderColor: "var(--rule)", color: "var(--parchment-dim)" }}
            title={joinLink}
          >
            {copied ? "✅ لینک کپی شد" : `🔗 ${joinLink}`}
          </button>
        </div>
      )}

      <p className="text-center text-sm" style={{ color: "var(--parchment-dim)" }}>
        {players.length} از {capacity} نفر
        {isFull ? " — همه اومدن" : " — در انتظار بقیه…"}
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

      {isPublic ? (
        <p className="text-center text-sm" style={{ color: "var(--muted)" }}>
          {isFull ? "روم پر شد — بازی داره شروع می‌شه…" : "به محض پر شدن ظرفیت، بازی خودش شروع می‌شه"}
        </p>
      ) : me?.isHost ? (
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
          منتظر بمون تا میزبان بازی رو شروع کنه
        </p>
      )}
      <p className="text-center text-xs" style={{ color: "var(--muted)" }}>
        بازی تا پر نشدن ظرفیت روم شروع نمی‌شه
      </p>

      <button
        onClick={() => setRolesOpen(true)}
        className="text-center text-xs underline underline-offset-4"
        style={{ color: "var(--parchment-dim)" }}
      >
        نقش‌های بازی رو ببین
      </button>

      <ChatPanel />
      <RolesGallery open={rolesOpen} onClose={() => setRolesOpen(false)} />
    </div>
  );
}
