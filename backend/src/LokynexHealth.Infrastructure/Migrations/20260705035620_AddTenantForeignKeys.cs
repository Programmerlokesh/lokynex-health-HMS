using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LokynexHealth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Doctors_TenantId",
                table: "Doctors",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Tenants_TenantId",
                table: "Doctors",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Tenants_TenantId",
                table: "Patients",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Tenants_TenantId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Tenants_TenantId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_TenantId",
                table: "Doctors");
        }
    }
}
