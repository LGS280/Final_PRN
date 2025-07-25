using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondAssessmentSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiamondEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "diamond_id",
                table: "Results",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Diamonds",
                columns: table => new
                {
                    DiamondId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateReturn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diamonds", x => x.DiamondId);
                    table.ForeignKey(
                        name: "FK_Diamonds_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_Diamonds_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "request_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_diamond_id",
                table: "Results",
                column: "diamond_id");

            migrationBuilder.CreateIndex(
                name: "IX_Diamonds_EmployeeId",
                table: "Diamonds",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Diamonds_RequestId",
                table: "Diamonds",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Diamonds_diamond_id",
                table: "Results",
                column: "diamond_id",
                principalTable: "Diamonds",
                principalColumn: "DiamondId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Diamonds_diamond_id",
                table: "Results");

            migrationBuilder.DropTable(
                name: "Diamonds");

            migrationBuilder.DropIndex(
                name: "IX_Results_diamond_id",
                table: "Results");

            migrationBuilder.AlterColumn<int>(
                name: "diamond_id",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
