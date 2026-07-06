using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LokynexHealth.Infrastructure.Migrations.SyncDocsSchema
{
    /// <inheritdoc />
    public partial class sync_docs_schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hms");

            migrationBuilder.CreateTable(
                name: "ai_interaction_log",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    feature_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_record_table = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    source_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    input_payload = table.Column<string>(type: "jsonb", nullable: true),
                    ai_suggestion = table.Column<string>(type: "jsonb", nullable: true),
                    confidence_score = table.Column<decimal>(type: "numeric(5,4)", nullable: true),
                    model_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    model_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    review_status = table.Column<string>(type: "text", nullable: false),
                    reviewed_by_staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    override_reason = table.Column<string>(type: "text", nullable: true),
                    phi_sent_externally = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_interaction_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "billing_rate_masters",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheme_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    package_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    service_description = table.Column<string>(type: "text", nullable: true),
                    rate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_rate_masters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctors",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    nmc_registration_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    specialization = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    aadhaar_pki_cert_ref = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    uhid = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    abha_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "text", nullable: false),
                    aadhaar_number_enc = table.Column<byte[]>(type: "bytea", nullable: true),
                    aadhaar_last4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    pan_number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    mobile = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    alt_mobile = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pin_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    preferred_language = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    blood_group = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    is_duplicate_of = table.Column<Guid>(type: "uuid", nullable: true),
                    registered_at_tenant_branch = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "billing_invoices",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    admission_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invoice_date = table.Column<DateOnly>(type: "date", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    cgst_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    sgst_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    igst_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    payment_status = table.Column<string>(type: "text", nullable: false),
                    eway_bill_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_invoices", x => x.id);
                    table.ForeignKey(
                        name: "FK_billing_invoices_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "opd_token_queue",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    queue_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    checked_in_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    called_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opd_token_queue", x => x.id);
                    table.ForeignKey(
                        name: "FK_opd_token_queue_doctors_doctor_id",
                        column: x => x.doctor_id,
                        principalSchema: "hms",
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opd_token_queue_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_consents",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consent_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    granted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    captured_via = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_consents", x => x.id);
                    table.ForeignKey(
                        name: "FK_patient_consents_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patient_documents",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    ocr_extracted_json = table.Column<string>(type: "jsonb", nullable: true),
                    ocr_confidence = table.Column<decimal>(type: "numeric(5,4)", nullable: true),
                    uploaded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_patient_documents_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patient_insurance",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    insurance_type = table.Column<string>(type: "text", nullable: false),
                    scheme_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tpa_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: true),
                    valid_to = table.Column<DateOnly>(type: "date", nullable: true),
                    eligibility_verified = table.Column<bool>(type: "boolean", nullable: false),
                    eligibility_checked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_insurance", x => x.id);
                    table.ForeignKey(
                        name: "FK_patient_insurance_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "billing_invoice_items",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_module = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    source_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    hsn_sac_code = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    gst_rate_pct = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_invoice_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_billing_invoice_items_billing_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "hms",
                        principalTable: "billing_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "billing_payments",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    method = table.Column<string>(type: "text", nullable: false),
                    reference_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    paid_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_billing_payments_billing_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "hms",
                        principalTable: "billing_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "opd_visits",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    visit_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_id = table.Column<Guid>(type: "uuid", nullable: true),
                    scheme_tag = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    chief_complaint = table.Column<string>(type: "text", nullable: true),
                    visit_started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    visit_ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    signed_off = table.Column<bool>(type: "boolean", nullable: false),
                    signed_off_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    esign_qr_code_ref = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opd_visits", x => x.id);
                    table.ForeignKey(
                        name: "FK_opd_visits_doctors_doctor_id",
                        column: x => x.doctor_id,
                        principalSchema: "hms",
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opd_visits_opd_token_queue_token_id",
                        column: x => x.token_id,
                        principalSchema: "hms",
                        principalTable: "opd_token_queue",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_opd_visits_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "billing_claims",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_insurance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    claim_type = table.Column<string>(type: "text", nullable: false),
                    claim_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    claimed_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    approved_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    settled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_billing_claims_billing_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "hms",
                        principalTable: "billing_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_billing_claims_patient_insurance_patient_insurance_id",
                        column: x => x.patient_insurance_id,
                        principalSchema: "hms",
                        principalTable: "patient_insurance",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "billing_room_charges",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    charge_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    charge_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    daily_rate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    computed_amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    invoice_item_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_room_charges", x => x.id);
                    table.ForeignKey(
                        name: "FK_billing_room_charges_billing_invoice_items_invoice_item_id",
                        column: x => x.invoice_item_id,
                        principalSchema: "hms",
                        principalTable: "billing_invoice_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "opd_diagnoses",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    icd10_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    icd10_description = table.Column<string>(type: "text", nullable: true),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    ai_suggested = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opd_diagnoses", x => x.id);
                    table.ForeignKey(
                        name: "FK_opd_diagnoses_ai_interaction_log_ai_log_id",
                        column: x => x.ai_log_id,
                        principalSchema: "hms",
                        principalTable: "ai_interaction_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_opd_diagnoses_opd_visits_visit_id",
                        column: x => x.visit_id,
                        principalSchema: "hms",
                        principalTable: "opd_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "opd_investigation_orders",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    test_or_study_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    ordered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fulfilled_reference_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opd_investigation_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_opd_investigation_orders_opd_visits_visit_id",
                        column: x => x.visit_id,
                        principalSchema: "hms",
                        principalTable: "opd_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "opd_soap_notes",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subjective = table.Column<string>(type: "text", nullable: true),
                    objective = table.Column<string>(type: "text", nullable: true),
                    assessment = table.Column<string>(type: "text", nullable: true),
                    plan = table.Column<string>(type: "text", nullable: true),
                    source_language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ai_drafted = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: true),
                    finalized_by_doctor = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opd_soap_notes", x => x.id);
                    table.ForeignKey(
                        name: "FK_opd_soap_notes_ai_interaction_log_ai_log_id",
                        column: x => x.ai_log_id,
                        principalSchema: "hms",
                        principalTable: "ai_interaction_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_opd_soap_notes_opd_visits_visit_id",
                        column: x => x.visit_id,
                        principalSchema: "hms",
                        principalTable: "opd_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prescriptions",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prescription_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    pushed_to_abha = table.Column<bool>(type: "boolean", nullable: false),
                    pushed_to_pharmacy_queue = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_prescriptions_doctors_doctor_id",
                        column: x => x.doctor_id,
                        principalSchema: "hms",
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_prescriptions_opd_visits_visit_id",
                        column: x => x.visit_id,
                        principalSchema: "hms",
                        principalTable: "opd_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_prescriptions_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prescription_items",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prescription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    drug_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    rxnorm_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    dosage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    frequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    duration_days = table.Column<short>(type: "smallint", nullable: true),
                    schedule_flag = table.Column<string>(type: "text", nullable: false),
                    interaction_checked = table.Column<bool>(type: "boolean", nullable: false),
                    interaction_warning = table.Column<string>(type: "text", nullable: true),
                    allergy_warning = table.Column<string>(type: "text", nullable: true),
                    ai_autocompleted = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescription_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_prescription_items_ai_interaction_log_ai_log_id",
                        column: x => x.ai_log_id,
                        principalSchema: "hms",
                        principalTable: "ai_interaction_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_prescription_items_prescriptions_prescription_id",
                        column: x => x.prescription_id,
                        principalSchema: "hms",
                        principalTable: "prescriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_interaction_log_module_name_feature_name",
                schema: "hms",
                table: "ai_interaction_log",
                columns: new[] { "module_name", "feature_name" });

            migrationBuilder.CreateIndex(
                name: "IX_ai_interaction_log_source_record_table_source_record_id",
                schema: "hms",
                table: "ai_interaction_log",
                columns: new[] { "source_record_table", "source_record_id" });

            migrationBuilder.CreateIndex(
                name: "IX_billing_claims_invoice_id",
                schema: "hms",
                table: "billing_claims",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_claims_patient_insurance_id",
                schema: "hms",
                table: "billing_claims",
                column: "patient_insurance_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_invoice_items_invoice_id",
                schema: "hms",
                table: "billing_invoice_items",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_invoices_invoice_number",
                schema: "hms",
                table: "billing_invoices",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_billing_invoices_patient_id",
                schema: "hms",
                table: "billing_invoices",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_payments_invoice_id",
                schema: "hms",
                table: "billing_payments",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_room_charges_invoice_item_id",
                schema: "hms",
                table: "billing_room_charges",
                column: "invoice_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_doctors_nmc_registration_no",
                schema: "hms",
                table: "doctors",
                column: "nmc_registration_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_opd_diagnoses_ai_log_id",
                schema: "hms",
                table: "opd_diagnoses",
                column: "ai_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_opd_diagnoses_visit_id",
                schema: "hms",
                table: "opd_diagnoses",
                column: "visit_id");

            migrationBuilder.CreateIndex(
                name: "IX_opd_investigation_orders_visit_id",
                schema: "hms",
                table: "opd_investigation_orders",
                column: "visit_id");

            migrationBuilder.CreateIndex(
                name: "IX_opd_soap_notes_ai_log_id",
                schema: "hms",
                table: "opd_soap_notes",
                column: "ai_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_opd_soap_notes_visit_id",
                schema: "hms",
                table: "opd_soap_notes",
                column: "visit_id");

            migrationBuilder.CreateIndex(
                name: "IX_opd_token_queue_doctor_id_queue_date_token_number",
                schema: "hms",
                table: "opd_token_queue",
                columns: new[] { "doctor_id", "queue_date", "token_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_opd_token_queue_patient_id",
                schema: "hms",
                table: "opd_token_queue",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_opd_token_queue_queue_date_doctor_id",
                schema: "hms",
                table: "opd_token_queue",
                columns: new[] { "queue_date", "doctor_id" });

            migrationBuilder.CreateIndex(
                name: "IX_opd_visits_doctor_id_visit_started_at",
                schema: "hms",
                table: "opd_visits",
                columns: new[] { "doctor_id", "visit_started_at" });

            migrationBuilder.CreateIndex(
                name: "IX_opd_visits_patient_id",
                schema: "hms",
                table: "opd_visits",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_opd_visits_token_id",
                schema: "hms",
                table: "opd_visits",
                column: "token_id");

            migrationBuilder.CreateIndex(
                name: "IX_opd_visits_visit_number",
                schema: "hms",
                table: "opd_visits",
                column: "visit_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patient_consents_patient_id_consent_type",
                schema: "hms",
                table: "patient_consents",
                columns: new[] { "patient_id", "consent_type" });

            migrationBuilder.CreateIndex(
                name: "IX_patient_documents_patient_id",
                schema: "hms",
                table: "patient_documents",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_insurance_patient_id",
                schema: "hms",
                table: "patient_insurance",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_abha_id",
                schema: "hms",
                table: "patients",
                column: "abha_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_mobile",
                schema: "hms",
                table: "patients",
                column: "mobile");

            migrationBuilder.CreateIndex(
                name: "IX_patients_uhid",
                schema: "hms",
                table: "patients",
                column: "uhid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prescription_items_ai_log_id",
                schema: "hms",
                table: "prescription_items",
                column: "ai_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_prescription_items_prescription_id",
                schema: "hms",
                table: "prescription_items",
                column: "prescription_id");

            migrationBuilder.CreateIndex(
                name: "IX_prescriptions_doctor_id",
                schema: "hms",
                table: "prescriptions",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_prescriptions_patient_id",
                schema: "hms",
                table: "prescriptions",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_prescriptions_prescription_number",
                schema: "hms",
                table: "prescriptions",
                column: "prescription_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prescriptions_visit_id",
                schema: "hms",
                table: "prescriptions",
                column: "visit_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "billing_claims",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "billing_payments",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "billing_rate_masters",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "billing_room_charges",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "opd_diagnoses",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "opd_investigation_orders",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "opd_soap_notes",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "patient_consents",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "patient_documents",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "prescription_items",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "patient_insurance",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "billing_invoice_items",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ai_interaction_log",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "prescriptions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "billing_invoices",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "opd_visits",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "opd_token_queue",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "doctors",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "patients",
                schema: "hms");
        }
    }
}
