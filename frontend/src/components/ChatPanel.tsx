import { useMemo, useState } from "react";
import { useGameStore } from "../store/gameStore";
import type { ChatThreadType } from "../types";

const THREAD_LABEL: Record<ChatThreadType, string> = {
  lobby: "چت روم",
  dayPublic: "چت عمومی روز",
  nightMafia: "چت خصوصی مافیا",
  deadChat: "چت مرده‌ها 👻",
};

export function ChatPanel() {
  const chatMessages = useGameStore((s) => s.chatMessages);
  const sendChatMessage = useGameStore((s) => s.sendChatMessage);
  const thread = useGameStore((s) => s.activeThread());
  const [draft, setDraft] = useState("");

  const messages = useMemo(
    () => (thread ? chatMessages.filter((m) => m.thread === thread) : []),
    [chatMessages, thread]
  );

  if (!thread) {
    return (
      <div
        className="rounded-lg border p-3 text-center text-xs"
        style={{ borderColor: "var(--rule)", color: "var(--muted)" }}
      >
        شهر خوابه — چت غیرفعاله تا صبح بشه
      </div>
    );
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    sendChatMessage(draft);
    setDraft("");
  }

  return (
    <div
      className="flex h-56 flex-col rounded-lg border"
      style={{ borderColor: "var(--rule)", background: "var(--table)" }}
    >
      <div
        className="border-b px-3 py-1.5 font-mono text-[0.65rem] tracking-wide"
        style={{ borderColor: "var(--rule)", color: "var(--lamp)" }}
      >
        {THREAD_LABEL[thread]}
      </div>
      <div className="flex-1 space-y-1.5 overflow-y-auto px-3 py-2 text-sm">
        {messages.length === 0 && (
          <p className="text-xs" style={{ color: "var(--muted)" }}>
            هنوز پیامی نیست…
          </p>
        )}
        {messages.map((m) => (
          <div key={m.id}>
            <span className="font-bold" style={{ color: "var(--parchment-dim)" }}>
              {m.senderName}:
            </span>{" "}
            <span>{m.text}</span>
          </div>
        ))}
      </div>
      <form onSubmit={handleSubmit} className="flex gap-1.5 border-t p-1.5" style={{ borderColor: "var(--rule)" }}>
        <input
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          placeholder="پیام بنویس…"
          className="flex-1 rounded-md bg-transparent px-2 py-1.5 text-sm outline-none"
          style={{ color: "var(--parchment)" }}
        />
        <button
          type="submit"
          className="rounded-md px-3 py-1.5 text-sm font-bold"
          style={{ background: "var(--blood)", color: "var(--parchment)" }}
        >
          ارسال
        </button>
      </form>
    </div>
  );
}
