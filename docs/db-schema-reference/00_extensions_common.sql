-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 00_extensions_common.sql
-- Purpose: Extensions, shared schema namespace, common types/triggers used
--          by ALL 14 module files below. Run this FIRST on every tenant DB
--          (db_schema_name = tenant_{uuid}, per Blueprint §A3).
-- Ref: Blueprint B1 (Modular-first), B18 (PostgreSQL 16, RLS, AES-256 PII)
-- ============================================================================

CREATE SCHEMA IF NOT EXISTS hms;
SET search_path TO hms, public;

-- Extensions
CREATE EXTENSION IF NOT EXISTS pgcrypto;     -- gen_random_uuid(), pgp_sym_encrypt (AES-256 PII)
CREATE EXTENSION IF NOT EXISTS pg_trgm;      -- fuzzy name search (patient lookup, duplicate detection)
CREATE EXTENSION IF NOT EXISTS vector;       -- pgvector — embeddings for AI features (B17)
CREATE EXTENSION IF NOT EXISTS btree_gist;   -- exclusion constraints (bed/OT double-booking guards)

-- ---------------------------------------------------------------------------
-- Shared trigger: auto-maintain updated_at on every table that has the column
-- ---------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION hms.set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ---------------------------------------------------------------------------
-- Shared ENUM types (cross-module reuse keeps the catalog consistent)
-- ---------------------------------------------------------------------------
CREATE TYPE hms.gender_type        AS ENUM ('male','female','other','unknown');
CREATE TYPE hms.record_status      AS ENUM ('active','inactive','cancelled','completed','draft');
CREATE TYPE hms.consent_status     AS ENUM ('granted','revoked','expired');
CREATE TYPE hms.payment_status     AS ENUM ('pending','partial','paid','refunded','failed','written_off');
CREATE TYPE hms.payment_method     AS ENUM ('cash','card','upi','neft','rtgs','cheque','insurance','wallet');
CREATE TYPE hms.schedule_drug_flag AS ENUM ('none','H','H1','X','narcotic');
CREATE TYPE hms.priority_level     AS ENUM ('routine','urgent','stat','emergency');
CREATE TYPE hms.ai_review_status   AS ENUM ('ai_suggested','doctor_reviewed','doctor_overridden','doctor_accepted');

-- ---------------------------------------------------------------------------
-- AI Audit Trail (B17 Safety Rules: every AI call logged, override mandatory)
-- Shared by ALL modules that surface an AI suggestion (Doctor, Lab, Pharmacy,
-- Ward, ICU, ER, OT, Blood Bank, Radiology, HR, Patient Portal).
-- ---------------------------------------------------------------------------
CREATE TABLE hms.ai_interaction_log (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    module_name         VARCHAR(50)  NOT NULL,        -- e.g. 'doctor_opd', 'icu_monitoring'
    feature_name        VARCHAR(100) NOT NULL,        -- e.g. 'Diagnosis Assist', 'Sepsis Prediction'
    source_record_table VARCHAR(100),
    source_record_id    UUID,
    input_payload        JSONB,
    ai_suggestion        JSONB,
    confidence_score      NUMERIC(5,4),
    model_name            VARCHAR(100),                -- pinned model version, B17
    model_version          VARCHAR(50),
    review_status         hms.ai_review_status NOT NULL DEFAULT 'ai_suggested',
    reviewed_by_staff_id   UUID,
    override_reason         TEXT,                       -- mandatory if doctor_overridden
    phi_sent_externally      BOOLEAN NOT NULL DEFAULT FALSE,  -- must always be FALSE per safety rule
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_ai_log_module ON hms.ai_interaction_log (module_name, feature_name);
CREATE INDEX idx_ai_log_source ON hms.ai_interaction_log (source_record_table, source_record_id);

COMMENT ON TABLE hms.ai_interaction_log IS 'Mandatory audit trail for every AI-assisted suggestion across all 14 modules — DPDP / CDSCO reproducibility requirement (B17).';
