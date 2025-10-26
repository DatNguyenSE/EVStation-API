using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updateReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledTime",
                table: "Reports",
                newName: "MaintenanceStartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceEndTime",
                table: "Reports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceEndTime",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "MaintenanceStartTime",
                table: "Reports",
                newName: "ScheduledTime");
        }
    }
}
