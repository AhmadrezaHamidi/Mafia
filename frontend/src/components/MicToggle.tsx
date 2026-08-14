import { useGameStore } from "../store/gameStore";

export function MicToggle() {
  const me = useGameStore((s) => s.me());
  const toggleMic = useGameStore((s) => s.toggleMic);

  if (!me) return null;

  return (
    <button
      type="button"
      onClick={toggleMic}
      className="flex items-center gap-2 self-center rounded-full border px-3 py-1.5 text-xs font-bold transition"
      style={{
        borderColor: me.micMuted ? "var(--rule)" : "var(--town)",
        color: me.micMuted ? "var(--muted)" : "var(--town)",
      }}
      aria-pressed={!me.micMuted}
    >
      {me.micMuted ? "🔇 میکروفون خاموش" : "🎙️ میکروفون روشن"}
    </button>
  );
}
