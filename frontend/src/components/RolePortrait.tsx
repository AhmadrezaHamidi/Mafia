// تصویرسازی SVG اختصاصی هر کاراکتر — به‌جای ایموجی، یه چهره‌ی برداری مخصوص خودشون.
// همه از یه پایه‌ی مشترک (صورت + بدن) ساخته می‌شن و فقط اکسسوری/رنگشون فرق می‌کنه،
// که هم هماهنگ به‌نظر بیان هم نگه‌داریشون ساده بمونه.

import type { ReactNode } from "react";
import type { Role } from "../types";

const palette = {
  skin: "#e8c9a0",
  skinShadow: "#c9a878",
  blood: "#9c2b32",
  bloodBright: "#d9495c",
  town: "#4f8f82",
  lamp: "#d4a54a",
  neutral: "#6b4e8e",
  dark: "#241d1a",
  parchment: "#f0e6d2",
};

function Base({ children, bg }: { children: ReactNode; bg: string }) {
  return (
    <svg viewBox="0 0 100 100" width="100%" height="100%" role="img" aria-hidden>
      <circle cx="50" cy="50" r="48" fill={bg} />
      <circle cx="50" cy="50" r="48" fill="none" stroke="rgba(0,0,0,0.25)" strokeWidth="2" />
      {/* بدن */}
      <path d="M20 92 Q50 68 80 92 L80 100 L20 100 Z" fill={palette.dark} />
      {/* گردن */}
      <rect x="43" y="55" width="14" height="14" fill={palette.skinShadow} />
      {/* صورت */}
      <circle cx="50" cy="42" r="20" fill={palette.skin} />
      {children}
    </svg>
  );
}

