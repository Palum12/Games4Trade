using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4Trade.Data.Migrations
{
    public partial class fixmessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Messages");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivered",
                table: "Messages",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelivered",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Messages",
                nullable: false,
                defaultValue: 0);
        }
    }
}
