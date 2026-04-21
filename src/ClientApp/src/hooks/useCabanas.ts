import { useState, useEffect } from "react";

export type CabanaStatus = "available" | "booked";

export interface Cabana {
  id: string;
  row: number;
  col: number;
  status: CabanaStatus;
  bookedBy?: { room: string; guestName: string };
}

export interface BookingRequest {
  room: string;
  guestName: string;
}

export interface BookingResponse {
  success: boolean;
  message: string;
}

export function useCabanas() {
  const [cabanas, setCabanas] = useState<Cabana[] | undefined>(undefined);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  async function fetchCabanas(showLoadingSpinner = false) {
    if (showLoadingSpinner) setIsLoading(true);
    setError(null);
    try {
      const response = await fetch("/api/cabanas");
      if (!response.ok) throw new Error("Failed to fetch cabanas");
      setCabanas(await response.json());
    } catch (err) {
      setError(err as Error);
    } finally {
      if (showLoadingSpinner) setIsLoading(false);
    }
  }

  useEffect(() => { fetchCabanas(true); }, []);

  return { cabanas, isLoading, error, refetch: fetchCabanas };
}

export function useBookCabana(onBookingSuccess: () => void) {
  const [isBooking, setIsBooking] = useState(false);
  const [bookingError, setBookingError] = useState<Error | null>(null);
  const [bookingResult, setBookingResult] = useState<BookingResponse | null>(null);

  async function bookCabana(cabanaId: string, request: BookingRequest) {
    setIsBooking(true);
    setBookingError(null);
    try {
      const response = await fetch(`/api/cabanas/${cabanaId}/book`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
      });
      if (!response.ok) {
        const errorBody = await response.json().catch(() => null);
        throw new Error(errorBody?.message ?? "Failed to book cabana");
      }
      setBookingResult(await response.json());
      onBookingSuccess();
    } catch (err) {
      setBookingError(err as Error);
    } finally {
      setIsBooking(false);
    }
  }

  function resetBookingState() {
    setBookingResult(null);
    setBookingError(null);
  }

  return { bookCabana, isBooking, bookingError, bookingResult, resetBookingState };
}
