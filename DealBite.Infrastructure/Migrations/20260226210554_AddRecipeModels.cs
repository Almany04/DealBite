using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealBite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Products_ProductId",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeGenerationCacheId",
                table: "Recipes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Servings",
                table: "Recipes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSavings",
                table: "Recipes",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TotalSavingsCurrency",
                table: "Recipes",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "RecipeIngredients",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "IngredientName",
                table: "RecipeIngredients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SavingsAmount",
                table: "RecipeIngredients",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SavingsAmountCurrency",
                table: "RecipeIngredients",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RecipeGenerationCaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mode = table.Column<string>(type: "text", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: true),
                    Segment = table.Column<int>(type: "integer", nullable: false),
                    GeneratedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeGenerationCaches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StepNumber = table.Column<int>(type: "integer", nullable: false),
                    Instruction = table.Column<string>(type: "text", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeSteps_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_RecipeGenerationCacheId",
                table: "Recipes",
                column: "RecipeGenerationCacheId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSteps_RecipeId",
                table: "RecipeSteps",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Products_ProductId",
                table: "RecipeIngredients",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_RecipeGenerationCaches_RecipeGenerationCacheId",
                table: "Recipes",
                column: "RecipeGenerationCacheId",
                principalTable: "RecipeGenerationCaches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Products_ProductId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_RecipeGenerationCaches_RecipeGenerationCacheId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "RecipeGenerationCaches");

            migrationBuilder.DropTable(
                name: "RecipeSteps");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_RecipeGenerationCacheId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeGenerationCacheId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Servings",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "TotalSavings",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "TotalSavingsCurrency",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IngredientName",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "SavingsAmount",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "SavingsAmountCurrency",
                table: "RecipeIngredients");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "RecipeIngredients",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Products_ProductId",
                table: "RecipeIngredients",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
