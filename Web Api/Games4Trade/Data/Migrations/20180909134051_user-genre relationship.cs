using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4Trade.Data.Migrations
{
    public partial class usergenrerelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGenreRelationship",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    GenreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGenreRelationship", x => new { x.GenreId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserGenreRelationship_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGenreRelationship_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGenreRelationship_UserId",
                table: "UserGenreRelationship",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGenreRelationship");
        }
    }
}
