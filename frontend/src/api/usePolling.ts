// هم‌گام‌سازی وضعیت با polling.
//
// طبق تصمیم معماری: به‌جای WebSocket برای state بازی، درخواست با timeout بلند
// می‌زنیم. فاصله‌ی بین درخواست‌ها adaptive است — نزدیک پایان فاز تندتر می‌شود
// تا تغییر فاز دیر دیده نشود، و وقتی تب مخفی است کند می‌شود تا سرور (۱ هسته‌ای)
// بی‌خود بار نگیرد.

import { useEffect, useRef, useState } from "react";

interface Options {
  /** فاصله‌ی معمول بین دو درخواست */
  intervalMs?: number;
  /** وقتی کمتر از این ثانیه تا پایان فاز مانده، تندتر بپرس */
  urgentBelowSeconds?: number;
  urgentIntervalMs?: number;
  enabled?: boolean;
}

interface Result<T> {
  data: T | null;
  error: string | null;
  /** برای وقتی بعد از یک اکشن می‌خواهیم فوراً تازه‌سازی شود */
  refresh: () => void;
}

export function usePolling<T extends { timeLeftSeconds?: number }>(
  fetcher: (signal: AbortSignal) => Promise<T>,
  deps: unknown[],
  opts: Options = {},
): Result<T> {
  const {
    intervalMs = 2500,
    urgentBelowSeconds = 5,
    urgentIntervalMs = 800,
    enabled = true,
  } = opts;

  const [data, setData] = useState<T | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [tick, setTick] = useState(0);

  // در ref نگه می‌داریم تا تغییرش باعث ساخت دوباره‌ی افکت نشود
  const fetcherRef = useRef(fetcher);
  fetcherRef.current = fetcher;

  useEffect(() => {
    if (!enabled) return;

    let stopped = false;
    let timer: ReturnType<typeof setTimeout>;
    const ctrl = new AbortController();

    const loop = async () => {
      try {
        const result = await fetcherRef.current(ctrl.signal);
        if (stopped) return;
        setData(result);
        setError(null);

        const left = result?.timeLeftSeconds;
        const hidden = typeof document !== "undefined" && document.hidden;
        const wait = hidden
          ? 8000
          : typeof left === "number" && left <= urgentBelowSeconds
            ? urgentIntervalMs
            : intervalMs;

        timer = setTimeout(loop, wait);
      } catch (err) {
        if (stopped) return;
        setError((err as Error).message);
        // قطعی گذرا نباید polling را بکشد — کمی صبر و دوباره
        timer = setTimeout(loop, 4000);
      }
    };

    loop();
    return () => {
      stopped = true;
      ctrl.abort();
      clearTimeout(timer);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...deps, enabled, tick]);

  return { data, error, refresh: () => setTick((t) => t + 1) };
}
