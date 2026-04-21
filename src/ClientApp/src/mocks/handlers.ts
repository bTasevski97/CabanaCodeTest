import { http, HttpResponse } from "msw";

const mockMap = {
  rows: 2,
  cols: 2,
  tiles: [
    { type: "path", row: 0, col: 0, pathVariant: "straight", rotation: 90 },
    { type: "cabana", row: 0, col: 1, cabanaId: "W-0-1" },
    { type: "pool", row: 1, col: 0 },
    { type: "cabana", row: 1, col: 1, cabanaId: "W-1-1" },
  ],
};

const mockCabanas = [
  { id: "W-0-1", row: 0, col: 1, status: "available" },
  { id: "W-1-1", row: 1, col: 1, status: "booked" },
];

export const handlers = [
  http.get("/api/map", () => {
    return HttpResponse.json(mockMap);
  }),

  http.get("/api/cabanas", () => {
    return HttpResponse.json(mockCabanas);
  }),

  http.post("/api/cabanas/:id/book", async ({ params, request }) => {
    const { id } = params;
    const body = (await request.json()) as { room: string; guestName: string };

    if (body.room === "999") {
      return HttpResponse.json(
        { success: false, message: "Invalid room number or guest name." },
        { status: 400 }
      );
    }

    if (id === "W-1-1") {
      return HttpResponse.json(
        { success: false, message: "This cabana is already booked." },
        { status: 409 }
      );
    }

    return HttpResponse.json({
      success: true,
      message: `Cabana booked for ${body.guestName} (Room ${body.room}).`,
    });
  }),
];
