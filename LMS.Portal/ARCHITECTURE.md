# LMS.Portal Architecture

## Goal

LMS.Portal is the frontend workspace for LMS, implemented as a React + TypeScript monorepo with stable defaults for long-term growth.

## Workspace Layout

```text
LMS.Portal/
|- apps/
|  `- portal/
|     `- src/
|        |- app/                  # app bootstrapping and composition
|        |- features/             # domain feature slices
|        `- shared/               # app-level shared code
`- packages/
   `- ui/                         # shared UI primitives
```

## Folder Rules

### app

- Owns startup, providers, and route composition.
- No domain business logic.

### features

- Organized by LMS business domains (catalog, enrollment, learning, assessment, commerce, profile).
- A feature may use `shared` and `@lmsportal/ui`.
- A feature should not import internals from another feature.

### shared

- Cross-feature helpers, providers, layouts, and local app primitives.
- Promote code to `packages/*` when reused by multiple apps.

### packages/ui

- Reusable UI primitives and style utilities.
- Keeps app implementation independent from low-level presentation details.

## Recommended First Feature Slices

- `features/auth`
- `features/catalog`
- `features/enrollment`
- `features/learning`
- `features/profile`

## Commands

Run from `LMS.Portal`:

```bash
corepack pnpm install
corepack pnpm -r typecheck
corepack pnpm -r build
corepack pnpm --filter portal dev
```

## Stability Notes

- Node 20+ and pnpm 9/10 are supported.
- Keep TypeScript strict mode enabled.
- Prefer small vertical slices (API hook + state + page) per feature.
