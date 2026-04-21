import { useState } from "react";
import { ResortMap } from "./components/ResortMap";
import { BookingModal } from "./components/BookingModal";
import { useCabanas, type Cabana } from "./hooks/useCabanas";

export default function App() {
  const [selectedCabana, setSelectedCabana] = useState<Cabana | null>(null);
  const { cabanas, isLoading: cabanasLoading, error: cabanasError, refetch } = useCabanas();

  return (
    <div className="flex min-h-screen flex-col items-center bg-(--color-bg) px-6 py-10 font-sans text-(--color-text)">
      <header className="mb-6 text-center">
        <h1 className="text-2xl font-semibold tracking-tight text-(--color-text)">
          Resort Cabana Booking
        </h1>
        <p className="mt-1 text-sm text-(--color-text-muted)">
          Click an available cabana to reserve your spot
        </p>
      </header>

      <nav aria-label="Map legend" className="mb-6">
        <ul className="flex flex-wrap items-center justify-center gap-4 rounded-lg border border-(--color-border) bg-(--color-surface) px-5 py-3 shadow-sm md:gap-5 list-none m-0 p-0" style={{ padding: "0.75rem 1.25rem" }}>
          <li className="flex items-center gap-2 text-sm font-medium text-(--color-text)">
            <span
              className="relative flex size-6 items-center justify-center overflow-hidden border-2 border-(--color-available)"
              style={{ backgroundImage: "url(/assets/parchmentBasic.png)", backgroundSize: "cover" }}
            >
              <img src="/assets/cabana.png" alt="" className="size-full object-contain" />
            </span>
            Cabana (Available)
          </li>
          <li aria-hidden="true" className="hidden h-4 w-px bg-(--color-border) lg:block" />
          <li className="flex items-center gap-2 text-sm font-medium text-(--color-text)">
            <span
              className="relative flex size-6 items-center justify-center overflow-hidden border-2 border-(--color-booked) opacity-50"
              style={{ backgroundImage: "url(/assets/parchmentBasic.png)", backgroundSize: "cover" }}
            >
              <img src="/assets/cabana.png" alt="" className="size-full object-contain" />
              <div className="absolute inset-0 bg-(--color-booked)/20" />
            </span>
            Cabana (Booked)
          </li>
          <li aria-hidden="true" className="hidden h-4 w-px bg-(--color-border) lg:block" />
          <li className="flex items-center gap-2 text-sm font-medium text-(--color-text)">
            <img src="/assets/pool.png" alt="" className="size-5 object-contain" />
            Pool
          </li>
          <li aria-hidden="true" className="hidden h-4 w-px bg-(--color-border) lg:block" />
          <li className="flex items-center gap-2 text-sm font-medium text-(--color-text)">
            <img src="/assets/houseChimney.png" alt="" className="size-5 object-contain opacity-80" />
            Chalet
          </li>
          <li aria-hidden="true" className="hidden h-4 w-px bg-(--color-border) lg:block" />
          <li className="flex items-center gap-2 text-sm font-medium text-(--color-text)">
            <img src="/assets/arrowStraight.png" alt="" className="size-5 object-contain opacity-60" />
            Path
          </li>
        </ul>
      </nav>

      <main id="main-content" className="w-full max-w-screen-lg">
        <ResortMap
          cabanas={cabanas}
          cabanasLoading={cabanasLoading}
          cabanasError={cabanasError}
          onCabanaSelect={setSelectedCabana}
        />
      </main>

      {selectedCabana && (
        <BookingModal
          cabana={selectedCabana}
          onClose={() => setSelectedCabana(null)}
          onBookingSuccess={refetch}
        />
      )}
    </div>
  );
}
