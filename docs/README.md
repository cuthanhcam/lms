# LMS Development Docs

This folder contains practical documentation to evolve this monorepo into a production-grade LMS template.

## Main Documents

- LMS_PRODUCTION_TEMPLATE_STATUS.md: Full capability map by domain with current status (Done, Partial, Not Started) and next actions.
- IMPLEMENTATION_PLAN.md: Execution plan grouped by priority and dependency order.
- IMPLEMENTATION_BACKLOG.md: Detailed module backlog with acceptance criteria and test requirements.
- SECURITY_CONFIGURATION.md: Secure configuration guidance for appsettings, User Secrets, and environment variables.
- MIGRATIONS_GUIDE.md: Migration workflow for LMS and SimpleLMS.

## Recommended Reading Order

1. Read SECURITY_CONFIGURATION.md and configure local secrets first.
2. Review LMS_PRODUCTION_TEMPLATE_STATUS.md to understand current gaps.
3. Follow IMPLEMENTATION_PLAN.md for execution order.
4. Use IMPLEMENTATION_BACKLOG.md for implementation details.
5. Follow MIGRATIONS_GUIDE.md for every schema change.

## Working Principles

- Never commit real secrets to source control.
- Every new feature should include test coverage.
- Prioritize system reliability before feature breadth.
- Do not scale event-driven features before outbox/retry/idempotency are in place.
