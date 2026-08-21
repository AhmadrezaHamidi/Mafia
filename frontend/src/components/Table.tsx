import type { ReactNode } from "react";
import type { Player } from "../types";

interface TableProps {
  players: Player[];
  center: ReactNode;
  selectable?: boolean;
  selectedId?: string | null;
  onSelect?: (playerId: string) => void;
  voteCounts?: Record<string, number>;
  /** برای دکتر — نجاتِ خود امکان‌پذیره، پس باید بشه روی خودش هم زد */
  allowSelf?: boolean;
}

function seatPositions(n: number) {
  const pts: { x: number; y: number }[] = [];
  for (let i = 0; i < n; i++) {
    const angle = (i / n) * Math.PI * 2 - Math.PI / 2;
    pts.push({ x: 50 + 42 * Math.cos(angle), y: 50 + 42 * Math.sin(angle) });
  }
  return pts;
}

export function Table({ players, center, selectable, selectedId, onSelect, voteCounts, allowSelf }: TableProps) {
  const positions = seatPositions(players.length);

  return (
    <div className="relative mx-auto my-2 aspect-square w-full max-w-md">
      <div
        className="absolute -inset-[20%] rounded-full pointer-events-none animate-[flicker_6s_ease-in-out_infinite] motion-reduce:animate-none"
        style={{ background: "radial-gradient(circle at 50% 45%, var(--lamp-dim), transparent 65%)" }}
      />
      <div
        className="absolute inset-[14%] rounded-full border"
        style={{
          background: "radial-gradient(circle at 50% 40%, var(--table-edge), var(--table) 70%)",
          borderColor: "var(--rule)",
          boxShadow: "inset 0 0 40px rgba(0,0,0,0.5)",
        }}
      />
      <div className="absolute inset-0 flex items-center justify-center">
        <div className="relative z-[3] flex w-[62%] flex-col items-center gap-1 text-center">
          {center}
        </div>
      </div>

      {players.map((p, i) => {
        const pos = positions[i];
        const canSelect = selectable && p.alive && (allowSelf || !p.isMe);
        const isSelected = selectedId === p.id;
        const votes = voteCounts?.[p.id];
        return (
          <button
            type="button"
            key={p.id}
            disabled={!canSelect}
            onClick={() => canSelect && onSelect?.(p.id)}
            className="absolute w-14 -translate-x-1/2 -translate-y-1/2 text-center"
            style={{ left: `${pos.x}%`, top: `${pos.y}%` }}
          >
            <div
              className={[
                "relative mx-auto mb-1 flex h-12 w-12 items-center justify-center rounded-full border-2 font-bold transition-all",
                p.alive ? "" : "opacity-45 saturate-0",
                canSelect ? "cursor-pointer hover:scale-110" : "cursor-default",
              ].join(" ")}
              style={{
                background: "var(--table-edge)",
                borderColor: isSelected
                  ? "var(--blood-bright)"
                  : !p.alive
                    ? "var(--muted)"
                    : p.isMe
                      ? "var(--lamp)"
                      : "var(--rule)",
                borderStyle: p.alive ? "solid" : "dashed",
                boxShadow: isSelected ? "0 0 0 3px rgba(217,73,92,0.25)" : undefined,
              }}
            >
              {p.isHost && p.alive && (
                <span className="absolute -top-2 -end-1 text-xs" style={{ color: "var(--lamp)" }}>
                  ★
                </span>
              )}
              {votes ? (
                <span
                  className="absolute -top-1 -start-1 flex h-4 min-w-4 items-center justify-center rounded-full font-mono text-[0.62rem]"
                  style={{ background: "var(--blood)", color: "var(--parchment)" }}
                >
                  {votes}
                </span>
              ) : null}

              <span className={p.alive ? "" : "opacity-50"}>{p.name.charAt(0)}</span>

              {/* مرگ باید در یک نگاه دیده شود، نه از روی نبودن آیکون میکروفون */}
              {!p.alive && (
                <span
                  aria-hidden
                  className="pointer-events-none absolute inset-0 flex items-center justify-center text-2xl font-black"
                  style={{ color: "var(--blood-bright)", opacity: 0.85 }}
                >
                  ✕
                </span>
              )}

              {p.alive && (
                <span className="absolute -bottom-1 -end-1 text-[0.6rem] leading-none">
                  {p.micMuted ? "🔇" : "🎙️"}
                </span>
              )}
            </div>

            <div
              className="truncate text-[0.68rem]"
              style={{
                color: p.alive ? "var(--parchment-dim)" : "var(--muted)",
                textDecoration: p.alive ? undefined : "line-through",
              }}
            >
              {p.name}
            </div>
            {!p.alive && (
              <div className="text-[0.58rem] leading-tight" style={{ color: "var(--blood-bright)" }}>
                حذف شد
              </div>
            )}
          </button>
        );
      })}
    </div>
  );
}
