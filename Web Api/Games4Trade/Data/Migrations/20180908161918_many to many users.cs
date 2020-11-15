using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4TradeAPI.Data.Migrations
{
    public partial class manytomanyusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObservedUsersRelationship",
                columns: table => new
                {
                    ObservingUserId = table.Column<int>(nullable: false),
                    ObservedUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservedUsersRelationship", x => new { x.ObservingUserId, x.ObservedUserId });
                    table.ForeignKey(
                        name: "FK_ObservedUsersRelationship_Users_ObservedUserId",
                        column: x => x.ObservedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObservedUsersRelationship_Users_ObservingUserId",
                        column: x => x.ObservingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservedUsersRelationship_ObservedUserId",
                table: "ObservedUsersRelationship",
                column: "ObservedUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObservedUsersRelationship");
        }
    }
}
