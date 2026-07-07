using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LokynexHealth.Infrastructure.Migrations.SyncDocsSchema
{
    /// <inheritdoc />
    public partial class AddPharmacyPosAndWardBedModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attendance_records",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    attendance_date = table.Column<DateOnly>(type: "date", nullable: false),
                    capture_method = table.Column<string>(type: "text", nullable: true),
                    check_in_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    check_out_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bed_demand_forecasts",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    forecast_for_date = table.Column<DateOnly>(type: "date", nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    predicted_occupancy_pct = table.Column<decimal>(type: "numeric", nullable: false),
                    ward_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bed_demand_forecasts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blood_crossmatch_requests",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_auto_checked = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blood_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    compatibility_result = table.Column<string>(type: "text", nullable: true),
                    is_emergency = table.Column<bool>(type: "boolean", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    requested_for_module = table.Column<string>(type: "text", nullable: true),
                    requested_for_record_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blood_crossmatch_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blood_demand_forecasts",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blood_group = table.Column<string>(type: "text", nullable: true),
                    forecast_for_week = table.Column<DateOnly>(type: "date", nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    predicted_shortage_units = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blood_demand_forecasts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blood_donors",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_retention_risk = table.Column<decimal>(type: "numeric", nullable: false),
                    blood_group = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    is_regular_donor = table.Column<bool>(type: "boolean", nullable: false),
                    last_donation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    mobile = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blood_donors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blood_issue_records",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    blood_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    crossmatch_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    issued_by = table.Column<Guid>(type: "uuid", nullable: false),
                    issued_to_patient_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blood_issue_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blood_testing_log",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    blood_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    result = table.Column<string>(type: "text", nullable: true),
                    test_name = table.Column<string>(type: "text", nullable: true),
                    tested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tested_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blood_testing_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blood_units",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    blood_group = table.Column<string>(type: "text", nullable: true),
                    collected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    component_type = table.Column<string>(type: "text", nullable: true),
                    donor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: false),
                    rh_factor = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    storage_location = table.Column<string>(type: "text", nullable: true),
                    unit_number = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blood_units", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "donor_eligibility_screening",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    deferral_reason = table.Column<string>(type: "text", nullable: true),
                    donor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hemoglobin_g_dl = table.Column<decimal>(type: "numeric", nullable: false),
                    is_eligible = table.Column<bool>(type: "boolean", nullable: false),
                    screened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    screened_by = table.Column<Guid>(type: "uuid", nullable: false),
                    weight_kg = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donor_eligibility_screening", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dpdp_audit_log",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "text", nullable: true),
                    actor_staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ip_address = table.Column<string>(type: "text", nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_table = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dpdp_audit_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "duty_rosters",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_staffing_synced = table.Column<bool>(type: "boolean", nullable: false),
                    department = table.Column<string>(type: "text", nullable: true),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shift_date = table.Column<DateOnly>(type: "date", nullable: false),
                    shift_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duty_rosters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employee_attrition_risk",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    computed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    risk_score = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_attrition_risk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employee_credentials",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    credential_type = table.Column<string>(type: "text", nullable: true),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    issuing_body = table.Column<string>(type: "text", nullable: true),
                    registration_number = table.Column<string>(type: "text", nullable: true),
                    valid_until = table.Column<DateOnly>(type: "date", nullable: false),
                    verified = table.Column<bool>(type: "boolean", nullable: false),
                    verified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_credentials", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aadhaar_number_enc = table.Column<byte[]>(type: "bytea", nullable: true),
                    bank_account_number_enc = table.Column<byte[]>(type: "bytea", nullable: true),
                    bank_ifsc = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    date_of_joining = table.Column<DateOnly>(type: "date", nullable: false),
                    department = table.Column<string>(type: "text", nullable: true),
                    designation = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    employee_code = table.Column<string>(type: "text", nullable: true),
                    esi_number = table.Column<string>(type: "text", nullable: true),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    mobile = table.Column<string>(type: "text", nullable: true),
                    pan_number = table.Column<string>(type: "text", nullable: true),
                    pf_uan_number = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "er_ambulance_calls",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    actual_arrival_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ambulance_service = table.Column<string>(type: "text", nullable: true),
                    dispatch_ref = table.Column<string>(type: "text", nullable: true),
                    er_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    predicted_eta_minutes = table.Column<short>(type: "smallint", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_er_ambulance_calls", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "er_critical_alerts",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_detected = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_type = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    detection_latency_seconds = table.Column<int>(type: "integer", nullable: false),
                    er_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_er_critical_alerts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "er_mlc_records",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    er_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    incident_type = table.Column<string>(type: "text", nullable: true),
                    mlc_number = table.Column<string>(type: "text", nullable: true),
                    police_sms_sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    police_station = table.Column<string>(type: "text", nullable: true),
                    reported_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_er_mlc_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "er_triage",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_suggested_level = table.Column<string>(type: "text", nullable: true),
                    er_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    esi_level = table.Column<string>(type: "text", nullable: true),
                    response_zone = table.Column<string>(type: "text", nullable: true),
                    triaged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    triaged_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_er_triage", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "er_visits",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    arrival_mode = table.Column<string>(type: "text", nullable: true),
                    arrived_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    disposition = table.Column<string>(type: "text", nullable: true),
                    disposition_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    visit_number = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_er_visits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fhir_export_log",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bundle_id = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pushed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    pushed_to_abha = table.Column<bool>(type: "boolean", nullable: false),
                    resource_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fhir_export_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "icu_admissions",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    admitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    apache_ii_score = table.Column<short>(type: "smallint", nullable: false),
                    discharged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    icu_unit_type = table.Column<string>(type: "text", nullable: true),
                    sofa_score = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_icu_admissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "icu_ai_scores",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    computed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    icu_admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    predicted_deterioration_window_hours = table.Column<short>(type: "smallint", nullable: false),
                    score_type = table.Column<string>(type: "text", nullable: true),
                    score_value = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_icu_ai_scores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "icu_io_charts",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chart_date = table.Column<DateOnly>(type: "date", nullable: false),
                    icu_admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    intake_ml = table.Column<decimal>(type: "numeric", nullable: false),
                    output_ml = table.Column<decimal>(type: "numeric", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_icu_io_charts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "icu_ventilator_records",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fio2_pct = table.Column<short>(type: "smallint", nullable: false),
                    icu_admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mode = table.Column<string>(type: "text", nullable: true),
                    peep_cmh2o = table.Column<decimal>(type: "numeric", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tidal_volume_ml = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_icu_ventilator_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "icu_vitals",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    breach_parameters = table.Column<string>(type: "text", nullable: true),
                    diastolic_bp = table.Column<short>(type: "smallint", nullable: false),
                    heart_rate = table.Column<short>(type: "smallint", nullable: false),
                    icu_admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_threshold_breach = table.Column<bool>(type: "boolean", nullable: false),
                    map_mmhg = table.Column<short>(type: "smallint", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    respiratory_rate = table.Column<short>(type: "smallint", nullable: false),
                    spo2_pct = table.Column<short>(type: "smallint", nullable: false),
                    systolic_bp = table.Column<short>(type: "smallint", nullable: false),
                    temperature_c = table.Column<decimal>(type: "numeric", nullable: false),
                    urine_output_ml_per_kg_hr = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_icu_vitals", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lab_orders",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordering_doctor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_visit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    scheme_tag = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ai_panel_suggested = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ordered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    released_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_lab_orders_ai_interaction_log_ai_log_id",
                        column: x => x.ai_log_id,
                        principalSchema: "hms",
                        principalTable: "ai_interaction_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_lab_orders_doctors_ordering_doctor_id",
                        column: x => x.ordering_doctor_id,
                        principalSchema: "hms",
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_lab_orders_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lab_test_catalog",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    test_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    loinc_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    specimen_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nabl_panel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    standard_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    cghs_price = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    tat_hours_std = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_test_catalog", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "leave_requests",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    is_anomalous = table.Column<bool>(type: "boolean", nullable: false),
                    leave_type = table.Column<string>(type: "text", nullable: true),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nabh_indicator_definitions",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    indicator_code = table.Column<string>(type: "text", nullable: true),
                    indicator_name = table.Column<string>(type: "text", nullable: true),
                    source_module = table.Column<string>(type: "text", nullable: true),
                    target_value = table.Column<decimal>(type: "numeric", nullable: false),
                    unit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nabh_indicator_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nabh_indicator_logs",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    indicator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    logged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    measured_value = table.Column<decimal>(type: "numeric", nullable: false),
                    measurement_period_end = table.Column<DateOnly>(type: "date", nullable: false),
                    measurement_period_start = table.Column<DateOnly>(type: "date", nullable: false),
                    source_record_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nabh_indicator_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_bookings",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_schedule_optimized = table.Column<bool>(type: "boolean", nullable: false),
                    booking_number = table.Column<string>(type: "text", nullable: true),
                    ot_room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    primary_surgeon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    procedure_name = table.Column<string>(type: "text", nullable: true),
                    scheduled_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    scheduled_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    time_range = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_bookings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_consent",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    consent_document_url = table.Column<string>(type: "text", nullable: true),
                    ot_booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    signed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    signed_by_patient_or_guardian = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_consent", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_consumable_forecasts",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    forecast_for_week = table.Column<DateOnly>(type: "date", nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    item_name = table.Column<string>(type: "text", nullable: true),
                    predicted_quantity_needed = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_consumable_forecasts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_implant_consumable_usage",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_or_serial_number = table.Column<string>(type: "text", nullable: true),
                    item_name = table.Column<string>(type: "text", nullable: true),
                    item_type = table.Column<string>(type: "text", nullable: true),
                    ot_booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity_used = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_implant_consumable_usage", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_intra_op_notes",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blood_loss_ml = table.Column<int>(type: "integer", nullable: false),
                    complications = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    ot_booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_by = table.Column<Guid>(type: "uuid", nullable: false),
                    ssi_risk_score = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_intra_op_notes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_post_op_recovery",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ot_booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recovery_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recovery_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    transferred_to_admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vitals_json = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_post_op_recovery", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_rooms",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    room_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_rooms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_safety_checklist",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    checklist_json = table.Column<string>(type: "text", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    ot_booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phase = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_safety_checklist", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ot_staff_allocation",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ot_booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_in_surgery = table.Column<string>(type: "text", nullable: true),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ot_staff_allocation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payroll_runs",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payroll_month = table.Column<DateOnly>(type: "date", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_runs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payslips",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    allowances = table.Column<decimal>(type: "numeric", nullable: false),
                    basic_salary = table.Column<decimal>(type: "numeric", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    esi_deduction = table.Column<decimal>(type: "numeric", nullable: false),
                    net_pay = table.Column<decimal>(type: "numeric", nullable: false),
                    other_deductions = table.Column<decimal>(type: "numeric", nullable: false),
                    paid_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    payroll_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payslip_pdf_url = table.Column<string>(type: "text", nullable: true),
                    pf_deduction = table.Column<decimal>(type: "numeric", nullable: false),
                    tds_deduction = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payslips", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pharmacy_claim_fraud_flags",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flag_reason = table.Column<string>(type: "text", nullable: true),
                    flagged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reviewed = table.Column<bool>(type: "boolean", nullable: false),
                    risk_score = table.Column<decimal>(type: "numeric", nullable: false),
                    sale_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacy_claim_fraud_flags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pharmacy_drug_catalog",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    drug_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    generic_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_jan_aushadhi_generic = table.Column<bool>(type: "boolean", nullable: false),
                    hsn_code = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    gst_rate_pct = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    schedule_flag = table.Column<string>(type: "text", nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacy_drug_catalog", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pharmacy_reorder_suggestions",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    actioned = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    drug_id = table.Column<Guid>(type: "uuid", nullable: false),
                    forecast_model = table.Column<string>(type: "text", nullable: true),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    suggested_quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacy_reorder_suggestions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pharmacy_sales",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: true),
                    prescription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    cgst_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    sgst_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    payment_status = table.Column<string>(type: "text", nullable: false),
                    payment_method = table.Column<string>(type: "text", nullable: true),
                    eway_bill_triggered = table.Column<bool>(type: "boolean", nullable: false),
                    eway_bill_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    sold_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacy_sales", x => x.id);
                    table.ForeignKey(
                        name: "FK_pharmacy_sales_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "portal_accounts",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    login_method = table.Column<string>(type: "text", nullable: true),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    preferred_language = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portal_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portal_appointments",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_no_show_risk = table.Column<decimal>(type: "numeric", nullable: false),
                    appointment_type = table.Column<string>(type: "text", nullable: true),
                    booked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    portal_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_slot = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portal_appointments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portal_bill_payments",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_paid = table.Column<decimal>(type: "numeric", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    paid_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    payment_gateway_ref = table.Column<string>(type: "text", nullable: true),
                    portal_account_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portal_bill_payments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portal_chatbot_sessions",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    escalated_to_er = table.Column<bool>(type: "boolean", nullable: false),
                    language = table.Column<string>(type: "text", nullable: true),
                    portal_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    transcript_json = table.Column<string>(type: "text", nullable: true),
                    triage_suggestion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portal_chatbot_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portal_feedback",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    comments = table.Column<string>(type: "text", nullable: true),
                    portal_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<short>(type: "smallint", nullable: false),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portal_feedback", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portal_medication_reminders",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_personalized = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    portal_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prescription_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reminder_time = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portal_medication_reminders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portal_record_access_logs",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accessed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    portal_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    record_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portal_record_access_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portal_teleconsultations",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    linked_opd_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    video_session_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portal_teleconsultations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "radiology_critical_alerts",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    acknowledged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    notified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    notified_doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    report_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_radiology_critical_alerts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "radiology_dicom_studies",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    acquired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    image_count = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pacs_storage_url = table.Column<string>(type: "text", nullable: true),
                    series_count = table.Column<short>(type: "smallint", nullable: false),
                    study_instance_uid = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_radiology_dicom_studies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "radiology_dose_logs",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    dose_unit = table.Column<string>(type: "text", nullable: true),
                    dose_value = table.Column<decimal>(type: "numeric", nullable: false),
                    equipment_id = table.Column<string>(type: "text", nullable: true),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    qa_check_passed = table.Column<bool>(type: "boolean", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_radiology_dose_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "radiology_modality_worklist",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    modality_aet = table.Column<string>(type: "text", nullable: true),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_radiology_modality_worklist", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "radiology_orders",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinical_indication = table.Column<string>(type: "text", nullable: true),
                    modality = table.Column<string>(type: "text", nullable: true),
                    order_number = table.Column<string>(type: "text", nullable: true),
                    ordered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ordering_doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: true),
                    source_module = table.Column<string>(type: "text", nullable: true),
                    source_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    study_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_radiology_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "radiology_reports",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_critical_finding = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_preliminary_findings = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    dicom_study_id = table.Column<Guid>(type: "uuid", nullable: false),
                    findings = table.Column<string>(type: "text", nullable: true),
                    impression = table.Column<string>(type: "text", nullable: true),
                    is_teleradiology = table.Column<bool>(type: "boolean", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pushed_to_abha = table.Column<bool>(type: "boolean", nullable: false),
                    radiologist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    signed_off = table.Column<bool>(type: "boolean", nullable: false),
                    signed_off_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_radiology_reports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "radiology_tat_predictions",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    predicted_tat_hours = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_radiology_tat_predictions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "report_definitions",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    query_definition = table.Column<string>(type: "text", nullable: true),
                    report_name = table.Column<string>(type: "text", nullable: true),
                    report_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "report_exports",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    export_format = table.Column<string>(type: "text", nullable: true),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    report_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    requested_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_exports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wards",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ward_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ward_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    floor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lab_samples",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sample_barcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    collected_by = table.Column<Guid>(type: "uuid", nullable: true),
                    collected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    received_at_lab = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejected = table.Column<bool>(type: "boolean", nullable: false),
                    rejection_reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_samples", x => x.id);
                    table.ForeignKey(
                        name: "FK_lab_samples_lab_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "hms",
                        principalTable: "lab_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lab_order_tests",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_applied = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_order_tests", x => x.id);
                    table.ForeignKey(
                        name: "FK_lab_order_tests_lab_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "hms",
                        principalTable: "lab_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lab_order_tests_lab_test_catalog_test_id",
                        column: x => x.test_id,
                        principalSchema: "hms",
                        principalTable: "lab_test_catalog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pharmacy_stock_batches",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    drug_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity_received = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    quantity_on_hand = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    purchase_price = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    mrp = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    supplier_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacy_stock_batches", x => x.id);
                    table.ForeignKey(
                        name: "FK_pharmacy_stock_batches_pharmacy_drug_catalog_drug_id",
                        column: x => x.drug_id,
                        principalSchema: "hms",
                        principalTable: "pharmacy_drug_catalog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "beds",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ward_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bed_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    bed_category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    daily_rate = table.Column<decimal>(type: "numeric(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beds", x => x.id);
                    table.ForeignKey(
                        name: "FK_beds_wards_ward_id",
                        column: x => x.ward_id,
                        principalSchema: "hms",
                        principalTable: "wards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lab_results",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_test_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sample_id = table.Column<Guid>(type: "uuid", nullable: true),
                    parameter_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    result_value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    reference_range = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_critical = table.Column<bool>(type: "boolean", nullable: false),
                    is_abnormal = table.Column<bool>(type: "boolean", nullable: false),
                    entered_by = table.Column<Guid>(type: "uuid", nullable: true),
                    validated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    validated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_lab_results_lab_order_tests_order_test_id",
                        column: x => x.order_test_id,
                        principalSchema: "hms",
                        principalTable: "lab_order_tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lab_results_lab_samples_sample_id",
                        column: x => x.sample_id,
                        principalSchema: "hms",
                        principalTable: "lab_samples",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "pharmacy_sale_items",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sale_id = table.Column<Guid>(type: "uuid", nullable: false),
                    drug_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    gst_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    interaction_checked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacy_sale_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_pharmacy_sale_items_pharmacy_drug_catalog_drug_id",
                        column: x => x.drug_id,
                        principalSchema: "hms",
                        principalTable: "pharmacy_drug_catalog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pharmacy_sale_items_pharmacy_sales_sale_id",
                        column: x => x.sale_id,
                        principalSchema: "hms",
                        principalTable: "pharmacy_sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pharmacy_sale_items_pharmacy_stock_batches_batch_id",
                        column: x => x.batch_id,
                        principalSchema: "hms",
                        principalTable: "pharmacy_stock_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "admissions",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    admission_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    admitting_doctor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    bed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pmjay_package_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    admitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    discharged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_admissions_beds_bed_id",
                        column: x => x.bed_id,
                        principalSchema: "hms",
                        principalTable: "beds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_admissions_doctors_admitting_doctor_id",
                        column: x => x.admitting_doctor_id,
                        principalSchema: "hms",
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_admissions_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "hms",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "housekeeping_tasks",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    assigned_to = table.Column<Guid>(type: "uuid", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_housekeeping_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_housekeeping_tasks_beds_bed_id",
                        column: x => x.bed_id,
                        principalSchema: "hms",
                        principalTable: "beds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lab_critical_alerts",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    result_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notified_doctor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notified_via = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ai_pre_alert = table.Column<bool>(type: "boolean", nullable: false),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    acknowledged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_critical_alerts", x => x.id);
                    table.ForeignKey(
                        name: "FK_lab_critical_alerts_ai_interaction_log_ai_log_id",
                        column: x => x.ai_log_id,
                        principalSchema: "hms",
                        principalTable: "ai_interaction_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_lab_critical_alerts_doctors_notified_doctor_id",
                        column: x => x.notified_doctor_id,
                        principalSchema: "hms",
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_lab_critical_alerts_lab_results_result_id",
                        column: x => x.result_id,
                        principalSchema: "hms",
                        principalTable: "lab_results",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bed_transfers",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_bed_id = table.Column<Guid>(type: "uuid", nullable: true),
                    to_bed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    isbar_identify = table.Column<string>(type: "text", nullable: true),
                    isbar_situation = table.Column<string>(type: "text", nullable: true),
                    isbar_background = table.Column<string>(type: "text", nullable: true),
                    isbar_assessment = table.Column<string>(type: "text", nullable: true),
                    isbar_recommendation = table.Column<string>(type: "text", nullable: true),
                    transferred_by = table.Column<Guid>(type: "uuid", nullable: true),
                    transferred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bed_transfers", x => x.id);
                    table.ForeignKey(
                        name: "FK_bed_transfers_admissions_admission_id",
                        column: x => x.admission_id,
                        principalSchema: "hms",
                        principalTable: "admissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bed_transfers_beds_from_bed_id",
                        column: x => x.from_bed_id,
                        principalSchema: "hms",
                        principalTable: "beds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_bed_transfers_beds_to_bed_id",
                        column: x => x.to_bed_id,
                        principalSchema: "hms",
                        principalTable: "beds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nursing_assessments",
                schema: "hms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    admission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    vitals_json = table.Column<string>(type: "jsonb", nullable: true),
                    gcs_score = table.Column<short>(type: "smallint", nullable: true),
                    braden_score = table.Column<short>(type: "smallint", nullable: true),
                    morse_fall_score = table.Column<short>(type: "smallint", nullable: true),
                    nrs2002_score = table.Column<short>(type: "smallint", nullable: true),
                    ai_los_prediction_days = table.Column<decimal>(type: "numeric(5,1)", nullable: true),
                    ai_readmission_risk = table.Column<decimal>(type: "numeric(5,4)", nullable: true),
                    ai_log_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assessed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nursing_assessments", x => x.id);
                    table.ForeignKey(
                        name: "FK_nursing_assessments_admissions_admission_id",
                        column: x => x.admission_id,
                        principalSchema: "hms",
                        principalTable: "admissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_nursing_assessments_ai_interaction_log_ai_log_id",
                        column: x => x.ai_log_id,
                        principalSchema: "hms",
                        principalTable: "ai_interaction_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admissions_admission_number",
                schema: "hms",
                table: "admissions",
                column: "admission_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admissions_admitting_doctor_id",
                schema: "hms",
                table: "admissions",
                column: "admitting_doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_admissions_bed_id",
                schema: "hms",
                table: "admissions",
                column: "bed_id");

            migrationBuilder.CreateIndex(
                name: "IX_admissions_patient_id",
                schema: "hms",
                table: "admissions",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_bed_transfers_admission_id",
                schema: "hms",
                table: "bed_transfers",
                column: "admission_id");

            migrationBuilder.CreateIndex(
                name: "IX_bed_transfers_from_bed_id",
                schema: "hms",
                table: "bed_transfers",
                column: "from_bed_id");

            migrationBuilder.CreateIndex(
                name: "IX_bed_transfers_to_bed_id",
                schema: "hms",
                table: "bed_transfers",
                column: "to_bed_id");

            migrationBuilder.CreateIndex(
                name: "IX_beds_status",
                schema: "hms",
                table: "beds",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_beds_ward_id_bed_number",
                schema: "hms",
                table: "beds",
                columns: new[] { "ward_id", "bed_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_housekeeping_tasks_bed_id",
                schema: "hms",
                table: "housekeeping_tasks",
                column: "bed_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_critical_alerts_ai_log_id",
                schema: "hms",
                table: "lab_critical_alerts",
                column: "ai_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_critical_alerts_notified_doctor_id",
                schema: "hms",
                table: "lab_critical_alerts",
                column: "notified_doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_critical_alerts_result_id",
                schema: "hms",
                table: "lab_critical_alerts",
                column: "result_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_order_tests_order_id",
                schema: "hms",
                table: "lab_order_tests",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_order_tests_test_id",
                schema: "hms",
                table: "lab_order_tests",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_orders_ai_log_id",
                schema: "hms",
                table: "lab_orders",
                column: "ai_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_orders_order_number",
                schema: "hms",
                table: "lab_orders",
                column: "order_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lab_orders_ordering_doctor_id",
                schema: "hms",
                table: "lab_orders",
                column: "ordering_doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_orders_patient_id",
                schema: "hms",
                table: "lab_orders",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_orders_status",
                schema: "hms",
                table: "lab_orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_lab_results_order_test_id",
                schema: "hms",
                table: "lab_results",
                column: "order_test_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_results_sample_id",
                schema: "hms",
                table: "lab_results",
                column: "sample_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_samples_order_id",
                schema: "hms",
                table: "lab_samples",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_samples_sample_barcode",
                schema: "hms",
                table: "lab_samples",
                column: "sample_barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lab_test_catalog_test_code",
                schema: "hms",
                table: "lab_test_catalog",
                column: "test_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nursing_assessments_admission_id",
                schema: "hms",
                table: "nursing_assessments",
                column: "admission_id");

            migrationBuilder.CreateIndex(
                name: "IX_nursing_assessments_ai_log_id",
                schema: "hms",
                table: "nursing_assessments",
                column: "ai_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacy_sale_items_batch_id",
                schema: "hms",
                table: "pharmacy_sale_items",
                column: "batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacy_sale_items_drug_id",
                schema: "hms",
                table: "pharmacy_sale_items",
                column: "drug_id");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacy_sale_items_sale_id",
                schema: "hms",
                table: "pharmacy_sale_items",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacy_sales_invoice_number",
                schema: "hms",
                table: "pharmacy_sales",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pharmacy_sales_patient_id",
                schema: "hms",
                table: "pharmacy_sales",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacy_stock_batches_drug_id_expiry_date",
                schema: "hms",
                table: "pharmacy_stock_batches",
                columns: new[] { "drug_id", "expiry_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendance_records",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "bed_demand_forecasts",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "bed_transfers",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "blood_crossmatch_requests",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "blood_demand_forecasts",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "blood_donors",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "blood_issue_records",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "blood_testing_log",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "blood_units",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "donor_eligibility_screening",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "dpdp_audit_log",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "duty_rosters",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "employee_attrition_risk",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "employee_credentials",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "employees",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "er_ambulance_calls",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "er_critical_alerts",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "er_mlc_records",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "er_triage",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "er_visits",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "fhir_export_log",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "housekeeping_tasks",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "icu_admissions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "icu_ai_scores",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "icu_io_charts",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "icu_ventilator_records",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "icu_vitals",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "lab_critical_alerts",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "leave_requests",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "nabh_indicator_definitions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "nabh_indicator_logs",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "nursing_assessments",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_bookings",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_consent",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_consumable_forecasts",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_implant_consumable_usage",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_intra_op_notes",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_post_op_recovery",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_rooms",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_safety_checklist",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "ot_staff_allocation",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "payroll_runs",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "payslips",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "pharmacy_claim_fraud_flags",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "pharmacy_reorder_suggestions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "pharmacy_sale_items",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "portal_accounts",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "portal_appointments",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "portal_bill_payments",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "portal_chatbot_sessions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "portal_feedback",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "portal_medication_reminders",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "portal_record_access_logs",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "portal_teleconsultations",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "radiology_critical_alerts",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "radiology_dicom_studies",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "radiology_dose_logs",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "radiology_modality_worklist",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "radiology_orders",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "radiology_reports",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "radiology_tat_predictions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "report_definitions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "report_exports",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "lab_results",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "admissions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "pharmacy_sales",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "pharmacy_stock_batches",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "lab_order_tests",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "lab_samples",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "beds",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "pharmacy_drug_catalog",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "lab_test_catalog",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "lab_orders",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "wards",
                schema: "hms");
        }
    }
}
