// نوار وضعیت بالای صفحه‌های داخل بازی — شمارش زنده‌ها و هشدار قطعی اتصال.

import { useGameStore } from "../store/gameStore";

export function StatusBar() {
  const players = useGameStore((s) => s.players);
  const error = useGameStore((s) => s.error);
  const me = useGameStore((s) => s.me());

  const alive = players.filter((p) => p.alive).length;
  const dead = players.length - alive;

  return (
    <div className="mb-3 flex flex-col gap-2">
      <div
        className="flex items-center justify-between rounded-lg border px-3 py-2 text-[0.72rem]"
        style={{ background: "var(--table)", borderColor: "var(--rule)" }}
      >
        <span className="flex items-center gap-1.5">
          <span
            className="inline-block h-2 w-2 rounded-full"
            style={{ background: "var(--town)" }}
            aria-hidden
          />
          <span style={{ color: "var(--parchment-dim)" }}>زنده</span>
          <b className="font-mono tabular-nums" style={{ color: "var(--parchment)" }}>
            {alive}
          </b>
          <span style={{ color: "var(--muted)" }}>از {players.length}</span>
        </span>

        {dead > 0 && (
          <span className="flex items-center gap-1.5">
            <span style={{ color: "var(--muted)" }}>حذف‌شده</span>
            <b className="font-mono tabular-nums" style={{ color: "var(--blood-bright)" }}>
              {dead}
            </b>
          </span>
        )}

        {me && !me.alive && (
          <span
            className="rounded px-2 py-0.5 font-bold"
            style={{ background: "rgba(156,43,50,0.2)", color: "var(--blood-bright)" }}
          >
            تو حذف شدی
          </span>
        )}
      </div>

      {/* قطعی اتصال باید وسط بازی هم دیده شود، نه فقط موقع ورود */}
      {error && (
        <div
          role="alert"
          className="flex items-center gap-2 rounded-lg border px-3 py-2 text-[0.72rem]"
          style={{
            background: "rgba(156,43,50,0.12)",
            borderColor: "rgba(156,43,50,0.35)",
            color: "var(--blood-bright)",
          }}
        >
          <span
            className="inline-block h-2 w-2 shrink-0 animate-pulse rounded-full"
            style={{ background: "var(--blood-bright)" }}
            aria-hidden
          />
          <span>ارتباط با سرور قطع شد — در حال تلاش دوباره…</span>
        </div>
      )}
    </div>
  );
}
