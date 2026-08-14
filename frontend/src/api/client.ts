// کلاینت پایه‌ی API — پوشش ApiResponse بک‌اند را باز می‌کند و خطا را یکدست می‌سازد.

const BASE = import.meta.env.VITE_API_BASE ?? "/api/v1";

/** شکل پاسخ بک‌اند: { success, message, data } */
interface ApiEnvelope<T> {
  success: boolean;
  message: string | null;
  data: T;
}

export class ApiError extends Error {
  status: number;

  constructor(message: string, status: number) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

interface RequestOptions {
  method?: "GET" | "POST" | "PUT" | "DELETE";
  body?: unknown;
  query?: Record<string, string | number | undefined>;
  /** برای long-polling — پیش‌فرض ۱۵ ثانیه */
  timeoutMs?: number;
  signal?: AbortSignal;
}

function buildUrl(path: string, query?: RequestOptions["query"]): string {
  const url = BASE + path;
  if (!query) return url;
  const qs = Object.entries(query)
    .filter(([, v]) => v !== undefined)
    .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`)
    .join("&");
  return qs ? `${url}?${qs}` : url;
}

export async function request<T>(path: string, opts: RequestOptions = {}): Promise<T> {
  const { method = "GET", body, query, timeoutMs = 15_000, signal } = opts;

  // تایم‌اوت خودمان، ولی اگر caller هم signal داده باشد هر دو محترم‌اند
  const ctrl = new AbortController();
  const timer = setTimeout(() => ctrl.abort(), timeoutMs);
  const onAbort = () => ctrl.abort();
  signal?.addEventListener("abort", onAbort);

  try {
    const res = await fetch(buildUrl(path, query), {
      method,
      headers: body ? { "Content-Type": "application/json" } : undefined,
      body: body ? JSON.stringify(body) : undefined,
      signal: ctrl.signal,
    });

    const text = await res.text();
    const parsed: unknown = text ? JSON.parse(text) : null;

    if (!res.ok) {
      const msg =
        (parsed as { message?: string; errors?: { exception?: string[] } })?.message ??
        (parsed as { errors?: { exception?: string[] } })?.errors?.exception?.[0] ??
        `خطای سرور (${res.status})`;
      throw new ApiError(msg, res.status);
    }

    // بعضی endpoint ها بدنه‌ی خام برمی‌گردانند، بقیه داخل ApiResponse
    const env = parsed as ApiEnvelope<T> | T;
    if (env && typeof env === "object" && "success" in env && "data" in env) {
      return (env as ApiEnvelope<T>).data;
    }
    return env as T;
  } catch (err) {
    if (err instanceof ApiError) throw err;
    if ((err as Error)?.name === "AbortError") {
      throw new ApiError("پاسخی از سرور دریافت نشد", 0);
    }
    throw new ApiError("ارتباط با سرور برقرار نشد", 0);
  } finally {
    clearTimeout(timer);
    signal?.removeEventListener("abort", onAbort);
  }
}
