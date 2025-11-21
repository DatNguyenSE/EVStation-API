using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class SeedLargeTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Receipts",
                columns: new[] { "Id", "AppUserId", "ConfirmedAt", "ConfirmedByStaffId", "CreateAt", "DiscountAmount", "EnergyConsumed", "EnergyCost", "IdleEndTime", "IdleFee", "IdleStartTime", "OverstayFee", "PackageId", "PaymentMethod", "PricePerKwhSnapshot", "PricingName", "StationId", "Status", "TotalCost" },
                values: new object[,]
                {
                    { 101, "18", null, null, new DateTime(2025, 11, 21, 12, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 2, 1, 48000m },
                    { 102, "22", null, null, new DateTime(2025, 11, 21, 14, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 3, 1, 48000m },
                    { 103, "23", null, null, new DateTime(2025, 11, 21, 16, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 4, 1, 48000m },
                    { 104, "24", null, null, new DateTime(2025, 11, 21, 18, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 5, 1, 48000m },
                    { 105, "17", null, null, new DateTime(2025, 11, 21, 20, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 1, 1, 48000m },
                    { 106, "18", null, null, new DateTime(2025, 11, 21, 22, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 2, 1, 48000m },
                    { 107, "22", null, null, new DateTime(2025, 11, 22, 0, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 3, 1, 48000m },
                    { 108, "23", null, null, new DateTime(2025, 11, 22, 2, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 4, 1, 48000m },
                    { 109, "24", null, null, new DateTime(2025, 11, 22, 4, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 5, 1, 48000m },
                    { 110, "17", null, null, new DateTime(2025, 11, 22, 6, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 1, 1, 48000m },
                    { 111, "18", null, null, new DateTime(2025, 11, 22, 8, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 2, 1, 48000m },
                    { 112, "22", null, null, new DateTime(2025, 11, 22, 10, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 3, 1, 48000m },
                    { 113, "23", null, null, new DateTime(2025, 11, 22, 12, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 4, 1, 48000m },
                    { 114, "24", null, null, new DateTime(2025, 11, 22, 14, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 5, 1, 48000m },
                    { 115, "17", null, null, new DateTime(2025, 11, 22, 16, 30, 0, 0, DateTimeKind.Utc), 0m, 10m, 42000m, null, 6000m, null, 0m, null, null, 4200m, "Thành viên - Sạc nhanh DC", 1, 1, 48000m }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "BatteryCapacityKWh", "ConnectorType", "IsActive", "MaxChargingPowerKW", "Model", "OwnerId", "Plate", "RegistrationStatus", "Type", "VehicleRegistrationBackUrl", "VehicleRegistrationFrontUrl" },
                values: new object[,]
                {
                    { 2, 42.0, "CCS2", true, 60.0, "VF e34", "18", "51F-D12.34", "Approved", "Car", null, null },
                    { 3, 59.600000000000001, "CCS2", true, 150.0, "VF 6", "19", "60A-F98.76", "Approved", "Car", null, null },
                    { 4, 87.700000000000003, "CCS2", true, 150.0, "VF 8", "20", "51G-333.33", "Approved", "Car", null, null },
                    { 5, 18.640000000000001, "CCS2", true, 60.0, "VF 3", "21", "51C-555.55", "Approved", "Car", null, null },
                    { 6, 75.299999999999997, "CCS2", true, 150.0, "VF 7", "22", "51A-777.77", "Approved", "Car", null, null },
                    { 7, 3.5, "VinEScooter", true, 1.2, "Theon S", "23", "59-E56.78", "Approved", "Motorbike", null, null },
                    { 8, 3.5, "VinEScooter", true, 1.2, "Evo 200/200 Lite", "24", "51H-222.22", "Approved", "Motorbike", null, null },
                    { 9, 3.5, "VinEScooter", true, 1.2, "Klara S2 (2022)", "25", "59-B44.44", "Approved", "Motorbike", null, null },
                    { 10, 3.5, "VinEScooter", true, 1.2, "Feliz S", "26", "59-C66.66", "Approved", "Motorbike", null, null }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "ChargingPostId", "CreatedAt", "DriverId", "IsProcessedByDiscipline", "Status", "TimeSlotEnd", "TimeSlotStart", "VehicleId" },
                values: new object[,]
                {
                    { 101, 31, new DateTime(2025, 11, 21, 11, 0, 0, 0, DateTimeKind.Utc), "18", false, "Completed", new DateTime(2025, 11, 21, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 21, 11, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 102, 32, new DateTime(2025, 11, 21, 13, 0, 0, 0, DateTimeKind.Utc), "22", false, "Completed", new DateTime(2025, 11, 21, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 21, 13, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 103, 41, new DateTime(2025, 11, 21, 15, 0, 0, 0, DateTimeKind.Utc), "23", false, "Completed", new DateTime(2025, 11, 21, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 21, 15, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 104, 42, new DateTime(2025, 11, 21, 17, 0, 0, 0, DateTimeKind.Utc), "24", false, "Completed", new DateTime(2025, 11, 21, 18, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 21, 17, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 105, 43, new DateTime(2025, 11, 21, 19, 0, 0, 0, DateTimeKind.Utc), "17", false, "Completed", new DateTime(2025, 11, 21, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 21, 19, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 106, 25, new DateTime(2025, 11, 21, 21, 0, 0, 0, DateTimeKind.Utc), "18", false, "Completed", new DateTime(2025, 11, 21, 22, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 21, 21, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 107, 31, new DateTime(2025, 11, 21, 23, 0, 0, 0, DateTimeKind.Utc), "22", false, "Completed", new DateTime(2025, 11, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 21, 23, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 108, 32, new DateTime(2025, 11, 22, 1, 0, 0, 0, DateTimeKind.Utc), "23", false, "Completed", new DateTime(2025, 11, 22, 2, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 22, 1, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 109, 41, new DateTime(2025, 11, 22, 3, 0, 0, 0, DateTimeKind.Utc), "24", false, "Completed", new DateTime(2025, 11, 22, 4, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 22, 3, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 110, 42, new DateTime(2025, 11, 22, 5, 0, 0, 0, DateTimeKind.Utc), "17", false, "Completed", new DateTime(2025, 11, 22, 6, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 22, 5, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 111, 43, new DateTime(2025, 11, 22, 7, 0, 0, 0, DateTimeKind.Utc), "18", false, "Completed", new DateTime(2025, 11, 22, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 22, 7, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 112, 25, new DateTime(2025, 11, 22, 9, 0, 0, 0, DateTimeKind.Utc), "22", false, "Completed", new DateTime(2025, 11, 22, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 22, 9, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 113, 31, new DateTime(2025, 11, 22, 11, 0, 0, 0, DateTimeKind.Utc), "23", false, "Completed", new DateTime(2025, 11, 22, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 22, 11, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 114, 32, new DateTime(2025, 11, 22, 13, 0, 0, 0, DateTimeKind.Utc), "24", false, "Completed", new DateTime(2025, 11, 22, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 22, 13, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 115, 41, new DateTime(2025, 11, 22, 15, 0, 0, 0, DateTimeKind.Utc), "17", false, "Completed", new DateTime(2025, 11, 22, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 22, 15, 0, 0, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.InsertData(
                table: "ChargingSessions",
                columns: new[] { "Id", "ChargingPostId", "CompletedTime", "Cost", "EndBatteryPercentage", "EndTime", "EnergyConsumed", "IdleFee", "IdleFeeStartTime", "IsOverstay", "IsPaid", "IsWalkInSession", "OverstayFee", "ReceiptId", "ReservationId", "StartBatteryPercentage", "StartTime", "Status", "StopReason", "VehicleId", "VehiclePlate" },
                values: new object[,]
                {
                    { 101, 31, new DateTime(2025, 11, 21, 12, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 21, 12, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 101, 101, 20m, new DateTime(2025, 11, 21, 12, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-1" },
                    { 102, 32, new DateTime(2025, 11, 21, 14, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 21, 14, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 102, 102, 20m, new DateTime(2025, 11, 21, 14, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-2" },
                    { 103, 41, new DateTime(2025, 11, 21, 16, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 21, 16, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 103, 103, 20m, new DateTime(2025, 11, 21, 16, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-3" },
                    { 104, 42, new DateTime(2025, 11, 21, 18, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 21, 18, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 104, 104, 20m, new DateTime(2025, 11, 21, 18, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-4" },
                    { 105, 43, new DateTime(2025, 11, 21, 20, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 21, 20, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 105, 105, 20m, new DateTime(2025, 11, 21, 20, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-5" },
                    { 106, 25, new DateTime(2025, 11, 21, 22, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 21, 22, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 106, 106, 20m, new DateTime(2025, 11, 21, 22, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-6" },
                    { 107, 31, new DateTime(2025, 11, 22, 0, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 0, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 107, 107, 20m, new DateTime(2025, 11, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-7" },
                    { 108, 32, new DateTime(2025, 11, 22, 2, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 2, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 108, 108, 20m, new DateTime(2025, 11, 22, 2, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-8" },
                    { 109, 41, new DateTime(2025, 11, 22, 4, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 4, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 109, 109, 20m, new DateTime(2025, 11, 22, 4, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-9" },
                    { 110, 42, new DateTime(2025, 11, 22, 6, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 6, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 110, 110, 20m, new DateTime(2025, 11, 22, 6, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-10" },
                    { 111, 43, new DateTime(2025, 11, 22, 8, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 8, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 111, 111, 20m, new DateTime(2025, 11, 22, 8, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-11" },
                    { 112, 25, new DateTime(2025, 11, 22, 10, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 10, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 112, 112, 20m, new DateTime(2025, 11, 22, 10, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-12" },
                    { 113, 31, new DateTime(2025, 11, 22, 12, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 12, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 113, 113, 20m, new DateTime(2025, 11, 22, 12, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-13" },
                    { 114, 32, new DateTime(2025, 11, 22, 14, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 14, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 114, 114, 20m, new DateTime(2025, 11, 22, 14, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-14" },
                    { 115, 41, new DateTime(2025, 11, 22, 16, 30, 5, 0, DateTimeKind.Utc), 42000, 60m, new DateTime(2025, 11, 22, 16, 30, 0, 0, DateTimeKind.Utc), 10.0, 6000, null, false, true, false, 0, 115, 115, 20m, new DateTime(2025, 11, 22, 16, 0, 0, 0, DateTimeKind.Utc), "Completed", 0, null, "Demo-15" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "ChargingSessions",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "Receipts",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
