-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 12_blood_bank.sql
-- Module 11: Blood Bank Management (NEW)
-- Ref: Blueprint B13 — donor registration & eligibility, blood grouping &
--      cross-matching, component separation (PRBC/FFP/Platelets),
--      NACO/NBTC-compliant testing log, emergency requests, expiry inventory
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

CREATE TYPE hms.blood_component_type AS ENUM ('whole_blood','prbc','ffp','platelets','cryoprecipitate');

-- ---------------------------------------------------------------------------
-- Donors
-- ---------------------------------------------------------------------------
CREATE TABLE hms.blood_donors (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name       VARCHAR(150) NOT NULL,
    mobile          VARCHAR(15) NOT NULL,
    blood_group     VARCHAR(5) NOT NULL,
    date_of_birth   DATE,
    last_donation_date DATE,
    is_regular_donor BOOLEAN NOT NULL DEFAULT FALSE,
    ai_retention_risk NUMERIC(5,4),                -- Donor Retention Prediction (B13)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_blood_donors_group ON hms.blood_donors (blood_group);

CREATE TABLE hms.donor_eligibility_screening (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    donor_id        UUID NOT NULL REFERENCES hms.blood_donors(id) ON DELETE CASCADE,
    screened_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    hemoglobin_g_dl NUMERIC(4,1),
    weight_kg       NUMERIC(5,1),
    is_eligible     BOOLEAN NOT NULL,
    deferral_reason TEXT,
    screened_by     UUID
);

-- ---------------------------------------------------------------------------
-- Blood units — collection, grouping, component separation, expiry tracking
-- ---------------------------------------------------------------------------
CREATE TABLE hms.blood_units (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    unit_number     VARCHAR(30) UNIQUE NOT NULL,
    donor_id        UUID REFERENCES hms.blood_donors(id),
    blood_group     VARCHAR(5) NOT NULL,
    rh_factor       VARCHAR(10),
    component_type  hms.blood_component_type NOT NULL DEFAULT 'whole_blood',
    collected_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    expiry_date     DATE NOT NULL,
    status          VARCHAR(20) NOT NULL DEFAULT 'in_stock',  -- in_stock/reserved/issued/discarded/expired
    storage_location VARCHAR(50)
);
CREATE INDEX idx_blood_units_group_status ON hms.blood_units (blood_group, status, expiry_date);

-- ---------------------------------------------------------------------------
-- NACO/NBTC-compliant testing log (TTI screening: HIV, HBV, HCV, malaria, syphilis)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.blood_testing_log (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    blood_unit_id   UUID NOT NULL REFERENCES hms.blood_units(id) ON DELETE CASCADE,
    test_name       VARCHAR(100) NOT NULL,         -- 'HIV','HBsAg','HCV','VDRL','Malaria'
    result          VARCHAR(20) NOT NULL,          -- 'non_reactive','reactive'
    tested_by       UUID,
    tested_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Cross-match requests & issue records
-- ---------------------------------------------------------------------------
CREATE TABLE hms.blood_crossmatch_requests (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    blood_unit_id   UUID REFERENCES hms.blood_units(id),
    requested_for_module VARCHAR(30),              -- 'ot','er','ward','icu'
    requested_for_record_id UUID,                  -- e.g. hms.ot_bookings.id
    compatibility_result VARCHAR(20),               -- 'compatible','incompatible','pending'
    ai_auto_checked  BOOLEAN NOT NULL DEFAULT FALSE, -- Cross-match Auto-check (B13)
    ai_log_id        UUID REFERENCES hms.ai_interaction_log(id),
    requested_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    is_emergency      BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE hms.blood_issue_records (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    crossmatch_request_id UUID NOT NULL REFERENCES hms.blood_crossmatch_requests(id),
    blood_unit_id   UUID NOT NULL REFERENCES hms.blood_units(id),
    issued_to_patient_id UUID NOT NULL REFERENCES hms.patients(id),
    issued_by       UUID,
    issued_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Demand forecasting (shortage prediction per blood group, B13)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.blood_demand_forecasts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    blood_group     VARCHAR(5) NOT NULL,
    forecast_for_week DATE NOT NULL,
    predicted_shortage_units SMALLINT,
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    generated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.blood_units IS 'Module 11 (B13): NACO/NBTC-compliant — every unit must pass blood_testing_log (TTI screening) before status can move to in_stock for issue.';
