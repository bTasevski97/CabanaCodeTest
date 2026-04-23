import type { Tile, PathVariant } from "../hooks/useMap";
import { Tooltip } from "./Tooltip";

const TILE_ASSETS: Record<string, string> = {
  empty: "/assets/parchmentBasic.png",
  chalet: "/assets/houseChimney.png",
  pool: "/assets/pool.png",
  cabana: "/assets/cabana.png",
};

const PATH_ASSETS: Record<PathVariant, string> = {
  straight: "/assets/arrowStraight.png",
  corner: "/assets/arrowCornerSquare.png",
  crossing: "/assets/arrowCrossing.png",
  end: "/assets/arrowEnd.png",
  split: "/assets/arrowSplit.png",
};

const TILE_ALT: Record<string, string> = {
  empty: "",
  path: "",
  chalet: "Chalet",
  pool: "Swimming pool",
};

interface MapTileProps {
  tile: Tile;
}

export function MapTile({ tile }: MapTileProps) {
  const assetUrl =
    tile.type === "path"
      ? PATH_ASSETS[tile.pathVariant ?? "straight"]
      : TILE_ASSETS[tile.type];

  const rotation = tile.type === "path" ? tile.rotation ?? 0 : 0;
  const alt = TILE_ALT[tile.type] ?? "";
  const isDecorative = alt === "" || tile.type === "empty" || tile.type === "path";
  const isInteractive = tile.type === "chalet" || tile.type === "pool";

  const content = (
    <div
      aria-hidden={isDecorative}
      aria-label={isInteractive ? alt : undefined}
      className={`relative flex w-full aspect-square items-center justify-center overflow-hidden transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-(--color-accent) focus-visible:ring-offset-1 focus-visible:z-20 ${
        isInteractive ? "cursor-help hover:z-10 hover:-translate-y-0.5 hover:scale-105 hover:shadow-lg focus:z-20" : ""
      }`}
      style={{
        backgroundImage: `url(${TILE_ASSETS.empty})`,
        backgroundSize: "cover",
      }}
    >
      <img
        src={assetUrl}
        alt={alt}
        className="pointer-events-none block size-full object-contain"
        style={rotation ? { transform: `rotate(${rotation}deg)` } : undefined}
        draggable={false}
      />    
    </div>
  );

  if (isInteractive) {
    return <Tooltip content={alt}>{content}</Tooltip>;
  }

  return content;
}
