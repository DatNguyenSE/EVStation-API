using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updateReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAt",
                table: "Receipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmedByStaffId",
                table: "Receipts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Receipts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ConfirmedByStaffId",
                table: "Receipts",
                column: "ConfirmedByStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_AspNetUsers_ConfirmedByStaffId",
                table: "Receipts",
                column: "ConfirmedByStaffId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_AspNetUsers_ConfirmedByStaffId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_ConfirmedByStaffId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ConfirmedByStaffId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Receipts");
        }
    }
}
