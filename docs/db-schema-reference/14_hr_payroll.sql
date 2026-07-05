-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 14_hr_payroll.sql
-- Module 13: HR & Payroll (NEW — universal across all facility types)
-- Ref: Blueprint B15 — onboarding & licence verification, duty roster/shift
--      scheduling, biometric/QR attendance, PF/ESI/TDS payroll, leave mgmt
-- Requires: 00_extensions_common.sql
-- ============================================================================

SET search_path TO hms, public;

-- ---------------------------------------------------------------------------
-- Employee master (Doctors/Nurses table in other modules can FK into this)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.employees (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_code   VARCHAR(30) UNIQUE NOT NULL,
    full_name       VARCHAR(150) NOT NULL,
    designation     VARCHAR(100) NOT NULL,         -- Doctor/Nurse/Pharmacist/Lab Tech/Reception/Accountant/etc.
    department      VARCHAR(100),
    date_of_joining DATE NOT NULL,
    mobile          VARCHAR(15),
    email           VARCHAR(200),
    pan_number      VARCHAR(10),
    aadhaar_number_enc BYTEA,
    bank_account_number_enc BYTEA,
    bank_ifsc       VARCHAR(11),
    pf_uan_number   VARCHAR(20),
    esi_number      VARCHAR(20),
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Credential / licence verification (NMC, Nursing Council, Pharmacy Council)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.employee_credentials (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id     UUID NOT NULL REFERENCES hms.employees(id) ON DELETE CASCADE,
    credential_type VARCHAR(50) NOT NULL,          -- 'nmc_registration','nursing_council','pharmacy_council'
    registration_number VARCHAR(50) NOT NULL,
    issuing_body    VARCHAR(150),
    valid_until     DATE,
    verified        BOOLEAN NOT NULL DEFAULT FALSE,
    verified_at     TIMESTAMPTZ
);

-- ---------------------------------------------------------------------------
-- Duty roster / shift scheduling
-- ---------------------------------------------------------------------------
CREATE TABLE hms.duty_rosters (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id     UUID NOT NULL REFERENCES hms.employees(id) ON DELETE CASCADE,
    shift_date      DATE NOT NULL,
    shift_type      VARCHAR(20) NOT NULL,          -- 'morning','evening','night','on_call'
    department      VARCHAR(100),
    ai_staffing_synced BOOLEAN NOT NULL DEFAULT FALSE,  -- syncs with bed-demand forecast (B15)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    UNIQUE (employee_id, shift_date, shift_type)
);
CREATE INDEX idx_duty_rosters_date ON hms.duty_rosters (shift_date);

-- ---------------------------------------------------------------------------
-- Biometric / QR attendance
-- ---------------------------------------------------------------------------
CREATE TABLE hms.attendance_records (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id     UUID NOT NULL REFERENCES hms.employees(id) ON DELETE CASCADE,
    attendance_date DATE NOT NULL,
    check_in_at     TIMESTAMPTZ,
    check_out_at    TIMESTAMPTZ,
    capture_method  VARCHAR(20) DEFAULT 'biometric',  -- 'biometric','qr'
    UNIQUE (employee_id, attendance_date)
);

-- ---------------------------------------------------------------------------
-- Leave management + anomaly detection
-- ---------------------------------------------------------------------------
CREATE TABLE hms.leave_requests (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id     UUID NOT NULL REFERENCES hms.employees(id) ON DELETE CASCADE,
    leave_type      VARCHAR(30) NOT NULL,          -- 'casual','sick','earned','maternity'
    start_date      DATE NOT NULL,
    end_date        DATE NOT NULL,
    status          VARCHAR(20) NOT NULL DEFAULT 'pending', -- pending/approved/rejected
    is_anomalous    BOOLEAN NOT NULL DEFAULT FALSE,  -- Leave Anomaly Detection (B15)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    requested_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    approved_by     UUID REFERENCES hms.employees(id)
);

-- ---------------------------------------------------------------------------
-- Attrition risk prediction
-- ---------------------------------------------------------------------------
CREATE TABLE hms.employee_attrition_risk (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id     UUID NOT NULL REFERENCES hms.employees(id) ON DELETE CASCADE,
    risk_score      NUMERIC(5,4) NOT NULL,
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    computed_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Payroll (PF/ESI/TDS-compliant)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.payroll_runs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    payroll_month   DATE NOT NULL,                 -- first day of month represents the period
    status          VARCHAR(20) NOT NULL DEFAULT 'draft',  -- draft/processed/paid
    processed_at    TIMESTAMPTZ,
    UNIQUE (payroll_month)
);

CREATE TABLE hms.payslips (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    payroll_run_id  UUID NOT NULL REFERENCES hms.payroll_runs(id) ON DELETE CASCADE,
    employee_id     UUID NOT NULL REFERENCES hms.employees(id),
    basic_salary    NUMERIC(10,2) NOT NULL,
    allowances      NUMERIC(10,2) NOT NULL DEFAULT 0,
    pf_deduction    NUMERIC(10,2) NOT NULL DEFAULT 0,   -- EPF Act
    esi_deduction   NUMERIC(10,2) NOT NULL DEFAULT 0,   -- ESI Act
    tds_deduction   NUMERIC(10,2) NOT NULL DEFAULT 0,
    other_deductions NUMERIC(10,2) NOT NULL DEFAULT 0,
    net_pay         NUMERIC(10,2) NOT NULL,
    paid_at         TIMESTAMPTZ,
    payslip_pdf_url TEXT
);
CREATE INDEX idx_payslips_employee ON hms.payslips (employee_id);

COMMENT ON TABLE hms.employees IS 'Module 13 (B15): universal HR master used by every facility type; Doctor/Nurse/Pharmacist module tables link here via employee_id.';
