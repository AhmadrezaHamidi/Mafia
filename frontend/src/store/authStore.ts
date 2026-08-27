// هویت واردشده — تنها چیزی که بین بازی‌ها می‌ماند.
//
// localStorage و نه sessionStorage: برخلاف هویتِ درون‌روم (که عمداً به تب
// گره خورده تا دو تب دو بازیکن باشند)، حساب کاربر همان آدم است در هر تب،
// و انتظار دارد دفعه‌ی بعد لازم نباشد دوباره وارد شود.

import { create } from "zustand";
import type { VerifyOtpResult } from "../api/auth";

export interface Account {
  playerId: number;
  mobile: string;
  displayName: string;
  token: string;
}

const KEY = "mafia.account";

function load(): Account | null {
  try {
    const raw = localStorage.getItem(KEY);
    return raw ? (JSON.parse(raw) as Account) : null;
  } catch {
    return null; // حالت خصوصی مرورگر یا داده‌ی خراب
  }
}

function save(account: Account | null) {
  try {
    if (account) localStorage.setItem(KEY, JSON.stringify(account));
    else localStorage.removeItem(KEY);
  } catch { /* حالت خصوصی مرورگر */ }
}

interface AuthState {
  account: Account | null;
  signIn: (result: VerifyOtpResult) => void;
  signOut: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  account: load(),

  signIn: (result) => {
    const account: Account = {
      playerId: result.playerId,
      mobile: result.mobile,
      displayName: result.displayName,
      token: result.token,
    };
    save(account);
    set({ account });
  },

  signOut: () => {
    save(null);
    set({ account: null });
  },
}));
