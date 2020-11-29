using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4TradeAPI.Data.Migrations
{
    public partial class usersystemrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSystemRelationship",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSystemRelationship", x => new { x.SystemId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserSystemRelationship_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSystemRelationship_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSystemRelationship_UserId",
                table: "UserSystemRelationship",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSystemRelationship");
        }
    }
}
