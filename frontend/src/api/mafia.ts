// توابع تایپ‌دار روی ۱۰ endpoint بک‌اند.
// شکل تایپ‌ها دقیقاً از DTO های سمت سرور گرفته شده است.

import { request } from "./client";

// ── Room ─────────────────────────────────────────────────────────────────────

export interface CreateRoomResult {
  roomId: number;
  roomCode: string;
  hostPlayerId: number;
}

export interface JoinRoomResult {
  roomId: number;
  playerId: number;
}

export interface QuickJoinResult {
  roomId: number;
  roomCode: string;
  playerId: number;
  isHost: boolean;
}

export interface RoomMember {
  playerId: number;
  nickname: string;
  isHost: boolean;
}

export interface RoomView {
  roomId: number;
  roomCode: string;
  capacity: number;
  /** WaitingForPlayers | ReadyToStart | InGame | ... */
  status: string;
  /** Public | Private */
  visibility: string;
  members: RoomMember[];
  gameSessionId: number | null;
}

// ── Game ─────────────────────────────────────────────────────────────────────

export interface GamePlayerView {
  playerId: number;
  nickname: string;
  isAlive: boolean;
  connection: string;
}

export interface GameStateView {
  gameSessionId: number;
  /** Night | Day | Voting | Ended */
  phase: string;
  round: number;
  timeLeftSeconds: number;
  /** فقط نقش خودِ درخواست‌دهنده — نقش بقیه هرگز برنمی‌گردد */
  myRole: string | null;
  iAmAlive: boolean;
  /** فقط وقتی بیش از یک مافیا زنده باشه معنا داره؛ برای غیرمافیا همیشه null */
  myIsMafiaLeader: boolean | null;
  myNightTarget: number | null;
  players: GamePlayerView[];
  /** voterId → targetId */
  votes: Record<number, number> | null;
}

export interface RevealedPlayer {
  playerId: number;
  nickname: string;
  role: string;
  isAlive: boolean;
}

export interface GameResultView {
  gameSessionId: number;
  winningTeam: string;
  reveal: RevealedPlayer[];
}

export const roomApi = {
  create: (hostNickname: string, capacity: number, visibility: "Public" | "Private" = "Private") =>
    request<CreateRoomResult>("/Room/", { method: "POST", body: { hostNickname, capacity, visibility } }),

  join: (roomCode: string, nickname: string) =>
    request<JoinRoomResult>("/Room/Join", { method: "POST", body: { roomCode, nickname } }),

  /** «بازی سریع» — سرور یا به یه روم عمومیِ منتظر وصلمون می‌کنه یا یکی می‌سازه */
  quickJoin: (nickname: string) =>
    request<QuickJoinResult>("/Room/QuickJoin", { method: "POST", body: { nickname } }),

  get: (roomCode: string, signal?: AbortSignal) =>
    request<RoomView>(`/Room/${encodeURIComponent(roomCode)}`, { signal }),

  start: (roomId: number, requestingPlayerId: number) =>
    request<number>(`/Room/${roomId}/Start`, { method: "PUT", body: { requestingPlayerId } }),

  leave: (roomId: number, playerId: number) =>
    request<number>(`/Room/${roomId}/Members/${playerId}`, { method: "DELETE" }),
};

export const gameApi = {
  /** timeoutMs بالا داده می‌شود چون این همان کالی است که برای هم‌گام‌سازی مکرر صدا زده می‌شود */
  state: (gameSessionId: number, requestingPlayerId: number, signal?: AbortSignal) =>
    request<GameStateView>(`/Game/${gameSessionId}/State`, {
      query: { requestingPlayerId },
      timeoutMs: 30_000,
      signal,
    }),

  result: (gameSessionId: number) =>
    request<GameResultView>(`/Game/${gameSessionId}/Result`),

  nightAction: (gameSessionId: number, actorId: number, targetId: number) =>
    request<number>(`/Game/${gameSessionId}/Night/Action`, {
      method: "POST",
      body: { actorId, targetId },
    }),

  vote: (gameSessionId: number, voterId: number, targetId: number) =>
    request<number>(`/Game/${gameSessionId}/Day/Vote`, {
      method: "POST",
      body: { voterId, targetId },
    }),

  retractVote: (gameSessionId: number, voterId: number) =>
    request<number>(`/Game/${gameSessionId}/Day/Vote`, {
      method: "DELETE",
      query: { voterId },
    }),

  rematch: (gameSessionId: number) =>
    request<number>(`/Game/${gameSessionId}/Rematch`, { method: "POST" }),
};
