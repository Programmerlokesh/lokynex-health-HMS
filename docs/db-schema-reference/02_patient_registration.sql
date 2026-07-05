-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 02_patient_registration.sql
-- Module 7: Patient Registration — universal entry point to the HMS.
--           Every clinical module (Doctor, Lab, Ward, ICU, ER, OT, Blood
--           Bank, Radiology, Portal) references hms.patients.
-- Ref: Blueprint B9
-- Requires: 00_extensions_common.sql
-- ============================================================================

SET search_path TO hms, public;

CREATE TYPE hms.insurance_type AS ENUM ('pmjay','cghs','esic','echs','private_tpa','self_pay');

-- ---------------------------------------------------------------------------
-- Core patient master (UHID = Unique Hospital ID)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.patients (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    uhid                VARCHAR(30) UNIQUE NOT NULL,         -- auto-generated
    abha_id             VARCHAR(20) UNIQUE,                   -- ABDM health ID
    full_name           VARCHAR(150) NOT NULL,
    date_of_birth       DATE,
    gender              hms.gender_type NOT NULL DEFAULT 'unknown',
    aadhaar_number_enc  BYTEA,                                 -- pgp_sym_encrypt, AES-256 (IT Act §43A)
    aadhaar_last4       VARCHAR(4),                             -- masked display only
    pan_number          VARCHAR(10),
    mobile              VARCHAR(15) NOT NULL,
    alt_mobile          VARCHAR(15),
    email                VARCHAR(200),
    address              TEXT,
    city                 VARCHAR(100),
    district             VARCHAR(100),
    state                VARCHAR(100),
    pin_code             VARCHAR(6),
    preferred_language   VARCHAR(30) DEFAULT 'en',              -- of 12 Indian languages (B16)
    blood_group           VARCHAR(5),
    is_duplicate_of        UUID REFERENCES hms.patients(id),     -- duplicate-detection link
    registered_at_tenant_branch UUID,                            -- for multi-branch facilities
    status                 hms.record_status NOT NULL DEFAULT 'active',
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at              TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_patients_name_trgm ON hms.patients USING gin (full_name gin_trgm_ops);
CREATE INDEX idx_patients_mobile ON hms.patients (mobile);
CREATE INDEX idx_patients_abha ON hms.patients (abha_id);
CREATE TRIGGER trg_patients_updated_at BEFORE UPDATE ON hms.patients
    FOR EACH ROW EXECUTE FUNCTION hms.set_updated_at();

-- ---------------------------------------------------------------------------
-- Insurance / Government scheme linkage
-- ---------------------------------------------------------------------------
CREATE TABLE hms.patient_insurance (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id      UUID NOT NULL REFERENCES hms.patients(id) ON DELETE CASCADE,
    insurance_type  hms.insurance_type NOT NULL,
    scheme_number   VARCHAR(100),                  -- PM-JAY/CGHS/ESIC beneficiary no.
    tpa_name        VARCHAR(150),
    valid_from      DATE,
    valid_to        DATE,
    eligibility_verified BOOLEAN NOT NULL DEFAULT FALSE,
    eligibility_checked_at TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_patient_insurance_patient ON hms.patient_insurance (patient_id);

-- ---------------------------------------------------------------------------
-- DPDP consent register (treatment / ABHA-sharing / data processing)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.patient_consents (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id      UUID NOT NULL REFERENCES hms.patients(id) ON DELETE CASCADE,
    consent_type    VARCHAR(50) NOT NULL,           -- 'dpdp_data_processing','treatment','abha_sharing','cross_branch_lookup'
    status          hms.consent_status NOT NULL DEFAULT 'granted',
    granted_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    revoked_at      TIMESTAMPTZ,
    expires_at      TIMESTAMPTZ,
    captured_via    VARCHAR(50),                     -- 'otp','signature','portal'
    ip_address      INET
);
CREATE INDEX idx_consents_patient ON hms.patient_consents (patient_id, consent_type);

-- ---------------------------------------------------------------------------
-- Identity document OCR captures (Aadhaar/PAN via Azure AI Document Intelligence)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.patient_documents (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id      UUID NOT NULL REFERENCES hms.patients(id) ON DELETE CASCADE,
    document_type   VARCHAR(30) NOT NULL,            -- 'aadhaar','pan','other_id'
    file_url        TEXT NOT NULL,
    ocr_extracted_json JSONB,
    ocr_confidence  NUMERIC(5,4),
    uploaded_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.patients IS 'Universal patient master — referenced by every clinical module (B9). Aadhaar stored encrypted (AES-256) per IT Act 2000 §43A.';
