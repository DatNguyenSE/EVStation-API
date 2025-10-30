using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReceiptAppUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_AspNetUsers_AppUserId",
                table: "Receipts");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId1",
                table: "Receipts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_AppUserId1",
                table: "Receipts",
                column: "AppUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_AspNetUsers_AppUserId",
                table: "Receipts",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_AspNetUsers_AppUserId1",
                table: "Receipts",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_AspNetUsers_AppUserId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_AspNetUsers_AppUserId1",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_AppUserId1",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "Receipts");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_AspNetUsers_AppUserId",
                table: "Receipts",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