export function RolePortrait({ role, className }: { role: Role | string; className?: string }) {
  const content = (() => {
    switch (role) {
      case "SimpleCitizen":
        return (
          <Base bg="#3a4a52">
            <circle cx="43" cy="41" r="2" fill={palette.dark} />
            <circle cx="57" cy="41" r="2" fill={palette.dark} />
            <path d="M42 50 Q50 55 58 50" stroke={palette.dark} strokeWidth="2" fill="none" strokeLinecap="round" />
          </Base>
        );

      case "SimpleMafia":
      case "SimpleMafia-nights":
        return (
          <Base bg={palette.blood}>
            {/* عینک دودی */}
            <rect x="34" y="37" width="14" height="8" rx="2" fill={palette.dark} />
            <rect x="52" y="37" width="14" height="8" rx="2" fill={palette.dark} />
            <rect x="48" y="39" width="4" height="2" fill={palette.dark} />
            <path d="M42 53 Q50 50 58 53" stroke={palette.dark} strokeWidth="2" fill="none" strokeLinecap="round" />
            {/* کلاه فدورا */}
            <path d="M28 30 Q50 12 72 30 L68 33 Q50 22 32 33 Z" fill={palette.dark} />
            <rect x="26" y="29" width="48" height="5" rx="2" fill={palette.dark} />
          </Base>
        );

      case "GodFather":
        return (
          <Base bg="#5c1e22">
            {/* سبیل */}
            <path d="M40 51 Q50 56 60 51 Q50 49 40 51 Z" fill="#2b2320" />
            <circle cx="43" cy="41" r="2" fill={palette.dark} />
            <circle cx="57" cy="41" r="2" fill={palette.dark} />
            {/* کلاه‌شاپو */}
            <rect x="38" y="12" width="24" height="16" rx="1" fill={palette.dark} />
            <rect x="28" y="26" width="44" height="5" rx="2" fill={palette.dark} />
            <rect x="38" y="18" width="24" height="3" fill={palette.lamp} opacity="0.8" />
            {/* کراوات */}
            <path d="M47 68 L53 68 L56 82 L50 90 L44 82 Z" fill={palette.lamp} />
          </Base>
        );

      case "Doctor":
        return (
          <Base bg={palette.town}>
            <circle cx="43" cy="41" r="2" fill={palette.dark} />
            <circle cx="57" cy="41" r="2" fill={palette.dark} />
            <path d="M42 50 Q50 55 58 50" stroke={palette.dark} strokeWidth="2" fill="none" strokeLinecap="round" />
            {/* هدبند آینه‌ی دکتر */}
            <circle cx="50" cy="24" r="6" fill="none" stroke={palette.parchment} strokeWidth="2" />
            <path d="M30 20 Q50 10 70 20" stroke={palette.parchment} strokeWidth="3" fill="none" strokeLinecap="round" />
            {/* علامت صلیب پزشکی روی سینه */}
            <rect x="46" y="78" width="8" height="18" fill={palette.parchment} />
            <rect x="41" y="83" width="18" height="8" fill={palette.parchment} />
          </Base>
        );

      case "Detective":
        return (
          <Base bg={palette.lamp}>
            <circle cx="43" cy="41" r="2" fill={palette.dark} />
            <circle cx="57" cy="41" r="2" fill={palette.dark} />
            <path d="M42 51 Q50 47 58 51" stroke={palette.dark} strokeWidth="2" fill="none" strokeLinecap="round" />
            {/* کلاه شکارچی (دیرستاکر) */}
            <path d="M30 30 Q50 16 70 30 L66 34 Q50 24 34 34 Z" fill={palette.dark} />
            <path d="M40 16 L34 8 L46 14 Z" fill={palette.dark} />
            <path d="M60 16 L66 8 L54 14 Z" fill={palette.dark} />
            {/* ذره‌بین */}
            <circle cx="72" cy="66" r="9" fill="none" stroke={palette.dark} strokeWidth="3" />
            <line x1="78" y1="72" x2="86" y2="80" stroke={palette.dark} strokeWidth="4" strokeLinecap="round" />
          </Base>
        );

      case "Mayor":
        return (
          <Base bg={palette.town}>
            <circle cx="43" cy="41" r="2" fill={palette.dark} />
            <circle cx="57" cy="41" r="2" fill={palette.dark} />
            <path d="M42 50 Q50 55 58 50" stroke={palette.dark} strokeWidth="2" fill="none" strokeLinecap="round" />
            {/* تاج شهردار */}
            <path d="M32 25 L38 12 L46 22 L50 10 L54 22 L62 12 L68 25 Z" fill={palette.lamp} />
            <rect x="32" y="25" width="36" height="5" fill={palette.lamp} />
            {/* روبان روی سینه */}
            <path d="M50 68 L58 96 L50 90 L42 96 Z" fill={palette.lamp} />
          </Base>
        );

      case "Bodyguard":
        return (
          <Base bg="#3c5a6b">
            <circle cx="43" cy="41" r="2" fill={palette.dark} />
            <circle cx="57" cy="41" r="2" fill={palette.dark} />
            <path d="M42 51 Q50 47 58 51" stroke={palette.dark} strokeWidth="2" fill="none" strokeLinecap="round" />
            {/* عینک آفتابی محافظتی */}
            <rect x="35" y="38" width="30" height="6" rx="2" fill={palette.dark} />
            {/* سپر */}
            <path d="M50 62 L66 68 L66 82 Q50 96 34 82 L34 68 Z" fill={palette.parchment} />
            <path d="M50 68 L60 72 L60 80 Q50 90 40 80 L40 72 Z" fill={palette.town} />
          </Base>
        );

      case "SerialKiller":
        return (
          <Base bg={palette.neutral}>
            {/* ماسک هاکی */}
            <path d="M30 34 Q50 24 70 34 Q72 54 50 60 Q28 54 30 34 Z" fill={palette.parchment} />
            <path d="M40 40 L46 46 M46 40 L40 46" stroke={palette.dark} strokeWidth="2.5" strokeLinecap="round" />
            <path d="M54 40 L60 46 M60 40 L54 46" stroke={palette.dark} strokeWidth="2.5" strokeLinecap="round" />
            <path d="M44 52 Q50 55 56 52" stroke={palette.dark} strokeWidth="2" fill="none" strokeLinecap="round" />
            {/* چاقو */}
            <path d="M72 60 L84 48 L88 52 L76 64 Z" fill="#cfd6d8" />
            <rect x="70" y="60" width="8" height="4" fill={palette.dark} transform="rotate(45 74 62)" />
          </Base>
        );

      default:
        return (
          <Base bg="#4a4038">
            <text x="50" y="48" textAnchor="middle" fontSize="20" fill={palette.dark}>؟</text>
          </Base>
        );
    }
  })();

  return <div className={className}>{content}</div>;
}
