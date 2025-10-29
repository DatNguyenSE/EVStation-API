using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePulledMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                keyValue: 21,
                column: "StationName",
                value: "");

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 22,
                column: "StationName",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 5,
                column: "StationName",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 6,
                column: "StationName",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 13,
                column: "StationName",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 14,
                column: "StationName",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 21,
                column: "StationName",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 22,
                column: "StationName",
                value: null);
        }
    }
}
