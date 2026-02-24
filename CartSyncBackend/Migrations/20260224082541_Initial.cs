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
                    ItemId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    ItemTemp = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultUnitType = table.Column<int>(type: "INTEGER", nullable: false),
                    CartAmount = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    StoreName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                });

            migrationBuilder.CreateTable(
                name: "Aisles",
                columns: table => new
                {
                    AisleId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    StoreId = table.Column<byte[]>(type: "BLOB", nullable: false),
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
                    ItemId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    StoreId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    AisleId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Bay = table.Column<int>(type: "INTEGER", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAisles");

            migrationBuilder.DropTable(
                name: "Aisles");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
