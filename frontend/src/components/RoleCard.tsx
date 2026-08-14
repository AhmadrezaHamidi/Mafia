import type { Role } from "../types";

export function RoleCard({ role, hint }: { role: Role; hint: string }) {
  const isMafia = role === "SimpleMafia";
  return (
    <div
      className="mt-auto flex items-center gap-3 rounded-[10px] border p-4"
      style={{ background: "var(--table)", borderColor: "var(--rule)" }}
    >
      <span
        className="flex-none rounded-full px-2 py-1 font-mono text-[0.65rem] tracking-wide"
        style={
          isMafia
            ? { background: "rgba(156,43,50,0.2)", color: "var(--blood-bright)" }
            : { background: "rgba(79,143,130,0.2)", color: "var(--town)" }
        }
      >
        {isMafia ? "مافیا" : "شهروند"}
      </span>
      <p className="m-0 text-sm" style={{ color: "var(--parchment-dim)" }}>
        {hint}
      </p>
    </div>
  );
}
