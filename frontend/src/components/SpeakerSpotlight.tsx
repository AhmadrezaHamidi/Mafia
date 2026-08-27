// گوینده‌ی نوبت‌دار، با هاله‌ای که با صدا نفس می‌کشد.
//
// هاله تنها انیمیشن ضروری بازی است: در بازی حضوری نگاه‌ها روی گوینده است و
// این همان کار را می‌کند. مقدارش از `--level` می‌آید که hook صدا مستقیم روی
// همین عنصر می‌نویسد — نه از props، تا رندر ۶۰ فریمی رخ ندهد.

import type { Player } from "../types";
import { useVoiceLevel } from "../hooks/useVoiceLevel";

interface Props {
  speaker: Player | null;
  /** ثانیه‌ی باقی‌مانده از نوبت این گوینده */
  secondsLeft: number;
  /** کل مدت نوبت، برای محاسبه‌ی حلقه‌ی پیشرفت */
  turnSeconds: number;
  isMyTurn: boolean;
}

export function SpeakerSpotlight({ speaker, secondsLeft, turnSeconds, isMyTurn }: Props) {
  const haloRef = useVoiceLevel({ active: isMyTurn });

  if (!speaker) {
    return (
      <div
        className="flex h-44 flex-col items-center justify-center gap-1 rounded-2xl border text-center"
        style={{ borderColor: "var(--rule)", background: "var(--table)" }}
      >
        <p className="text-sm" style={{ color: "var(--parchment-dim)" }}>
          نوبت‌ها تمام شد
        </p>
        <p className="text-xs" style={{ color: "var(--muted)" }}>
          وقت رأی‌گیریه
        </p>
      </div>
    );
  }

  const pct = turnSeconds > 0 ? Math.max(0, Math.min(1, secondsLeft / turnSeconds)) : 0;
  const R = 54;
  const CIRC = 2 * Math.PI * R;
  const urgent = secondsLeft <= 10;

  return (
    <div className="flex flex-col items-center gap-3 py-2">
      {/* اعلام نوبت — یک live region واحد و atomic، نه چند تای رقیب */}
      <p
        role="status"
        aria-atomic="true"
        className="font-mono text-[0.7rem] tracking-[0.14em]"
        style={{ color: isMyTurn ? "var(--blood-bright)" : "var(--parchment-dim)" }}
      >
        {isMyTurn ? "نوبت توئه — حرف بزن" : `نوبت ${speaker.name}`}
      </p>

      <div
        ref={haloRef as React.RefObject<HTMLDivElement>}
        className="halo relative flex h-32 w-32 items-center justify-center"
        style={{ ["--level" as string]: 0 }}
      >
        {/* هاله: با --level بزرگ و پررنگ می‌شود */}
        <span aria-hidden="true" className="halo-ring" />
        <span aria-hidden="true" className="halo-ring halo-ring--outer" />

        {/* حلقه‌ی زمان باقی‌مانده */}
        <svg viewBox="0 0 128 128" className="absolute inset-0 -rotate-90">
          <circle
            cx="64" cy="64" r={R} fill="none" strokeWidth="3"
            style={{ stroke: "var(--rule)" }}
          />
          <circle
            cx="64" cy="64" r={R} fill="none" strokeWidth="3" strokeLinecap="round"
            style={{
              stroke: urgent ? "var(--blood-bright)" : "var(--lamp)",
              strokeDasharray: CIRC,
              strokeDashoffset: CIRC * (1 - pct),
              transition: "stroke-dashoffset 1s linear, stroke 300ms ease",
            }}
          />
        </svg>

        {/* آواتار */}
        <div
          className="relative z-[2] flex h-20 w-20 items-center justify-center rounded-full border-2 text-2xl font-bold"
          style={{
            background: "var(--table-edge)",
            borderColor: isMyTurn ? "var(--blood-bright)" : "var(--rule)",
            color: "var(--parchment)",
          }}
        >
          {speaker.name.charAt(0)}
        </div>
      </div>

      <div className="text-center">
        <p className="font-bold">{speaker.name}</p>
        <p
          className="font-mono text-2xl tabular-nums"
          style={{ color: urgent ? "var(--blood-bright)" : "var(--lamp)" }}
        >
          {String(Math.floor(secondsLeft / 60)).padStart(2, "0")}:
          {String(secondsLeft % 60).padStart(2, "0")}
        </p>
      </div>
    </div>
  );
}
