using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserandPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "5", 0, "BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF", new DateTime(1994, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "d1@evsystem.com", true, "Tài xế 1", false, null, "D1@EVSYSTEM.COM", "D1", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0933334333", true, "C1111111-D222-4333-E444-F55555555555", false, "d1" },
                    { "6", 0, "BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF", new DateTime(1994, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "d2@evsystem.com", true, "Tài xế 2", false, null, "D2@EVSYSTEM.COM", "D2", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0933333343", true, "C1111111-D222-4333-E444-F55555555555", false, "d2" }
                });

            migrationBuilder.InsertData(
                table: "ChargingPackages",
                columns: new[] { "Id", "CreatedAt", "Description", "DurationDays", "IsActive", "Name", "Price", "VehicleType" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sử dụng trạm sạc không giới hạn trong 30 ngày cho xe máy điện.", 30, true, "Gói Xe Máy 30 Ngày Không Giới Hạn", 99000m, "Motorbike" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sử dụng trạm sạc không giới hạn trong 30 ngày cho ô tô điện.", 30, true, "Gói Ô Tô 30 Ngày Không Giới Hạn", 499000m, "Car" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "2", "5" },
                    { "2", "6" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "5" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "6" });

            migrationBuilder.DeleteData(
                table: "ChargingPackages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ChargingPackages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6");
        }
    }
}
