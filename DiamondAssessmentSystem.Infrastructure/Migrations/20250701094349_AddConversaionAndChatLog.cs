using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondAssessmentSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConversaionAndChatLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Employees_ApprovedBy",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_ApprovedBy",
                table: "Certificates");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByEmployeeEmployeeId",
                table: "Certificates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ApprovedByEmployeeEmployeeId",
                table: "Certificates",
                column: "ApprovedByEmployeeEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Employees_ApprovedByEmployeeEmployeeId",
                table: "Certificates",
                column: "ApprovedByEmployeeEmployeeId",
                principalTable: "Employees",
                principalColumn: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Employees_ApprovedByEmployeeEmployeeId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_ApprovedByEmployeeEmployeeId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ApprovedByEmployeeEmployeeId",
                table: "Certificates");

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
    }
}
