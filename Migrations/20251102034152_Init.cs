using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargingPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pricings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerKwh = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerMinute = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatteryCapacityKWh = table.Column<double>(type: "float", nullable: false),
                    HasDualBattery = table.Column<bool>(type: "bit", nullable: false),
                    MaxChargingPowerKW = table.Column<double>(type: "float", nullable: true),
                    MaxChargingPowerAC_KW = table.Column<double>(type: "float", nullable: true),
                    MaxChargingPowerDC_KW = table.Column<double>(type: "float", nullable: true),
                    ConnectorType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatteryCapacityKWh = table.Column<double>(type: "float", nullable: false),
                    MaxChargingPowerKW = table.Column<double>(type: "float", nullable: false),
                    ConnectorType = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Plate = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    VehicleRegistrationImageUrl = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    RegistrationStatus = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Dept = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDept = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverPackages_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverPackages_ChargingPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "ChargingPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StaffId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_AspNetUsers_StaffId",
                        column: x => x.StaffId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignments_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargingPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    StationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    PowerKW = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    ConnectorType = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    IsWalkIn = table.Column<bool>(type: "bit", nullable: false),
                    QRCode = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargingPosts_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 15, nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PackageId = table.Column<int>(type: "int", nullable: true),
                    EnergyConsumed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnergyCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdleStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdleEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdleFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverstayFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricingName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PricePerKwhSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConfirmedByStaffId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipts_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receipts_AspNetUsers_ConfirmedByStaffId",
                        column: x => x.ConfirmedByStaffId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receipts_DriverPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "DriverPackages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaintenanceStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FixedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FixedNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_ChargingPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "ChargingPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    ChargingPostId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeSlotStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeSlotEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_ChargingPosts_ChargingPostId",
                        column: x => x.ChargingPostId,
                        principalTable: "ChargingPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    BalanceBefore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VnpTxnRef = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiptId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargingSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: true),
                    ChargingPostId = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: true),
                    ReceiptId = table.Column<int>(type: "int", nullable: true),
                    VehiclePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartBatteryPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EndBatteryPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EnergyConsumed = table.Column<double>(type: "float", nullable: false),
                    Cost = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    IdleFeeStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdleFee = table.Column<int>(type: "int", nullable: false),
                    StopReason = table.Column<int>(type: "int", nullable: true),
                    IsWalkInSession = table.Column<bool>(type: "bit", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    IsOverstay = table.Column<bool>(type: "bit", nullable: false),
                    OverstayFee = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargingSessions_ChargingPosts_ChargingPostId",
                        column: x => x.ChargingPostId,
                        principalTable: "ChargingPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargingSessions_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChargingSessions_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChargingSessions_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "Driver", "DRIVER" },
                    { "3", null, "Manager", "MANAGER" },
                    { "4", null, "Operator", "OPERATOR" },
                    { "5", null, "Technician", "TECHNICIAN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "F2A4C7B8-9E1D-5B6C-8F7A-6D5E4B3C2A1F", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@evsystem.com", true, "System Administrator", false, null, "ADMIN@EVSYSTEM.COM", "ADMIN", "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==", "0900000000", true, "E1F3B6A7-8D9C-4A5B-9E8F-7C6D5B4A3E2D", false, "admin" },
                    { "2", 0, "11111111-2222-3333-4444-555555555555", new DateTime(1995, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "operator@evsystem.com", true, "Trạm Operator", false, null, "OPERATOR@EVSYSTEM.COM", "OPERATOR", "AQAAAAIAAYagAAAAEPti/a9dQXrb7L6sjniNdM3QWjQhWtlZLB7tQwUaCxsyewD+D8MBhuXsE4afjntGfg==", "0911111111", true, "A1111111-B222-4333-C444-D55555555555", false, "operator" },
                    { "3", 0, "66666666-7777-8888-9999-AAAAAAAAAAAA", new DateTime(1992, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager@evsystem.com", true, "Khu vực Manager", false, null, "MANAGER@EVSYSTEM.COM", "MANAGER", "AQAAAAIAAYagAAAAENMyFIG2LA4//qtHgDgkZB8TC+wvdKnkwxiD6JHIkMCX0dd+twv8zV7ea/CMfQnChw==", "0922222222", true, "B1111111-C222-4333-D444-E55555555555", false, "manager" },
                    { "4", 0, "BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF", new DateTime(1994, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "technician@evsystem.com", true, "Kỹ thuật viên bảo trì", false, null, "TECHNICIAN@EVSYSTEM.COM", "TECHNICIAN", "AQAAAAIAAYagAAAAEKV4vb55tRNp0q0sO0pF/Ua5A46af0IC1l5PZuNofciWemJVAk7vjQYutf5YQKjxfQ==", "0933333333", true, "C1111111-D222-4333-E444-F55555555555", false, "technician" }
                });

            migrationBuilder.InsertData(
                table: "Pricings",
                columns: new[] { "Id", "EffectiveFrom", "EffectiveTo", "IsActive", "Name", "PricePerKwh", "PricePerMinute", "PriceType" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Khách vãng lai - Sạc thường AC", 4000m, null, "Guest_AC" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Khách vãng lai - Sạc nhanh DC", 4800m, null, "Guest_DC" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Thành viên - Sạc thường AC", 3500m, null, "Member_AC" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Thành viên - Sạc nhanh DC", 4200m, null, "Member_DC" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Phí chiếm dụng", 0m, 1000m, "OccupancyFee" }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "CloseTime", "Code", "Description", "Latitude", "Longitude", "Name", "OpenTime", "Status" },
                values: new object[,]
                {
                    { 1, "12 Lê Lợi, Quận 1, TP.HCM", new TimeSpan(0, 22, 0, 0, 0), "HCM01", "Trạm sạc trung tâm TP.HCM, hỗ trợ cả AC và DC", 10.776899999999999, 106.7009, "Trạm sạc VinFast Quận 1", new TimeSpan(0, 6, 0, 0, 0), "Active" },
                    { 2, "35 Võ Văn Ngân, TP. Thủ Đức, TP.HCM", new TimeSpan(0, 22, 0, 0, 0), "HCM02", "Trạm sạc khu vực Thủ Đức, gần Vincom", 10.849500000000001, 106.7689, "Trạm sạc VinFast Thủ Đức", new TimeSpan(0, 6, 0, 0, 0), "Active" },
                    { 3, "88 Đại Lộ Bình Dương, Thuận An, Bình Dương", new TimeSpan(0, 22, 0, 0, 0), "BD03", "Trạm sạc khu vực Bình Dương, thuận tiện cho xe di chuyển xa", 10.949999999999999, 106.75, "Trạm sạc VinFast Bình Dương", new TimeSpan(0, 6, 0, 0, 0), "Active" }
                });

            migrationBuilder.InsertData(
                table: "VehicleModels",
                columns: new[] { "Id", "BatteryCapacityKWh", "ConnectorType", "HasDualBattery", "MaxChargingPowerAC_KW", "MaxChargingPowerDC_KW", "MaxChargingPowerKW", "Model", "Type" },
                values: new object[,]
                {
                    { 1, 3.5, 2, false, null, null, 1.2, "Theon S", 0 },
                    { 2, 3.5, 2, false, null, null, 1.2, "Vento S", 0 },
                    { 3, 3.5, 2, false, null, null, 1.2, "Vento Neo", 0 },
                    { 4, 3.5, 2, false, null, null, 1.2, "Klara S2 (2022)", 0 },
                    { 5, 2.0, 2, false, null, null, 1.2, "Klara Neo", 0 },
                    { 6, 3.5, 2, false, null, null, 1.2, "Feliz S", 0 },
                    { 7, 2.0, 2, false, null, null, 1.2, "Feliz Neo/Lite", 0 },
                    { 8, 2.3999999999999999, 2, true, null, null, 1.2, "Feliz 2025", 0 },
                    { 9, 3.5, 2, false, null, null, 1.2, "Evo 200/200 Lite", 0 },
                    { 10, 2.3999999999999999, 2, true, null, null, 1.2, "Evo Grand", 0 },
                    { 11, 2.0, 2, false, null, null, 1.2, "Evo Neo/Lite Neo", 0 },
                    { 12, 2.0, 2, false, null, null, 1.2, "Motio", 0 },
                    { 13, 18.640000000000001, 1, false, 7.4000000000000004, 60.0, null, "VF 3", 1 },
                    { 14, 37.229999999999997, 1, false, 7.4000000000000004, 60.0, null, "VF 5 Plus", 1 },
                    { 15, 42.0, 1, false, 7.4000000000000004, 60.0, null, "VF e34", 1 },
                    { 16, 59.600000000000001, 1, false, 11.0, 150.0, null, "VF 6", 1 },
                    { 17, 75.299999999999997, 1, false, 11.0, 150.0, null, "VF 7", 1 },
                    { 18, 87.700000000000003, 1, false, 11.0, 150.0, null, "VF 8", 1 },
                    { 19, 123.0, 1, false, 11.0, 250.0, null, "VF 9", 1 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "1" },
                    { "4", "2" },
                    { "3", "3" },
                    { "5", "4" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_StaffId",
                table: "Assignments",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_StationId",
                table: "Assignments",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingPosts_StationId",
                table: "ChargingPosts",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingSessions_ChargingPostId",
                table: "ChargingSessions",
                column: "ChargingPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingSessions_ReceiptId",
                table: "ChargingSessions",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingSessions_ReservationId",
                table: "ChargingSessions",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingSessions_VehicleId",
                table: "ChargingSessions",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPackages_AppUserId",
                table: "DriverPackages",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPackages_PackageId",
                table: "DriverPackages",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_AppUserId",
                table: "Receipts",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ConfirmedByStaffId",
                table: "Receipts",
                column: "ConfirmedByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_PackageId",
                table: "Receipts",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatedById",
                table: "Reports",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PostId",
                table: "Reports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TechnicianId",
                table: "Reports",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ChargingPostId",
                table: "Reservations",
                column: "ChargingPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_VehicleId",
                table: "Reservations",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_OwnerId",
                table: "Vehicles",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Plate",
                table: "Vehicles",
                column: "Plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ReceiptId",
                table: "WalletTransactions",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "ChargingSessions");

            migrationBuilder.DropTable(
                name: "Pricings");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "VehicleModels");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "ChargingPosts");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "DriverPackages");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ChargingPackages");
        }
    }
}
