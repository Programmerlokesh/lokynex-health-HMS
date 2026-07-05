-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 11_ot_management.sql
-- Module 10: OT Management (NEW)
-- Ref: Blueprint B12 — conflict-free OT scheduling, WHO Surgical Safety
--      Checklist, staff allocation, implant/consumable tracking, post-op
--      recovery, OT utilisation analytics
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql,
--           06_ward_bed_management.sql (post-op -> ward transfer)
-- ============================================================================

SET search_path TO hms, public;

-- ---------------------------------------------------------------------------
-- OT rooms
-- ---------------------------------------------------------------------------
CREATE TABLE hms.ot_rooms (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    room_name       VARCHAR(50) NOT NULL,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE
);

-- ---------------------------------------------------------------------------
-- OT bookings (conflict-free scheduling — exclusion constraint blocks overlap)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.ot_bookings (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_number  VARCHAR(30) UNIQUE NOT NULL,
    ot_room_id      UUID NOT NULL REFERENCES hms.ot_rooms(id),
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    primary_surgeon_id UUID REFERENCES hms.doctors(id),
    procedure_name  VARCHAR(200) NOT NULL,
    scheduled_start TIMESTAMPTZ NOT NULL,
    scheduled_end   TIMESTAMPTZ NOT NULL,
    status          VARCHAR(20) NOT NULL DEFAULT 'booked',  -- booked/in_progress/completed/cancelled
    ai_schedule_optimized BOOLEAN NOT NULL DEFAULT FALSE,   -- OT Schedule Optimization (B12)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    time_range      TSTZRANGE GENERATED ALWAYS AS (tstzrange(scheduled_start, scheduled_end)) STORED,
    EXCLUDE USING gist (ot_room_id WITH =, time_range WITH &&)  -- conflict-free guard
);
CREATE INDEX idx_ot_bookings_patient ON hms.ot_bookings (patient_id);

-- ---------------------------------------------------------------------------
-- Staff allocation (surgeon/anaesthetist/nurses)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.ot_staff_allocation (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ot_booking_id   UUID NOT NULL REFERENCES hms.ot_bookings(id) ON DELETE CASCADE,
    staff_id        UUID NOT NULL,                 -- hms.employees.id (Module 13)
    role_in_surgery VARCHAR(50) NOT NULL            -- surgeon/assistant_surgeon/anaesthetist/scrub_nurse/circulating_nurse
);

-- ---------------------------------------------------------------------------
-- Pre-op consent + WHO Surgical Safety Checklist (Sign-in / Timeout / Sign-out)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.ot_consent (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ot_booking_id   UUID NOT NULL REFERENCES hms.ot_bookings(id) ON DELETE CASCADE,
    consent_document_url TEXT,
    signed_by_patient_or_guardian BOOLEAN NOT NULL DEFAULT FALSE,
    signed_at       TIMESTAMPTZ
);

CREATE TABLE hms.ot_safety_checklist (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ot_booking_id   UUID NOT NULL REFERENCES hms.ot_bookings(id) ON DELETE CASCADE,
    phase           VARCHAR(20) NOT NULL,          -- 'sign_in','time_out','sign_out'
    checklist_json  JSONB NOT NULL,                -- WHO checklist items + responses
    completed_by    UUID,
    completed_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Intra-op notes & SSI risk prediction
-- ---------------------------------------------------------------------------
CREATE TABLE hms.ot_intra_op_notes (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ot_booking_id   UUID NOT NULL REFERENCES hms.ot_bookings(id) ON DELETE CASCADE,
    notes           TEXT,
    blood_loss_ml   INTEGER,
    complications   TEXT,
    ssi_risk_score  NUMERIC(5,4),                  -- SSI Risk Prediction (B12)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    recorded_by     UUID,
    recorded_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Post-op recovery -> transfer to Ward (Module 4)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.ot_post_op_recovery (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ot_booking_id   UUID NOT NULL REFERENCES hms.ot_bookings(id) ON DELETE CASCADE,
    recovery_start  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    recovery_end    TIMESTAMPTZ,
    vitals_json     JSONB,
    transferred_to_admission_id UUID REFERENCES hms.admissions(id)   -- Module 4 link
);

-- ---------------------------------------------------------------------------
-- Implant & consumable tracking + forecasting
-- ---------------------------------------------------------------------------
CREATE TABLE hms.ot_implant_consumable_usage (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ot_booking_id   UUID NOT NULL REFERENCES hms.ot_bookings(id) ON DELETE CASCADE,
    item_name       VARCHAR(200) NOT NULL,
    item_type       VARCHAR(20) NOT NULL,          -- 'implant' or 'consumable'
    batch_or_serial_number VARCHAR(100),
    quantity_used   NUMERIC(10,2) NOT NULL DEFAULT 1
);

CREATE TABLE hms.ot_consumable_forecasts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    item_name       VARCHAR(200) NOT NULL,
    forecast_for_week DATE NOT NULL,
    predicted_quantity_needed NUMERIC(10,2),
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    generated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.ot_bookings IS 'Module 10 (B12): Flow = OT Booking -> Pre-Op Consent -> Safety Checklist -> Intra-op Notes -> Post-Op Recovery -> Ward Transfer.';
