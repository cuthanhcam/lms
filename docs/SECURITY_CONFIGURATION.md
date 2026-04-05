# Security Configuration for AppSettings

Goal: keep real secrets out of source control.

## Principles

- appsettings.json should be a committable template.
- appsettings.Development.json should only contain non-sensitive local settings.
- Secrets (JWT keys, API keys, passwords) must come from User Secrets or environment variables.

## 1) LMS.API (net8)

Target key:
- JwtSettings:SecretKey

Example commands:

```powershell
cd LMS/src/LMS.API
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:SecretKey" "replace-with-strong-secret"
```

Equivalent environment variable:

```powershell
$env:JwtSettings__SecretKey="replace-with-strong-secret"
```

## 2) SimpleLMS.API (net10)

Target key:
- Jwt:Secret

Example commands:

```powershell
cd SimpleLMS/src/SimpleLMS.API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Secret" "replace-with-strong-secret"
```

Equivalent environment variable:

```powershell
$env:Jwt__Secret="replace-with-strong-secret"
```

## 3) Quick Pre-Push Check

Checklist:
- No real secrets are present in tracked appsettings files.
- No suspicious tokens appear in staged diffs.
- Exposed keys are rotated immediately.

Suggested command:

```powershell
git diff --cached
```

## 4) Team Recommendations

- Define a secret-handling policy in CONTRIBUTING.md.
- Enforce key rotation when exposure is detected.
- In CI/CD, use platform secret stores (for example GitHub Actions Secrets or Azure Key Vault).
