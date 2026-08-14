import { useNavigate } from "react-router-dom";
import { useGameStore } from "../store/gameStore";

export function End() {
  const navigate = useNavigate();
  const players = useGameStore((s) => s.players);
  const winningTeam = useGameStore((s) => s.winningTeam);
  const requestRematch = useGameStore((s) => s.requestRematch);
  const leaveRoom = useGameStore((s) => s.leaveRoom);
  const me = useGameStore((s) => s.me());

  const isTownWin = winningTeam === "town";

  function handleExit() {
    leaveRoom();
    navigate("/");
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col items-center justify-center gap-6 p-6 text-center screen-enter">
      <div>
        <p className="mb-1 font-mono text-[0.68rem] tracking-[0.12em]" style={{ color: "var(--lamp)" }}>
          پایان بازی
        </p>
        <div
          className="text-3xl font-extrabold"
          style={{ color: isTownWin ? "var(--town)" : "var(--blood-bright)" }}
        >
          {isTownWin ? "شهر برد" : "مافیا برد"}
        </div>
        <p className="mx-auto mt-2 max-w-[22rem] text-sm" style={{ color: "var(--parchment-dim)" }}>
          {isTownWin ? "همه‌ی مافیاها شناسایی و حذف شدن." : "مافیا تونست شهر رو به تعداد مساوی برسونه."}
        </p>
      </div>

      <div className="grid w-full grid-cols-3 gap-2">
        {players.map((p) => {
          const isMafiaRole = p.role === "SimpleMafia";
          return (
            <div
              key={p.id}
              className={`rounded-lg border p-2 ${!p.alive ? "opacity-50" : ""}`}
              style={{ background: "var(--table)", borderColor: "var(--rule)" }}
            >
              <div
                className="mx-auto mb-1 flex h-9 w-9 items-center justify-center rounded-full border-2 text-sm font-bold"
                style={{ background: "var(--table-edge)", borderColor: "var(--rule)" }}
              >
                {p.name.charAt(0)}
              </div>
              <div className="truncate text-[0.66rem]" style={{ color: "var(--parchment-dim)" }}>
                {p.name}
              </div>
              <div
                className="text-[0.68rem] font-bold"
                style={{ color: isMafiaRole ? "var(--blood-bright)" : "var(--town)" }}
              >
                {isMafiaRole ? "مافیا" : "شهروند"}
              </div>
            </div>
          );
        })}
      </div>

      <div className="flex w-full flex-col gap-3">
        {me?.isHost && (
          <button
            onClick={requestRematch}
            className="rounded-lg px-5 py-3 font-bold transition active:scale-[0.98]"
            style={{ background: "var(--blood)", color: "var(--parchment)" }}
          >
            بازی دوباره
          </button>
        )}
        <button
          onClick={handleExit}
          className="rounded-lg border px-5 py-3 font-bold"
          style={{ borderColor: "var(--rule)", color: "var(--parchment)" }}
        >
          خروج به منو
        </button>
      </div>
    </div>
  );
}
