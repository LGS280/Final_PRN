using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondAssessmentSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDateForMissingThisField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Service_prices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Service_prices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Results",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Service_prices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Service_prices");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AspNetUsers");
        }
    }
}
