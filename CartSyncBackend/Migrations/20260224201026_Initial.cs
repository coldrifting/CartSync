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
                name: "ItemPrep",
                columns: table => new
                {
                    ItemsItemId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false),
                    PrepsPrepId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 26, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPrep", x => new { x.ItemsItemId, x.PrepsPrepId });
                    table.ForeignKey(
                        name: "FK_ItemPrep_Items_ItemsItemId",
                        column: x => x.ItemsItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemPrep_Preps_PrepsPrepId",
                        column: x => x.PrepsPrepId,
                        principalTable: "Preps",
                        principalColumn: "PrepId",
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
                name: "IX_ItemPrep_PrepsPrepId",
                table: "ItemPrep",
                column: "PrepsPrepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAisles");

            migrationBuilder.DropTable(
                name: "ItemPrep");

            migrationBuilder.DropTable(
                name: "Aisles");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Preps");

            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
