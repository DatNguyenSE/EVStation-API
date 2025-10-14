namespace API.Data;

using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using API.Entities.Wallet;
using API.Helpers.Enums;

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
    // public DbSet<ChargingSession> ChargingSessions { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }

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
            new IdentityRole {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole {
                Id = "2",
                Name = "Driver",
                NormalizedName = "DRIVER"
            },
            new IdentityRole {
                Id = "3",
                Name = "Manager",
                NormalizedName = "MANAGER"
            },
            new IdentityRole {
                Id = "4",
                Name = "Staff",
                NormalizedName = "STAFF"
            },
            new IdentityRole {
                Id = "5",
                Name = "Technician",
                NormalizedName = "TECHNICIAN"
            },
        };
        builder.Entity<IdentityRole>().HasData(roles);
    }
}