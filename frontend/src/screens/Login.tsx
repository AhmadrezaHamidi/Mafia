// ورود با کد یک‌بارمصرف — دو مرحله: شماره، سپس کد.
//
// چرا دو مرحله و نه یک فرم: تا کد نیامده، فیلد کد فقط سر و صداست. و چون
// پاسخِ مرحله‌ی اول می‌گوید کاربر حساب دارد یا نه، در مرحله‌ی دوم فیلد
// «نام نمایشی» را فقط به تازه‌واردها نشان می‌دهیم.

import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { authApi } from "../api/auth";
import { ApiError } from "../api/client";
import { useAuthStore } from "../store/authStore";
import { OtpInput } from "../components/OtpInput";
import { LogoMark } from "../components/LogoMark";

type Step = "mobile" | "code";

/** ارقام فارسی/عربی → لاتین. کیبورد فارسی پیش‌فرض همان‌ها را می‌فرستد. */
function toLatinDigits(input: string): string {
  let out = "";
  for (const ch of input) {
    const code = ch.codePointAt(0)!;
    if (ch >= "0" && ch <= "9") out += ch;
    else if (code >= 0x06f0 && code <= 0x06f9) out += String(code - 0x06f0);
    else if (code >= 0x0660 && code <= 0x0669) out += String(code - 0x0660);
    else if (ch === "+") out += ch;
  }
  return out;
}

