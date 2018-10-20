using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4Trade.Data.Migrations
{
    public partial class deleteunnecessaryfieldsfromad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowUserEmail",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "ShowUserPhoneNumber",
                table: "Advertisements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowUserEmail",
                table: "Advertisements",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowUserPhoneNumber",
                table: "Advertisements",
                nullable: false,
                defaultValue: false);
        }
    }
}
