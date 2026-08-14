import { useGameStore } from "../store/gameStore";
import { Table } from "../components/Table";
import { RoleCard } from "../components/RoleCard";
import { MicToggle } from "../components/MicToggle";
import { ChatPanel } from "../components/ChatPanel";
import { StatusBar } from "../components/StatusBar";

export function Night() {
  const players = useGameStore((s) => s.players);
  const round = useGameStore((s) => s.round);
  const timeLeftSec = useGameStore((s) => s.timeLeftSec);
  const nightTarget = useGameStore((s) => s.nightTarget);
  const submitNightAction = useGameStore((s) => s.submitNightAction);
  const lastDeath = useGameStore((s) => s.lastDeath);
  const me = useGameStore((s) => s.me());

  const isMafia = me?.role === "SimpleMafia";
  const deadPlayer = lastDeath ? players.find((p) => p.id === lastDeath.playerId) : null;
  const minutes = String(Math.floor(timeLeftSec / 60)).padStart(2, "0");
  const seconds = String(timeLeftSec % 60).padStart(2, "0");

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col p-6 pb-10 screen-enter">
      <StatusBar />
      <div className="mb-1 text-center">
        <p
          className="mb-1 font-mono text-[0.68rem] uppercase tracking-[0.12em]"
          style={{ color: "var(--blood-bright)" }}
        >
          راند {round} · شب
        </p>
        <h2 className="text-2xl font-extrabold">شهر خوابیده…</h2>
        <p className="mt-1 text-sm" style={{ color: "var(--parchment-dim)" }}>
          {isMafia ? "قربانی امشب رو انتخاب کن" : "مافیا دارن قربانی امشب رو انتخاب می‌کنن"}
        </p>
      </div>

      {deadPlayer && (
        <div
          className="mb-3 rounded-lg border px-3 py-2 text-center text-sm"
          style={{
            background: "rgba(156,43,50,0.14)",
            borderColor: "rgba(156,43,50,0.4)",
            color: "var(--blood-bright)",
          }}
        >
          {lastDeath?.cause === "night" ? "دیشب" : "امروز با رأی شهر"} «{deadPlayer.name}» حذف شد
        </div>
      )}

      <Table
        players={players}
        selectable={isMafia && me?.alive}
        selectedId={nightTarget}
        onSelect={submitNightAction}
        center={
          <>
            <div className="font-mono text-3xl tabular-nums" style={{ color: "var(--lamp)" }}>
              {minutes}:{seconds}
            </div>
            <div className="text-sm" style={{ color: "var(--parchment-dim)" }}>
              تا پایان فاز شب
            </div>
          </>
        }
      />

      {me?.alive && me.role && (
        <RoleCard
          role={me.role}
          hint={
            isMafia
              ? "نقش تو مافیای ساده‌ست. روی یکی از هم‌بازی‌ها بزن تا هدف امشب رو انتخاب کنی."
              : "نقش تو شهروند ساده‌ست. فقط منتظر بمون تا صبح بشه."
          }
        />
      )}
      {isMafia && (
        <p className="mt-2 mb-2 text-center text-xs" style={{ color: "var(--muted)" }}>
          تا وقتی تایمر تموم نشه می‌تونی نظرت رو عوض کنی
        </p>
      )}

      <div className="mt-3 flex flex-col gap-3">
        <MicToggle />
        <ChatPanel />
      </div>
    </div>
  );
}