export function Login() {
  const navigate = useNavigate();
  const signIn = useAuthStore((s) => s.signIn);

  const [step, setStep] = useState<Step>("mobile");
  const [mobile, setMobile] = useState("");
  const [code, setCode] = useState("");
  const [displayName, setDisplayName] = useState("");
  const [isRegistered, setIsRegistered] = useState(true);
  const [demoCode, setDemoCode] = useState<string | null>(null);
  const [resendIn, setResendIn] = useState(0);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);

  const nameRef = useRef<HTMLInputElement>(null);
  // جلوی ارسال دوباره‌ی همان کد را می‌گیرد: تکمیل خودکار و کلیک دستی
  // می‌توانند هم‌زمان شلیک کنند.
  const submittedCode = useRef<string | null>(null);

  // شمارش معکوس ارسال دوباره
  useEffect(() => {
    if (resendIn <= 0) return;
    const t = setTimeout(() => setResendIn((s) => s - 1), 1000);
    return () => clearTimeout(t);
  }, [resendIn]);

  async function sendCode(target: string) {
    setError("");
    setBusy(true);
    try {
      const res = await authApi.requestOtp(target);
      setMobile(res.mobile);
      setIsRegistered(res.isRegistered);
      setDemoCode(res.demoCode);
      setResendIn(res.resendAfterSeconds);
      setCode("");
      submittedCode.current = null;
      setStep("code");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "ارتباط با سرور برقرار نشد");
    } finally {
      setBusy(false);
    }
  }

  async function verify(candidate: string) {
    if (submittedCode.current === candidate) return;
    submittedCode.current = candidate;

    if (!isRegistered && displayName.trim().length < 2) {
      setError("برای ساخت حساب، اسمت رو بنویس");
      nameRef.current?.focus();
      submittedCode.current = null;
      return;
    }

    setError("");
    setBusy(true);
    try {
      const res = await authApi.verifyOtp(mobile, candidate, isRegistered ? undefined : displayName);
      signIn(res);
      navigate("/", { replace: true });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "ارتباط با سرور برقرار نشد");
      setCode("");
      submittedCode.current = null;
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col items-center justify-center gap-6 p-6 text-center">
      <div>
        <p className="mb-1 font-mono text-[0.68rem] tracking-[0.12em]" style={{ color: "var(--lamp)" }}>
          بازی گروهی آنلاین
        </p>
        <LogoMark />
      </div>

      {step === "mobile" ? (
        <form
          className="flex w-full flex-col gap-3"
          onSubmit={(e) => { e.preventDefault(); void sendCode(mobile); }}
        >
          <h2 className="text-2xl">وارد شو</h2>
          <p className="-mt-2 text-sm" style={{ color: "var(--parchment-dim)" }}>
            شماره‌ت رو بده تا یه کد برات بفرستیم
          </p>

          <input
            value={mobile}
            onChange={(e) => setMobile(toLatinDigits(e.target.value))}
            dir="ltr"
            inputMode="tel"
            autoComplete="tel"
            autoFocus
            maxLength={13}
            placeholder="09123456789"
            aria-label="شماره موبایل"
            aria-invalid={error ? true : undefined}
            className="w-full rounded-xl border-2 px-4 py-3 text-center font-mono text-lg tracking-[0.1em] outline-none transition"
            style={{
              background: "var(--table)",
              borderColor: error ? "var(--blood-bright)" : "var(--rule)",
              color: "var(--parchment)",
            }}
          />

          <button
            type="submit"
            disabled={busy || mobile.length < 10}
            className="rounded-xl px-5 py-3 font-bold transition active:scale-[0.98] disabled:opacity-50"
            style={{ background: "var(--blood)", color: "var(--parchment)", minHeight: 52 }}
          >
            {busy ? "در حال ارسال..." : "ارسال کد"}
          </button>
        </form>
      ) : (
        <form
          className="flex w-full flex-col gap-3"
          onSubmit={(e) => { e.preventDefault(); void verify(code); }}
        >
          <h2 className="text-2xl">کد رو وارد کن</h2>
          <p className="-mt-2 text-sm" style={{ color: "var(--parchment-dim)" }}>
            کد به <span dir="ltr" className="font-mono">{mobile}</span> فرستاده شد
          </p>

          <OtpInput
            value={code}
            onChange={(v) => { setCode(v); if (error) setError(""); }}
            onComplete={(v) => { if (isRegistered || displayName.trim().length >= 2) void verify(v); }}
            disabled={busy}
            invalid={Boolean(error)}
          />

          {demoCode && (
            <button
              type="button"
              onClick={() => setCode(demoCode)}
              className="mx-auto flex items-center gap-2 rounded-lg border px-3 py-2 text-xs transition active:scale-[0.98]"
              style={{ borderColor: "var(--lamp)", color: "var(--lamp)" }}
            >
              🧪 نسخه‌ی آزمایشی — کد:
              <span dir="ltr" className="font-mono text-sm tracking-[0.15em]">{demoCode}</span>
              <span style={{ color: "var(--muted)" }}>(بزن تا پر بشه)</span>
            </button>
          )}

          {!isRegistered && (
            <div className="flex flex-col gap-1 text-right">
              <label htmlFor="display-name" className="text-sm" style={{ color: "var(--parchment-dim)" }}>
                اولین باره اومدی — اسمت رو بنویس
              </label>
              <input
                id="display-name"
                ref={nameRef}
                value={displayName}
                onChange={(e) => setDisplayName(e.target.value)}
                maxLength={20}
                placeholder="مثلاً احمد"
                className="w-full rounded-xl border-2 px-4 py-3 text-center outline-none transition"
                style={{ background: "var(--table)", borderColor: "var(--rule)", color: "var(--parchment)" }}
              />
            </div>
          )}

          <button
            type="submit"
            disabled={busy || code.length !== 6}
            className="rounded-xl px-5 py-3 font-bold transition active:scale-[0.98] disabled:opacity-50"
            style={{ background: "var(--blood)", color: "var(--parchment)", minHeight: 52 }}
          >
            {busy ? "..." : isRegistered ? "ورود" : "ساخت حساب و ورود"}
          </button>

          <div className="flex items-center justify-between text-xs">
            <button
              type="button"
              onClick={() => { setStep("mobile"); setCode(""); setError(""); }}
              className="underline underline-offset-4"
              style={{ color: "var(--parchment-dim)" }}
            >
              تغییر شماره
            </button>

            {resendIn > 0 ? (
              <span style={{ color: "var(--muted)" }}>
                ارسال دوباره تا {resendIn} ثانیه
              </span>
            ) : (
              <button
                type="button"
                onClick={() => void sendCode(mobile)}
                disabled={busy}
                className="underline underline-offset-4 disabled:opacity-50"
                style={{ color: "var(--lamp)" }}
              >
                ارسال دوباره‌ی کد
              </button>
            )}
          </div>
        </form>
      )}

      {/* aria-live تا صفحه‌خوان خطا را بخواند بدون اینکه فوکوس بپرد */}
      <p role="alert" aria-live="polite" className="min-h-[1.25rem] text-sm" style={{ color: "var(--blood-bright)" }}>
        {error}
      </p>
    </div>
  );
}
