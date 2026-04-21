import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, afterEach } from "vitest";
import App from "./App";
import { server } from "./mocks/server";
import { http, HttpResponse } from "msw";

afterEach(() => {
  vi.clearAllMocks();
  vi.useRealTimers();
});

describe("Booking Flow Integration", () => {
  it("allows a user to book a cabana successfully", async () => {
    const user = userEvent.setup();
    render(<App />);

    await waitFor(() => {
      expect(screen.queryByLabelText(/loading resort map/i)).not.toBeInTheDocument();
    });

    const cabanaButton = screen.getByRole("button", { name: /Cabana W-0-1, available/i });
    await user.click(cabanaButton);

    expect(screen.getByRole("heading", { name: /book cabana/i, hidden: true })).toBeInTheDocument();

    const roomInput = screen.getByLabelText(/room number/i);
    const nameInput = screen.getByLabelText(/guest name/i);
    await user.type(roomInput, "123");
    await user.type(nameInput, "Jane Smith");

    const confirmButton = screen.getByRole("button", { name: /confirm booking/i, hidden: true });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(screen.getByText(/booking confirmed/i)).toBeInTheDocument();
    });
    expect(screen.getByText(/Cabana booked for Jane Smith \(Room 123\)/i)).toBeInTheDocument();

    vi.useFakeTimers();
    expect(screen.getByText(/5s/i)).toBeInTheDocument();

    await vi.advanceTimersByTimeAsync(5000);

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
  });

  it("shows an error message when booking fails with 400", async () => {
    const user = userEvent.setup();
    render(<App />);

    await waitFor(() => expect(screen.queryByLabelText(/loading resort map/i)).not.toBeInTheDocument());

    const cabanaButton = screen.getByRole("button", { name: /Cabana W-0-1, available/i });
    await user.click(cabanaButton);

    await user.type(screen.getByLabelText(/room number/i), "999");
    await user.type(screen.getByLabelText(/guest name/i), "Error Test");

    await user.click(screen.getByRole("button", { name: /confirm booking/i, hidden: true }));

    await waitFor(() => {
      expect(screen.getByText(/invalid room number/i)).toBeInTheDocument();
    });
  });

  it("shows an error message when booking fails with server error (500)", async () => {
    server.use(
      http.post("/api/cabanas/:id/book", () => {
        return new HttpResponse(null, { status: 500 });
      })
    );

    const user = userEvent.setup();
    render(<App />);

    await waitFor(() => expect(screen.queryByLabelText(/loading resort map/i)).not.toBeInTheDocument());

    const cabanaButton = screen.getByRole("button", { name: /Cabana W-0-1, available/i });
    await user.click(cabanaButton);

    await user.type(screen.getByLabelText(/room number/i), "123");
    await user.type(screen.getByLabelText(/guest name/i), "Server Error Test");

    await user.click(screen.getByRole("button", { name: /confirm booking/i, hidden: true }));

    await waitFor(() => {
      expect(screen.getByText(/failed to book cabana/i)).toBeInTheDocument();
    });
  });
});
