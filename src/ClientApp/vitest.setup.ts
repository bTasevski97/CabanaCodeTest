import "@testing-library/jest-dom/vitest";
import { cleanup } from "@testing-library/react";
import { afterEach, beforeAll, afterAll, vi } from "vitest";
import { server } from "./src/mocks/server";

// jsdom doesn't support HTMLDialogElement.showModal/close
beforeAll(() => {
  HTMLDialogElement.prototype.showModal = vi.fn();
  HTMLDialogElement.prototype.close = vi.fn();

  // JSDOM Form Fix: Ensure named properties (form.room) are accessible during tests
  // This helps Vitest handle the original code's form access style without changes.
  Object.defineProperty(HTMLFormElement.prototype, "room", {
    get() { return this.elements.namedItem("room"); }
  });
  Object.defineProperty(HTMLFormElement.prototype, "guestName", {
    get() { return this.elements.namedItem("guestName"); }
  });

  server.listen({ onUnhandledRequest: "error" });
});

afterEach(() => {
  server.resetHandlers();
  cleanup();
});

afterAll(() => server.close());
