-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 07_icu_monitoring.sql
-- Module 5: ICU Monitoring (MICU/SICU/NICU/PICU/CICU)
-- Ref: Blueprint B7 — real-time vitals (Kafka -> TimescaleDB), APACHE II/SOFA,
--      AI Early Warning Score, Sepsis prediction, Ventilator & I/O tracking
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql,
--           06_ward_bed_management.sql (reuses hms.beds, hms.admissions)
-- ============================================================================

SET search_path TO hms, public;

CREATE TYPE hms.icu_unit_type AS ENUM ('micu','sicu','nicu','picu','cicu');

-- ---------------------------------------------------------------------------
-- ICU admission (clinical layer on top of the bed-level admission)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.icu_admissions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    admission_id    UUID NOT NULL REFERENCES hms.admissions(id) ON DELETE CASCADE,  -- Module 4 link
    icu_unit_type   hms.icu_unit_type NOT NULL,
    admitted_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    discharged_at   TIMESTAMPTZ,
    apache_ii_score SMALLINT,
    sofa_score      SMALLINT,
    status          hms.record_status NOT NULL DEFAULT 'active'
);
CREATE INDEX idx_icu_admissions_admission ON hms.icu_admissions (admission_id);

-- ---------------------------------------------------------------------------
-- Real-time vitals stream (Kafka -> TimescaleDB hypertable in production;
-- modeled here as a regular time-partitioned table)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.icu_vitals (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    icu_admission_id UUID NOT NULL REFERENCES hms.icu_admissions(id) ON DELETE CASCADE,
    recorded_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    heart_rate      SMALLINT,             -- alert thresholds: <40 or >150
    systolic_bp     SMALLINT,             -- <90 or >180
    diastolic_bp    SMALLINT,
    map_mmhg        SMALLINT,             -- Mean Arterial Pressure, alert <65
    spo2_pct        SMALLINT,             -- alert <90
    temperature_c   NUMERIC(4,1),         -- alert <35 or >39
    urine_output_ml_per_kg_hr NUMERIC(5,2), -- alert <0.5
    respiratory_rate SMALLINT,
    is_threshold_breach BOOLEAN NOT NULL DEFAULT FALSE,
    breach_parameters    TEXT[]
);
CREATE INDEX idx_icu_vitals_admission_time ON hms.icu_vitals (icu_admission_id, recorded_at DESC);

-- ---------------------------------------------------------------------------
-- AI Early Warning Score / Sepsis / Mortality predictions
-- ---------------------------------------------------------------------------
CREATE TABLE hms.icu_ai_scores (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    icu_admission_id UUID NOT NULL REFERENCES hms.icu_admissions(id) ON DELETE CASCADE,
    score_type      VARCHAR(30) NOT NULL,        -- 'early_warning_lstm','sepsis_xgboost','mortality_daily'
    score_value     NUMERIC(6,4),
    predicted_deterioration_window_hours SMALLINT,  -- e.g. 4-6 hrs early
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    computed_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_icu_ai_scores_admission ON hms.icu_ai_scores (icu_admission_id, score_type);

-- ---------------------------------------------------------------------------
-- Ventilator records
-- ---------------------------------------------------------------------------
CREATE TABLE hms.icu_ventilator_records (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    icu_admission_id UUID NOT NULL REFERENCES hms.icu_admissions(id) ON DELETE CASCADE,
    mode            VARCHAR(30),                  -- e.g. SIMV, CPAP
    fio2_pct        SMALLINT,
    peep_cmh2o      NUMERIC(4,1),
    tidal_volume_ml SMALLINT,
    recorded_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Intake / Output charting
-- ---------------------------------------------------------------------------
CREATE TABLE hms.icu_io_charts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    icu_admission_id UUID NOT NULL REFERENCES hms.icu_admissions(id) ON DELETE CASCADE,
    chart_date      DATE NOT NULL DEFAULT CURRENT_DATE,
    intake_ml       NUMERIC(8,2) DEFAULT 0,
    output_ml       NUMERIC(8,2) DEFAULT 0,
    recorded_by     UUID,
    recorded_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.icu_vitals IS 'Module 5 (B7): production deployment streams via Kafka into a TimescaleDB hypertable for high-frequency vitals; alert thresholds per B7.';
