import { render, screen } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { ResortMap } from "./ResortMap";
import type { Cabana } from "../hooks/useCabanas";

// Mock useMap since we are unit testing ResortMap
vi.mock("../hooks/useMap", () => ({
  useMap: () => ({
    map: {
      rows: 1,
      cols: 1,
      tiles: [{ type: "cabana", row: 0, col: 0, cabanaId: "W-0-0" }],
    },
    isLoading: false,
    error: null,
  }),
}));

describe("ResortMap", () => {
  it("shows the 'fully booked' alert when no cabanas are available", () => {
    const cabanas: Cabana[] = [{ id: "W-0-0", row: 0, col: 0, status: "booked" }];

    render(
      <ResortMap
        cabanas={cabanas}
        cabanasLoading={false}
        cabanasError={null}
        onCabanaSelect={() => {}}
      />
    );

    expect(screen.getByRole("status")).toHaveTextContent(/all cabanas are fully booked/i);
    expect(screen.getByRole("region")).toHaveAttribute(
      "aria-label",
      expect.stringContaining("fully booked")
    );
  });

  it("shows number of available cabanas in the accessible label", () => {
    const cabanas: Cabana[] = [{ id: "W-0-0", row: 0, col: 0, status: "available" }];

    render(
      <ResortMap
        cabanas={cabanas}
        cabanasLoading={false}
        cabanasError={null}
        onCabanaSelect={() => {}}
      />
    );

    expect(screen.queryByRole("status")).not.toBeInTheDocument();
    expect(screen.getByRole("region")).toHaveAttribute(
      "aria-label",
      expect.stringContaining("1 of 1 cabana available")
    );
  });

  it("shows error state when cabanas fail to load", () => {
    render(
      <ResortMap
        cabanas={undefined}
        cabanasLoading={false}
        cabanasError={new Error("Failed to load cabanas")}
        onCabanaSelect={() => {}}
      />
    );

    expect(screen.getByRole("alert")).toHaveTextContent(/unable to load the resort map/i);
    expect(screen.getByText("Failed to load cabanas")).toBeInTheDocument();
  });
});
