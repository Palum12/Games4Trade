using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Games4Trade.Data.Migrations
{
    public partial class addedadvertisementitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvertisementItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdvertisementId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: false),
                    StateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementItems_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvertisementItems_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvertisementItems_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementItems_AdvertisementId",
                table: "AdvertisementItems",
                column: "AdvertisementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementItems_StateId",
                table: "AdvertisementItems",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementItems_SystemId",
                table: "AdvertisementItems",
                column: "SystemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementItems");
        }
    }
}
