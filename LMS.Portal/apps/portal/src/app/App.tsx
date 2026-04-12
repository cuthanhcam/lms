import { Button } from "@lmsportal/ui/components/button"

export function App() {
  return (
    <div className="flex min-h-svh p-6">
      <div className="flex max-w-md min-w-0 flex-col gap-4 text-sm leading-loose">
        <div>
          <h1 className="font-medium">LMS.Portal is ready</h1>
          <p>Start building learner, instructor, and admin experiences.</p>
          <p>The shared UI package is configured and ready to use.</p>
          <Button className="mt-2">Get Started</Button>
        </div>
        <div className="text-muted-foreground font-mono text-xs">
          (Press <kbd>d</kbd> to toggle dark mode)
        </div>
      </div>
    </div>
  )
}
