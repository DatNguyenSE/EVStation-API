using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSessions_ChargingPosts_PostId",
                table: "ChargingSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pricing",
                table: "Pricing");

            migrationBuilder.RenameTable(
                name: "Pricing",
                newName: "Pricings");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "ChargingSessions",
                newName: "ChargingPostId");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingSessions_PostId",
                table: "ChargingSessions",
                newName: "IX_ChargingSessions_ChargingPostId");

            migrationBuilder.AddColumn<int>(
                name: "ReceiptId",
                table: "WalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Dept",
                table: "Wallets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDept",
                table: "Wallets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pricings",
                table: "Pricings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Assignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    StaffId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignment_AspNetUsers_StaffId",
                        column: x => x.StaffId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignment_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    ChargingSessionId = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PackageId = table.Column<int>(type: "int", nullable: true),
                    EnergyConsumed = table.Column<double>(type: "float", nullable: false),
                    EnergyCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdleStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdleEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdleFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricingName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PricePerKwhSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipt_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receipt_ChargingSessions_ChargingSessionId",
                        column: x => x.ChargingSessionId,
                        principalTable: "ChargingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Receipt_DriverPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "DriverPackages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FixedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FixedNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Report_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Report_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_ChargingPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "ChargingPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1", 0, "F2A4C7B8-9E1D-5B6C-8F7A-6D5E4B3C2A1F", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@evsystem.com", true, "System Administrator", false, null, "ADMIN@EVSYSTEM.COM", "ADMIN", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0900000000", true, "E1F3B6A7-8D9C-4A5B-9E8F-7C6D5B4A3E2D", false, "admin" });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "CloseTime", "Code", "Description", "Latitude", "Longitude", "Name", "OpenTime", "Status" },
                values: new object[,]
                {
                    { 1, "12 Lê Lợi, Quận 1, TP.HCM", new TimeSpan(0, 22, 0, 0, 0), "Q1HCM", "Trạm sạc trung tâm TP.HCM, hỗ trợ cả AC và DC", 10.776899999999999, 106.7009, "Trạm sạc VinFast Quận 1", new TimeSpan(0, 6, 0, 0, 0), "Active" },
                    { 2, "35 Võ Văn Ngân, TP. Thủ Đức, TP.HCM", new TimeSpan(0, 22, 0, 0, 0), "TDHCM", "Trạm sạc khu vực Thủ Đức, gần Vincom", 10.849500000000001, 106.7689, "Trạm sạc VinFast Thủ Đức", new TimeSpan(0, 6, 0, 0, 0), "Active" },
                    { 3, "88 Đại Lộ Bình Dương, Thuận An, Bình Dương", new TimeSpan(0, 22, 0, 0, 0), "BDBD", "Trạm sạc khu vực Bình Dương, thuận tiện cho xe di chuyển xa", 10.949999999999999, 106.75, "Trạm sạc VinFast Bình Dương", new TimeSpan(0, 6, 0, 0, 0), "Active" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1" });

            migrationBuilder.InsertData(
                table: "ChargingPosts",
                columns: new[] { "Id", "Code", "ConnectorType", "IsWalkIn", "PowerKW", "QRCode", "StationId", "Status", "Type" },
                values: new object[,]
                {
                    { 1, "Q1-Type2-A", "Type2", false, 11m, null, 1, "Available", "Normal" },
                    { 2, "Q1-Type2-B", "Type2", true, 11m, null, 1, "Available", "Normal" },
                    { 3, "Q1-CCS2-A", "CCS2", false, 60m, null, 1, "Available", "Fast" },
                    { 4, "Q1-CCS2-B", "CCS2", true, 60m, null, 1, "Available", "Fast" },
                    { 5, "Q1-SC-A", "VinEScooter", false, 1.2m, null, 1, "Available", "Scooter" },
                    { 6, "Q1-SC-B", "VinEScooter", true, 1.2m, null, 1, "Available", "Scooter" },
                    { 7, "TD-Type2-A", "Type2", false, 11m, null, 2, "Available", "Normal" },
                    { 8, "TD-Type2-B", "Type2", true, 11m, null, 2, "Available", "Normal" },
                    { 9, "TD-CCS2-A", "CCS2", false, 60m, null, 2, "Available", "Fast" },
                    { 10, "TD-CCS2-B", "CCS2", true, 60m, null, 2, "Available", "Fast" },
                    { 11, "TD-SC-A", "VinEScooter", false, 1.2m, null, 2, "Available", "Scooter" },
                    { 12, "TD-SC-B", "VinEScooter", true, 1.2m, null, 2, "Available", "Scooter" },
                    { 13, "BD-Type2-A", "Type2", false, 11m, null, 3, "Available", "Normal" },
                    { 14, "BD-Type2-B", "Type2", true, 11m, null, 3, "Available", "Normal" },
                    { 15, "BD-CCS2-A", "CCS2", false, 60m, null, 3, "Available", "Fast" },
                    { 16, "BD-CCS2-B", "CCS2", true, 60m, null, 3, "Available", "Fast" },
                    { 17, "BD-SC-A", "VinEScooter", false, 1.2m, null, 3, "Available", "Scooter" },
                    { 18, "BD-SC-B", "VinEScooter", true, 1.2m, null, 3, "Available", "Scooter" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ReceiptId",
                table: "WalletTransactions",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_StaffId",
                table: "Assignment",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_StationId",
                table: "Assignment",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_AppUserId",
                table: "Receipt",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_ChargingSessionId",
                table: "Receipt",
                column: "ChargingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_PackageId",
                table: "Receipt",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_CreatedById",
                table: "Report",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Report_PostId",
                table: "Report",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_TechnicianId",
                table: "Report",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSessions_ChargingPosts_ChargingPostId",
                table: "ChargingSessions",
                column: "ChargingPostId",
                principalTable: "ChargingPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Receipt_ReceiptId",
                table: "WalletTransactions",
                column: "ReceiptId",
                principalTable: "Receipt",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargingSessions_ChargingPosts_ChargingPostId",
                table: "ChargingSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Receipt_ReceiptId",
                table: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "Assignment");

            migrationBuilder.DropTable(
                name: "Receipt");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ReceiptId",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pricings",
                table: "Pricings");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1" });

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
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "Dept",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "IsDept",
                table: "Wallets");

            migrationBuilder.RenameTable(
                name: "Pricings",
                newName: "Pricing");

            migrationBuilder.RenameColumn(
                name: "ChargingPostId",
                table: "ChargingSessions",
                newName: "PostId");

            migrationBuilder.RenameIndex(
                name: "IX_ChargingSessions_ChargingPostId",
                table: "ChargingSessions",
                newName: "IX_ChargingSessions_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pricing",
                table: "Pricing",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingSessions_ChargingPosts_PostId",
                table: "ChargingSessions",
                column: "PostId",
                principalTable: "ChargingPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
