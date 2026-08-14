import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useGameStore } from "../store/gameStore";

export function Landing() {
  const navigate = useNavigate();
  const createRoom = useGameStore((s) => s.createRoom);
  const joinRoom = useGameStore((s) => s.joinRoom);
  const [nickname, setNickname] = useState("");
  const [capacity, setCapacity] = useState(8);
  const [code, setCode] = useState("");
  const [error, setError] = useState("");

  function handleCreate() {
    const name = nickname.trim();
    if (name.length < 2) {
      setError("اسمت باید حداقل ۲ حرف باشه");
      return;
    }
    createRoom(name, capacity);
    const roomCode = useGameStore.getState().roomCode!;
    navigate(`/room/${roomCode}`);
  }

  function handleJoin() {
    const name = nickname.trim();
    if (name.length < 2) {
      setError("اسمت باید حداقل ۲ حرف باشه");
      return;
    }
    if (code.trim().length < 4) {
      setError("کد روم رو کامل وارد کن");
      return;
    }
    joinRoom(code.trim(), name);
    navigate(`/room/${code.trim().toUpperCase()}`);
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col items-center justify-center gap-8 p-6 text-center">
      <div>
        <p className="mb-1 font-mono text-[0.68rem] tracking-[0.12em]" style={{ color: "var(--lamp)" }}>
          بازی گروهی آنلاین
        </p>
        <h1 className="text-4xl font-extrabold">مافیا</h1>
        <p className="mx-auto mt-2 max-w-[22rem] text-sm" style={{ color: "var(--parchment-dim)" }}>
          یه میز، یه چراغ، و یه شهر که نمی‌دونه کی بینشون قایم شده.
        </p>
      </div>

      <div className="flex w-full flex-col gap-3">
        <input
          value={nickname}
          onChange={(e) => setNickname(e.target.value)}
          maxLength={20}
          placeholder="اسمت رو بنویس"
          className="rounded-lg border px-4 py-3 text-center outline-none"
          style={{ background: "var(--table)", borderColor: "var(--rule)", color: "var(--parchment)" }}
        />

        <div className="flex flex-col gap-1.5">
          <span className="text-sm" style={{ color: "var(--parchment-dim)" }}>
            ظرفیت روم
          </span>
          <div
            role="radiogroup"
            aria-label="ظرفیت روم"
            className="grid grid-cols-4 gap-1.5 rounded-lg border p-1"
            style={{ borderColor: "var(--rule)", background: "var(--table)" }}
          >
            {[6, 8, 10, 12].map((n) => {
              const active = capacity === n;
              return (
                <button
                  key={n}
                  type="button"
                  role="radio"
                  aria-checked={active}
                  onClick={() => setCapacity(n)}
                  className="rounded-md py-2 text-sm font-bold transition"
                  style={
                    active
                      ? { background: "var(--blood)", color: "var(--parchment)" }
                      : { background: "transparent", color: "var(--parchment-dim)" }
                  }
                >
                  {n}
                </button>
              );
            })}
          </div>
        </div>

        <button
          onClick={handleCreate}
          className="rounded-lg px-5 py-3 font-bold transition active:scale-[0.98]"
          style={{ background: "var(--blood)", color: "var(--parchment)" }}
        >
          ساخت روم جدید
        </button>

        <div
          className="flex items-center gap-2 rounded-lg border p-1"
          style={{ background: "var(--table)", borderColor: "var(--rule)" }}
        >
          <input
            value={code}
            onChange={(e) => setCode(e.target.value.toUpperCase())}
            maxLength={6}
            placeholder="کد روم"
            className="flex-1 bg-transparent px-2 py-2 text-center font-mono text-lg tracking-[0.3em] uppercase outline-none"
            style={{ color: "var(--parchment)" }}
          />
          <button
            onClick={handleJoin}
            className="rounded-md border px-4 py-2 font-bold"
            style={{ borderColor: "var(--rule)", color: "var(--parchment)" }}
          >
            ورود
          </button>
        </div>

        {error && (
          <p className="text-sm" style={{ color: "var(--blood-bright)" }} role="alert">
            {error}
          </p>
        )}
      </div>
    </div>
  );
}
