using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class CreateStationNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_AspNetUsers_AppUserId",
                table: "Receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_ChargingSessions_ChargingSessionId",
                table: "Receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_DriverPackages_PackageId",
                table: "Receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Receipt_ReceiptId",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipt",
                table: "Receipt");

            migrationBuilder.RenameTable(
                name: "Receipt",
                newName: "Receipts");

            migrationBuilder.RenameIndex(
                name: "IX_Receipt_PackageId",
                table: "Receipts",
                newName: "IX_Receipts_PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_Receipt_ChargingSessionId",
                table: "Receipts",
                newName: "IX_Receipts_ChargingSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Receipt_AppUserId",
                table: "Receipts",
                newName: "IX_Receipts_AppUserId");

            migrationBuilder.AddColumn<string>(
                name: "StationName",
                table: "ChargingPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Receipts",
                type: "int",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AddColumn<decimal>(
                name: "OverstayFee",
                table: "Receipts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 1,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 2,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 3,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 4,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 5,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 6,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 7,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 8,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 9,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 10,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 11,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 12,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 13,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 14,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 15,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 16,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 17,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 18,
                column: "StationName",
                value: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_AspNetUsers_AppUserId",
                table: "Receipts",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_ChargingSessions_ChargingSessionId",
                table: "Receipts",
                column: "ChargingSessionId",
                principalTable: "ChargingSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_DriverPackages_PackageId",
                table: "Receipts",
                column: "PackageId",
                principalTable: "DriverPackages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Receipts_ReceiptId",
                table: "WalletTransactions",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_AspNetUsers_AppUserId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_ChargingSessions_ChargingSessionId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_DriverPackages_PackageId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Receipts_ReceiptId",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "StationName",
                table: "ChargingPosts");

            migrationBuilder.DropColumn(
                name: "OverstayFee",
                table: "Receipts");

            migrationBuilder.RenameTable(
                name: "Receipts",
                newName: "Receipt");

            migrationBuilder.RenameIndex(
                name: "IX_Receipts_PackageId",
                table: "Receipt",
                newName: "IX_Receipt_PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_Receipts_ChargingSessionId",
                table: "Receipt",
                newName: "IX_Receipt_ChargingSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Receipts_AppUserId",
                table: "Receipt",
                newName: "IX_Receipt_AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Receipt",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 15);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipt",
                table: "Receipt",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_AspNetUsers_AppUserId",
                table: "Receipt",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_ChargingSessions_ChargingSessionId",
                table: "Receipt",
                column: "ChargingSessionId",
                principalTable: "ChargingSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_DriverPackages_PackageId",
                table: "Receipt",
                column: "PackageId",
                principalTable: "DriverPackages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Receipt_ReceiptId",
                table: "WalletTransactions",
                column: "ReceiptId",
                principalTable: "Receipt",
                principalColumn: "Id");
        }
    }
}
