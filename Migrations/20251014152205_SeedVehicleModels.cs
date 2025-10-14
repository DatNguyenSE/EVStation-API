using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class SeedVehicleModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleModels");
        }
    }
}
