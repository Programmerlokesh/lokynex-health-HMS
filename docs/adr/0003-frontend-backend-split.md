# ADR 0003: Separate Next.js Frontend and .NET Core Backend

## Context
Need a modern, fast UI (multiple dashboards: SuperAdmin, Doctor, Pharmacy, etc.)
and a robust, strongly-typed backend for healthcare data and business rules.

## Decision
Next.js 14 (TypeScript) as a separate frontend calling a .NET Core 8 Web API
over REST, rather than a monolithic server-rendered app.

## Consequences
+ Frontend and backend can be developed, tested, deployed independently
+ .NET's strong typing and Clean Architecture fit backend healthcare logic well
- Requires CORS setup, API contract discipline, and separate deployment pipelines