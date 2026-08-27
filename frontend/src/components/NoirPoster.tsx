// پیکره‌ی نوآر پس‌زمینه — بر اساس ترکیب‌بندی لوگوی بازی:
// کلاه لبه‌پهن، صورتِ خالی، یقه‌ی کت، پیراهن سفید، کراوات قرمز، سیگار.
//
// چرا وکتور و نه عکس: در هر اندازه تیز می‌ماند، چند کیلوبایت است، و رنگ‌هایش
// از همان پالت تم می‌آید. اگر mafia-logo.png در public/ باشد، AppBackground
// به‌جای این از عکس استفاده می‌کند.
//
// دو نکته‌ی مقیاس که با آزمایش روی صفحه‌ی واقعی معلوم شد:
//  ۱) viewBox از خودِ پیکره بلندتر است (۸۲۰ در برابر ~۵۲۰). این فضای خالی
//     باعث می‌شود در حالت meet پیکره کل ارتفاع صفحه را نگیرد و نسبت پوستری
//     بماند؛ بدون آن روی صفحه‌های بلند غول‌آسا می‌شد.
//  ۲) خطوط نازک‌اند (۲ به‌جای ۳٫۵) چون در بزرگ‌نمایی چند برابر می‌شوند و
//     ضخیم که شوند، تصویر به‌جای سیلوئت شبیه wireframe می‌شود.
//
// خوانایی متن روی این را scrim مشترکِ AppBackground می‌سازد، نه ماسک داخلی —
// وگرنه دو بار محو می‌شد و پیکره گم می‌شد.

export function NoirPoster() {
  return (
    <svg
      className="app-bg__layer app-bg__poster"
      viewBox="0 0 400 820"
      preserveAspectRatio="xMidYMin meet"
    >
      <defs>
        {/* نور پشت سر — شکل را همین می‌سازد، نه رنگ خودِ پیکره */}
        <radialGradient id="np-rim" cx="50%" cy="20%" r="42%">
          <stop offset="0%" stopColor="#FF2A44" stopOpacity="0.30" />
          <stop offset="55%" stopColor="#C8102E" stopOpacity="0.12" />
          <stop offset="100%" stopColor="#000" stopOpacity="0" />
        </radialGradient>

        {/* کت از بالا کمی روشن‌تر است تا از سیاهیِ زمینه جدا شود */}
        <linearGradient id="np-coat" x1="0" y1="0" x2="0" y2="1">
          <stop offset="0%" stopColor="#171013" />
          <stop offset="55%" stopColor="#0C080A" />
          <stop offset="100%" stopColor="#0A0709" />
        </linearGradient>

        {/* محوشدگی به سمت پایین: بخش شناختنی (کلاه، صورت، کراوات) می‌ماند و
            لبه‌های صافِ بلندِ کت حل می‌شوند — وگرنه مثل دو خط عمودیِ اتفاقی
            از وسط کارت‌ها رد می‌شدند. */}
        <linearGradient id="np-fade" x1="0" y1="0" x2="0" y2="1">
          <stop offset="0%" stopColor="#fff" stopOpacity="1" />
          <stop offset="30%" stopColor="#fff" stopOpacity="1" />
          <stop offset="78%" stopColor="#fff" stopOpacity="0" />
          <stop offset="100%" stopColor="#fff" stopOpacity="0" />
        </linearGradient>
        <mask id="np-mask">
          <rect width="400" height="820" fill="url(#np-fade)" />
        </mask>

        <linearGradient id="np-smoke" x1="0" y1="1" x2="0" y2="0">
          <stop offset="0%" stopColor="#EFE7E4" stopOpacity="0.28" />
          <stop offset="100%" stopColor="#EFE7E4" stopOpacity="0" />
        </linearGradient>
      </defs>

      <ellipse cx="200" cy="150" rx="185" ry="165" fill="url(#np-rim)" />

      <g mask="url(#np-mask)">
      <g stroke="#EFE7E4" strokeOpacity="0.5" strokeLinejoin="round" fill="url(#np-coat)">
        {/* ── کلاه ───────────────────────────────────────────────────── */}
        {/* لبه‌ی پهن با نوک‌های تیز — امضای بصری این سبک */}
        <path
          d="M86 156 q40 -22 114 -22 q74 0 114 22 q-10 20 -46 27 q-38 7 -68 7 q-30 0 -68 -7 q-36 -7 -46 -27 Z"
          strokeWidth="2"
        />
        {/* تاج با فرورفتگی وسط */}
        <path d="M132 150 q4 -66 68 -66 q64 0 68 66 q-34 -14 -68 -14 q-34 0 -68 14 Z" strokeWidth="2" />

        {/* ── شانه و کت؛ تا کف قاب ادامه دارد ────────────────────────── */}
        <path d="M96 300 q40 -44 104 -52 q64 8 104 52 l14 520 H82 Z" strokeWidth="2" />

        {/* برگردان‌های کت */}
        <path d="M176 244 l24 62 l-40 -26 Z" strokeWidth="1.6" />
        <path d="M224 244 l-24 62 l40 -26 Z" strokeWidth="1.6" />
      </g>

      {/* نوار دور کلاه */}
      <path d="M133 146 q34 -12 67 -12 q33 0 67 12 l0 8 q-34 -12 -67 -12 q-33 0 -67 12 Z" fill="#9C2B32" opacity="0.8" />

      {/* صورتِ خالی زیر کلاه — تیره‌تر از کت، تا حفره به نظر برسد */}
      <path d="M162 176 q38 12 76 0 l0 46 q-6 22 -38 22 q-32 0 -38 -22 Z" fill="#040203" />

      {/* پیراهن سفید — روشن‌ترین نقطه‌ی تصویر، نگاه را وسط نگه می‌دارد */}
      <path d="M176 244 q24 16 48 0 l10 20 l-34 42 l-34 -42 Z" fill="#EFE7E4" opacity="0.92" />
      {/* کراوات قرمز */}
      <path d="M193 262 h14 l-3 12 l9 44 l-13 16 l-13 -16 l9 -44 Z" fill="#C8102E" />

      {/* ── سیگار روی لب ───────────────────────────────────────────────
          کج است، نه افقی: افقی که بود دقیقاً هم‌تراز خط تیتر می‌افتاد و
          مثل خط‌خوردگی روی متن دیده می‌شد. چرخش حول انتهای دهان است. */}
      <g transform="rotate(16 208 214.5)">
        <rect x="208" y="212" width="30" height="5" rx="2.5" fill="#EFE7E4" opacity="0.7" />
        <circle cx="240" cy="214.5" r="4" fill="#FF5A3C" />
        <circle cx="240" cy="214.5" r="9" fill="#FF5A3C" opacity="0.22" />
      </g>

      {/* دود — تنها انیمیشن تصویر؛ از نوکِ چرخیده‌ی سیگار بلند می‌شود */}
      <g className="noir-smoke">
        <path d="M238 220 q16 -32 -2 -58 q-18 -26 2 -50" fill="none" stroke="url(#np-smoke)" strokeWidth="8" strokeLinecap="round" />
        <path d="M246 218 q22 -28 8 -54" fill="none" stroke="url(#np-smoke)" strokeWidth="5" strokeLinecap="round" opacity="0.6" />
      </g>
      </g>
    </svg>
  );
}
