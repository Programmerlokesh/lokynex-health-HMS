-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 09_billing_finance.sql
-- Module 8: Billing & Finance
-- Ref: Blueprint B10 — GST invoices (CGST/SGST/IGST), PM-JAY pre-auth, CGHS/
--      ESIC/ECHS rate masters, TPA cashless/reimbursement, UPI/Card/NEFT/Split
--      payment, e-Way Bill, room charges from exact allocation time
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

CREATE TYPE hms.claim_type AS ENUM ('pmjay_preauth','cghs','esic','echs','tpa_cashless','tpa_reimbursement');
CREATE TYPE hms.claim_status AS ENUM ('submitted','approved','partially_approved','rejected','settled');

-- ---------------------------------------------------------------------------
-- Scheme / TPA rate masters
-- ---------------------------------------------------------------------------
CREATE TABLE hms.billing_rate_masters (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    scheme_name     VARCHAR(50) NOT NULL,          -- CGHS/ESIC/ECHS/PM-JAY
    package_code    VARCHAR(30),
    service_description TEXT,
    rate            NUMERIC(10,2) NOT NULL,
    effective_from  DATE NOT NULL,
    effective_to    DATE
);

-- ---------------------------------------------------------------------------
-- Master invoice (consolidates charges across modules: OPD, Pharmacy, Lab,
-- Ward room charges, OT, Blood Bank, Radiology)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.billing_invoices (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    invoice_number  VARCHAR(30) UNIQUE NOT NULL,
    patient_id      UUID NOT NULL REFERENCES hms.patients(id),
    admission_id    UUID REFERENCES hms.admissions(id),     -- nullable for OPD-only bills
    invoice_date    DATE NOT NULL DEFAULT CURRENT_DATE,
    subtotal        NUMERIC(12,2) NOT NULL DEFAULT 0,
    discount_amount NUMERIC(12,2) NOT NULL DEFAULT 0,
    cgst_amount     NUMERIC(12,2) NOT NULL DEFAULT 0,
    sgst_amount     NUMERIC(12,2) NOT NULL DEFAULT 0,
    igst_amount     NUMERIC(12,2) NOT NULL DEFAULT 0,
    total_amount    NUMERIC(12,2) NOT NULL DEFAULT 0,
    payment_status  hms.payment_status NOT NULL DEFAULT 'pending',
    eway_bill_number VARCHAR(50),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_billing_invoices_patient ON hms.billing_invoices (patient_id);

CREATE TABLE hms.billing_invoice_items (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    invoice_id      UUID NOT NULL REFERENCES hms.billing_invoices(id) ON DELETE CASCADE,
    source_module   VARCHAR(30) NOT NULL,          -- 'opd','pharmacy','lab','ward','ot','blood_bank','radiology'
    source_record_id UUID,                          -- pointer into the source module's table
    description     TEXT NOT NULL,
    hsn_sac_code    VARCHAR(15),
    quantity        NUMERIC(10,2) NOT NULL DEFAULT 1,
    unit_price      NUMERIC(10,2) NOT NULL,
    gst_rate_pct    NUMERIC(4,2) NOT NULL DEFAULT 0,
    line_total      NUMERIC(12,2) NOT NULL
);

-- ---------------------------------------------------------------------------
-- Room charges (computed from exact bed-allocation timestamps, B10)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.billing_room_charges (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    admission_id    UUID NOT NULL REFERENCES hms.admissions(id),
    bed_id          UUID NOT NULL REFERENCES hms.beds(id),
    charge_from     TIMESTAMPTZ NOT NULL,
    charge_to       TIMESTAMPTZ NOT NULL,
    daily_rate      NUMERIC(10,2) NOT NULL,
    computed_amount NUMERIC(12,2) NOT NULL,
    invoice_item_id UUID REFERENCES hms.billing_invoice_items(id)
);

-- ---------------------------------------------------------------------------
-- Payments (split payment supported via multiple rows per invoice)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.billing_payments (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    invoice_id      UUID NOT NULL REFERENCES hms.billing_invoices(id),
    amount          NUMERIC(12,2) NOT NULL,
    method          hms.payment_method NOT NULL,
    reference_number VARCHAR(100),
    paid_at         TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- Insurance / scheme claims (PM-JAY pre-auth, TPA cashless/reimbursement)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.billing_claims (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    invoice_id      UUID NOT NULL REFERENCES hms.billing_invoices(id),
    patient_insurance_id UUID REFERENCES hms.patient_insurance(id),
    claim_type      hms.claim_type NOT NULL,
    claim_number    VARCHAR(50),
    claimed_amount  NUMERIC(12,2) NOT NULL,
    approved_amount NUMERIC(12,2),
    status          hms.claim_status NOT NULL DEFAULT 'submitted',
    submitted_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    settled_at      TIMESTAMPTZ
);
CREATE INDEX idx_billing_claims_invoice ON hms.billing_claims (invoice_id);

COMMENT ON TABLE hms.billing_invoices IS 'Module 8 (B10): consolidated, GST-compliant billing across all clinical modules.';
