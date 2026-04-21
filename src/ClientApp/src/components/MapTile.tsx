import type { Tile, PathVariant } from "../hooks/useMap";

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
  const isDecorative = alt === "";

  return (
    <div
      aria-hidden={isDecorative}
      className="relative flex w-full aspect-square items-center justify-center overflow-hidden"
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
}
