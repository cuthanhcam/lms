# LMS Production Template Status

This document maps a full-scale LMS scope and tracks what is already implemented, partially implemented, and still required.

Status labels:
- Done: implemented and usable.
- Partial: foundation exists but not production-complete.
- Not Started: module not implemented yet.

## I. Core Domain (Must Have)

### 1) Identity and Access Management

Status: Partial

Current:
- Register/login flows exist.
- JWT-based authentication and role policies exist.

Missing:
- Refresh token rotation and revoke strategy hardening.
- Email verification.
- Forgot/reset password.
- User profile management.
- Optional: 2FA and OAuth provider support.

Patterns:
- Strategy for auth providers.
- Specification for authorization rules.

Next Actions:
- Introduce token family model (rotation and revoke).
- Add verification/reset workflows and API tests.

### 2) Course Management

Status: Partial

Current:
- Core course CRUD and publish/unpublish behavior exists.

Missing:
- Strict Draft -> Review -> Published workflow.
- Visibility policy matrix (public/private/unlisted) validation.
- Versioning strategy.
- Admin approval workflow.

Patterns:
- State pattern for lifecycle.
- Aggregate root boundary for course.

Next Actions:
- Formalize lifecycle transitions in domain.
- Add review queue and policy checks.

### 3) Lesson and Content System

Status: Partial

Current:
- Lesson management and ordering foundation exists.

Missing:
- Explicit lesson-type behavior model (video/article/quiz).
- Section/chapter composition hardening.
- Drip content and preview access control.

Patterns:
- Composite for course -> section -> lesson.
- Strategy for lesson-type behavior.

Next Actions:
- Add section aggregate and lesson type strategy interfaces.

### 4) Enrollment System

Status: Partial

Current:
- Enrollment flow and completion flow exist.
- Enrollment-related domain events exist.

Missing:
- Strict enrollment state transitions.
- Resume learning flow hardening.
- Idempotency for enroll command.

Patterns:
- State pattern.
- Domain events.

Next Actions:
- Add transition guards and duplicate-enroll prevention.

### 5) Progress Tracking

Status: Partial

Current:
- Progress percentage and completion handling exist.

Missing:
- Last viewed lesson consistency rules.
- Time spent tracking.
- Learning streak.

Next Actions:
- Introduce progress metrics model and history snapshots.

## II. Commerce

### 6) Payment System

Status: Not Started

Scope:
- Checkout, payment history, refund support.
- Strategy pattern for payment provider adapters.

Next Actions:
- Start with provider abstraction and mock implementation.

### 7) Coupon and Discount

Status: Not Started

Scope:
- Coupon code, fixed/percent discount, expiration, usage limits.

Next Actions:
- Implement coupon rule engine with edge-case tests.

## III. Engagement

### 8) Review and Rating

Status: Not Started

Scope:
- Course rating and comments.
- Instructor replies and sorting/filtering.

Next Actions:
- Create review aggregate with one-review-per-student policy.

### 9) Q&A and Discussion

Status: Not Started

Scope:
- Lesson-level questions and answers.
- Upvotes and accepted answers.

Next Actions:
- Design threaded discussion model and moderation rules.

### 10) Notification System

Status: Partial

Current:
- Domain event handlers exist as integration points.

Missing:
- Real notification channels (email/in-app).
- Retry policy and delivery tracking.

Patterns:
- Observer via domain events.

Next Actions:
- Implement notification outbox consumers and channel adapters.

## IV. Analytics and Reporting

### 11) Instructor Dashboard

Status: Not Started

Scope:
- Student count, revenue, course performance.

### 12) Admin Dashboard

Status: Not Started

Scope:
- Global users, active courses, revenue stats.

### 13) Learning Analytics

Status: Not Started

Scope:
- Completion rate, drop-off points.

Next Actions:
- Build read-model projections optimized for reporting.

## V. System and Architecture

### 14) CQRS

Status: Partial

Current:
- Layer separation and use-case-oriented services exist.

Missing:
- Explicit command/query split with dedicated handlers/read models.

Next Actions:
- Introduce command and query contracts for critical flows first.

### 15) Domain Events

Status: Partial

Current:
- Event base classes, dispatcher, and handlers exist.

Missing:
- Reliable delivery guarantees and monitoring.

Next Actions:
- Integrate outbox-backed event publication.

### 16) Outbox Pattern

Status: Not Started

Next Actions:
- Add outbox table, writer, publisher worker, and retry tracking.

### 17) Background Jobs

Status: Not Started

Scope:
- Email sending, payment processing callbacks, report generation.

Next Actions:
- Introduce job processor and standard retry policy.

### 18) Caching

Status: Not Started

Scope:
- Course detail and popular courses.

Next Actions:
- Add distributed cache abstraction and invalidation strategy.

### 19) API Versioning

Status: Done

Current:
- API versioning convention is already present in the API setup.

### 20) File Storage

Status: Not Started

Scope:
- Video upload and thumbnail upload.
- Local provider and cloud provider compatibility.

Next Actions:
- Define storage provider interface and secure upload pipeline.

## VI. Security

Status: Partial

Current:
- Input validation and role/policy authorization foundations exist.

Missing:
- Rate limiting hardening.
- Centralized audit log for sensitive operations.
- Security event monitoring.

Next Actions:
- Add API throttling and audit trail for admin/instructor operations.

## VII. Testing

Status: Partial

Current:
- Unit and integration tests exist across LMS and SimpleLMS.

Missing:
- API contract tests.
- Testcontainers-based environment parity for database integration.

Next Actions:
- Add contract test suite for external API stability.
- Add Testcontainers for reliable local/CI integration runs.

## VIII. Advanced (Optional)

### 21) Modular Monolith

Status: Not Started

### 22) Search System

Status: Not Started

### 23) Recommendation System

Status: Not Started

### 24) Feature Flags

Status: Not Started

Next Actions:
- Prioritize only after core reliability modules are complete.

## Practical Scope Recommendation

Must Have:
- Identity hardening.
- Course and lesson lifecycle.
- Enrollment and progress reliability.
- Review module.
- Notification module.
- CQRS and domain events with production-grade reliability.

Should Have:
- Payment and coupon.
- Analytics dashboards.
- Background jobs.
- Outbox.

Nice to Have:
- Recommendation.
- Feature flags.
- Advanced search.

## Key Insight

A production-grade LMS is not defined by feature count.
It is defined by:
- Clear domain model.
- Consistent workflow rules.
- Clean architecture boundaries.
- Reliable event-driven behavior.
