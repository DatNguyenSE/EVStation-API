using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class seedAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUsers_CreatedById",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUsers_TechnicianId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_ChargingPosts_PostId",
                table: "Report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Report",
                table: "Report");

            migrationBuilder.RenameTable(
                name: "Report",
                newName: "Reports");

            migrationBuilder.RenameIndex(
                name: "IX_Report_TechnicianId",
                table: "Reports",
                newName: "IX_Reports_TechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_PostId",
                table: "Reports",
                newName: "IX_Reports_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_CreatedById",
                table: "Reports",
                newName: "IX_Reports_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Vehicles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledTime",
                table: "Reports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Operator", "OPERATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "2", 0, "11111111-2222-3333-4444-555555555555", new DateTime(1995, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "operator@evsystem.com", true, "Trạm Operator", false, null, "OPERATOR@EVSYSTEM.COM", "OPERATOR", "AQAAAAIAAYagAAAAEPti/a9dQXrb7L6sjniNdM3QWjQhWtlZLB7tQwUaCxsyewD+D8MBhuXsE4afjntGfg==", "0911111111", true, "A1111111-B222-4333-C444-D55555555555", false, "operator" },
                    { "3", 0, "66666666-7777-8888-9999-AAAAAAAAAAAA", new DateTime(1992, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager@evsystem.com", true, "Khu vực Manager", false, null, "MANAGER@EVSYSTEM.COM", "MANAGER", "AQAAAAIAAYagAAAAENMyFIG2LA4//qtHgDgkZB8TC+wvdKnkwxiD6JHIkMCX0dd+twv8zV7ea/CMfQnChw==", "0922222222", true, "B1111111-C222-4333-D444-E55555555555", false, "manager" },
                    { "4", 0, "BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF", new DateTime(1994, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "technician@evsystem.com", true, "Kỹ thuật viên bảo trì", false, null, "TECHNICIAN@EVSYSTEM.COM", "TECHNICIAN", "AQAAAAIAAYagAAAAEKV4vb55tRNp0q0sO0pF/Ua5A46af0IC1l5PZuNofciWemJVAk7vjQYutf5YQKjxfQ==", "0933333333", true, "C1111111-D222-4333-E444-F55555555555", false, "technician" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "4", "2" },
                    { "3", "3" },
                    { "5", "4" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_CreatedById",
                table: "Reports",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_TechnicianId",
                table: "Reports",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ChargingPosts_PostId",
                table: "Reports",
                column: "PostId",
                principalTable: "ChargingPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_CreatedById",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_TechnicianId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ChargingPosts_PostId",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4", "2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3", "3" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5", "4" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                table: "Reports");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "Report");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_TechnicianId",
                table: "Report",
                newName: "IX_Report_TechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_PostId",
                table: "Report",
                newName: "IX_Report_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_CreatedById",
                table: "Report",
                newName: "IX_Report_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Vehicles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Report",
                table: "Report",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Staff", "STAFF" });

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_CreatedById",
                table: "Report",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_TechnicianId",
                table: "Report",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_ChargingPosts_PostId",
                table: "Report",
                column: "PostId",
                principalTable: "ChargingPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
