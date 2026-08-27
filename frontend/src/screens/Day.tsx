// فاز روز — دو مرحله‌ی پشت‌سرهم: نوبت صحبت، بعد رأی‌گیری.
//
// چیدمان از بالا به پایین بر اساس چیزی که بازیکن در آن لحظه لازم دارد:
// خبر شب → گوینده‌ی فعلی → صف نوبت → لیست بازیکنان برای رأی.

import { useEffect, useMemo, useRef, useState } from "react";
import { useGameStore } from "../store/gameStore";
import { SpeakerSpotlight } from "../components/SpeakerSpotlight";
import { MicToggle } from "../components/MicToggle";
import { ChatPanel } from "../components/ChatPanel";
import { StatusBar } from "../components/StatusBar";
import type { Player } from "../types";

// مدت نوبت را قرار است میزبان موقع ساخت بازی تعیین کند (۱ تا ۵ دقیقه).
// تا وقتی بک‌اند آن را برنگرداند، این پیش‌فرض محلی است.
const TURN_SECONDS = 90;

export function Day() {
  const players = useGameStore((s) => s.players);
  const round = useGameStore((s) => s.round);
  const lastDeath = useGameStore((s) => s.lastDeath);
  const votes = useGameStore((s) => s.votes);
  const castVote = useGameStore((s) => s.castVote);
  const retractVote = useGameStore((s) => s.retractVote);
  const me = useGameStore((s) => s.me());

  // ترتیب نوبت باید برای همه‌ی بازیکن‌ها یکسان باشد، پس بر اساس id مرتب می‌شود
  const alive = useMemo(
    () => players.filter((p) => p.alive).sort((a, b) => Number(a.id) - Number(b.id)),
    [players],
  );

  // ── مرحله‌ی نوبت صحبت ──────────────────────────────────────────────────
  const [turnIndex, setTurnIndex] = useState(0);
  const [secondsLeft, setSecondsLeft] = useState(TURN_SECONDS);
  const speakingDone = turnIndex >= alive.length;
  const speaker = speakingDone ? null : alive[turnIndex];
  const isMyTurn = !!speaker && !!me && speaker.id === me.id;

  const tickRef = useRef<ReturnType<typeof setInterval> | null>(null);
  useEffect(() => {
    if (speakingDone) return;
    tickRef.current = setInterval(() => {
      setSecondsLeft((s) => {
        if (s > 1) return s - 1;
        setTurnIndex((i) => i + 1);
        return TURN_SECONDS;
      });
    }, 1000);
    return () => { if (tickRef.current) clearInterval(tickRef.current); };
  }, [speakingDone]);

  function skipTurn() {
    setTurnIndex((i) => i + 1);
    setSecondsLeft(TURN_SECONDS);
  }

  // ── رأی‌گیری ───────────────────────────────────────────────────────────
  const voteCounts = useMemo(() => {
    const c: Record<string, number> = {};
    for (const t of Object.values(votes)) c[t] = (c[t] ?? 0) + 1;
    return c;
  }, [votes]);

  const myVote = me ? votes[me.id] : undefined;

  const leader = useMemo(() => {
    let top: { id: string; n: number } | null = null;
    for (const [id, n] of Object.entries(voteCounts)) {
      if (!top || n > top.n) top = { id, n };
    }
    return top;
  }, [voteCounts]);

  const deadPlayer = lastDeath ? players.find((p) => p.id === lastDeath.playerId) : null;
  const canVote = !!me?.alive && speakingDone;
  const votedFor = myVote ? players.find((p) => p.id === myVote) : undefined;

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col gap-4 p-5 pb-10 screen-enter">

      <header className="text-center">
        <p className="font-mono text-[0.68rem] tracking-[0.14em]" style={{ color: "var(--lamp)" }}>
          راند {round} · روز
        </p>
        <h2 className="text-2xl font-extrabold">
          {speakingDone ? "رأی‌گیری" : "نوبت صحبت"}
        </h2>
      </header>

      <StatusBar />

      {/* خبر شب اول از همه — تعیین می‌کند بحث از کجا شروع شود */}
      {deadPlayer && (
        <div
          role="status"
          aria-atomic="true"
          className="rounded-xl border px-3 py-2.5 text-center text-sm"
          style={{
            background: "rgba(156,43,50,0.14)",
            borderColor: "rgba(156,43,50,0.42)",
            color: "var(--blood-bright)",
          }}
        >
          {lastDeath?.cause === "night" ? "دیشب" : "دیروز"} «{deadPlayer.name}» از بازی خارج شد
        </div>
      )}

      {/* گوینده‌ی فعلی با هاله‌ی صدا */}
      <section
        className="rounded-2xl border"
        style={{ borderColor: "var(--rule)", background: "var(--table)" }}
      >
        <SpeakerSpotlight
          speaker={speaker}
          secondsLeft={secondsLeft}
          turnSeconds={TURN_SECONDS}
          isMyTurn={isMyTurn}
        />

        {isMyTurn && (
          <div className="px-4 pb-4">
            <button
              onClick={skipTurn}
              className="w-full cursor-pointer rounded-xl border py-3 text-sm font-bold transition-colors"
              style={{ borderColor: "var(--rule)", color: "var(--parchment-dim)", minHeight: 44 }}
            >
              حرفم تمومه، نوبت بعدی
            </button>
          </div>
        )}
      </section>

      {/* صف نوبت — بازیکن باید بداند کی نوبتش می‌رسد */}
      {!speakingDone && (
        <section aria-label="صف نوبت صحبت">
          <p className="mb-2 text-[0.7rem]" style={{ color: "var(--muted)" }}>ترتیب صحبت</p>
          <ol className="flex gap-2 overflow-x-auto pb-1">
            {alive.map((p, i) => {
              const done = i < turnIndex;
              const now = i === turnIndex;
              return (
                <li
                  key={p.id}
                  className="flex shrink-0 items-center gap-1.5 rounded-full border px-3 text-xs"
                  style={{
                    minHeight: 32,
                    borderColor: now ? "var(--blood-bright)" : "var(--rule)",
                    background: now ? "rgba(217,73,92,0.12)" : "transparent",
                    color: done ? "var(--muted)" : now ? "var(--parchment)" : "var(--parchment-dim)",
                    opacity: done ? 0.5 : 1,
                  }}
                >
                  {done && <span aria-hidden="true">✓</span>}
                  {p.name}
                </li>
              );
            })}
          </ol>
        </section>
      )}

      {/* رأی‌گیری */}
      <section aria-label="رأی‌گیری">
        <div className="mb-2 flex items-baseline justify-between gap-2">
          <p className="text-[0.7rem]" style={{ color: "var(--muted)" }}>
            {canVote ? "روی یک نفر بزن تا رأی بدی" : "بعد از پایان نوبت‌ها باز می‌شود"}
          </p>
          {leader && leader.n > 0 && (
            <p
              role="status"
              aria-atomic="true"
              className="shrink-0 text-[0.7rem]"
              style={{ color: "var(--blood-bright)" }}
            >
              بیشترین رأی: {players.find((p) => p.id === leader.id)?.name} با {leader.n} رأی
            </p>
          )}
        </div>

        <ul className="grid grid-cols-2 gap-2">
          {alive.map((p) => (
            <VoteCard
              key={p.id}
              player={p}
              count={voteCounts[p.id] ?? 0}
              isMine={myVote === p.id}
              selectable={canVote && p.id !== me?.id}
              onToggle={() => (myVote === p.id ? retractVote() : castVote(p.id))}
            />
          ))}
        </ul>

        {votedFor && (
          <p className="mt-2 text-center text-xs" style={{ color: "var(--parchment-dim)" }}>
            به «{votedFor.name}» رأی دادی — دوباره بزن تا پس بگیری
          </p>
        )}
      </section>

      <div className="flex flex-col gap-3">
        <MicToggle />
        <ChatPanel />
      </div>
    </div>
  );
}

