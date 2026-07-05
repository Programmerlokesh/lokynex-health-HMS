-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 04_laboratory.sql
-- Module 2: Laboratory
-- Ref: Blueprint B4 — NABL workflow, LOINC mapping, QR specimen barcodes,
--      HL7 v2 analyser integration, critical-value alerts, CGHS dual pricing
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

CREATE TYPE hms.lab_order_status AS ENUM ('ordered','sample_collected','in_analysis','validated','released','cancelled');

-- ---------------------------------------------------------------------------
-- Test catalog (LOINC mapped), with CGHS dual pricing
-- ---------------------------------------------------------------------------
CREATE TABLE hms.lab_test_catalog (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    test_code       VARCHAR(30) UNIQUE NOT NULL,
    test_name       VARCHAR(200) NOT NULL,
    loinc_code      VARCHAR(20),
    specimen_type   VARCHAR(50),                  -- blood/urine/swab etc.
    nabl_panel      VARCHAR(100),
    standard_price  NUMERIC(10,2) NOT NULL,
    cghs_price      NUMERIC(10,2),
    tat_hours_std   NUMERIC(5,2),                 -- standard turnaround estimate
    is_active       BOOLEAN NOT NULL DEFAULT TRUE
);

-- ---------------------------------------------------------------------------
-- Lab order (header) — order -> collect -> analyse -> validate -> release
-- ---------------------------------------------------------------------------
CREATE TABLE hms.lab_orders (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_number    VARCHAR(30) UNIQUE NOT NULL,           -- LAB-2026-008741
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    ordering_doctor_id UUID REFERENCES hms.doctors(id),
    source_visit_id UUID,                                  -- opd_visits.id / er_visits.id (cross-module)
    status          hms.lab_order_status NOT NULL DEFAULT 'ordered',
    priority        hms.priority_level NOT NULL DEFAULT 'routine',
    scheme_tag      VARCHAR(20),                            -- CGHS pricing trigger
    ai_panel_suggested BOOLEAN NOT NULL DEFAULT FALSE,       -- Smart Panel Suggestion (B4)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    ordered_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    released_at     TIMESTAMPTZ
);
CREATE INDEX idx_lab_orders_patient ON hms.lab_orders (patient_id);
CREATE INDEX idx_lab_orders_status ON hms.lab_orders (status);

CREATE TABLE hms.lab_order_tests (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id        UUID NOT NULL REFERENCES hms.lab_orders(id) ON DELETE CASCADE,
    test_id         UUID NOT NULL REFERENCES hms.lab_test_catalog(id),
    price_applied   NUMERIC(10,2) NOT NULL
);

-- ---------------------------------------------------------------------------
-- Specimens (QR-barcoded)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.lab_samples (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id        UUID NOT NULL REFERENCES hms.lab_orders(id) ON DELETE CASCADE,
    sample_barcode  VARCHAR(50) UNIQUE NOT NULL,           -- QR-encoded label
    collected_by    UUID,                                  -- staff id
    collected_at    TIMESTAMPTZ,
    received_at_lab TIMESTAMPTZ,
    rejected        BOOLEAN NOT NULL DEFAULT FALSE,
    rejection_reason TEXT
);

-- ---------------------------------------------------------------------------
-- Results
-- ---------------------------------------------------------------------------
CREATE TABLE hms.lab_results (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_test_id   UUID NOT NULL REFERENCES hms.lab_order_tests(id) ON DELETE CASCADE,
    sample_id       UUID REFERENCES hms.lab_samples(id),
    parameter_name  VARCHAR(150) NOT NULL,
    result_value    VARCHAR(100),
    unit            VARCHAR(30),
    reference_range VARCHAR(100),
    is_critical     BOOLEAN NOT NULL DEFAULT FALSE,
    is_abnormal     BOOLEAN NOT NULL DEFAULT FALSE,
    entered_by      UUID,
    validated_by    UUID,
    validated_at    TIMESTAMPTZ,
    source          VARCHAR(20) DEFAULT 'manual',          -- 'manual' or 'hl7_analyser'
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_lab_results_order_test ON hms.lab_results (order_test_id);

-- ---------------------------------------------------------------------------
-- Critical-value SMS alerts
-- ---------------------------------------------------------------------------
CREATE TABLE hms.lab_critical_alerts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    result_id       UUID NOT NULL REFERENCES hms.lab_results(id),
    notified_doctor_id UUID REFERENCES hms.doctors(id),
    notified_via    VARCHAR(20) DEFAULT 'sms',
    ai_pre_alert    BOOLEAN NOT NULL DEFAULT FALSE,         -- Critical Value Prediction (B4)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    sent_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    acknowledged_at TIMESTAMPTZ
);

COMMENT ON TABLE hms.lab_orders IS 'NABL workflow: order -> sample_collected -> in_analysis -> validated -> released (Module 2, B4).';
