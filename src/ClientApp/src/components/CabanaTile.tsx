import type { Tile } from "../hooks/useMap";
import type { CabanaStatus } from "../hooks/useCabanas";
import { Tooltip } from "./Tooltip";

interface CabanaTileProps {
  tile: Tile;
  status: CabanaStatus | undefined;
  onClick: () => void;
}

export function CabanaTile({ tile, status, onClick }: CabanaTileProps) {
  const isAvailable = status === "available";
  const hasStatus = status != null;

  const tooltipText = !hasStatus
    ? `Cabana ${tile.cabanaId} • Status unknown`
    : `Cabana ${tile.cabanaId} • ${isAvailable ? "Available" : "Booked"}`;

  const borderClass = !hasStatus
    ? "border-(--color-unknown) border-dashed"
    : isAvailable
      ? "border-(--color-available)"
      : "border-(--color-booked)";

  const interactionClass = isAvailable
    ? "hover:z-10 hover:-translate-y-0.5 hover:scale-105 hover:shadow-xl focus:z-20"
    : "cursor-not-allowed";

  const opacityClass = !hasStatus ? "opacity-35" : !isAvailable ? "opacity-50 hover:opacity-65" : "";

  return (
    <Tooltip content={tooltipText}>
      <button
        className={`relative flex w-full aspect-square cursor-pointer items-center justify-center overflow-hidden border-2 p-0 transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-(--color-accent) focus-visible:ring-offset-1 focus-visible:z-20 ${borderClass} ${interactionClass} ${opacityClass}`}
        onClick={isAvailable ? onClick : undefined}
        aria-label={
          !hasStatus
            ? `Cabana ${tile.cabanaId}, status unknown – not available for booking`
            : `Cabana ${tile.cabanaId}, ${isAvailable ? "available. Press Enter to book" : "already booked"}`
        }
        aria-disabled={!isAvailable}
        style={{
          backgroundImage: "url(/assets/parchmentBasic.png)",
          backgroundSize: "cover",
        }}
      >
      <img
        src="/assets/cabana.png"
        alt=""
        className="pointer-events-none block size-full object-contain"
        draggable={false}
      />
      {!isAvailable && hasStatus && (
        <div className="pointer-events-none absolute inset-0 bg-(--color-booked)/20" />
      )}
      {!hasStatus && (
        <div className="pointer-events-none absolute inset-0 bg-(--color-unknown)/15" />
      )}
      </button>
    </Tooltip>
  );
}
