using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RedesignAssignmentAsRoster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftEnd",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "ShiftStart",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "ShiftDate",
                table: "Assignments",
                newName: "EffectiveFrom");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "Assignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "EffectiveFrom",
                table: "Assignments",
                newName: "ShiftDate");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ShiftEnd",
                table: "Assignments",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ShiftStart",
                table: "Assignments",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
