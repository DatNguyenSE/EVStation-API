using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnOfChargingSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VehicleId",
                table: "ChargingSessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<float>(
                name: "EndBatteryPercentage",
                table: "ChargingSessions",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "StartBatteryPercentage",
                table: "ChargingSessions",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehiclePlate",
                table: "ChargingSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndBatteryPercentage",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "StartBatteryPercentage",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "VehiclePlate",
                table: "ChargingSessions");

            migrationBuilder.AlterColumn<int>(
                name: "VehicleId",
                table: "ChargingSessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
