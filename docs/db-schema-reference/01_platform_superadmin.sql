-- ============================================================================
-- MediCore HMS — India Edition  |  v3.0 Schema Pack
-- File: 01_platform_superadmin.sql
-- Module: PART A — Owner / SuperAdmin Layer (lives in the PLATFORM database,
--         NOT inside any tenant schema — this is the single control-plane DB)
-- Ref: Blueprint A1–A12
-- ============================================================================

CREATE SCHEMA IF NOT EXISTS superadmin;
SET search_path TO superadmin, public;

CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE superadmin.tenant_status   AS ENUM ('trial','active','locked_readonly','locked_full','archived');
CREATE TYPE superadmin.facility_type   AS ENUM ('hospital','clinic','diagnostic_lab','pharmacy','polyclinic');
CREATE TYPE superadmin.internal_role   AS ENUM ('super_owner','sales_manager','support_admin','finance_admin');
CREATE TYPE superadmin.plan_type       AS ENUM ('monthly','quarterly','half_yearly','yearly','custom');

-- ---------------------------------------------------------------------------
-- A2/A12 — Internal platform staff (Sales / Support / Finance / Owner)
-- ---------------------------------------------------------------------------
CREATE TABLE superadmin.internal_users (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name       VARCHAR(150) NOT NULL,
    email           VARCHAR(200) UNIQUE NOT NULL,
    role            superadmin.internal_role NOT NULL,
    two_fa_enabled  BOOLEAN NOT NULL DEFAULT TRUE,            -- A12: 2FA mandatory
    ip_whitelist    INET[],
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    last_login_at   TIMESTAMPTZ,
    failed_login_attempts SMALLINT NOT NULL DEFAULT 0,
    locked_until    TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- A1/A3 — Tenants (one row per Hospital / Clinic / Lab / Pharmacy)
-- ---------------------------------------------------------------------------
CREATE TABLE superadmin.tenants (
    id                   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name                 VARCHAR(200) NOT NULL,
    facility_type        superadmin.facility_type NOT NULL,
    registration_number  VARCHAR(100),                         -- MCI/State reg no.
    gstin                VARCHAR(15),
    address              TEXT,
    city                 VARCHAR(100),
    state                VARCHAR(100),
    pin_code             VARCHAR(6),
    phone                VARCHAR(15),
    email                VARCHAR(200),
    website              VARCHAR(200),
    bed_count            INTEGER,
    doctor_count         INTEGER,
    subdomain            VARCHAR(100) UNIQUE NOT NULL,          -- clientname.medicorehms.in
    db_schema_name       VARCHAR(100) UNIQUE NOT NULL,          -- tenant_{uuid}
    rls_tag              VARCHAR(100) UNIQUE NOT NULL,
    parent_tenant_id      UUID REFERENCES superadmin.tenants(id),  -- A7: multi-branch groups
    status                superadmin.tenant_status NOT NULL DEFAULT 'trial',
    created_by_user_id      UUID REFERENCES superadmin.internal_users(id),
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at              TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_tenants_parent ON superadmin.tenants (parent_tenant_id);
CREATE INDEX idx_tenants_status ON superadmin.tenants (status);

-- A3 Section B — Primary admin contact per tenant (becomes Tier-3 Hospital Admin)
CREATE TABLE superadmin.tenant_admins (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID NOT NULL REFERENCES superadmin.tenants(id) ON DELETE CASCADE,
    full_name       VARCHAR(150) NOT NULL,
    mobile          VARCHAR(15) NOT NULL,
    email           VARCHAR(200) NOT NULL,            -- login ID
    designation     VARCHAR(100),
    temp_password_hash TEXT,
    must_reset_password BOOLEAN NOT NULL DEFAULT TRUE,
    first_login_at  TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------------------------------------------------------------------------
-- A4 — Module catalog (the 14 modules, master price list) + per-tenant toggles
-- ---------------------------------------------------------------------------
CREATE TABLE superadmin.module_catalog (
    module_id       SMALLINT PRIMARY KEY,             -- 1..14
    module_name     VARCHAR(100) NOT NULL,
    family          VARCHAR(50) NOT NULL,              -- Clinical Care / Critical / Diagnostics / etc.
    standalone_use  VARCHAR(150),
    base_monthly_price NUMERIC(10,2) NOT NULL,
    requires_module_id SMALLINT REFERENCES superadmin.module_catalog(module_id), -- dependency, e.g. ICU->Ward
    is_universal    BOOLEAN NOT NULL DEFAULT FALSE     -- HR & Payroll: available to every facility type
);

CREATE TABLE superadmin.tenant_modules (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID NOT NULL REFERENCES superadmin.tenants(id) ON DELETE CASCADE,
    module_id       SMALLINT NOT NULL REFERENCES superadmin.module_catalog(module_id),
    is_active       BOOLEAN NOT NULL DEFAULT FALSE,
    monthly_price   NUMERIC(10,2) NOT NULL,            -- snapshot at activation (post-discount)
    activated_at    TIMESTAMPTZ,
    deactivated_at  TIMESTAMPTZ,
    UNIQUE (tenant_id, module_id)
);
CREATE INDEX idx_tenant_modules_tenant ON superadmin.tenant_modules (tenant_id);

CREATE TABLE superadmin.bundles (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    bundle_name     VARCHAR(100) NOT NULL,             -- 'Hospital Full', 'Clinic Pack', etc.
    module_ids      SMALLINT[] NOT NULL,
    bundle_price    NUMERIC(10,2) NOT NULL
);

-- ---------------------------------------------------------------------------
-- A5/A6 — Subscriptions, trials, billing/grace-period lifecycle
-- ---------------------------------------------------------------------------
CREATE TABLE superadmin.subscriptions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID NOT NULL REFERENCES superadmin.tenants(id) ON DELETE CASCADE,
    plan_type       superadmin.plan_type NOT NULL,
    start_date      DATE NOT NULL,
    end_date        DATE NOT NULL,
    is_trial        BOOLEAN NOT NULL DEFAULT FALSE,
    trial_days      SMALLINT DEFAULT 0 CHECK (trial_days BETWEEN 0 AND 30),
    trial_extended_by UUID REFERENCES superadmin.internal_users(id),
    monthly_amount  NUMERIC(10,2) NOT NULL,
    discount_pct    NUMERIC(5,2) NOT NULL DEFAULT 0,
    final_amount    NUMERIC(10,2) NOT NULL,
    auto_renew      BOOLEAN NOT NULL DEFAULT FALSE,
    grace_day_count SMALLINT NOT NULL DEFAULT 0,        -- 0-7 warn, 8-15 read-only, >15 full lock
    status          VARCHAR(20) NOT NULL DEFAULT 'active',
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_subscriptions_tenant ON superadmin.subscriptions (tenant_id, status);
CREATE INDEX idx_subscriptions_end_date ON superadmin.subscriptions (end_date);

-- ---------------------------------------------------------------------------
-- A5 — Invoices & Payments
-- ---------------------------------------------------------------------------
CREATE TABLE superadmin.invoices (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    invoice_number  VARCHAR(50) UNIQUE NOT NULL,
    tenant_id       UUID NOT NULL REFERENCES superadmin.tenants(id),
    invoice_date    DATE NOT NULL,
    billing_from    DATE NOT NULL,
    billing_to      DATE NOT NULL,
    subtotal        NUMERIC(10,2) NOT NULL,
    discount_amount NUMERIC(10,2) NOT NULL DEFAULT 0,
    cgst_amount     NUMERIC(10,2) NOT NULL DEFAULT 0,
    sgst_amount     NUMERIC(10,2) NOT NULL DEFAULT 0,
    igst_amount     NUMERIC(10,2) NOT NULL DEFAULT 0,
    total_amount    NUMERIC(10,2) NOT NULL,
    payment_status  VARCHAR(20) NOT NULL DEFAULT 'pending',  -- pending/partial/paid/overdue
    pdf_url         TEXT,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE superadmin.payments (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    invoice_id      UUID NOT NULL REFERENCES superadmin.invoices(id),
    tenant_id       UUID NOT NULL REFERENCES superadmin.tenants(id),
    amount_paid     NUMERIC(10,2) NOT NULL,
    payment_method  VARCHAR(50),                        -- UPI/Card/NEFT/Cheque
    payment_ref     VARCHAR(200),                        -- UTR
    paid_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    recorded_by     UUID REFERENCES superadmin.internal_users(id)
);

-- ---------------------------------------------------------------------------
-- A7 — Multi-branch view is derived via tenants.parent_tenant_id (self-join);
-- no separate table needed. Helper view below.
-- ---------------------------------------------------------------------------
CREATE VIEW superadmin.v_branch_hierarchy AS
SELECT child.id AS branch_id, child.name AS branch_name,
       parent.id AS group_id, parent.name AS group_name
FROM superadmin.tenants child
JOIN superadmin.tenants parent ON child.parent_tenant_id = parent.id;

-- ---------------------------------------------------------------------------
-- A8 — System health snapshots (per tenant, polled periodically)
-- ---------------------------------------------------------------------------
CREATE TABLE superadmin.tenant_health_snapshots (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id         UUID NOT NULL REFERENCES superadmin.tenants(id),
    api_online        BOOLEAN NOT NULL,
    db_avg_query_ms   NUMERIC(6,2),
    redis_active      BOOLEAN,
    active_users      INTEGER,
    storage_used_gb   NUMERIC(10,2),
    storage_quota_gb  NUMERIC(10,2),
    active_modules_count SMALLINT,
    open_support_tickets SMALLINT DEFAULT 0,
    captured_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_health_tenant_time ON superadmin.tenant_health_snapshots (tenant_id, captured_at DESC);

-- ---------------------------------------------------------------------------
-- A9 — Global, append-only audit log (every action, every tenant)
-- ---------------------------------------------------------------------------
CREATE TABLE superadmin.audit_logs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    event_type      VARCHAR(100) NOT NULL,    -- TENANT_CREATED, MODULE_ACTIVATED, SUBSCRIPTION_LOCKED, etc.
    performed_by    UUID NOT NULL REFERENCES superadmin.internal_users(id),
    tenant_id       UUID REFERENCES superadmin.tenants(id),
    module_id       SMALLINT,
    previous_state  JSONB,
    new_state       JSONB,
    reason          TEXT,
    ip_address      INET,
    timestamp_ist   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_audit_tenant ON superadmin.audit_logs (tenant_id, timestamp_ist DESC);
CREATE INDEX idx_audit_event_type ON superadmin.audit_logs (event_type);
-- Append-only enforcement (A12): block UPDATE/DELETE at the DB level
REVOKE UPDATE, DELETE ON superadmin.audit_logs FROM PUBLIC;

COMMENT ON TABLE superadmin.audit_logs IS 'Append-only, undeletable (A12). Privileged role only — no patient clinical data ever stored here (DPDP hard rule).';
