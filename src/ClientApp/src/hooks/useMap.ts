import { useState, useEffect } from "react";

export type TileType = "empty" | "chalet" | "path" | "pool" | "cabana";
export type PathVariant = "straight" | "corner" | "crossing" | "end" | "split";

export interface Tile {
  type: TileType;
  row: number;
  col: number;
  pathVariant?: PathVariant;
  rotation?: number;
  cabanaId?: string;
}

export interface MapData {
  rows: number;
  cols: number;
  tiles: Tile[];
}

export function useMap() {
  const [map, setMap] = useState<MapData | undefined>(undefined);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    async function load() {
      try {
        const response = await fetch("/api/map");
        if (!response.ok) throw new Error("Failed to fetch map data");
        setMap(await response.json());
      } catch (err) {
        setError(err as Error);
      } finally {
        setIsLoading(false);
      }
    }

    load();
  }, []);

  return { map, isLoading, error };
}
