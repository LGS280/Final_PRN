using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondAssessmentSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityToConversationAndChatLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ChatLogs__custom__72C60C4A",
                table: "ChatLogs");

            migrationBuilder.DropForeignKey(
                name: "FK__ChatLogs__employ__73BA3083",
                table: "ChatLogs");

            migrationBuilder.DropForeignKey(
                name: "FK__ChatLogs__reques__71D1E811",
                table: "ChatLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ChatLogs__FD040B175AFF3BC5",
                table: "ChatLogs");

            migrationBuilder.DropIndex(
                name: "IX_ChatLogs_customer_id",
                table: "ChatLogs");

            migrationBuilder.DropIndex(
                name: "IX_ChatLogs_employee_id",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "ChatLogs");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "ChatLogs",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "request_id",
                table: "ChatLogs",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "message_type",
                table: "ChatLogs",
                newName: "MessageType");

            migrationBuilder.RenameColumn(
                name: "chat_id",
                table: "ChatLogs",
                newName: "ChatId");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "ChatLogs",
                newName: "FileSize");

            migrationBuilder.RenameIndex(
                name: "IX_ChatLogs_request_id",
                table: "ChatLogs",
                newName: "IX_ChatLogs_RequestId");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ChatLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MessageType",
                table: "ChatLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "ChatLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ChatLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ChatLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "ChatLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SavedFileName",
                table: "ChatLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "ChatLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "ChatLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderRole",
                table: "ChatLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "ChatLogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatLogs",
                table: "ChatLogs",
                column: "ChatId");

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "open")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ConversationId);
                    table.ForeignKey(
                        name: "FK_Conversations_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "employee_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatLogs_ConversationId",
                table: "ChatLogs",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_CustomerId",
                table: "Conversations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_EmployeeId",
                table: "Conversations",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatLogs_Conversations_ConversationId",
                table: "ChatLogs",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatLogs_Requests_RequestId",
                table: "ChatLogs",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "request_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatLogs_Conversations_ConversationId",
                table: "ChatLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatLogs_Requests_RequestId",
                table: "ChatLogs");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatLogs",
                table: "ChatLogs");

            migrationBuilder.DropIndex(
                name: "IX_ChatLogs_ConversationId",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "SavedFileName",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "SenderRole",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "ChatLogs");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "ChatLogs",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "ChatLogs",
                newName: "request_id");

            migrationBuilder.RenameColumn(
                name: "MessageType",
                table: "ChatLogs",
                newName: "message_type");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "ChatLogs",
                newName: "chat_id");

            migrationBuilder.RenameColumn(
                name: "FileSize",
                table: "ChatLogs",
                newName: "employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_ChatLogs_RequestId",
                table: "ChatLogs",
                newName: "IX_ChatLogs_request_id");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "ChatLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "message_type",
                table: "ChatLogs",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "ChatLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "timestamp",
                table: "ChatLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK__ChatLogs__FD040B175AFF3BC5",
                table: "ChatLogs",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatLogs_customer_id",
                table: "ChatLogs",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatLogs_employee_id",
                table: "ChatLogs",
                column: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "FK__ChatLogs__custom__72C60C4A",
                table: "ChatLogs",
                column: "customer_id",
                principalTable: "Customers",
                principalColumn: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "FK__ChatLogs__employ__73BA3083",
                table: "ChatLogs",
                column: "employee_id",
                principalTable: "Employees",
                principalColumn: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "FK__ChatLogs__reques__71D1E811",
                table: "ChatLogs",
                column: "request_id",
                principalTable: "Requests",
                principalColumn: "request_id");
        }
    }
}
