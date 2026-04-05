# Implementation Backlog

This document is a task-level checklist for implementation and verification.

## Epic 1: Identity and Access Hardening

Tasks:
- Add email verification flow.
- Add forgot/reset password flow.
- Add refresh token rotation and revoke mechanism.
- Expand policy-based authorization for administrative endpoints.

Acceptance Criteria:
- Old refresh tokens are invalid after rotation.
- Role and policy tests protect critical endpoints.

Tests:
- Unit tests for token service behavior.
- Integration tests for login/refresh/revoke flows.

## Epic 2: Course Lifecycle Standardization

Tasks:
- Model a course state machine.
- Add admin review queue before publishing.
- Add publish guard (minimum content requirements).

Acceptance Criteria:
- Courses without required content cannot be published.
- Only authorized roles can execute lifecycle transitions.

Tests:
- Domain tests for transition matrix.
- API tests for publish/unpublish/review flows.

## Epic 3: Enrollment and Progress Reliability

Tasks:
- Standardize progress update flow (lesson completion to percentage).
- Add resume learning pointer.
- Add enrollment completion event handling.

Acceptance Criteria:
- Progress never exceeds 100%.
- Last viewed lesson is consistently updated.

Tests:
- Domain tests for progress calculations.
- Integration tests for lesson completion flow.

## Epic 4: Notification and Event Pipeline

Tasks:
- Implement outbox entity and migration.
- Implement outbox publisher worker.
- Implement notification handlers (in-app and email adapter).

Acceptance Criteria:
- Events are not lost when API transaction succeeds.
- Handler failures are retried and logged.

Tests:
- Integration tests for outbox write/read/publish lifecycle.
- Failure scenario tests for retry behavior.

## Epic 5: Observability and Operations

Tasks:
- Add structured logging and correlation-id middleware.
- Add readiness and liveness health checks.
- Add core metrics (request count, failures, outbox pending).

Acceptance Criteria:
- A single request can be traced across logs by correlation id.
- Health endpoints clearly represent system state.

Tests:
- Smoke tests for health endpoints.

## Epic 6: Commerce Foundations

Tasks:
- Implement payment provider strategy abstraction.
- Implement checkout mock flow.
- Implement coupon rules (expiry, usage limits, discount type).

Acceptance Criteria:
- Discount logic handles edge cases correctly.
- Failed payment does not grant enrollment.

Tests:
- Unit tests for coupon calculation rules.
- Integration tests for checkout workflow.

## Epic 7: Reviews and Engagement

Tasks:
- Implement review and rating aggregate.
- Implement instructor reply capability.
- Implement sort and filter options for reviews.

Acceptance Criteria:
- One student can only own one review per course (with update policy).
- Aggregate rating is updated correctly after edit/delete.

Tests:
- Domain tests for aggregate rating behavior.
- API tests for review authorization and CRUD.

## Epic 8: Analytics Query Layer

Tasks:
- Build read models for instructor and admin dashboards.
- Add completion rate and drop-off reports.
- Optimize query performance and add indexes where needed.

Acceptance Criteria:
- Dashboard latency is acceptable under representative sample data.
- Reported metrics are consistent with source data.

Tests:
- Integration tests for reporting queries.
- Data sanity checks with seeded datasets.

## Suggested Commit Types

- feat: new functional capability.
- fix: bug fix.
- refactor: structure changes without behavior changes.
- test: test additions or updates.
- docs: documentation updates.
- chore: migrations, infrastructure, or configuration.
