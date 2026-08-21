import type { Role, Scenario } from "../types";
import { roleByKey } from "../data/roles";

const accentColors: Record<string, { bg: string; fg: string }> = {
  blood: { bg: "rgba(156,43,50,0.2)", fg: "var(--blood-bright)" },
  town: { bg: "rgba(79,143,130,0.2)", fg: "var(--town)" },
  lamp: { bg: "rgba(212,165,74,0.2)", fg: "var(--lamp)" },
};

/**
 * کارت نقشِ خودِ بازیکن — طراحیِ اختصاصی هر نقش (آیکون، رنگ تیم، توضیح مخصوص وضعیت فعلی).
 * منبع محتوا data/roles.ts است تا با گالری نقش‌ها هماهنگ بمونه.
 */
export function RoleCard({ role, hint, scenario }: { role: Role; hint: string; scenario?: Scenario }) {
  const info = roleByKey(role, scenario);
  const accent = accentColors[info?.accent ?? "town"];

  return (
    <div
      className="mt-auto flex items-center gap-3 rounded-[10px] border p-4"
      style={{ background: "var(--table)", borderColor: "var(--rule)" }}
    >
      <span
        className="flex flex-none h-10 w-10 items-center justify-center rounded-full text-xl"
        style={{ background: accent.bg }}
        aria-hidden
      >
        {info?.icon ?? "❔"}
      </span>
      <div className="min-w-0">
        <div className="mb-0.5 flex items-center gap-2">
          <span className="font-bold" style={{ color: "var(--parchment)" }}>
            {info?.name ?? role}
          </span>
          <span
            className="rounded-full px-2 py-0.5 font-mono text-[0.62rem] tracking-wide"
            style={{ background: accent.bg, color: accent.fg }}
          >
            تیم {info?.team ?? ""}
          </span>
        </div>
        <p className="m-0 text-sm" style={{ color: "var(--parchment-dim)" }}>
          {hint}
        </p>
      </div>
    </div>
  );
}
