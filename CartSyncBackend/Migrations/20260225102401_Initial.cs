using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartSyncBackend.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    ItemTemp = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultUnitType = table.Column<string>(type: "TEXT", nullable: false),
                    CartAmount = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "Preps",
                columns: table => new
                {
                    PrepId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    PrepName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preps", x => x.PrepId);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    RecipeId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    RecipeName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    IsPinned = table.Column<bool>(type: "INTEGER", nullable: false),
                    CartAmount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.RecipeId);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    StoreName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                });

            migrationBuilder.CreateTable(
                name: "ItemPreps",
                columns: table => new
                {
                    ItemId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    PrepId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPreps", x => new { x.ItemId, x.PrepId });
                    table.ForeignKey(
                        name: "FK_ItemPreps_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemPreps_Preps_PrepId",
                        column: x => x.PrepId,
                        principalTable: "Preps",
                        principalColumn: "PrepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeSteps",
                columns: table => new
                {
                    RecipeStepId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    RecipeId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    RecipeStepContent = table.Column<string>(type: "TEXT", nullable: false),
                    RecipeStepOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsImage = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeSteps", x => x.RecipeStepId);
                    table.ForeignKey(
                        name: "FK_RecipeSteps_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aisles",
                columns: table => new
                {
                    AisleId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    StoreId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    AisleName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    AisleOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aisles", x => x.AisleId);
                    table.ForeignKey(
                        name: "FK_Aisles_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAisles",
                columns: table => new
                {
                    ItemId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    StoreId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    AisleId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    Bay = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAisles", x => new { x.ItemId, x.StoreId });
                    table.ForeignKey(
                        name: "FK_ItemAisles_Aisles_AisleId",
                        column: x => x.AisleId,
                        principalTable: "Aisles",
                        principalColumn: "AisleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemAisles_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemAisles_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aisles_StoreId",
                table: "Aisles",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAisles_AisleId",
                table: "ItemAisles",
                column: "AisleId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAisles_StoreId",
                table: "ItemAisles",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPreps_PrepId",
                table: "ItemPreps",
                column: "PrepId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSteps_RecipeId",
                table: "RecipeSteps",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAisles");

            migrationBuilder.DropTable(
                name: "ItemPreps");

            migrationBuilder.DropTable(
                name: "RecipeSteps");

            migrationBuilder.DropTable(
                name: "Aisles");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Preps");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
