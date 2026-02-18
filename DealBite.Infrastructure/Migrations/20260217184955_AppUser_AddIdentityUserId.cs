using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealBite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AppUser_AddIdentityUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "AppUsers");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "AppUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_IdentityUserId",
                table: "AppUsers",
                column: "IdentityUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppUsers_IdentityUserId",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "AppUsers");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "AppUsers",
                type: "text",
                nullable: true);
        }
    }
}
