using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartSync._Migrations
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
                    ItemId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    ItemName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ItemTemp = table.Column<string>(type: "text", nullable: false),
                    DefaultUnitType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "Preps",
                columns: table => new
                {
                    PrepId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    PrepName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preps", x => x.PrepId);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    RecipeId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    RecipeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    CartAmount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.RecipeId);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    StoreName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                });

            migrationBuilder.CreateTable(
                name: "ItemPreps",
                columns: table => new
                {
                    ItemId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    PrepId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false)
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
                name: "RecipeInstructions",
                columns: table => new
                {
                    RecipeInstructionId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    RecipeId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    RecipeInstructionContent = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    IsImage = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeInstructions", x => x.RecipeInstructionId);
                    table.ForeignKey(
                        name: "FK_RecipeInstructions_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeSections",
                columns: table => new
                {
                    RecipeSectionId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    RecipeId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    RecipeSectionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeSections", x => x.RecipeSectionId);
                    table.ForeignKey(
                        name: "FK_RecipeSections_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aisles",
                columns: table => new
                {
                    AisleId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    StoreId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    AisleName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
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
                name: "RecipeSectionEntries",
                columns: table => new
                {
                    RecipeSectionEntryId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    RecipeSectionId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    PrepId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: true),
                    Amount = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeSectionEntries", x => x.RecipeSectionEntryId);
                    table.ForeignKey(
                        name: "FK_RecipeSectionEntries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeSectionEntries_Preps_PrepId",
                        column: x => x.PrepId,
                        principalTable: "Preps",
                        principalColumn: "PrepId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeSectionEntries_RecipeSections_RecipeSectionId",
                        column: x => x.RecipeSectionId,
                        principalTable: "RecipeSections",
                        principalColumn: "RecipeSectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAisles",
                columns: table => new
                {
                    ItemId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    StoreId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    AisleId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    Bay = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_RecipeInstructions_RecipeId",
                table: "RecipeInstructions",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSectionEntries_ItemId",
                table: "RecipeSectionEntries",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSectionEntries_PrepId",
                table: "RecipeSectionEntries",
                column: "PrepId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSectionEntries_RecipeSectionId",
                table: "RecipeSectionEntries",
                column: "RecipeSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSections_RecipeId",
                table: "RecipeSections",
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
                name: "RecipeInstructions");

            migrationBuilder.DropTable(
                name: "RecipeSectionEntries");

            migrationBuilder.DropTable(
                name: "Aisles");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Preps");

            migrationBuilder.DropTable(
                name: "RecipeSections");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
