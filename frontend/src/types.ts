export type Role = "SimpleCitizen" | "SimpleMafia";

export type GamePhase = "lobby" | "night" | "day" | "end";

export interface Player {
  id: string;
  name: string;
  isMe: boolean;
  isHost: boolean;
  alive: boolean;
  connected: boolean;
  micMuted: boolean;
  role?: Role;
}

export type WinningTeam = "town" | "mafia" | null;

export type ChatThreadType = "lobby" | "dayPublic" | "nightMafia" | "deadChat";

export interface ChatMessage {
  id: string;
  thread: ChatThreadType;
  senderId: string;
  senderName: string;
  text: string;
  sentAtMs: number;
}
