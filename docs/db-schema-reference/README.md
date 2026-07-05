# MediCore HMS — Database Schema Pack (India Edition v3.0)

PostgreSQL 16 schema set generated from the *MediCore HMS Complete SaaS Platform Blueprint*. Covers the SuperAdmin/Owner control layer (Part A) and all 14 Hospital Operating modules (Part B).

## Architecture

The platform is **multi-tenant, schema-per-tenant**:

- **One platform database** holds the `superadmin` schema — tenants, billing, module licensing, global audit log. SuperAdmin never sees clinical data (DPDP hard rule, §A12).
- **One PostgreSQL schema per tenant** (`tenant_{uuid}`, per §A3) holds the `hms` schema with all 14 module tables, isolated via Row-Level Security per the blueprint's RLS tagging.

Within a tenant schema, tables stay **independent per module** (own primary keys, own lifecycle) but link to a shared **patient master** (Module 7) and **employee master** (Module 13), matching the "Modular-first" principle in §B1.

## File order (dependencies matter — install in this sequence)

| # | File | Module | Depends on |
|---|------|--------|------------|
| 00 | `00_extensions_common.sql` | Shared extensions, enums, AI audit log | — |
| 01 | `01_platform_superadmin.sql` | Part A — SuperAdmin/Owner | Platform DB only |
| 02 | `02_patient_registration.sql` | Module 7 — Patient Registration | 00 |
| 03 | `03_doctor_opd.sql` | Module 1 — Doctor/OPD | 02 |
| 04 | `04_laboratory.sql` | Module 2 — Laboratory | 02 |
| 05 | `05_pharmacy_pos.sql` | Module 3 — Pharmacy POS | 02 |
| 06 | `06_ward_bed_management.sql` | Module 4 — Ward & Bed | 02 |
| 07 | `07_icu_monitoring.sql` | Module 5 — ICU Monitoring | 06 (reuses beds/admissions) |
| 08 | `08_emergency_er.sql` | Module 6 — Emergency/ER | 02 |
| 09 | `09_billing_finance.sql` | Module 8 — Billing & Finance | 06 |
| 10 | `10_reports_nabh.sql` | Module 9 — Reports & NABH | 00 |
| 11 | `11_ot_management.sql` | Module 10 — OT Management 🆕 | 06 |
| 12 | `12_blood_bank.sql` | Module 11 — Blood Bank 🆕 | 02 |
| 13 | `13_radiology_pacs.sql` | Module 12 — Radiology/PACS 🆕 | 02, 03 |
| 14 | `14_hr_payroll.sql` | Module 13 — HR & Payroll 🆕 | 00 (universal) |
| 15 | `15_patient_portal_telemedicine.sql` | Module 14 — Patient Portal & Tele 🆕 | 02, 03, 09 |
| 99 | `99_master_run_all.sql` | Runs all of the above in order | — |

Run with `psql -f 99_master_run_all.sql` (or apply each file individually via your migration tool — Flyway/Liquibase/EF Core migrations).

## Cross-module reference map

This mirrors the **Data Flow Architecture** in §B20 of the blueprint:

- `hms.patients` (Module 7) is the hub — every clinical module's tables reference `patient_id`.
- `hms.opd_visits.id` → `hms.opd_investigation_orders.fulfilled_reference_id` → `hms.lab_orders.id` / `hms.radiology_orders.id` (Doctor writes order → Lab/Radiology fulfills it).
- `hms.admissions` (Module 4) is shared by ICU (Module 5), OT post-op transfer (Module 10), and room-charge billing (Module 8).
- `hms.ot_bookings` (Module 10) can trigger `hms.blood_crossmatch_requests` (Module 11) for surgical blood needs.
- `hms.billing_invoice_items.source_module` + `source_record_id` is a soft polymorphic link — every module's billable event (OPD consult, lab test, pharmacy sale, room-day, OT procedure, blood unit, radiology study) lands here as one line item, consolidated per §B10.
- `hms.employees` (Module 13) is the staff hub — `hms.doctors.employee_id`, `hms.ot_staff_allocation.staff_id`, etc. point back to it.
- `hms.ai_interaction_log` (shared, file 00) is referenced by every AI-assisted feature across all 14 modules — mandatory per the B17 safety rule ("every AI call audited, override reason mandatory").

## Design notes

- **PII encryption**: Aadhaar numbers and bank account numbers are stored as `BYTEA` (intended for `pgp_sym_encrypt`/`pgcrypto`), never plaintext — IT Act 2000 §43A.
- **Append-only audit tables**: `superadmin.audit_logs` and `hms.dpdp_audit_log` have `UPDATE`/`DELETE` revoked from `PUBLIC` to enforce the "undeletable" rule (§A12).
- **Conflict-free OT scheduling**: `hms.ot_bookings` uses a PostgreSQL `EXCLUDE USING gist` constraint on `(ot_room_id, time_range)` so the database itself rejects overlapping bookings — not just application logic.
- **ICU vitals**: modeled as a plain time-series table here; in production this should be converted to a TimescaleDB hypertable (per §B17/B7, vitals stream via Kafka).
- **Enums vs. lookup tables**: stable, small value sets (gender, payment method, ESI triage level) use native PostgreSQL `ENUM` types for integrity; open-ended catalogs (drugs, lab tests, ICD-10 codes) are regular tables so they can grow without schema migrations.
- Every table that needs it has an `updated_at` trigger (`hms.set_updated_at()`) wired via `CREATE TRIGGER ... EXECUTE FUNCTION` — add this trigger to any table you extend with an `updated_at` column.

## What's NOT in these files

Auth/session tables (handled by your identity provider / JWT layer per §B21), Row-Level Security *policies* (the blueprint specifies RLS isolation but policy definitions depend on your tenant-claim strategy), and full-text/DICOM storage internals (PACS stores actual pixel data; this schema only holds metadata pointers).
