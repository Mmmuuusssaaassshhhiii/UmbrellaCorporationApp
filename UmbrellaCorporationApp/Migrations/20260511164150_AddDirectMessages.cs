using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbrellaCorporationApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyMessages_Employees_SentById",
                table: "EmergencyMessages");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyMessages_SentById",
                table: "EmergencyMessages");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "EmergencyMessages");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "EmergencyMessages");

            migrationBuilder.DropColumn(
                name: "SentById",
                table: "EmergencyMessages");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "EmergencyMessages",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "IsAcknowledged",
                table: "EmergencyMessages",
                newName: "IsRead");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EmergencyMessages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "EmergencyMessages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReceiverId",
                table: "EmergencyMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "EmergencyMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyMessages_ReceiverId",
                table: "EmergencyMessages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyMessages_SenderId",
                table: "EmergencyMessages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyMessages_Employees_ReceiverId",
                table: "EmergencyMessages",
                column: "ReceiverId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyMessages_Employees_SenderId",
                table: "EmergencyMessages",
                column: "SenderId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyMessages_Employees_ReceiverId",
                table: "EmergencyMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyMessages_Employees_SenderId",
                table: "EmergencyMessages");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyMessages_ReceiverId",
                table: "EmergencyMessages");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyMessages_SenderId",
                table: "EmergencyMessages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EmergencyMessages");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "EmergencyMessages");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "EmergencyMessages");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "EmergencyMessages");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "EmergencyMessages",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "EmergencyMessages",
                newName: "IsAcknowledged");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "EmergencyMessages",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                table: "EmergencyMessages",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SentById",
                table: "EmergencyMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyMessages_SentById",
                table: "EmergencyMessages",
                column: "SentById");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyMessages_Employees_SentById",
                table: "EmergencyMessages",
                column: "SentById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
