namespace API.Data;

using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using API.Entities.Wallet;
using API.Helpers.Enums;
using System.Collections.Generic;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<ChargingPost> ChargingPosts { get; set; }
    public DbSet<ChargingPackage> ChargingPackages { get; set; }
    public DbSet<DriverPackage> DriverPackages { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ChargingSession> ChargingSessions { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<Pricing> Pricings { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<Report> Reports { get; set; }

    private static readonly DateTime effectiveDate = new DateTime(2025, 1, 1);
    private static readonly DateTime expiryDate = new DateTime(2099, 12, 31);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Vehicle>()
            .HasIndex(v => v.Plate)
            .IsUnique();

        builder.Entity<Vehicle>()
            .HasOne(v => v.Owner)
            .WithMany(u => u.Vehicles)
            .HasForeignKey(v => v.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Wallet>()
                .HasOne(w => w.appUser)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.UserId);

        builder.Entity<ChargingPost>()
            .Property(p => p.PowerKW)
            .HasPrecision(5, 2);

        builder.Entity<ChargingPackage>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Entity<Pricing>()
        .Property(p => p.PriceType)
        .HasConversion<string>(); // <-- Lưu enum dưới dạng chuỗi

        builder.Entity<Receipt>()
            .HasMany(r => r.ChargingSessions)
            .WithOne(s => s.Receipt)
            .HasForeignKey(s => s.ReceiptId)
            // Nếu Receipt bị xóa thì:
            // .OnDelete(DeleteBehavior.SetNull); // giữ session, but set ReceiptId = null
            // hoặc .OnDelete(DeleteBehavior.Restrict); // không cho xóa Receipt khi có session
            // hoặc .OnDelete(DeleteBehavior.Cascade); // xóa luôn session theo Receipt (cẩn thận!)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Receipt>()
            .HasOne(r => r.AppUser)
            .WithMany(u => u.Receipts)
            .HasForeignKey(r => r.AppUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Pricing>().HasData(
            new Pricing
            {
                Id = 1,
                Name = "Khách vãng lai - Sạc thường AC",
                PriceType = PriceType.Guest_AC,
                PricePerKwh = 4000m, // Giá mỗi kWh
                PricePerMinute = null, // Không áp dụng giá mỗi phút
                EffectiveFrom = effectiveDate,
                EffectiveTo = expiryDate,
                IsActive = true
            },
            new Pricing
            {
                Id = 2,
                Name = "Khách vãng lai - Sạc nhanh DC",
                PriceType = PriceType.Guest_DC,
                PricePerKwh = 4800m, // Giá mỗi kWh
                PricePerMinute = null,
                EffectiveFrom = effectiveDate,
                EffectiveTo = expiryDate,
                IsActive = true
            },
            new Pricing
            {
                Id = 3,
                Name = "Thành viên - Sạc thường AC",
                PriceType = PriceType.Member_AC,
                PricePerKwh = 3500m, // Giá mỗi kWh
                PricePerMinute = null,
                EffectiveFrom = effectiveDate,
                EffectiveTo = expiryDate,
                IsActive = true
            },
            new Pricing
            {
                Id = 4,
                Name = "Thành viên - Sạc nhanh DC",
                PriceType = PriceType.Member_DC,
                PricePerKwh = 4200m, // Giá mỗi kWh
                PricePerMinute = null,
                EffectiveFrom = effectiveDate,
                EffectiveTo = expiryDate,
                IsActive = true
            },
            new Pricing
            {
                Id = 5,
                Name = "Phí chiếm dụng",
                PriceType = PriceType.OccupancyFee,
                PricePerKwh = 0m, // Không tính giá mỗi kWh
                PricePerMinute = 1000m, // Giá mỗi phút
                EffectiveFrom = effectiveDate,
                EffectiveTo = expiryDate,
                IsActive = true
            }
        );

        builder.Entity<VehicleModel>().HasData(
            // 🛵 Motorbikes
            new VehicleModel { Id = 1, Type = VehicleType.Motorbike, Model = "Theon S", BatteryCapacityKWh = 3.5, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 2, Type = VehicleType.Motorbike, Model = "Vento S", BatteryCapacityKWh = 3.5, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 3, Type = VehicleType.Motorbike, Model = "Vento Neo", BatteryCapacityKWh = 3.5, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 4, Type = VehicleType.Motorbike, Model = "Klara S2 (2022)", BatteryCapacityKWh = 3.5, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 5, Type = VehicleType.Motorbike, Model = "Klara Neo", BatteryCapacityKWh = 2.0, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 6, Type = VehicleType.Motorbike, Model = "Feliz S", BatteryCapacityKWh = 3.5, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 7, Type = VehicleType.Motorbike, Model = "Feliz Neo/Lite", BatteryCapacityKWh = 2.0, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 8, Type = VehicleType.Motorbike, Model = "Feliz 2025", BatteryCapacityKWh = 2.4, HasDualBattery = true, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 9, Type = VehicleType.Motorbike, Model = "Evo 200/200 Lite", BatteryCapacityKWh = 3.5, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 10, Type = VehicleType.Motorbike, Model = "Evo Grand", BatteryCapacityKWh = 2.4, HasDualBattery = true, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 11, Type = VehicleType.Motorbike, Model = "Evo Neo/Lite Neo", BatteryCapacityKWh = 2.0, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },
            new VehicleModel { Id = 12, Type = VehicleType.Motorbike, Model = "Motio", BatteryCapacityKWh = 2.0, HasDualBattery = false, MaxChargingPowerKW = 1.2, ConnectorType = ConnectorType.VinEScooter },

            // 🚗 Cars
            new VehicleModel { Id = 13, Type = VehicleType.Car, Model = "VF 3", BatteryCapacityKWh = 18.64, HasDualBattery = false, MaxChargingPowerAC_KW = 7.4, MaxChargingPowerDC_KW = 60, ConnectorType = ConnectorType.CCS2 },
            new VehicleModel { Id = 14, Type = VehicleType.Car, Model = "VF 5 Plus", BatteryCapacityKWh = 37.23, HasDualBattery = false, MaxChargingPowerAC_KW = 7.4, MaxChargingPowerDC_KW = 60, ConnectorType = ConnectorType.CCS2 },
            new VehicleModel { Id = 15, Type = VehicleType.Car, Model = "VF e34", BatteryCapacityKWh = 42, HasDualBattery = false, MaxChargingPowerAC_KW = 7.4, MaxChargingPowerDC_KW = 60, ConnectorType = ConnectorType.CCS2 },
            new VehicleModel { Id = 16, Type = VehicleType.Car, Model = "VF 6", BatteryCapacityKWh = 59.6, HasDualBattery = false, MaxChargingPowerAC_KW = 11, MaxChargingPowerDC_KW = 150, ConnectorType = ConnectorType.CCS2 },
            new VehicleModel { Id = 17, Type = VehicleType.Car, Model = "VF 7", BatteryCapacityKWh = 75.3, HasDualBattery = false, MaxChargingPowerAC_KW = 11, MaxChargingPowerDC_KW = 150, ConnectorType = ConnectorType.CCS2 },
            new VehicleModel { Id = 18, Type = VehicleType.Car, Model = "VF 8", BatteryCapacityKWh = 87.7, HasDualBattery = false, MaxChargingPowerAC_KW = 11, MaxChargingPowerDC_KW = 150, ConnectorType = ConnectorType.CCS2 },
            new VehicleModel { Id = 19, Type = VehicleType.Car, Model = "VF 9", BatteryCapacityKWh = 123, HasDualBattery = false, MaxChargingPowerAC_KW = 11, MaxChargingPowerDC_KW = 250, ConnectorType = ConnectorType.CCS2 }
        );

        // Seed Roles
        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole {Id = "1", Name = "Admin", NormalizedName = "ADMIN"},
            new IdentityRole {Id = "2", Name = "Driver", NormalizedName = "DRIVER"},
            new IdentityRole {Id = "3", Name = "Manager", NormalizedName = "MANAGER"},
            new IdentityRole {Id = "4", Name = "Operator", NormalizedName = "OPERATOR"},
            new IdentityRole {Id = "5", Name = "Technician", NormalizedName = "TECHNICIAN"},
        };
        builder.Entity<IdentityRole>().HasData(roles);

        var hasher = new PasswordHasher<AppUser>();
        builder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = "1",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@evsystem.com",
                NormalizedEmail = "ADMIN@EVSYSTEM.COM",
                EmailConfirmed = true,
                FullName = "System Administrator",
                DateOfBirth = new DateTime(1990, 1, 1),
                PasswordHash = "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==",
                SecurityStamp = "E1F3B6A7-8D9C-4A5B-9E8F-7C6D5B4A3E2D",
                ConcurrencyStamp = "F2A4C7B8-9E1D-5B6C-8F7A-6D5E4B3C2A1F",
                PhoneNumber = "0900000000",
                PhoneNumberConfirmed = true,
                Vehicles = new List<Vehicle>()
            },
            new AppUser
            {
                Id = "2",
                UserName = "operator",
                NormalizedUserName = "OPERATOR",
                Email = "operator@evsystem.com",
                NormalizedEmail = "OPERATOR@EVSYSTEM.COM",
                EmailConfirmed = true,
                FullName = "Trạm Operator",
                DateOfBirth = new DateTime(1995, 5, 5),
                PasswordHash = "AQAAAAIAAYagAAAAEPti/a9dQXrb7L6sjniNdM3QWjQhWtlZLB7tQwUaCxsyewD+D8MBhuXsE4afjntGfg==",
                SecurityStamp = "A1111111-B222-4333-C444-D55555555555",
                ConcurrencyStamp = "11111111-2222-3333-4444-555555555555",
                PhoneNumber = "0911111111",
                PhoneNumberConfirmed = true
            },
            new AppUser
            {
                Id = "3",
                UserName = "manager",
                NormalizedUserName = "MANAGER",
                Email = "manager@evsystem.com",
                NormalizedEmail = "MANAGER@EVSYSTEM.COM",
                EmailConfirmed = true,
                FullName = "Khu vực Manager",
                DateOfBirth = new DateTime(1992, 3, 3),
                PasswordHash = "AQAAAAIAAYagAAAAENMyFIG2LA4//qtHgDgkZB8TC+wvdKnkwxiD6JHIkMCX0dd+twv8zV7ea/CMfQnChw==",
                SecurityStamp = "B1111111-C222-4333-D444-E55555555555",
                ConcurrencyStamp = "66666666-7777-8888-9999-AAAAAAAAAAAA",
                PhoneNumber = "0922222222",
                PhoneNumberConfirmed = true
            },
            new AppUser
            {
                Id = "4",
                UserName = "technician",
                NormalizedUserName = "TECHNICIAN",
                Email = "technician@evsystem.com",
                NormalizedEmail = "TECHNICIAN@EVSYSTEM.COM",
                EmailConfirmed = true,
                FullName = "Kỹ thuật viên bảo trì",
                DateOfBirth = new DateTime(1994, 8, 8),
                PasswordHash = "AQAAAAIAAYagAAAAEKV4vb55tRNp0q0sO0pF/Ua5A46af0IC1l5PZuNofciWemJVAk7vjQYutf5YQKjxfQ==",
                SecurityStamp = "C1111111-D222-4333-E444-F55555555555",
                ConcurrencyStamp = "BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF",
                PhoneNumber = "0933333333",
                PhoneNumberConfirmed = true
            }
        );
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { RoleId = "1", UserId = "1" }, // Admin
            new IdentityUserRole<string> { RoleId = "4", UserId = "2" }, // Staff (Operator)
            new IdentityUserRole<string> { RoleId = "3", UserId = "3" }, // Manager
            new IdentityUserRole<string> { RoleId = "5", UserId = "4" }  // Technician
        );

        builder.Entity<Station>().HasData(
            new Station { Id = 1, Name = "Trạm sạc VinFast Quận 1", Code = "Q1HCM", Address = "12 Lê Lợi, Quận 1, TP.HCM", Latitude = 10.7769, Longitude = 106.7009, Description = "Trạm sạc trung tâm TP.HCM, hỗ trợ cả AC và DC", OpenTime = new TimeSpan(6, 0, 0), CloseTime = new TimeSpan(22, 0, 0), Status = StationStatus.Active },
            new Station { Id = 2, Name = "Trạm sạc VinFast Thủ Đức", Code = "TDHCM", Address = "35 Võ Văn Ngân, TP. Thủ Đức, TP.HCM", Latitude = 10.8495, Longitude = 106.7689, Description = "Trạm sạc khu vực Thủ Đức, gần Vincom", OpenTime = new TimeSpan(6, 0, 0), CloseTime = new TimeSpan(22, 0, 0), Status = StationStatus.Active },
            new Station { Id = 3, Name = "Trạm sạc VinFast Bình Dương", Code = "BDBD", Address = "88 Đại Lộ Bình Dương, Thuận An, Bình Dương", Latitude = 10.9500, Longitude = 106.7500, Description = "Trạm sạc khu vực Bình Dương, thuận tiện cho xe di chuyển xa", OpenTime = new TimeSpan(6, 0, 0), CloseTime = new TimeSpan(22, 0, 0), Status = StationStatus.Active }
        );

        builder.Entity<ChargingPost>().HasData(
            // ==== Trạm 1: Quận 1 ====
            new ChargingPost { Id = 1, StationId = 1, Code = "Q1-Type2-A", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 2, StationId = 1, Code = "Q1-Type2-B", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 3, StationId = 1, Code = "Q1-CCS2-A", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 4, StationId = 1, Code = "Q1-CCS2-B", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 5, StationId = 1, Code = "Q1-ULTRA-A", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 6, StationId = 1, Code = "Q1-ULTRA-B", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 7, StationId = 1, Code = "Q1-SC-A", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 8, StationId = 1, Code = "Q1-SC-B", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = true },

            // ==== Trạm 2: Thủ Đức ====
            new ChargingPost { Id = 9, StationId = 2, Code = "TD-Type2-A", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 10, StationId = 2, Code = "TD-Type2-B", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 11, StationId = 2, Code = "TD-CCS2-A", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 12, StationId = 2, Code = "TD-CCS2-B", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 13, StationId = 2, Code = "TD-ULTRA-A", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 14, StationId = 2, Code = "TD-ULTRA-B", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 15, StationId = 2, Code = "TD-SC-A", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 16, StationId = 2, Code = "TD-SC-B", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = true },

            // ==== Trạm 3: Bình Dương ====
            new ChargingPost { Id = 17, StationId = 3, Code = "BD-Type2-A", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 18, StationId = 3, Code = "BD-Type2-B", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 19, StationId = 3, Code = "BD-CCS2-A", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 20, StationId = 3, Code = "BD-CCS2-B", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 21, StationId = 3, Code = "BD-ULTRA-A", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 22, StationId = 3, Code = "BD-ULTRA-B", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
            new ChargingPost { Id = 23, StationId = 3, Code = "BD-SC-A", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = false },
            new ChargingPost { Id = 24, StationId = 3, Code = "BD-SC-B", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = true }
        );
    }
}