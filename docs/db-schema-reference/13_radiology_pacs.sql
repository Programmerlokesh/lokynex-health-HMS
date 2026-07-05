-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 13_radiology_pacs.sql
-- Module 12: Radiology / PACS (NEW)
-- Ref: Blueprint B14 — DICOM viewer (Cornerstone.js/OHIF), PACS/RIS, structured
--      reporting, teleradiology, AERB radiation-safety dose logs
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

-- ---------------------------------------------------------------------------
-- Imaging orders (from Doctor/ER modules) -> Modality Worklist -> Acquisition
-- ---------------------------------------------------------------------------
CREATE TABLE hms.radiology_orders (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_number    VARCHAR(30) UNIQUE NOT NULL,
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    ordering_doctor_id UUID REFERENCES hms.doctors(id),
    source_module   VARCHAR(20),                   -- 'opd','er'
    source_record_id UUID,
    modality        VARCHAR(20) NOT NULL,          -- 'XR','CT','MRI','USG','MAMMO'
    study_description VARCHAR(200),
    clinical_indication TEXT,                       -- mandatory for USG orders (PCPNDT Act)
    priority        hms.priority_level NOT NULL DEFAULT 'routine',
    status          VARCHAR(20) NOT NULL DEFAULT 'ordered',  -- ordered/worklisted/acquired/reported/signed_off
    ordered_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_radiology_orders_patient ON hms.radiology_orders (patient_id);

CREATE TABLE hms.radiology_modality_worklist (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id        UUID NOT NULL REFERENCES hms.radiology_orders(id) ON DELETE CASCADE,
    scheduled_at    TIMESTAMPTZ,
    modality_aet    VARCHAR(50),                   -- AE Title of imaging device
    technician_id   UUID,
    status          VARCHAR(20) NOT NULL DEFAULT 'scheduled'
);

-- ---------------------------------------------------------------------------
-- DICOM studies (PACS storage reference — actual pixel data stored in PACS,
-- this table holds metadata/pointers)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.radiology_dicom_studies (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id        UUID NOT NULL REFERENCES hms.radiology_orders(id) ON DELETE CASCADE,
    study_instance_uid VARCHAR(100) UNIQUE NOT NULL,
    pacs_storage_url TEXT NOT NULL,
    series_count    SMALLINT,
    image_count     INTEGER,
    acquired_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Structured radiology report + AI abnormality flagging
-- ---------------------------------------------------------------------------
CREATE TABLE hms.radiology_reports (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id        UUID NOT NULL REFERENCES hms.radiology_orders(id) ON DELETE CASCADE,
    dicom_study_id  UUID REFERENCES hms.radiology_dicom_studies(id),
    radiologist_id  UUID,
    findings        TEXT,
    impression      TEXT,
    ai_preliminary_findings TEXT,                  -- Abnormality Flagging, e.g. CXR/CT (B14)
    ai_critical_finding BOOLEAN NOT NULL DEFAULT FALSE,
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    is_teleradiology BOOLEAN NOT NULL DEFAULT FALSE,
    signed_off      BOOLEAN NOT NULL DEFAULT FALSE,
    signed_off_at   TIMESTAMPTZ,
    pushed_to_abha  BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_radiology_reports_order ON hms.radiology_reports (order_id);

-- ---------------------------------------------------------------------------
-- Critical finding alerts to ordering doctor
-- ---------------------------------------------------------------------------
CREATE TABLE hms.radiology_critical_alerts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    report_id       UUID NOT NULL REFERENCES hms.radiology_reports(id),
    notified_doctor_id UUID REFERENCES hms.doctors(id),
    notified_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    acknowledged_at TIMESTAMPTZ
);

-- ---------------------------------------------------------------------------
-- AERB radiation-safety dose logs
-- ---------------------------------------------------------------------------
CREATE TABLE hms.radiology_dose_logs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id        UUID NOT NULL REFERENCES hms.radiology_orders(id),
    equipment_id    VARCHAR(50),
    dose_value      NUMERIC(10,4),
    dose_unit       VARCHAR(20),                   -- mGy, mSv, etc.
    qa_check_passed BOOLEAN,
    recorded_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Report TAT prediction
-- ---------------------------------------------------------------------------
CREATE TABLE hms.radiology_tat_predictions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id        UUID NOT NULL REFERENCES hms.radiology_orders(id),
    predicted_tat_hours NUMERIC(5,2),
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    generated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.radiology_orders IS 'Module 12 (B14): Flow = Order -> Modality Worklist -> Acquisition -> PACS Storage -> Report -> Sign-off -> ABHA PHR push.';
