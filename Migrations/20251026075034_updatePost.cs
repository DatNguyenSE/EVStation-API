using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updatePost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PowerKW",
                table: "ChargingPosts",
                type: "decimal(18,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "Q1-ULTRA-A", "CCS2", 150m, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "Q1-ULTRA-B", "CCS2", 150m, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "Q1-SC-A", "VinEScooter", 1.2m, 1, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "Q1-SC-B", "VinEScooter", 1.2m, 1, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "TD-Type2-A", "Type2", 11m, "Normal" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "TD-Type2-B", "Type2", 11m, "Normal" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "TD-CCS2-A", "CCS2", 60m, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "TD-CCS2-B", "CCS2", 60m, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "TD-ULTRA-A", "CCS2", 150m, 2, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "TD-ULTRA-B", "CCS2", 150m, 2, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "TD-SC-A", "VinEScooter", 1.2m, 2, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "TD-SC-B", "VinEScooter", 1.2m, 2, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "BD-Type2-A", "Type2", 11m, "Normal" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "BD-Type2-B", "Type2", 11m, "Normal" });

            migrationBuilder.InsertData(
                table: "ChargingPosts",
                columns: new[] { "Id", "Code", "ConnectorType", "IsWalkIn", "PowerKW", "QRCode", "StationId", "Status", "Type" },
                values: new object[,]
                {
                    { 19, "BD-CCS2-A", "CCS2", false, 60m, null, 3, "Available", "Fast" },
                    { 20, "BD-CCS2-B", "CCS2", true, 60m, null, 3, "Available", "Fast" },
                    { 21, "BD-ULTRA-A", "CCS2", false, 150m, null, 3, "Available", "Fast" },
                    { 22, "BD-ULTRA-B", "CCS2", true, 150m, null, 3, "Available", "Fast" },
                    { 23, "BD-SC-A", "VinEScooter", false, 1.2m, null, 3, "Available", "Scooter" },
                    { 24, "BD-SC-B", "VinEScooter", true, 1.2m, null, 3, "Available", "Scooter" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.AlterColumn<decimal>(
                name: "PowerKW",
                table: "ChargingPosts",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "Q1-SC-A", "VinEScooter", 1.2m, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "Q1-SC-B", "VinEScooter", 1.2m, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "TD-Type2-A", "Type2", 11m, 2, "Normal" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "TD-Type2-B", "Type2", 11m, 2, "Normal" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "TD-CCS2-A", "CCS2", 60m, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "TD-CCS2-B", "CCS2", 60m, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "TD-SC-A", "VinEScooter", 1.2m, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "TD-SC-B", "VinEScooter", 1.2m, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "BD-Type2-A", "Type2", 11m, 3, "Normal" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "BD-Type2-B", "Type2", 11m, 3, "Normal" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "BD-CCS2-A", "CCS2", 60m, 3, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "StationId", "Type" },
                values: new object[] { "BD-CCS2-B", "CCS2", 60m, 3, "Fast" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "BD-SC-A", "VinEScooter", 1.2m, "Scooter" });

            migrationBuilder.UpdateData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Code", "ConnectorType", "PowerKW", "Type" },
                values: new object[] { "BD-SC-B", "VinEScooter", 1.2m, "Scooter" });
        }
    }
}