interface VoteCardProps {
  player: Player;
  count: number;
  isMine: boolean;
  selectable: boolean;
  onToggle: () => void;
}

function VoteCard({ player, count, isMine, selectable, onToggle }: VoteCardProps) {
  return (
    <li>
      <button
        disabled={!selectable}
        onClick={onToggle}
        aria-pressed={isMine}
        className="flex w-full items-center gap-2.5 rounded-xl border p-2.5 text-right transition-colors"
        style={{
          minHeight: 56, // بالاتر از حداقل ۴۴pt هدف لمسی
          borderColor: isMine ? "var(--blood-bright)" : "var(--rule)",
          background: isMine ? "rgba(217,73,92,0.12)" : "var(--table)",
          cursor: selectable ? "pointer" : "default",
          opacity: selectable || isMine ? 1 : 0.6,
        }}
      >
        <span
          className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full border text-sm font-bold"
          style={{
            background: "var(--table-edge)",
            borderColor: player.isMe ? "var(--lamp)" : "var(--rule)",
          }}
        >
          {player.name.charAt(0)}
        </span>

        <span className="min-w-0 flex-1">
          <span className="block truncate text-sm">{player.name}</span>
          {player.isMe && (
            <span className="text-[0.62rem]" style={{ color: "var(--lamp)" }}>خودت</span>
          )}
        </span>

        {count > 0 && (
          <span
            className="flex h-6 min-w-6 items-center justify-center rounded-full px-1.5 font-mono text-xs tabular-nums"
            style={{ background: "var(--blood)", color: "var(--parchment)" }}
          >
            {count}
          </span>
        )}
      </button>
    </li>
  );
}
