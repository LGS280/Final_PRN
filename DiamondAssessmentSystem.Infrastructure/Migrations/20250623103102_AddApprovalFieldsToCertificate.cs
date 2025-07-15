using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondAssessmentSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalFieldsToCertificate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "issue_date",
                table: "Certificates",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "Certificates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Certificates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ApprovedBy",
                table: "Certificates",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Employees_ApprovedBy",
                table: "Certificates",
                column: "ApprovedBy",
                principalTable: "Employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Employees_ApprovedBy",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_ApprovedBy",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Certificates");

            migrationBuilder.AlterColumn<DateTime>(
                name: "issue_date",
                table: "Certificates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
