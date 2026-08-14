// اتصال SignalR برای چت بلادرنگ.
//
// تقسیم کار عمدی: وضعیت بازی با polling می‌آید (تصمیم معماری مرحله‌ی ۲)،
// و SignalR فقط برای چیزی است که واقعاً باید لحظه‌ای باشد — حرف زدن بازیکن‌ها.
//
// کلاینت هیچ‌وقت نمی‌گوید در کدام کانال است؛ فقط roomCode و playerId می‌فرستد
// و سرور تصمیم می‌گیرد. بعد از هر تغییر فاز، rejoin صدا زده می‌شود تا سرور
// عضویت را بازمحاسبه کند.

import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import type { ChatMessage } from "../types";

const HUB_URL = (import.meta.env.BASE_URL.replace(/\/$/, "") || "") + "/hubs/chat";

let connection: HubConnection | null = null;
let currentRoom: string | null = null;
let currentPlayer: number | null = null;

type MessageHandler = (msg: ChatMessage) => void;
type StatusHandler = (connected: boolean) => void;

const messageHandlers = new Set<MessageHandler>();
const statusHandlers = new Set<StatusHandler>();

export function onChatMessage(fn: MessageHandler): () => void {
  messageHandlers.add(fn);
  return () => messageHandlers.delete(fn);
}

export function onChatStatus(fn: StatusHandler): () => void {
  statusHandlers.add(fn);
  return () => statusHandlers.delete(fn);
}

function emitStatus(connected: boolean) {
  statusHandlers.forEach((h) => h(connected));
}

/** اتصال و عضویت در کانالِ مجاز. اگر از قبل وصل باشد فقط rejoin می‌کند. */
export async function connectChat(roomCode: string, playerId: number): Promise<void> {
  currentRoom = roomCode;
  currentPlayer = playerId;

  if (connection?.state === HubConnectionState.Connected) {
    await rejoin();
    return;
  }
  if (connection) return; // در حال اتصال است

  // StrictMode ری‌اکت هر افکت را دوبار اجرا می‌کند. بدون این نگهبان، تلاشِ
  // اتصالِ اولِ (لغوشده) موقع شکست، متغیر مشترک را پاک می‌کند و اتصال دومِ
  // سالم را هم با خودش می‌برد. پس فقط وقتی state مشترک را دست می‌زنیم که
  // هنوز همان نمونه‌ی خودمان باشد.
  const conn = new HubConnectionBuilder()
    .withUrl(HUB_URL)
    .withAutomaticReconnect([0, 1000, 3000, 6000, 10000])
    .configureLogging(LogLevel.Warning)
    .build();
  connection = conn;

  conn.on("message", (msg: ChatMessage) => {
    messageHandlers.forEach((h) => h(msg));
  });

  conn.onreconnected(async () => {
    if (connection !== conn) return;
    emitStatus(true);
    await rejoin(); // بعد از قطعی، عضویت گروه از بین می‌رود و باید دوباره ساخته شود
  });
  conn.onreconnecting(() => { if (connection === conn) emitStatus(false); });
  conn.onclose(() => { if (connection === conn) emitStatus(false); });

  try {
    await conn.start();
    if (connection !== conn) { await conn.stop().catch(() => {}); return; }
    emitStatus(true);
    await rejoin();
  } catch {
    if (connection !== conn) return;   // نمونه‌ی ما دیگر فعال نیست — دست نزن
    emitStatus(false);
    connection = null;
  }
}

/** بعد از هر تغییر فاز یا مرگ صدا زده می‌شود تا سرور کانال را بازمحاسبه کند. */
export async function rejoin(): Promise<string | null> {
  if (!connection || connection.state !== HubConnectionState.Connected) return null;
  if (!currentRoom || currentPlayer === null) return null;
  try {
    return await connection.invoke<string | null>("Join", currentRoom, currentPlayer);
  } catch {
    return null;
  }
}

export async function sendChat(nickname: string, text: string): Promise<void> {
  if (!connection || connection.state !== HubConnectionState.Connected) {
    throw new Error("اتصال چت برقرار نیست");
  }
  if (!currentRoom || currentPlayer === null) return;
  await connection.invoke("Send", currentRoom, currentPlayer, nickname, text);
}

export async function disconnectChat(): Promise<void> {
  const c = connection;
  connection = null;
  currentRoom = null;
  currentPlayer = null;
  // handlerها را اینجا پاک نمی‌کنیم — صاحبشان store است و خودش unsubscribe
  // می‌کند. پاک کردن اینجا باعث می‌شد mount دوم StrictMode بدون شنونده بماند.
  await c?.stop().catch(() => {});
}
