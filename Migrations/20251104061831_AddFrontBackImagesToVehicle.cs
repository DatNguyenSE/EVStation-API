using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddFrontBackImagesToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VehicleRegistrationImageUrl",
                table: "Vehicles",
                newName: "VehicleRegistrationFrontUrl");

            migrationBuilder.AddColumn<string>(
                name: "VehicleRegistrationBackUrl",
                table: "Vehicles",
                type: "nvarchar(500)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleRegistrationBackUrl",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "VehicleRegistrationFrontUrl",
                table: "Vehicles",
                newName: "VehicleRegistrationImageUrl");
        }
    }
}
