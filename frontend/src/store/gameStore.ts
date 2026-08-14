// وضعیت بازی — پشتش API واقعی است، نه شبیه‌سازی محلی.
//
// منبع حقیقت سرور است. این store فقط آخرین snapshot را نگه می‌دارد و با
// polling تازه‌اش می‌کند (تصمیم معماری: به‌جای WebSocket برای state، درخواست
// با فاصله‌ی adaptive). تایمر بین دو poll به‌صورت محلی می‌شمارد تا روان بماند.

import { create } from "zustand";
import { gameApi, roomApi, type GameStateView, type RoomView } from "../api/mafia";
import {
  connectChat,
  disconnectChat,
  onChatMessage,
  onChatStatus,
  rejoin as rejoinChat,
  sendChat,
} from "../api/chatHub";
import type { ChatMessage, ChatThreadType, GamePhase, Player, Role, WinningTeam } from "../types";

const POLL_NORMAL = 2500;
const POLL_URGENT = 800;   // نزدیک پایان فاز
const POLL_HIDDEN = 8000;  // تب مخفی — سرور ۱ هسته‌ای را اذیت نکنیم
const URGENT_BELOW_SEC = 5;

// ── هویت بازیکن، تا refresh صفحه بازی را از دست ندهد ─────────────────────────
interface Identity {
  playerId: number;
  roomId: number;
}
const idKey = (code: string) => `mafia:id:${code.toUpperCase()}`;

// sessionStorage و نه localStorage: هویت باید مخصوص همین تب باشد.
// با localStorage اگر دو تب روی یک مرورگر باز شود (که برای دمو دادن کاملاً
// محتمل است) بازیکن دوم هویت اولی را بازنویسی می‌کند و هر دو تب فکر می‌کنند
// یک نفرند. sessionStorage هم refresh را تحمل می‌کند هم تب‌ها را جدا نگه می‌دارد.
function saveIdentity(code: string, id: Identity) {
  try { sessionStorage.setItem(idKey(code), JSON.stringify(id)); } catch { /* حالت خصوصی مرورگر */ }
}
function loadIdentity(code: string): Identity | null {
  try {
    const raw = sessionStorage.getItem(idKey(code));
    return raw ? (JSON.parse(raw) as Identity) : null;
  } catch { return null; }
}

// ── تبدیل شکل سرور به شکل UI ─────────────────────────────────────────────────
function mapPhase(serverPhase: string): GamePhase {
  switch (serverPhase) {
    case "Night": return "night";
    case "Day":
    case "Voting": return "day";
    case "Ended": return "end";
    default: return "lobby";
  }
}

function mapWinner(team: string | null | undefined): WinningTeam {
  if (!team) return null;
  const t = team.toLowerCase();
  if (t.includes("mafia")) return "mafia";
  if (t.includes("town") || t.includes("citizen")) return "town";
  return null;
}

interface GameState {
  roomCode: string | null;
  capacity: number;
  players: Player[];
  phase: GamePhase;
  round: number;
  deadline: number | null;
  timeLeftSec: number;
  lastDeath: { playerId: string; cause: "night" | "day" } | null;
  nightTarget: string | null;
  votes: Record<string, string>;
  winningTeam: WinningTeam;
  timerHandle: ReturnType<typeof setInterval> | null;
  chatMessages: ChatMessage[];
  /** اتصال SignalR برقرار است یا نه — برای نشان دادن وضعیت چت در UI */
  chatConnected: boolean;
  error: string | null;

  createRoom: (nickname: string, capacity: number) => Promise<string>;
  joinRoom: (code: string, nickname: string) => Promise<string>;
  /** شروع هم‌گام‌سازی برای یک روم — صفحه‌ی Room موقع mount صدا می‌زند */
  enterRoom: (code: string) => void;
  stopSync: () => void;
  startGame: () => void;
  submitNightAction: (targetId: string) => void;
  castVote: (targetId: string) => void;
  retractVote: () => void;
  toggleMic: () => void;
  sendChatMessage: (text: string) => Promise<void>;
  requestRematch: () => void;
  leaveRoom: () => void;

