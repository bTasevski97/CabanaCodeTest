import { render, screen } from "@testing-library/react";
import { describe, it, expect } from "vitest";
import { BookingModal } from "./BookingModal";
import type { Cabana } from "../hooks/useCabanas";

const availableCabana: Cabana = { id: "W-3-5", row: 3, col: 5, status: "available" };
const bookedCabana: Cabana = { id: "W-3-5", row: 3, col: 5, status: "booked" };

function renderBookingModal(cabana: Cabana) {
  return render(
    <BookingModal cabana={cabana} onClose={() => {}} onBookingSuccess={() => {}} />
  );
}

describe("BookingModal", () => {
  it("shows a booking form with room and guest name fields for an available cabana", () => {
    renderBookingModal(availableCabana);

    expect(screen.getByRole("heading", { name: /book cabana/i, hidden: true })).toBeInTheDocument();
    expect(screen.getByLabelText(/room number/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/guest name/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /confirm booking/i, hidden: true })).toBeInTheDocument();
  });

  it("shows 'already booked' message instead of form for a booked cabana", () => {
    renderBookingModal(bookedCabana);

    expect(screen.getByText(/already booked/i)).toBeInTheDocument();
    expect(screen.queryByLabelText(/room number/i)).not.toBeInTheDocument();
    expect(screen.queryByLabelText(/guest name/i)).not.toBeInTheDocument();
  });

  it("has a close button in the header", () => {
    renderBookingModal(availableCabana);

    expect(screen.getByRole("button", { name: /close dialog/i, hidden: true })).toBeInTheDocument();
  });

  it("room and guest inputs are required", () => {
    renderBookingModal(availableCabana);

    expect(screen.getByLabelText(/room number/i)).toBeRequired();
    expect(screen.getByLabelText(/guest name/i)).toBeRequired();
  });
});
