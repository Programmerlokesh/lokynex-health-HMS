-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 06_ward_bed_management.sql
-- Module 4: Ward & Bed Management
-- Ref: Blueprint B6 — colour-coded bed map, Braden/Morse/NRS-2002 assessments,
--      ISBAR handover, housekeeping tasks, PM-JAY package linking
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

CREATE TYPE hms.bed_status AS ENUM ('available','occupied','reserved','cleaning','maintenance','blocked');

-- ---------------------------------------------------------------------------
-- Wards & Beds (live colour-coded bed map source)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.wards (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ward_name       VARCHAR(100) NOT NULL,
    ward_type       VARCHAR(50),                  -- general/semi-private/private/maternity
    floor           VARCHAR(20),
    is_active       BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE hms.beds (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ward_id         UUID NOT NULL REFERENCES hms.wards(id),
    bed_number      VARCHAR(20) NOT NULL,
    bed_category    VARCHAR(30),                  -- general/semi-private/private/icu (icu beds also in Module 5)
    status          hms.bed_status NOT NULL DEFAULT 'available',
    daily_rate      NUMERIC(10,2),
    UNIQUE (ward_id, bed_number)
);
CREATE INDEX idx_beds_status ON hms.beds (status);

-- ---------------------------------------------------------------------------
-- Admissions
-- ---------------------------------------------------------------------------
CREATE TABLE hms.admissions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    admission_number VARCHAR(30) UNIQUE NOT NULL,
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    admitting_doctor_id UUID REFERENCES hms.doctors(id),
    bed_id          UUID NOT NULL REFERENCES hms.beds(id),
    pmjay_package_code VARCHAR(30),
    admitted_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),    -- exact allocation time -> room charge calc (B10)
    discharged_at   TIMESTAMPTZ,
    status          hms.record_status NOT NULL DEFAULT 'active'
);
CREATE INDEX idx_admissions_patient ON hms.admissions (patient_id);
CREATE INDEX idx_admissions_bed ON hms.admissions (bed_id);

-- ---------------------------------------------------------------------------
-- Nursing assessments — Braden (pressure ulcer), Morse (fall risk), NRS-2002 (nutrition)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.nursing_assessments (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    admission_id    UUID NOT NULL REFERENCES hms.admissions(id) ON DELETE CASCADE,
    assessed_by     UUID,                                  -- nurse staff id
    vitals_json     JSONB,
    gcs_score       SMALLINT,                               -- Glasgow Coma Scale
    braden_score    SMALLINT,
    morse_fall_score SMALLINT,
    nrs2002_score   SMALLINT,
    ai_los_prediction_days NUMERIC(5,1),                    -- Length of Stay Prediction (B6)
    ai_readmission_risk    NUMERIC(5,4),                    -- Readmission Prevention (B6)
    ai_log_id              UUID REFERENCES hms.ai_interaction_log(id),
    assessed_at             TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_nursing_assessments_admission ON hms.nursing_assessments (admission_id);

-- ---------------------------------------------------------------------------
-- ISBAR handover / bed transfers
-- ---------------------------------------------------------------------------
CREATE TABLE hms.bed_transfers (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    admission_id    UUID NOT NULL REFERENCES hms.admissions(id) ON DELETE CASCADE,
    from_bed_id     UUID REFERENCES hms.beds(id),
    to_bed_id       UUID NOT NULL REFERENCES hms.beds(id),
    -- ISBAR: Identify / Situation / Background / Assessment / Recommendation
    isbar_identify  TEXT,
    isbar_situation TEXT,
    isbar_background TEXT,
    isbar_assessment  TEXT,
    isbar_recommendation TEXT,
    transferred_by      UUID,
    transferred_at        TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Auto-generated housekeeping tasks (on discharge/transfer)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.housekeeping_tasks (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    bed_id          UUID NOT NULL REFERENCES hms.beds(id),
    task_type       VARCHAR(30) DEFAULT 'terminal_cleaning',
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',  -- pending/in_progress/done
    assigned_to     UUID,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at    TIMESTAMPTZ
);

-- ---------------------------------------------------------------------------
-- Bed demand forecasting (48hrs ahead, B6)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.bed_demand_forecasts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ward_id         UUID NOT NULL REFERENCES hms.wards(id),
    forecast_for_date DATE NOT NULL,
    predicted_occupancy_pct NUMERIC(5,2),
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    generated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.admissions IS 'Module 4 (B6). ICU admissions (Module 5) reference the same beds table when bed_category = icu; ICU clinical detail lives in hms.icu_admissions.';
