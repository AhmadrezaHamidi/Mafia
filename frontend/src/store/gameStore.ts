import { create } from "zustand";
import type { ChatMessage, ChatThreadType, GamePhase, Player, Role, WinningTeam } from "../types";

const NPC_NAMES = [
  "امیر", "نگار", "رضا", "پویا", "مریم", "کیان", "الهام",
  "سینا", "بهار", "آرش", "شیدا", "نیما", "ترانه", "یاسین",
];

const NIGHT_DURATION = 45;
const DAY_DURATION = 90;
const NPC_ACT_DELAY_MS = [1200, 3600];
const NPC_JOIN_DELAY_MS = [700, 2200];

const LOBBY_CHATTER = [
  "سلام به همه 👋", "امیدوارم این‌بار زودتر شناسایی بشن", "کی هاستمونه؟", "آماده‌ام شروع کنیم",
];
const DAY_CHATTER = [
  "به‌نظرم باید رو حرفای دیشب دقت کنیم", "کسی مشکوک نمی‌بینه؟", "من که بی‌گناهم!", "رأی من هنوز مشخص نیست",
];

function randomRoomCode(): string {
  const chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
  let code = "";
  for (let i = 0; i < 6; i++) code += chars[Math.floor(Math.random() * chars.length)];
  return code;
}

function assignRoles(players: Player[]): Player[] {
  const mafiaCount = Math.max(1, Math.round(players.length / 4));
  const shuffled = [...players].sort(() => Math.random() - 0.5);
  const mafiaIds = new Set(shuffled.slice(0, mafiaCount).map((p) => p.id));
  return players.map((p) => ({
    ...p,
    role: (mafiaIds.has(p.id) ? "SimpleMafia" : "SimpleCitizen") as Role,
  }));
}

function currentThread(me: Player | undefined, phase: GamePhase): ChatThreadType | null {
  if (!me) return null;
  if (!me.alive) return "deadChat";
  if (phase === "lobby") return "lobby";
  if (phase === "night") return me.role === "SimpleMafia" ? "nightMafia" : null;
  return "dayPublic";
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

  createRoom: (nickname: string, capacity: number) => void;
  joinRoom: (code: string, nickname: string) => void;
  startGame: () => void;
  submitNightAction: (targetId: string) => void;
  castVote: (targetId: string) => void;
  retractVote: () => void;
  toggleMic: () => void;
  sendChatMessage: (text: string) => void;
  requestRematch: () => void;
  leaveRoom: () => void;

  me: () => Player | undefined;
  alivePlayers: () => Player[];
  voteCounts: () => Record<string, number>;
  activeThread: () => ChatThreadType | null;
}

function makePlayer(id: string, name: string, isMe: boolean, isHost: boolean): Player {
  return { id, name, isMe, isHost, alive: true, connected: true, micMuted: false };
}

