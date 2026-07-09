using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LokynexHealth.Infrastructure.Migrations.SyncDocsSchema
{
    /// <inheritdoc />
    public partial class AddIcuMonitoringModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "map_mmhg",
                schema: "hms",
                table: "icu_vitals");

            migrationBuilder.DropColumn(
                name: "spo2_pct",
                schema: "hms",
                table: "icu_vitals");

            migrationBuilder.DropColumn(
                name: "peep_cmh2o",
                schema: "hms",
                table: "icu_ventilator_records");

            migrationBuilder.AlterColumn<decimal>(
                name: "urine_output_ml_per_kg_hr",
                schema: "hms",
                table: "icu_vitals",
                type: "numeric(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "temperature_c",
                schema: "hms",
                table: "icu_vitals",
                type: "numeric(4,1)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<short>(
                name: "systolic_bp",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<short>(
                name: "respiratory_rate",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<short>(
                name: "heart_rate",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<short>(
                name: "diastolic_bp",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.Sql(
                "ALTER TABLE hms.icu_vitals ALTER COLUMN breach_parameters TYPE text[] USING breach_parameters::text[];");

            migrationBuilder.AlterColumn<List<string>>(
                name: "breach_parameters",
                schema: "hms",
                table: "icu_vitals",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "map_mm_hg",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "sp_o2_pct",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "tidal_volume_ml",
                schema: "hms",
                table: "icu_ventilator_records",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "mode",
                schema: "hms",
                table: "icu_ventilator_records",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "fio2_pct",
                schema: "hms",
                table: "icu_ventilator_records",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddColumn<decimal>(
                name: "peep_cm_h2_o",
                schema: "hms",
                table: "icu_ventilator_records",
                type: "numeric(4,1)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "recorded_by",
                schema: "hms",
                table: "icu_io_charts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "output_ml",
                schema: "hms",
                table: "icu_io_charts",
                type: "numeric(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "intake_ml",
                schema: "hms",
                table: "icu_io_charts",
                type: "numeric(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "score_value",
                schema: "hms",
                table: "icu_ai_scores",
                type: "numeric(6,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "score_type",
                schema: "hms",
                table: "icu_ai_scores",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "predicted_deterioration_window_hours",
                schema: "hms",
                table: "icu_ai_scores",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<Guid>(
                name: "ai_log_id",
                schema: "hms",
                table: "icu_ai_scores",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "hms",
                table: "icu_admissions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "sofa_score",
                schema: "hms",
                table: "icu_admissions",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "icu_unit_type",
                schema: "hms",
                table: "icu_admissions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "discharged_at",
                schema: "hms",
                table: "icu_admissions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<short>(
                name: "apache_ii_score",
                schema: "hms",
                table: "icu_admissions",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateIndex(
                name: "IX_icu_vitals_icu_admission_id_recorded_at",
                schema: "hms",
                table: "icu_vitals",
                columns: new[] { "icu_admission_id", "recorded_at" });

            migrationBuilder.CreateIndex(
                name: "IX_icu_ventilator_records_icu_admission_id",
                schema: "hms",
                table: "icu_ventilator_records",
                column: "icu_admission_id");

            migrationBuilder.CreateIndex(
                name: "IX_icu_io_charts_icu_admission_id_chart_date",
                schema: "hms",
                table: "icu_io_charts",
                columns: new[] { "icu_admission_id", "chart_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_icu_ai_scores_ai_log_id",
                schema: "hms",
                table: "icu_ai_scores",
                column: "ai_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_icu_ai_scores_icu_admission_id_score_type",
                schema: "hms",
                table: "icu_ai_scores",
                columns: new[] { "icu_admission_id", "score_type" });

            migrationBuilder.CreateIndex(
                name: "IX_icu_admissions_admission_id",
                schema: "hms",
                table: "icu_admissions",
                column: "admission_id");

            migrationBuilder.AddForeignKey(
                name: "FK_icu_admissions_admissions_admission_id",
                schema: "hms",
                table: "icu_admissions",
                column: "admission_id",
                principalSchema: "hms",
                principalTable: "admissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_icu_ai_scores_ai_interaction_log_ai_log_id",
                schema: "hms",
                table: "icu_ai_scores",
                column: "ai_log_id",
                principalSchema: "hms",
                principalTable: "ai_interaction_log",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_icu_ai_scores_icu_admissions_icu_admission_id",
                schema: "hms",
                table: "icu_ai_scores",
                column: "icu_admission_id",
                principalSchema: "hms",
                principalTable: "icu_admissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_icu_io_charts_icu_admissions_icu_admission_id",
                schema: "hms",
                table: "icu_io_charts",
                column: "icu_admission_id",
                principalSchema: "hms",
                principalTable: "icu_admissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_icu_ventilator_records_icu_admissions_icu_admission_id",
                schema: "hms",
                table: "icu_ventilator_records",
                column: "icu_admission_id",
                principalSchema: "hms",
                principalTable: "icu_admissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_icu_vitals_icu_admissions_icu_admission_id",
                schema: "hms",
                table: "icu_vitals",
                column: "icu_admission_id",
                principalSchema: "hms",
                principalTable: "icu_admissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_icu_admissions_admissions_admission_id",
                schema: "hms",
                table: "icu_admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_icu_ai_scores_ai_interaction_log_ai_log_id",
                schema: "hms",
                table: "icu_ai_scores");

            migrationBuilder.DropForeignKey(
                name: "FK_icu_ai_scores_icu_admissions_icu_admission_id",
                schema: "hms",
                table: "icu_ai_scores");

            migrationBuilder.DropForeignKey(
                name: "FK_icu_io_charts_icu_admissions_icu_admission_id",
                schema: "hms",
                table: "icu_io_charts");

            migrationBuilder.DropForeignKey(
                name: "FK_icu_ventilator_records_icu_admissions_icu_admission_id",
                schema: "hms",
                table: "icu_ventilator_records");

            migrationBuilder.DropForeignKey(
                name: "FK_icu_vitals_icu_admissions_icu_admission_id",
                schema: "hms",
                table: "icu_vitals");

            migrationBuilder.DropIndex(
                name: "IX_icu_vitals_icu_admission_id_recorded_at",
                schema: "hms",
                table: "icu_vitals");

            migrationBuilder.DropIndex(
                name: "IX_icu_ventilator_records_icu_admission_id",
                schema: "hms",
                table: "icu_ventilator_records");

            migrationBuilder.DropIndex(
                name: "IX_icu_io_charts_icu_admission_id_chart_date",
                schema: "hms",
                table: "icu_io_charts");

            migrationBuilder.DropIndex(
                name: "IX_icu_ai_scores_ai_log_id",
                schema: "hms",
                table: "icu_ai_scores");

            migrationBuilder.DropIndex(
                name: "IX_icu_ai_scores_icu_admission_id_score_type",
                schema: "hms",
                table: "icu_ai_scores");

            migrationBuilder.DropIndex(
                name: "IX_icu_admissions_admission_id",
                schema: "hms",
                table: "icu_admissions");

            migrationBuilder.DropColumn(
                name: "map_mm_hg",
                schema: "hms",
                table: "icu_vitals");

            migrationBuilder.DropColumn(
                name: "sp_o2_pct",
                schema: "hms",
                table: "icu_vitals");

            migrationBuilder.DropColumn(
                name: "peep_cm_h2_o",
                schema: "hms",
                table: "icu_ventilator_records");

            migrationBuilder.AlterColumn<decimal>(
                name: "urine_output_ml_per_kg_hr",
                schema: "hms",
                table: "icu_vitals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "temperature_c",
                schema: "hms",
                table: "icu_vitals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(4,1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "systolic_bp",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "respiratory_rate",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "heart_rate",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "diastolic_bp",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "breach_parameters",
                schema: "hms",
                table: "icu_vitals",
                type: "text",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AddColumn<short>(
                name: "map_mmhg",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "spo2_pct",
                schema: "hms",
                table: "icu_vitals",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "tidal_volume_ml",
                schema: "hms",
                table: "icu_ventilator_records",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "mode",
                schema: "hms",
                table: "icu_ventilator_records",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "fio2_pct",
                schema: "hms",
                table: "icu_ventilator_records",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "peep_cmh2o",
                schema: "hms",
                table: "icu_ventilator_records",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "recorded_by",
                schema: "hms",
                table: "icu_io_charts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "output_ml",
                schema: "hms",
                table: "icu_io_charts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "intake_ml",
                schema: "hms",
                table: "icu_io_charts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "score_value",
                schema: "hms",
                table: "icu_ai_scores",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(6,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "score_type",
                schema: "hms",
                table: "icu_ai_scores",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<short>(
                name: "predicted_deterioration_window_hours",
                schema: "hms",
                table: "icu_ai_scores",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ai_log_id",
                schema: "hms",
                table: "icu_ai_scores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "hms",
                table: "icu_admissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<short>(
                name: "sofa_score",
                schema: "hms",
                table: "icu_admissions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "icu_unit_type",
                schema: "hms",
                table: "icu_admissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "discharged_at",
                schema: "hms",
                table: "icu_admissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "apache_ii_score",
                schema: "hms",
                table: "icu_admissions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);
        }
    }
}
