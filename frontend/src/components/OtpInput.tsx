// ورودی کد ۶ رقمی — شش خانه‌ی جدا.
//
// چند تصمیم که از تجربه‌ی واقعی کاربر می‌آید، نه از ظاهر:
//
//  • کانتینر dir="ltr" است هرچند صفحه راست‌به‌چپ است. عدد ذاتاً چپ‌به‌راست
//    خوانده می‌شود؛ با rtl، رقم اولی که تایپ می‌کنی سمت راست می‌نشیند و
//    ترتیب با چیزی که در پیامک دیدی جابه‌جا می‌شود.
//
//  • autoComplete="one-time-code" روی خانه‌ی اول: iOS و اندروید کد را از
//    پیامک پیشنهاد می‌دهند. بدون این، کاربر باید بین دو اپ جابه‌جا شود.
//
//  • paste روی هر خانه‌ای کل کد را پخش می‌کند — کسی که کد را کپی کرده
//    انتظار ندارد اول روی خانه‌ی اول کلیک کند.
//
//  • ارقام فارسی هم پذیرفته می‌شود؛ کیبورد فارسی به‌طور پیش‌فرض همان‌ها را
//    می‌فرستد و رد کردنشان کاربر را بی‌دلیل گیر می‌اندازد.

import { useEffect, useRef } from "react";

const LENGTH = 6;

interface Props {
  value: string;
  onChange: (value: string) => void;
  /** وقتی هر ۶ رقم پر شد — برای ارسال خودکار */
  onComplete?: (value: string) => void;
  disabled?: boolean;
  invalid?: boolean;
}

/** ارقام فارسی/عربی → لاتین، و هر چیز دیگری دور ریخته می‌شود. */
function toLatinDigits(input: string): string {
  let out = "";
  for (const ch of input) {
    const code = ch.codePointAt(0)!;
    if (ch >= "0" && ch <= "9") out += ch;
    else if (code >= 0x06f0 && code <= 0x06f9) out += String(code - 0x06f0);
    else if (code >= 0x0660 && code <= 0x0669) out += String(code - 0x0660);
  }
  return out;
}

export function OtpInput({ value, onChange, onComplete, disabled, invalid }: Props) {
  const refs = useRef<(HTMLInputElement | null)[]>([]);
  const digits = value.padEnd(LENGTH, " ").slice(0, LENGTH).split("");

  useEffect(() => {
    if (value.length === LENGTH) onComplete?.(value);
    // onComplete عمداً در وابستگی‌ها نیست: اگر والد هر رندر تابع تازه بسازد،
    // این افکت بی‌دلیل دوباره اجرا و کد دوباره ارسال می‌شود.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  function setAt(index: number, digit: string) {
    const next = value.padEnd(LENGTH, " ").split("");
    next[index] = digit || " ";
    onChange(next.join("").replace(/ +$/, "").trimEnd());
  }

  function handleInput(index: number, raw: string) {
    const clean = toLatinDigits(raw);
    if (!clean) return;

    if (clean.length > 1) {
      // چند رقم یک‌جا (paste یا autofill) — از همین خانه به بعد پخش کن
      const merged = (value.slice(0, index) + clean).slice(0, LENGTH);
      onChange(merged);
      refs.current[Math.min(merged.length, LENGTH - 1)]?.focus();
      return;
    }

    setAt(index, clean);
    if (index < LENGTH - 1) refs.current[index + 1]?.focus();
  }

  function handleKeyDown(index: number, e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key === "Backspace") {
      e.preventDefault();
      if (digits[index].trim()) {
        setAt(index, "");
      } else if (index > 0) {
        // خانه‌ی خالی: یک قدم عقب برو و آن را پاک کن — رفتاری که کاربر انتظار دارد
        setAt(index - 1, "");
        refs.current[index - 1]?.focus();
      }
    } else if (e.key === "ArrowLeft" && index > 0) {
      refs.current[index - 1]?.focus();
    } else if (e.key === "ArrowRight" && index < LENGTH - 1) {
      refs.current[index + 1]?.focus();
    }
  }

  return (
    <div dir="ltr" className="flex justify-center gap-2" role="group" aria-label="کد ۶ رقمی">
      {digits.map((digit, i) => (
        <input
          key={i}
          ref={(el) => { refs.current[i] = el; }}
          value={digit.trim()}
          onChange={(e) => handleInput(i, e.target.value)}
          onKeyDown={(e) => handleKeyDown(i, e)}
          onFocus={(e) => e.target.select()}
          disabled={disabled}
          inputMode="numeric"
          autoComplete={i === 0 ? "one-time-code" : "off"}
          aria-label={`رقم ${i + 1}`}
          aria-invalid={invalid || undefined}
          className="h-14 w-11 rounded-xl border-2 text-center font-mono text-2xl outline-none transition disabled:opacity-50"
          style={{
            background: "var(--table)",
            borderColor: invalid ? "var(--blood-bright)" : digit.trim() ? "var(--lamp)" : "var(--rule)",
            color: "var(--parchment)",
          }}
        />
      ))}
    </div>
  );
}
