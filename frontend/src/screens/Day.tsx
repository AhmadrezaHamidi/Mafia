import { useMemo } from "react";
import { useGameStore } from "../store/gameStore";
import { Table } from "../components/Table";
import { RoleCard } from "../components/RoleCard";
import { MicToggle } from "../components/MicToggle";
import { ChatPanel } from "../components/ChatPanel";

export function Day() {
  const players = useGameStore((s) => s.players);
  const round = useGameStore((s) => s.round);
  const timeLeftSec = useGameStore((s) => s.timeLeftSec);
  const lastDeath = useGameStore((s) => s.lastDeath);
  const votes = useGameStore((s) => s.votes);
  const castVote = useGameStore((s) => s.castVote);
  const me = useGameStore((s) => s.me());

  const voteCounts = useMemo(() => {
    const counts: Record<string, number> = {};
    for (const target of Object.values(votes)) counts[target] = (counts[target] ?? 0) + 1;
    return counts;
  }, [votes]);

  const minutes = String(Math.floor(timeLeftSec / 60)).padStart(2, "0");
  const seconds = String(timeLeftSec % 60).padStart(2, "0");
  const myVoteTarget = me ? votes[me.id] : undefined;
  const deadPlayer = lastDeath ? players.find((p) => p.id === lastDeath.playerId) : null;

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col p-6 pb-10 screen-enter">
      <div className="mb-1 text-center">
        <p
          className="mb-1 font-mono text-[0.68rem] uppercase tracking-[0.12em]"
          style={{ color: "var(--lamp)" }}
        >
          راند {round} · روز
        </p>
        <h2 className="text-2xl font-extrabold">رأی‌گیری علنی</h2>
      </div>

      {deadPlayer && (
        <div
          className="mb-4 rounded-lg border px-3 py-2 text-center text-sm"
          style={{
            background: "rgba(156,43,50,0.14)",
            borderColor: "rgba(156,43,50,0.4)",
            color: "var(--blood-bright)",
          }}
        >
          {lastDeath?.cause === "night" ? "دیشب" : "دیروز"} «{deadPlayer.name}» حذف شد
        </div>
      )}

      <Table
        players={players}
        selectable={me?.alive}
        selectedId={myVoteTarget}
        onSelect={castVote}
        voteCounts={voteCounts}
        center={
          <>
            <div className="font-mono text-3xl tabular-nums" style={{ color: "var(--lamp)" }}>
              {minutes}:{seconds}
            </div>
            <div className="text-sm" style={{ color: "var(--parchment-dim)" }}>
              تا پایان رأی‌گیری
            </div>
          </>
        }
      />

      {me?.alive && me.role && (
        <RoleCard role={me.role} hint="روی یکی از بازیکن‌های زنده بزن تا رأی بدی — رأی‌ها برای همه نمایش داده می‌شه." />
      )}

      <div className="mt-3 flex flex-col gap-3">
        <MicToggle />
        <ChatPanel />
      </div>
    </div>
  );
}
