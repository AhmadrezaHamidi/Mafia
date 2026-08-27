// احراز هویت با کد یک‌بارمصرف روی موبایل.

import { request } from "./client";

export interface RequestOtpResult {
  /** شماره‌ی نرمال‌شده که سرور شناخت — همین را برای verify بفرست */
  mobile: string;
  expiresInSeconds: number;
  resendAfterSeconds: number;
  /** حساب دارد؟ اگر نه، در مرحله‌ی بعد باید نام نمایشی بگیریم */
  isRegistered: boolean;
  /** فقط در نسخه‌ی آزمایشی پر است (درگاه پیامکی وصل نیست) */
  demoCode: string | null;
}

export interface VerifyOtpResult {
  playerId: number;
  mobile: string;
  displayName: string;
  isNewAccount: boolean;
  token: string;
}

export const authApi = {
  requestOtp: (mobile: string) =>
    request<RequestOtpResult>("/Auth/Otp/Request", { method: "POST", body: { mobile } }),

  verifyOtp: (mobile: string, code: string, displayName?: string) =>
    request<VerifyOtpResult>("/Auth/Otp/Verify", {
      method: "POST",
      body: { mobile, code, displayName: displayName?.trim() || null },
    }),
};
