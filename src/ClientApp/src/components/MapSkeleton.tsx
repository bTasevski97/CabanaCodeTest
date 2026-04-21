export function MapSkeleton() {
  return (
    <div
      role="status"
      aria-label="Loading resort map"
      className="flex flex-col items-center gap-3 rounded-xl border border-(--color-border) bg-(--color-surface) px-14 py-10 shadow-sm"
    >
      <div aria-hidden="true" className="size-8 animate-spin rounded-full border-2 border-(--color-border) border-t-(--color-accent)" />
      <p className="text-sm text-(--color-text-muted)">Loading resort map…</p>
    </div>
  );
}
