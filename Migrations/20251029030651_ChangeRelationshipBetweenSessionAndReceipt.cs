using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationshipBetweenSessionAndReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_ChargingSessions_ChargingSessionId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_ChargingSessionId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ChargingSessionId",
                table: "Receipts");

            migrationBuilder.AddColumn<int>(
                name: "ReceiptId",
                table: "ChargingSessions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChargingSessions_ReceiptId",
                table: "ChargingSessions",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSessions_Receipts_ReceiptId",
                table: "ChargingSessions",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSessions_Receipts_ReceiptId",
                table: "ChargingSessions");

            migrationBuilder.DropIndex(
                name: "IX_ChargingSessions_ReceiptId",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "ChargingSessions");

            migrationBuilder.AddColumn<int>(
                name: "ChargingSessionId",
                table: "Receipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ChargingSessionId",
                table: "Receipts",
                column: "ChargingSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_ChargingSessions_ChargingSessionId",
                table: "Receipts",
                column: "ChargingSessionId",
                principalTable: "ChargingSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
