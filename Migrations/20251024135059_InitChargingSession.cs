using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitChargingSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedTime",
                table: "ChargingSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdleFee",
                table: "ChargingSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "IdleFeeStartTime",
                table: "ChargingSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverstay",
                table: "ChargingSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "ChargingSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWalkInSession",
                table: "ChargingSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OverstayFee",
                table: "ChargingSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StopReason",
                table: "ChargingSessions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedTime",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "IdleFee",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "IdleFeeStartTime",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "IsOverstay",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "IsWalkInSession",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "OverstayFee",
                table: "ChargingSessions");

            migrationBuilder.DropColumn(
                name: "StopReason",
                table: "ChargingSessions");
        }
    }
}
