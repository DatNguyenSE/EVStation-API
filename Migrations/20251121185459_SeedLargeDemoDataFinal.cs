using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class SeedLargeDemoDataFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4", "2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5", "4" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "5" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "6" });

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ChargingPosts",
                keyColumn: "Id",
                keyValue: 18);

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

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "3", "2" },
                    { "3", "4" },
                    { "3", "5" },
                    { "3", "6" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1995, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager1@evsystem.com", "Lý Quản Lý 1", "MANAGER1@EVSYSTEM.COM", "MANAGER1", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999000001", "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", "manager1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1995, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager2@evsystem.com", "Lý Quản Lý 2", "MANAGER2@EVSYSTEM.COM", "MANAGER2", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999000002", "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", "manager2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1995, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager3@evsystem.com", "Lý Quản Lý 3", "MANAGER3@EVSYSTEM.COM", "MANAGER3", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999000003", "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", "manager3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1995, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager4@evsystem.com", "Lý Quản Lý 4", "MANAGER4@EVSYSTEM.COM", "MANAGER4", "0999000004", "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", "manager4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1995, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager5@evsystem.com", "Lý Quản Lý 5", "MANAGER5@EVSYSTEM.COM", "MANAGER5", "0999000005", "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", "manager5" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "10", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1996, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "operator4@evsystem.com", true, "Trần Vận Hành 4", false, null, "OPERATOR4@EVSYSTEM.COM", "OPERATOR4", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999100004", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "operator4" },
                    { "11", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1996, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "operator5@evsystem.com", true, "Trần Vận Hành 5", false, null, "OPERATOR5@EVSYSTEM.COM", "OPERATOR5", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999100005", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "operator5" },
                    { "12", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1997, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "tech1@evsystem.com", true, "Hoàng Kỹ Thuật 1", false, null, "TECH1@EVSYSTEM.COM", "TECH1", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999200001", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "tech1" },
                    { "13", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1997, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "tech2@evsystem.com", true, "Hoàng Kỹ Thuật 2", false, null, "TECH2@EVSYSTEM.COM", "TECH2", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999200002", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "tech2" },
                    { "14", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1997, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "tech3@evsystem.com", true, "Hoàng Kỹ Thuật 3", false, null, "TECH3@EVSYSTEM.COM", "TECH3", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999200003", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "tech3" },
                    { "15", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1997, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "tech4@evsystem.com", true, "Hoàng Kỹ Thuật 4", false, null, "TECH4@EVSYSTEM.COM", "TECH4", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999200004", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "tech4" },
                    { "16", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1997, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "tech5@evsystem.com", true, "Hoàng Kỹ Thuật 5", false, null, "TECH5@EVSYSTEM.COM", "TECH5", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999200005", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "tech5" },
                    { "17", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver1@evsystem.com", true, "Phạm Tài Xế 1", false, null, "DRIVER1@EVSYSTEM.COM", "DRIVER1", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver1" },
                    { "18", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver2@evsystem.com", true, "Phạm Tài Xế 2", false, null, "DRIVER2@EVSYSTEM.COM", "DRIVER2", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver2" },
                    { "19", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver3@evsystem.com", true, "Phạm Tài Xế 3", false, null, "DRIVER3@EVSYSTEM.COM", "DRIVER3", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver3" },
                    { "20", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver4@evsystem.com", true, "Phạm Tài Xế 4", false, null, "DRIVER4@EVSYSTEM.COM", "DRIVER4", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver4" },
                    { "21", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver5@evsystem.com", true, "Phạm Tài Xế 5", false, null, "DRIVER5@EVSYSTEM.COM", "DRIVER5", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver5" },
                    { "22", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver6@evsystem.com", true, "Phạm Tài Xế 6", false, null, "DRIVER6@EVSYSTEM.COM", "DRIVER6", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver6" },
                    { "23", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver7@evsystem.com", true, "Phạm Tài Xế 7", false, null, "DRIVER7@EVSYSTEM.COM", "DRIVER7", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver7" },
                    { "24", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver8@evsystem.com", true, "Phạm Tài Xế 8", false, null, "DRIVER8@EVSYSTEM.COM", "DRIVER8", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver8" },
                    { "25", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver9@evsystem.com", true, "Phạm Tài Xế 9", false, null, "DRIVER9@EVSYSTEM.COM", "DRIVER9", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver9" },
                    { "26", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1998, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "driver10@evsystem.com", true, "Phạm Tài Xế 10", false, null, "DRIVER10@EVSYSTEM.COM", "DRIVER10", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", null, false, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "driver10" },
                    { "7", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1996, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "operator1@evsystem.com", true, "Trần Vận Hành 1", false, null, "OPERATOR1@EVSYSTEM.COM", "OPERATOR1", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999100001", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "operator1" },
                    { "8", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1996, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "operator2@evsystem.com", true, "Trần Vận Hành 2", false, null, "OPERATOR2@EVSYSTEM.COM", "OPERATOR2", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999100002", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "operator2" },
                    { "9", 0, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", new DateTime(1996, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "operator3@evsystem.com", true, "Trần Vận Hành 3", false, null, "OPERATOR3@EVSYSTEM.COM", "OPERATOR3", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0999100003", true, "A1B2C3D4-E5F6-7890-1234-567890ABCDEF", false, "operator3" }
                });

            migrationBuilder.InsertData(
                table: "Assignments",
                columns: new[] { "Id", "EffectiveFrom", "EffectiveTo", "IsActive", "StaffId", "StationId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "2", 1 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "3", 2 },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "4", 3 }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "CloseTime", "Code", "Description", "Latitude", "Longitude", "Name", "OpenTime", "Status" },
                values: new object[,]
                {
                    { 4, "Sân bay Tân Sơn Nhất, Quận Tân Bình, TP.HCM", new TimeSpan(0, 22, 0, 0, 0), "HCM04", "Trạm sạc tại khu vực sân bay, tiện cho xe công nghệ", 10.8169828, 106.64705360000001, "Trạm sạc VinFast Sân bay", new TimeSpan(0, 6, 0, 0, 0), "Active" },
                    { 5, "172 Nguyễn Văn Linh, Quận 7, TP.HCM", new TimeSpan(0, 22, 0, 0, 0), "HCM05", "Trạm sạc khu vực Phú Mỹ Hưng, tập trung nhiều xe ô tô", 10.751982699999999, 106.7210539, "Trạm sạc VinFast Quận 7", new TimeSpan(0, 6, 0, 0, 0), "Active" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "4", "10" },
                    { "4", "11" },
                    { "5", "12" },
                    { "5", "13" },
                    { "5", "14" },
                    { "5", "15" },
                    { "5", "16" },
                    { "2", "17" },
                    { "2", "18" },
                    { "2", "19" },
                    { "2", "20" },
                    { "2", "21" },
                    { "2", "22" },
                    { "2", "23" },
                    { "2", "24" },
                    { "2", "25" },
                    { "2", "26" },
                    { "4", "7" },
                    { "4", "8" },
                    { "4", "9" }
                });

            migrationBuilder.InsertData(
                table: "Assignments",
                columns: new[] { "Id", "EffectiveFrom", "EffectiveTo", "IsActive", "StaffId", "StationId" },
                values: new object[,]
                {
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "7", 1 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "12", 1 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "8", 2 },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "13", 2 },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "9", 3 },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "14", 3 },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "5", 4 },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "10", 4 },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "15", 4 },
                    { 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "6", 5 },
                    { 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "11", 5 },
                    { 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "16", 5 }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "Dept", "IsDept", "UserId" },
                values: new object[,]
                {
                    { 1, 500000m, 0m, false, "17" },
                    { 2, 100000m, 0m, false, "18" },
                    { 3, 0m, 25000m, true, "19" },
                    { 4, 200000m, 0m, false, "20" },
                    { 5, 0m, 0m, false, "21" },
                    { 6, 150000m, 0m, false, "22" },
                    { 7, 50000m, 0m, false, "23" },
                    { 8, 0m, 10000m, true, "24" },
                    { 9, 300000m, 0m, false, "25" },
                    { 10, 50000m, 0m, false, "26" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4", "10" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4", "11" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5", "12" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5", "13" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5", "14" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5", "15" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5", "16" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "17" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "18" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "19" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3", "2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "20" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "21" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "22" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "23" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "24" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "25" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "26" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3", "4" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3", "5" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3", "6" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4", "7" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4", "8" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4", "9" });

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "10");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "12");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "13");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "14");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "15");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "16");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "17");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "18");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "19");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "20");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "21");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "23");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "24");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "25");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "26");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9");

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "4", "2" },
                    { "5", "4" },
                    { "2", "5" },
                    { "2", "6" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "11111111-2222-3333-4444-555555555555", new DateTime(1995, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "operator@evsystem.com", "Trạm Operator", "OPERATOR@EVSYSTEM.COM", "OPERATOR", "AQAAAAIAAYagAAAAEPti/a9dQXrb7L6sjniNdM3QWjQhWtlZLB7tQwUaCxsyewD+D8MBhuXsE4afjntGfg==", "0911111111", "A1111111-B222-4333-C444-D55555555555", "operator" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "66666666-7777-8888-9999-AAAAAAAAAAAA", new DateTime(1992, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager@evsystem.com", "Khu vực Manager", "MANAGER@EVSYSTEM.COM", "MANAGER", "AQAAAAIAAYagAAAAENMyFIG2LA4//qtHgDgkZB8TC+wvdKnkwxiD6JHIkMCX0dd+twv8zV7ea/CMfQnChw==", "0922222222", "B1111111-C222-4333-D444-E55555555555", "manager" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF", new DateTime(1994, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "technician@evsystem.com", "Kỹ thuật viên bảo trì", "TECHNICIAN@EVSYSTEM.COM", "TECHNICIAN", "AQAAAAIAAYagAAAAEKV4vb55tRNp0q0sO0pF/Ua5A46af0IC1l5PZuNofciWemJVAk7vjQYutf5YQKjxfQ==", "0933333333", "C1111111-D222-4333-E444-F55555555555", "technician" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF", new DateTime(1994, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "d1@evsystem.com", "Tài xế 1", "D1@EVSYSTEM.COM", "D1", "0933334333", "C1111111-D222-4333-E444-F55555555555", "d1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "Email", "FullName", "NormalizedEmail", "NormalizedUserName", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[] { "BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF", new DateTime(1994, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "d2@evsystem.com", "Tài xế 2", "D2@EVSYSTEM.COM", "D2", "0933333343", "C1111111-D222-4333-E444-F55555555555", "d2" });

            migrationBuilder.InsertData(
                table: "ChargingPosts",
                columns: new[] { "Id", "Code", "ConnectorType", "IsWalkIn", "PowerKW", "QRCode", "StationId", "StationName", "Status", "Type" },
                values: new object[,]
                {
                    { 1, "HCM01-CHG001", "Type2", false, 11m, null, 1, "", "Available", "Normal" },
                    { 2, "HCM01-CHG002", "Type2", true, 11m, null, 1, "", "Available", "Normal" },
                    { 3, "HCM01-CHG003", "CCS2", false, 60m, null, 1, "", "Available", "Fast" },
                    { 4, "HCM01-CHG004", "CCS2", true, 60m, null, 1, "", "Available", "Fast" },
                    { 5, "HCM01-CHG005", "CCS2", false, 150m, null, 1, "", "Available", "Fast" },
                    { 6, "HCM01-CHG006", "CCS2", true, 150m, null, 1, "", "Available", "Fast" },
                    { 7, "HCM01-CHG007", "VinEScooter", false, 1.2m, null, 1, "", "Available", "Scooter" },
                    { 8, "HCM01-CHG008", "VinEScooter", true, 1.2m, null, 1, "", "Available", "Scooter" },
                    { 9, "HCM02-CHG001", "Type2", false, 11m, null, 2, "", "Available", "Normal" },
                    { 10, "HCM02-CHG002", "Type2", true, 11m, null, 2, "", "Available", "Normal" },
                    { 11, "HCM02-CHG003", "CCS2", false, 60m, null, 2, "", "Available", "Fast" },
                    { 12, "HCM02-CHG004", "CCS2", true, 60m, null, 2, "", "Available", "Fast" },
                    { 13, "HCM02-CHG005", "CCS2", false, 150m, null, 2, "", "Available", "Fast" },
                    { 14, "HCM02-CHG006", "CCS2", true, 150m, null, 2, "", "Available", "Fast" },
                    { 15, "HCM02-CHG007", "VinEScooter", false, 1.2m, null, 2, "", "Available", "Scooter" },
                    { 16, "HCM02-CHG008", "VinEScooter", true, 1.2m, null, 2, "", "Available", "Scooter" },
                    { 17, "BD03-CHG001", "Type2", false, 11m, null, 3, "", "Available", "Normal" },
                    { 18, "BD03-CHG002", "Type2", true, 11m, null, 3, "", "Available", "Normal" },
                    { 19, "BD03-CHG003", "CCS2", false, 60m, null, 3, "", "Available", "Fast" },
                    { 20, "BD03-CHG004", "CCS2", true, 60m, null, 3, "", "Available", "Fast" },
                    { 21, "BD03-CHG005", "CCS2", false, 150m, null, 3, "", "Available", "Fast" },
                    { 22, "BD03-CHG006", "CCS2", true, 150m, null, 3, "", "Available", "Fast" },
                    { 23, "BD03-CHG007", "VinEScooter", false, 1.2m, null, 3, "", "Available", "Scooter" },
                    { 24, "BD03-CHG008", "VinEScooter", true, 1.2m, null, 3, "", "Available", "Scooter" }
                });
        }
    }
}
