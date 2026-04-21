import { ReactNode } from "react";

interface TooltipProps {
  children: ReactNode;
  content: string | undefined;
}

export function Tooltip({ children, content }: TooltipProps) {
  if (!content) return <>{children}</>;

  return (
    <div className="group relative flex h-full w-full">
      {children}
      <div 
        className="pointer-events-none absolute -top-2 left-1/2 z-50 -translate-x-1/2 -translate-y-full opacity-0 transition-all duration-200 ease-out group-focus-within:-top-3 group-focus-within:opacity-100 group-hover:-top-3 group-hover:opacity-100" 
        role="tooltip"
      >
        <div className="flex flex-col items-center gap-1 rounded-lg border border-(--color-border) bg-white/90 px-3 py-2 text-xs font-semibold whitespace-nowrap text-(--color-text) shadow-xl backdrop-blur-md">
          <span>{content}</span>
          <div className="absolute -bottom-1 left-1/2 size-2 -translate-x-1/2 rotate-45 border-b border-r border-(--color-border) bg-white/90 backdrop-blur-md" />
        </div>
      </div>
    </div>
  );
}
