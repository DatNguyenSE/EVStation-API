using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RenameAssignmentTableToAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_AspNetUsers_StaffId",
                table: "Assignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Stations_StationId",
                table: "Assignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assignment",
                table: "Assignment");

            migrationBuilder.RenameTable(
                name: "Assignment",
                newName: "Assignments");

            migrationBuilder.RenameIndex(
                name: "IX_Assignment_StationId",
                table: "Assignments",
                newName: "IX_Assignments_StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignment_StaffId",
                table: "Assignments",
                newName: "IX_Assignments_StaffId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationTime",
                table: "Receipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmedByStaffId",
                table: "Receipts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Receipts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assignments",
                table: "Assignments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ConfirmedByStaffId",
                table: "Receipts",
                column: "ConfirmedByStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_AspNetUsers_StaffId",
                table: "Assignments",
                column: "StaffId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Stations_StationId",
                table: "Assignments",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_AspNetUsers_ConfirmedByStaffId",
                table: "Receipts",
                column: "ConfirmedByStaffId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AspNetUsers_StaffId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Stations_StationId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_AspNetUsers_ConfirmedByStaffId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_ConfirmedByStaffId",
                table: "Receipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assignments",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "ConfirmationTime",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ConfirmedByStaffId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Receipts");

            migrationBuilder.RenameTable(
                name: "Assignments",
                newName: "Assignment");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_StationId",
                table: "Assignment",
                newName: "IX_Assignment_StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_StaffId",
                table: "Assignment",
                newName: "IX_Assignment_StaffId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assignment",
                table: "Assignment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_AspNetUsers_StaffId",
                table: "Assignment",
                column: "StaffId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Stations_StationId",
                table: "Assignment",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
