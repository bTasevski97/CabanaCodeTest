import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi } from "vitest";
import { CabanaTile } from "./CabanaTile";
import type { Tile } from "../hooks/useMap";
import type { CabanaStatus } from "../hooks/useCabanas";

const baseTile: Tile = {
  type: "cabana",
  row: 3,
  col: 5,
  cabanaId: "W-3-5",
};

function renderCabanaTile(status: CabanaStatus, onClick = vi.fn()) {
  render(<CabanaTile tile={baseTile} status={status} onClick={onClick} />);
  return { onClick };
}

describe("CabanaTile", () => {
  it("renders an available cabana as a clickable button", () => {
    renderCabanaTile("available");

    const button = screen.getByRole("button", { name: /cabana W-3-5.*available/i });
    expect(button).toBeInTheDocument();
    expect(button).not.toHaveAttribute("aria-disabled", "true");
  });

  it("renders a booked cabana with disabled state", () => {
    renderCabanaTile("booked");

    const button = screen.getByRole("button", { name: /cabana W-3-5.*booked/i });
    expect(button).toHaveAttribute("aria-disabled", "true");
  });

  it("calls onClick when an available cabana is clicked", async () => {
    const user = userEvent.setup();
    const { onClick } = renderCabanaTile("available");

    await user.click(screen.getByRole("button", { name: /cabana W-3-5/i }));
    expect(onClick).toHaveBeenCalledTimes(1);
  });

  it("does not call onClick when a booked cabana is clicked", async () => {
    const user = userEvent.setup();
    const { onClick } = renderCabanaTile("booked");

    await user.click(screen.getByRole("button", { name: /cabana W-3-5/i }));
    expect(onClick).not.toHaveBeenCalled();
  });
});
