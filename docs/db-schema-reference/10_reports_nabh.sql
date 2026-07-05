-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 10_reports_nabh.sql
-- Module 9: Reports & NABH
-- Ref: Blueprint B11 — NABH quality indicators, MIS dashboards, DPDP audit
--      export, ABDM FHIR bundle export, Excel export, Prometheus/Grafana SLA
-- Requires: 00_extensions_common.sql
-- ============================================================================

SET search_path TO hms, public;

-- ---------------------------------------------------------------------------
-- NABH quality indicator definitions + logged measurements
-- ---------------------------------------------------------------------------
CREATE TABLE hms.nabh_indicator_definitions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    indicator_code  VARCHAR(30) UNIQUE NOT NULL,
    indicator_name  VARCHAR(200) NOT NULL,        -- 'Door-to-Triage Time', 'Rx-to-Dispensing Time', 'ICU Response Time'
    target_value    NUMERIC(10,2),
    unit            VARCHAR(30),                   -- minutes/percentage/count
    source_module   VARCHAR(30)
);

CREATE TABLE hms.nabh_indicator_logs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    indicator_id    UUID NOT NULL REFERENCES hms.nabh_indicator_definitions(id),
    measured_value  NUMERIC(10,2) NOT NULL,
    measurement_period_start DATE NOT NULL,
    measurement_period_end   DATE NOT NULL,
    source_record_id UUID,
    logged_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_nabh_logs_indicator ON hms.nabh_indicator_logs (indicator_id, measurement_period_start);

-- ---------------------------------------------------------------------------
-- Saved report definitions (MIS dashboards) and generated exports
-- ---------------------------------------------------------------------------
CREATE TABLE hms.report_definitions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    report_name     VARCHAR(150) NOT NULL,
    report_type     VARCHAR(30) NOT NULL,          -- 'mis','nabh','financial','clinical'
    query_definition JSONB,
    created_by      UUID,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE hms.report_exports (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    report_definition_id UUID REFERENCES hms.report_definitions(id),
    export_format   VARCHAR(20) NOT NULL,          -- 'excel','pdf','fhir_bundle','csv'
    file_url        TEXT,
    requested_by    UUID,
    requested_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at    TIMESTAMPTZ
);

-- ---------------------------------------------------------------------------
-- DPDP audit export log (separate from superadmin.audit_logs — this is the
-- tenant-side clinical/operational audit trail)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.dpdp_audit_log (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    actor_staff_id  UUID,
    action          VARCHAR(100) NOT NULL,         -- 'VIEW_PATIENT_RECORD','EXPORT_DATA','CONSENT_REVOKED', etc.
    target_table    VARCHAR(100),
    target_record_id UUID,
    patient_id      UUID,
    ip_address      INET,
    occurred_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_dpdp_audit_patient ON hms.dpdp_audit_log (patient_id);
REVOKE UPDATE, DELETE ON hms.dpdp_audit_log FROM PUBLIC;

-- ---------------------------------------------------------------------------
-- ABDM FHIR R4 bundle export tracking
-- ---------------------------------------------------------------------------
CREATE TABLE hms.fhir_export_log (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id      UUID NOT NULL,
    resource_type   VARCHAR(50),                   -- Patient/Encounter/Observation/MedicationRequest
    bundle_id       VARCHAR(100),
    pushed_to_abha  BOOLEAN NOT NULL DEFAULT FALSE,
    pushed_at       TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.dpdp_audit_log IS 'Tenant-side append-only clinical audit trail (Module 9, B11/B19) — distinct from platform-level superadmin.audit_logs.';
