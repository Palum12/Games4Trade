using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4TradeAPI.Data.Migrations
{
    public partial class deleteduselessarchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasReciverArchived",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "HasSenderArchived",
                table: "Messages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasReciverArchived",
                table: "Messages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSenderArchived",
                table: "Messages",
                nullable: false,
                defaultValue: false);
        }
    }
}
