import type { ButtonHTMLAttributes } from "react"

import { cn } from "../lib/utils"

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement>

export function Button({ className, type = "button", ...props }: ButtonProps) {
  return (
    <button
      type={type}
      className={cn(
        "inline-flex items-center justify-center rounded-md border px-3 py-2 text-sm font-medium transition-colors",
        "bg-foreground text-background hover:opacity-90",
        "disabled:pointer-events-none disabled:opacity-60",
        className
      )}
      {...props}
    />
  )
}
