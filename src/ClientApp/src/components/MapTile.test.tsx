import { render, screen } from "@testing-library/react";
import { describe, it, expect } from "vitest";
import { MapTile } from "./MapTile";
import type { Tile } from "../hooks/useMap";

function renderMapTile(tile: Tile) {
  return render(<MapTile tile={tile} />);
}

describe("MapTile", () => {
  it("renders a pool tile with alt text 'Swimming pool'", () => {
    renderMapTile({ type: "pool", row: 0, col: 0 });

    expect(screen.getByAltText("Swimming pool")).toBeInTheDocument();
  });

  it("renders a chalet tile with alt text 'Chalet'", () => {
    renderMapTile({ type: "chalet", row: 0, col: 0 });

    expect(screen.getByAltText("Chalet")).toBeInTheDocument();
  });

  it("renders an empty tile as decorative (aria-hidden)", () => {
    const { container } = renderMapTile({ type: "empty", row: 0, col: 0 });

    expect(container.firstElementChild).toHaveAttribute("aria-hidden", "true");
  });

  it("renders a path tile as decorative (aria-hidden)", () => {
    const { container } = renderMapTile({ type: "path", row: 0, col: 0, pathVariant: "straight", rotation: 0 });

    expect(container.firstElementChild).toHaveAttribute("aria-hidden", "true");
  });

  it("applies rotation to a path tile image", () => {
    const { container } = renderMapTile({ type: "path", row: 1, col: 2, pathVariant: "corner", rotation: 90 });

    const img = container.querySelector("img") as HTMLImageElement;
    expect(img.style.transform).toBe("rotate(90deg)");
  });

  it("does not apply rotation to non-path tiles", () => {
    const { container } = renderMapTile({ type: "pool", row: 0, col: 0 });

    const img = container.querySelector("img") as HTMLImageElement;
    expect(img.style.transform).toBe("");
  });
});
