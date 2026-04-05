# Migration Guide for LMS and SimpleLMS

This document standardizes migration creation and commit practices to avoid model snapshot drift.

## Scope

- LMS migration folder: LMS/src/LMS.Infrastructure/Migrations
- SimpleLMS migration folder: SimpleLMSư/src/SimpleLMS.Infrastructure/Migrations

## LMS Workflow

```powershell
cd LMS
dotnet ef migrations add <MigrationName> --project .\src\LMS.Infrastructure\LMS.Infrastructure.csproj --startup-project .\src\LMS.API\LMS.API.csproj
dotnet ef database update --project .\src\LMS.Infrastructure\LMS.Infrastructure.csproj --startup-project .\src\LMS.API\LMS.API.csproj
```

## SimpleLMS Workflow

```powershell
cd SimpleLMS
dotnet ef migrations add <MigrationName> --project .\src\SimpleLMS.Infrastructure\SimpleLMS.Infrastructure.csproj --startup-project .\src\SimpleLMS.API\SimpleLMS.API.csproj
dotnet ef database update --project .\src\SimpleLMS.Infrastructure\SimpleLMS.Infrastructure.csproj --startup-project .\src\SimpleLMS.API\SimpleLMS.API.csproj
```

## Pre-Commit Migration Checklist

- Migration file, designer file, and AppDbContextModelSnapshot are updated together.
- Relevant solution build succeeds.
- Database-related integration tests pass.
- No sensitive local database configuration is committed.

## When to Split Migrations

- Split large schema changes for easier review and rollback.
- Avoid mixing unrelated modules in one migration.

## Migration Review Checklist

- Verify foreign keys, indexes, and delete behavior.
- Verify data migration scripts when changing column types.
- Check backward compatibility for staging and deployment scenarios.
