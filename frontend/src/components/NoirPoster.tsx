// سیلوئت نوآر پس‌زمینه‌ی صفحه‌ی ورود.
//
// چرا سیلوئت و نه عکس: صورتِ مشخص، بازیکن را به یک شخصیت خاص گره می‌زند،
// در حالی که مافیا بازیِ «هرکسی ممکنه اون باشه» است. سایه‌ی بی‌چهره دقیقاً
// همان حس را می‌دهد و ضمناً چند کیلوبایت است نه چند مگابایت.

export function NoirPoster() {
  return (
    <div aria-hidden="true" className="noir-poster">
      <svg viewBox="0 0 400 560" preserveAspectRatio="xMidYMax slice">
        <defs>
          {/* از پایین به بالا محو می‌شود تا متن روی آن خوانا بماند */}
          <linearGradient id="fadeUp" x1="0" y1="1" x2="0" y2="0">
            <stop offset="0%"   stopColor="#000" stopOpacity="1" />
            <stop offset="55%"  stopColor="#000" stopOpacity="0.55" />
            <stop offset="100%" stopColor="#000" stopOpacity="0" />
          </linearGradient>
          <mask id="softMask">
            <rect width="400" height="560" fill="url(#fadeUp)" />
          </mask>

          {/* نور قرمزِ پشت سر — منبع نور صحنه */}
          {/* نور پشت سر، ملایم — باید شکل را بسازد نه صفحه را قرمز کند */}
          <radialGradient id="rimLight" cx="50%" cy="34%" r="46%">
            <stop offset="0%"   stopColor="#FF2A44" stopOpacity="0.34" />
            <stop offset="45%"  stopColor="#C8102E" stopOpacity="0.16" />
            <stop offset="100%" stopColor="#000000" stopOpacity="0" />
          </radialGradient>

          <linearGradient id="smokeFade" x1="0" y1="1" x2="0" y2="0">
            <stop offset="0%"   stopColor="#E9DCD6" stopOpacity="0.30" />
            <stop offset="100%" stopColor="#E9DCD6" stopOpacity="0" />
          </linearGradient>
        </defs>

        {/* هاله‌ی قرمز پشت شخصیت */}
        <ellipse cx="200" cy="165" rx="200" ry="185" fill="url(#rimLight)" />

        <g mask="url(#softMask)" fill="#000000">
          {/* لبه‌ی کلاه */}
          <path d="M104 150 q96 -30 192 0 q-14 17 -96 20 q-82 -3 -96 -20 Z" />
          {/* تاج کلاه با فرورفتگی وسط */}
          <path d="M140 150 q6 -62 60 -62 q54 0 60 62 q-30 -12 -60 -12 q-30 0 -60 12 Z" />
          {/* نوار دور کلاه */}
          <rect x="138" y="140" width="124" height="11" opacity="0.55" />

          {/* سر و گردن */}
          <path d="M154 168 q46 16 92 0 l4 62 q-8 26 -50 26 q-42 0 -50 -26 Z" />
          {/* یقه و شانه‌ها */}
          <path d="M108 300 q42 -34 92 -34 q50 0 92 34 l16 260 H92 Z" />
          {/* برگردان کت */}
          <path d="M176 274 l24 44 l24 -44 l30 20 l-54 62 l-54 -62 Z" opacity="0.45" />
          {/* کراوات */}
          <path d="M196 288 l8 0 l6 18 l-10 46 l-10 -46 Z" opacity="0.7" />
        </g>

        {/* سیگار روی لب — تنها عنصر روشن صورت */}
        <g mask="url(#softMask)">
          <rect x="212" y="232" width="34" height="6" rx="3" fill="#E9DCD6" opacity="0.85" />
          {/* خاکستر روشن نوک سیگار */}
          <circle cx="248" cy="235" r="4.5" fill="#FF5A3C" opacity="0.95" />
          <circle cx="248" cy="235" r="9" fill="#FF5A3C" opacity="0.22" />
        </g>

        {/* دود — حرکت آرام و بی‌پایان، تنها انیمیشن این تصویر */}
        <g mask="url(#softMask)" className="noir-smoke">
          <path
            d="M250 228 q16 -34 -2 -60 q-18 -26 2 -52 q14 -18 4 -34"
            fill="none" stroke="url(#smokeFade)" strokeWidth="9" strokeLinecap="round"
          />
          <path
            d="M258 226 q22 -30 8 -56 q-14 -26 6 -46"
            fill="none" stroke="url(#smokeFade)" strokeWidth="6" strokeLinecap="round" opacity="0.7"
          />
        </g>

        {/* تفنگ در دست، رو به پایین — تهدید هست ولی نشانه گرفته نشده */}
        <g mask="url(#softMask)" fill="#0A0709">
          <path d="M286 372 l40 0 l0 15 l-11 0 l-2 26 q-2 9 -10 9 q-8 0 -9 -9 l-2 -26 l-6 0 Z" />
          <rect x="286" y="366" width="30" height="8" rx="3" opacity="0.85" />
        </g>
      </svg>
    </div>
  );
}
