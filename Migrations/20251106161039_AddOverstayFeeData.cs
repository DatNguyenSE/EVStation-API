using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddOverstayFeeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Pricings",
                columns: new[] { "Id", "EffectiveFrom", "EffectiveTo", "IsActive", "Name", "PricePerKwh", "PricePerMinute", "PriceType" },
                values: new object[] { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Phí phạt quá giờ đặt chỗ", 0m, 2000m, "OverstayFee" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pricings",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
