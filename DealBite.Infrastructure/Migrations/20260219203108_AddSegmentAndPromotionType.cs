using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealBite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSegmentAndPromotionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Segment",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AppRequired",
                table: "ProductPrices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromotionType",
                table: "ProductPrices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Segment",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AppRequired",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "PromotionType",
                table: "ProductPrices");
        }
    }
}
