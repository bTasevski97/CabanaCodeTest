import { MapTile } from "./MapTile";
import { CabanaTile } from "./CabanaTile";
import { MapSkeleton } from "./MapSkeleton";
import { useMap } from "../hooks/useMap";
import type { Cabana } from "../hooks/useCabanas";

interface ResortMapProps {
  cabanas: Cabana[] | undefined;
  cabanasLoading: boolean;
  cabanasError: Error | null;
  onCabanaSelect: (cabana: Cabana) => void;
}

export function ResortMap({ cabanas, cabanasLoading, cabanasError, onCabanaSelect }: ResortMapProps) {
  const { map, isLoading: mapLoading, error: mapError } = useMap();

  if (mapError || cabanasError) {
    return (
      <div role="alert" className="mx-auto max-w-sm rounded-xl border border-(--color-border) bg-(--color-surface) p-10 text-center shadow-sm">
        <p className="font-medium text-(--color-text)">Unable to load the resort map</p>
        <p className="mt-2 text-sm text-(--color-error)">
          {mapError?.message || cabanasError?.message}
        </p>
      </div>
    );
  }

  if (mapLoading || cabanasLoading || !map || !cabanas) {
    return <MapSkeleton />;
  }

  const cabanaMap = new Map(cabanas.map((cabana) => [cabana.id, cabana]));
  const available = cabanas.filter((c) => c.status === "available").length;
  const fullyBooked = cabanas.length > 0 && available === 0;

  return (
    <div className="flex flex-col gap-4">
      {fullyBooked  && (
        <div
          role="status"
          aria-live="polite"
          className="flex items-start gap-3 rounded-xl border border-amber-300 bg-amber-50 px-5 py-4 shadow-sm"
        >
          <span aria-hidden="true" className="mt-0.5 text-xl leading-none">⚠️</span>
          <div>
            <p className="text-sm font-semibold text-amber-900">All cabanas are fully booked</p>
            <p className="mt-0.5 text-sm text-amber-800">
              There are no cabanas available right now. Please check back later or contact reception.
            </p>
          </div>
        </div>
      )}

      <div
        role="region"
        aria-label={`Resort map – ${fullyBooked ? "fully booked" : `${available} of ${cabanas.length} cabana${cabanas.length !== 1 ? "s" : ""} available`}`}
        aria-busy={cabanasLoading}
        className="grid w-full overflow-hidden rounded-xl border border-(--color-border) bg-(--color-surface) shadow-md"
        style={{
          gridTemplateColumns: `repeat(${map.cols}, 1fr)`,
          gridTemplateRows: `repeat(${map.rows}, 1fr)`,
        }}
      >
        {map.tiles.map((tile) => {
          if (tile.type === "cabana" && tile.cabanaId) {
            const cabana = cabanaMap.get(tile.cabanaId);
            return (
              <CabanaTile
                key={`${tile.row}-${tile.col}`}
                tile={tile}
                status={cabana?.status ?? "available"}
                onClick={() => cabana && onCabanaSelect(cabana)}
              />
            );
          }
          return <MapTile key={`${tile.row}-${tile.col}`} tile={tile} />;
        })}
      </div>
    </div>
  );
}
