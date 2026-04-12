import { StrictMode } from "react"
import { createRoot } from "react-dom/client"

import "@lmsportal/ui/globals.css"
import { App } from "@/app/App.tsx"
import { ThemeProvider } from "@/shared/providers/theme-provider.tsx"

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider>
      <App />
    </ThemeProvider>
  </StrictMode>
)
