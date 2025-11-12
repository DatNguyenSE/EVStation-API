using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class StationIdColumnOnReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StationId",
                table: "Receipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_StationId",
                table: "Receipts",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Stations_StationId",
                table: "Receipts",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Stations_StationId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_StationId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "Receipts");
        }
    }
}
