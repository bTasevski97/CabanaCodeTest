import { useState, useEffect, type SubmitEvent } from "react";
import { useBookCabana, type Cabana, type BookingResponse } from "../hooks/useCabanas";

interface BookingModalProps {
  cabana: Cabana;
  onClose: () => void;
  onBookingSuccess: () => void;
}

export function BookingModal({ cabana, onClose, onBookingSuccess }: BookingModalProps) {
  const { bookCabana, isBooking, bookingError, bookingResult } = useBookCabana(onBookingSuccess);

  const handleCancel = (e: React.SyntheticEvent) => {
    e.preventDefault();
    if (!isBooking) onClose();
  };

  const handleSubmit = (e: SubmitEvent) => {
    e.preventDefault();
    bookCabana(cabana.id, {
      room: (e.currentTarget as HTMLFormElement).room.value.trim(),
      guestName: (e.currentTarget as HTMLFormElement).guestName.value.trim()
    });
  };

  return (
    <dialog
      ref={(node) => {
        node?.showModal();
        return () => node?.close();
      }}
      onCancel={handleCancel}
      onClick={(e) => {
        if (e.target === e.currentTarget && !isBooking) onClose();
      }}
      aria-labelledby="dialog-title"
      className="m-auto w-[90vw] max-w-md rounded-2xl border border-(--color-border) bg-(--color-surface) p-0 shadow-lg backdrop:bg-black/30 backdrop:backdrop-blur-xs open:flex open:flex-col"
    >
      {bookingResult ? (
        <BookingSuccessView result={bookingResult} onClose={onClose} />
      ) : cabana.status === "booked" ? (
        <>
          <ModalHeader id="dialog-title" title="Cabana Unavailable" onClose={onClose} locked={false} />
          <div className="p-6">
            <p className="py-4 text-center text-sm text-(--color-text-muted)">
              This cabana is already booked.
            </p>
          </div>
        </>
      ) : (
        <>
          <ModalHeader id="dialog-title" title="Book Cabana" onClose={onClose} locked={isBooking} />
          <form className="p-6" onSubmit={handleSubmit}>
            <div className="mb-4">
              <label htmlFor="room" className="mb-1 block text-sm font-medium text-(--color-text)">
                Room Number
              </label>
              <input
                id="room"
                name="room"
                type="text"
                placeholder="e.g. 101"
                required
                disabled={isBooking}
                autoFocus
                className="w-full rounded-lg border border-(--color-border) bg-(--color-bg) px-3 py-2 text-sm text-(--color-text) outline-none transition-colors placeholder:text-(--color-text-muted) focus:border-(--color-accent) focus:ring-1 focus:ring-(--color-accent)/30 disabled:opacity-50"
              />
            </div>
            <div className="mb-5">
              <label htmlFor="guestName" className="mb-1 block text-sm font-medium text-(--color-text)">
                Guest Name
              </label>
              <input
                id="guestName"
                name="guestName"
                type="text"
                placeholder="e.g. Alice Smith"
                required
                disabled={isBooking}
                className="w-full rounded-lg border border-(--color-border) bg-(--color-bg) px-3 py-2 text-sm text-(--color-text) outline-none transition-colors placeholder:text-(--color-text-muted) focus:border-(--color-accent) focus:ring-1 focus:ring-(--color-accent)/30 disabled:opacity-50"
              />
            </div>
            {bookingError && (
              <p role="alert" className="mb-4 rounded-lg border border-(--color-error)/20 bg-(--color-error)/5 px-3 py-2 text-sm text-(--color-error)">
                {bookingError.message}
              </p>
            )}
            <button
              type="submit"
              disabled={isBooking}
              className="flex w-full items-center justify-center gap-2 cursor-pointer rounded-lg bg-(--color-accent) px-4 py-2.5 text-sm font-medium text-white transition-colors hover:opacity-90 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {isBooking && (
                <svg className="size-4 animate-spin" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" aria-hidden="true">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                </svg>
              )}
              {isBooking ? "Booking…" : "Confirm Booking"}
            </button>
          </form>
        </>
      )}
    </dialog>
  );
}

interface SuccessViewProps {
  result: BookingResponse;
  onClose: () => void;
}

function BookingSuccessView({ result, onClose }: SuccessViewProps) {
  const [secondsLeft, setSecondsLeft] = useState(5);

  useEffect(() => {
    if (secondsLeft <= 0) {
      onClose();
      return;
    }

    const timer = setInterval(() => {
      setSecondsLeft((prev) => prev - 1);
    }, 1000);

    return () => clearInterval(timer);
  }, [secondsLeft, onClose]);

  return (
    <div className="p-8 text-center">
      <div className="mx-auto mb-4 flex size-12 items-center justify-center rounded-full bg-(--color-available)/10 text-xl text-(--color-available)">
        ✓
      </div>
      <h2 id="dialog-title" className="mb-1 text-lg font-semibold text-(--color-text)">
        Booking Confirmed
      </h2>
      <p className="mb-2 text-sm text-(--color-text-muted)">{result.message}</p>
      <p className="mb-6 text-xs font-medium text-(--color-text-muted)/70">
        Returning to map in <span className="font-bold text-(--color-available)">{secondsLeft}s</span>...
      </p>
      <button
        autoFocus
        className="cursor-pointer rounded-lg border border-(--color-border) bg-(--color-surface) px-5 py-2 text-sm font-medium text-(--color-text) transition-colors hover:bg-(--color-bg)"
        onClick={onClose}
      >
        Close Now
      </button>
    </div>
  );
}

interface ModalHeaderProps {
  id: string;
  title: string;
  onClose: () => void;
  locked: boolean;
}

function ModalHeader({ id, title, onClose, locked }: ModalHeaderProps) {
  return (
    <div className="flex items-center justify-between border-b border-(--color-border) px-6 py-4">
      <h2 id={id} className="text-lg font-semibold text-(--color-text)">
        {title}
      </h2>
      <button
        onClick={onClose}
        disabled={locked}
        aria-label="Close dialog"
        className="cursor-pointer rounded-md border-0 bg-transparent p-1 leading-none text-(--color-text-muted) transition-colors hover:text-(--color-text) disabled:cursor-not-allowed disabled:opacity-30 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-(--color-accent)"
      >
        <span aria-hidden="true" className="block text-xl">
          ✕
        </span>
      </button>
    </div>
  );
}