  me: () => Player | undefined;
  alivePlayers: () => Player[];
  voteCounts: () => Record<string, number>;
  activeThread: () => ChatThreadType | null;
}

// وضعیت هم‌گام‌سازی که نباید باعث re-render شود
let syncTimer: ReturnType<typeof setTimeout> | null = null;
let syncAbort: AbortController | null = null;
let myPlayerId: number | null = null;
let myRoomId: number | null = null;
let myGameSessionId: number | null = null;
let micMuted = false;
let unsubMessage: (() => void) | null = null;
let unsubStatus: (() => void) | null = null;
/** آخرین فازی که برای آن عضویت کانال چت را از سرور گرفتیم */
let chatJoinedForPhase: string | null = null;

export const useGameStore = create<GameState>((set, get) => {

  /** یک دور هم‌گام‌سازی؛ خودش دور بعدی را زمان‌بندی می‌کند */
  async function syncLoop() {
    const code = get().roomCode;
    if (!code) return;

    syncAbort?.abort();
    syncAbort = new AbortController();
    const signal = syncAbort.signal;

    let nextDelay = POLL_NORMAL;

    try {
      if (myGameSessionId == null) {
        // ── هنوز در Lobby ─────────────────────────────────────────────────
        const room: RoomView = await roomApi.get(code, signal);
        myRoomId = room.roomId;

        set({
          capacity: room.capacity,
          phase: room.gameSessionId ? get().phase : "lobby",
          players: room.members.map<Player>((m) => ({
            id: String(m.playerId),
            name: m.nickname,
            isMe: m.playerId === myPlayerId,
            isHost: m.isHost,
            alive: true,
            connected: true,
            micMuted: m.playerId === myPlayerId ? micMuted : false,
          })),
          error: null,
        });

        if (room.gameSessionId) {
          myGameSessionId = room.gameSessionId;
          nextDelay = 300; // فوراً برو سراغ state بازی
        }
      } else {
        // ── داخل بازی ─────────────────────────────────────────────────────
        const st: GameStateView = await gameApi.state(myGameSessionId, myPlayerId!, signal);
        const phase = mapPhase(st.phase);

        const votes: Record<string, string> = {};
        for (const [voter, target] of Object.entries(st.votes ?? {})) {
          votes[String(voter)] = String(target);
        }

        // تشخیص مرگ تازه با مقایسه‌ی snapshot قبلی — سرور رویدادی برای این نمی‌فرستد.
        // علت از فازی که *وارد* آن شده‌ایم استنتاج می‌شود: اگر روز شده یعنی
        // دیشب کشته شده، اگر شب شده یعنی با رأی روز حذف شده.
        const prevAlive = new Map(get().players.map((p) => [p.id, p.alive]));
        const justDied = st.players.find(
          (p) => !p.isAlive && prevAlive.get(String(p.playerId)) === true,
        );
        if (justDied) {
          set({
            lastDeath: {
              playerId: String(justDied.playerId),
              cause: phase === "night" ? "day" : "night",
            },
          });
        }

        // ForceChannelSwitch (سند ۰۷): با تغییر فاز یا مرگِ خودم، عضویت کانال
        // چت باید بازمحاسبه شود. کلاینت فقط درخواست می‌دهد؛ کانال را سرور
        // تعیین می‌کند — پس حتی اگر اینجا اشتباه صدا بزنیم، نشتی رخ نمی‌دهد.
        const chatKey = `${st.phase}:${st.iAmAlive}`;
        if (chatKey !== chatJoinedForPhase) {
          chatJoinedForPhase = chatKey;
          void rejoinChat();
        }

        set({
          phase,
          round: st.round,
          timeLeftSec: st.timeLeftSeconds,
          deadline: Date.now() + st.timeLeftSeconds * 1000,
          nightTarget: st.myNightTarget != null ? String(st.myNightTarget) : null,
          votes,
          players: st.players.map<Player>((p) => ({
            id: String(p.playerId),
            name: p.nickname,
            isMe: p.playerId === myPlayerId,
            isHost: String(p.playerId) === String(get().players.find((x) => x.isHost)?.id ?? ""),
            alive: p.isAlive,
            connected: p.connection === "Connected",
            micMuted: p.playerId === myPlayerId ? micMuted : false,
            role: p.playerId === myPlayerId ? (st.myRole as Role | undefined) : undefined,
          })),
          error: null,
        });

        if (phase === "end") {
          // نقش‌ها فقط بعد از پایان بازی افشا می‌شوند
          try {
            const res = await gameApi.result(myGameSessionId);
            set({
              winningTeam: mapWinner(res.winningTeam),
              players: res.reveal.map<Player>((r) => ({
                id: String(r.playerId),
                name: r.nickname,
                isMe: r.playerId === myPlayerId,
                isHost: false,
                alive: r.isAlive,
                connected: true,
                micMuted: false,
                role: r.role as Role,
              })),
            });
          } catch { /* نتیجه هنوز آماده نیست */ }
          return; // بازی تمام شده، polling لازم نیست
        }

        const hidden = typeof document !== "undefined" && document.hidden;
        nextDelay = hidden
          ? POLL_HIDDEN
          : st.timeLeftSeconds <= URGENT_BELOW_SEC
            ? POLL_URGENT
            : POLL_NORMAL;
      }
    } catch (err) {
      if ((err as Error)?.name === "AbortError") return;
      set({ error: (err as Error).message });
      nextDelay = 4000; // قطعی گذرا نباید هم‌گام‌سازی را بکشد
    }

    syncTimer = setTimeout(syncLoop, nextDelay);
  }

  /** شمارش محلی تایمر بین دو poll تا عدد روی صفحه نپرد */
  function ensureTicker() {
    if (get().timerHandle) return;
    const h = setInterval(() => {
      const { deadline, phase } = get();
      if (!deadline || phase === "lobby" || phase === "end") return;
      const left = Math.max(0, Math.round((deadline - Date.now()) / 1000));
      if (left !== get().timeLeftSec) set({ timeLeftSec: left });
    }, 1000);
    set({ timerHandle: h });
  }

  /** بعد از هر اکشن، فوراً تازه‌سازی کن به‌جای صبر تا poll بعدی */
  function refreshNow() {
    if (syncTimer) clearTimeout(syncTimer);
    syncTimer = setTimeout(syncLoop, 120);
  }

  async function withError(fn: () => Promise<unknown>) {
    try {
      await fn();
      refreshNow();
    } catch (err) {
      set({ error: (err as Error).message });
    }
  }

  return {
    roomCode: null,
    capacity: 0,
    players: [],
    phase: "lobby",
    round: 0,
    deadline: null,
    timeLeftSec: 0,
    lastDeath: null,
    nightTarget: null,
    votes: {},
    winningTeam: null,
    timerHandle: null,
    chatMessages: [],
    chatConnected: false,
    error: null,

    createRoom: async (nickname, capacity) => {
      const res = await roomApi.create(nickname.trim(), capacity);
      myPlayerId = res.hostPlayerId;
      myRoomId = res.roomId;
      myGameSessionId = null;
      saveIdentity(res.roomCode, { playerId: res.hostPlayerId, roomId: res.roomId });
      set({ roomCode: res.roomCode, phase: "lobby", error: null });
      return res.roomCode;
    },

    joinRoom: async (code, nickname) => {
      const c = code.trim().toUpperCase();
      const res = await roomApi.join(c, nickname.trim());
      myPlayerId = res.playerId;
      myRoomId = res.roomId;
      myGameSessionId = null;
      saveIdentity(c, { playerId: res.playerId, roomId: res.roomId });
      set({ roomCode: c, phase: "lobby", error: null });
      return c;
    },

    enterRoom: (code) => {
      const c = code.trim().toUpperCase();
      // بعد از refresh صفحه، هویت را از localStorage برگردان
      if (myPlayerId == null) {
        const saved = loadIdentity(c);
        if (saved) { myPlayerId = saved.playerId; myRoomId = saved.roomId; }
      }
      set({ roomCode: c });
      ensureTicker();
      if (syncTimer) clearTimeout(syncTimer);
      syncLoop();

      // چت بلادرنگ — تنها چیزی که واقعاً به push نیاز دارد
      if (myPlayerId != null) {
        unsubMessage?.();
        unsubStatus?.();
        unsubMessage = onChatMessage((msg) => {
          // پیام تکراری نگیریم (reconnect می‌تواند باعث دوباره رسیدن شود)
          if (get().chatMessages.some((m) => m.id === msg.id)) return;
          set({ chatMessages: [...get().chatMessages, msg] });
        });
        unsubStatus = onChatStatus((connected) => set({ chatConnected: connected }));
        void connectChat(c, myPlayerId);
      }
    },

    stopSync: () => {
      if (syncTimer) { clearTimeout(syncTimer); syncTimer = null; }
      syncAbort?.abort();
      const h = get().timerHandle;
      if (h) { clearInterval(h); set({ timerHandle: null }); }
      unsubMessage?.(); unsubMessage = null;
      unsubStatus?.();  unsubStatus = null;
      void disconnectChat();
    },

    startGame: () => {
      if (myRoomId == null || myPlayerId == null) return;
      void withError(() => roomApi.start(myRoomId!, myPlayerId!));
    },

    submitNightAction: (targetId) => {
      if (myGameSessionId == null || myPlayerId == null) return;
      void withError(() => gameApi.nightAction(myGameSessionId!, myPlayerId!, Number(targetId)));
    },

    castVote: (targetId) => {
      if (myGameSessionId == null || myPlayerId == null) return;
      void withError(() => gameApi.vote(myGameSessionId!, myPlayerId!, Number(targetId)));
    },

    retractVote: () => {
      if (myGameSessionId == null || myPlayerId == null) return;
      void withError(() => gameApi.retractVote(myGameSessionId!, myPlayerId!));
    },

    requestRematch: () => {
      if (myGameSessionId == null) return;
      void withError(async () => {
        await gameApi.rematch(myGameSessionId!);
        myGameSessionId = null; // بازی جدید؛ برگرد به رصد روم
        set({ phase: "lobby", winningTeam: null, votes: {}, round: 0 });
      });
    },

    leaveRoom: () => {
      const code = get().roomCode;
      if (myRoomId != null && myPlayerId != null) {
        void roomApi.leave(myRoomId, myPlayerId).catch(() => { /* در حال خروج، مهم نیست */ });
      }
      get().stopSync();
      if (code) { try { localStorage.removeItem(idKey(code)); } catch { /* ignore */ } }
      myPlayerId = null; myRoomId = null; myGameSessionId = null;
      set({
        roomCode: null, players: [], phase: "lobby", round: 0, votes: {},
        winningTeam: null, nightTarget: null, deadline: null, timeLeftSec: 0,
        chatMessages: [], error: null,
      });
    },

    // ── چت و میکروفن: فعلاً محلی. در مرحله‌ی بعد با SignalR واقعی می‌شوند ──────
    toggleMic: () => {
      micMuted = !micMuted;
      set({ players: get().players.map((p) => (p.isMe ? { ...p, micMuted } : p)) });
    },

    // پیام از طریق hub می‌رود و از همان مسیر برمی‌گردد (echo سرور)، پس
    // اینجا چیزی به لیست اضافه نمی‌کنیم — وگرنه پیام خودمان دوبار دیده می‌شود.
    sendChatMessage: async (text) => {
      const body = text.trim();
      const me = get().me();
      if (!body || !me) return;
      try {
        await sendChat(me.name, body);
      } catch (err) {
        set({ error: (err as Error).message });
      }
    },

    me: () => get().players.find((p) => p.isMe),
    alivePlayers: () => get().players.filter((p) => p.alive),

    voteCounts: () => {
      const counts: Record<string, number> = {};
      for (const target of Object.values(get().votes)) {
        counts[target] = (counts[target] ?? 0) + 1;
      }
      return counts;
    },

    activeThread: () => {
      const { phase } = get();
      const me = get().me();
      if (!me) return null;
      if (phase === "lobby") return "lobby";
      if (!me.alive) return "deadChat";
      if (phase === "night") return me.role === "SimpleMafia" ? "nightMafia" : null;
      if (phase === "day") return "dayPublic";
      return null;
    },
  };
});
