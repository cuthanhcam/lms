# LMS Monorepo

A backend-first Learning Management System monorepo for architecture practice and production-style API development.

This repository currently contains:

- `LMS`: full layered implementation (Clean Architecture style).
- `SimpleLMS`: simplified implementation for faster iteration and comparison.
- `database`: SQL bootstrap scripts.
- `LMS.Web`: frontend application (in progress, excluded from current primary commit scope).

## Why This Repository Exists

The repository is designed to help you:

- Compare a fully layered backend against a simplified backend.
- Practice API design, domain modeling, and validation patterns.
- Work with authentication, course management, lessons, and enrollment flows.
- Evolve an LMS backend with tests and incremental refactoring.

## Feature Overview

### LMS (Layered/Clean Architecture style)

- Authentication and authorization with JWT and role policies (Student, Instructor, Admin)
- Course lifecycle management (create, update, publish/unpublish, soft-delete)
- Lesson management with ordering and course ownership constraints
- Enrollment lifecycle and progress tracking
- Application-level validation and centralized exception handling
- Unit tests and integration tests for core workflows

### SimpleLMS (Simplified implementation)

- Core user, course, lesson, and enrollment flows
- Reduced layering for faster experimentation
- Test suite for domain/application/infrastructure behavior

Current quality state:

- Build: successful
- Tests: fully passing across monorepo (124/124)

### LMS.Web (Frontend)

- In progress and intentionally excluded from current primary backend commit scope

## Architecture Explanation

This monorepo contains two backend implementations with different complexity levels:

- LMS applies a Clean Architecture direction with DDD concepts.
	- Domain: entities, value objects, domain events, business invariants.
	- Application: use-case services, DTOs, interfaces, validators.
	- Infrastructure: EF Core persistence, repositories, identity services.
	- API: HTTP endpoints, middleware, auth policies, dependency wiring.
- SimpleLMS keeps a simpler structure to optimize learning speed and iteration.

This dual-track setup makes architectural trade-offs explicit and easier to evaluate in practice.

## Why It Matters

- Maintainers can compare architecture depth vs implementation speed in one repository.
- New contributors can start with SimpleLMS, then move to LMS for deeper patterns.
- Teams can reuse LMS patterns (validation boundaries, domain rules, event flow) in production APIs.
- The repository serves as a practical reference for backend engineering interviews and portfolio review.

## Repository Layout

```text
lms/
|- LMS/
|  |- src/
|  |  |- LMS.API/
|  |  |- LMS.Application/
|  |  |- LMS.Domain/
|  |  |- LMS.Infrastructure/
|  |  `- LMS.Shared/
|  `- tests/
|     |- LMS.API.IntegrationTests/
|     `- LMS.Application.UnitTests/
|- SimpleLMS/
|  |- src/
|  |  |- SimpleLMS.API/
|  |  |- SimpleLMS.Application/
|  |  |- SimpleLMS.Domain/
|  |  `- SimpleLMS.Infrastructure/
|  `- tests/
|     `- SimpleLMS.Tests/
|- database/
|  `- create-database-lms.sql
`- LMS.Web/ (work in progress)
```

## Technology Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- xUnit (tests)
- FluentValidation (LMS)
- Manual DTO mapping in SimpleLMS (AutoMapper removed during hardening)

## Prerequisites

Install the following before running locally:

- SQL Server (local or remote dev instance)
- .NET SDK 8.0.x for `LMS`
- .NET SDK 10.0.x for `SimpleLMS`

Notes:

- `LMS/global.json` pins SDK `8.0.416`.
- `SimpleLMS/global.json` pins SDK `10.0.201` with `rollForward: latestFeature`.
- If only .NET 8 is installed, `SimpleLMS` cannot be built and will fail with `NETSDK1045`.

## Quick Start

### LMS (net8.0)

```powershell
cd LMS
dotnet restore
dotnet build .\src\LMS.API\LMS.API.csproj -c Release
dotnet run --project .\src\LMS.API\LMS.API.csproj
```

### SimpleLMS (net10.0)

```powershell
cd SimpleLMS
dotnet restore
dotnet build .\src\SimpleLMS.API\SimpleLMS.API.csproj -c Release
dotnet run --project .\src\SimpleLMS.API\SimpleLMS.API.csproj
```

## Build Verification (March 28, 2026)

The following checks were executed from the repository root:

- `LMS` projects: build succeeded.
- `SimpleLMS` projects: build succeeded.

Test verification:

- `LMS`: 33/33 tests passed.
- `SimpleLMS`: 91/91 tests passed.
- Monorepo total: 124/124 tests passed.

Interpretation:

- Both LMS and SimpleLMS are currently in a stable, fully passing test state.

## Hardening Status

Security and quality hardening completed in the latest backend pass:

- Removed vulnerable AutoMapper dependency from SimpleLMS application layer.
- Replaced AutoMapper usage with explicit mapping in service layer.
- Added explicit DI abstractions package to keep dependency graph clear.
- Re-audited packages with `dotnet list package --vulnerable --include-transitive`.

Current result:

- No vulnerable packages reported for `SimpleLMS` projects.
- Full test suite remains green (`124/124`).

## Database Setup

Use the bootstrap script in:

- `database/create-database-lms.sql`

Then configure connection strings in each API project:

- `LMS/src/LMS.API/appsettings.json`
- `LMS/src/LMS.API/appsettings.Development.json`
- `SimpleLMS/src/SimpleLMS.API/appsettings.json` (if present in your branch)

## Testing

Run tests per implementation:

```powershell
# LMS
dotnet test .\LMS\tests\LMS.Application.UnitTests\LMS.Application.UnitTests.csproj -c Release
dotnet test .\LMS\tests\LMS.API.IntegrationTests\LMS.API.IntegrationTests.csproj -c Release

# SimpleLMS (requires .NET 10 SDK)
dotnet test .\SimpleLMS\tests\SimpleLMS.Tests\SimpleLMS.Tests.csproj -c Release
```

## Current Commit Scope

Recommended scope for backend-first commits:

- Include: `LMS`, `SimpleLMS`, `database`, and repository governance/documentation files.
- Exclude for now: `LMS.Web` (frontend under active development).

## Roadmap

### Phase 1: Stabilize backend baseline

- Keep LMS test suite green on every change
- Keep SimpleLMS test suite green and aligned with domain behavior
- Continue periodic dependency health scans

### Phase 2: Improve architecture consistency

- Remove duplicate/overlapping configuration in infrastructure registrations
- Tighten architectural boundaries and reduce unnecessary cross-layer coupling
- Expand architecture decision notes and developer onboarding docs

### Phase 3: Expand functional coverage

- Add richer course workflows (draft/review/publish lifecycle)
- Improve enrollment analytics and reporting endpoints
- Add more robust integration and contract tests

### Phase 4: Frontend completion and integration

- Finalize LMS.Web production-ready workflows
- Integrate end-to-end scenarios across API and frontend
- Prepare release checklist and deployment documentation

## Open Source Governance

See the project policies and templates:

- `CONTRIBUTING.md`
- `CODE_OF_CONDUCT.md`
- `SECURITY.md`
- `.github/PULL_REQUEST_TEMPLATE.md`
- `.github/ISSUE_TEMPLATE/bug_report.md`
- `.github/ISSUE_TEMPLATE/feature_request.md`
- `COMMIT_MESSAGE_GUIDELINES.md`
- `GIT_GUIDELINES.md`

## Maintainer

- GitHub: https://github.com/cuthanhcam
- Email: cuthanhcam04@gmail.com

## License

Licensed under the MIT License. See `LICENSE` for details.
