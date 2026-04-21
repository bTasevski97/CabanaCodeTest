import type { Tile } from "../hooks/useMap";
import type { CabanaStatus } from "../hooks/useCabanas";
import { Tooltip } from "./Tooltip";

interface CabanaTileProps {
  tile: Tile;
  status: CabanaStatus;
  onClick: () => void;
}

export function CabanaTile({ tile, status, onClick }: CabanaTileProps) {
  const isAvailable = status === "available";
  const tooltipText = `Cabana ${tile.cabanaId} • ${isAvailable ? "Available" : "Booked"}`;

  return (
    <Tooltip content={tooltipText}>
      <button
        className={`relative flex w-full aspect-square cursor-pointer items-center justify-center overflow-hidden border-2 p-0 transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-(--color-accent) focus-visible:ring-offset-1 focus-visible:z-20 ${
          isAvailable
            ? "border-(--color-available) hover:z-10 hover:-translate-y-0.5 hover:scale-105 hover:shadow-xl focus:z-20"
            : "border-(--color-booked) opacity-50 hover:opacity-65 cursor-not-allowed"
        }`}
        onClick={isAvailable ? onClick : undefined}
        aria-label={`Cabana ${tile.cabanaId}, ${isAvailable ? "available. Press Enter to book" : "already booked"}`}
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
      {!isAvailable && (
        <div className="pointer-events-none absolute inset-0 bg-(--color-booked)/20" />
      )}
      </button>
    </Tooltip>
  );
}
