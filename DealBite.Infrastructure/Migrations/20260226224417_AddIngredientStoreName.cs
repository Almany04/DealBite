using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealBite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientStoreName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreName",
                table: "RecipeIngredients",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreName",
                table: "RecipeIngredients");
        }
    }
}
