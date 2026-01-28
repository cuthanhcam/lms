namespace LMS.Domain.Common
{
    /// <summary>
    /// Marker interface for Aggregate Roots
    /// 
    /// Aggregate Root is the entry point to an aggregate - a cluster of domain objects that should be treated as a single unit
    /// 
    /// Key principles:
    /// 1. External objects can only hold references to the aggregate root
    /// 2. All changes to objects inside the aggregate must go through the aggregate root
    /// 3. Each aggregate has a consistency boundary - invariants are maintained within the aggregate
    /// 4. References between aggregates should use IDs, not object references
    /// 
    /// Example: Course is an aggregate root for Lessons
    /// - External code cannot directly create or modify Lessons
    /// - Lessons can only be added/modified through the Course aggregate root
    /// - This ensures Course can maintain its invariants (e.g., "published course must have lessons")
    /// </summary>
    public interface IAggregateRoot
    {
    }
}
