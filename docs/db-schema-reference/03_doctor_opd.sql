-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 03_doctor_opd.sql
-- Module 1: Doctor / OPD
-- Ref: Blueprint B3 — NMC e-Prescription, ICD-10, drug interaction checker,
--      AI SOAP notes, ABHA PHR push, OPD token queue
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

-- ---------------------------------------------------------------------------
-- Staff master (lightweight here; full HR profile lives in Module 13)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.doctors (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id         UUID,                              -- FK to hms.employees (Module 13), nullable cross-module link
    full_name           VARCHAR(150) NOT NULL,
    nmc_registration_no VARCHAR(50) UNIQUE NOT NULL,
    specialization       VARCHAR(100),
    aadhaar_pki_cert_ref  TEXT,                              -- reference to eSign certificate
    is_active             BOOLEAN NOT NULL DEFAULT TRUE,
    created_at             TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- OPD token queue
-- ---------------------------------------------------------------------------
CREATE TABLE hms.opd_token_queue (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    token_number    VARCHAR(20) NOT NULL,
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    doctor_id       UUID NOT NULL REFERENCES hms.doctors(id),
    queue_date      DATE NOT NULL DEFAULT CURRENT_DATE,
    status          VARCHAR(20) NOT NULL DEFAULT 'waiting',  -- waiting/in_consultation/done/no_show
    checked_in_at   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    called_at       TIMESTAMPTZ,
    UNIQUE (doctor_id, queue_date, token_number)
);
CREATE INDEX idx_opd_queue_date ON hms.opd_token_queue (queue_date, doctor_id);

-- ---------------------------------------------------------------------------
-- OPD encounter / visit
-- ---------------------------------------------------------------------------
CREATE TABLE hms.opd_visits (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    visit_number    VARCHAR(30) UNIQUE NOT NULL,
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    doctor_id       UUID NOT NULL REFERENCES hms.doctors(id),
    token_id        UUID REFERENCES hms.opd_token_queue(id),
    scheme_tag      VARCHAR(20),                              -- CGHS/PM-JAY/ESIC/self-pay
    chief_complaint TEXT,
    visit_started_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    visit_ended_at   TIMESTAMPTZ,
    status            hms.record_status NOT NULL DEFAULT 'active',
    signed_off          BOOLEAN NOT NULL DEFAULT FALSE,         -- Aadhaar PKI eSign
    signed_off_at        TIMESTAMPTZ,
    esign_qr_code_ref     TEXT
);
CREATE INDEX idx_opd_visits_patient ON hms.opd_visits (patient_id);
CREATE INDEX idx_opd_visits_doctor_date ON hms.opd_visits (doctor_id, visit_started_at);

-- ---------------------------------------------------------------------------
-- Diagnoses (ICD-10-CM, AI-assisted)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.opd_diagnoses (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    visit_id        UUID NOT NULL REFERENCES hms.opd_visits(id) ON DELETE CASCADE,
    icd10_code      VARCHAR(10) NOT NULL,
    icd10_description TEXT,
    is_primary      BOOLEAN NOT NULL DEFAULT FALSE,
    ai_suggested    BOOLEAN NOT NULL DEFAULT FALSE,           -- AI Diagnosis Assist (B3)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- SOAP notes (voice-dictated, AI auto-drafted, Hindi/English)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.opd_soap_notes (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    visit_id        UUID NOT NULL REFERENCES hms.opd_visits(id) ON DELETE CASCADE,
    subjective      TEXT,
    objective       TEXT,
    assessment      TEXT,
    plan            TEXT,
    source_language VARCHAR(10) DEFAULT 'en',
    ai_drafted      BOOLEAN NOT NULL DEFAULT FALSE,
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    finalized_by_doctor BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Prescriptions
-- ---------------------------------------------------------------------------
CREATE TABLE hms.prescriptions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    prescription_number VARCHAR(30) UNIQUE NOT NULL,
    visit_id        UUID NOT NULL REFERENCES hms.opd_visits(id),
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    doctor_id       UUID NOT NULL REFERENCES hms.doctors(id),
    issued_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    pushed_to_abha  BOOLEAN NOT NULL DEFAULT FALSE,
    pushed_to_pharmacy_queue BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE hms.prescription_items (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    prescription_id UUID NOT NULL REFERENCES hms.prescriptions(id) ON DELETE CASCADE,
    drug_name       VARCHAR(200) NOT NULL,
    rxnorm_code     VARCHAR(30),
    dosage          VARCHAR(50),                  -- e.g. '500mg'
    frequency       VARCHAR(50),                  -- e.g. 'BD','TDS'
    duration_days   SMALLINT,
    schedule_flag   hms.schedule_drug_flag NOT NULL DEFAULT 'none',
    interaction_checked BOOLEAN NOT NULL DEFAULT FALSE,
    interaction_warning TEXT,
    allergy_warning     TEXT,
    ai_autocompleted     BOOLEAN NOT NULL DEFAULT FALSE,        -- Rx Autocomplete (B3)
    ai_log_id              UUID REFERENCES hms.ai_interaction_log(id)
);
CREATE INDEX idx_rx_items_prescription ON hms.prescription_items (prescription_id);

-- ---------------------------------------------------------------------------
-- Investigation orders (links to Lab / Radiology modules)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.opd_investigation_orders (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    visit_id        UUID NOT NULL REFERENCES hms.opd_visits(id) ON DELETE CASCADE,
    order_type      VARCHAR(20) NOT NULL,          -- 'lab' or 'radiology'
    test_or_study_name VARCHAR(200) NOT NULL,
    priority        hms.priority_level NOT NULL DEFAULT 'routine',
    ordered_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    -- fulfilled_reference_id points to hms.lab_orders.id or hms.radiology_orders.id (cross-module, no hard FK)
    fulfilled_reference_id UUID
);

COMMENT ON TABLE hms.opd_visits IS 'Doctor/OPD core encounter table (Module 1, B3). Flow: Patient Context -> Diagnosis -> Medication Orders -> Investigations -> Sign-off.';
