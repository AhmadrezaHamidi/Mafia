/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** ریشه‌ی REST — در اپ موبایل آدرس کامل سرور، در وب مسیر نسبی */
  readonly VITE_API_BASE?: string;
  /** ریشه‌ی SignalR — همان منطق؛ در وب خالی می‌ماند */
  readonly VITE_HUB_BASE?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
