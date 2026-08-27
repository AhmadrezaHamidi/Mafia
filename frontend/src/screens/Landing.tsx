import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useGameStore } from "../store/gameStore";
import { RolesGallery } from "../components/RolesGallery";
import { NoirPoster } from "../components/NoirPoster";
import { LogoMark } from "../components/LogoMark";
import { scenarios } from "../data/roles";
import type { Scenario } from "../types";

type Mode = "quick" | "private";

export function Landing() {
  const navigate = useNavigate();
  const createRoom = useGameStore((s) => s.createRoom);
  const joinRoom = useGameStore((s) => s.joinRoom);
  const quickJoin = useGameStore((s) => s.quickJoin);
  const [nickname, setNickname] = useState("");
  const [capacity, setCapacity] = useState(8);
  const [scenario, setScenario] = useState<Scenario>("RussianMafia");
  const [code, setCode] = useState("");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState<Mode | null>(null);
  const [rolesOpen, setRolesOpen] = useState(false);

  function requireNickname() {
    const name = nickname.trim();
    if (name.length < 2) {
      setError("اسمت باید حداقل ۲ حرف باشه");
      return null;
    }
    return name;
  }

  async function handleQuickJoin() {
    const name = requireNickname();
    if (!name) return;
    setError("");
    setBusy("quick");
    try {
      const roomCode = await quickJoin(name);
      navigate(`/room/${roomCode}`);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(null);
    }
  }

  async function handleCreatePrivate() {
    const name = requireNickname();
    if (!name) return;
    setError("");
    setBusy("private");
    try {
      const roomCode = await createRoom(name, capacity, scenario);
      navigate(`/room/${roomCode}`);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(null);
    }
  }

  async function handleJoinWithCode() {
    const name = requireNickname();
    if (!name) return;
    if (code.trim().length !== 6) {
      setError("کد روم باید ۶ کاراکتر باشه");
      return;
    }
    setError("");
    setBusy("private");
    try {
      const roomCode = await joinRoom(code.trim(), name);
      navigate(`/room/${roomCode}`);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(null);
    }
  }

  const mafiaApprox = Math.max(1, Math.round(capacity / 4));

  return (
    <div className="relative mx-auto flex min-h-screen max-w-md flex-col items-center justify-center gap-6 p-6 text-center">
      <NoirPoster />
      <div>
        <p className="mb-1 font-mono text-[0.68rem] tracking-[0.12em]" style={{ color: "var(--lamp)" }}>
          بازی گروهی آنلاین
        </p>
        <LogoMark />
        <p className="mx-auto mt-2 max-w-[22rem] text-sm" style={{ color: "var(--parchment-dim)" }}>
          یه میز، یه چراغ، و یه شهر که نمی‌دونه کی بینشون قایم شده.
        </p>
      </div>

      <input
        value={nickname}
        onChange={(e) => setNickname(e.target.value)}
        maxLength={20}
        placeholder="اسمت رو بنویس"
        className="w-full rounded-lg border px-4 py-3 text-center outline-none"
        style={{ background: "var(--table)", borderColor: "var(--rule)", color: "var(--parchment)" }}
      />

      <div className="flex w-full flex-col gap-3">
        {/* ── بازی سریع (عمومی) ─────────────────────────────────────────── */}
        <button
          onClick={handleQuickJoin}
          disabled={busy !== null}
          className="rounded-lg px-5 py-3 font-bold transition active:scale-[0.98] disabled:opacity-60"
          style={{ background: "var(--blood)", color: "var(--parchment)" }}
        >
          {busy === "quick" ? "در حال اتصال..." : "🎲 بازی سریع"}
        </button>
        <p className="-mt-1 text-xs" style={{ color: "var(--muted)" }}>
          به یه بازی عمومی وصل می‌شی که منتظر پر شدنه — بدون نیاز به کد
        </p>

        <div className="my-1 flex items-center gap-2" aria-hidden>
          <div className="h-px flex-1" style={{ background: "var(--rule)" }} />
          <span className="text-xs" style={{ color: "var(--muted)" }}>یا</span>
          <div className="h-px flex-1" style={{ background: "var(--rule)" }} />
        </div>

        {/* ── ساخت بازی خصوصی ───────────────────────────────────────────── */}
        <div className="flex flex-col gap-2 rounded-lg border p-3" style={{ borderColor: "var(--rule)" }}>
          <div className="flex items-center justify-between">
            <span className="text-sm font-bold">ساخت بازی خصوصی</span>
            <span className="font-mono text-xs" style={{ color: "var(--parchment-dim)" }}>
              {capacity} نفر · ~{mafiaApprox} مافیا
            </span>
          </div>
          <input
            type="range"
            min={6}
            max={15}
            value={capacity}
            onChange={(e) => setCapacity(Number(e.target.value))}
            aria-label="ظرفیت روم"
            className="w-full"
            style={{ accentColor: "var(--blood)" }}
          />

          <div className="grid grid-cols-2 gap-2">
            {scenarios.map((s) => (
              <button
                key={s.key}
                type="button"
                onClick={() => setScenario(s.key)}
                className="rounded-lg border px-2 py-2 text-xs font-bold transition"
                style={
                  scenario === s.key
                    ? { background: "var(--blood)", borderColor: "var(--blood)", color: "var(--parchment)" }
                    : { background: "var(--table)", borderColor: "var(--rule)", color: "var(--parchment-dim)" }
                }
                title={s.description}
              >
                {s.name}
              </button>
            ))}
          </div>
          <p className="-mt-1 text-xs" style={{ color: "var(--muted)" }}>
            {scenarios.find((s) => s.key === scenario)?.description}
          </p>

          <button
            onClick={handleCreatePrivate}
            disabled={busy !== null}
            className="rounded-lg border px-5 py-2.5 font-bold transition active:scale-[0.98] disabled:opacity-60"
            style={{ borderColor: "var(--lamp)", color: "var(--lamp)" }}
          >
            {busy === "private" ? "..." : "🔒 ساخت روم و گرفتن لینک دعوت"}
          </button>
        </div>

        <div
          className="flex items-center gap-2 rounded-lg border p-1"
          style={{ background: "var(--table)", borderColor: "var(--rule)" }}
        >
          <input
            value={code}
            onChange={(e) => setCode(e.target.value.toUpperCase())}
            maxLength={6}
            placeholder="کد روم رو داری؟"
            className="flex-1 bg-transparent px-2 py-2 text-center font-mono text-lg tracking-[0.3em] uppercase outline-none"
            style={{ color: "var(--parchment)" }}
          />
          <button
            onClick={handleJoinWithCode}
            disabled={busy !== null}
            className="rounded-md border px-4 py-2 font-bold disabled:opacity-60"
            style={{ borderColor: "var(--rule)", color: "var(--parchment)" }}
          >
            ورود
          </button>
        </div>

        <button
          onClick={() => setRolesOpen(true)}
          className="text-sm underline underline-offset-4"
          style={{ color: "var(--parchment-dim)" }}
        >
          نقش‌های بازی رو ببین
        </button>

        {error && (
          <p className="text-sm" style={{ color: "var(--blood-bright)" }} role="alert">
            {error}
          </p>
        )}
      </div>

      <RolesGallery open={rolesOpen} onClose={() => setRolesOpen(false)} initialScenario={scenario} />
    </div>
  );
}
