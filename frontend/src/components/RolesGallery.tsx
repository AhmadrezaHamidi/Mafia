// گالری نقش‌ها — یه modal که هرکسی هر وقت خواست (قبل از بازی، توی لابی، وسط بازی)
// می‌تونه نقش‌های هر دو سناریو رو با توضیحاتشون ببینه. محتوا از data/roles.ts می‌آد.

import { useEffect, useState } from "react";
import { rolesByScenario, scenarios, mafiaLeaderNote, type RoleInfo } from "../data/roles";
import type { Scenario } from "../types";

const accentColors: Record<RoleInfo["accent"], { bg: string; fg: string }> = {
  blood: { bg: "rgba(156,43,50,0.2)", fg: "var(--blood-bright)" },
  town: { bg: "rgba(79,143,130,0.2)", fg: "var(--town)" },
  lamp: { bg: "rgba(212,165,74,0.2)", fg: "var(--lamp)" },
};

export function RolesGallery({
  open,
  onClose,
  initialScenario = "RussianMafia",
}: {
  open: boolean;
  onClose: () => void;
  initialScenario?: Scenario;
}) {
  const [tab, setTab] = useState<Scenario>(initialScenario);
  useEffect(() => {
    if (open) setTab(initialScenario);
  }, [open, initialScenario]);
  if (!open) return null;

  const list = rolesByScenario(tab);

  return (
    <div
      className="fixed inset-0 z-50 flex items-end justify-center p-0 sm:items-center sm:p-6"
      style={{ background: "rgba(13,11,10,0.72)" }}
      role="dialog"
      aria-modal="true"
      aria-label="گالری نقش‌ها"
      onClick={onClose}
    >
      <div
        className="max-h-[85vh] w-full max-w-md overflow-y-auto rounded-t-2xl border p-5 sm:rounded-2xl"
        style={{ background: "var(--table)", borderColor: "var(--rule)" }}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-lg font-extrabold">نقش‌های بازی</h2>
          <button
            onClick={onClose}
            aria-label="بستن"
            className="rounded-full px-2.5 py-1 text-sm"
            style={{ background: "var(--table-edge)", color: "var(--parchment-dim)" }}
          >
            ✕
          </button>
        </div>

        <div className="mb-4 flex gap-2">
          {scenarios.map((s) => (
            <button
              key={s.key}
              onClick={() => setTab(s.key)}
              className="flex-1 rounded-lg border px-2 py-2 text-xs font-bold transition"
              style={
                tab === s.key
                  ? { background: "var(--blood)", borderColor: "var(--blood)", color: "var(--parchment)" }
                  : { background: "var(--table-edge)", borderColor: "var(--rule)", color: "var(--parchment-dim)" }
              }
              title={s.description}
            >
              {s.name}
            </button>
          ))}
        </div>
        <p className="mb-3 text-xs" style={{ color: "var(--muted)" }}>
          {scenarios.find((s) => s.key === tab)?.description}
        </p>

        <div className="flex flex-col gap-3">
          {list.map((r) => {
            const accent = accentColors[r.accent];
            return (
              <div
                key={r.key}
                className="rounded-xl border p-4"
                style={{ borderColor: "var(--rule)", background: "var(--table-edge)" }}
              >
                <div className="mb-2 flex items-center gap-2">
                  <span
                    className="flex h-10 w-10 flex-none items-center justify-center rounded-full text-xl"
                    style={{ background: accent.bg }}
                    aria-hidden
                  >
                    {r.icon}
                  </span>
                  <div>
                    <div className="font-bold">{r.name}</div>
                    <span
                      className="rounded-full px-2 py-0.5 font-mono text-[0.62rem] tracking-wide"
                      style={{ background: accent.bg, color: accent.fg }}
                    >
                      تیم {r.team}
                    </span>
                  </div>
                </div>
                <p className="mb-1.5 text-sm" style={{ color: "var(--parchment-dim)" }}>{r.summary}</p>
                <p className="mb-1 text-xs" style={{ color: "var(--muted)" }}>
                  <b style={{ color: "var(--parchment-dim)" }}>قابلیت شب: </b>{r.nightAbility}
                </p>
                <p className="text-xs" style={{ color: "var(--muted)" }}>
                  <b style={{ color: "var(--parchment-dim)" }}>شرط برد: </b>{r.winCondition}
                </p>
              </div>
            );
          })}

          <div
            className="rounded-xl border border-dashed p-4 text-xs"
            style={{ borderColor: "var(--rule)", color: "var(--parchment-dim)" }}
          >
            <b style={{ color: "var(--lamp)" }}>رئیس مافیا: </b>{mafiaLeaderNote}
          </div>
        </div>
      </div>
    </div>
  );
}
