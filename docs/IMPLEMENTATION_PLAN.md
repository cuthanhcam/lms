# Implementation Plan (No Fixed Timeline)

Goal: Evolve this repository into a production-style LMS template with strong domain design, reliable event processing, and practical product modules.

## Priority 1: Core Reliability Baseline

Objectives:
- Stabilize lifecycle rules in core domain flows.
- Ensure API behavior is deterministic and testable.

Scope:
- Course lifecycle: Draft -> Review -> Published with strict transition guards.
- Enrollment lifecycle: Active -> Completed -> Dropped with explicit domain invariants.
- Standardize validation and error response contracts.
- Expand integration tests for critical API workflows.

Exit Criteria:
- Invalid state transitions are blocked at domain layer.
- Core workflow tests pass for both success and failure scenarios.

## Priority 2: Event Reliability and Async Processing

Objectives:
- Prevent event loss and duplicate side effects.
- Move side effects to resilient background processing.

Scope:
- Add outbox persistence and publisher process.
- Add retry policy and failed-event handling strategy.
- Add idempotency keys for sensitive commands (for example enrollment and payment).
- Move notifications and report generation to background jobs.

Exit Criteria:
- Successful transactions always create reliable event records.
- Retry operations do not create duplicate business effects.

## Priority 3: Product Value Modules

Objectives:
- Add user-facing modules that increase product realism.

Scope:
- Review and rating.
- Q&A/discussion per lesson.
- Instructor dashboard and admin dashboard read models.
- Caching for course detail and popular courses.

Exit Criteria:
- Modules expose complete API flows with authorization and validation.
- Read models are measured and optimized for common queries.

## Priority 4: Commerce and Extended Capabilities

Objectives:
- Add commerce and advanced capabilities without compromising architecture boundaries.

Scope:
- Payment provider strategy and checkout flow.
- Coupon and discount rules (expiry, limits, fixed/percent).
- Search by title/category/tags.
- Optional recommendation and feature flags.

Exit Criteria:
- Checkout flow is testable end-to-end in a mock provider setup.
- Coupon and payment edge cases are covered by tests.

## Execution Rules

- Implement by dependency order, not by feature popularity.
- Keep each module independently testable.
- Favor small commits with clear scope and verification.
- Require migration checks for every schema change.
