using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartSync.Database.Migrations
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
                    Temp = table.Column<string>(type: "text", nullable: false),
                    DefaultUnitType = table.Column<string>(type: "text", nullable: false),
                    UncapCartUnits = table.Column<bool>(type: "boolean", nullable: false)
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
                    CartQuantity = table.Column<int>(type: "integer", nullable: false)
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
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    Username = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CartSelectItems",
                columns: table => new
                {
                    CartSelectItemId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    ItemId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    PrepId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: true),
                    Amounts = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartSelectItems", x => x.CartSelectItemId);
                    table.ForeignKey(
                        name: "FK_CartSelectItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartSelectItems_Preps_PrepId",
                        column: x => x.PrepId,
                        principalTable: "Preps",
                        principalColumn: "PrepId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "RecipeSteps",
                columns: table => new
                {
                    RecipeStepId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    RecipeId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    RecipeStepContent = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    IsImage = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
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
                name: "UserInfo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    StoreId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    CartLastGeneratedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CartSelectionLastUpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserInfo_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInfo_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeEntries",
                columns: table => new
                {
                    RecipeEntryId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    RecipeSectionId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    ItemId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    PrepId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: true),
                    Amount = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeEntries", x => x.RecipeEntryId);
                    table.ForeignKey(
                        name: "FK_RecipeEntries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeEntries_Preps_PrepId",
                        column: x => x.PrepId,
                        principalTable: "Preps",
                        principalColumn: "PrepId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeEntries_RecipeSections_RecipeSectionId",
                        column: x => x.RecipeSectionId,
                        principalTable: "RecipeSections",
                        principalColumn: "RecipeSectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartEntries",
                columns: table => new
                {
                    CartEntryId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    ItemId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: false),
                    PrepId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: true),
                    Amounts = table.Column<string>(type: "text", nullable: false),
                    AisleId = table.Column<string>(type: "character varying(26)", unicode: false, maxLength: 26, nullable: true),
                    Bay = table.Column<string>(type: "text", nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartEntries", x => x.CartEntryId);
                    table.ForeignKey(
                        name: "FK_CartEntries_Aisles_AisleId",
                        column: x => x.AisleId,
                        principalTable: "Aisles",
                        principalColumn: "AisleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartEntries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartEntries_Preps_PrepId",
                        column: x => x.PrepId,
                        principalTable: "Preps",
                        principalColumn: "PrepId",
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
                name: "IX_CartEntries_AisleId",
                table: "CartEntries",
                column: "AisleId");

            migrationBuilder.CreateIndex(
                name: "IX_CartEntries_ItemId_PrepId",
                table: "CartEntries",
                columns: new[] { "ItemId", "PrepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartEntries_PrepId",
                table: "CartEntries",
                column: "PrepId");

            migrationBuilder.CreateIndex(
                name: "IX_CartSelectItems_ItemId_PrepId",
                table: "CartSelectItems",
                columns: new[] { "ItemId", "PrepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartSelectItems_PrepId",
                table: "CartSelectItems",
                column: "PrepId");

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
                name: "IX_RecipeEntries_ItemId",
                table: "RecipeEntries",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeEntries_PrepId",
                table: "RecipeEntries",
                column: "PrepId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeEntries_RecipeSectionId_ItemId_PrepId",
                table: "RecipeEntries",
                columns: new[] { "RecipeSectionId", "ItemId", "PrepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSections_RecipeId",
                table: "RecipeSections",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSteps_RecipeId",
                table: "RecipeSteps",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_StoreId",
                table: "UserInfo",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartEntries");

            migrationBuilder.DropTable(
                name: "CartSelectItems");

            migrationBuilder.DropTable(
                name: "ItemAisles");

            migrationBuilder.DropTable(
                name: "ItemPreps");

            migrationBuilder.DropTable(
                name: "RecipeEntries");

            migrationBuilder.DropTable(
                name: "RecipeSteps");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropTable(
                name: "Aisles");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Preps");

            migrationBuilder.DropTable(
                name: "RecipeSections");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
