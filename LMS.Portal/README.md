# LMS.Portal

LMS.Portal is the frontend workspace for LMS, built as a stable Vite + React + TypeScript monorepo.

## Adding components

To add components to your app, run the following command at the root of your `portal` app:

```bash
pnpm dlx shadcn@latest add button -c apps/portal
```

This will place UI components in `packages/ui/src/components`.

## Using components

To use the components in your app, import them from the shared UI package.

```tsx
import { Button } from "@lmsportal/ui/components/button";
```
