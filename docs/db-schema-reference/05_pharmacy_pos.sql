-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 05_pharmacy_pos.sql
-- Module 3: Pharmacy POS
-- Ref: Blueprint B5 — GST invoicing (HSN/CGST/SGST), Schedule H/H1/X,
--      Jan Aushadhi generics, FIFO/FEFO inventory, e-Way Bill (>₹50,000)
-- Requires: 00_extensions_common.sql, 02_patient_registration.sql
-- ============================================================================

SET search_path TO hms, public;

-- ---------------------------------------------------------------------------
-- Drug master catalog
-- ---------------------------------------------------------------------------
CREATE TABLE hms.pharmacy_drug_catalog (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    drug_name       VARCHAR(200) NOT NULL,
    generic_name    VARCHAR(200),
    is_jan_aushadhi_generic BOOLEAN NOT NULL DEFAULT FALSE,
    hsn_code        VARCHAR(15),
    gst_rate_pct    NUMERIC(4,2) NOT NULL DEFAULT 12.00,
    schedule_flag   hms.schedule_drug_flag NOT NULL DEFAULT 'none',
    unit_of_measure VARCHAR(20) DEFAULT 'strip',
    is_active       BOOLEAN NOT NULL DEFAULT TRUE
);

-- ---------------------------------------------------------------------------
-- Stock batches (FIFO/FEFO — First-Expiry-First-Out)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.pharmacy_stock_batches (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    drug_id         UUID NOT NULL REFERENCES hms.pharmacy_drug_catalog(id),
    batch_number    VARCHAR(50) NOT NULL,
    expiry_date     DATE NOT NULL,
    quantity_received NUMERIC(10,2) NOT NULL,
    quantity_on_hand   NUMERIC(10,2) NOT NULL,
    purchase_price     NUMERIC(10,2),
    mrp                 NUMERIC(10,2),
    supplier_name       VARCHAR(200),
    received_at         TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_pharmacy_batches_fefo ON hms.pharmacy_stock_batches (drug_id, expiry_date);

-- ---------------------------------------------------------------------------
-- Sales (POS invoice header + items)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.pharmacy_sales (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    invoice_number  VARCHAR(30) UNIQUE NOT NULL,           -- RX-INV-2026-044821
    patient_id      UUID REFERENCES hms.patients(id),       -- nullable: walk-in counter sale
    prescription_id UUID,                                   -- hms.prescriptions.id, when fulfilling Rx
    subtotal        NUMERIC(10,2) NOT NULL,
    cgst_amount     NUMERIC(10,2) NOT NULL DEFAULT 0,
    sgst_amount     NUMERIC(10,2) NOT NULL DEFAULT 0,
    total_amount    NUMERIC(10,2) NOT NULL,
    payment_status  hms.payment_status NOT NULL DEFAULT 'paid',
    payment_method  hms.payment_method,
    eway_bill_triggered BOOLEAN NOT NULL DEFAULT FALSE,      -- auto-trigger if > ₹50,000
    eway_bill_number     VARCHAR(50),
    sold_at                TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_pharmacy_sales_patient ON hms.pharmacy_sales (patient_id);

CREATE TABLE hms.pharmacy_sale_items (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sale_id         UUID NOT NULL REFERENCES hms.pharmacy_sales(id) ON DELETE CASCADE,
    drug_id         UUID NOT NULL REFERENCES hms.pharmacy_drug_catalog(id),
    batch_id        UUID NOT NULL REFERENCES hms.pharmacy_stock_batches(id),
    quantity        NUMERIC(10,2) NOT NULL,
    unit_price      NUMERIC(10,2) NOT NULL,
    gst_amount      NUMERIC(10,2) NOT NULL DEFAULT 0,
    line_total      NUMERIC(10,2) NOT NULL,
    interaction_checked_at TIMESTAMPTZ,                      -- real-time dispensing check (B5)
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id)
);

-- ---------------------------------------------------------------------------
-- Reorder / demand forecasting (Prophet/ARIMA driven, B5)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.pharmacy_reorder_suggestions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    drug_id         UUID NOT NULL REFERENCES hms.pharmacy_drug_catalog(id),
    suggested_quantity NUMERIC(10,2) NOT NULL,
    forecast_model  VARCHAR(30),                            -- 'prophet' / 'arima'
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    generated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    actioned        BOOLEAN NOT NULL DEFAULT FALSE
);

-- ---------------------------------------------------------------------------
-- PM-JAY claim fraud flags (Fraud Detection, B5)
-- ---------------------------------------------------------------------------
CREATE TABLE hms.pharmacy_claim_fraud_flags (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sale_id         UUID NOT NULL REFERENCES hms.pharmacy_sales(id),
    flag_reason     TEXT NOT NULL,
    risk_score      NUMERIC(5,4),
    ai_log_id       UUID REFERENCES hms.ai_interaction_log(id),
    reviewed        BOOLEAN NOT NULL DEFAULT FALSE,
    flagged_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE hms.pharmacy_sales IS 'POS invoice header (Module 3, B5): Customer ID -> Cart -> GST Summary -> Payment -> Invoice.';
