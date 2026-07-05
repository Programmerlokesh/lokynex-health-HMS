-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 08_emergency_er.sql
-- Module 6: Emergency / ER
-- Ref: Blueprint B8 — ESI Triage (1-5) + MCI Black Tag, MLC auto-gen + police
--      SMS, STEMI/Stroke/Sepsis auto-alerts, 108/NDMA ambulance integration
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

CREATE TYPE hms.esi_level AS ENUM ('1','2','3','4','5','mci_black');

-- ---------------------------------------------------------------------------
-- ER visit
-- ---------------------------------------------------------------------------
CREATE TABLE hms.er_visits (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    visit_number    VARCHAR(30) UNIQUE NOT NULL,
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    arrival_mode    VARCHAR(30),                   -- 'walk_in','ambulance_108','referred'
    arrived_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    disposition     VARCHAR(30),                   -- 'admitted','discharged','referred_out','deceased'
    disposition_at  TIMESTAMPTZ,
    status          hms.record_status NOT NULL DEFAULT 'active'
);
CREATE INDEX idx_er_visits_patient ON hms.er_visits (patient_id);

-- ---------------------------------------------------------------------------
-- Triage (ESI level, AI-assisted)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.er_triage (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    er_visit_id     UUID NOT NULL REFERENCES hms.er_visits(id) ON DELETE CASCADE,
    esi_level       hms.esi_level NOT NULL,
    response_zone   VARCHAR(30),                   -- Resuscitation/Acute/Semi-Acute/Fast Track/Waiting/Comfort
    triaged_by      UUID,
    ai_suggested_level hms.esi_level,                -- AI Triage Assist (B8)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    triaged_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_er_triage_visit ON hms.er_triage (er_visit_id);

-- ---------------------------------------------------------------------------
-- Medico-Legal Case (MLC) records — auto-gen + police SMS
-- ---------------------------------------------------------------------------
CREATE TABLE hms.er_mlc_records (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    er_visit_id     UUID NOT NULL REFERENCES hms.er_visits(id) ON DELETE CASCADE,
    mlc_number      VARCHAR(30) UNIQUE NOT NULL,
    incident_type   VARCHAR(100),                  -- RTA/assault/poisoning/burns etc.
    police_station   VARCHAR(150),
    police_sms_sent_at TIMESTAMPTZ,
    reported_by      UUID,
    created_at        TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Critical condition auto-alerts: STEMI / Stroke / Sepsis
-- ---------------------------------------------------------------------------
CREATE TABLE hms.er_critical_alerts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    er_visit_id     UUID NOT NULL REFERENCES hms.er_visits(id) ON DELETE CASCADE,
    alert_type      VARCHAR(30) NOT NULL,          -- 'stemi','stroke','sepsis','mci'
    team_activated_at TIMESTAMPTZ,
    detection_latency_seconds INTEGER,              -- target <2 min (B8)
    ai_detected     BOOLEAN NOT NULL DEFAULT FALSE,
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Ambulance integration (108/NDMA)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.er_ambulance_calls (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    er_visit_id     UUID REFERENCES hms.er_visits(id),
    ambulance_service VARCHAR(30) DEFAULT '108',
    dispatch_ref    VARCHAR(50),
    predicted_eta_minutes SMALLINT,                -- Ambulance ETA Prediction (B8)
    actual_arrival_at TIMESTAMPTZ,
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    requested_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.er_triage IS 'ESI Level 1=Resuscitation(now) ... 5=Waiting(<120min); MCI Black=comfort care only (Module 6, B8).';
