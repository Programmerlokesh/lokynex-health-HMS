# ADR 0002: Multi-Tenancy via PostgreSQL Row-Level Security (RLS)

## Context
Lokynex Health is multi-tenant — many hospitals share the same database.
Options considered: separate DB per tenant, separate schema per tenant,
or shared tables with a tenant_id + Row-Level Security.

## Decision
Use shared tables + PostgreSQL RLS, with tenant_id on every tenant-scoped table.

## Consequences
+ Simple to manage at MVP scale (one DB, one migration path)
+ Postgres enforces isolation at the DB level, reducing app-layer bugs
- Requires discipline: every query must be tenant-aware; RLS policies must be tested
- Will need re-evaluation if a very large enterprise tenant demands full isolation