// پیکره‌ی نوآر پس‌زمینه — بر اساس ترکیب‌بندی لوگوی بازی:
// کلاه لبه‌پهن، صورتِ خالی، یقه‌ی کت، پیراهن سفید، کراوات قرمز.
//
// چرا وکتور و نه عکس: در هر اندازه تیز می‌ماند، چند کیلوبایت است، و رنگ‌هایش
// از همان توکن‌های تم می‌آید — پس اگر تم عوض شود، خودش هم عوض می‌شود.
//
// اگر فایل mafia-logo.png در public/ باشد، LogoMark به‌جای این استفاده می‌شود.

export function NoirPoster() {
  return (
    <div aria-hidden="true" className="noir-poster">
      <svg viewBox="0 0 400 520" preserveAspectRatio="xMidYMax slice">
        <defs>
          {/* از پایین محو می‌شود تا متن روی آن خوانا بماند */}
          <linearGradient id="np-fade" x1="0" y1="1" x2="0" y2="0">
            <stop offset="0%" stopColor="#000" stopOpacity="1" />
            <stop offset="60%" stopColor="#000" stopOpacity="0.5" />
            <stop offset="100%" stopColor="#000" stopOpacity="0" />
          </linearGradient>
          <mask id="np-mask">
            <rect width="400" height="520" fill="url(#np-fade)" />
          </mask>

          {/* نور پشت سر — شکل را همین می‌سازد، نه رنگ خودِ پیکره */}
          <radialGradient id="np-rim" cx="50%" cy="32%" r="46%">
            <stop offset="0%" stopColor="#FF2A44" stopOpacity="0.32" />
            <stop offset="50%" stopColor="#C8102E" stopOpacity="0.14" />
            <stop offset="100%" stopColor="#000" stopOpacity="0" />
          </radialGradient>

          <linearGradient id="np-smoke" x1="0" y1="1" x2="0" y2="0">
            <stop offset="0%" stopColor="#EFE7E4" stopOpacity="0.26" />
            <stop offset="100%" stopColor="#EFE7E4" stopOpacity="0" />
          </linearGradient>
        </defs>

        <ellipse cx="200" cy="170" rx="185" ry="170" fill="url(#np-rim)" />

        <g mask="url(#np-mask)">
          {/* ── کلاه ───────────────────────────────────────────────── */}
          {/* لبه‌ی پهن با نوک‌های تیز — امضای بصری این سبک */}
          <path
            d="M86 156 q40 -22 114 -22 q74 0 114 22 q-10 20 -46 27 q-38 7 -68 7 q-30 0 -68 -7 q-36 -7 -46 -27 Z"
            fill="#0A0709" stroke="#EFE7E4" strokeWidth="3.5" strokeLinejoin="round"
          />
          {/* تاج با فرورفتگی وسط */}
          <path
            d="M132 150 q4 -66 68 -66 q64 0 68 66 q-34 -14 -68 -14 q-34 0 -68 14 Z"
            fill="#0A0709" stroke="#EFE7E4" strokeWidth="3.5" strokeLinejoin="round"
          />
          {/* نوار دور کلاه */}
          <path d="M133 146 q34 -12 67 -12 q33 0 67 12 l0 8 q-34 -12 -67 -12 q-33 0 -67 12 Z" fill="#9C2B32" opacity="0.85" />

          {/* ── صورتِ خالی و گردن ──────────────────────────────────── */}
          <path d="M162 176 q38 12 76 0 l0 46 q-6 22 -38 22 q-32 0 -38 -22 Z" fill="#050304" />

          {/* ── شانه و کت ──────────────────────────────────────────── */}
          <path
            d="M96 300 q40 -44 104 -52 q64 8 104 52 l14 220 H82 Z"
            fill="#0A0709" stroke="#EFE7E4" strokeWidth="3.5" strokeLinejoin="round"
          />
          {/* پیراهن سفید */}
          <path d="M176 244 q24 16 48 0 l10 20 l-34 42 l-34 -42 Z" fill="#EFE7E4" />
          {/* برگردان‌های کت */}
          <path d="M176 244 l24 62 l-40 -26 Z" fill="#0A0709" stroke="#EFE7E4" strokeWidth="3" strokeLinejoin="round" />
          <path d="M224 244 l-24 62 l40 -26 Z" fill="#0A0709" stroke="#EFE7E4" strokeWidth="3" strokeLinejoin="round" />
          {/* کراوات قرمز */}
          <path d="M193 262 h14 l-3 12 l9 44 l-13 16 l-13 -16 l9 -44 Z" fill="#C8102E" />
          {/* خطوط قرمز کناری — لهجه‌ی رنگی همان سبک */}
          <path d="M112 296 l10 -10 l6 150 l-12 0 Z" fill="#9C2B32" opacity="0.75" />
          <path d="M288 296 l-10 -10 l-6 150 l12 0 Z" fill="#9C2B32" opacity="0.75" />

          {/* ── سیگار روی لب ───────────────────────────────────────── */}
          <rect x="208" y="212" width="32" height="6" rx="3" fill="#EFE7E4" opacity="0.9" />
          <circle cx="242" cy="215" r="4.5" fill="#FF5A3C" />
          <circle cx="242" cy="215" r="10" fill="#FF5A3C" opacity="0.2" />
        </g>

        {/* دود — تنها انیمیشن تصویر */}
        <g mask="url(#np-mask)" className="noir-smoke">
          <path
            d="M244 208 q16 -32 -2 -58 q-18 -26 2 -50"
            fill="none" stroke="url(#np-smoke)" strokeWidth="9" strokeLinecap="round"
          />
          <path
            d="M252 206 q22 -28 8 -54"
            fill="none" stroke="url(#np-smoke)" strokeWidth="6" strokeLinecap="round" opacity="0.65"
          />
        </g>
      </svg>
    </div>
  );
}