export const useGameStore = create<GameState>((set, get) => ({
  roomCode: null,
  capacity: 8,
  players: [],
  phase: "lobby",
  round: 1,
  deadline: null,
  timeLeftSec: 0,
  lastDeath: null,
  nightTarget: null,
  votes: {},
  winningTeam: null,
  timerHandle: null,
  chatMessages: [],

  createRoom: (nickname, capacity) => {
    clearPhaseTimer(get);
    set({
      roomCode: randomRoomCode(),
      capacity,
      players: [makePlayer("me", nickname || "من", true, true)],
      phase: "lobby",
      round: 1,
      lastDeath: null,
      nightTarget: null,
      votes: {},
      winningTeam: null,
      chatMessages: [],
    });
    scheduleNpcJoins(set, get, capacity - 1);
  },

  joinRoom: (code, nickname) => {
    clearPhaseTimer(get);
    const capacity = 8;
    const existingNpcCount = capacity - 2;
    const npcs = NPC_NAMES.slice(0, existingNpcCount).map((name, i) => makePlayer(`npc-${i}`, name, false, false));
    set({
      roomCode: code.toUpperCase(),
      capacity,
      players: [makePlayer("me", nickname || "من", true, false), ...npcs],
      phase: "lobby",
      round: 1,
      lastDeath: null,
      nightTarget: null,
      votes: {},
      winningTeam: null,
      chatMessages: [],
    });
    scheduleNpcJoins(set, get, 1);
  },

  startGame: () => {
    const { players } = get();
    set({ players: assignRoles(players), phase: "night", round: 1 });
    startPhaseTimer(set, get, "night", NIGHT_DURATION);
    scheduleNpcNightActions(set, get);
  },

  submitNightAction: (targetId) => {
    const me = get().me();
    if (!me || me.role !== "SimpleMafia" || !me.alive || get().phase !== "night") return;
    set({ nightTarget: targetId });
  },

  castVote: (targetId) => {
    const me = get().me();
    if (!me || !me.alive || get().phase !== "day") return;
    set((s) => ({ votes: { ...s.votes, [me.id]: targetId } }));
    maybeResolveVotingEarly(set, get);
  },

  retractVote: () => {
    const me = get().me();
    if (!me) return;
    set((s) => {
      const next = { ...s.votes };
      delete next[me.id];
      return { votes: next };
    });
  },

  toggleMic: () => {
    const me = get().me();
    if (!me) return;
    set((s) => ({
      players: s.players.map((p) => (p.id === me.id ? { ...p, micMuted: !p.micMuted } : p)),
    }));
  },

  sendChatMessage: (text) => {
    const trimmed = text.trim();
    if (!trimmed) return;
    const me = get().me();
    const thread = currentThread(me, get().phase);
    if (!me || !thread) return;
    pushChatMessage(set, thread, me.id, me.name, trimmed);
  },

  requestRematch: () => {
    const { players, roomCode, capacity } = get();
    const reset = players.map((p) => ({ ...p, alive: true, role: undefined }));
    clearPhaseTimer(get);
    set({
      roomCode,
      capacity,
      players: reset,
      phase: "lobby",
      round: 1,
      lastDeath: null,
      nightTarget: null,
      votes: {},
      winningTeam: null,
      chatMessages: [],
    });
  },

  leaveRoom: () => {
    clearPhaseTimer(get);
    set({
      roomCode: null,
      players: [],
      phase: "lobby",
      round: 1,
      lastDeath: null,
      nightTarget: null,
      votes: {},
      winningTeam: null,
      chatMessages: [],
    });
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
  activeThread: () => currentThread(get().me(), get().phase),
}));

function pushChatMessage(
  set: (partial: Partial<GameState>) => void,
  thread: ChatThreadType,
  senderId: string,
  senderName: string,
  text: string
) {
  const message: ChatMessage = {
    id: `${senderId}-${Date.now()}-${Math.random().toString(36).slice(2, 7)}`,
    thread,
    senderId,
    senderName,
    text,
    sentAtMs: Date.now(),
  };
  set((s) => ({ chatMessages: [...s.chatMessages, message] }));
}

function scheduleNpcJoins(
  set: (partial: Partial<GameState>) => void,
  get: () => GameState,
  npcCount: number
) {
  for (let i = 0; i < npcCount; i++) {
    const delay = (i + 1) * (NPC_JOIN_DELAY_MS[0] + Math.random() * (NPC_JOIN_DELAY_MS[1] - NPC_JOIN_DELAY_MS[0]));
    setTimeout(() => {
      if (get().phase !== "lobby") return;
      const npc = makePlayer(`npc-${i}`, NPC_NAMES[i % NPC_NAMES.length], false, false);
      set((s) => (s.players.some((p) => p.id === npc.id) ? {} : { players: [...s.players, npc] }));
      if (Math.random() < 0.5) {
        const line = LOBBY_CHATTER[Math.floor(Math.random() * LOBBY_CHATTER.length)];
        pushChatMessage(set, "lobby", npc.id, npc.name, line);
      }
    }, delay);
  }
}

function clearPhaseTimer(get: () => GameState) {
  const handle = get().timerHandle;
  if (handle) clearInterval(handle);
}

function startPhaseTimer(
  set: (partial: Partial<GameState>) => void,
  get: () => GameState,
  phase: GamePhase,
  durationSec: number
) {
  clearPhaseTimer(get);
  const deadline = Date.now() + durationSec * 1000;
  set({ deadline, timeLeftSec: durationSec });
  const handle = setInterval(() => {
    const remaining = Math.max(0, Math.round((get().deadline! - Date.now()) / 1000));
    set({ timeLeftSec: remaining });
    if (remaining <= 0) {
      clearInterval(handle);
      if (phase === "night") resolveNightPhase(set, get);
      if (phase === "day") resolveVoting(set, get);
    }
  }, 250);
  set({ timerHandle: handle });
}

function scheduleNpcNightActions(
  set: (partial: Partial<GameState>) => void,
  get: () => GameState
) {
  const mafiaNpcs = get().players.filter((p) => !p.isMe && p.role === "SimpleMafia" && p.alive);
  if (mafiaNpcs.length === 0) return;
  const delay = NPC_ACT_DELAY_MS[0] + Math.random() * (NPC_ACT_DELAY_MS[1] - NPC_ACT_DELAY_MS[0]);
  setTimeout(() => {
    if (get().phase !== "night") return;
    if (get().nightTarget) return;
    const candidates = get().players.filter((p) => p.alive && p.role !== "SimpleMafia");
    if (candidates.length === 0) return;
    const target = candidates[Math.floor(Math.random() * candidates.length)];
    set({ nightTarget: target.id });
  }, delay);
}

function resolveNightPhase(set: (partial: Partial<GameState>) => void, get: () => GameState) {
  const { round, nightTarget, players } = get();
  const killEnabled = round === 1;
  let updatedPlayers = players;
  let lastDeath: GameState["lastDeath"] = null;

  if (killEnabled && nightTarget) {
    updatedPlayers = players.map((p) => (p.id === nightTarget ? { ...p, alive: false } : p));
    lastDeath = { playerId: nightTarget, cause: "night" };
  }

  set({ players: updatedPlayers, lastDeath, nightTarget: null });

  if (checkWinCondition(set, get)) return;

  set({ phase: "day", votes: {} });
  startPhaseTimer(set, get, "day", DAY_DURATION);
  scheduleNpcVotes(set, get);
  scheduleNpcDayChatter(set, get);
}

function scheduleNpcVotes(set: (partial: Partial<GameState>) => void, get: () => GameState) {
  const npcs = get().players.filter((p) => !p.isMe && p.alive);
  npcs.forEach((npc, i) => {
    const delay = 1500 + i * 900 + Math.random() * 1500;
    setTimeout(() => {
      if (get().phase !== "day") return;
      if (get().votes[npc.id]) return;
      const candidates = get().players.filter((p) => p.alive && p.id !== npc.id);
      if (candidates.length === 0) return;
      const target = candidates[Math.floor(Math.random() * candidates.length)];
      set((s) => ({ votes: { ...s.votes, [npc.id]: target.id } }));
      maybeResolveVotingEarly(set, get);
    }, delay);
  });
}

function scheduleNpcDayChatter(set: (partial: Partial<GameState>) => void, get: () => GameState) {
  const npcs = get().players.filter((p) => !p.isMe && p.alive);
  npcs.forEach((npc, i) => {
    if (Math.random() > 0.6) return;
    const delay = 800 + i * 700 + Math.random() * 1200;
    setTimeout(() => {
      if (get().phase !== "day") return;
      const line = DAY_CHATTER[Math.floor(Math.random() * DAY_CHATTER.length)];
      pushChatMessage(set, "dayPublic", npc.id, npc.name, line);
    }, delay);
  });
}

function maybeResolveVotingEarly(
  set: (partial: Partial<GameState>) => void,
  get: () => GameState
) {
  const alive = get().alivePlayers();
  const voteCount = Object.keys(get().votes).length;
  if (get().phase === "day" && voteCount >= alive.length) {
    resolveVoting(set, get);
  }
}

function resolveVoting(set: (partial: Partial<GameState>) => void, get: () => GameState) {
  clearPhaseTimer(get);
  const counts = get().voteCounts();
  const entries = Object.entries(counts);
  let lastDeath: GameState["lastDeath"] = null;
  let updatedPlayers = get().players;

  if (entries.length > 0) {
    const max = Math.max(...entries.map(([, c]) => c));
    const topVoted = entries.filter(([, c]) => c === max);
    if (topVoted.length === 1) {
      const [eliminatedId] = topVoted[0];
      updatedPlayers = get().players.map((p) =>
        p.id === eliminatedId ? { ...p, alive: false } : p
      );
      lastDeath = { playerId: eliminatedId, cause: "day" };
    }
  }

  set({ players: updatedPlayers, lastDeath, votes: {} });

  if (checkWinCondition(set, get)) return;

  set((s) => ({ phase: "night", round: s.round + 1 }));
  startPhaseTimer(set, get, "night", NIGHT_DURATION);
  scheduleNpcNightActions(set, get);
}

function checkWinCondition(set: (partial: Partial<GameState>) => void, get: () => GameState): boolean {
  const alive = get().alivePlayers();
  const mafiaAlive = alive.filter((p) => p.role === "SimpleMafia").length;
  const townAlive = alive.length - mafiaAlive;

  if (mafiaAlive === 0) {
    clearPhaseTimer(get);
    set({ phase: "end", winningTeam: "town" });
    return true;
  }
  if (mafiaAlive >= townAlive) {
    clearPhaseTimer(get);
    set({ phase: "end", winningTeam: "mafia" });
    return true;
  }
  return false;
}
