using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealBite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "storeSlug",
                table: "Stores",
                newName: "StoreSlug");

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "Products",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "StoreSlug",
                table: "Stores",
                newName: "storeSlug");
        }
    }
}
