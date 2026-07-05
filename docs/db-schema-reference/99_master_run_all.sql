-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 99_master_run_all.sql
-- Purpose: Run every module file IN ORDER on a single database/schema.
--          Use this for local dev/testing. In production, 01_platform_*
--          runs once on the control-plane DB, while 00 + 02-15 are applied
--          to EACH new tenant's own schema (tenant_{uuid}) during onboarding
--          (Blueprint §A3 — "DB Schema: tenant_{uuid}").
-- ============================================================================

\echo 'Installing MediCore HMS schema pack...'

\i 00_extensions_common.sql          -- Run on platform DB (shared) AND every tenant schema
\i 01_platform_superadmin.sql        -- PLATFORM DB ONLY — Part A, owner/control layer

-- The following run inside EACH TENANT schema (Part B, 14 modules):
\i 02_patient_registration.sql       -- Module 7  (universal entry point — install first)
\i 03_doctor_opd.sql                 -- Module 1
\i 04_laboratory.sql                 -- Module 2
\i 05_pharmacy_pos.sql               -- Module 3
\i 06_ward_bed_management.sql        -- Module 4
\i 07_icu_monitoring.sql             -- Module 5  (depends on Module 4: beds/admissions)
\i 08_emergency_er.sql               -- Module 6
\i 09_billing_finance.sql            -- Module 8  (depends on Module 4: admissions/beds)
\i 10_reports_nabh.sql               -- Module 9
\i 11_ot_management.sql              -- Module 10 (depends on Module 4: ward transfer)
\i 12_blood_bank.sql                 -- Module 11
\i 13_radiology_pacs.sql             -- Module 12
\i 14_hr_payroll.sql                 -- Module 13 (universal — Doctor/OT/etc. reference employees)
\i 15_patient_portal_telemedicine.sql -- Module 14 (depends on Module 8: billing_invoices)

\echo 'Schema pack installed successfully — 14 modules + platform layer.'
