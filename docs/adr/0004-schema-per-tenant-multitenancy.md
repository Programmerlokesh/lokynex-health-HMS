# ADR 0004: Multi-Tenancy via Schema-per-Tenant (Supersedes ADR 0002)

## Context
Original ADR 0002 chose shared-tables + RLS for MVP simplicity. After importing
a complete 14-module schema pack (MediCore HMS blueprint), the decision has
been revised to schema-per-tenant for stronger isolation, matching the
blueprint's architecture (§A3).

## Decision
- One **Platform database** holds a `superadmin` schema (Tenant, Subscription,
  Billing, Module catalog, global audit log). No clinical data here.
- Each tenant gets its own PostgreSQL **schema** (`tenant_{uuid}`) containing
  the full `hms` schema table set (Patient, Doctor, OPD, Billing, Pharmacy,
  Ward, etc.)
- Tenant schema creation/migration is automated via a provisioning service
  triggered on tenant onboarding (SuperAdmin action).

## Consequences
+ Strong physical isolation between tenants
+ Matches the full blueprint exactly — future modules slot in without rework
- Higher infra complexity: dynamic schema creation, per-tenant migration runner
- Requires a "Platform" EF Core DbContext separate from the "Tenant" DbContext
- Existing Day 1-23 shared-table code needs migration to this model

## Status
ADR 0002 is superseded by this decision.
