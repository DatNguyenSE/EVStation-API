namespace API.Data;

using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using API.Entities.Wallet;
using API.Helpers.Enums;
using System.Collections.Generic;
using API.Helpers;

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
    public DbSet<Assignment> Assignments { get; set; }

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

        builder.Entity<Station>()
                .Property(s => s.Status)
                .HasConversion<string>()        // ✅ Enum → string
                .HasColumnType("nvarchar(20)");

        builder.Entity<ChargingPost>()
            .Property(p => p.PowerKW)
            .HasPrecision(5, 2);

        builder.Entity<ChargingPost>()
            .Property(p => p.Type)
            .HasConversion<string>()        // ✅ Enum → string
            .HasColumnType("nvarchar(20)");

        builder.Entity<ChargingPost>()
            .Property(p => p.ConnectorType)
            .HasConversion<string>()        // ✅ Enum → string
            .HasColumnType("nvarchar(20)");

        builder.Entity<ChargingPost>()
            .Property(p => p.Status)
            .HasConversion<string>()        // ✅ Enum → string
            .HasColumnType("nvarchar(20)");

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

        builder.Entity<Assignment>().ToTable("Assignments");

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
            },
            new Pricing
            {
                Id = 6,
                Name = "Phí phạt quá giờ đặt chỗ",
                PriceType = PriceType.OverstayFee,
                PricePerKwh = 0m, // Không tính giá mỗi kWh
                PricePerMinute = 2000m, // Giá mỗi phút (Mức giá phạt cao hơn)
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
                PhoneNumberConfirmed = true
            }
        );

        var commonPasswordHash = "AQAAAAIAAYagAAAAEPObFX2yWUOPm4hpjM163Nl64+ipd6Xpz7yGYFOE0vsE1lMTJvMlNk75wZn25hBatA==";
        var newUsers = new List<AppUser>();
        var newRoles = new List<IdentityUserRole<string>>();
        var stamp = "A1B2C3D4-E5F6-7890-1234-567890ABCDEF";
        var currentId = 2;

        // --- 5 MANAGERS ---
        for (int i = 1; i <= 5; i++)
        {
            var id = currentId++.ToString();
            newUsers.Add(new AppUser { Id = id, UserName = $"manager{i}", Email = $"manager{i}@evsystem.com", NormalizedUserName = $"MANAGER{i}", NormalizedEmail = $"MANAGER{i}@EVSYSTEM.COM", EmailConfirmed = true, FullName = $"Lý Quản Lý {i}", DateOfBirth = new DateTime(1995, 1, i), PasswordHash = commonPasswordHash, SecurityStamp = stamp, ConcurrencyStamp = stamp, PhoneNumber = $"099900000{i}", PhoneNumberConfirmed = true });
            newRoles.Add(new IdentityUserRole<string> { UserId = id, RoleId = "3" }); // RoleId 3: Manager
        }

        // --- 5 OPERATORS ---
        for (int i = 1; i <= 5; i++)
        {
            var id = currentId++.ToString();
            newUsers.Add(new AppUser { Id = id, UserName = $"operator{i}", Email = $"operator{i}@evsystem.com", NormalizedUserName = $"OPERATOR{i}", NormalizedEmail = $"OPERATOR{i}@EVSYSTEM.COM", EmailConfirmed = true, FullName = $"Trần Vận Hành {i}", DateOfBirth = new DateTime(1996, 2, i), PasswordHash = commonPasswordHash, SecurityStamp = stamp, ConcurrencyStamp = stamp, PhoneNumber = $"099910000{i}", PhoneNumberConfirmed = true });
            newRoles.Add(new IdentityUserRole<string> { UserId = id, RoleId = "4" }); // RoleId 4: Operator
        }

        // --- 5 TECHNICIANS ---
        for (int i = 1; i <= 5; i++)
        {
            var id = currentId++.ToString();
            newUsers.Add(new AppUser { Id = id, UserName = $"tech{i}", Email = $"tech{i}@evsystem.com", NormalizedUserName = $"TECH{i}", NormalizedEmail = $"TECH{i}@EVSYSTEM.COM", EmailConfirmed = true, FullName = $"Hoàng Kỹ Thuật {i}", DateOfBirth = new DateTime(1997, 3, i), PasswordHash = commonPasswordHash, SecurityStamp = stamp, ConcurrencyStamp = stamp, PhoneNumber = $"099920000{i}", PhoneNumberConfirmed = true });
            newRoles.Add(new IdentityUserRole<string> { UserId = id, RoleId = "5" }); // RoleId 5: Technician
        }

        // --- 10 DRIVERS ---
        for (int i = 1; i <= 10; i++)
        {
            var id = currentId++.ToString();
            newUsers.Add(new AppUser { Id = id, UserName = $"driver{i}", Email = $"driver{i}@evsystem.com", NormalizedUserName = $"DRIVER{i}", NormalizedEmail = $"DRIVER{i}@EVSYSTEM.COM", EmailConfirmed = true, FullName = $"Phạm Tài Xế {i}", DateOfBirth = new DateTime(1998, 4, i), PasswordHash = commonPasswordHash, SecurityStamp = stamp, ConcurrencyStamp = stamp });
            newRoles.Add(new IdentityUserRole<string> { UserId = id, RoleId = "2" }); // RoleId 2: Driver
        }
        builder.Entity<AppUser>().HasData(newUsers);

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { RoleId = "1", UserId = "1" } // Technician
        );
        builder.Entity<IdentityUserRole<string>>().HasData(newRoles);

        builder.Entity<Station>().HasData(
            new Station
            {
                Id = 1,
                Name = "Trạm sạc VinFast Quận 1",
                Code = StationCodeHelper.GenerateStationCode("12 Lê Lợi, Quận 1, TP.HCM", 1),
                Address = "12 Lê Lợi, Quận 1, TP.HCM",
                Latitude = 10.7769,
                Longitude = 106.7009,
                Description = "Trạm sạc trung tâm TP.HCM, hỗ trợ cả AC và DC",
                OpenTime = new TimeSpan(6, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                Status = StationStatus.Active
            },
            new Station
            {
                Id = 2,
                Name = "Trạm sạc VinFast Thủ Đức",
                Code = StationCodeHelper.GenerateStationCode("35 Võ Văn Ngân, TP. Thủ Đức, TP.HCM", 2),
                Address = "35 Võ Văn Ngân, TP. Thủ Đức, TP.HCM",
                Latitude = 10.8495,
                Longitude = 106.7689,
                Description = "Trạm sạc khu vực Thủ Đức, gần Vincom",
                OpenTime = new TimeSpan(6, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                Status = StationStatus.Active
            },
            new Station
            {
                Id = 3,
                Name = "Trạm sạc VinFast Bình Dương",
                Code = StationCodeHelper.GenerateStationCode("88 Đại Lộ Bình Dương, Thuận An, Bình Dương", 3),
                Address = "88 Đại Lộ Bình Dương, Thuận An, Bình Dương",
                Latitude = 10.9500,
                Longitude = 106.7500,
                Description = "Trạm sạc khu vực Bình Dương, thuận tiện cho xe di chuyển xa",
                OpenTime = new TimeSpan(6, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                Status = StationStatus.Active
            },
            new Station
            {
                Id = 4,
                Name = "Trạm sạc VinFast Sân bay",
                Code = StationCodeHelper.GenerateStationCode("Quận Tân Bình, TP.HCM", 4),
                Address = "Sân bay Tân Sơn Nhất, Quận Tân Bình, TP.HCM",
                Latitude = 10.8169828,
                Longitude = 106.6470536,
                Description = "Trạm sạc tại khu vực sân bay, tiện cho xe công nghệ",
                OpenTime = new TimeSpan(6, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                Status = StationStatus.Active
            },
            new Station
            {
                Id = 5,
                Name = "Trạm sạc VinFast Quận 7",
                Code = StationCodeHelper.GenerateStationCode("Quận 7, TP.HCM", 5),
                Address = "172 Nguyễn Văn Linh, Quận 7, TP.HCM",
                Latitude = 10.7519827,
                Longitude = 106.7210539,
                Description = "Trạm sạc khu vực Phú Mỹ Hưng, tập trung nhiều xe ô tô",
                OpenTime = new TimeSpan(6, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                Status = StationStatus.Active
            }
        );

        // var station1Code = StationCodeHelper.GenerateStationCode("12 Lê Lợi, Quận 1, TP.HCM", 1);
        // var station2Code = StationCodeHelper.GenerateStationCode("35 Võ Văn Ngân, TP. Thủ Đức, TP.HCM", 2);
        // var station3Code = StationCodeHelper.GenerateStationCode("88 Đại Lộ Bình Dương, Thuận An, Bình Dương", 3);

        // builder.Entity<ChargingPost>().HasData(
        //     // ==== Trạm 1 ====
        //     new ChargingPost { Id = 1, StationId = 1, Code = $"{station1Code}-CHG001", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 2, StationId = 1, Code = $"{station1Code}-CHG002", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 3, StationId = 1, Code = $"{station1Code}-CHG003", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 4, StationId = 1, Code = $"{station1Code}-CHG004", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 5, StationId = 1, Code = $"{station1Code}-CHG005", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 6, StationId = 1, Code = $"{station1Code}-CHG006", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 7, StationId = 1, Code = $"{station1Code}-CHG007", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 8, StationId = 1, Code = $"{station1Code}-CHG008", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = true },

        //     // ==== Trạm 2 ====
        //     new ChargingPost { Id = 9, StationId = 2, Code = $"{station2Code}-CHG001", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 10, StationId = 2, Code = $"{station2Code}-CHG002", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 11, StationId = 2, Code = $"{station2Code}-CHG003", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 12, StationId = 2, Code = $"{station2Code}-CHG004", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 13, StationId = 2, Code = $"{station2Code}-CHG005", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 14, StationId = 2, Code = $"{station2Code}-CHG006", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 15, StationId = 2, Code = $"{station2Code}-CHG007", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 16, StationId = 2, Code = $"{station2Code}-CHG008", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = true },

        //     // ==== Trạm 3 ====
        //     new ChargingPost { Id = 17, StationId = 3, Code = $"{station3Code}-CHG001", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 18, StationId = 3, Code = $"{station3Code}-CHG002", Type = PostType.Normal, PowerKW = 11, ConnectorType = ConnectorType.Type2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 19, StationId = 3, Code = $"{station3Code}-CHG003", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 20, StationId = 3, Code = $"{station3Code}-CHG004", Type = PostType.Fast, PowerKW = 60, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 21, StationId = 3, Code = $"{station3Code}-CHG005", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 22, StationId = 3, Code = $"{station3Code}-CHG006", Type = PostType.Fast, PowerKW = 150, ConnectorType = ConnectorType.CCS2, Status = PostStatus.Available, IsWalkIn = true },
        //     new ChargingPost { Id = 23, StationId = 3, Code = $"{station3Code}-CHG007", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = false },
        //     new ChargingPost { Id = 24, StationId = 3, Code = $"{station3Code}-CHG008", Type = PostType.Scooter, PowerKW = 1.2m, ConnectorType = ConnectorType.VinEScooter, Status = PostStatus.Available, IsWalkIn = true }
        // );

        builder.Entity<ChargingPackage>().HasData(
            new ChargingPackage
            {
                Id = 1,
                Name = "Gói Xe Máy 30 Ngày Không Giới Hạn",
                Description = "Sử dụng trạm sạc không giới hạn trong 30 ngày cho xe máy điện.",
                VehicleType = VehicleType.Motorbike,
                Price = 99000,
                DurationDays = 30,
                IsActive = true,
                CreatedAt = effectiveDate
            },
            new ChargingPackage
            {
                Id = 2,
                Name = "Gói Ô Tô 30 Ngày Không Giới Hạn",
                Description = "Sử dụng trạm sạc không giới hạn trong 30 ngày cho ô tô điện.",
                VehicleType = VehicleType.Car,
                Price = 499000,
                DurationDays = 30,
                IsActive = true,
                CreatedAt = effectiveDate
            }
        );

        // ====================================================================
        // C. GÁN VỊ TRÍ (ASSIGNMENTS)
        // ====================================================================

        var newAssignments = new List<Assignment>();
        var effectiveFrom = new DateTime(2025, 1, 1);
        var effectiveTo = new DateTime(2026, 2, 1);

        for (int stationId = 1; stationId <= 5; stationId++)
        {
            // Gán Manager (ID 2 -> 6)
            newAssignments.Add(new Assignment { Id = newAssignments.Count + 1, EffectiveFrom = effectiveFrom, EffectiveTo = effectiveTo, IsActive = true, StaffId = (2 + (stationId - 1)).ToString(), StationId = stationId });
            // Gán Operator (ID 7 -> 11)
            newAssignments.Add(new Assignment { Id = newAssignments.Count + 1, EffectiveFrom = effectiveFrom, EffectiveTo = effectiveTo, IsActive = true, StaffId = (7 + (stationId - 1)).ToString(), StationId = stationId });
            // Gán Technician (ID 12 -> 16)
            newAssignments.Add(new Assignment { Id = newAssignments.Count + 1, EffectiveFrom = effectiveFrom, EffectiveTo = effectiveTo, IsActive = true, StaffId = (12 + (stationId - 1)).ToString(), StationId = stationId });
        }

        builder.Entity<Assignment>().HasData(newAssignments);

        // ====================================================================
        // D. VÍ TIỀN (WALLETS)
        // ====================================================================

        var newWallets = new List<Wallet>();
        var driverUserIds = newUsers.Where(u => u.Email!.Contains("@evsystem.com") && u.UserName!.StartsWith("driver")).Select(u => u.Id).ToList();

        newWallets.Add(new Wallet { Id = 1, UserId = driverUserIds[0], Balance = 500000m, Dept = 0m, IsDept = false });  // Driver 3: Có tiền
        newWallets.Add(new Wallet { Id = 2, UserId = driverUserIds[1], Balance = 100000m, Dept = 0m, IsDept = false });  // Driver 4: Có tiền
        newWallets.Add(new Wallet { Id = 3, UserId = driverUserIds[2], Balance = 0m, Dept = 25000m, IsDept = true });   // Driver 5: Nợ
        newWallets.Add(new Wallet { Id = 4, UserId = driverUserIds[3], Balance = 200000m, Dept = 0m, IsDept = false }); // Driver 6: Có tiền
        newWallets.Add(new Wallet { Id = 5, UserId = driverUserIds[4], Balance = 0m, Dept = 0m, IsDept = false });   // Driver 7: Vừa đủ
        newWallets.Add(new Wallet { Id = 6, UserId = driverUserIds[5], Balance = 150000m, Dept = 0m, IsDept = false }); // Driver 8
        newWallets.Add(new Wallet { Id = 7, UserId = driverUserIds[6], Balance = 50000m, Dept = 0m, IsDept = false });  // Driver 9
        newWallets.Add(new Wallet { Id = 8, UserId = driverUserIds[7], Balance = 0m, Dept = 10000m, IsDept = true });  // Driver 10: Nợ
        newWallets.Add(new Wallet { Id = 9, UserId = driverUserIds[8], Balance = 300000m, Dept = 0m, IsDept = false }); // Driver 11
        newWallets.Add(new Wallet { Id = 10, UserId = driverUserIds[9], Balance = 50000m, Dept = 0m, IsDept = false });  // Driver 12

        builder.Entity<Wallet>().HasData(newWallets);
    }
}