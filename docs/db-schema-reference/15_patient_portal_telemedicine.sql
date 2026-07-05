-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 15_patient_portal_telemedicine.sql
-- Module 14: Patient Portal & Telemedicine (NEW)
-- Ref: Blueprint B16 — ABHA/OTP login, appointment booking, video teleconsult,
--      e-Rx & lab report access, bill view & online payment, multilingual
--      chatbot (12 Indian languages), medication reminders
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

-- ---------------------------------------------------------------------------
-- Portal account (patient-facing login)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.portal_accounts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id      UUID NOT NULL UNIQUE REFERENCES hms.patients(id) ON DELETE CASCADE,
    login_method    VARCHAR(20) NOT NULL DEFAULT 'abha_otp',  -- 'abha_otp','mobile_otp'
    last_login_at   TIMESTAMPTZ,
    preferred_language VARCHAR(30) DEFAULT 'en',
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Appointment booking (links to Doctor/OPD module)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.portal_appointments (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portal_account_id UUID NOT NULL REFERENCES hms.portal_accounts(id) ON DELETE CASCADE,
    doctor_id       UUID NOT NULL REFERENCES hms.doctors(id),
    requested_slot  TIMESTAMPTZ NOT NULL,
    appointment_type VARCHAR(20) NOT NULL DEFAULT 'in_person', -- 'in_person','teleconsult'
    status          VARCHAR(20) NOT NULL DEFAULT 'booked',     -- booked/confirmed/cancelled/completed/no_show
    ai_no_show_risk NUMERIC(5,4),                  -- No-show Prediction (B16)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    booked_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_portal_appointments_account ON hms.portal_appointments (portal_account_id);

-- ---------------------------------------------------------------------------
-- Video teleconsultation sessions
-- ---------------------------------------------------------------------------
CREATE TABLE hms.portal_teleconsultations (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    appointment_id  UUID NOT NULL REFERENCES hms.portal_appointments(id) ON DELETE CASCADE,
    video_session_url TEXT,
    started_at      TIMESTAMPTZ,
    ended_at        TIMESTAMPTZ,
    linked_opd_visit_id UUID                        -- hms.opd_visits.id once consultation begins
);

-- ---------------------------------------------------------------------------
-- Access logs — e-Rx / lab reports viewed via portal (DPDP-relevant)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.portal_record_access_logs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portal_account_id UUID NOT NULL REFERENCES hms.portal_accounts(id),
    record_type     VARCHAR(30) NOT NULL,          -- 'prescription','lab_report','radiology_report'
    record_id       UUID NOT NULL,
    accessed_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Bill view & online payment (links to Billing module)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.portal_bill_payments (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portal_account_id UUID NOT NULL REFERENCES hms.portal_accounts(id),
    invoice_id      UUID NOT NULL REFERENCES hms.billing_invoices(id),
    amount_paid     NUMERIC(10,2) NOT NULL,
    payment_gateway_ref VARCHAR(100),
    paid_at         TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Multilingual chatbot sessions (symptom triage, first-response)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.portal_chatbot_sessions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portal_account_id UUID REFERENCES hms.portal_accounts(id),
    language        VARCHAR(30) NOT NULL DEFAULT 'en',
    transcript_json JSONB,
    triage_suggestion TEXT,
    escalated_to_er BOOLEAN NOT NULL DEFAULT FALSE,
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    started_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ended_at        TIMESTAMPTZ
);

-- ---------------------------------------------------------------------------
-- Medication reminders & follow-up nudges
-- ---------------------------------------------------------------------------
CREATE TABLE hms.portal_medication_reminders (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portal_account_id UUID NOT NULL REFERENCES hms.portal_accounts(id) ON DELETE CASCADE,
    prescription_item_id UUID REFERENCES hms.prescription_items(id),
    reminder_time   TIME NOT NULL,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    ai_personalized BOOLEAN NOT NULL DEFAULT FALSE,  -- Personalized Reminders (B16)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Feedback (end of patient journey, per portal flow diagram)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.portal_feedback (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portal_account_id UUID NOT NULL REFERENCES hms.portal_accounts(id),
    appointment_id  UUID REFERENCES hms.portal_appointments(id),
    rating          SMALLINT CHECK (rating BETWEEN 1 AND 5),
    comments        TEXT,
    submitted_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.portal_accounts IS 'Module 14 (B16): Flow = Login -> Book Appointment/Teleconsult -> Video Consult -> Access Rx/Reports -> Pay Bill -> Feedback.';
